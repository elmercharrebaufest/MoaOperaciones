using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadosConsulta
    {
        Iniciado,
        Reasignado,
        EnGestion,
        SolicitudInformacion,
        Rechazado,
        Finalizado
    }

    public static class EstadoConsultaExtensions
    {
        public static string Code(this EstadosConsulta me)
        {
            switch (me)
            {
                case EstadosConsulta.Iniciado: return "INI";
                case EstadosConsulta.Reasignado: return "REA";
                case EstadosConsulta.EnGestion: return "ENG";
                case EstadosConsulta.SolicitudInformacion: return "SOL";
                case EstadosConsulta.Rechazado: return "REC";
                case EstadosConsulta.Finalizado: return "CER";
                default:
                    return string.Empty;
            }
        }
    }
}
