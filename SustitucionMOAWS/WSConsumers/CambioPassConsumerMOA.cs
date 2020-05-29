using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAWS.CambioPassWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CambioPassConsumerMOA
    {
        SI_MPMF_MOAOP_LOGIN_CAMBIOPASSClient service = new SI_MPMF_MOAOP_LOGIN_CAMBIOPASSClient();

        public LoginWSMOAResponse request(string usuario, string pass, string passNew)
        {
            try
            {
                DT_Moaop_Login_Req requestInfo = new DT_Moaop_Login_Req { Usuario = SAPFormatter.PrepararString(usuario), Passw = SAPFormatter.PrepararString(pass), Nvpassw = SAPFormatter.PrepararString(passNew) };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Z_MPMF_MOAOP_LOGINResponse response = service.SI_MPMF_MOAOP_LOGIN_CAMBIOPASS(requestInfo);
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
                result.nombre= response.NAME1;
                result.proveedor = response.PS_PROVEEDOR;
                result.granosFlag = response.TIPO;
                result.tipoUsuario = response.T_PROVEEDOR;
                result.texto = response.TEXTO;
                foreach (ZMPES6050 permiso in response.PERMISOS) {
                    result.permisos.Add(permiso.PERMISO);
                }
            }
            return result;

        }
    }
}
