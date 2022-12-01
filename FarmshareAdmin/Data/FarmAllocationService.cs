using FarmshareAdmin.Models;
using FarmshareAdmin.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Cryptography;

using mdl = FarmshareAdmin.Models;
using utl = FarmshareAdmin.Utilities;

namespace FarmshareAdmin.Data
{
    public class FarmAllocationService : IFarmAllocationService
    {
        private UnitOfWork _uow;
        private readonly mdl.ACF_FarmshareContext _context;
        private utl.Logging _logging;
        private SendMail _sMail;

        public FarmAllocationService(mdl.ACF_FarmshareContext context, utl.Logging logging, UnitOfWork uow, SendMail sMail)
        {
            _context = context;
            _logging = logging;
            _uow = uow;
            _sMail = sMail;
        }
        public async Task<Data.Vm_Farm_Allocations> Get()
        {
            Vm_Farm_Allocations fa = new();
            try
            {
                string[] excludedSt = new string[] { "Closed", "Submitted" };
                var statusService = new Data.Status(_context);
                var redeemableStatuses = statusService.redeemableStatuses();
                int previousYear = DateTime.Now.Year - 1;
                fa.ShareValue = Data.Shares.getShareValue(_context, _logging);
                var farms = await _context.FARMS.Where(r => r.YEAR == DateTime.Now.Year && !excludedSt.Contains(r.STATUS))
                    .OrderBy(r => r.FARM_NAME).ToListAsync();
                fa.TotalFarms = 0;
                fa.TotalCurrentAllocations = 0;
                foreach (var farm in farms)
                {
                    fa.TotalFarms += 1;
                    var alloc = new Data.Vm_Farm_Allocation();
                    alloc.FarmId = farm.FARM_ID;
                    alloc.FarmName = farm.FARM_NAME;
                    alloc.Status = farm.STATUS;
                    if (farm.FIRST_YEAR == DateTime.Now.Year)
                        alloc.Note = "New Farm";
                    var fy = _context.FARM_YEARS.Where(r => r.FARM_ID == farm.FARM_ID && r.YEAR >= previousYear).Select(r =>
                        new { r.YEAR, r.SHARES_REQUESTED, r.SHARES_ALLOCATED, r.SHARES_REDEEMED }).ToList();
                    foreach (var item in fy)
                    {
                        if (item.YEAR == previousYear)
                        {
                            alloc.PrevYearSharesRequested = item.SHARES_REQUESTED;
                            alloc.PrevYearSharesAlloc = item.SHARES_ALLOCATED;
                            alloc.PrevYearSharesRedeemed = item.SHARES_REDEEMED;
                        }
                        else if (item.YEAR == DateTime.Now.Year)
                        {
                            alloc.CurYearSharesRequested = item.SHARES_REQUESTED;
                            alloc.CurYearSharesAlloc = item.SHARES_ALLOCATED;
                        }
                    }
                    try
                    {
                        alloc.PrevYearSharesRedeemed = _context.CITIZEN_AGREEMENT_REDEMPTIONS.Where(r => r.FARM_ID == farm.FARM_ID && r.DATE_REDEEMED.Year == previousYear).Sum(r => r.AMOUNT);
                    }
                    catch (Exception)
                    {
                        alloc.PrevYearSharesRedeemed = 0;
                    }
                    alloc.PrevYearSharesRedeemed /= fa.ShareValue;         // convert to shares
                    alloc.PrevYearSharesRedeemed = Math.Round(alloc.PrevYearSharesRedeemed, 2);
                    try
                    {
                        alloc.PrevYearSharesPaid = _context.FARM_PAYMENTS.Where(r => r.FARM_ID == farm.FARM_ID && r.DATE_ENTERED.Year == previousYear
                            && r.PAYMENT_TYPE != "Return").Sum(r => r.AMOUNT);
                    }
                    catch (Exception)
                    {
                        alloc.PrevYearSharesPaid = 0;
                    }
                    alloc.PrevYearSharesPaid /= fa.ShareValue;       // Convert to shares
                    alloc.PrevYearSharesPaid = Math.Round(alloc.PrevYearSharesPaid, 2);
                    try
                    {
                        alloc.CurYearSharesInvoiced = _context.FARM_PAYMENTS.Where
                            (r => r.FARM_ID == farm.FARM_ID && r.DATE_INVOICED != null && r.YEAR == DateTime.Now.Year).Sum(r => r.AMOUNT);
                    }
                    catch (Exception)
                    {
                        alloc.CurYearSharesInvoiced = 0;
                    }
                    alloc.CurYearSharesInvoiced /= fa.ShareValue;         // convert to shares
                    alloc.CurYearSharesInvoiced = Math.Round(alloc.CurYearSharesInvoiced, 2);
                    var TotalSharesAssigned = _context.CITIZEN_AGREEMENT_YEARS.Include(r => r.CITIZEN_AGREEMENT)
                        .Where(r => r.CITIZEN_AGREEMENT.FARM_ID == farm.FARM_ID && r.YEAR == DateTime.Now.Year
                        && redeemableStatuses.Contains(r.STATUS)).Count();
                    alloc.CurYearSharesNotInvoiced = TotalSharesAssigned - alloc.CurYearSharesInvoiced;
                    alloc.SharesNewAlloc = alloc.CurYearSharesAlloc;
                    fa.TotalCurrentAllocations += alloc.CurYearSharesAlloc;
                    fa.items.Add(alloc);
                }
                fa.TotalNewAllocations = fa.TotalCurrentAllocations;
                var fundingAllocations = _context.FIELD_VALUES.Where(r => r.FIELD_ID.StartsWith("FundingAllocation")).ToList();
                fa.FirstFundingAllocation = 0;
                fa.SecondFundingAllocation = 0;
                if (fundingAllocations.Count == 2) { }
                else MessageService.AddErrorMessage(fa.messages, "Error: funding allocation entries missing from the field values table");
                foreach (var alloc in fundingAllocations)
                {
                    if (alloc.FIELD_ID == "FundingAllocation1")
                        fa.FirstFundingAllocation = alloc.FIELD_AMOUNT ?? 0;
                    else if (alloc.FIELD_ID == "FundingAllocation2")
                        fa.SecondFundingAllocation = alloc.FIELD_AMOUNT ?? 0;
                }
            }
            catch (Exception ex)
            {
                _logging.logError("Error detected in " + this.GetType().Name, ex);
                MessageService.AddErrorMessage(fa.messages, ex.Message);
                _logging.logError("alloc", ex);
            }
            return fa;
        }

