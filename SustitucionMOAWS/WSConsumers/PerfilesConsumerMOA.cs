using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PerfilesWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class PerfilesConsumerMOA
    {
        SI_MPMF_MOAOP_PERFILESClient service = new SI_MPMF_MOAOP_PERFILESClient();

        public PerfilesWSMOAResponse request()
        {
            try
            {
                ZMPES6080[] perfiles = new ZMPES6080[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password =  SAPCredential.getPassword();
                Z_MPMF_MOAOP_PERFILESResponse response = service.SI_MPMF_MOAOP_PERFILES(perfiles);
                PerfilesWSMOAResponse result = map(response);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private PerfilesWSMOAResponse map(Z_MPMF_MOAOP_PERFILESResponse response)
        {
            PerfilesWSMOAResponse result = new PerfilesWSMOAResponse();

            foreach (ZMPES6080 perfil in response.PERFILES) {
                result.perfiles.Add(new Perfil()
                {
                    nombre = perfil.NOMBRE,
                    perfil = perfil.PERFIL
                });
            }

            foreach (ZMPES4950 tipo in response.TIPO_PROVEEDOR)
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
