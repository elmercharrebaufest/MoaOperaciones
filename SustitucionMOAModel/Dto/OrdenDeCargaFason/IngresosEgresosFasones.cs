using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{

	public class IngresosEgresosFasones
	{
		public long FasonId { get; set; }
		public double PesadaTara { get; set; }
        public double PesadaNeto { get; set; }
        public DateTime FechaIngreso { get; set; }
		public DateTime FechaEgreso { get; set; }
		public string NroRemito { get; set; }
		public string UniMedCant { get; set; }
	}
}
