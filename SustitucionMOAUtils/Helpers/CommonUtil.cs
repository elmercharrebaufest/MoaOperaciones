using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAFotmatter;
using SustitucionMOAValidator;

namespace SustitucionMOAUtils.Services
{
    public static class CommonUtil
    {
        public static List<FechaWS> toDateList(string fechaInicio, string fechaFin) {
            DateTime fechaIncioDateTime = DateTime.Now;
            DateTime fechaFinDateTime = DateTime.Now;
            fechaIncioDateTime = DataFormatter.StringToDateTime(fechaInicio, "inicio");
            fechaFinDateTime = DataFormatter.StringToDateTime(fechaFin, "fin");
            InputValidator.rangoFechas(fechaIncioDateTime, fechaFinDateTime);
            return new List<FechaWS>() { new FechaWS() { fechaInicio = fechaIncioDateTime, fechaFin = fechaFinDateTime } };
        }

        public static FechaWS toDate(string fechaInicio, string fechaFin)
        {
            DateTime fechaIncioDateTime = DateTime.Now;
            DateTime fechaFinDateTime = DateTime.Now;
            fechaIncioDateTime = DataFormatter.StringToDateTime(fechaInicio, "inicio");
            fechaFinDateTime = DataFormatter.StringToDateTime(fechaFin, "fin");
            InputValidator.rangoFechas(fechaIncioDateTime, fechaFinDateTime);
            return new FechaWS() { fechaInicio = fechaIncioDateTime, fechaFin = fechaFinDateTime };
        }

        public static DateTime toDateFecha(string fecha, string name)
        {
            DateTime fechaDateTime = DateTime.Now;
            fechaDateTime = DataFormatter.StringToDateTime(fecha, name);
            return fechaDateTime;
        }
    }
}
