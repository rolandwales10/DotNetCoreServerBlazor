using FarmshareAdmin.Models;
using System.Security.Cryptography;
using mdl = FarmshareAdmin.Models;
using utl = FarmshareAdmin.Utilities;

namespace FarmshareAdmin.Data
{
    public class Shares
    {
        private static int amount = 0;

        public static int getShareValue(mdl.ACF_FarmshareContext context, utl.Logging logging)
        {
            try
            {
                if (amount == 0)        // This static field will remember its value after the first call
                {
                    var amt = (from r in context.FIELD_VALUES where r.FIELD_ID == "ShareValue$" select new { r.FIELD_AMOUNT }).FirstOrDefault();
                    if (amt != null)
                        amount = Convert.ToInt16(amt.FIELD_AMOUNT);
                }
                if (amount == 0)
                {
                    // Error detected in shares.cs: amount is 0.  Setting to -1 to avoid divide by 0
                    amount = -1;
                }
            }
            catch (Exception ex)
            {
                logging.logError("shares.cs", ex);
            }
            return amount;
        }
    }
}
