using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.LoginWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class LoginConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public LoginWSMOAResponse request(string username, string password)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_LOGIN()
                    {
                        PE_USUARIO = username.ToUpper(),
                        PE_PASSW = password.ToUpper()
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_LOGIN request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_LOGIN(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_LOGIN response");
                    Log.Info(response.ToXml());
                    LoginWSMOAResponse result = MapSinPI(response);
                    return result;
                }
                else
                {
                    SI_MPMF_MOAOP_LOGIN_INClient service = new SI_MPMF_MOAOP_LOGIN_INClient();
                    LoginWebServiceMOA.DT_Moaop_Login_Req requestInfo = new DT_Moaop_Login_Req { Usuario = username.ToUpper(), Passw = password.ToUpper() };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    LoginWebServiceMOA.Z_MPMF_MOAOP_LOGINResponse response = service.SI_MPMF_MOAOP_LOGIN_IN(requestInfo);
                    LoginWSMOAResponse result = Map(response);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private LoginWSMOAResponse Map(LoginWebServiceMOA.Z_MPMF_MOAOP_LOGINResponse response)
        {
            LoginWSMOAResponse result = new LoginWSMOAResponse();
            if (response != null)
            {
                result.error = response.RETURN;
                result.texto = response.TEXTO;
                result.nombre = response.NAME1;
                result.proveedor = response.PS_PROVEEDOR;
                result.granosFlag = response.T_PROVEEDOR.ToUpper();
                result.tipoUsuario = response.TIPO.ToUpper();
                foreach (LoginWebServiceMOA.ZMPES6050 permiso in response.PERMISOS) {
                    result.permisos.Add(permiso.PERMISO);
                }
            }
            return result;
        }
        private LoginWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.Z_MPMF_MOAOP_LOGINResponse response)
        {
            LoginWSMOAResponse result = new LoginWSMOAResponse();
            if (response != null)
            {
                result.error = response.RETURN;
                result.texto = response.TEXTO;
                result.nombre = response.NAME1;
                result.proveedor = response.PS_PROVEEDOR;
                result.granosFlag = response.T_PROVEEDOR.ToUpper();
                result.tipoUsuario = response.TIPO.ToUpper();
                foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6050 permiso in response.PERMISOS)
                {
                    result.permisos.Add(permiso.PERMISO);
                }
            }
            return result;
        }

    }
}
