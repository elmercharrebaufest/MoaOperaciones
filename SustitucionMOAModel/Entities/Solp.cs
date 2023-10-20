using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Solp
    {
        [Key]
        public int Id { get; set; }
        public int? UsuarioCreacion_Id { get; set; }
        public int? UsuarioModificacion_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? Pliego_Id { get; set; }
        public int? ClaseDocumento_Id { get; set; }
        public string NroSolp { get; set; }
        public int? EstadoSolpSap_Id { get; set; }
        public int? EstadoDocumento_Id { get; set; }
        public DateTime? FechaBorrado { get; set; }
        public DateTime? FechaCreacionSap { get; set; }
        public DateTime? FechaLiberacionSap { get; set; }
        public int? PasoCompletado { get; set; }
        public string EstadoPasos { get; set; }
        public int? TipoSolp_Id { get; set; }
        public int? UsuarioCompras_Id { get; set; }
        public int? TipoSolpSap { get; set; }
        public Guid? EmailLinkToken { get; set; }
        public bool? TrabajoYaHecho { get; set; }
        public int? ProveedorAsignado_Id { get; set; }
        public bool? Adicional { get; set; }
        public bool? Urgencia { get; set; }
        public string NroOrdenDeCompraAdicional { get; set; }
        public bool? SeEnvioMailLiberacion { get; set; }

        public bool? SeEnvioMailAnulacion { get; set; }
        

        [ForeignKey("ProveedorAsignado_Id")]
        public virtual Usuario ProveedorAsignado { get; set; }

        [ForeignKey("UsuarioCreacion_Id")]
        public virtual Usuario UsuarioCreacion { get; set; }
        [ForeignKey("UsuarioModificacion_Id")]
        public virtual Usuario UsuarioModificacion { get; set; }
        [ForeignKey("Pliego_Id")]
        public virtual Pliego Pliego { get; set; }
        [ForeignKey("ClaseDocumento_Id")]
        public virtual TablaSap ClaseDocumento { get; set; }
        [ForeignKey("EstadoSolpSap_Id")]
        public virtual TablaSap EstadoSolpSap { get; set; }
        [ForeignKey("EstadoDocumento_Id")]
        public virtual TablaEstado EstadoDocumento { get; set; }
        [ForeignKey("TipoSolp_Id")]
        public virtual TablaGeneral TipoSolp { get; set; }

        public virtual ICollection<SolpPosicion> Posiciones { get; set; }

        [ForeignKey("UsuarioCompras_Id")]
        public virtual UsuarioCompras UsuarioCompras { get; set; }

        [InverseProperty("Solp")]

        public virtual ICollection<Adjudicacion> Adjudicacions { get; set; } = new List<Adjudicacion>();

        [InverseProperty("Solp")]
        public virtual ICollection<PeticionDeOferta> PeticionesDeOferta { get; set; } = new List<PeticionDeOferta>();

    }
}