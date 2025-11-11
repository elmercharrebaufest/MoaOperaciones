using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.UsuariosWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class UsuariosConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public UsuariosWSMOAResponse request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_USUARIOS()
                    {

                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_USUARIOS request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_USUARIOS(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_USUARIOS");
                    UsuariosWSMOAResponse result = MapSinPI(response);
                    return result;
                }
                else
                {
                    UsuariosWebServiceMOA.ZMPES6060[] usuarios = new UsuariosWebServiceMOA.ZMPES6060[] { };
                    SI_MPMF_MOAOP_USUARIOSClient service = new SI_MPMF_MOAOP_USUARIOSClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    UsuariosWebServiceMOA.Z_MPMF_MOAOP_USUARIOSResponse response = service.SI_MPMF_MOAOP_USUARIOS(usuarios);
                    UsuariosWSMOAResponse result = Map(response);
                    return result;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private UsuariosWSMOAResponse Map(UsuariosWebServiceMOA.Z_MPMF_MOAOP_USUARIOSResponse response)
        {
            UsuariosWSMOAResponse result = new UsuariosWSMOAResponse();
            if (response != null)
            {
                foreach (UsuariosWebServiceMOA.ZMPES6060 usuario in response.USUARIOS)
                {
                    result.usuarios.Add(new Usuario()
                    {
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
        private UsuariosWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.Z_MPMF_MOAOP_USUARIOSResponse response)
        {
            UsuariosWSMOAResponse result = new UsuariosWSMOAResponse();
            if (response != null)
            {
                foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6060 usuario in response.USUARIOS)
                {
                    result.usuarios.Add(new Usuario()
                    {
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
