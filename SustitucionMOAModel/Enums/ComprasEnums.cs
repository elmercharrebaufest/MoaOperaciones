using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoDocumentoSolp
    {
        Incompleto,
        Creado,
        Finalizado
    }

    public enum TipoSolpSap
    {
        Web = 1,
        Mantenimiento = 2,
        Sap = 3
    }

    public enum SolpDescargaZipPorLink
    {
        SolpIdNoExiste = 1,
        EmailTokenInvalido = 2,
        PuedeDescargar = 3
    }

    public static class ComprasEnumsExtensions
    {
        public static string Code(this EstadoDocumentoSolp me)
        {
            switch (me)
            {
                case EstadoDocumentoSolp.Incompleto: return "INCOMPLETO";
                case EstadoDocumentoSolp.Creado: return "CREADO";
                case EstadoDocumentoSolp.Finalizado: return "FINALIZADO";
                default:
                    return string.Empty;
            }
        }
        public static string CodeTipoSolpSap(this TipoSolpSap me)
        {
            switch (me)
            {
                case TipoSolpSap.Web: return "R";
                case TipoSolpSap.Mantenimiento: return "F";
                case TipoSolpSap.Sap: return "R";
                default:
                    return string.Empty;
            }
        }
    }
}
