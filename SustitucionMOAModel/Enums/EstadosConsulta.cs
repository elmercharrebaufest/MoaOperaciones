using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadosConsulta
    {
        Iniciado
    }

    public static class EstadoConsultaExtensions
    {
        public static string Code(this EstadosConsulta me)
        {
            switch (me)
            {
                case EstadosConsulta.Iniciado:
                    return "INI";
                default:
                    return string.Empty;
            }
        }
    }
}
