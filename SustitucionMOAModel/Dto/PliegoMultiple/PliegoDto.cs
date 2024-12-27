using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto.PliegoMultiple
{
    public class PliegoDto
    {
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

        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public DateTime? FechaModificacion { get; set; }

        public int? Usuario_Id { get; set; }

        public IEnumerable<int> VisitasMasivas { get; set; }

        public virtual IEnumerable<int> Archivos { get; set; }

        public bool? RequisitoCiberseguridad { get; set; }

        public static PliegoDto FromPliego(Pliego pliego)
        {
            return new PliegoDto
            {
                Id = pliego.Id,
                Codigo = pliego.Codigo,
                NombreObra = pliego.NombreObra,
                FiscalContrato = pliego.FiscalContrato,
                Email = pliego.Email,
                Telefono = pliego.Telefono,
                FechaHoraEntrega = pliego.FechaHoraEntrega,
                SupervisorSector = pliego.SupervisorSector,
                SupervisorTrabajo = pliego.SupervisorTrabajo,
                TieneVisitaObraMasiva = pliego.TieneVisitaObraMasiva,
                TieneObradores = pliego.TieneObradores,
                TieneMedioElevacion = pliego.TieneMedioElevacion,
                TieneAndamio = pliego.TieneAndamio,
                TieneTecnicoSeguridad = pliego.TieneTecnicoSeguridad,
                TieneGrillaPersonal = pliego.TieneGrillaPersonal,
                TieneFabricacionTallerExterno = pliego.TieneFabricacionTallerExterno,
                TieneDescripcionTecnica = pliego.TieneDescripcionTecnica,
                TieneDocumentacionTecnica = pliego.TieneDocumentacionTecnica,
                FechaHoraLimiteConsulta = pliego.FechaHoraLimiteConsulta,
                ObservacionesGeneracion = pliego.ObservacionesGeneracion,
                DiasEjecucion = pliego.DiasEjecucion,
                ObservacionesCotizacion = pliego.ObservacionesCotizacion,
                ObservacionesCotizacionCondEsp = pliego.ObservacionesCotizacionCondEsp,
                JornadaLaboralDias = pliego.JornadaLaboralDias,
                JornadaLaboralHorasDesde = pliego.JornadaLaboralHorasDesde,
                JornadaLaboralHorasHasta = pliego.JornadaLaboralHorasHasta,
                TieneCondicionesGenerales = pliego.TieneCondicionesGenerales,
                RevisadoPor = pliego.RevisadoPor,
                Multiple = pliego.Multiple,
                FechaAlta = pliego.FechaAlta,
                FechaModificacion = pliego.FechaModificacion,
                Usuario_Id = pliego.Usuario_Id,
                VisitasMasivas = pliego.VisitasMasivas?.Select(v => v.Id).AsEnumerable(),
                Archivos = pliego.Archivos?.Select(a => a.Id).AsEnumerable(),
                RequisitoCiberseguridad = pliego.RequisitoCiberseguridad
            };
        }

        public static explicit operator PliegoDto(Pliego v)
        {
            return FromPliego(v);
        }
    }
}
