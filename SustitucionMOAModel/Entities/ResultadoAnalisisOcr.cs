using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class ResultadoAnalisisOcr
    {
        [Key]
        public int Id { get; set; }
        public int Archivo_Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string ValidataionType { get; set; }
        public string Value { get; set; }
        public string Input { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}
