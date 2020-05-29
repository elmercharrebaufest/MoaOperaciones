using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PesificacionWebServiceMOA;
using System;

namespace SustitucionMOAWS.WSConsumers
{
    public class PesificacionConsumerMOA
    {
        SI_MPMF_MOAOP_OBTENER_PEND_USDClient service = new SI_MPMF_MOAOP_OBTENER_PEND_USDClient();

        public object request(string proveedor)
        {
            try
            {
                ZMPES5470[] contratos = new ZMPES5470[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                contratos = service.SI_MPMF_MOAOP_OBTENER_PEND_USD(proveedor);
                return Map(contratos);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(ZMPES5470[] contratos)
        {
            PesificacionGetContratosWSMOAResponse result = new PesificacionGetContratosWSMOAResponse();

            foreach (ZMPES5470 contrato in contratos)
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