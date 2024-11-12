using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
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

        public DateTime PlazoDeOferta
        {
            get
            {
                if (PlazoDeOfertaCierre == null && FechaCircular == null)
                {
                    return PlazoDeOfertaOriginal;
                }

                if (PlazoDeOfertaCierre == null)
                {
                    return PlazoDeOfertaCircular.Value;
                }

                if (FechaCircular == null)
                {
                    return PlazoDeOfertaCierre.Value;
                }

                if (PlazoDeOfertaCierre.Value > FechaCircular.Value)
                {
                    return PlazoDeOfertaCierre.Value;
                }

                return PlazoDeOfertaCircular.Value;
            }
        }

        public string Observaciones { get; set; }
        public string PlazoDeOfertaFormateado { get { return PlazoDeOferta.ToString("dd/MM/yyyy HH:mm"); } }

        public string Estado
        {
            get { return PlazoDeOferta >= DateTime.Now ? "Abierto" : "Cerrado"; }
        }

        public int Estado_Id
        {
            get { return PlazoDeOferta >= DateTime.Now ? 1 : 2; }
        }

        public string EstadoColor
        {
            get { return PlazoDeOferta >= DateTime.Now ? "Green" : "Red"; }
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
        public string ObservacionCotizacion { get; set; }
        public IEnumerable<ArchivoDto> ArchivosPaso4Cotizacion { get; set; }

        public bool SolpModificada { get; set; }
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

        private string razonSocial;
        private bool razonSocialOverride;
        public string RazonSocial
        {
            get
            {
                if (razonSocialOverride) { return razonSocial; }
                return DatosProveedor?.RazonSocial ?? CuitRegistroUsuario;
            }
            set
            {
                razonSocial = value;
                razonSocialOverride = true;
            }
        }

        public int Id { get; set; }

        private string cuit;
        private bool cuitOverride;
        public string CUIT
        {
            get
            {
                if (cuitOverride) { return cuit; }
                return DatosProveedor?.CUIT ?? CuitRegistroUsuario;
            }
            set
            {
                cuit = value;
                cuitOverride = true;
            }
        }

        private string mail;
        private bool mailOverride;
        public string Mail
        {
            get
            {
                if (mailOverride) { return mail; }
                return DatosProveedor?.Mail ?? CuitRegistroUsuario;
            }
            set
            {
                mail = value;
                mailOverride = true;
            }
        }

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

        private EstadoAprobacion? proveedorEstadoAprobacion;
        private bool proveedorEstadoAprobacionOverride;
        public EstadoAprobacion? ProveedorEstadoAprobacion
        {
            get
            {
                if (proveedorEstadoAprobacionOverride) { return proveedorEstadoAprobacion; }
                return DatosProveedor?.EstadoAprobacion;
            }
            set
            {
                proveedorEstadoAprobacion = value;
                proveedorEstadoAprobacionOverride = true;
            }
        }

        public string MensajeAdjudicar { get; set; }
        public bool ValidacionCircularSolicitante { get; set; }
        public string ObservacionNoCumple { get; set; }
        public DateTime PlazoDeOferta { get; set; }
        public DateTime PlazoDeOfertaOriginal { get; set; }
        public DateTime? PlazoDeOfertaCircular { get; set; }
        public DateTime? PlazoDeOfertaCierre { get; set; }
        public DateTime? FechaCircular { get; set; }

        private string codigoProveedor;
        private bool codigoProveedorOverride;
        public string CodigoProveedor
        {
            get
            {
                if (codigoProveedorOverride) { return codigoProveedor; }
                return DatosProveedor?.CodigoProveedor ?? "";
            }
            set
            {
                codigoProveedor = value;
                codigoProveedorOverride = true;
            }
        }

        public string THCategoria { get; set; }
        public bool? VisibleSolicitante { get; set; }
        public bool Deshabilitado { get; set; }
        public List<MonedaTotalDto> TotalesPorMoneda { get; set; }

        public Proveedor DatosProveedor { private get; set; }
        public string CuitRegistroUsuario { private get; set; }
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