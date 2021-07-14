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
    }
}
