using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Comprobante;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PagoComprobantesWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class PagoComprobantesConsumerMOA
    {
        SI_MPMF_MOAOP_DET_PAGOS_NGClient service = new SI_MPMF_MOAOP_DET_PAGOS_NGClient();

        public PagoComprobanteWSMOAResponse request(string documento, DateTime fecha, string sociedad, string fiscalYear)
        {
            try
            {
                ZMPES4940[] comprobantes = new ZMPES4940[] { };
                string fechaString = SAPFormatter.PrepararFecha(fecha); 
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                decimal totalRetenciones = service.SI_MPMF_MOAOP_DET_PAGOS_NG(documento, fechaString, sociedad, out comprobantes);
                return map(comprobantes, totalRetenciones, fiscalYear);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual PagoComprobanteWSMOAResponse map(ZMPES4940[] comprobantes, decimal totalRetenciones, string fiscalYear)
        {
            PagoComprobanteWSMOAResponse result = new PagoComprobanteWSMOAResponse();

            result.totalRetenciones = totalRetenciones;
            result.fiscalYear = fiscalYear;

            foreach (ZMPES4940 comprobante in comprobantes)
            {
                result.comprobantes.Add(new Comprobante()
                {
                    fechaDoc = SAPFormatter.FormatearFecha(comprobante.FECHA_DOC),
                    importe = SAPFormatter.FormatearMonto(comprobante.IMPORTE, "$"),
                    nroComprobante = comprobante.NRO_COMPROBANTE
                });
            }

            return result;
        }
    }
}

