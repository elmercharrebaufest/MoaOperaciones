using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PagosNGWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class PagosNGConsumerMOA
    {
        SI_MPMF_MOAOP_PAGOS_NGClient service = new SI_MPMF_MOAOP_PAGOS_NGClient();

        public object request(string proveedor, List<FechaWS> fechas, string sociedad)
        {
            try
            {
                ZMPES4100 fechaSAP = new ZMPES4100();
                if (fechas.Count > 0)
                {
                    fechaSAP.FECHA_OP = SAPFormatter.PrepararFecha(fechas.First().fechaInicio);
                    fechaSAP.FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fechas.First().fechaFin);
                }
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4930[] pagos = service.SI_MPMF_MOAOP_PAGOS_NG(fechaSAP, proveedor,sociedad);
                return map(pagos);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4930[] pagos)
        {
            PagosNGWSMOAResponse result = new PagosNGWSMOAResponse();

            foreach (ZMPES4930 pagoInfo in pagos)
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
        protected override object map(ZMPES4930[] pagos)
        {
            PagosNGExcelWSMOAReponse result = new PagosNGExcelWSMOAReponse();

            foreach (ZMPES4930 pagoInfo in pagos)
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
