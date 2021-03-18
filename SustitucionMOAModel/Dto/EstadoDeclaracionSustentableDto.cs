using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoDeclaracionSustentableDto
    {
        public bool DeclaracionFirmada { get; set; }
        public string CosechaActual { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EstadoDeclaracionSustentableDto dto &&
                   DeclaracionFirmada == dto.DeclaracionFirmada &&
                   CosechaActual == dto.CosechaActual &&
                   RazonSocial == dto.RazonSocial &&
                   CUIT == dto.CUIT;
        }

        public override int GetHashCode()
        {
            int hashCode = -1794181187;
            hashCode = hashCode * -1521134295 + DeclaracionFirmada.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CosechaActual);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocial);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUIT);
            return hashCode;
        }
    }
}
