using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras.PrecargaSolp
{
    public class SolpSubposicionPrecargadaDto
    {
        public int Numero { get; set; }
        public string Tarea { get; set; }
        public string Codigo { get; set; }
        public decimal? Cantidad { get; set; }
        public int? UnidadId { get; set; }
        public TablaSapDto CuentaMayor { get; set; }
        public TablaSapDto Unidad { get; set; }


        //public int Indice { get; set; }
        //public int Id { get; set; }
        //public int? CodigoServicioSapId { get; set; }
        //public decimal? PrecioBruto { get; set; }
        //public TablaSapDto TipoImputacionValor { get; set; }
        //public ServicioSolpDto CodigoServicioSap { get; set; }
        //public string UnidadComprasDescripcion { get; set; }
        //public string UnidadDescripcion { get; set; }
        //public decimal? CantidadCotizacion { get; set; }
        //public string UnidadCotizacionDescripcion { get; set; }
        //public int? UnidadCotizacionId { get; set; }
        //public string MonedaCotizacionDescripcion { get; set; }
        //public int? MonedaCotizacionId { get; set; }
        //public decimal PrecioSubPosicion { get; set; }
        //public decimal PrecioTotalSubPosicion { get; set; }
        //public string MonedaCotizacionCodigo { get; set; }
        //public int? CotizacionSubPosicionId { get; set; }
        //public int? CodigoSolpServicioSap { get; set; }
        //public int? ServicioSolpCodigo { get; set; }
        //public TablaSapDto MonedaCotizacion { get; set; }
        //public TablaSapDto UnidadMedidaCotizacion { get; set; }
        //public int? CodigoSolp { get; set; }
        //public bool Eliminado { get; set; }
        //public decimal? ValorNeto { get; set; }
    }
}
