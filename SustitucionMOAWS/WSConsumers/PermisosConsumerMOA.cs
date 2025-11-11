using SustitucionMOAModel.Models.WSMapMOA.Usuario.Permiso;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PermisosWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    class PermisosConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public PermisosWSMOAResponse request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {

                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_PERMISOS();
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PERMISOS request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PERMISOS(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_PERMISOS");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_PERMISOSClient service = new SI_MPMF_MOAOP_PERMISOSClient();
                    PermisosWebServiceMOA.ZMPES6070[] permisos = new PermisosWebServiceMOA.ZMPES6070[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    PermisosWebServiceMOA.Z_MPMF_MOAOP_PERMISOSResponse response = service.SI_MPMF_MOAOP_PERMISOS(permisos);
                    PermisosWSMOAResponse result = Map(response);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private PermisosWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.Z_MPMF_MOAOP_PERMISOSResponse response)
        {
            PermisosWSMOAResponse result = new PermisosWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6070 permiso in response.PERMISOS)
            {
                result.permisos.Add(new Permiso()
                {
                    permiso = permiso.PERMISO,
                    detalle = permiso.DETALLE,
                    pestania = permiso.PESTANIA
                });
            }

            return result;
        }
        private PermisosWSMOAResponse Map(PermisosWebServiceMOA.Z_MPMF_MOAOP_PERMISOSResponse response)
        {
            PermisosWSMOAResponse result = new PermisosWSMOAResponse();

            foreach (PermisosWebServiceMOA.ZMPES6070 permiso in response.PERMISOS)
            {
                result.permisos.Add(new Permiso()
                {
                    permiso = permiso.PERMISO,
                    detalle = permiso.DETALLE,
                    pestania = permiso.PESTANIA
                });
            }

            return result;
        }
    }
}
