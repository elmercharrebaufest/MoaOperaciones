
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Xml.Serialization;

namespace SustitucionMOAWS.Util
{
    public static class ExtensionesSerializacion
    {
        public static string ToJson<T>(this T data)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));

                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, data);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception)
            {
                try
                {
                    string jsonObjecto = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    return jsonObjecto;
                }
                catch (Exception)
                {
                    return "error al serializar el objeto.";
                }
            }

        }

        public static string ToXml<T>(this T data)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, data);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T FromJson<T>(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return default(T);
            }
            var js = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            return (T)js.ReadObject(ms);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static List<SolpSubposicion> GetClone(List<SolpSubposicion> source)
        {
            return source;
        }

        public static IList<T> CloneList<T>(this IList<T> source) where T : ICloneable
        {
            return source.Select(item => (T)item.Clone()).ToList();
        }

        public static BasicHttpBinding getBindingSapBasic()
        {
            BasicHttpBinding binding = new BasicHttpBinding

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
                MaxBufferSize = int.MaxValue, // 2147483647
                MaxReceivedMessageSize = int.MaxValue, // 2147483647
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true,
            };


            binding.ReaderQuotas.MaxDepth = 32;
            binding.ReaderQuotas.MaxStringContentLength = 8192;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
            return binding;

        }
    }
}
