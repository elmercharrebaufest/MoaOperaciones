using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CampoProveedor
    {
        [Key, Column(Order = 0)]
        public int CampoCosecha_Id { get; set; }
        public virtual CampoCosecha CampoCosecha { get; set; }
        [Key, Column(Order = 1)]
        public int Proveedor_Id { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        public double HectareasTotales { get; set; }
        public double HectareasSoja { get; set; }
        public double Longitud { get; set; }
        public double Latitud { get; set; }
        
        public int Archivo_Id { get; set; }
        public virtual Archivo Archivo { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public bool Borrado { get; set; }
    }
}
