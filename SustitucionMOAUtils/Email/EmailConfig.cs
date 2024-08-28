using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Email
{
    public class EmailConfig
    {
        private static string emailAddTo = ConfigurationManager.AppSettings["EmailTo"];
        private static string emailAddToDocumentacion = ConfigurationManager.AppSettings["EmailToDocumentacion"];
        private static string emailAddToFletes = ConfigurationManager.AppSettings["EmailToFletes"];
        private static string emailAddFrom = ConfigurationManager.AppSettings["EmailFrom"];
        private static string emailHost = ConfigurationManager.AppSettings["HostEmail"];
        private static string emailPort = ConfigurationManager.AppSettings["PortEmail"];
        private static string emailHab = ConfigurationManager.AppSettings["HabilitarEnvioMail"];

        private static string emailRegexFormato = ConfigurationManager.AppSettings["EmailRegexFormato"];

        public static string getEmailAddTo()
        {
            return emailAddTo;
        }

        public static string getEmailAddToFletes()
        {
            return emailAddToFletes;
        }

        public static string getEmailAddToDocumentacion()
        {
            return emailAddToDocumentacion;
        }

        public static string getEmailAddFrom()
        {
            return emailAddFrom;
        }

        public static string getEmailHost()
        {
            return emailHost;
        }

        public static int getEmailPort()
        {
            try
            {
                return Convert.ToInt32(emailPort);
            }
            catch {
                return 26;
            }
        }

        public static bool getEmailHab()
        {
            try
            {
                string emailHabLower = emailHab.ToLower();
                if (emailHabLower != "true")
                {
                    return false;
                }
            }
            catch {
                return false;
            }
            return true;
        }

        public static string getEmailRegexFormato()
        {
            return emailRegexFormato;
        }
    }
}
