using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class ChatInternoCompras
    {
        [Key]
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public int Solp_Id { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool Leido { get; set; }
        public string Mensaje { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Solp_Id")]
        public virtual Solp Solp { get; set; }

    }
}


    
