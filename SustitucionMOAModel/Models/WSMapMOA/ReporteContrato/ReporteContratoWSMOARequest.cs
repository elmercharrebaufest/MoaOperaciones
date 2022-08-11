using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.ReporteContrato
{
    public class ReporteContratoWSMOARequest
    {
		public string Cliente { get; set; }
		public string Contrato { get; set; }
		public string Corredor { get; set; }
		public List<FechaWS> Fechas { get; set; }
		public string Material { get; set; }
		public string Pendiente { get; set; }
		public string TipoContrato { get; set; }
	}
}
