using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Permiso;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PermisosWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    class PermisosConsumerMOA
    {
        SI_MPMF_MOAOP_PERMISOSClient service = new SI_MPMF_MOAOP_PERMISOSClient();

        public PermisosWSMOAResponse request()
        {
            try
            {
                ZMPES6070[] permisos = new ZMPES6070[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Z_MPMF_MOAOP_PERMISOSResponse response = service.SI_MPMF_MOAOP_PERMISOS(permisos);
                PermisosWSMOAResponse result = map(response);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private PermisosWSMOAResponse map(Z_MPMF_MOAOP_PERMISOSResponse response)
        {
            PermisosWSMOAResponse result = new PermisosWSMOAResponse();

            foreach (ZMPES6070 permiso in response.PERMISOS)
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
