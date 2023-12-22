using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaRevisionTecnica
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime Fecha { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}