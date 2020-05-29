using System;
using System.Collections.Generic;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;

namespace SustitucionMOAUtils.Services
{
    public class VendedorService
    {
        public VendedorDetalleWSMOAResponse getDatosFiscales(string vendedor, string proveedor)
        {
            try
            {
                if (proveedor == null || proveedor == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Proveedor"));
                }

                VendedorDetalleWSMOAResponse response = new VendedorDetalleConsumerMOA().request(vendedor, proveedor);
                if (response == null)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Proveedor", proveedor));
                }

                if (response.error != null && response.error != "" && response.error != "11" && response.error != "00")
                   throw new InfoCustomException(String.Format(InfoMsg.ElementoNoExiste, "Situación Fiscal", "Proveedor: " + proveedor));

                return response;
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

        public VendedoresWSMOAResponse getVendedores(string proveedor, string fechaInicio, string fechaFin ) {
            if (fechaInicio == "") {
                fechaInicio = DateTime.Now.AddDays(-1).ToShortDateString();
            }
            if (fechaFin == "")
            {
                fechaFin = DateTime.Now.ToShortDateString();
            }
            List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
            VendedoresWSMOAResponse response = new VendedoresConsumerMOA().request(proveedor, fechas);
            return response;
        }

        public VendedorHabilitadoWSMOAResponse getVendedorStatus(string cuit, string user)
        {
            try
            {
                if (cuit == null || cuit == "")
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT"));
                }

                VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoConsumerMOA().request(cuit, "MOA", user);
                if (response == null)
                {
                    throw new InfoCustomException(InfoMsg.ProveedorSinAlta);
                }

                if (response.status == null || response.status == "" || response.status == "Proveedor inexistente")
                    throw new InfoCustomException(InfoMsg.ProveedorSinAlta);

                return response;
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
