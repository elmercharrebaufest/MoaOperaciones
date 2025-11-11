using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PerfilesWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class PerfilesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public PerfilesWSMOAResponse request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new Z_MPMF_MOAOP_PERFILES();
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PERFILES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PERFILES(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_PERFILES");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_PERFILESClient service = new SI_MPMF_MOAOP_PERFILESClient();
                    PerfilesWebServiceMOA.ZMPES6080[] perfiles = new PerfilesWebServiceMOA.ZMPES6080[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    PerfilesWebServiceMOA.Z_MPMF_MOAOP_PERFILESResponse response = service.SI_MPMF_MOAOP_PERFILES(perfiles);
                    PerfilesWSMOAResponse result = Map(response);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private PerfilesWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.Z_MPMF_MOAOP_PERFILESResponse response)
        {
            PerfilesWSMOAResponse result = new PerfilesWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6080 perfil in response.PERFILES)
            {
                result.perfiles.Add(new Perfil()
                {
                    nombre = perfil.NOMBRE,
                    perfil = perfil.PERFIL
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4950 tipo in response.TIPO_PROVEEDOR)
            {
                result.tipos.Add(new TipoProveedor()
                {
                    descripcion = tipo.DESCRIPCION,
                    tipo = tipo.TIPO
                });
            }

            return result;
        }
        private PerfilesWSMOAResponse Map(PerfilesWebServiceMOA.Z_MPMF_MOAOP_PERFILESResponse response)
        {
            PerfilesWSMOAResponse result = new PerfilesWSMOAResponse();

            foreach (PerfilesWebServiceMOA.ZMPES6080 perfil in response.PERFILES)
            {
                result.perfiles.Add(new Perfil()
                {
                    nombre = perfil.NOMBRE,
                    perfil = perfil.PERFIL
                });
            }

            foreach (PerfilesWebServiceMOA.ZMPES4950 tipo in response.TIPO_PROVEEDOR)
            {
                result.tipos.Add(new TipoProveedor()
                {
                    descripcion = tipo.DESCRIPCION,
                    tipo = tipo.TIPO
                });
            }

            return result;
        }
    }
}
