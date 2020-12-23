using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum IngresosBrutos
    {
        Local = 1,
        ConvenioMultilateral = 2,
        NoInscripto = 3
    }

    public static class IngresosBrutosExtensions
    {
        public static string ToFriendlyString(this IngresosBrutos me)
        {
            switch (me)
            {
                case IngresosBrutos.Local:
                    return "Local";
                case IngresosBrutos.ConvenioMultilateral:
                    return "Convenio Multilateral";
                case IngresosBrutos.NoInscripto:
                    return "No inscripto";
                default:
                    return "";
            }
        }
    }
}
