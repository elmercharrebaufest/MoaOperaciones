using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOACrypting;

namespace SustitucionMOAUtils.Credentials
{
    public static class CamCredential
    {
        private static string CamUser = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["CamUserEnc"]);
        private static string CamPass = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["CamPassEnc"]);

        public static string getCamUser()
        {
            return CamUser;
        }

        public static string getCamPass()
        {
            return CamPass;
        }
    }
}
