using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroAuthWebServiceMOA;
using System;


namespace SustitucionMOAWS.WSConsumers
{
    public class DataAgroAuthConsumerMOA
    {
        AuthServiceClient service = new AuthServiceClient();

        public object request(Int64 cuit, string nombreUsuario)
        {
            try
            {
                service.ClientCredentials.Windows.ClientCredential.UserName = DataAgroWSCredential.getUserName();
                service.ClientCredentials.Windows.ClientCredential.Password = DataAgroWSCredential.getPassword();
                service.ClientCredentials.Windows.ClientCredential.Domain = DataAgroWSCredential.getDominio();
                TokenDto response = service.GenerarUrl(cuit, nombreUsuario);
                return map(response);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(TokenDto response)
        {
            DataAgroAuthWSMOAResponse result = new DataAgroAuthWSMOAResponse();

            result.cuit = response.Cuit;
            result.error = response.Error;
            result.nombreUsuario = response.NombreUsuario;
            result.url = response.Url;
            result.vencimiento = response.Vencimiento;

            return result;
        }
    }
}
