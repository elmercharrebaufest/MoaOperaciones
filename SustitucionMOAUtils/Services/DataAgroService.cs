using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class DataAgroService : Interfaces.IDataAgroService
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
                    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Proveedor"));
                }

                VendedorDetalleWSMOAResponse responseVendedorDetalle = new VendedorDetalleConsumerMOA().request(proveedor, proveedor);
                if (responseVendedorDetalle == null)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.ElementoNoExiste, "Proveedor", proveedor));
                }

                if (responseVendedorDetalle.error != null && responseVendedorDetalle.error != "" && responseVendedorDetalle.error != "11" && responseVendedorDetalle.error != "00")
                    throw new InfoCustomException(string.Format(InfoMsg.ElementoNoExiste, "Datos Fiscales", "Proveedor: " + proveedor));

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
                var mail = usuario.Mail;

                if (repositorio.Existe<Proveedor>(x => x.CUIT == proveedor.CUIT && x.Mail == mail))
                {
                    var proveedorExistente = repositorio.Obtener<Proveedor>(x => x.CUIT == proveedor.CUIT && x.Mail == mail);

                    var tipoUsuarioGranos = ObtenerTipoPorNombreCorto("G");

                    usuario.Roles = new List<Rol>();
                    usuario.Proveedores = new List<Proveedor>();
                    usuario.TipoUsuario = tipoUsuarioGranos;

                    Rol rolGranos = proveedorExistente.EstadoAprobacion == EstadoAprobacion.Aprobado ? ObtenerRolPorCodigo("GRAN") : ObtenerRolPorCodigo("NUEG");

                    usuario.Roles.Add(rolGranos);
                    usuario.Proveedores.Add(proveedorExistente);
                    usuario.Habilitado = true;

                    repositorio.Agregar(usuario);
                    repositorio.GuardarCambios();

                    return true;
                }
                else
                {
                    SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(proveedor.CUIT, null);

                    var tipoUsuarioGranos = ObtenerTipoPorNombreCorto("G");

                    usuario.Roles = new List<Rol>();
                    usuario.Proveedores = new List<Proveedor>();
                    usuario.TipoUsuario = tipoUsuarioGranos;
                    proveedor.Mail = usuario.Mail;
                    proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);
                    proveedor.TipoProveedor = tipoUsuarioGranos;

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

                                proveedor.FechaSolicitud = DateTime.Now;

                                Rol rolUsuario = ObtenerRolPorCodigo(respuesta.ProveedorOperando ? "GRAN" : "NUEG");

                                //proveedor.EstadoAprobacion = respuesta.ProveedorOperando ? EstadoAprobacion.Aprobado : EstadoAprobacion.DocumentacionPendiente;
                                proveedor.EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;
                                var observacionEstado = respuesta.ProveedorOperando ? EstadoAprobacion.AprobacionPendiente.ToFriendlyString() : EstadoAprobacion.DocumentacionPendiente.ToFriendlyString();
                                var hist = new ProveedorHistorialAprobacion
                                {
                                    Fecha = DateTime.Now,
                                    Proveedor_Id = proveedor.Id,
                                    Usuario_Id = usuario.Id,
                                    EstadoAprobacion = proveedor.EstadoAprobacion,
                                    Observacion = observacionEstado
                                };
                                repositorio.Agregar(hist);

                                usuario.Roles.Add(rolUsuario);
                            }
                            else
                            {
                                Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                                usuario.Roles.Add(rolDesabilitado);

                                proveedor.EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro;

                                var hist = new ProveedorHistorialAprobacion
                                {
                                    Fecha = DateTime.Now,
                                    Proveedor_Id = proveedor.Id,
                                    Usuario_Id = usuario.Id,
                                    EstadoAprobacion = proveedor.EstadoAprobacion,
                                    Observacion = "El mail del registro no coincide con el del proveedor."
                                };
                                repositorio.Agregar(hist);

                                proveedor.Observaciones = "El mail del registro no coincide con el del proveedor. Comunicarse con su comercial.";
                            }
                        }
                        else
                        {
                            Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                            usuario.Roles.Add(rolDesabilitado);
                            proveedor.EstadoAprobacion = EstadoAprobacion.SinAlta;

                            var hist = new ProveedorHistorialAprobacion
                            {
                                Fecha = DateTime.Now,
                                Proveedor_Id = proveedor.Id,
                                Usuario_Id = usuario.Id,
                                EstadoAprobacion = proveedor.EstadoAprobacion,
                                Observacion = respuesta.ListaErrores.First().Message
                            };
                            repositorio.Agregar(hist);

                            proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                        }
                    }
                    else
                    {
                        Rol rolDesabilitado = ObtenerRolPorCodigo("DDAG");
                        usuario.Roles.Add(rolDesabilitado);
                        proveedor.EstadoAprobacion = EstadoAprobacion.SinAlta;

                        var hist = new ProveedorHistorialAprobacion
                        {
                            Fecha = DateTime.Now,
                            Proveedor_Id = proveedor.Id,
                            Usuario_Id = usuario.Id,
                            EstadoAprobacion = proveedor.EstadoAprobacion,
                            Observacion = "Ocurrió un error comunicandose con Data Agro"
                        };
                        repositorio.Agregar(hist);

                        proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
                    }

                    usuario.Proveedores.Add(proveedor);
                    usuario.Habilitado = true;

                    repositorio.Agregar(usuario);

                    repositorio.GuardarCambios();

                    return respuesta.HayError;
                }
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

        public SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial ObtenerValidarCUITProveedorGranos(string CUIT, bool? corredor = false)
        {
            try
            {
                SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(CUIT, corredor);

                return respuesta;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public decimal TraerTipoDeCambio()
        {
            try
            {
                var respuesta = new DataAgroConsumer().TraerTipoDeCambio();

                return respuesta;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return 0;
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
                    proveedor.Observaciones = "El mail del registro no coincide con el del proveedor. Comunicarse con su comercial.";
                }
            }
            else
            {
                proveedor.EstadoAprobacion = EstadoAprobacion.SinAlta;
                proveedor.Observaciones = "El mail del registro no se encuentra habilitado. Comunicarse con su comercial.";
            }
        }

        public string VerificarEstadoProveedor(int proveedorId, string usuarioMail)
        {
            Proveedor proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            if (proveedor == null)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "Empresas"));
            }
            int usuarioId = repositorio.Obtener<Usuario, int>(u => u.Mail == usuarioMail, x => x.Id);
            var respuesta = ObtenerValidarCUITProveedorGranos(proveedor.CUIT, null);

            var hist = new ProveedorHistorialAprobacion
            {
                Fecha = DateTime.Now,
                Proveedor_Id = proveedor.Id,
                Usuario_Id = usuarioId,
                EstadoAprobacion = proveedor.EstadoAprobacion,
            };

            if (respuesta.HayError)
            {
                hist.Observacion = respuesta.ListaErrores.First().Message;
                repositorio.Agregar(hist);
                repositorio.GuardarCambios();
                throw new InfoCustomException(respuesta.ListaErrores.First().Message);
            }

            if (!respuesta.ProveedorMails.Contains(proveedor.Mail, StringComparer.OrdinalIgnoreCase))
            {
                hist.Observacion = "El mail del proveedor no coincide con el cargado en DataAgro";
                repositorio.Agregar(hist);
                repositorio.GuardarCambios();
                throw new InfoCustomException("El mail del proveedor no coincide con el cargado en DataAgro");
            }

            hist.Observacion = "Proveedor habilitado en DataAgro";

            proveedor.Comercial = string.Concat(respuesta.ComercialNombres, " ", respuesta.ComercialApellido);
            proveedor.IdComercialDataAgro = respuesta.ComercialId;
            proveedor.IdDataAgro = respuesta.ProveedorId;
            proveedor.RazonSocial = respuesta.ProveedorRazonSocial;

            if (string.IsNullOrEmpty(proveedor.CodigoProveedor))
                proveedor.CodigoProveedor = FormatearCodigoProveedor(proveedor.CUIT);

            proveedor.Observaciones = "";

            string resultado;

            if (respuesta.ProveedorOperando)
            {
                resultado = "El proveedor ha sido habilitado en estado 'Aprobado' debido a que tenía contratos. De tratarse de un usuario nuevo recuerde actualizar los roles.";
                proveedor.EstadoAprobacion = EstadoAprobacion.Aprobado;

                proveedor.HistorialAprobaciones.Clear();

                var historiales = repositorio.Listar<ProveedorHistorialAprobacion>(h => h.Proveedor_Id == proveedor.Id);

                repositorio.RemoverTodos(historiales);
            }
            else
            {
                proveedor.HistorialAprobaciones.Add(hist);
                resultado = "El proveedor ha sido habilitado para cargar la documentación.";
                proveedor.EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente;
            }

            repositorio.GuardarCambios();

            resultado = "El proveedor (CUIT: " + proveedor.CUIT + ") ha sido habilitado para cargar la documentación.";

            return resultado;
        }

        public Rol ObtenerRolPorCodigo(string codigo)
        {
            return repositorio.Obtener<Rol>(u => u.Codigo.Equals(codigo));
        }

        public string ObtenerCBUProveedor(string CUITproveedor)
        {
            try
            {
                SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial respuesta = new DataAgroConsumer().ValidarCUIT(CUITproveedor);

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

        public bool ProveedorApocrifo(string CUIT)
        {
            return new DataAgroConsumer().ProveedorApocrifo(CUIT);
        }

        public SustitucionMOAWS.DataAgroServices.ResultadoAltaCampoSustentable AltaCampoSustentable(CampoProveedor campo, string kmz)
        {
            try
            {
                var respuesta = new DataAgroConsumer().AltaCampoSustentable(campo, kmz);

                return respuesta;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                var resultado = new SustitucionMOAWS.DataAgroServices.ResultadoAltaCampoSustentable();
                var error = new SustitucionMOAWS.DataAgroServices.ErrorMessage { Message = ex.Message };
                var errores = new List<SustitucionMOAWS.DataAgroServices.ErrorMessage>();
                errores.Add(error);
                resultado.Errores = errores.ToArray();
                return resultado;
            }
        }

        public List<EstadoProveedorDto> ObtenerEstadoProveedores(string[] cuits)
        {
            try
            {
                var ResultEstadoProveedores = new DataAgroConsumer().ObtenerEstadoProveedores(cuits);
                List<EstadoProveedorDto> proveedores = new List<EstadoProveedorDto>();
                foreach (var item in ResultEstadoProveedores.Contactos)
                {
                    proveedores.Add(new EstadoProveedorDto
                    {
                        CUIT = item.Cuit,
                        EstadoHomeDescripcion = item.EstadoHomeDescripcion
                    });
                }
                return proveedores.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

        }

        public string InicializarContrato(int tipoNegocioId)
        {
            try
            {
                var datosIniContrato = new DataAgroConsumer().InicializarContrato(tipoNegocioId);
                return SerializeAndSanitize(datosIniContrato);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string ObtenerDatosCompraNet(int id)
        {
            try
            {
                var datosCompraNet = new DataAgroConsumer().ObtenerDatosCompraNet(id);
                return SerializeAndSanitize(datosCompraNet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual)
        {
            try
            {
                var fijaciones = new DataAgroConsumer().ObtenerFijacionesAutomaticas(cuitProveedor, cuitCorredor, materialId, filtro, fijacionId, esVirtual);
                return SerializeAndSanitize(fijaciones);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string ValidarProveedor(int proveedorId)
        {
            try
            {
                var altaTempranaNRCO = new DataAgroConsumer().ValidarProveedor(proveedorId);
                return SerializeAndSanitize(altaTempranaNRCO);

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string HabilitarPizarra(int material, int tipoNegocio)
        {
            try
            {
                var habilitacionPizarra = new DataAgroConsumer().HabilitarPizarra(material, tipoNegocio);
                return SerializeAndSanitize(habilitacionPizarra);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string TraerPagosDiferido()
        {
            try
            {
                var habilitacionPagos = new DataAgroConsumer().TraerPagosDiferido();
                return SerializeAndSanitize(habilitacionPagos);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string HabilitarCampaña(int material)
        {
            try
            {
                var habilitacionCampaña = new DataAgroConsumer().HabilitarCampaña(material);
                return SerializeAndSanitize(habilitacionCampaña);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string TraerPrecioMoa(int tipoNegocio)
        {
            try
            {
                var precioMoaCompraNet = new DataAgroConsumer().TraerPrecioMOA(tipoNegocio);
                return SerializeAndSanitize(precioMoaCompraNet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string TraerPrecioMoaV2(int material, int tipoNegocio)
        {
            try
            {
                var precioMoaCompraNet = new DataAgroConsumer().TraerPrecioMoaV2(material, tipoNegocio);
                return SerializeAndSanitize(precioMoaCompraNet);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string AnularNegocio(int negocioId, int tipoNegocioId, string motivoRechazo)
        {
            try
            {
                if (tipoNegocioId == 1 || tipoNegocioId == 2)
                {
                    return SerializeAndSanitize(new DataAgroConsumer().AnularContrato(negocioId, motivoRechazo));
                }
                else if (tipoNegocioId == 3)
                {
                    return SerializeAndSanitize(new DataAgroConsumer().AnularFijacion(negocioId, motivoRechazo));

                }
                else
                {
                    throw new ValidationCustomException("No se puede anular este tipo de negocios.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string HabilitarSustentable()
        {
            try
            {
                var habilitacion = new DataAgroConsumer().HabilitarSustentable();
                return SerializeAndSanitize(habilitacion);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId)
        {
            try
            {
                var busqueda = new DataAgroConsumer().BuscarProveedoresConCorredor(filtroProveedor, filtro, agenteCompraId);
                return SerializeAndSanitize(busqueda);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public List<MaterialDto> BuscarMateriales()
        {
            try
            {
                var result = new DataAgroConsumer().BuscarMateriales();
                var materiales = SerializeAndSanitize(result);
                JObject json = JObject.Parse(materiales);
                var data = ((Newtonsoft.Json.Linq.JArray)((Newtonsoft.Json.Linq.JContainer)json.First).First).ToObject<List<MaterialDto>>();
                return data;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string CrearContratoAPrecio(ContratoAPrecio contrato)
        {
            try
            {
                var contratoAPrecio = ConvertirAModeloDataAgro(contrato);
                var grabarContratoResult = new DataAgroConsumer().GrabarContratoAPrecio(contratoAPrecio);
                return SerializeAndSanitize(grabarContratoResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string CrearContratoAFijar(ContratoAFijar contrato)
        {
            try
            {
                var contratoAFijar = ConvertirAModeloDataAgro(contrato);
                var grabarContratoResult = new DataAgroConsumer().GrabarContratoAFijar(contratoAFijar);
                return SerializeAndSanitize(grabarContratoResult);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string ValidarDirecto(string cuit)
        {
            try
            {
                var esValido = new DataAgroConsumer().ValidarDirecto(cuit);
                return SerializeAndSanitize(esValido);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        private TipoUsuario ObtenerTipoPorNombreCorto(string nombreCorto)
        {
            return repositorio.Obtener<TipoUsuario>(t => t.NombreCorto == nombreCorto);
        }

        private static string FormatearCodigoProveedor(string CUIT)
        {
            return string.Concat("00", CUIT.Substring(2, 8));
        }

        private static SustitucionMOAWS.DataAgroServices.Contrato ConvertirAModeloDataAgro(ContratoAPrecio contrato)
        {
            var contratoDA = new SustitucionMOAWS.DataAgroServices.Contrato();

            contratoDA.BolsaId = contrato.BolsaId;
            contratoDA.CampanaId = contrato.CampanaId;
            contratoDA.Cantidad = contrato.Cantidad;
            contratoDA.CantidadCamiones = contrato.CantidadCamiones;
            contratoDA.CalidadTercero = contrato.CalidadTercero;
            contratoDA.ClasificacionId = contrato.ClasificacionId;
            contratoDA.ComercialCreadorId = contrato.ComercialCreadorId;
            contratoDA.ComercialId = contrato.ComercialId;
            contratoDA.CondicionFijacionId = contrato.CondicionFijacionId;
            contratoDA.Consignatario = contrato.Consignatario;
            contratoDA.ContratoCorredor = contrato.ContratoCorredor;
            contratoDA.ContratoSAP = contrato.ContratoSAP;
            contratoDA.ContratoVendedor = contrato.ContratoVendedor;
            contratoDA.CorredorId = contrato.CorredorId;
            contratoDA.DestinoId = contrato.DestinoId;
            contratoDA.DiasPesificado = contrato.DiasPesificado;
            contratoDA.DolarizadoTercero = contrato.DolarizadoTercero;
            contratoDA.EstadoId = contrato.EstadoId;
            contratoDA.EstablecimientoPropio = contrato.EstablecimientoPropio;
            contratoDA.Fecha = contrato.Fecha;
            contratoDA.FechaDesde = contrato.FechaDesde;
            contratoDA.FechaEntrega = contrato.FechaEntrega;
            contratoDA.FechaHasta = contrato.FechaHasta;
            contratoDA.FechaOperacion = contrato.FechaOperacion;
            contratoDA.Id = contrato.Id;
            contratoDA.ImporteSustentable = contrato.ImporteSustentable;
            contratoDA.LocalidadId = contrato.LocalidadId;
            contratoDA.MaterialId = contrato.MaterialId;
            contratoDA.MonedaId = contrato.MonedaId;
            contratoDA.MonedaSustentableId = contrato.MonedaSustentableId;
            contratoDA.Observacion = contrato.Observacion;
            contratoDA.ObservacionTercero = contrato.ObservacionTercero;
            contratoDA.PagoDiferidoTercero = contrato.PagoDiferidoTercero;
            contratoDA.Pizarra = contrato.Pizarra;
            contratoDA.PlanCanje = contrato.PlanCanje;
            contratoDA.PorcentajeDePago = contrato.PorcentajeDePago;
            contratoDA.Precio = contrato.Precio;
            contratoDA.PrecioNeto = contrato.PrecioNeto;
            contratoDA.ProveedorCreadorId = contrato.ProveedorCreadorId;
            contratoDA.ProveedorId = contrato.ProveedorId;
            contratoDA.ProvinciaId = contrato.ProvinciaId;
            contratoDA.StandardDeCalidadId = contrato.StandardDeCalidadId;
            contratoDA.Sustentable = contrato.Sustentable ?? false;
            contratoDA.SustentableTercero = contrato.SustentableTercero;
            contratoDA.TipoNegocioId = contrato.TipoNegocioId;
            contratoDA.UsuarioTercero = contrato.UsuarioTercero;
            contratoDA.ZonaId = contrato.ZonaId;

            return contratoDA;
        }

        private static SustitucionMOAWS.DataAgroServices.Contrato ConvertirAModeloDataAgro(ContratoAFijar contrato)
        {
            var contratoDA = new SustitucionMOAWS.DataAgroServices.Contrato();

            contratoDA.BolsaId = contrato.BolsaId;
            contratoDA.CalidadTercero = contrato.CalidadTercero;
            contratoDA.CampanaId = contrato.CampanaId;
            contratoDA.Cantidad = contrato.Cantidad;
            contratoDA.CantidadCamiones = contrato.CantidadCamiones;
            contratoDA.ClasificacionId = contrato.ClasificacionId;
            contratoDA.ComercialCreadorId = contrato.ComercialCreadorId;
            contratoDA.ComercialId = contrato.ComercialId;
            contratoDA.Consignatario = contrato.Consignatario;
            contratoDA.CondicionFijacionId = contrato.CondicionFijacionId;
            contratoDA.ContratoCorredor = contrato.ContratoCorredor;
            contratoDA.ContratoSAP = contrato.ContratoSAP;
            contratoDA.ContratoVendedor = contrato.ContratoVendedor;
            contratoDA.CorredorId = contrato.CorredorId;
            contratoDA.DestinoId = contrato.DestinoId;
            contratoDA.DesdeFijacion = contrato.DesdeFijacion;
            contratoDA.EstadoId = contrato.EstadoId;
            contratoDA.EstablecimientoPropio = contrato.EstablecimientoPropio;
            contratoDA.Fecha = contrato.Fecha;
            contratoDA.FechaDesde = contrato.FechaDesde;
            contratoDA.FechaEntrega = contrato.FechaEntrega;
            contratoDA.FechaHasta = contrato.FechaHasta;
            contratoDA.FechaOperacion = contrato.FechaOperacion;
            contratoDA.HastaFijacion = contrato.HastaFijacion;
            contratoDA.Id = contrato.Id;
            contratoDA.ImporteSustentable = contrato.ImporteSustentable;
            contratoDA.LocalidadId = contrato.LocalidadId;
            contratoDA.MaterialId = contrato.MaterialId;
            contratoDA.MonedaId = contrato.MonedaId;
            contratoDA.MonedaSustentableId = contrato.MonedaSustentableId;
            contratoDA.Observacion = contrato.Observacion;
            contratoDA.ObservacionTercero = contrato.ObservacionTercero;
            contratoDA.PlanCanje = contrato.PlanCanje;
            contratoDA.PorcentajeDePago = contrato.PorcentajeDePago;
            contratoDA.Precio = contrato.Precio;
            contratoDA.PrecioNeto = contrato.PrecioNeto;
            contratoDA.ProveedorCreadorId = contrato.ProveedorCreadorId;
            contratoDA.ProveedorId = contrato.ProveedorId;
            contratoDA.ProvinciaId = contrato.ProvinciaId;
            contratoDA.StandardDeCalidadId = contrato.StandardDeCalidadId;
            contratoDA.Sustentable = contrato.Sustentable ?? false;
            contratoDA.SustentableTercero = contrato.SustentableTercero;
            contratoDA.TipoNegocioId = contrato.TipoNegocioId;
            contratoDA.UsuarioTercero = contrato.UsuarioTercero;
            contratoDA.ZonaId = contrato.ZonaId;

            return contratoDA;
        }

        public string GrabarFijacion(ContratoFijacion contratoFijacion)
        {
            try
            {
                var fijacionDePrecioContrato = new SustitucionMOAWS.DataAgroServices.FijacionDePrecioContrato
                {
                    TipoNegocioId = contratoFijacion.TipoNegocioId,
                    Id = contratoFijacion.Id,
                    MaterialId = contratoFijacion.MaterialId,
                    Cantidad = contratoFijacion.Cantidad,
                    Precio = contratoFijacion.Precio,
                    PrecioNeto = contratoFijacion.PrecioNeto,
                    CampanaId = contratoFijacion.CampanaId,
                    MonedaId = contratoFijacion.MonedaId,
                    ComercialId = contratoFijacion.ComercialId,
                    ContratoSAP = contratoFijacion.ContratoSAP,
                    PorcentajeDePago = contratoFijacion.PorcentajeDePago,
                    ComercialCreadorId = contratoFijacion.ComercialCreadorId,
                    EstadoId = contratoFijacion.EstadoId,
                    Observacion = contratoFijacion.Observacion,
                    DestinoId = contratoFijacion.DestinoId,
                    BoletoId = contratoFijacion.BoletoId,
                    BolsaId = contratoFijacion.BolsaId,
                    ProveedorId = contratoFijacion.ProveedorId,
                    CorredorId = contratoFijacion.CorredorId,
                    DiasPesificado = contratoFijacion.DiasPesificado,
                    FechaOperacion = contratoFijacion.FechaOperacion,
                    FechaEntrega = contratoFijacion.FechaEntrega,
                    FechaDesde = contratoFijacion.FechaDesde,
                    Fecha = contratoFijacion.Fecha,
                    FechaHasta = contratoFijacion.FechaHasta,
                    ProveedorCreadorId = contratoFijacion.ProveedorCreadorId,
                    StandardDeCalidadId = contratoFijacion.StandardDeCalidadId,
                    CondicionFijacionId = contratoFijacion.CondicionFijacionId,
                    CantidadCamiones = contratoFijacion.CantidadCamiones,
                    ImporteSustentable = contratoFijacion.ImporteSustentable,
                    EstablecimientoPropio = contratoFijacion.EstablecimientoPropio,
                    Consignatario = contratoFijacion.Consignatario,
                    PlanCanje = contratoFijacion.PlanCanje,
                    ZonaId = contratoFijacion.ZonaId,
                    Pizarra = contratoFijacion.Pizarra,
                    ContratoId = contratoFijacion.ContratoId,
                    Posicion = contratoFijacion.Posicion,
                    ObservacionTercero = contratoFijacion.ObservacionTercero,
                    CalidadTercero = contratoFijacion.CalidadTercero,
                    PagoDiferidoTercero = contratoFijacion.PagoDiferidoTercero,
                    DolarizadoTercero = contratoFijacion.DolarizadoTercero,
                    TrigoEspecial = contratoFijacion.TrigoEspecial,
                    UsuarioTercero = contratoFijacion.UsuarioTercero
                };

                var busqueda = new DataAgroConsumer().GrabarFijacion(fijacionDePrecioContrato);
                return SerializeAndSanitize(busqueda);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string ObteneContratosAcuerdo(int corredorId)
        {
            try
            {
                var contratos = new DataAgroConsumer().TraerContratosAcuerdoPorCorredor(corredorId);
                return SerializeAndSanitize(contratos);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public List<GrabarContratoResult> CrearContratoMasivo(List<BasicoContrato> contratos)
        {
            try
            {
                // Mapear modelos MOA a los DTOs del consumer de DataAgro
                var contratosDto = contratos.Select(c => new SustitucionMOAWS.DataAgroServices.BasicoContrato
                {
                    ContratoAcuerdoId = c.ContratoAcuerdoId,
                    CorredorId = c.CorredorId,
                    ContratoCorredor = c.ContratoCorredor,
                    ContratoVendedor = c.ContratoVendedor,
                    MaterialId = c.MaterialId,
                    CampanaId = c.CampanaId,
                    FechaOperacion = c.FechaOperacion,
                    FechaDesde = c.FechaDesde,
                    FechaHasta = c.FechaHasta,
                    FechaEntrega = c.FechaEntrega,
                    Cantidad = c.Cantidad,
                    Cuit = c.Cuit,
                    ClasificacionId = c.ClasificacionId,
                    PlanCanje = c.PlanCanje,
                    Consignatario = c.Consignatario,
                    DestinoId = c.DestinoId,
                    LocalidadId = c.LocalidadId,
                    ProvinciaId = c.ProvinciaId,
                    Observacion = c.Observacion,
                    UsuarioTercero = c.UsuarioTercero,
                }).ToArray();

                // Llamada al consumer que ejecuta la operación en DataAgro
                var rawResult = new DataAgroConsumer().GrabarContratoMasivo(contratosDto);

                var json = JsonConvert.SerializeObject(rawResult);
                var result = JsonConvert.DeserializeObject<List<GrabarContratoResult>>(json);

                return result ?? new List<GrabarContratoResult>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string TraerContratoCompleto(int negocioId, int tipoNegocioId)
        {
            try
            {
                if (tipoNegocioId == 1 || tipoNegocioId == 2)
                {
                    return SerializeAndSanitize(new DataAgroConsumer().TraerContratoCompleto(negocioId, null));
                }
                else if (tipoNegocioId == 3)
                {
                    return SerializeAndSanitize(new DataAgroConsumer().TraerFijacionCompleto(negocioId));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public byte[] ObtenerExcelModeloAltaMasiva()
        {
            try
            {
                var result = new DataAgroConsumer().ExcelModeloAltaMasiva();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }


        private static string SerializeAndSanitize(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            return json.Replace("ñ", "ni");
        }
    }
}
