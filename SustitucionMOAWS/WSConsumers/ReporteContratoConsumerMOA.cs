using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.OrdenCargaVisualizarCliente;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ReporteContratoConsumerMOA : IReporteContratoConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public ReporteContratoWSMOAResponse ReporteContratoExecute(ReporteContratoWSMOARequest request)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    if (request.Fechas != null)
                    {
                        foreach (var fecha in request.Fechas)
                        {
                            fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                            {
                                FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                                FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                            });
                        }
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    var requestFAS = new Z_MPMF_MOAOP_VISUALIZAR_ZFAS()
                    {
                        IM_CLIENTE = request.Cliente,
                        IM_CONTRATO = request.Contrato,
                        IM_CORREDOR = request.Corredor,
                        IM_FECHA = fechasSAPArray,
                        IM_MATERIAL = request.Material ?? "",
                        IM_PENDIENTE = request.Pendiente ?? "",
                        IM_TIPO_CONTRATO = request.TipoContrato ?? ""
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VISUALIZAR_ZFAS request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPMF_MOAOP_VISUALIZAR_ZFAS(requestFAS);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_VISUALIZAR_ZFAS");


                    var result = MapReporteContratoSinPI(response.EX_SALIDA);
                    return result;
                }
                else
                {
                    var service = new SI_MPMF_MOAOP_VISUALIZAR_ZFASClient();

                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    List<OrdenCargaVisualizarCliente.ZMPES4100> fechasSAP = new List<OrdenCargaVisualizarCliente.ZMPES4100>() { };
                    if (request.Fechas != null)
                    {
                        foreach (var fecha in request.Fechas)
                        {
                            fechasSAP.Add(new OrdenCargaVisualizarCliente.ZMPES4100()
                            {
                                FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                                FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                            });
                        }
                    }
                    OrdenCargaVisualizarCliente.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    Log.Info($"SI_MPMF_MOAOP_VISUALIZAR_ZFAS Reporte contrato Request: {new { request.Cliente, request.Contrato, request.Corredor, Fechas = string.Concat(request.Fechas.Select(x => x.fechaInicio.ToShortDateString() + x.fechaFin.ToShortDateString())), request.Material, request.Pendiente, request.TipoContrato }}");
                    var result = service.SI_MPMF_MOAOP_VISUALIZAR_ZFAS(request.Cliente, request.Contrato, request.Corredor, fechasSAPArray, request.Material, request.Pendiente, request.TipoContrato);

                    var response = MapReporteContrato(result);
                    return response;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected virtual ReporteContratoWSMOAResponse MapReporteContrato(OrdenCargaVisualizarCliente.ZMPES6750[] result)
        {
            var response = new ReporteContratoWSMOAResponse();
            var resultados = new List<Result>();

            foreach (var item in result)
            {
                var resultado = new Result()
                {
                    Contrato = item.CONTRATO,
                    PedidoCliente = item.PEDIDO_CLIENTE,
                    PosNr = item.POSNR,
                    Cliente = item.CLIENTE,
                    NombreCliente = item.NOMBRE_CLIENTE,
                    NombreClienteCUIT = $"{item.NOMBRE_CLIENTE} {item.CUIT_CLIENTE}",
                    Corredor = item.CORREDOR,
                    DescripcionMaterial = item.DESC_MATERIAL,
                    KilosTotales = item.KILOS_TOTALES,
                    KilosEntregados = item.KILOS_ENTREGADOS,
                    KilosFacturados = item.KILOS_FACTURADOS,
                    KilosPendienteEntrega = item.KILOS_PEND_ENTREGA,
                    KilosPendienteFactura = item.KILOS_PEND_FACTURA,
                    KilosTotalesStr = SAPFormatter.FormatearCantidad(item.KILOS_TOTALES, "KG"),
                    KilosEntregadosStr = SAPFormatter.FormatearCantidad(item.KILOS_ENTREGADOS, "KG"),
                    KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(item.KILOS_PEND_ENTREGA, "KG"),
                    FechaDesde = item.FECHA_DESDE,
                    FechaHasta = item.FECHA_HASTA,
                    Precio = item.PRECIO,
                    Moneda = item.MONEDA,
                    Motivo = item.MOTIVO,
                    DetalleMotivo = item.DET_MOTIVO,
                    CondicionEntrega = item.CONDICION_ENTREGA,
                    Producto = item.PRODUCTO,
                    PuntoExpedicion = item.PTO_EXPEDICION,
                    TipoContrato = item.TIPO_CONTRATO,
                    ColorProducto = SetearColorProducto(item.PRODUCTO.TrimStart('0')),
                    CodigoProducto = item.PRODUCTO.TrimStart('0'),
                };
                var detalles = new List<Detail>();
                if (item.DETALLE != null)
                {
                    foreach (var detalle in item.DETALLE)
                    {
                        detalles.Add(new Detail()
                        {
                            Pedido = detalle.PEDIDO,
                            Entrega = detalle.ENTREGA,
                            FechaPedido = detalle.FECHA_PEDIDO,
                            FechaCarga = detalle.FECHA_CARGA,
                            CantidadEntregada = detalle.CANTIDAD_ENTREGADA,
                            CantidadEntregadaStr = SAPFormatter.FormatearCantidad(detalle.CANTIDAD_ENTREGADA, "KG"),
                            Remito = detalle.REMITO,
                            Factura = detalle.FACTURA,
                            CantidadFactura = detalle.CANTIDAD_FACTURA,
                            CantidadFacturaStr = SAPFormatter.FormatearCantidad(detalle.CANTIDAD_FACTURA, "KG"),
                            FacturaLegal = detalle.FACTURA_LEGAL,
                            Chasis = detalle.CHASIS,
                            Acoplado = detalle.ACOPLADO,
                            Chofer = detalle.CHOFER,
                            Destinatario = detalle.DESTINATARIO,
                            NombreDestinatario = detalle.NOMBRE_DESTINATARIO,
                            CPE = detalle.CPE
                        });
                    }
                }
                if (detalles.Count > 0)
                {
                    resultado.Detalles = detalles;
                }
                resultados.Add(resultado);
            }

            if (resultados != null)
            {
                if (resultados.Count > 0)
                {

                    response.Resultados = resultados;
                }
            }
            return response;
        }
        protected virtual ReporteContratoWSMOAResponse MapReporteContratoSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6750[] result)
        {
            var response = new ReporteContratoWSMOAResponse();
            var resultados = new List<Result>();

            foreach (var item in result)
            {
                var resultado = new Result()
                {
                    Contrato = item.CONTRATO,
                    PedidoCliente = item.PEDIDO_CLIENTE,
                    PosNr = item.POSNR,
                    Cliente = item.CLIENTE,
                    NombreCliente = item.NOMBRE_CLIENTE,
                    NombreClienteCUIT = $"{item.NOMBRE_CLIENTE} {item.CUIT_CLIENTE}",
                    Corredor = item.CORREDOR,
                    DescripcionMaterial = item.DESC_MATERIAL,
                    KilosTotales = item.KILOS_TOTALES,
                    KilosEntregados = item.KILOS_ENTREGADOS,
                    KilosFacturados = item.KILOS_FACTURADOS,
                    KilosPendienteEntrega = item.KILOS_PEND_ENTREGA,
                    KilosPendienteFactura = item.KILOS_PEND_FACTURA,
                    KilosTotalesStr = SAPFormatter.FormatearCantidad(item.KILOS_TOTALES, "KG"),
                    KilosEntregadosStr = SAPFormatter.FormatearCantidad(item.KILOS_ENTREGADOS, "KG"),
                    KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(item.KILOS_PEND_ENTREGA, "KG"),
                    FechaDesde = item.FECHA_DESDE,
                    FechaHasta = item.FECHA_HASTA,
                    Precio = item.PRECIO,
                    Moneda = item.MONEDA,
                    Motivo = item.MOTIVO,
                    DetalleMotivo = item.DET_MOTIVO,
                    CondicionEntrega = item.CONDICION_ENTREGA,
                    Producto = item.PRODUCTO,
                    PuntoExpedicion = item.PTO_EXPEDICION,
                    TipoContrato = item.TIPO_CONTRATO,
                    ColorProducto = SetearColorProducto(item.PRODUCTO.TrimStart('0')),
                    CodigoProducto = item.PRODUCTO.TrimStart('0'),
                };
                var detalles = new List<Detail>();
                if (item.DETALLE != null)
                {
                    foreach (var detalle in item.DETALLE)
                    {
                        detalles.Add(new Detail()
                        {
                            Pedido = detalle.PEDIDO,
                            Entrega = detalle.ENTREGA,
                            FechaPedido = detalle.FECHA_PEDIDO,
                            FechaCarga = detalle.FECHA_CARGA,
                            CantidadEntregada = detalle.CANTIDAD_ENTREGADA,
                            CantidadEntregadaStr = SAPFormatter.FormatearCantidad(detalle.CANTIDAD_ENTREGADA, "KG"),
                            Remito = detalle.REMITO,
                            Factura = detalle.FACTURA,
                            CantidadFactura = detalle.CANTIDAD_FACTURA,
                            CantidadFacturaStr = SAPFormatter.FormatearCantidad(detalle.CANTIDAD_FACTURA, "KG"),
                            FacturaLegal = detalle.FACTURA_LEGAL,
                            Chasis = detalle.CHASIS,
                            Acoplado = detalle.ACOPLADO,
                            Chofer = detalle.CHOFER,
                            Destinatario = detalle.DESTINATARIO,
                            NombreDestinatario = detalle.NOMBRE_DESTINATARIO,
                            CPE = detalle.CPE
                        });
                    }
                }
                if (detalles.Count > 0)
                {
                    resultado.Detalles = detalles;
                }
                resultados.Add(resultado);
            }

            if (resultados != null)
            {
                if (resultados.Count > 0)
                {

                    response.Resultados = resultados;
                }
            }
            return response;
        }

        public string SetearColorProducto(string codigoProducto)
        {
            var color = "";
            switch (codigoProducto)
            {
                case "99704":
                    return color = "#0B8610";
                case "94705":
                    return color = "#2C83C3";
                case "50866":
                    return color = "#9D9107";
                case "94687":
                    return color = "#1A3B7B";
                case "99059":
                    return color = "#530C80";
                case "99709":
                    return color = "#4AB241";
                case "99591":
                    return color = "#9D9107";
                case "99056":
                    return color = "#530C80";
                case "98855":
                    return color = "#C431C4";
                case "99214":
                    return color = "#B93232";
                case "99098":
                    return color = "#28A089";




            }
            return color;
        }

    }
}
