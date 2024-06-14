using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaDto
    {
        public int Id { get; set; }
        public int Solp_Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }

        private DateTime pPlazoDeOferta;
        public DateTime PlazoDeOferta
        {
            get
            {
                return (PlazoDeOfertaCierre == null && FechaCircular == null) ? PlazoDeOfertaOriginal :
                  PlazoDeOfertaCierre == null ? PlazoDeOfertaCircular.Value :
                  FechaCircular == null ? PlazoDeOfertaCierre.Value :
                  PlazoDeOfertaCierre.Value > FechaCircular.Value ? PlazoDeOfertaCierre.Value : PlazoDeOfertaCircular.Value;
            }
            set { pPlazoDeOferta = value; }
        }
        public string Observaciones { get; set; }
        public string PlazoDeOfertaFormateado { get { return PlazoDeOferta.ToString("dd/MM/yyyy HH:mm"); } }

        private string pEstado;

        public string Estado
        {
            get { return PlazoDeOferta >= DateTime.Now ? "Abierto" : "Cerrado"; }
            set { pEstado = value; }
        }

        private int pEstado_Id;

        public int Estado_Id
        {
            get { return PlazoDeOferta >= DateTime.Now ? 1 : 2; }
            set { pEstado_Id = value; }
        }

        private string pEstadoColor;

        public string EstadoColor
        {
            get { return PlazoDeOferta >= DateTime.Now ? "Green" : "Red"; }
            set { pEstadoColor = value; }
        }

        public List<PeticionDeOfertaUsarioDto> Usuarios { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string FechaEntregaFormateado { get; set; }
        public IQueryable<CircularDto> CircularDto { get; set; }
        public SolpDto SolpDto { get; set; }
        public string NroSolp { get; set; }
        public string NombreDeObra { get; set; } //nombre de pedido
        public string UsuarioCreador { get; set; }
        public int ItemPorPagina { get; set; }
        public int Pagina { get; set; }
        public int ItemsTotales { get; set; }
        public string TieneVisitaObra { get; set; }
        public bool TieneVisitaObraMasiva { get; set; }
        public IQueryable<CotizacionDto> CotizacionEstado { get; set; }
        public string CotizacionEstadoDescripcion { get; set; }
        public DateTime? PlazoDeOfertaCircular { get; set; }
        public int CotizacionEstado_Id { get; set; }
        public CircularDto Circular { get; set; }
        public IEnumerable<PeticionDeOfertaSolpPosicionDto> PeticionDeOfertaPosicion { get; set; }
        public int CotizacionId { get; set; }
        public string TipoPosicionCodigo { get; set; }
        public CotizacionDto Cotizacion { get; set; }
        public string ObservacionEconomica { get; set; }
        public string ObservacionTecnica { get; set; }
        public bool? RespetaMateriales { get; set; }
        public bool? RespetaServicios { get; set; }
        public string FechaCreacionFormateada { get; set; }
        public DateTime FechaCreacionSolp { get; set; }
        public string FechaCreacionFormateadaSolp { get; set; }
        public IEnumerable<DateTime?> VisitasMasivas { get; set; }
        public bool PersonalHoras { get; set; }
        public string Texto { get; set; }
        public bool EstaLiberado { get; set; }
        public string PlazoDeOfertaEstado { get; set; }
        public bool? RegistroInfo { get; set; }
        public bool? Adicional { get; set; }
        public bool? TieneAdjudicacion { get; set; }
        public string NroOrdenDeCompraAdicional { get; set; }
        public DateTime PlazoDeOfertaOriginal { get; set; }
        public DateTime? PlazoDeOfertaCierre { get; set; }
        public DateTime? FechaCircular { get; set; }
        public decimal? PorcentajeDeHoras { get; set; }
        public bool? AdjuntoPliego { get; set; }
        public bool? Urgencia { get; set; }
        public bool? TieneVisitaObraBool { get; set; }
        public bool RevisionFinalizada { get; set; }
        public bool VerBotonVerPrecio { get; set; }
        public bool RecotizacionEconomica { get; set; }
        public PeticionDeOfertaRevisionTecnicaDto RevisionTecnica { get; set; }
        public List<PeticionDeOfertaUsuarioAdicionalDto> UsuariosAdicionales { get; set; }
        public bool PideDescripcionTecnica { get; set; }
        public bool PideDocumentacionTecnica { get; set; }
        public int? RevisionTecnicaId { get; set; }
        public bool? TrabajoHecho { get; set; }
        public IEnumerable<string> NrosSolp { get; set; }
        public bool TienePosicionesEliminadas { get; set; }
        public bool VerCotizar { get; set; }
        public bool EsNuevaCotizacion { get; set; }
        public string ObservacionTecnicaOriginal { get; set; }
        public string ObservacionEconomicaOriginal { get; set; }
        public bool ChatSinLeer { get; set; }
        public string RolUsuario { get; set; }
        public bool CondEspProveedorAsignado { get; set; }
        public bool? RequisitoCiberseguridad { get; set; }
    }

    public class PeticionDeOfertaSolpPosicionDto
    {
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int SolpPosicion_Id { get; set; }
        public SolpPosicionDto Posicion { get; set; }
        public SolpPosicionDto Posiciones { get; set; }
        public int SolpId { get; set; }
        public SolpPosicionDto PosicionPeticion { get; set; }
        public bool EstaEliminado { get; set; }
    }

    public class PeticionDeOfertaUsarioDto
    {
        public int UsuarioId { get; set; }
        public string RazonSocial { get; set; }
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string Mail { get; set; }
        public bool? PropuestaTecnicaAprobada { get; set; }
        public bool? RealizoVisita { get; set; }
        public CotizacionDto Cotizacion { get; set; }
        public bool CircularSinLeer { get; set; }
        public IEnumerable<int> CircularesSinLeer { get; set; }
        public string EstadoVisita { get; set; }
        public string EstadoVisitaColor { get; set; }
        public string EstadoPropuestaTecnica { get; set; }
        public string EstadoPropuestaTecnicaColor { get; set; }
        public string CotizacionEstado { get; set; }
        public bool VerAdjudicar { get; set; }
        public bool VerImportes { get; set; }
        public bool EstaHabilitado { get; set; }
        public string MensajeAdjudicar { get; set; }
        public bool ValidacionCircularSolicitante { get; set; }
        public string ObservacionNoCumple { get; set; }
        public DateTime PlazoDeOferta { get; set; }
        public DateTime PlazoDeOfertaOriginal { get; set; }
        public DateTime? PlazoDeOfertaCircular { get; set; }
        public DateTime? PlazoDeOfertaCierre { get; set; }
        public DateTime? FechaCircular { get; set; }
        public string CodigoProveedor { get; set; }
        public string THCategoria { get; set; }
        public bool? VisibleSolicitante { get; set; }
        public bool Deshabilitado { get; set; }
    }

    public class PeticionDeOfertaCierreDto
    {
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Observacion { get; set; }
    }

    public class PeticionDeOfertaRevisionTecnicaDto
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool RecotizacionEconomica { get; set; }
        public bool? ModificacionSolp { get; set; }
        public string ObservacionRecotizacion { get; set; }
        public bool Finalizada { get; set; }
    }

    public class PeticionDeOfertaUsuarioAdicionalDto
    {

        public PeticionDeOfertaUsuarioAdicionalDto()
        {
        }

        public PeticionDeOfertaUsuarioAdicionalDto(PeticionDeOfertaUsuarioAdicional entidad)
        {
            UsuarioId = entidad.Usuario_Id;
            Id = entidad.Id;
            RazonSocial = entidad.Usuario.ObtenerRazonSocial();
            CUIT = entidad.Usuario.CUITRegistro;
            Mail = entidad.Usuario.Mail;
        }

        public int UsuarioId { get; set; }
        public string RazonSocial { get; set; }
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string Mail { get; set; }
    }
}