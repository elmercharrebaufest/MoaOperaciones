using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.UsuariosWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class UsuariosConsumerMOA
    {
        SI_MPMF_MOAOP_USUARIOSClient service = new SI_MPMF_MOAOP_USUARIOSClient();

        public UsuariosWSMOAResponse request()
        {
            try
            {
                ZMPES6060[] usuarios = new ZMPES6060[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Z_MPMF_MOAOP_USUARIOSResponse response = service.SI_MPMF_MOAOP_USUARIOS(usuarios);
                UsuariosWSMOAResponse result = map(response);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private UsuariosWSMOAResponse map(Z_MPMF_MOAOP_USUARIOSResponse response)
        {
            UsuariosWSMOAResponse result = new UsuariosWSMOAResponse();
            if (response != null)
            {
                foreach (ZMPES6060 usuario in response.USUARIOS)
                {
                    result.usuarios.Add(new Usuario() {
                        id = usuario.ID,
                        tipo = usuario.TIPO,
                        usuario = usuario.USUARIO,
                        vendedor = usuario.VENDEDOR,
                        bloqueo = usuario.BLOQUEO,
                        estado = usuario.ESTADO,
                        descripcion = usuario.DESCRIPCION
                        
                    });
                }
            }
            return result;

        }
    }
}
