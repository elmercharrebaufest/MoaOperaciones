using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Reporte
{
    public class ReporteLogin : ReporteBase
    {
        public string Mail { get; set; }
        public string CUITRegistro { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public string Nombre { get; set; }

        public string CUITProveedor { get; set; }
        public string RazonSocial { get; set; }


        public override string GetBody()
        {
            return "Adjuntamos el excel con los últimos datos de login";
        }

        public override string GetFecha()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }
    }
}
