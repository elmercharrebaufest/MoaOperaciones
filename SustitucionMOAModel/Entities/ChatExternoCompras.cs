using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class ChatExternoCompras
    {
        [Key]
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
        public string Mensaje { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }


        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("PeticionDeOfertaUsuario_Id")]
        public virtual PeticionDeOfertaUsuario PeticionDeOfertaUsuario { get; set; }

    }
}


    
