using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaUsuarioAdicional
    {
        [Key]
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Usuario_Id { get; set; }

        [ForeignKey("PeticionDeOferta_Id")]
        public virtual PeticionDeOferta PeticionDeOferta { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

    }
}
