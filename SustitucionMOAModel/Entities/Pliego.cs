using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Pliego
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string NombreObra { get; set; }
        public string FiscalContrato { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime? FechaHoraEntrega { get; set; }
        public string SupervisorSector { get; set; }
        public string SupervisorTrabajo { get; set; }
        public bool TieneVisitaObraMasiva { get; set; }
        public bool? TieneObradores { get; set; }
        public bool? TieneMedioElevacion { get; set; }
        public bool? TieneAndamio { get; set; }
        public bool? TieneTecnicoSeguridad { get; set; }
        public bool? TieneGrillaPersonal { get; set; }
        public bool? TieneFabricacionTallerExterno { get; set; }
        public bool? TieneDescripcionTecnica { get; set; }
        public bool? TieneDocumentacionTecnica { get; set; }
        public DateTime? FechaHoraLimiteConsulta { get; set; }
        public string ObservacionesGeneracion { get; set; }
        public int? DiasEjecucion { get; set; }
        public string ObservacionesCotizacion { get; set; }
        public string ObservacionesCotizacionCondEsp { get; set; }
        public string JornadaLaboralDias { get; set; }
        public DateTimeOffset? JornadaLaboralHorasDesde { get; set; }
        public DateTimeOffset? JornadaLaboralHorasHasta { get; set; }
        public bool? TieneCondicionesGenerales { get; set; }
        public string RevisadoPor { get; set; }

        public bool Multiple { get; set; }

        public DateTime FechaAlta { get; set; }
        public DateTime FechaModificacion { get; set; }

        public int Usuario_Id { get; set; }
        [ForeignKey(nameof(Usuario_Id))]
        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<PliegoVisita> VisitasMasivas { get; set; }

        [InverseProperty("Pliegos")]
        public virtual ICollection<Archivo> Archivos { get; set; }
        public bool? RequisitoCiberseguridad { get; set; }

    }
}