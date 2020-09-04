using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoAprobacionDto
    {
        public EstadoAprobacion Estado { get; set; }
        public string EstadoDescripcion { get; set; }
        public string Observaciones { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EstadoAprobacionDto dto &&
                   Estado == dto.Estado &&
                   EstadoDescripcion == dto.EstadoDescripcion &&
                   Observaciones == dto.Observaciones;
        }

        public override int GetHashCode()
        {
            int hashCode = 851899469;
            hashCode = hashCode * -1521134295 + Estado.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EstadoDescripcion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Observaciones);
            return hashCode;
        }
    }

}
