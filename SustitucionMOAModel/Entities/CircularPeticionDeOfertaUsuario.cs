using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CircularPeticionDeOfertaUsuario
    {
        [Key]
        public int Id { get; set; }
        public int Circular_Id { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }

        public DateTime? FechaLeida { get; set; }
        public bool? Leida { get; set; }


        [ForeignKey("Circular_Id")]
        public virtual Circular Circular { get; set; }

        [ForeignKey("PeticionDeOfertaUsuario_Id")]
        public virtual PeticionDeOfertaUsuario PeticionDeOfertaUsuario { get; set; }

    }
}
