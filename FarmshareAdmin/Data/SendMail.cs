using System.Linq;
using ml = System.Net.Mail;
using mdl = FarmshareAdmin.Models;
using System;
using System.Configuration;
using System.Collections.Generic;
using utl = FarmshareAdmin.Utilities;
using Microsoft.Extensions.Configuration;

namespace FarmshareAdmin.Data
{
    public class SendMail
    {
        private readonly mdl.ACF_FarmshareContext _context;
        private utl.Logging _logging;
        private IConfiguration _config;

        public SendMail(IConfiguration config, mdl.ACF_FarmshareContext context, utl.Logging logging)
        {
            _context = context;
            _logging = logging;
            _config = config;
        }
        public void Send(int farmId, int prevShareCount, int newShareCount)
        {
            var email = _context.USERS.Where(r => r.FARM_ID == farmId).Select(r => r.EMAIL_ADDRESS).FirstOrDefault();
            if (email == null)
                return;
            var farmName = _context.FARMS.Where(r => r.FARM_ID == farmId).Select(r => r.FARM_NAME).FirstOrDefault();
            int changeInShares = newShareCount - prevShareCount;
            var msg = new ml.MailMessage();
            var env = _config.GetValue<string>("Environment");
            //var env = ConfigurationManager.AppSettings["Environment"];
            var subjectTrailer = "";
            if (env == "Prod")
                subjectTrailer = "";
            else subjectTrailer = " (" + env + ")";
            msg.To.Add(email);

            msg.Subject = "Farmshare allocation for " + farmName + subjectTrailer;
            msg.From = new ml.MailAddress("farmshare@maine.gov");

            msg.IsBodyHtml = true;
            msg.Body = "<h1>" + msg.Subject + "</h1>"
                    + "<br>"
                    + "This farm has a change in approved allocations."
                    + "<br>"
                    + "<table>"
                    + "<tr>"
                    + "<th width='20%' style='text-align:right'>Previous Allocations</th>"
                    + "<th width='20%' style='text-align:right'>New Allocations</th>"
                    + "<th width='20%' style='text-align:right'>Total</th>"
                    + "</tr>"
                    + "<tr>"
                    + "<td style='text-align:right'>" + prevShareCount + "</td>"
                    + "<td style='text-align:right'>" + changeInShares + "</td>"
                    + "<td style='text-align:right'>" + newShareCount + "</td>"
                    + "</tr>"
                    + "</table>"
                    + "<br>";
                ml.SmtpClient smtp = new ml.SmtpClient("smtp.state.me.us");
            smtp.Send(msg);
        }
    }
}

