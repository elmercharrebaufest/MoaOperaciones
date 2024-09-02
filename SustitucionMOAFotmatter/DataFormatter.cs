using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;

namespace SustitucionMOAFotmatter
{
    public class DataFormatter
    {
        public static DateTime StringToDateTime(string fecha, string tipoFecha)
        {
            try
            {
                return DateTime.Parse(fecha);
            }
            catch
            {
                try
                {
                    var stringDate = new string(fecha.Where(c => c != '\u200E').ToArray());
                    return DateTime.Parse(stringDate);
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, tipoFecha), e);

                }
            }

        }
        public static string CuitConGuion(string cuit)
        {
            string formateado = cuit;
            if (string.IsNullOrEmpty(cuit))
                throw new ValidationCustomException("No se puede formatear como CUIT una string vacía");
            if (!cuit.Contains("-"))
            {
                formateado = cuit.Insert(2, "-").Insert(11, "-");
            }
            return formateado;
        }

        public static string CuitACodigoSap(string cuit)
        {
            if (string.IsNullOrEmpty(cuit) || cuit.Length < 4)
            {
                return cuit;
            }
            if (cuit.Contains("-"))
            {
                throw new ValidationCustomException("No se puede obtener código SAP de CUIT " + cuit);
            }
            return cuit.Substring(2, cuit.Length - 3);
        }
        public static int ParseMinutes(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
            {
                throw new ArgumentNullException(nameof(timeString));
            }

            string[] parts = timeString.Split(':');

            if (parts.Length != 3)
            {
                throw new FormatException("Invalid time format. Expected 'hhhh:mm:ss.ms'.");
            }

            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2].Split('.')[0]); // Extract seconds before decimal point
            int milliseconds = int.Parse(parts[2].Split('.')[1]); // Extract milliseconds after decimal point

            return hours * 60 + minutes + seconds / 60 + milliseconds / (60 * 1000);
        }

        public static string FormatMinutes(int totalMinutes)
        {
            if (totalMinutes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalMinutes), "Total minutes cannot be negative.");
            }

            int hours = totalMinutes / 60;
            int remainingMinutes = totalMinutes % 60;

            int seconds = remainingMinutes % 60;
            int milliseconds = (remainingMinutes * 60 * 1000) % 1000; // Calculate milliseconds preserving precision

            return $"{hours:04d}:{remainingMinutes:02d}:{seconds:02d}.{milliseconds:03d}";
        }

        public static string FormatEncodedURI(string encoded)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(encoded)) != encoded)
                encoded = newUrl;
            return newUrl;
        }
        public static T GetDtoFromJsonString<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy() // or PascalCaseNamingStrategy()
                }
            };

            using (var jsonReader = new JsonTextReader(new StringReader(json)))
            {
                var serializer = JsonSerializer.Create(settings);
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}
