using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Cotizacion
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public int CotizacionEstado_Id { get; set; }       
        public int PeticionDeOfertaUsuario_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool? RespetaMateriales { get; set; }
        public bool? RespetaServicios { get; set; }
        public string ObservacionTecnica { get; set; }
        public string ObservacionEconomica { get; set; }
        public int Revision { get; set; }

        [ForeignKey("UsuarioCreador_Id")]
        public virtual Usuario UsuarioCreador { get; set; }
        [ForeignKey("CotizacionEstado_Id")]
        public virtual CotizacionEstado CotizacionEstado { get; set; }
        [ForeignKey("PeticionDeOfertaUsuario_Id")]
        public virtual PeticionDeOfertaUsuario PeticionDeOfertaUsuario { get; set; }
        [InverseProperty("Cotizaciones")]
        public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();
        [InverseProperty("Cotizacion")]
        public virtual ICollection<CotizacionPosicion> CotizacionPosiciones { get; set; } = new List<CotizacionPosicion>();
        [InverseProperty("Cotizacion")]
        public virtual ICollection<Adjudicacion> Adjudicaciones { get; set; } = new List<Adjudicacion>();
        public virtual ICollection<CotizacionHora> CotizacionesHoras { get; set; } = new List<CotizacionHora>();
    }
}