using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class InfoProveedorDataAgroDto
    {
        public string ProveedorCBU { get; set; }
        public string ProveedorClasificacion { get; set; }
        public string EstadoSISA { get; set; }
        
        public string RazonSocial { get; set; }
        
        public string ProveedorCUIT { get; set; }
        public bool AltaInterna { get; set; }
        public string Observacion { get; set; }

        public override bool Equals(object obj)
        {
            return obj is InfoProveedorDataAgroDto dto &&
                   ProveedorCBU == dto.ProveedorCBU &&
                   ProveedorClasificacion == dto.ProveedorClasificacion &&
                   EstadoSISA == dto.EstadoSISA &&
                   RazonSocial == dto.RazonSocial &&
                   ProveedorCUIT == dto.ProveedorCUIT &&
                   AltaInterna == dto.AltaInterna;
        }

        public override int GetHashCode()
        {
            int hashCode = 735836869;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProveedorCBU);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProveedorClasificacion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EstadoSISA);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocial);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProveedorCUIT);
            hashCode = hashCode * -1521134295 + AltaInterna.GetHashCode();
            return hashCode;
        }
    }
}
