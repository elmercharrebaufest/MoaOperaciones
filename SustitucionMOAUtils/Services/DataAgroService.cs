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
using System.Configuration;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using SustitucionMOARepositorio;

namespace SustitucionMOAUtils.Services
{
    public class DataAgroService : IDataAgroService
    {
        protected readonly IRepositorio repositorio;


        public DataAgroService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public DataAgroService()
        {
        }

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

        public bool ValidarCUITProveedorGranos(ref UsuarioGranos usuario, Proveedor proveedor)
        {
            try
            {
                ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(proveedor.CUIT);

                usuario.Roles = new List<Rol>();
                usuario.Proveedores = new List<Proveedor>();
                usuario.TipoUsuario = ObtenerTipoPorNombreCorto("G");
                proveedor.Mail = usuario.Mail;

                if (!respuesta.HayError)
                {
                    if (respuesta.ProveedorMails.Contains(usuario.Mail) || bool.Parse(ConfigurationManager.AppSettings["EsLocal"]))
                    {
                        usuario.Comercial = string.Concat(respuesta.ComercialNombres, " ", respuesta.ComercialApellido);

                        proveedor.IdComercialDataAgro = respuesta.ComercialId;
                        proveedor.IdDataAgro = respuesta.ProveedorId;
                        proveedor.RazonSocial = respuesta.ProveedorRazonSocial;
                        proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);

                        Rol rolUsuario = ObtenerRolPorCodigo(respuesta.ProveedorOperando ?  "GRAN" : "NUEG" );

                        proveedor.EstadoAprobacion = respuesta.ProveedorOperando ? EstadoAprobacion.Aprobado : EstadoAprobacion.DocumentacionPendiente;
                
                        usuario.Roles.Add(rolUsuario);
                    }
                    else
                    {
                        Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                        usuario.Roles.Add(rolDesabilitado);

                        proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                        proveedor.Observaciones = "El mail del registro no está dentro de los mails registrados en Data Agro.";
                    }
                }
                else
                {
                    Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                    usuario.Roles.Add(rolDesabilitado);
                    proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                    proveedor.Observaciones = "El proveedor no está habilitado en Data Agro.";
                }

                usuario.Proveedores.Add(proveedor);
                usuario.Habilitado = true;

                repositorio.Agregar(usuario);

                repositorio.GuardarCambios();

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

        public ResultadoValidarProveedorComercial ObtenerValidarCUITProveedorGranos(string CUIT)
        {
            try
            {
                ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(CUIT);

                return respuesta;
            }
            catch (InfoCustomException e)
            {
                return null;
            }
            catch (ValidationCustomException e)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto)
        {
            return repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
        }

        public string ObtenerCBUProveedor(string CUITproveedor)
        {
            try
            {
                ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(CUITproveedor);

                if (respuesta.HayError)
                {
                    throw new Exception(String.Join(" - ", respuesta.ListaErrores.ToList()));
                }

                return respuesta.ProveedorCBU;
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
            return CUIT.Substring(2, 8);
        }
    }

}
