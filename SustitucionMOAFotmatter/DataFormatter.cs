using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public static string CuitConGuion(string cuit) {
            string formateado = cuit;
            if (string.IsNullOrEmpty(cuit))
                throw new ValidationCustomException("No se puede formatear como CUIT una string vacía");
            if (!cuit.Contains("-"))
            {
                formateado = cuit.Insert(2,"-").Insert(11,"-");
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
    }
}
