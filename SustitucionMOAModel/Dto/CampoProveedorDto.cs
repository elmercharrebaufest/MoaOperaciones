using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CampoProveedorDto
    {
        public string NombreCampo { get; set; }

        public string NombreCosecha { get; set; }

        public double HectareasTotales { get; set; }

        public double HectareasSoja { get; set; }

        public double ToneladasAprobadas { get; set; }

        public override bool Equals(object obj)
        {
            return obj is CampoProveedorDto dto &&
                   NombreCampo == dto.NombreCampo &&
                   NombreCosecha == dto.NombreCosecha &&
                   HectareasTotales == dto.HectareasTotales &&
                   HectareasSoja == dto.HectareasSoja &&
                   ToneladasAprobadas == dto.ToneladasAprobadas;
        }

        public override int GetHashCode()
        {
            int hashCode = -1372739976;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreCampo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreCosecha);
            hashCode = hashCode * -1521134295 + HectareasTotales.GetHashCode();
            hashCode = hashCode * -1521134295 + HectareasSoja.GetHashCode();
            hashCode = hashCode * -1521134295 + ToneladasAprobadas.GetHashCode();
            return hashCode;
        }
    }
}
