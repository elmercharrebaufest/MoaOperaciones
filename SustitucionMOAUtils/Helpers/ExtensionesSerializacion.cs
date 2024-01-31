using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace SustitucionMOAUtils.Helpers
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
                catch (Exception ex)
                {
                    Log.Error(ex);
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
    }
}
