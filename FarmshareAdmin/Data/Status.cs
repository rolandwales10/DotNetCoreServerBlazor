using FarmshareAdmin.Models;
using System.Security.Cryptography;
using mdl = FarmshareAdmin.Models;
using utl = FarmshareAdmin.Utilities;

namespace FarmshareAdmin.Data
{
    public class Status
    {
        private readonly mdl.ACF_FarmshareContext _context;
        private utl.Logging _logging;

        public Status(mdl.ACF_FarmshareContext context, utl.Logging logging)
        {
            _context = context;
            _logging = logging;
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
