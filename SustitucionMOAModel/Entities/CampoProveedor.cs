using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CampoProveedor
    {
        [Key, Column(Order = 0)]
        public int CampoCosecha_Id { get; set; }
        [ForeignKey("CampoCosecha_Id")]
        public virtual CampoCosecha CampoCosecha { get; set; }
        [Key, Column(Order = 1)]
        public int Proveedor_Id { get; set; }
        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }
        public double HectareasTotales { get; set; }
        public double HectareasSoja { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }        
        public int Archivo_Id { get; set; }
        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public bool Borrado { get; set; }
        public double? HectareasTotalesUcropit { get; set; }
        public double? HectareasSojaUcropit { get; set; }
        public int? EvidenciaEPA_Id { get; set; }
        [ForeignKey("EvidenciaEPA_Id")]
        public virtual Archivo EvidenciaEPA { get; set; }
        public bool BSVS2 { get; set; }
        public bool EPA { get; set; }
        public bool EUDR { get; set; } 

    }
}
