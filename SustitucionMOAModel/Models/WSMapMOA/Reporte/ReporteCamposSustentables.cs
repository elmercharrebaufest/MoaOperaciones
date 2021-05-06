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
        public int CantidadCampos { get; set; }

        public override string GetBody()
        {
            return $"{CantidadCampos}";
        }

        public override string GetFecha()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }
    }
}
