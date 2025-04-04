using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class DetalleOrdenDeCompraDto
    {
        public string NumeroOrdenDeCompra { get; set; }
        public string FechaCreacion { get; set; }
        public string Proveedor { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public string UsuarioCreador { get; set; }
        public int Moneda_Id { get; set; }
        public string MonedaDescripcion { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal PrecioFinal { get; set; }
        public string TipoPosicionCodigo { get; set; }

        public string TextoDeCabecera { get; set; }
        public string CondicionesDeEntrega { get; set; }
        public string CondicionesDePago { get; set; }
        public string Garantias { get; set; }
        public string Centro { get; set; }
        public string CalleEntrega { get; set; }
        public string CodigoPostal { get; set; }

        public int ItemPorPagina { get; set; }
        public int Pagina { get; set; }
        public int ItemsTotales { get; set; }

        public string NombreProveedor { get; set; }
        public string Cuit { get; set; }
        public string SubjToR { get; set; }

        public bool AdmiteCertificacionesParciales { get; set; } = true;

        /// <summary>
        /// Monto Total with added separators as requested - MMSN-491
        /// </summary>
        public string MontoTotalString { get; set; }

        public List<PosicionDto> Posiciones { get; set; } = new List<PosicionDto>();
    }
}
