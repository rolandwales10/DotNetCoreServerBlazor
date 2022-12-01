using FarmshareAdmin.Models;
using System.Security.Cryptography;
using mdl = FarmshareAdmin.Models;
using utl = FarmshareAdmin.Utilities;

namespace FarmshareAdmin.Data
{
    public class Status
    {
        private readonly mdl.ACF_FarmshareContext _context;
        private utl.Error error;
        private utl.Logging logging;

        public Status(mdl.ACF_FarmshareContext context)
        {
            _context = context;
            error = new utl.Error(_context);
            //logging = new utl.Logging(_context);
        }

        private List<string> redeemableStatusList = new();
        public List<string> redeemableStatuses()
        {
            if (redeemableStatusList.Count == 0)
                redeemableStatusList =  _context.STATUS_CODES.Where(r => r.REDEEMABLE_STATUS).Select(r => r.STATUS).ToList();
            return redeemableStatusList;
        }
    }
}
