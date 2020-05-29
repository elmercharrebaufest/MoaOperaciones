using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOACrypting;

namespace SustitucionMOAUtils.Credentials
{
    public static class DBCredential
    {
        private static string ConnectionString = CryptoServiceProvider.Decrypt(ConfigurationManager.ConnectionStrings["ConnectionMOADB"].ConnectionString);

        public static string getConnectionString()
        {
            return ConnectionString;
        }
    }
}
