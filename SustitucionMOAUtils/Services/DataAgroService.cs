using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using System;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;

namespace SustitucionMOAUtils.Services
{
    public class DataAgroService
    {
        public DataAgroAuthWSMOAResponse goToDataAgro(string proveedor, string nombre)
        {
            try
            {
                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Proveedor"));
                }

                VendedorDetalleWSMOAResponse responseVendedorDetalle = new VendedorDetalleConsumerMOA().request(proveedor, proveedor);
                if (responseVendedorDetalle == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Proveedor", proveedor));
                }

                if (responseVendedorDetalle.error != null && responseVendedorDetalle.error != "" && responseVendedorDetalle.error != "11" && responseVendedorDetalle.error != "00")
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Datos Fiscales", "Proveedor: " + proveedor));

                DataAgroAuthWSMOAResponse responseDataAgroAuth = (DataAgroAuthWSMOAResponse)new DataAgroAuthConsumerMOA().request(Int64.Parse(responseVendedorDetalle.cabeceras[0].cuit), nombre);

                return responseDataAgroAuth;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
