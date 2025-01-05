using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras.PrecargaSolp
{
    public class SolpPosicionPrecargadaDto
    {
        public int Indice { get; set; }

        public int TipoPosicionId { get; set; }
        public TablaGeneralDto TipoPosicion { get; set; }
        
        public string Codigo { get; set; }
        
        public int? TipoImputacionId { get; set; }
        public TablaGeneralDto TipoImputacion { get; set; }

        public string Tarea { get; set; }

        public int? MonedaId { get; set; }
        public TablaSapDto Moneda { get; set; }

        public DateTime? FechaEntregaServicio { get; set; }

        public int? GrupoComprasId { get; set; }
        public TablaSapDto GrupoCompras { get; set; }

        public int? GrupoArticuloId { get; set; }
        public TablaSapDto GrupoArticulo { get; set; }

        public int? CentroId { get; set; }
        public TablaSapDto Centro { get; set; }

        public int? AlmacenId { get; set; }
        public TablaSapDto Almacen { get; set; }

        public decimal? Cantidad { get; set; }

        public int? UnidadId { get; set; }
        public TablaSapDto Unidad { get; set; }

        public TablaSapDto CuentaMayor { get; set; }

        public List<SolpSubposicionPrecargadaDto> Subposiciones { get; set; }


        //public DateTime? FechaLiberacion { get; set; }
        //public int? PlazoEntrega { get; set; }
        //public string CentroCodigo { get; set; }
        //public string NombreEntrega { get; set; }
        //public string CalleEntrega { get; set; }
        //public string NumeroEntrega { get; set; }
        //public string CpEntrega { get; set; }
        //public string PaisEntrega { get; set; }
        //public string Solicitante { get; set; }
        //public string NroNecesidad { get; set; }
        //public string TextoSuministro { get; set; }
        //public string Motivo { get; set; }
        //public string Modelo { get; set; }
        //public bool Estado { get; set; }
        //public int? CodigoServicioSapId { get; set; }
        //public int? CodigoMaterialSapId { get; set; }
        //public decimal? PrecioBruto { get; set; }
        //public bool? EsConcluido { get; set; }

        ////Contrato Marco
        //public string NumeroContratoSuperior { get; set; }
        //public string NumeroPosicionContratoSuperior { get; set; }
        //public string NombreProveedor { get; set; }
        //public string ProveedorFijo { get; set; }
        //public string OrganizacionCompras { get; set; }
        //public string NumeroPedido { get; set; }

        //public ServicioSolpDto CodigoServicioSap { get; set; }
        //public MaterialSolpDto CodigoMaterialSap { get; set; }
        //public TablaSapDto TipoImputacionValor { get; set; }
        //public List<SolpProveedorDto> Proveedores { get; set; }
        //public ProvinciaDto Provincia { get; set; }
        //public string GrupoComprasDescripcion { get; set; }
        //public string CentroComprasDescripcion { get; set; }
        //public bool TieneCotizacion { get; set; }
        //public string AlmacenComprasDescripcion { get; set; }
        //public string UnidadComprasDescripcion { get; set; }
        //public string MonedaSolpDescripcion { get; set; }
        //public IEnumerable<SolpProveedorDto> ProveedoresCompras { get; set; }
        //public IEnumerable<SolpSubposicionDto> SubposicionesCompras { get; set; }
        //public string MaterialComprasCodigo { get; set; }
        //public int Id { get; set; }
        //public DateTime? FechaOferta { get; set; }
        //public CotizacionPosicionDto CotizacionPosicion { get; set; }
        //public decimal? CantidadPendiente { get; set; }
        //public decimal? CantidadAdjudicacion { get; set; }
        //public bool AdjudicacionCompleta { get; set; }
        //public AdjudicacionPosicion AdjudicacionPosicion { get; set; }
        //public string SolpTipo { get; set; }
        //public int Solp_Id { get; set; }
        //public decimal? CantidadAdjudicada { get; set; }
        //public string GrupoComprasCodigo { get; set; }
        //public List<TablaSapDto> UnidadesDeMedida { get; set; }
        //public string NroSolp { get; set; }
        //public string TipoPosicionCodigo { get; set; }
        //public string CentroCodigoSap { get; set; }
        //public string GrupoComprasCodigoSap { get; set; }
        //public string MaterialDescripcion { get; set; }
    }
}
