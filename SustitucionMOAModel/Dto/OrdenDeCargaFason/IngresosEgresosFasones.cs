using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{

	public class IngresosEgresosFasones
	{
		public int FazonId { get; set; }
		public string Almacen { get; set; }

		public int Cantidad { get; set; }

		public double Destino { get; set; }

		public DateTime FechaIng { get; set; }

		public int IMNUMSCATO { get; set; }

		public int Material { get; set; }

		public string NombreChofer { get; set; }

		public int NroDocumento { get; set; }

		public string Patente { get; set; }

		public string Patente2 { get; set; }

		public int Procedencia { get; set; }

		public object ProvinciaOrig { get; set; }

		public int TipoDoc { get; set; }

		public string TipoMov { get; set; }

		public double Transportista { get; set; }

		public string UniMedCant { get; set; }
	}
}
