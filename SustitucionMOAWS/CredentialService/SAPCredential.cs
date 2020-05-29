using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOACrypting;

namespace SustitucionMOAWS.CredentialService
{
    public static class SAPCredential
    {
        private static string UserName = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["UserNameSapEnc"]);
        private static string Password = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["PasswordSapEnc"]);

        public static string getUserName() {
            return UserName;
        }

        public static string getPassword()
        {
            return Password;
        }
    }
}
