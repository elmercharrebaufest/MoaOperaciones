using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ObtenerSolpWebServiceMOA;
using System;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_SOLPEDClient service = new SI_MMRFC_OBTENER_SOLPEDClient();

        public object request(string proveedor)
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                //string error = service.SI_MMRFC_OBTENER_SOLPED()
                //return map(error, contratos_in, contratos_out, cosechas, fechasSAPArray, materiales, vendedores);
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
