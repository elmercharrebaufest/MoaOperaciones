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
            catch (Exception e)
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, tipoFecha), e);
            }
            
        }
    }
}
