using System.Globalization;

namespace SustitucionMOAModel.Models
{
    public class GeoCoordenada
    {
        public double Longitud { get; set; }

        public double Latitud { get; set; }


        public string Valor
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0},{1}", Longitud, Latitud);
            }
        }
    }
}
