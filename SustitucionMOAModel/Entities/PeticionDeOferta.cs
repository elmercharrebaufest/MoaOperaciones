using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOferta
    {
        [Key]
        public int Id { get; set; }
        public int Solp_Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime PlazoDeOferta { get; set; }
        public string Observaciones { get; set; }
        public bool? RegistroInfo { get; set; }
        public bool? AdjuntoPliego { get; set; }
        public int? RevisionTecnica_Id { get; set; }

        [ForeignKey("UsuarioCreador_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Solp_Id")]
        public virtual Solp Solp { get; set; }

        [InverseProperty("PeticionDeOferta")]
        public virtual ICollection<PeticionDeOfertaSolpPosicion> Posiciones { get; set; } = new List<PeticionDeOfertaSolpPosicion>();

        [InverseProperty("PeticionDeOferta")]
        public virtual ICollection<PeticionDeOfertaUsuario> Usuarios { get; set; } = new List<PeticionDeOfertaUsuario>();

        [InverseProperty("PeticionDeOferta")]
        public virtual ICollection<PeticionDeOfertaArchivo> Archivos { get; set; } = new List<PeticionDeOfertaArchivo>();

        [InverseProperty("PeticionDeOferta")]
        public virtual ICollection<PeticionDeOfertaCierre> Cierres { get; set; } = new List<PeticionDeOfertaCierre>();

        [ForeignKey("RevisionTecnica_Id")]
        public virtual PeticionDeOfertaRevisionTecnica RevisionTecnica { get; set; }

        [InverseProperty("PeticionDeOferta")]
        public virtual ICollection<ChatInternoCompras> ChatInternoCompras { get; set; } = new List<ChatInternoCompras>();

        [InverseProperty("PeticionDeOferta")]
        public virtual ICollection<PeticionDeOfertaUsuarioAdicional> UsuariosAdicionales { get; set; } = new List<PeticionDeOfertaUsuarioAdicional>();
    }
}
