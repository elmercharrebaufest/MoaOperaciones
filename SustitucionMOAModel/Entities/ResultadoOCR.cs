using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class ResultadoOcr
    {
        [Key]
        public int Id { get; set; }

        public string Texto { get; set; }

        public int Archivo_Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaAlta { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}
