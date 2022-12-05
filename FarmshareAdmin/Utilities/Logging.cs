
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using mdl = FarmshareAdmin.Models;
/*
 *  Synopsis: Writes log messages.
 *  
 *  October 2016  Roland Wales
 *  
 *  Change log:
 */

namespace FarmshareAdmin.Utilities
{
    public class Logging
    {
        mdl.ACF_FarmshareContext _context;
        System.Data.IDbConnection conn;
        public Logging(mdl.ACF_FarmshareContext context)
        {
            _context = context;
            conn = _context.Database.GetDbConnection();
            conn.Open();
        }

        //public string writeLog(string logMsg)
        //{
        //    string x = "";
        //    try
        //    {
        //        using (var writer = new StreamWriter("D:\\IIS\\Content\\wwwroot\\ACF"))
        //        {
        //            writer.WriteLine(logMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        x = ex.Message;
        //    }
        //    return x;
        //}

        public void writeLog(string logMsg)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText =
                    "insert into farmshare.message_log (userid, create_time, log_message)  values ('', @create_time, @log_message)";
                cmd.Parameters.Add(new SqlParameter("@create_time", DateTime.Now.ToString()));
                cmd.Parameters.Add(new SqlParameter("@log_message", logMsg));
                using (var result = cmd.ExecuteReader())
                {
                    /*
                     * Insert returns result.rowsAffected.  I can see this with the debugger, but not with intellesense.
                     */
                    //while (result.Read())
                }
            }
        }

        public void logError(string referenceLocation, Exception exParm)
        {
            /*
             *  Write the database error message to the log
             */

            string msg = referenceLocation;
            try
            {
                if (exParm.Message != null)
                    msg += ". " + exParm.Message;
                else
                    msg += ". Undetermined error message";
                writeLog(msg);
                if (exParm.StackTrace != null)
                    writeLog(exParm.StackTrace);
                if (exParm.InnerException != null)
                    getInnerExceptions(exParm.InnerException);
            }
            catch (Exception)
            {
                string st = exParm.Message;   /* no where to log this error, use this for debugging */
            }
        }

        public void getInnerExceptions(object comServer)
        {
            string exception = "";

            exception = (((Exception)comServer).Message);
            writeLog(exception);
            if (((Exception)comServer).InnerException != null)
            {
                getInnerExceptions(((Exception)comServer).InnerException);
            }
        }
        public void removeOldLogEntries()
        {
            try
            {
                using (var conn = _context.Database.GetDbConnection())
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        //  Delete entries over 14 days old
                        cmd.CommandText = "delete from farmshare.message_log where create_time < dateadd(day, -14, getdate())";
                        cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception ex)
            {
                writeLog("Error in removeOldLogEntries: " + ex.ToString());
            }
        }

        ~Logging()
        {
            conn.Close();
        }
    }
}