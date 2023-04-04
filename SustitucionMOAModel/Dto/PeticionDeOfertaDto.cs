using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Estado { get { return PlazoDeOferta >= DateTime.Now.Date ? "Abierto" : "Cerrado"; } }
        public int Estado_Id { get { return PlazoDeOferta >= DateTime.Now.Date ? 1 : 2; } }
        public string EstadoColor { get { return PlazoDeOferta >= DateTime.Now.Date ? "Green" : "Red"; } }
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
    }


    public class PeticionDeOfertaUsarioDto
    {
        public int UsuarioId { get; set; }
        public string RazonSocial { get; set; }
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string Mail { get; set; }
    }
}
