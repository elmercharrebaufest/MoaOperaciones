using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CertificacionRegistrada
    {
        [Key]
        public int Id { get; set; }
        public string NombreDeArchivo { get; set; }
        public string NRO_OC { get; set; }
        public string NRO_Certificacion { get; set; }
        public decimal Importe { get; set; }
        public string Moneda { get; set; }
        public DateTime FechaDeRegistro { get; set; }
        public int UsuarioId { get; set; }
        public int ProveedorId { get; set; }
        public int ArchivoId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("ArchivoId")]
        public virtual Archivo Archivo { get; set; }
    }
}
