using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PagosWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class PagosConsumerMOA
    {
        SI_MPMF_MOAOP_PAGOS_CTA_CTEClient service = new SI_MPMF_MOAOP_PAGOS_CTA_CTEClient();

        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                ZMPES5310[] pagos = new ZMPES5310[] { };
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };
                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_PAGOS_CTA_CTE(proveedor, "P", ref fechasSAPArray, out pagos);
                return map(pagos, error);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES5310[] pagos, ZMPES4910 error)
        {
            PagosWSMOAReponse result = new PagosWSMOAReponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES5310 pagoInfo in pagos)
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

        protected override object map(ZMPES5310[] pagos, ZMPES4910 error)
        {
            PagosExcelWSMOAReponse result = new PagosExcelWSMOAReponse();
            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES5310 pagoInfo in pagos)
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
