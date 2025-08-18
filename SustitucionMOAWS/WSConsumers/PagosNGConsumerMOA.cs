using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PagosNGWebServiceMOA;
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
    public class PagosNGConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        public object request(string proveedor, List<FechaWS> fechas, string sociedad)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100 fechaSAP = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100();
                    if (fechas.Count > 0)
                    {
                        fechaSAP.FECHA_OP = SAPFormatter.PrepararFecha(fechas.First().fechaInicio);
                        fechaSAP.FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fechas.First().fechaFin);
                    }
                    var request = new Z_MPMF_MOAOP_PAGOS_NG()
                    {
                        PE_FECHA_IN = fechaSAP,
                        PE_PROVEEDOR = proveedor,
                        PE_SOCIEDAD = sociedad
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PAGOS_NG request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PAGOS_NG(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PAGOS_NG response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_PAGOS_NGClient service = new SI_MPMF_MOAOP_PAGOS_NGClient();
                    PagosNGWebServiceMOA.ZMPES4100 fechaSAP = new PagosNGWebServiceMOA.ZMPES4100();
                    if (fechas.Count > 0)
                    {
                        fechaSAP.FECHA_OP = SAPFormatter.PrepararFecha(fechas.First().fechaInicio);
                        fechaSAP.FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fechas.First().fechaFin);
                    }
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    PagosNGWebServiceMOA.ZMPES4930[] pagos = service.SI_MPMF_MOAOP_PAGOS_NG(fechaSAP, proveedor, sociedad);
                    return Map(pagos);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        protected virtual object MapSinPI(Z_MPMF_MOAOP_PAGOS_NGResponse response)
        {
            PagosNGWSMOAResponse result = new PagosNGWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4930 pagoInfo in response.T_SALIDA)
            {
                result.pagos.Add(new PagoNGView()
                {
                    numeroPago = pagoInfo.NUM_PAGO,
                    fiscYear = pagoInfo.FISC_YEAR,
                    montoString = SAPFormatter.FormatearMonto(pagoInfo.MONTO, pagoInfo.MONEDA),
                    monto = pagoInfo.MONTO,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    fechaPagoDate = SAPFormatter.GetDateTime(pagoInfo.FECHA_PAGO),
                    viaPago = pagoInfo.VIA_PAGO,
                    retencion = pagoInfo.RETENCION,
                    retencionString = SAPFormatter.FormatearMonto(pagoInfo.RETENCION, pagoInfo.MONEDA),
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA,
                    totalMercaderiaString = SAPFormatter.FormatearMonto(pagoInfo.TOTAL_MERCADERIA, pagoInfo.MONEDA)
                });
            }

            return result;
        }
        protected virtual object Map(PagosNGWebServiceMOA.ZMPES4930[] pagos)
        {
            PagosNGWSMOAResponse result = new PagosNGWSMOAResponse();

            foreach (PagosNGWebServiceMOA.ZMPES4930 pagoInfo in pagos)
            {
                result.pagos.Add(new PagoNGView()
                {
                    numeroPago = pagoInfo.NUM_PAGO,
                    fiscYear = pagoInfo.FISC_YEAR,
                    montoString = SAPFormatter.FormatearMonto(pagoInfo.MONTO, pagoInfo.MONEDA),
                    monto = pagoInfo.MONTO,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    fechaPagoDate = SAPFormatter.GetDateTime(pagoInfo.FECHA_PAGO),
                    viaPago = pagoInfo.VIA_PAGO,
                    retencion = pagoInfo.RETENCION,
                    retencionString = SAPFormatter.FormatearMonto(pagoInfo.RETENCION, pagoInfo.MONEDA),
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA,
                    totalMercaderiaString = SAPFormatter.FormatearMonto(pagoInfo.TOTAL_MERCADERIA, pagoInfo.MONEDA)
                });
            }

            return result;
        }
    }

    public class PagosExcelNGConsumerMOA : PagosNGConsumerMOA
    {
        protected override object MapSinPI(Z_MPMF_MOAOP_PAGOS_NGResponse response)
        {
            PagosNGExcelWSMOAReponse result = new PagosNGExcelWSMOAReponse();
            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4930 pagoInfo in response.T_SALIDA)
            {
                result.pagos.Add(new PagoNG()
                {
                    numeroPago = pagoInfo.NUM_PAGO,
                    fiscYear = pagoInfo.FISC_YEAR,
                    moneda = pagoInfo.MONEDA,
                    monto = pagoInfo.MONTO,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    viaPago = pagoInfo.VIA_PAGO,
                    retencion = pagoInfo.RETENCION,
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA
                });
            }
            return result;
        }

        protected override object Map(PagosNGWebServiceMOA.ZMPES4930[] pagos)
        {
            PagosNGExcelWSMOAReponse result = new PagosNGExcelWSMOAReponse();
            foreach (PagosNGWebServiceMOA.ZMPES4930 pagoInfo in pagos)
            {
                result.pagos.Add(new PagoNG()
                {
                    numeroPago = pagoInfo.NUM_PAGO,
                    fiscYear = pagoInfo.FISC_YEAR,
                    moneda = pagoInfo.MONEDA,
                    monto = pagoInfo.MONTO,
                    fechaPago = SAPFormatter.FormatearFecha(pagoInfo.FECHA_PAGO),
                    viaPago = pagoInfo.VIA_PAGO,
                    retencion = pagoInfo.RETENCION,
                    totalMercaderia = pagoInfo.TOTAL_MERCADERIA
                });
            }
            return result;
        }
    }
}
