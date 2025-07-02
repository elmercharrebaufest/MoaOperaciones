using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PesificacionGuadarWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class PesificacionGuardarConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public object request(PesificacionGuadarWebServiceMOA.ZMPES5480[] comprobantes)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5480> lstComprobantes = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5480>();
                    foreach (var comprobante in comprobantes)
                    {
                        string FECHA_COMPROBANTE = comprobante.FECHA;
                        if (FECHA_COMPROBANTE.Length == 8)
                            FECHA_COMPROBANTE = string.Format("{0}-{1}-{2}", FECHA_COMPROBANTE.Substring(0, 4), FECHA_COMPROBANTE.Substring(4, 2), FECHA_COMPROBANTE.Substring(6, 2));

                        lstComprobantes.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5480()
                        {
                            CONTRATO = comprobante.CONTRATO,
                            FIJACION = comprobante.FIJACION,
                            CANTIDAD = comprobante.CANTIDAD,
                            FECHA = FECHA_COMPROBANTE,
                            IMPORTE = comprobante.IMPORTE,
                            MONEDA = comprobante.MONEDA,
                            UNIDAD = comprobante.UNIDAD
                        });
                    }

                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_GUARDAR_PESIF()
                    {
                        IM_COMPROBANTES = lstComprobantes.ToArray(),
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_GUARDAR_PESIF request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_GUARDAR_PESIF(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_GUARDAR_PESIF response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);

                }
                else
                {
                    SI_MPMF_MOAOP_GUARDAR_PESIFClient service = new SI_MPMF_MOAOP_GUARDAR_PESIFClient();
                    PesificacionGuadarWebServiceMOA.ZMPES5490[] log = new PesificacionGuadarWebServiceMOA.ZMPES5490[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    log = service.SI_MPMF_MOAOP_GUARDAR_PESIF(comprobantes);
                    return Map(log);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        protected virtual object MapSinPI(Z_MPMF_MOAOP_GUARDAR_PESIFResponse response)
        {
            PesificacionSetContratosWSMOAResponse result = new PesificacionSetContratosWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5490 item in response.EX_LOG)
            {
                result.Log.Add(new ItemView()
                {
                    Contrato = item.CONTRATO,
                    Fijacion = item.FIJACION,
                    Mensaje = item.MENSAJE
                });
            }

            return result;
        }
        protected virtual object Map(PesificacionGuadarWebServiceMOA.ZMPES5490[] log)
        {
            PesificacionSetContratosWSMOAResponse result = new PesificacionSetContratosWSMOAResponse();

            foreach (PesificacionGuadarWebServiceMOA.ZMPES5490 item in log)
            {
                result.Log.Add(new ItemView()
                {
                    Contrato = item.CONTRATO,
                    Fijacion = item.FIJACION,
                    Mensaje = item.MENSAJE
                });
            }

            return result;
        }
    }
}
