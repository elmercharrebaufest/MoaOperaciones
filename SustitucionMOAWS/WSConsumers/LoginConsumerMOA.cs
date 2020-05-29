using System;
using System.Collections.Generic;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.LoginWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class LoginConsumerMOA
    {
        SI_MPMF_MOAOP_LOGIN_INClient service = new SI_MPMF_MOAOP_LOGIN_INClient();

        public LoginWSMOAResponse request(string username, string password)
        {
            try
            {
                DT_Moaop_Login_Req requestInfo = new DT_Moaop_Login_Req { Usuario = username.ToUpper(), Passw = password.ToUpper() };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Z_MPMF_MOAOP_LOGINResponse response = service.SI_MPMF_MOAOP_LOGIN_IN(requestInfo);
                LoginWSMOAResponse result = map(response);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private LoginWSMOAResponse map(Z_MPMF_MOAOP_LOGINResponse response)
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
                foreach (ZMPES6050 permiso in response.PERMISOS) {
                    result.permisos.Add(permiso.PERMISO);
                }
            }
            return result;
        }

    }
}
