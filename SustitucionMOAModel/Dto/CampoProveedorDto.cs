using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Dto
{
    public class CampoProveedorDto
    {
        public string NombreCampo { get; set; }
        public string Renspa { get; set; }
        public string NombreCosecha { get; set; }
        public double HectareasTotales { get; set; }
        public double HectareasSoja { get; set; }
        public double ToneladasAprobadas { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int CampoCosechaId { get; set; }
        public string ProveedorNombre { get; set; }
        public string LocalidadNombre { get; set; }
        public int Localidad_Id { get; set; }
        public int CampoSustentableId { get; set; }
        public int CosechaId { get; set; }
        public string CUIT { get; set; }
        public int Archivo_Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string CodigoProveedor { get; set; }
        public int? EvidenciaEPA_Id { get; set; }

        [ForeignKey("EvidenciaEPA_Id")]
        public bool BSVS2 { get; set; }
        public bool EPA { get; set; }
        public bool EUDER { get; set; }

        public override bool Equals(object obj)
        {
            return obj is CampoProveedorDto dto &&
                   NombreCampo == dto.NombreCampo &&
                   Renspa == dto.Renspa &&
                   NombreCosecha == dto.NombreCosecha &&
                   HectareasTotales == dto.HectareasTotales &&
                   HectareasSoja == dto.HectareasSoja &&
                   ToneladasAprobadas == dto.ToneladasAprobadas &&
                   Latitud == dto.Latitud &&
                   Longitud == dto.Longitud &&
                   CampoCosechaId == dto.CampoCosechaId &&
                   ProveedorNombre == dto.ProveedorNombre &&
                   LocalidadNombre == dto.LocalidadNombre &&
                   Localidad_Id == dto.Localidad_Id &&
                   CampoSustentableId == dto.CampoSustentableId &&
                   CosechaId == dto.CosechaId;
        }

        public override int GetHashCode()
        {
            int hashCode = 1599569244;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreCampo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Renspa);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreCosecha);
            hashCode = hashCode * -1521134295 + HectareasTotales.GetHashCode();
            hashCode = hashCode * -1521134295 + HectareasSoja.GetHashCode();
            hashCode = hashCode * -1521134295 + ToneladasAprobadas.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Latitud);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Longitud);
            hashCode = hashCode * -1521134295 + CampoCosechaId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProveedorNombre);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LocalidadNombre);
            hashCode = hashCode * -1521134295 + Localidad_Id.GetHashCode();
            hashCode = hashCode * -1521134295 + CampoSustentableId.GetHashCode();
            hashCode = hashCode * -1521134295 + CosechaId.GetHashCode();
            return hashCode;
        }
    }
}