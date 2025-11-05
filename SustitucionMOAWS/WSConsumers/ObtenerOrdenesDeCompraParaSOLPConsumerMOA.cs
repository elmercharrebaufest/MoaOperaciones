using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.OrdenesDeCompraParaSolpWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenesDeCompraParaSOLPConsumerMOA : IObtenerOrdenesDeCompraParaSOLPConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public ObtenerOrdenesDeCompraParaSOLPConsumerMOA(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<OrdenDeCompraSAPDto> Request(string SOLPED, string POSICION)
        {
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var request = new Z_MMRFC_OC_PARA_SOLP()
                {
                    IM_POSICION = POSICION,
                    IM_SOLPED = SOLPED,
                };
                Log.Info($"SAP sin PI Z_MMRFC_OC_PARA_SOLP request");
                Log.Info(request.ToXml());

                var response = agent.Z_MMRFC_OC_PARA_SOLP(request);

                SapLogHelper.LogResponse(response.ToXml(), "Z_MMRFC_OC_PARA_SOLP");
                return MapSinPI(response);
            }
            else
            {
                Z_MMRFC_OC_PARA_SOLPPortTypeClient service;
                var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=Z_MMRFC_OC_PARA_SOLP&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
                service = new Z_MMRFC_OC_PARA_SOLPPortTypeClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                OrdenesDeCompraParaSolpWebServiceMOA.ZMMES0027[] lista = service.Z_MMRFC_OC_PARA_SOLP(POSICION, SOLPED, out string exito, out string resultado);
                return Map(lista, exito, resultado);
            }
        }

        private List<OrdenDeCompraSAPDto> Map(OrdenesDeCompraParaSolpWebServiceMOA.ZMMES0027[] items, string exito, string resultado)
        {
            List<OrdenDeCompraSAPDto> result = new List<OrdenDeCompraSAPDto>();
            if (exito != "200")
            {
                if (resultado == "No hay Órdenes de Compra para la Solicitud de Pedido")
                {
                    return result;
                }
                else throw new ValidationCustomException(resultado);
            }

            var monedasConversion = repositorio.Listar<MonedaConversion, MonedaConversionDto>(x => new MonedaConversionDto
            {
                CantidadDecimal = x.CantidadDecimal,
                MonedaCodigo = x.MonedaCodigo
            });
            var agrupado = items.GroupBy(a => a.EBELN).ToList();
            foreach (var item in agrupado)
            {
                result.Add(new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = item.Key,
                        CodigoProveedor = item.First().LIFNR,
                        RazonSocialProveedor = item.First().NAME1,
                        CUITProveedor = item.First().STCD1,
                        Moneda = item.First().WAERS,
                        MontoTotal = CorregirImporte(item.Sum(a => a.NETWR), item.First().WAERS, monedasConversion),
                        CreadoPor = item.First().ERNAM,
                        ClaseDocumento = item.First().BSART,
                        FechaCreacion = SAPFormatter.GetDateTime(item.First().AEDAT),
                        Tipo = item.First().PSTYP == "0" ? "Materiales" : "Servicio",
                        TipoDocCompras = item.First().BSTYP,
                        MontoBruto = CorregirImporte(item.Sum(a => a.BRTWR), item.First().WAERS, monedasConversion),
                        EstadoLiberacionCodigo = item.First().FRGKE,
                    }
                });
            }
            return result;
        }
        private List<OrdenDeCompraSAPDto> MapSinPI(WS_GAQ_sin_PI_DIRECT_COMPRAS.Z_MMRFC_OC_PARA_SOLPResponse response)
        {
            List<OrdenDeCompraSAPDto> result = new List<OrdenDeCompraSAPDto>();
            if (response.EX_EXITO != "200")
            {
                if (response.EX_RETURN == "No hay Órdenes de Compra para la Solicitud de Pedido")
                {
                    return result;
                }
                else throw new ValidationCustomException(response.EX_RETURN);
            }

            var monedasConversion = repositorio.Listar<MonedaConversion, MonedaConversionDto>(x => new MonedaConversionDto
            {
                CantidadDecimal = x.CantidadDecimal,
                MonedaCodigo = x.MonedaCodigo
            });
            var agrupado = response.EX_DATOS.GroupBy(a => a.EBELN).ToList();
            foreach (var item in agrupado)
            {
                result.Add(new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = item.Key,
                        CodigoProveedor = item.First().LIFNR,
                        RazonSocialProveedor = item.First().NAME1,
                        CUITProveedor = item.First().STCD1,
                        Moneda = item.First().WAERS,
                        MontoTotal = CorregirImporte(item.Sum(a => a.NETWR), item.First().WAERS, monedasConversion),
                        CreadoPor = item.First().ERNAM,
                        ClaseDocumento = item.First().BSART,
                        FechaCreacion = SAPFormatter.GetDateTime(item.First().AEDAT),
                        Tipo = item.First().PSTYP == "0" ? "Materiales" : "Servicio",
                        TipoDocCompras = item.First().BSTYP,
                        MontoBruto = CorregirImporte(item.Sum(a => a.BRTWR), item.First().WAERS, monedasConversion),
                        EstadoLiberacionCodigo = item.First().FRGKE,
                    }
                });
            }
            return result;
        }

        private decimal CorregirImporte(decimal importe, string moneda, List<MonedaConversionDto> monedas)
        {
            int cantidadDecimal = monedas.Find(m => m.MonedaCodigo == moneda)?.CantidadDecimal ?? 2;
            decimal factorCorreccion = (decimal)Math.Pow(10, cantidadDecimal - 2);
            decimal importeCorregido = importe / factorCorreccion;
            return importeCorregido;
        }
    }

    public interface IObtenerOrdenesDeCompraParaSOLPConsumerMOA
    {
        List<OrdenDeCompraSAPDto> Request(string SOLPED, string POSICION);
    }
}