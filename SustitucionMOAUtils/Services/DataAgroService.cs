using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using System;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAWS.DataAgroServices;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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

        public bool ValidarCUITProveedorGranos(UsuarioGranos usuario, Proveedor proveedor)
        {
            try
            {
                ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(proveedor.CUIT);

                if (!respuesta.HayError)
                {
                    if (!respuesta.ProveedorMails.Contains(usuario.Mail))
                    {
                        usuario.Comercial = string.Concat(respuesta.Nombres, " ", respuesta.Apellido);

                        proveedor.IdComercialDataAgro = respuesta.ComercialId;
                        proveedor.IdDataAgro = respuesta.ProveedorId;
                        proveedor.RazonSocial = respuesta.ProveedorRazonSocial;
                        proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);
                        proveedor.Mail = usuario.Mail;
                    }
                    else
                    {
                        proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                        proveedor.Observaciones = "El mail del registro no está dentro de los mails registrados en Data Agro.";
                    }
                }
                else
                {
                    proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                    proveedor.Observaciones = "El proveedor no está habilitado en Data Agro.";

                }

                return respuesta.HayError;
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

        private string FormatearCodigoProveedor(string CUIT)
        {
            return CUIT.Substring(2, 10);
        }
    }

}
