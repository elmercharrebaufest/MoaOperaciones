using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAWS.CrearUsuarioWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
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
    public class UsuarioCrearConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        public UsuarioCrearWSMOAResponse request(string mail, string proveedor, string perfil, string tipo)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_CREA_USER()
                    {
                        PE_MAIL = mail,
                        PE_PERFIL = perfil,
                        PE_PROVEEDOR = proveedor,
                        PE_TIPO = tipo
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CREA_USER request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_CREA_USER(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CREA_USER response");
                    Log.Info(response.ToXml());
                    UsuarioCrearWSMOAResponse result = MapSinPI(response.RETURN, response.TEXTO);
                    return result;
                }
                else
                {
                    string texto = "";
                    SI_MPMF_MOAOP_CREA_USERClient service = new SI_MPMF_MOAOP_CREA_USERClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string error = service.SI_MPMF_MOAOP_CREA_USER(mail, perfil, proveedor, tipo, out texto);
                    UsuarioCrearWSMOAResponse result = Map(error, texto);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private UsuarioCrearWSMOAResponse Map(string error, string texto)
        {
            UsuarioCrearWSMOAResponse result = new UsuarioCrearWSMOAResponse();
            result.error = error;
            result.texto = texto;
            return result;
        }
        private UsuarioCrearWSMOAResponse MapSinPI(string error, string texto)
        {
            UsuarioCrearWSMOAResponse result = new UsuarioCrearWSMOAResponse();
            result.error = error;
            result.texto = texto;
            return result;
        }
    }
}
