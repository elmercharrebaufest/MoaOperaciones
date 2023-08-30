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
        public DateTime PlazoDeOferta { get; set; }
        public string Observaciones { get; set; }
        public string PlazoDeOfertaFormateado { get { return PlazoDeOferta.ToString("dd/MM/yyyy"); } }
        
        //public string Estado { get { return PlazoDeOferta >= DateTime.Now.Date ? "Abierto" : "Cerrado"; } }
        private string pEstado;

        public string Estado
        {
            get { return PlazoDeOferta >= DateTime.Now.Date ? "Abierto" : "Cerrado"; }
            set { pEstado = value; }
        }

        //public int Estado_Id { get { return PlazoDeOferta >= DateTime.Now.Date ? 1 : 2; } }
        private int pEstado_Id;

        public int Estado_Id
        {
            get { return PlazoDeOferta >= DateTime.Now.Date ? 1 : 2; }
            set { pEstado_Id = value; }
        }

        //public string EstadoColor { get { return PlazoDeOferta >= DateTime.Now.Date ? "Green" : "Red"; } }
        private string pEstadoColor;

        public string EstadoColor
        {
            get { return PlazoDeOferta >= DateTime.Now.Date ? "Green" : "Red"; }
            set { pEstadoColor = value; }
        }

        public List<PeticionDeOfertaUsarioDto> Usuarios {get; set;}
        public DateTime? FechaEntrega { get; set; }
        public string FechaEntregaFormateado { get; set; }
        public IQueryable<CircularDto> CircularDto { get; set; }
        public IQueryable<SolpDto> SolpDto { get; set; }
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
        public DateTime PlazoDeOferta { get; set; }
        public string CotizacionEstado { get; set; }
        public bool VerAdjudicar { get; set; }
        public bool EstaHabilitado { get; set; }
        public string MensajeAdjudicar { get; set; }
        public bool ValidacionCircularSolicitante { get; set; }
        public string ObservacionNoCumple { get; set; }
    }

}