        public async Task UpdateAsync(Vm_Farm_Allocations fa)
        {
            try
            {
                var fundingAllocations = _context.FIELD_VALUES.Where(r => r.FIELD_ID.StartsWith("FundingAllocation")).ToList();
                foreach (var alloc in fundingAllocations)
                {
                    if (alloc.FIELD_ID == "FundingAllocation1")
                        alloc.FIELD_AMOUNT = fa.FirstFundingAllocation;
                    else if (alloc.FIELD_ID == "FundingAllocation2")
                        alloc.FIELD_AMOUNT = fa.SecondFundingAllocation;
                    _uow._context.Entry(alloc).State = EntityState.Modified;
                }
                foreach (var farmEntry in fa.items)
                {
                    if (farmEntry.SharesAdjustment != 0 || farmEntry.Approved)
                    {
                        var farm = _uow._context.FARMS.Where(r => r.FARM_ID == farmEntry.FarmId).FirstOrDefault();
                        if (farm == null)
                            MessageService.AddErrorMessage(fa.messages, "Error: farm_id " + farmEntry.FarmId + " not found");
                        else
                        {
                            var fmy = _uow._context.FARM_YEARS.Where(r => r.FARM_ID == farmEntry.FarmId && r.YEAR == DateTime.Now.Year).FirstOrDefault();
                            if (fmy == null)
                                MessageService.AddErrorMessage(fa.messages, "Error: farm_id " + farmEntry.FarmId + " not found for the current year in FARM_YEARS");
                            else
                            {
                                if (farmEntry.Approved)
                                {
                                    if (fmy.SHARES_ALLOCATED != farmEntry.SharesNewAlloc)
                                        _sMail.Send(farmEntry.FarmId, fmy.SHARES_ALLOCATED, farmEntry.SharesNewAlloc);
                                    farm.STATUS = "Approved";
                                    fmy.DATE_APPLICATION_APPROVED = DateTime.Now;
                                }
                                fmy.SHARES_ALLOCATED = farmEntry.SharesNewAlloc;
                                _uow._context.Entry(farm).State = EntityState.Modified;
                                _uow._context.Entry(fmy).State = EntityState.Modified;
                            }
                        }
                    }
                }
                if (fa.messages.Count == 0)
                {
                    fa.messages = await _uow.SaveAsync("Allocations");
                }
            }
            catch (Exception ex)
            {
                _logging.logError("share allocations", ex);
                MessageService.AddErrorMessage(fa.messages, ex.Message);
            }
        }
    }
}
