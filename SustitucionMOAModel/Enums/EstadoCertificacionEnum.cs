using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoCertificacionEnum
    {
        [Description("Pendiente Aprobación")] Pendiente_Aprobacion = 0,
        [Description("Aprobada")] Aprobada = 1,
        [Description("Anulada")] Anulada = 2,
        [Description("Rechazado")] Rechazado = 3,
    }
}
