using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PliegoVisita
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTime? FechaHora { get; set; }
        
        public int Pliego_Id { get; set; }
        [ForeignKey("Pliego_Id")]
        public virtual Pliego Pliego { get; set; }
    }
}
