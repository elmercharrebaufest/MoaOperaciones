using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PesificacionWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class PesificacionConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public object request(string proveedor)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_OBTENER_PEND_USD()
                    {
                        IM_PROVEEDOR = proveedor
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_OBTENER_PEND_USD request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_OBTENER_PEND_USD(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_OBTENER_PEND_USD response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_OBTENER_PEND_USDClient service = new SI_MPMF_MOAOP_OBTENER_PEND_USDClient();
                    PesificacionWebServiceMOA.ZMPES5470[] contratos = new PesificacionWebServiceMOA.ZMPES5470[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    contratos = service.SI_MPMF_MOAOP_OBTENER_PEND_USD(proveedor);
                    return Map(contratos);
                }


            }
            catch (Exception e)
            {
                throw e;
            }

        }
        protected virtual object MapSinPI(Z_MPMF_MOAOP_OBTENER_PEND_USDResponse response)
        {
            PesificacionGetContratosWSMOAResponse result = new PesificacionGetContratosWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5470 contrato in response.EX_CONTRATOS)
            {
                result.Contratos.Add(new Contrato()
                {
                    NroContrato = contrato.CONTRATO,
                    Fijacion = contrato.FIJACION,
                    Vendedor = contrato.VENDEDOR,
                    NombreVendedor = contrato.NOM_VEND,
                    CantidadPendiente = contrato.CANT_PENDIENTE,
                    MontoPendiente = contrato.MONTO_PENDIENTE,
                    Precio = contrato.PRECIO,
                    Unidad = contrato.UNIDAD,
                    Moneda = contrato.MONEDA
                });
            }

            return result;
        }
        protected virtual object Map(PesificacionWebServiceMOA.ZMPES5470[] contratos)
        {
            PesificacionGetContratosWSMOAResponse result = new PesificacionGetContratosWSMOAResponse();

            foreach (PesificacionWebServiceMOA.ZMPES5470 contrato in contratos)
            {
                result.Contratos.Add(new Contrato()
                {
                    NroContrato = contrato.CONTRATO,
                    Fijacion = contrato.FIJACION,
                    Vendedor = contrato.VENDEDOR,
                    NombreVendedor = contrato.NOM_VEND,
                    CantidadPendiente = contrato.CANT_PENDIENTE,
                    MontoPendiente = contrato.MONTO_PENDIENTE,
                    Precio = contrato.PRECIO,
                    Unidad = contrato.UNIDAD,
                    Moneda = contrato.MONEDA
                });
            }

            return result;
        }
    }
}