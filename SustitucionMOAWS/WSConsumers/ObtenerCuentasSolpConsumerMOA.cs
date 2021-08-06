using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ObtenerCuentasSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    class ObtenerCuentasSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_CUENTASClient service = new SI_MMRFC_OBTENER_CUENTASClient();

        public object request()
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
