using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqVisualizarPendientePagoWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class EcheqVisualizarPendientePagoConsumerMOA : IEcheqVisualizarPendientePagoConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];


        public EcheqVisualizarPendientePagoConsumerMOA()
        {

        }

        public List<EcheqNegocioDto> Request(string proveedor, List<FechaWS> listaFechas, string contrato)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    if (!string.IsNullOrEmpty(contrato))
                    {
                        listaFechas.FirstOrDefault().fechaInicio = DateTime.Now.Date.AddYears(-10);
                    }

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] { };

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] {
                        new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                    }

                    string fechahasta = "";

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                    }


                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPRFC_VISU_PENDIENTE_PAGO()
                    {
                        IM_CONTRATO = contrato ?? "",
                        IM_FECHA = fechas,
                        IM_PROVEEDOR = proveedor
                    };
                    Log.Info($"SAP sin PI Z_MPRFC_VISU_PENDIENTE_PAGO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPRFC_VISU_PENDIENTE_PAGO(request);
                    Log.Info($"SAP sin PI Z_MPRFC_VISU_PENDIENTE_PAGO response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.EX_SALIDA);
                }
                else
                {
                    if (!string.IsNullOrEmpty(contrato))
                    {
                        listaFechas.FirstOrDefault().fechaInicio = DateTime.Now.Date.AddYears(-10);
                    }

                    EcheqVisualizarPendientePagoWebServiceMOA.ZMPES4100[] fechas = new EcheqVisualizarPendientePagoWebServiceMOA.ZMPES4100[] { };

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechas = new EcheqVisualizarPendientePagoWebServiceMOA.ZMPES4100[] {
                        new EcheqVisualizarPendientePagoWebServiceMOA.ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                    }

                    string fechahasta = "";

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                    }

                    SI_MPRFC_VISU_PENDIENTE_PAGOClient service = new SI_MPRFC_VISU_PENDIENTE_PAGOClient();
                    service = new SI_MPRFC_VISU_PENDIENTE_PAGOClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    Log.Info($"SI_MPRFC_VISU_PENDIENTE_PAGO Request: {new { contrato, fechas, proveedor }}");
                    var response = service.SI_MPRFC_VISU_PENDIENTE_PAGO(contrato ?? "", fechas, proveedor);
                    Log.Info($"SI_MPRFC_VISU_PENDIENTE_PAGO Response: {response}");

                    return Map(response);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EcheqNegocioDto> Map(EcheqVisualizarPendientePagoWebServiceMOA.ZMPES6900[] EX_SALIDA)
        {
            return EX_SALIDA.Select(pagosPendientes =>
                new EcheqNegocioDto()
                {
                    Contrato = pagosPendientes.CONTRATO,
                    Pedido = pagosPendientes.PEDIDO,
                    Kilos = pagosPendientes.KILOS,
                    //kILOSFieldSpecified = pagosPendientes.KILOSSpecified,
                    KilosPagados = pagosPendientes.KILOS_PAGADOS,
                    //kILOS_PAGADOSFieldSpecified = pagosPendientes.KILOS_PAGADOSSpecified,
                    Precio = pagosPendientes.PRECIO,
                    //pRECIOFieldSpecified = pagosPendientes.PRECIOSpecified,
                    Moneda = pagosPendientes.MONEDA,
                    MaterialCodigo = Int32.Parse(pagosPendientes.MATERIAL).ToString(),
                    DescripcionMaterial = pagosPendientes.DESC_MATERIAL,
                    Fecha = SAPFormatter.FormatearFecha(pagosPendientes.FECHA),
                    MarcaCheque = string.IsNullOrWhiteSpace(pagosPendientes.ZLSCH) ? false : true,
                    Clasificacion = pagosPendientes.CLASIFICACION,
                    TipoContrato = string.IsNullOrWhiteSpace(pagosPendientes.PEDIDO) ? "A Precio" : "Fijación",
                    Documentos = pagosPendientes.DOCUMENTOS == null ? new List<EcheqLiquidacionDto>() : pagosPendientes.DOCUMENTOS.Select(x => new EcheqLiquidacionDto
                    {
                        Contrato = x.CONTRATO,
                        Pedido = x.PEDIDO,
                        ImporteEnPesos = x.DMBTR,
                        //DMBTRSpecified = x.DMBTRSpecified,
                        Documento = x.DOCUMENTO,
                        Ejercicio = x.EJERCICIO,
                        Fecha = SAPFormatter.FormatearFecha(x.FECHA),
                        Moneda = x.MONEDA,
                        //Sociedad = x.SOCIEDAD,
                        Solapa = x.SOLAPA,
                        ImporteMonedaDocumento = x.WRBTR,
                        //WRBTRSpecified = x.WRBTRSpecified,
                        NumeroCOE = x.XBLNR,
                        //Clasificacion = x.CLASIFICACION
                        MarcaCheque = string.IsNullOrWhiteSpace(x.ZLSCH) ? false : true
                    }).ToList()
                }).ToList();
        }
        public List<EcheqNegocioDto> MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6900[] EX_SALIDA)
        {
            return EX_SALIDA.Select(pagosPendientes =>
                new EcheqNegocioDto()
                {
                    Contrato = pagosPendientes.CONTRATO,
                    Pedido = pagosPendientes.PEDIDO,
                    Kilos = pagosPendientes.KILOS,
                    //kILOSFieldSpecified = pagosPendientes.KILOSSpecified,
                    KilosPagados = pagosPendientes.KILOS_PAGADOS,
                    //kILOS_PAGADOSFieldSpecified = pagosPendientes.KILOS_PAGADOSSpecified,
                    Precio = pagosPendientes.PRECIO,
                    //pRECIOFieldSpecified = pagosPendientes.PRECIOSpecified,
                    Moneda = pagosPendientes.MONEDA,
                    MaterialCodigo = Int32.Parse(pagosPendientes.MATERIAL).ToString(),
                    DescripcionMaterial = pagosPendientes.DESC_MATERIAL,
                    Fecha = SAPFormatter.FormatearFecha(pagosPendientes.FECHA),
                    MarcaCheque = string.IsNullOrWhiteSpace(pagosPendientes.ZLSCH) ? false : true,
                    Clasificacion = pagosPendientes.CLASIFICACION,
                    TipoContrato = string.IsNullOrWhiteSpace(pagosPendientes.PEDIDO) ? "A Precio" : "Fijación",
                    Documentos = pagosPendientes.DOCUMENTOS == null ? new List<EcheqLiquidacionDto>() : pagosPendientes.DOCUMENTOS.Select(x => new EcheqLiquidacionDto
                    {
                        Contrato = x.CONTRATO,
                        Pedido = x.PEDIDO,
                        ImporteEnPesos = x.DMBTR,
                        //DMBTRSpecified = x.DMBTRSpecified,
                        Documento = x.DOCUMENTO,
                        Ejercicio = x.EJERCICIO,
                        Fecha = SAPFormatter.FormatearFecha(x.FECHA),
                        Moneda = x.MONEDA,
                        //Sociedad = x.SOCIEDAD,
                        Solapa = x.SOLAPA,
                        ImporteMonedaDocumento = x.WRBTR,
                        //WRBTRSpecified = x.WRBTRSpecified,
                        NumeroCOE = x.XBLNR,
                        //Clasificacion = x.CLASIFICACION
                        MarcaCheque = string.IsNullOrWhiteSpace(x.ZLSCH) ? false : true
                    }).ToList()
                }).ToList();
        }

    }

    public interface IEcheqVisualizarPendientePagoConsumerMOA
    {
        List<EcheqNegocioDto> Request(string proveedor, List<FechaWS> listaFechas, string contrato);
    }
}
