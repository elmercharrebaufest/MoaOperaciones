using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaUsuario
    {
        [Key]
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Usuario_Id { get; set; }
        public bool? RealizoVisita { get; set; }

        public DateTime? RealizoVisitaFecha { get; set; }
        public int? RealizoVisitaUsuario_Id { get; set; }

        public bool? PropuestaTecnicaAprobada { get; set; }

        public DateTime? PropuestaTecnicaFecha { get; set; }
        public int? PropuestaTecnicaUsuario_Id { get; set; }
        public string ObservacionNoCumple { get; set; }


        [ForeignKey("PeticionDeOferta_Id")]
        public virtual PeticionDeOferta PeticionDeOferta { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("RealizoVisitaUsuario_Id")]
        public virtual Usuario RealizoVisitaUsuario { get; set; }

        [ForeignKey("PropuestaTecnicaUsuario_Id")]
        public virtual Usuario PropuestaTecnicaUsuario { get; set; }


        [InverseProperty("PeticionDeOfertaUsuario")]
        public virtual ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();

        [InverseProperty("PeticionDeOfertaUsuario")]
        public virtual ICollection<CircularPeticionDeOfertaUsuario> Circulares { get; set; } = new List<CircularPeticionDeOfertaUsuario>();
    }
}
