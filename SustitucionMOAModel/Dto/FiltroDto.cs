using SustitucionMOAModel.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class FiltroDto
    {
        public int? Pagina { get; set; }
        public int? ItemsPorPagina { get; set; }
        public string Orden { get; set; }
        public string Columna { get; set; }
        public string NroSolp { get; set; }
        public string NroPo { get; set; }
        public string NombrePedido { get; set; }
        public int? EstadoLicitacion { get; set; }
        public int? EstadoCotizacion { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool? Sap { get; set; }
        public bool? Mantenimiento { get; set; }
        public bool? Web { get; set; }
        public bool? RepoAutomatica { get; set; }
        public bool? ContratoMarco { get; set; }
        public string Estados { get; set; }
        public string Usuarios { get; set; }
        public string Centros { get; set; }
        public string GrupoDeCompras { get; set; }
        public bool? Tratada { get; set; }
        public string ClaseDocumento { get; set; }
        public string TipoImputacion { get; set; }
        public string ValorTipoImputacion { get; set; }
        public string CodigoProveedor { get; set; }
        public bool? ListarPendiente { get; set; }
        public bool? EsServicio { get; set; }
        public bool? Agrupada { get; set; }
    }

    public class FiltroServiceDto
    {
        public int? UsuarioId { get; set; }
        public Paginacion Paginacion { get; set; }
        public string CodigoProveedor { get; set; }
        public string NroSolp { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public bool Sap { get; set; }
        public bool Mantenimiento { get; set; }
        public bool Web { get; set; }
        public bool RepoAutomatica { get; set; }
        public bool? ListarPendiente { get; set; }
        public bool? ContratoMarco { get; set; }
        public List<int> Usuarios { get; set; }
        public List<int> Estados { get; set; }
        public List<int> Centros { get; set; }
        public List<int> GrupoDeCompras { get; set; }
        public List<int> ClaseDocumento { get; set; }
        public List<string> TipoImputacion { get; set; }
        public List<int> ValorTipoImputacion { get; set; }
        public string NroPo { get; set; }
        public string[] NombrePedido { get; set; }
        public string Username { get; set; }
        public int? EstadoLicitacion { get; set; }
        public int? EstadoCotizacion { get; set; }
        public bool? Tratada { get; set; }
        public bool EsServicio { get; set; } = true;
        public bool? Agrupada { get; set; }
        public string OrganizacionDeCompra_Id { get; set; }
    }
}
