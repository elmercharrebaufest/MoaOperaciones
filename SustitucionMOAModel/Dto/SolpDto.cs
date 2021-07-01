using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class SolpDto
    {
        public UsuarioDto UsuarioActual { get; set; }
        public int? Id { get; set; }
        public string NombreDeObra { get; set; }
        public string FiscalContrato { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime? FechaHoraEntrega { get; set; }
        public string SupervisorSector { get; set; }
        public string SupervisorTrabajo { get; set; }
        public List<VisitaObraDto> VisitasObraMasiva { get; set; }
        public bool TieneVisitaObra { get; set; }
        public bool TieneVisitaObraMasiva { get; set; }
        public bool TieneObradores { get; set; }
        public bool TieneMedioElevacion { get; set; }
        public bool TieneTecnicoSeguridad { get; set; }
        public bool TieneDescripcionTecnica { get; set; }
        public bool TieneDocumentacionTecnica { get; set; }
        public DateTime? FechaHoraLimiteConsulta { get; set; }
        public string ObservacionesGeneracion { get; set; }
        public string EspecificacionesTecnicas { get; set; }
        public int? DiasEjecucion { get; set; }
        public string ObservacionesCotizacion { get; set; }
        public List<DayOfWeek> JornadaLaboral { get; set; }
        public DateTime? JornadaLaboralDesde { get; set; } 
        public DateTime? JornadaLaboralHasta { get; set; }
        public TablaSapDto ClaseDocumento { get; set; }
        public int? ClaseDocumentoId { get; set; }
        public List<ArchivoDto> Adjuntos { get; set; }
        public string NroSolp { get; set; }
        public int? EstadoSolpSap_Id { get; set; }
        public int? EstadoDocumento_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public TablaEstadoDto EstadoDocumento { get; set; }
        public bool VincularPliego { get; set; }
        public TablaSapDto EstadoSolpSap { get; set; }




        public SolpDto() {}

    }

    public class VisitaObraDto
    {
        public string Codigo { get; set; }
        public DateTime FechaHora { get; set; }

        public VisitaObraDto() {}
    }




}
