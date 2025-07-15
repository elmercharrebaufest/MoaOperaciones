using System;
using System.Configuration;
using System.ServiceModel;

namespace SustitucionMOAWS.CredentialService
{
    public static class SAPCredential
    {
        private static string UserName = ConfigurationManager.AppSettings["UserNameSap"];
        private static string Password = ConfigurationManager.AppSettings["PasswordSap"];

        public static string getUserName()
        {
            return UserName;
        }

        public static string getPassword()
        {
            return Password;
        }

        public static BasicHttpBinding CrearSapBasicBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding()
            {
                Name = "sap_basic",
                CloseTimeout = TimeSpan.FromMinutes(1),
                OpenTimeout = TimeSpan.FromMinutes(1),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                SendTimeout = TimeSpan.FromMinutes(1.5),
                AllowCookies = false,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxBufferPoolSize = 5000000,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = System.Text.Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true
            };

            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 8192;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;

            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            return binding;
        }

        // Método para crear la instancia de BasicHttpBinding con el nombre "sap_long"
        public static BasicHttpBinding CrearSapLongBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding()
            {
                Name = "sap_long",
                CloseTimeout = TimeSpan.FromHours(1),
                OpenTimeout = TimeSpan.FromHours(1),
                ReceiveTimeout = TimeSpan.FromHours(1),
                SendTimeout = TimeSpan.FromHours(1),
                AllowCookies = false,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxBufferPoolSize = 5000000,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = System.Text.Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true
            };

            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 8192;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;

            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            return binding;
        }

        public static EndpointAddress DevolverEndpoint(string url)
        {
            return new EndpointAddress(url.Replace("http://gslopidevqa00.molinosagro.ad:50000/", ConfigurationManager.AppSettings["Url"]).Replace("&amp;", "&").Replace("%3A", ":"));
        }


    }
}
