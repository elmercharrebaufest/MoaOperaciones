using System.Configuration;
using SustitucionMOACrypting;

namespace SustitucionMOAWS.CredentialService
{
    public static class DataAgroWSCredential
    {
        private static string UserName = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["UserNameDataAgroWS"]);
        private static string Password = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["PasswordDataAgroWS"]);
        private static string Dominio = CryptoServiceProvider.Decrypt(ConfigurationManager.AppSettings["DominioDataAgroWS"]);

        public static string getUserName()
        {
            return UserName;
            return "emartin";
        }

        public static string getPassword()
        {
            return Password;
            return "eugeniomartin3";
        }

        public static string getDominio()
        {
            return Dominio;
            return "baunet";
        }
    }
}
