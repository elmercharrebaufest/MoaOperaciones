using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.DataAgroServices;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

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

                if (respuesta != null)
                {
                    if (!respuesta.HayError)
                    {
                        if (respuesta.ProveedorMails.Contains(usuario.Mail, StringComparer.OrdinalIgnoreCase) || bool.Parse(ConfigurationManager.AppSettings["EsLocal"]))
                        {
                            proveedor.Comercial = string.Concat(respuesta.ComercialNombres, " ", respuesta.ComercialApellido);

                            proveedor.IdComercialDataAgro = respuesta.ComercialId;
                            proveedor.IdDataAgro = respuesta.ProveedorId;
                            proveedor.RazonSocial = respuesta.ProveedorRazonSocial;
                            proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);

                            Rol rolUsuario = ObtenerRolPorCodigo(respuesta.ProveedorOperando ? "GRAN" : "NUEG");

                            proveedor.EstadoAprobacion = respuesta.ProveedorOperando ? EstadoAprobacion.Aprobado : EstadoAprobacion.DocumentacionPendiente;

                            usuario.Roles.Add(rolUsuario);
                        }
                        else
                        {
                            Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                            usuario.Roles.Add(rolDesabilitado);

                            proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                            proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                        }
                    }
                    else
                    {
                        Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                        usuario.Roles.Add(rolDesabilitado);
                        proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                        proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                    }
                }
                else
                {
                    Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                    usuario.Roles.Add(rolDesabilitado);
                    proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                    proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
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
            catch (Exception)
            {
                return null;
            }

        }

        public void ValidarNuevoProveedorMultifirma(ref Proveedor proveedor)
        {
            var respuesta = ObtenerValidarCUITProveedorGranos(proveedor.CUIT);

            if (!respuesta.HayError)
            {
                if (respuesta.ProveedorMails.Contains(proveedor.Mail, StringComparer.OrdinalIgnoreCase) || bool.Parse(ConfigurationManager.AppSettings["EsLocal"]))
                {
                    proveedor.Comercial = string.Concat(respuesta.ComercialNombres, " ", respuesta.ComercialApellido);

                    proveedor.IdComercialDataAgro = respuesta.ComercialId;
                    proveedor.IdDataAgro = respuesta.ProveedorId;
                    proveedor.RazonSocial = respuesta.ProveedorRazonSocial;
                    proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);

                    proveedor.EstadoAprobacion = respuesta.ProveedorOperando ? EstadoAprobacion.Aprobado : EstadoAprobacion.DocumentacionPendiente;
                }
                else
                {
                    proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                    proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                }
            }
            else
            {
                proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;
                proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
            }
        }


        public string VerificarEstadoProveedor(int proveedorId)
        {
            Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            if (proveedor == null)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "Empresas"));
            }

            var respuesta = ObtenerValidarCUITProveedorGranos(proveedor.CUIT);

            if (respuesta.HayError)
            {
                throw new InfoCustomException(respuesta.ListaErrores.First().Message);
            }

            if (!respuesta.ProveedorMails.Contains(proveedor.Mail, StringComparer.OrdinalIgnoreCase))
            {
                throw new InfoCustomException("El mail del proveedor no coincide con el cargado en DataAgro");
            }

            proveedor.Comercial = string.Concat(respuesta.ComercialNombres, " ", respuesta.ComercialApellido);
            proveedor.IdComercialDataAgro = respuesta.ComercialId;
            proveedor.IdDataAgro = respuesta.ProveedorId;
            proveedor.RazonSocial = respuesta.ProveedorRazonSocial;
            proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);
            proveedor.Observaciones = "";

            proveedor.EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;

            repositorio.GuardarCambios();

            string resultado = "El proveedor ha sido habilitado para cargar la documentación.";

            return resultado;
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
            return string.Concat("00", CUIT.Substring(2, 8));
        }
    }

}
