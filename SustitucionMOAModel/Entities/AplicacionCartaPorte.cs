using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    internal class AplicacionCartaPorte
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Proveedor_Id { get; set; }
        [ForeignKey("Proveedor_Id")]
        public Proveedor Proveedor { get; set; }
        [Required]
        public string Contrato { get; set; }
        [Required]
        public string CartaPorte { get; set; }
        [Required]
        public EstadoAplicacionCartaPorte Estado { get; set; }
        [Required]
        public int Kilogramos { get; set; }
        [Required]
        public int Usuario_Id { get; set; }
        [ForeignKey("Usuario_Id")]
        public Usuario Usuario { get; set; }
        public string Error { get; set; }
        [Required]
        public DateTime FechaAlta { get; set; }
        [Required]
        public DateTime FechaActualizacion { get; set; }

    }
}
