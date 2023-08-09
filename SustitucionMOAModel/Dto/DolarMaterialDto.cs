using System;

namespace SustitucionMOAModel.Dto
{
    public class DolarMaterialDto
    {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public DateTime FechaCotizacion { get; set; }
        public double Cotizacion { get; set; }
        public string DesdeString { get { return Desde.ToString("dd-MM-yyyy"); } }
        public string HastaString { get { return Hasta.ToString("dd-MM-yyyy"); } }
        public string FechaCotizacionString { get { return FechaCotizacion.ToString("dd-MM-yyyy"); } }
    }
}
