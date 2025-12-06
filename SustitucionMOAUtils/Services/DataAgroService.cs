using Kendo.DynamicLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
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

        public string ConfiguracionBolsaAutomatica()
        {
            try
            {
                var resp = new DataAgroConsumer().ConfiguracionBolsaAutomatica();
                return SerializeAndSanitize(resp);
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

        public List<CentroDto> BuscarCentros()
        {
            try
            {
                var centrosDA = new DataAgroConsumer().BuscarCentro();

                var centros = new List<CentroDto>();

                if (centrosDA?.Datos != null && centrosDA.Datos.Length > 0)
                {
                    centros = centrosDA.Datos.Select(c => new CentroDto
                    {
                        CodigoSap = c.CodigoSap,
                        Descripcion = c.Descripcion,
                        Id = c.Id
                    }).ToList();
                }

                return centros;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public List<CampaniaDto> BuscarCampanias()
        {
            try
            {
                var campanasDA = new DataAgroConsumer().BuscarCampanas();

                var campanias = new List<CampaniaDto>();

                if (campanasDA != null && campanasDA.Length > 0)
                {
                    campanias = campanasDA.Select(c => new CampaniaDto
                    {
                        CampaniaId = c.CampañaId,
                        Descripcion = c.Descripcion
                    }).ToList();
                }

                return campanias;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public List<LocalidadDto> ListarLocalidades()
        {
            try
            {
                var localidadesDA = new DataAgroConsumer().ListarLocalidades();

                var localidades = new List<LocalidadDto>();

                if (localidadesDA != null && localidadesDA.Length > 0)
                {
                    localidades = localidadesDA.Select(l => new LocalidadDto
                    {
                        CodLocalidad = l.CodLocalidad,
                        LocalidadId = l.LocalidadId,
                        Nombre = l.Nombre,
                        PartidoId = l.PartidoId,
                        Partido_Nombre = l.Partido_Nombre,
                        ProvinciaId = l.ProvinciaId,
                        Provincia_Nombre = l.Provincia_Nombre
                    }).ToList();
                }

                return localidades;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public List<PartidoDto> ListarPartidos()
        {
            try
            {
                var partidosDA = new DataAgroConsumer().ListarPartidos();

                var partidos = new List<PartidoDto>();

                if (partidosDA != null && partidosDA.Length > 0)
                {
                    partidos = partidosDA.Select(p => new PartidoDto
                    {
                        Descripcion = p.Descripcion,
                        Id = p.Id,
                        Provincia = p.Provincia,
                        ProvinciaId = p.ProvinciaId
                    }).ToList();
                }

                return partidos;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public string GetContratos(DataSourceRequest request)
        {
            try
            {
                var filtro = new SustitucionMOAWS.DataAgroServices.KendoDataSourceRequestDto
                {
                    Filter = new SustitucionMOAWS.DataAgroServices.KendoFilterDto
                    {
                        field = request.Filter.Field,
                        logic = request.Filter.Logic,
                        @operator = request.Filter.Operator,
                        value = request.Filter.Value?.ToString(),
                        filters = request.Filter?.Filters?.Select(f => ConvertirFiltroKendo(f)).ToArray()
                    },
                    Skip = request.Skip,
                    Take = request.Take,
                    Sort = request.Sort?.Select(s => new SustitucionMOAWS.DataAgroServices.KendoSortDto
                    {
                        field = s.Field,
                        dir = s.Dir
                    }).ToArray()
                };

                var resultDto = new DataAgroConsumer().BuscaDatosTablaContrato(filtro);
                return SerializeAndSanitize(resultDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
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

        public List<SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto> CrearContratoMasivo(List<BasicoContrato> contratos)
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
                var result = JsonConvert.DeserializeObject<List<SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto>>(json);

                return result ?? new List<SustitucionMOAModel.Models.DataAgro.GrabarContratoResultDto>();
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

        public BasicoContrato TraerContratoCompleto(int id, string tipo)
        {
            try
            {
                var basicoContratoDA = new DataAgroConsumer().TraerContratoCompleto(id, tipo);

                var basicoContrato = ConvertirAModeloOperaciones(basicoContratoDA);

                return basicoContrato;
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
            contratoDA.Fecha = DateTime.SpecifyKind(contrato.Fecha.Date, DateTimeKind.Unspecified);
            contratoDA.FechaDesde = DateTime.SpecifyKind(contrato.FechaDesde.Date, DateTimeKind.Unspecified);
            contratoDA.FechaEntrega = DateTime.SpecifyKind(contrato.FechaEntrega.Date, DateTimeKind.Unspecified);
            contratoDA.FechaHasta = DateTime.SpecifyKind(contrato.FechaHasta.Date, DateTimeKind.Unspecified);
            contratoDA.FechaOperacion = DateTime.SpecifyKind(contrato.FechaOperacion.Date, DateTimeKind.Unspecified);
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
            contratoDA.BoletoId = contrato.BoletoId;

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

        private static BasicoContrato ConvertirAModeloOperaciones(SustitucionMOAWS.DataAgroServices.BasicoContrato modeloDA)
        {
            var basicoContrato = new BasicoContrato();

            basicoContrato.Acuerdo = modeloDA.Acuerdo;
            basicoContrato.AcuerdoId = modeloDA.AcuerdoId;
            basicoContrato.AgenteId = modeloDA.AgenteId;
            basicoContrato.Ampliaciones = modeloDA.Ampliaciones;
            basicoContrato.Anticipo = modeloDA.Anticipo;
            basicoContrato.AperturaPrecios = modeloDA.AperturaPrecios?.Select(x => ConvertirAModeloOperaciones(x)).ToList();
            basicoContrato.Base = modeloDA.Base;
            basicoContrato.BoletoDescripcion = modeloDA.BoletoDescripcion;
            basicoContrato.BoletoId = modeloDA.BoletoId;
            basicoContrato.BolsaDescripcion = modeloDA.BolsaDescripcion;
            basicoContrato.BolsaId = modeloDA.BolsaId;
            basicoContrato.CD = modeloDA.CD;
            basicoContrato.CUITCorredor = modeloDA.CUITCorredor;
            basicoContrato.CalidadDescripcion = modeloDA.CalidadDescripcion;
            basicoContrato.CalidadTercero = modeloDA.CalidadTercero;
            basicoContrato.Calidades = modeloDA.Calidades?.Select(x => ConvertirAModeloOperaciones(x)).ToList();
            basicoContrato.CampanaId = modeloDA.CampanaId;
            basicoContrato.CampanaMaterialId = modeloDA.CampanaMaterialId;
            basicoContrato.Campania = modeloDA.Campania;
            basicoContrato.Canje = modeloDA.Canje;
            basicoContrato.Cantidad = modeloDA.Cantidad;
            basicoContrato.CantidadCamiones = modeloDA.CantidadCamiones;
            basicoContrato.CantidadMaximaCupo = modeloDA.CantidadMaximaCupo;
            basicoContrato.CaratulaExtension = modeloDA.CaratulaExtension;
            basicoContrato.CaratulaMAT = modeloDA.CaratulaMAT;
            basicoContrato.Cesion = modeloDA.Cesion;
            basicoContrato.ChequeElectronico = modeloDA.ChequeElectronico;
            basicoContrato.ChequeElectronicoValor = modeloDA.ChequeElectronicoValor;
            basicoContrato.ClasificacionContrato = modeloDA.ClasificacionContrato;
            basicoContrato.ClasificacionDescripcion = modeloDA.ClasificacionDescripcion;
            basicoContrato.ClasificacionId = modeloDA.ClasificacionId;
            basicoContrato.Comercial = modeloDA.Comercial;
            basicoContrato.ComercialCreador = modeloDA.ComercialCreador;
            basicoContrato.ComercialCreadorId = modeloDA.ComercialCreadorId;
            basicoContrato.ComercialId = modeloDA.ComercialId;
            basicoContrato.ComercialZonaDescripcion = modeloDA.ComercialZonaDescripcion;
            basicoContrato.ComercialZonaId = modeloDA.ComercialZonaId;
            basicoContrato.Compensacion = modeloDA.Compensacion;
            basicoContrato.CondicionFijacion = modeloDA.CondicionFijacion;
            basicoContrato.CondicionFijacionDescripcion = modeloDA.CondicionFijacionDescripcion;
            basicoContrato.Consignatario = modeloDA.Consignatario;
            basicoContrato.ContratoAcuerdoId = modeloDA.ContratoAcuerdoId;
            basicoContrato.ContratoCorredor = modeloDA.ContratoCorredor;
            basicoContrato.ContratoId = modeloDA.ContratoId;
            basicoContrato.ContratoMadre = modeloDA.ContratoMadre;
            basicoContrato.ContratoSAP = modeloDA.ContratoSAP;
            basicoContrato.ContratoVendedor = modeloDA.ContratoVendedor;
            basicoContrato.Corredor = modeloDA.Corredor;
            basicoContrato.CorredorId = modeloDA.CorredorId;
            basicoContrato.Cuit = modeloDA.Cuit;
            basicoContrato.DatosFijacion = ConvertirAModeloOperaciones(modeloDA.DatosFijacion);
            basicoContrato.Descuentos = modeloDA.Descuentos?.Select(x => ConvertirAModeloOperaciones(x)).ToList();
            basicoContrato.DesdeFijacion = modeloDA.DesdeFijacion;
            basicoContrato.DesdeFijacionFormateado = modeloDA.DesdeFijacionFormateado;
            basicoContrato.DestinoDescripcion = modeloDA.DestinoDescripcion;
            basicoContrato.DestinoId = modeloDA.DestinoId;
            basicoContrato.Dias_Pesificado = modeloDA.Dias_Pesificado;
            basicoContrato.Dolarizado = modeloDA.Dolarizado;
            basicoContrato.DolarizadoCorredor = modeloDA.DolarizadoCorredor;
            basicoContrato.DolarizadoExpress = modeloDA.DolarizadoExpress;
            basicoContrato.DolarizadoExpressValor = modeloDA.DolarizadoExpressValor;
            basicoContrato.DolarizadoTercero = modeloDA.DolarizadoTercero;
            basicoContrato.DolarizadoValor = modeloDA.DolarizadoValor;
            basicoContrato.EsFason = modeloDA.EsFason;
            basicoContrato.EstablecimientoPropio = modeloDA.EstablecimientoPropio;
            basicoContrato.Estado = modeloDA.Estado;
            basicoContrato.Estado_Contrato = modeloDA.Estado_Contrato;
            basicoContrato.Estado_Order = modeloDA.Estado_Order;
            basicoContrato.FasonId = modeloDA.FasonId;
            basicoContrato.Fecha = modeloDA.Fecha;
            basicoContrato.FechaCierta = modeloDA.FechaCierta;
            basicoContrato.FechaCiertaFormateado = modeloDA.FechaCiertaFormateado;
            basicoContrato.FechaConfirmacion = modeloDA.FechaConfirmacion;
            basicoContrato.FechaDesde = modeloDA.FechaDesde;
            basicoContrato.FechaDesdeFormateado = modeloDA.FechaDesdeFormateado;
            basicoContrato.FechaDesde_Sustentable = modeloDA.FechaDesde_Sustentable;
            basicoContrato.FechaDesde_SustentableFormateado = modeloDA.FechaDesde_SustentableFormateado;
            basicoContrato.FechaEntrega = modeloDA.FechaEntrega;
            basicoContrato.FechaFormateado = modeloDA.FechaFormateado;
            basicoContrato.FechaHasta = modeloDA.FechaHasta;
            basicoContrato.FechaHastaFormateado = modeloDA.FechaHastaFormateado;
            basicoContrato.FechaHasta_Sustentable = modeloDA.FechaHasta_Sustentable;
            basicoContrato.FechaHasta_SustentableFormateado = modeloDA.FechaHasta_SustentableFormateado;
            basicoContrato.FechaOperacion = modeloDA.FechaOperacion;
            basicoContrato.FechaOperacionFormateado = modeloDA.FechaOperacionFormateado;
            basicoContrato.Fecha_Dolarizado = modeloDA.Fecha_Dolarizado;
            basicoContrato.Fecha_DolarizadoFormateado = modeloDA.Fecha_DolarizadoFormateado;
            basicoContrato.Fecha_Order = modeloDA.Fecha_Order;
            basicoContrato.FijacionDePrecioContratoId = modeloDA.FijacionDePrecioContratoId;
            basicoContrato.GrupoCompra = modeloDA.GrupoCompra;
            basicoContrato.GrupoCompraDescripcion = modeloDA.GrupoCompraDescripcion;
            basicoContrato.HastaFijacion = modeloDA.HastaFijacion;
            basicoContrato.HastaFijacionFormateado = modeloDA.HastaFijacionFormateado;
            basicoContrato.Hora = modeloDA.Hora;
            basicoContrato.Id = modeloDA.Id;
            basicoContrato.ImporteBonificacion = modeloDA.ImporteBonificacion;
            basicoContrato.ImporteComision = modeloDA.ImporteComision;
            basicoContrato.ImporteFinanciero = modeloDA.ImporteFinanciero;
            basicoContrato.ImporteRedespacho = modeloDA.ImporteRedespacho;
            basicoContrato.Importe_Sustentable = modeloDA.Importe_Sustentable;
            basicoContrato.Insumo = modeloDA.Insumo;
            basicoContrato.Localidad = modeloDA.Localidad;
            basicoContrato.LocalidadId = modeloDA.LocalidadId;
            basicoContrato.Madre = modeloDA.Madre;
            basicoContrato.Material = modeloDA.Material;
            basicoContrato.MaterialId = modeloDA.MaterialId;
            basicoContrato.MercsDeposito = modeloDA.MercsDeposito;
            basicoContrato.MesPosicion = modeloDA.MesPosicion;
            basicoContrato.Moneda = modeloDA.Moneda;
            basicoContrato.MonedaAjusteComisionId = modeloDA.MonedaAjusteComisionId;
            basicoContrato.MonedaBonificacion = modeloDA.MonedaBonificacion;
            basicoContrato.MonedaCanjeId = modeloDA.MonedaCanjeId;
            basicoContrato.MonedaId = modeloDA.MonedaId;
            basicoContrato.MonedaId_Sustentable = modeloDA.MonedaId_Sustentable;
            basicoContrato.Moneda_Sustentable = modeloDA.Moneda_Sustentable;
            basicoContrato.Monto = modeloDA.Monto;
            basicoContrato.MotivoOperacionAnterior = modeloDA.MotivoOperacionAnterior;
            basicoContrato.Negocio = modeloDA.Negocio;
            basicoContrato.NivelTarifa = modeloDA.NivelTarifa;
            basicoContrato.NivelTarifaId = modeloDA.NivelTarifaId;
            basicoContrato.NoInformaSIO = modeloDA.NoInformaSIO;
            basicoContrato.Observacion = modeloDA.Observacion;
            basicoContrato.ObservacionTercero = modeloDA.ObservacionTercero;
            basicoContrato.OcultarEnTablero = modeloDA.OcultarEnTablero;
            basicoContrato.Operador = modeloDA.Operador;
            basicoContrato.OperadorId = modeloDA.OperadorId;
            basicoContrato.PagoCBU = modeloDA.PagoCBU;
            basicoContrato.PagoDiferido = modeloDA.PagoDiferido;
            basicoContrato.PagoDiferidoTercero = modeloDA.PagoDiferidoTercero;
            basicoContrato.PagoDiferidoTerceroId = modeloDA.PagoDiferidoTerceroId;
            basicoContrato.PagoDirectoVendedor = modeloDA.PagoDirectoVendedor;
            basicoContrato.Pesificado = modeloDA.Pesificado;
            basicoContrato.Pizarra = modeloDA.Pizarra;
            basicoContrato.PlanCanje = modeloDA.PlanCanje;
            basicoContrato.PlantaDestinoDescripcion = modeloDA.PlantaDestinoDescripcion;
            basicoContrato.PlantaDestinoId = modeloDA.PlantaDestinoId;
            basicoContrato.PorcentajeBonificacion = modeloDA.PorcentajeBonificacion;
            basicoContrato.PorcentajeComision = modeloDA.PorcentajeComision;
            basicoContrato.PorcentajeDePago = modeloDA.PorcentajeDePago;
            basicoContrato.Posicion = modeloDA.Posicion;
            basicoContrato.Precio = modeloDA.Precio;
            basicoContrato.PrecioAjusteComision = modeloDA.PrecioAjusteComision;
            basicoContrato.PrecioNeto = modeloDA.PrecioNeto;
            basicoContrato.PrecioPlazo = modeloDA.PrecioPlazo;
            basicoContrato.PreciosPactados = modeloDA.PreciosPactados?.Select(x => ConvertirAModeloOperaciones(x)).ToList();
            basicoContrato.PrestamoDevolucion = modeloDA.PrestamoDevolucion;
            basicoContrato.Proveedor = modeloDA.Proveedor;
            basicoContrato.ProveedorId = modeloDA.ProveedorId;
            basicoContrato.Provincia = modeloDA.Provincia;
            basicoContrato.ProvinciaId = modeloDA.ProvinciaId;
            basicoContrato.Rechazo = modeloDA.Rechazo;
            basicoContrato.SelCargoMOA = modeloDA.SelCargoMOA;
            basicoContrato.SelCargoVendedor = modeloDA.SelCargoVendedor;
            basicoContrato.StandardCalidadId = modeloDA.StandardDeCalidadId;
            basicoContrato.StandardDeCalidadDescripcion = modeloDA.StandardDeCalidadDescripcion;
            basicoContrato.Sustentable = modeloDA.Sustentable;
            basicoContrato.SustentableTercero = modeloDA.SustentableTercero;
            basicoContrato.TarifaFlete = modeloDA.TarifaFlete;
            basicoContrato.TipoAgenteCompra = modeloDA.TipoAgenteCompra;
            basicoContrato.TipoAgenteCompraId = modeloDA.TipoAgenteCompraId;
            basicoContrato.TipoFason = modeloDA.TipoFason;
            basicoContrato.TipoFasonId = modeloDA.TipoFasonId;
            basicoContrato.TipoNegocio = modeloDA.TipoNegocio;
            basicoContrato.TipoNegocioId = modeloDA.TipoNegocioId;
            basicoContrato.TrigoEspecial = modeloDA.TrigoEspecial;
            basicoContrato.UsuarioConfirmador = modeloDA.UsuarioConfirmador;
            basicoContrato.UsuarioId = modeloDA.UsuarioId;
            basicoContrato.UsuarioTercero = modeloDA.UsuarioTercero;
            basicoContrato.Venta = modeloDA.Venta;
            basicoContrato.Warrant = modeloDA.Warrant;
            basicoContrato.ZonaDescripcion = modeloDA.ZonaDescripcion;
            basicoContrato.ZonaId = modeloDA.ZonaId;

            return basicoContrato;
        }

        private static AperturaPrecioDto ConvertirAModeloOperaciones(SustitucionMOAWS.DataAgroServices.AperturaPrecioDto x)
        {
            return x == null ? null : new AperturaPrecioDto
            {
                ConceptoAperturaPrecio = x.ConceptoAperturaPrecio,
                ConceptoAperturaPrecioId = x.ConceptoAperturaPrecioId,
                contratoId = x.contratoId,
                FijacionId = x.FijacionId,
                Id = x.Id,
                Importe = x.Importe,
                Moneda = x.Moneda,
                MonedaId = x.MonedaId,
                Porcentaje = x.Porcentaje
            };
        }

        private static PrecioPactadosDto ConvertirAModeloOperaciones(SustitucionMOAWS.DataAgroServices.PrecioPactadosDto modeloDA)
        {
            return modeloDA == null ? null : new PrecioPactadosDto
            {
                ContratoId = modeloDA.ContratoId,
                FechaDesde = modeloDA.FechaDesde,
                FechaHasta = modeloDA.FechaHasta,
                Id = modeloDA.Id,
                ImportePactado = modeloDA.ImportePactado,
                MonedaImportePactadoDesc = modeloDA.MonedaImportePactadoDesc,
                MonedaImportePactadoId = modeloDA.MonedaImportePactadoId,
                MonedaPactadoDesc = modeloDA.MonedaPactadoDesc,
                MonedaPactadoId = modeloDA.MonedaPactadoId,
                Porcentaje = modeloDA.Porcentaje,
                Precio = modeloDA.Precio
            };
        }

        private static DescuentoBonificacionDto ConvertirAModeloOperaciones(SustitucionMOAWS.DataAgroServices.DescuentoBonificacionDto modeloDA)
        {
            return new DescuentoBonificacionDto
            {
                ContratoId = modeloDA.ContratoId,
                FechaDesde = modeloDA.FechaDesde,
                FechaHasta = modeloDA.FechaHasta,
                Id = modeloDA.Id,
                Importe = modeloDA.Importe,
                Moneda = modeloDA.Moneda,
                MonedaId = modeloDA.MonedaId,
                Porcentaje = modeloDA.Porcentaje,
                TipoDBDesc = modeloDA.TipoDBDesc,
                TipoDBId = modeloDA.TipoDBId,
                TipoPeriodoDBDesc = modeloDA.TipoPeriodoDBDesc,
                TipoPeriodoDBId = modeloDA.TipoPeriodoDBId
            };
        }

        private static DatosFijacionDeContratoDto ConvertirAModeloOperaciones(SustitucionMOAWS.DataAgroServices.DatosFijacionDeContratoDto modeloDA)
        {
            return modeloDA == null ? null : new DatosFijacionDeContratoDto
            {
                Anticipo = modeloDA.Anticipo,
                ARecibirSinPrecio = modeloDA.ARecibirSinPrecio,
                Calidad = modeloDA.Calidad,
                Calidades = modeloDA.Calidades?.Select(x => ConvertirAModeloOperaciones(x)).ToList(),
                Campana = modeloDA.Campana,
                CampanaId = modeloDA.CampanaId,
                Centro = modeloDA.Centro,
                CentroDescripcion = modeloDA.CentroDescripcion,
                Cesion = modeloDA.Cesion,
                ChequeElectronico = modeloDA.ChequeElectronico,
                Clasificacion = modeloDA.Clasificacion,
                Color = modeloDA.Color,
                CondicionFijacionCod = modeloDA.CondicionFijacionCod,
                CondicionFijacionDescripcion = modeloDA.CondicionFijacionDescripcion,
                CondicionPagoCod = modeloDA.CondicionPagoCod,
                CondicionPagoDescripcion = modeloDA.CondicionPagoDescripcion,
                ContratoId = modeloDA.ContratoId,
                DesdeEntrega = modeloDA.DesdeEntrega,
                FechaDesde = modeloDA.FechaDesde,
                FechaHasta = modeloDA.FechaHasta,
                FijacionSap = modeloDA.FijacionSap,
                Filtro = modeloDA.Filtro,
                HastaEntrega = modeloDA.HastaEntrega,
                ImporteAPrecio = modeloDA.ImporteAPrecio,
                ImporteSobrePrecio = modeloDA.ImporteSobrePrecio,
                KilosAplicados = modeloDA.KilosAplicados,
                KilosContrato = modeloDA.KilosContrato,
                KilosPendiente = modeloDA.KilosPendiente,
                MonedaAPrecio = modeloDA.MonedaAPrecio,
                MonedaSobrePrecio = modeloDA.MonedaSobrePrecio,
                PagoDiferido = modeloDA.PagoDiferido,
                PorcentajeAPrecio = modeloDA.PorcentajeAPrecio,
                PorcentajeSobrePrecio = modeloDA.PorcentajeSobrePrecio,
                Posicion = modeloDA.Posicion,
                RecibidoSinFijar = modeloDA.RecibidoSinFijar
            };
        }

        private static CalidadDto ConvertirAModeloOperaciones(SustitucionMOAWS.DataAgroServices.CalidadDto modeloDA)
        {
            return new CalidadDto
            {
                AcuerdoId = modeloDA.AcuerdoId,
                CalidadEspecialDesc = modeloDA.CalidadEspecialDesc,
                CalidadEspecialId = modeloDA.CalidadEspecialId,
                ContratoId = modeloDA.ContratoId,
                Id = modeloDA.Id,
                PorcentajeDesde = modeloDA.PorcentajeDesde,
                PorcentajeHasta = modeloDA.PorcentajeHasta,
                Valor = modeloDA.Valor
            };
        }

        private SustitucionMOAWS.DataAgroServices.KendoFilterDto ConvertirFiltroKendo(Kendo.DynamicLinq.Filter filter)
        {
            return new SustitucionMOAWS.DataAgroServices.KendoFilterDto
            {
                field = filter.Field,
                logic = filter.Logic,
                @operator = filter.Operator,
                value = filter.Value,
                filters = filter.Filters?.Select(f => ConvertirFiltroKendo(f)).ToArray()
            };
        }

        private static string SerializeAndSanitize(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            return json.Replace("ñ", "ni");
        }
    }
}
