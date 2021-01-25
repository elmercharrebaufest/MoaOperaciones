using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Reporte
{
    public abstract class ReporteBase
    {
        public string Asunto { get; set; }
        public string Destinatario { get; set; }
        public string Template { get; set; }

        public abstract string GetBody();
        public abstract string GetFecha();
    }
}
