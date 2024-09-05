using SustitucionMOAModel.Enums;
using System.Collections.Generic;

namespace SustitucionMOAModel.Models.WSMapMOA.OrdenCarga
{
    public class OrdenCargaVisualizarClienteWSMOAResponse
    {
        public List<Result> Resultados { get; set; }

        public OrdenCargaVisualizarClienteWSMOAResponse()
        {
            Resultados = new List<Result>();
        }
    }

    public class Result
    {
        public string Contrato { get; set; }

        public string PedidoCliente { get; set; }

        public string PosNr { get; set; }

        public string Cliente { get; set; }

        public string NombreCliente { get; set; }

        public string CuitCliente { get; set; }

        public string Corredor { get; set; }

        public string DescripcionMaterial { get; set; }

        public decimal KilosTotales { get; set; }

        public string KilosTotalesStr { get; set; }
        //public bool kILOS_TOTALESFieldSpecified

        public decimal KilosEntregados { get; set; }

        public string KilosEntregadosStr { get; set; }
        //private bool kILOS_ENTREGADOSFieldSpecified

        public decimal KilosFacturados { get; set; }

        public string KilosFacturadosStr { get; set; }
        //private bool kILOS_FACTURADOSFieldSpecified

        public decimal KilosPendienteEntrega { get; set; }

        public string KilosPendienteEntregaStr { get; set; }
        //private bool kILOS_PEND_ENTREGAFieldSpecified

        public decimal KilosPendienteFactura { get; set; }

        //private bool kILOS_PEND_FACTURAFieldSpecified

        public string FechaDesde { get; set; }


        public string FechaHasta { get; set; }

        public decimal Precio { get; set; }

        //private bool pRECIOFieldSpecified

        public string Moneda { get; set; }

        public string Motivo { get; set; }

        public string DetalleMotivo { get; set; }

        public string CondicionEntrega { get; set; }

        public string Producto { get; set; }

        public string PuntoExpedicion { get; set; }

        public TipoContratoFAS TipoContrato { get; set; }

        public decimal PrecioFlete { get; set; }

        public List<Detail> Detalles { get; set; }

        public Result()
        {
            Detalles = new List<Detail>();
        }
    }

    public class Detail
    {
        public string Pedido { get; set; }

        public string Entrega { get; set; }

        public string FechaPedido { get; set; }

        public string FechaCarga { get; set; }

        public decimal CantidadEntregada { get; set; }

        //public bool cANTIDAD_ENTREGADAFieldSpecified

        public string Remito { get; set; }

        public string Factura { get; set; }

        public decimal CantidadFactura { get; set; }

        //public bool cANTIDAD_FACTURAFieldSpecified

        public string FacturaLegal { get; set; }

        public string Chasis { get; set; }

        public string Acoplado { get; set; }

        public string Chofer { get; set; }

        public string Destinatario { get; set; }

        public string NombreDestinatario { get; set; }

        public decimal KilosEntrega { get; set; }
    }
}
