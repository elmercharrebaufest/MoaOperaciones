using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Util.EntitiesExtensions
{
    public static class AprobacionesExtensions
    {
        private static readonly string _estadoPendienteAprobacion = "Pendiente Aprobación";
        private static readonly string _estadoAprobada = "Aprobada";
        private static readonly string _estadoRechazada = "Rechazado";
        private static readonly string _estadoAnulada = "Anulada";

        public static bool EstaPendienteAprobacion(this Aprobaciones certificacion)
        {
            return certificacion.Estado_certificacion == _estadoPendienteAprobacion;
        }

        public static bool EstaAprobada(this Aprobaciones certificacion)
        {
            return certificacion.Estado_certificacion == _estadoAprobada;
        }

        public static bool EstaRechazada(this Aprobaciones certificacion)
        {
            return certificacion.Estado_certificacion == _estadoRechazada;
        }

        public static bool EstaAnulada(this Aprobaciones certificacion)
        {
            return certificacion.Estado_certificacion == _estadoAnulada;
        }


        public static void SetEstadoPendienteAprobacion(this Aprobaciones certificacion)
        {
            certificacion.Estado_certificacion = _estadoPendienteAprobacion;
        }

        public static void SetEstadoAprobada(this Aprobaciones certificacion)
        {
            certificacion.Estado_certificacion = _estadoAprobada;
        }

        public static void SetEstadoRechazada(this Aprobaciones certificacion)
        {
            certificacion.Estado_certificacion = _estadoRechazada;
        }

        public static void SetEstadoAnulada(this Aprobaciones certificacion)
        {
            certificacion.Estado_certificacion = _estadoAnulada;
        }
    }
}
