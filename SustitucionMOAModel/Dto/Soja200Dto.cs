using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class Soja200Dto
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
