using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PesificacionGuadarWebServiceMOA;
using System;

namespace SustitucionMOAWS.WSConsumers
{
    public class PesificacionGuardarConsumerMOA
    {
        SI_MPMF_MOAOP_GUARDAR_PESIFClient service = new  SI_MPMF_MOAOP_GUARDAR_PESIFClient();

        public object request(ZMPES5480[] comprobantes)
        {
            try
            {
                ZMPES5490[] log = new ZMPES5490[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                log = service.SI_MPMF_MOAOP_GUARDAR_PESIF(comprobantes);
                return Map(log);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual object Map(ZMPES5490[] log)
        {
            PesificacionSetContratosWSMOAResponse result = new PesificacionSetContratosWSMOAResponse();

            foreach (ZMPES5490 item in log)
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
