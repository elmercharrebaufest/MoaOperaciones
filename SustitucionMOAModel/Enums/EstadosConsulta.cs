using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadosConsulta
    {
        Iniciado = 1,
        EnGestion = 2,
        EnGestionRta = 3,
        SolicitudInformacion = 4,
        Rechazado = 5,
        Finalizado = 6
    }

    public static class EstadoConsultaExtensions
    {
        public static string Code(this EstadosConsulta me)
        {
            switch (me)
            {
                case EstadosConsulta.Iniciado: return "INI";
                case EstadosConsulta.EnGestion: return "GES";
                case EstadosConsulta.EnGestionRta: return "GESRTA";
                case EstadosConsulta.SolicitudInformacion: return "DOC";
                case EstadosConsulta.Rechazado: return "REC";
                case EstadosConsulta.Finalizado: return "CER";
                default:
                    return string.Empty;
            }
        }
    }
}
