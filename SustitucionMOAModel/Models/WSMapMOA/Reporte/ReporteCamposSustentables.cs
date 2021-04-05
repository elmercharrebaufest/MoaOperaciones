using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Reporte
{
    public class ReporteCamposSustentables : ReporteBase
    {
        public string Excel { get; set; }
        public IList<CampoProveedor> Campos { get; set; }

        public override string GetBody()
        {
            return $"<p>Campos dados de alta a la fecha {GetFecha()}: {Campos.Count}</p>";
        }

        public override string GetFecha()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }
    }
}
