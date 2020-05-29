using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FletesRelacionWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class FleteRelacionConsumerMOA
    {
        SI_MPMF_MOAOP_RELAC_PLClient service = new SI_MPMF_MOAOP_RELAC_PLClient();

        public ErrorWS request(string factura, DateTime fechaEmision, decimal importe, string proforma, string proveedor)
        {
            try
            {
                string fechaEmisionString = SAPFormatter.PrepararFecha(fechaEmision);
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_RELAC_PL(factura, fechaEmisionString, importe, proforma, proveedor);
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
