using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum SituacionIVA
    {
        ResponsableInscripto,
        ResponsableNoInscripto,
        Exento,
        Monotributo
    }

    public static class SituacionIVAExtensions
    {
        public static string ToFriendlyString(this SituacionIVA me)
        {
            switch (me)
            {
                case SituacionIVA.ResponsableInscripto:
                    return "Responsable Inscripto";
                case SituacionIVA.ResponsableNoInscripto:
                    return "Responsable No Inscripto";
                case SituacionIVA.Exento:
                    return "Exento";
                case SituacionIVA.Monotributo:
                    return "Monotributo";
                default:
                    return "";
            }
        }
    }
}
