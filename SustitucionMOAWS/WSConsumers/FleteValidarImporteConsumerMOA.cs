using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FleteValidarImporteWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class FleteValidarImporteConsumerMOA
    {
        SI_MPMF_MOAOP_VAL_IMPORTEClient service = new SI_MPMF_MOAOP_VAL_IMPORTEClient();

        public ErrorWS request(decimal importe, string proforma, string proveedor)
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_VAL_IMPORTE(importe, proforma, proveedor);
                return map(error);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual ErrorWS map(ZMPES4910 error)
        {
            return new ErrorWS()
            {
                codigo = error.CODIGO,
                descripcion = error.DESCRIPCION,
                tipo = error.TIPO
            };
        }
    }
}
