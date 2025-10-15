using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PagosWebServiceMOA;
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
    public class PagosConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    var request = new Z_MPMF_MOAOP_PAGOS_CTA_CTE()
                    {
                        PE_PROVEEDOR = proveedor,
                        PE_SOLAPA = "P",
                        T_FECHA_IN = fechasSAPArray,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PAGOS_CTA_CTE request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PAGOS_CTA_CTE(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PAGOS_CTA_CTE response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_PAGOS_CTA_CTEClient service = new SI_MPMF_MOAOP_PAGOS_CTA_CTEClient();
                    PagosWebServiceMOA.ZMPES5310[] pagos = new PagosWebServiceMOA.ZMPES5310[] { };
                    List<PagosWebServiceMOA.ZMPES4100> fechasSAP = new List<PagosWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new PagosWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    PagosWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    PagosWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_PAGOS_CTA_CTE(proveedor, "P", ref fechasSAPArray, out pagos);
                    return Map(pagos, error);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        protected virtual object MapSinPI(Z_MPMF_MOAOP_PAGOS_CTA_CTEResponse response)
        {
            PagosWSMOAReponse result = new PagosWSMOAReponse();
            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5310 pagoInfo in response.T_SALIDA)
            {
                result.pagos.Add(new PagoView()
                {
                    proveedor = pagoInfo.PROVEEDOR,
                    facreditacion = SAPFormatter.FormatearFecha(pagoInfo.FACREDITACION),
                    facreditacionDate = SAPFormatter.GetDateTime(pagoInfo.FACREDITACION),
                    idPago = pagoInfo.ID_PAGO,
                    montoString = SAPFormatter.FormatearMonto(pagoInfo.MONTO, "$"),
                    monto = pagoInfo.MONTO,
                    contrato = pagoInfo.CONTRATO,
                    contrProv = pagoInfo.CONTR_PROV,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    fechaPagoDate = SAPFormatter.GetDateTime(pagoInfo.FECHA_PAGO),
                    comprobante = pagoInfo.COMPROBANTE,
                    tipoComprobante = pagoInfo.TIPO_COMPROBANTE,
                    concepto = pagoInfo.CONCEPTO,
                    gjahr = pagoInfo.GJAHR,
                    iva = pagoInfo.IVA,
                    ivaString = SAPFormatter.FormatearMonto(pagoInfo.IVA, "$"),
                    pdf = pagoInfo.PDF,
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA,
                    totalMercaderiaString = SAPFormatter.FormatearMonto(pagoInfo.TOTAL_MERCADERIA, "$"),
                    retencion = pagoInfo.RETENCION,
                    retencionString = SAPFormatter.FormatearMonto(pagoInfo.RETENCION, "$"),
                    vbeln = pagoInfo.VBELN,
                    vblnr = pagoInfo.VBLNR,
                    witht = pagoInfo.WITHT
                });
            }

            return result;
        }
        protected virtual object Map(PagosWebServiceMOA.ZMPES5310[] pagos, PagosWebServiceMOA.ZMPES4910 error)
        {
            PagosWSMOAReponse result = new PagosWSMOAReponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (PagosWebServiceMOA.ZMPES5310 pagoInfo in pagos)
            {
                result.pagos.Add(new PagoView()
                {
                    proveedor = pagoInfo.PROVEEDOR,
                    facreditacion = SAPFormatter.FormatearFecha(pagoInfo.FACREDITACION),
                    facreditacionDate = SAPFormatter.GetDateTime(pagoInfo.FACREDITACION),
                    idPago = pagoInfo.ID_PAGO,
                    montoString = SAPFormatter.FormatearMonto(pagoInfo.MONTO, "$"),
                    monto = pagoInfo.MONTO,
                    contrato = pagoInfo.CONTRATO,
                    contrProv = pagoInfo.CONTR_PROV,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    fechaPagoDate = SAPFormatter.GetDateTime(pagoInfo.FECHA_PAGO),
                    comprobante = pagoInfo.COMPROBANTE,
                    tipoComprobante = pagoInfo.TIPO_COMPROBANTE,
                    concepto = pagoInfo.CONCEPTO,
                    gjahr = pagoInfo.GJAHR,
                    iva = pagoInfo.IVA,
                    ivaString = SAPFormatter.FormatearMonto(pagoInfo.IVA, "$"),
                    pdf = pagoInfo.PDF,
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA,
                    totalMercaderiaString = SAPFormatter.FormatearMonto(pagoInfo.TOTAL_MERCADERIA, "$"),
                    retencion = pagoInfo.RETENCION,
                    retencionString = SAPFormatter.FormatearMonto(pagoInfo.RETENCION, "$"),
                    vbeln = pagoInfo.VBELN,
                    vblnr = pagoInfo.VBLNR,
                    witht = pagoInfo.WITHT
                });
            }
            
            return result;
        }
    }

    public class PagosExcelConsumerMOA : PagosConsumerMOA
    {

        protected override object Map(PagosWebServiceMOA.ZMPES5310[] pagos, PagosWebServiceMOA.ZMPES4910 error)
        {
            PagosExcelWSMOAReponse result = new PagosExcelWSMOAReponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (PagosWebServiceMOA.ZMPES5310 pagoInfo in pagos)
            {
                result.pagos.Add(new Pago()
                {
                    proveedor = pagoInfo.PROVEEDOR,
                    facreditacion = SAPFormatter.FormatearFecha(pagoInfo.FACREDITACION),
                    idPago = pagoInfo.ID_PAGO,
                    moneda = "$",
                    monto = pagoInfo.MONTO,
                    contrato = pagoInfo.CONTRATO,
                    contrProv = pagoInfo.CONTR_PROV,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    comprobante = pagoInfo.COMPROBANTE,
                    tipoComprobante = pagoInfo.TIPO_COMPROBANTE,
                    concepto = pagoInfo.CONCEPTO,
                    iva = pagoInfo.IVA,
                    retencion = pagoInfo.RETENCION,
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA,                    
                });
            }

            return result;
        }
        protected override object MapSinPI(Z_MPMF_MOAOP_PAGOS_CTA_CTEResponse response)
        {
            PagosExcelWSMOAReponse result = new PagosExcelWSMOAReponse();
            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5310 pagoInfo in response.T_SALIDA)
            {
                result.pagos.Add(new Pago()
                {
                    proveedor = pagoInfo.PROVEEDOR,
                    facreditacion = SAPFormatter.FormatearFecha(pagoInfo.FACREDITACION),
                    idPago = pagoInfo.ID_PAGO,
                    moneda = "$",
                    monto = pagoInfo.MONTO,
                    contrato = pagoInfo.CONTRATO,
                    contrProv = pagoInfo.CONTR_PROV,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    comprobante = pagoInfo.COMPROBANTE,
                    tipoComprobante = pagoInfo.TIPO_COMPROBANTE,
                    concepto = pagoInfo.CONCEPTO,
                    iva = pagoInfo.IVA,
                    retencion = pagoInfo.RETENCION,
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA,
                });
            }

            return result;
        }
    }
}
