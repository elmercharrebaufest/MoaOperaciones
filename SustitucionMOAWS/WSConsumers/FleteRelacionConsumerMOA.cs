using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FletesRelacionWebServiceMOA;
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
    public class FleteRelacionConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ErrorWS request(string factura, DateTime fechaEmision, decimal importe, string proforma, string proveedor)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    string fechaEmisionString = SAPFormatter.PrepararFecha(fechaEmision);
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new Z_MPMF_MOAOP_RELAC_PL()
                    {
                        PE_FACTURA = factura,
                        PE_FECHA_EMISION = fechaEmisionString,
                        PE_IMPORTE = importe,
                        PE_PROFORMA = proforma,
                        PE_PROVEEDOR = proveedor
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_RELAC_PL request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_RELAC_PL(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_RELAC_PL response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);

                }
                else
                {
                    SI_MPMF_MOAOP_RELAC_PLClient service = new SI_MPMF_MOAOP_RELAC_PLClient();
                    string fechaEmisionString = SAPFormatter.PrepararFecha(fechaEmision);
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    FletesRelacionWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_RELAC_PL(factura, fechaEmisionString, importe, proforma, proveedor);
                    return Map(error);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        protected virtual ErrorWS MapSinPI(Z_MPMF_MOAOP_RELAC_PLResponse response)
        {
            return new ErrorWS()
            {
                codigo = response.MENSAJE.CODIGO,
                descripcion = response.MENSAJE.DESCRIPCION,
                tipo = response.MENSAJE.TIPO
            };
        }
        protected virtual ErrorWS Map(FletesRelacionWebServiceMOA.ZMPES4910 error)
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
