using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class InfoProveedorDataAgroDto
    {
        public string ProveedorCBU { get; set; }
        public string ProveedorClasificacion { get; set; }
        public string EstadoSISA { get; set; }

        public override bool Equals(object obj)
        {
            return obj is InfoProveedorDataAgroDto dto &&
                   ProveedorCBU == dto.ProveedorCBU &&
                   ProveedorClasificacion == dto.ProveedorClasificacion &&
                   EstadoSISA == dto.EstadoSISA;
        }

        public override int GetHashCode()
        {
            int hashCode = 350282287;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProveedorCBU);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProveedorClasificacion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EstadoSISA);
            return hashCode;
        }
    }
}
