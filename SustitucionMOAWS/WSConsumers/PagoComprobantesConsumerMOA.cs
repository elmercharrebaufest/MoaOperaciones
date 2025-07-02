using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Comprobante;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PagoComprobantesWebServiceMOA;
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
    public class PagoComprobantesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];
        public PagoComprobanteWSMOAResponse request(string documento, DateTime fecha, string sociedad, string fiscalYear)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    string fechaString = SAPFormatter.PrepararFecha(fecha);
                    var request = new Z_MPMF_MOAOP_DET_PAGOS_NG()
                    {
                        PE_DOCUMENTO = documento,
                        PE_FECHA = fechaString,
                        PE_SOCIEDAD = sociedad,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_PAGOS_NG request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DET_PAGOS_NG(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_PAGOS_NG response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response, fiscalYear);
                }
                else
                {
                    SI_MPMF_MOAOP_DET_PAGOS_NGClient service = new SI_MPMF_MOAOP_DET_PAGOS_NGClient();
                    PagoComprobantesWebServiceMOA.ZMPES4940[] comprobantes = new PagoComprobantesWebServiceMOA.ZMPES4940[] { };
                    string fechaString = SAPFormatter.PrepararFecha(fecha);
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    decimal totalRetenciones = service.SI_MPMF_MOAOP_DET_PAGOS_NG(documento, fechaString, sociedad, out comprobantes);
                    return Map(comprobantes, totalRetenciones, fiscalYear);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        protected virtual PagoComprobanteWSMOAResponse MapSinPI(Z_MPMF_MOAOP_DET_PAGOS_NGResponse response, string fiscalYear)
        {
            PagoComprobanteWSMOAResponse result = new PagoComprobanteWSMOAResponse();

            result.totalRetenciones = response.TOTAL_RETENCIONES;
            result.fiscalYear = fiscalYear;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4940 comprobante in response.T_SALIDA)
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
        protected virtual PagoComprobanteWSMOAResponse Map(PagoComprobantesWebServiceMOA.ZMPES4940[] comprobantes, decimal totalRetenciones, string fiscalYear)
        {
            PagoComprobanteWSMOAResponse result = new PagoComprobanteWSMOAResponse();

            result.totalRetenciones = totalRetenciones;
            result.fiscalYear = fiscalYear;

            foreach (PagoComprobantesWebServiceMOA.ZMPES4940 comprobante in comprobantes)
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

