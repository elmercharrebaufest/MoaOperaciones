using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAWS.CrearUsuarioWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class UsuarioCrearConsumerMOA
    {
        SI_MPMF_MOAOP_CREA_USERClient service = new SI_MPMF_MOAOP_CREA_USERClient();

        public UsuarioCrearWSMOAResponse request(string mail, string proveedor, string perfil, string tipo)
        {
            try
            {
                string texto = "";
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string error = service.SI_MPMF_MOAOP_CREA_USER(mail, perfil, proveedor, tipo, out texto);
                UsuarioCrearWSMOAResponse result = map(error, texto);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private UsuarioCrearWSMOAResponse map(string error, string texto)
        {
            UsuarioCrearWSMOAResponse result = new UsuarioCrearWSMOAResponse();
            result.error = error;
            result.texto = texto;
            return result;

        }
    }
}
