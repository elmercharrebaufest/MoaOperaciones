using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using ScatoWS = SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaFasonService : OrdenDeCargaServiceBase, IOrdenDeCargaFasonService
    {
        private readonly IEnumerable<string> codigosRetiroEnPatagonia = new string[] { "98855", "99098" };
        private readonly IEmailFasonService emailFasonService;

        public OrdenDeCargaFasonService(IRepositorio repositorio,
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IScatoConsumer scatoConsumer,
            IScatoRepositorioClient scatoRepositorioClient,
            ICNRTClient cNRTClient,
            IEmailFasonService emailFasonService,
            IFeriadoService feriadoService
            ) : base(ordenCargaConsumer, scatoConsumer, scatoRepositorioClient, repositorio, cNRTClient, feriadoService)
        {
            this.emailFasonService = emailFasonService;
        }

        public ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request)
        {
            Log.Info($"Listar(request: {request.ToJson()})");

            try
            {
                var fechaIncioDateTime = DataFormatter.StringToDateTime(request.FechaDesde, "");
                var fechaFinDateTime = DataFormatter.StringToDateTime(request.FechaHasta, "");

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == request.MailUsuario);
                var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);
                fechaFinDateTime = fechaFinDateTime.AddDays(1);


                var codigoProveedorClientesRelacionados = usuario.Proveedores.Select(c => c.CodigoProveedor);
                var tipoUsuarioId = usuario.TipoUsuario.Id;
                var listadoDB = repositorio.Listar<OrdenDeCargaFason>(x =>
                (esInterno || codigoProveedorClientesRelacionados.Contains(x.Cliente.CodigoProveedor))
                && x.FechaCreacion >= fechaIncioDateTime && x.FechaCreacion <= fechaFinDateTime
                && (esInterno || (tipoUsuarioId == 5 ? x.CorredorId == null : x.CorredorId != null))
                );

                if (listadoDB == null || listadoDB.Count == 0)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga fason"));
                }

                var listado = listadoDB.OrderByDescending(x => x.FechaCreacion)
                    .Select(x => new OrdenDeCargaFasonDto(x, esInterno))
                    .ToList();

                var response = new ListarOrdenDeCargaFasonResponse();

                response.Response = listado;

                return response;

            }
            catch (InfoCustomException icex)
            {
                throw icex;
            }
            catch (Exception error)
            {
                Log.Error(error);
                throw new WSCustomException(ErrorMsg.ErrorWS, error);
            }

        }

        public DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario)
        {
            try
            {

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario.MailUsuario);
                var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);

                var orden = repositorio.Obtener<OrdenDeCargaFason>(IdOrdenCargaFason);

                var response = new OrdenDeCargaFasonDto(orden, esInterno);

                return new DetalleOrdenDeCargaFasonResponse { Response = response };
            }

            catch (Exception error)
            {
                Log.Error(error);
                throw new WSCustomException(ErrorMsg.ErrorWS, error);
            }
        }

        public OrdenDeCargaFasonDto VerificarTransporte(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            ActualizarOrdenDeCarga(orden);

            repositorio.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }

        public List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason()
        {

            var fechaLimite = DateTime.Now.Date;

            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesDeCargaFasonJob" && a.Habilitado) == null)
                return new List<OrdenDeCargaFason>();

            var ordenes = repositorio.Listar<OrdenDeCargaFason>((orden) => orden.Estado == EstadoOrdenDeCargaFason.Generada || orden.Estado == EstadoOrdenDeCargaFason.Pendiente);

            foreach (var orden in ordenes)
            {
                if (CalcularFechaVencimiento(orden.FechaCreacion) < fechaLimite)
                    orden.Estado = EstadoOrdenDeCargaFason.Vencida;
            }
            repositorio.GuardarCambios();

            return ordenes;
        }
        public List<ProveedorDto> GetCorredores()
        {
            var corredoresBD = repositorio
                        .Listar<Usuario>(u =>
                            u.TipoUsuario.Id == (int)TipoUsuarioEnum.Corredor &&
                            u.Habilitado &&
                            u.Roles.Any(r =>
                                r.Codigo == "FASON"));

            var corredores = corredoresBD.Select(c => new ProveedorDto(c.ObtenerProveedor()));

            return corredores.ToList();
        }

        public List<ProveedorDto> GetClientesDeCorredor(string codigoCorredor)
        {
            if (!string.IsNullOrEmpty(codigoCorredor))
            {
                var corredor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoCorredor);

                var corredores = repositorio
                            .Listar<Usuario>(u =>
                                u.TipoUsuario.Id == (int)TipoUsuarioEnum.Corredor &&
                                u.CUITRegistro == corredor.CUIT &&
                                u.Habilitado);

                var proveedores = new List<ProveedorDto>();
                foreach (var corr in corredores)
                {
                    proveedores.AddRange(
                    corr.Proveedores.Where(x => x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente)
                        .Select(x => new ProveedorDto(x, false)).ToList());
                }
                return proveedores;
            }
            else
            {
                // igual pero tipo cliente
                var corredores = repositorio
                            .Listar<Usuario>(u =>
                                u.TipoUsuario.Id == (int)TipoUsuarioEnum.Cliente &&
                                u.Habilitado &&
                                u.Roles.Any(r => r.Codigo == "FASON"));

                var proveedores = new List<ProveedorDto>();
                foreach (var corr in corredores)
                {
                    proveedores.AddRange(
                    corr.Proveedores.Where(x => x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente)
                        .Select(x => new ProveedorDto(x, false)).ToList());
                }
                return proveedores;
            }
        }

        public Resultado Crear(CrearOrdenDeCargaFasonRequest request, string mailUsuario)
        {
            try
            {
                ValidarRequest(request, mailUsuario);
                var existeTransporte = TransporteExiste(request.CUITTransporte);
                var existeIntermediarioFlete = string.IsNullOrEmpty(request.CUITIntermediarioFlete) || TransporteExiste(request.CUITIntermediarioFlete);
                var localidades = ObtenerDestinos(request.Cliente);
                if (localidades.Count == 0)
                {
                    return new Resultado { error = "El cliente no cuenta con ninguna localidad, imposible continuar con la carga." };
                }
                if (request.CantidadDeViajes > 3)
                {
                    return new Resultado { error = "No puede generar más de 3(tres) viajes." };
                }
                var localidad = localidades.First();
                request.DestinatarioExisteScato = CuitExisteScato(request.CUITDestinatario);
                request.DestinoExisteScato = CuitExisteScato(request.CUITDestino);

                var producto = repositorio.Obtener<Material>(request.Producto_Id);
                int? ultimoId = null;
                for (int i = 0; i < request.CantidadDeViajes; i++)
                {
                    var ordenEntity = new OrdenDeCargaFason(request);
                    ordenEntity.Producto = producto;
                    ordenEntity.LocalidadId = localidad.LocalidadId;
                    ordenEntity.LocalidadDescripcion = localidad.LocalidadDescripcion;
                    ordenEntity.KmARecorrer = localidad.KmARecorrer;
                    var detalleActualizar = ObtenerDetallesActualizar(ordenEntity, existeTransporte, existeIntermediarioFlete);
                    ActualizarOrdenDeCarga(detalleActualizar, i == 0);
                    repositorio.Agregar(ordenEntity);

                    repositorio.GuardarCambios();
                    ultimoId = (int)ordenEntity.Id;
                }


                var resultado = new Resultado { Mensaje = SuccessMsg.OrdenDeCargaAgregada, IdEntidad = ultimoId ?? 0 };
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new WSCustomException(ErrorMsg.ErrorWS, ex);
            }
        }

        public Resultado Editar(EditarOrdenDeCargaFasonRequest request, string mailUsuario)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
                ValidarRequest(request, usuario);
                var orden = repositorio.Obtener<OrdenDeCargaFason>(request.Id);

                orden.Cantidad = request.Cantidad;
                orden.Cliente_Id = request.Cliente;
                orden.CorredorId = request.CorredorId;
                orden.CUILChofer = request.CUILChofer;
                orden.CUITTransporte = request.CUITTransporte;
                orden.FechaCreacion = DateTime.Now;
                orden.NombreChofer = request.NombreChofer;
                orden.Observacion = request.Observacion;
                orden.PatenteAcoplado = request.PatenteAcoplado;
                orden.PatenteChasis = request.PatenteChasis;
                orden.Producto_Id = request.Producto_Id;
                orden.RazonSocialTransporte = request.RazonSocialTransporte;
                orden.FleteMOA = request.FleteMOA;
                orden.CUITIntermediarioFlete = request.CUITIntermediarioFlete;
                orden.RazonSocialIntermediarioFlete = request.RazonSocialIntermediarioFlete;
                orden.ClienteComoRemitenteComercial = !string.IsNullOrEmpty(request.CUITDestino) &&
                    request.CUITDestino != request.CUITCliente.ToString();
                orden.PlantaCodigo = request.PlantaCodigo;
                orden.DomicilioTipo = request.DomicilioTipo;
                orden.DomicilioOrden = request.DomicilioOrden;
                orden.DomicilioDescr = request.DomicilioDescr;
                orden.RazonSocialDestino = request.RazonSocialDestino;
                orden.Escalable = request.Escalable;
                ActualizarOrdenDeCarga(orden);

                var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);
                if (!esAdmin && ValidarOrdenActivaScato(orden.Id))
                    throw new ValidationCustomException("La orden está en activa, imposible editar.");

                repositorio.GuardarCambios();

                return new Resultado { IdEntidad = request.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Resultado { error = ex.Message };
            }
        }

        public List<ScatoWS.KmPorProveedorDto> ObtenerDestinos(int clienteId)
        {
            Proveedor proveedor = repositorio.Obtener<Proveedor>(a => a.Id == clienteId);
            if (proveedor.CUIT.Length != 11)
            {
                throw new ValidationCustomException("El cuit no tiene el formato correcto.");
            }
            return scatoConsumer.BuscarDestinos(proveedor.CUIT);
        }
        public void VerificarTransporteJob()
        {
            if (repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "VerificarTransporteOrdenesDeCargaFasonJob" && hj.Habilitado) == null)
                return;

            var ordenes = repositorio.Listar<OrdenDeCargaFason>(orden => !orden.TransporteExiste && orden.Estado == EstadoOrdenDeCargaFason.Pendiente);
            foreach (var orden in ordenes)
            {
                ActualizarOrdenDeCarga(orden);
            }
            repositorio.GuardarCambios();
        }
        public OrdenDeCargaFasonDto ActualizarSolicitudEdicion(EstadoSolicitudEdicionFason estadoSolicitud)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(estadoSolicitud.OrdenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado != EstadoOrdenDeCargaFason.EdicionSolicitada)
                throw new InfoCustomException("Esta orden no está en estado de edición solicitada.");

            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == estadoSolicitud.MailUsuario);
            if (!usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");

            if (estadoSolicitud.Aprobado)
                ActualizarOrdenDeCarga(orden);
            else
                orden.Estado = EstadoOrdenDeCargaFason.EdicionRechazada;

            repositorio.GuardarCambios();

            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public OrdenDeCargaFasonDto SolicitarAnulacion(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado == EstadoOrdenDeCargaFason.AnulacionSolicitada)
                throw new InfoCustomException("La anulación de esta orden ya fue solicitada.");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);
            if (esAdmin)
                throw new InfoCustomException("Usuario administrador, debería anular directamente.");
            if (ValidarOrdenActivaScato(ordenId))
                throw new InfoCustomException("La orden está en activa, imposible editar.");

            orden.Estado = EstadoOrdenDeCargaFason.AnulacionSolicitada;

            repositorio.GuardarCambios();

            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public OrdenDeCargaFasonDto ActualizarSolicitudAnulacion(EstadoSolicitudAnulacionFason estadoSolicitud)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(estadoSolicitud.OrdenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado != EstadoOrdenDeCargaFason.AnulacionSolicitada)
                throw new InfoCustomException("Esta orden no está en estado de anulación solicitada.");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == estadoSolicitud.MailUsuario);
            if (!usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");


            if (estadoSolicitud.Aprobado)
                orden.Estado = EstadoOrdenDeCargaFason.Anulada;
            else
                ActualizarOrdenDeCarga(orden);

            repositorio.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public OrdenDeCargaFasonDto AnularOrden(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado == EstadoOrdenDeCargaFason.Entregada)
                throw new InfoCustomException("Esta orden no puede ser anulada, ya fue entregada.");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            if (!usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");

            orden.Estado = EstadoOrdenDeCargaFason.Anulada;

            repositorio.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public List<AutoCompleteDropdownElement> ObtenerCuilsChofer(OrdenDeCargaFasonRequest orden, string mailUsuario)
        {
            List<AutoCompleteDropdownElement> cuils = new List<AutoCompleteDropdownElement>();
            Proveedor cliente;
            if (orden.Cliente == null && orden.PatenteAcoplado == null)
            {
                cuils.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return cuils;
            }
            cliente = repositorio.Obtener<Proveedor>(
            x => x.Id == orden.Cliente &&
            x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return cuils;
            }
            cuils = repositorio.Listar<OrdenDeCargaFason, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.CUILChofer.Substring(0, 2) + "-" + x.CUILChofer.Substring(2, 8) + "-" + x.CUILChofer.Substring(10, 1),
                value = x.CUILChofer
            }, x => x.Cliente_Id == cliente.Id && x.PatenteAcoplado == orden.PatenteAcoplado);

            return cuils.Distinct().ToList();
        }

        public List<AutoCompleteDropdownElement> ObtenerCuitsTransporte(OrdenDeCargaFasonRequest orden, string mailUsuario)
        {
            List<AutoCompleteDropdownElement> cuits = new List<AutoCompleteDropdownElement>();
            Proveedor cliente;
            if (orden.Cliente == null && orden.PatenteAcoplado == null)
            {
                cuits.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return cuits;
            }
            cliente = repositorio.Obtener<Proveedor>(
            x => x.Id == orden.Cliente &&
            x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return cuits;
            }
            cuits = repositorio.Listar<OrdenDeCargaFason, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.CUITTransporte.Substring(0, 2) + "-" + x.CUITTransporte.Substring(2, 8) + "-" + x.CUITTransporte.Substring(10, 1),
                value = x.CUITTransporte
            }, x => x.Cliente_Id == cliente.Id && x.PatenteAcoplado == orden.PatenteAcoplado);

            return cuits.Distinct().ToList();
        }
        public OrdenDeCargaDto ObtenerPatentes(OrdenDeCargaFasonRequest orden, string mailUsuario)
        {
            Proveedor cliente;
            OrdenDeCargaDto result = new OrdenDeCargaDto();
            if (orden.Cliente == null)
            {
                result.ordenes.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return result;
            }
            cliente = repositorio.Obtener<Proveedor>(
            x => x.Id == orden.Cliente &&
            x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return new OrdenDeCargaDto();
            }
            result.ordenes = repositorio.Listar<OrdenDeCargaFason, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.PatenteChasis,
                value = x.PatenteAcoplado
            }, x => x.Cliente_Id == cliente.Id);
            result.ordenes = result.ordenes.Distinct().ToList();
            return result;
        }

        public bool EnviarMailAltaCuitTerceros(bool gestionaFlete, bool gestionaDestino, bool gestionaDestinatario, string ordenId)
        {
            var ordenDeCarga = this.repositorio.Obtener<OrdenDeCargaFason>(o => o.Id.ToString() == ordenId);

            if (gestionaFlete)
            {
                emailFasonService.EnviarMailAltaIntermediarioFlete(ordenDeCarga.CUITIntermediarioFlete, ordenDeCarga.RazonSocialIntermediarioFlete, ordenDeCarga.Id.ToString());
            }

            if (gestionaDestino || gestionaDestinatario)
            {
                emailFasonService.EnviarMailAltaTempranaCuit(ordenDeCarga, ordenId, gestionaDestino, gestionaDestinatario);
            }

            return true;
        }
        public OrdenDeCargaFasonDto VerificarCuitsTerceros(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            orden.DestinoExisteScato = CuitExisteScato(orden.CUITDestino);
            orden.DestinatarioExisteScato = CuitExisteScato(orden.CUITDestinatario);
            ActualizarOrdenDeCarga(orden);

            repositorio.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }

        public bool ValidarOrdenActivaScato(long ordenId)
        {

            Log.Info($"Obteniendo estado de la orden fason {ordenId} en Scato con nro Entrega");
            var result = this.scatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroIdFason(ordenId);
            if (result == null)
                return false;
            Log.Info($"ScatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroDocumento Params => OrdenId: {ordenId}, Response => Terminado:{result.Terminado}");
            return !result.Terminado;
        }
        public bool ValidarSisaCliente(string codigoCliente, string codigoMaterial)
        {
            var controlarCargaReq = new ControlCargaRequest
            {
                Cliente = codigoCliente,
                Material = codigoMaterial,
                SoloSisa = true
            };

            var responseHandler = ordenCargaConsumer.ControlarCarga(controlarCargaReq);

            return !responseHandler.TieneRespuesta(ControlCargaResEnum.ClienteInhabilitadoEnSisa);
        }
        private void ActualizarOrdenDeCarga(OrdenDeCargaFason orden, bool enviarNotificaciones = false)
        {
            var detalleAActualizar = ObtenerDetallesActualizar(orden);

            ActualizarOrdenDeCarga(detalleAActualizar, enviarNotificaciones);
        }
        private void ActualizarOrdenDeCarga(DetallesActualizarOrdenDeCargaFason detalle, bool enviarNotificaciones = false)
        {
            var orden = detalle.orden;
            orden.TransporteExiste = detalle.existeTransporteEIntermediario;
            orden.Estado = ObtenerEstadoOrden(orden);
            if (enviarNotificaciones)
            {
                NotificacionesNecesarias(detalle);
            }
        }
        private EstadoOrdenDeCargaFason ObtenerEstadoOrden(OrdenDeCargaFason orden)
        {
            if (orden.TransporteExiste && orden.CuitsTerceroExisten)
            {
                var producto = orden.Producto ?? repositorio.Obtener<Material>(orden.Producto_Id);
                if (codigosRetiroEnPatagonia.Contains(producto.CodigoSap))
                    return EstadoOrdenDeCargaFason.PendienteCompensacion;
                else
                    return EstadoOrdenDeCargaFason.Generada;
            }
            return EstadoOrdenDeCargaFason.Pendiente;
        }
        /// <summary>
        /// Modifica los datos de la request según validaciones respecto al usuario que realiza la petición
        /// </summary>
        /// <param name="request"></param>
        /// <param name="mailUsuario"></param>
        private void ValidarRequest(OrdenDeCargaFasonRequest request, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            ValidarRequest(request, usuario);
        }
        private void ValidarRequest(OrdenDeCargaFasonRequest request, Usuario usuario)
        {
            Log.Info($"FASON - Validar Request {request.ToJson()}  usuario: {usuario.Mail}");
            var fleteMOA = usuario.TieneRol(RolEnum.FleteMOA);
            Log.Info($"FASON - Validar Request: RolFleteMOA={fleteMOA}");
            request.FleteMOA = fleteMOA && request.FleteMOA;
            if (!request.ProductoSeleccionado.ValidaSisaRuca)
            {
                request.CUITIntermediarioFlete = null;
                request.RazonSocialIntermediarioFlete = null;
                request.DomicilioDescr = null;
                request.DomicilioOrden = null;
                request.DomicilioTipo = null;
                request.PlantaCodigo = null;
                request.DestinatarioExisteScato = null;
                request.DestinoExisteScato = null;
            }
            else
            {
                request.DestinoMercaderia = null;
            }
        }
        private OrdenDeCargaFasonDto OrdenDeCargaFasonDto(OrdenDeCargaFason orden, Usuario usuario)
        {
            var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);
            return new OrdenDeCargaFasonDto(orden, esInterno);
        }

        private bool TransporteExiste(string CUITTransporte)
        {
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest " + $"Cuit {CUITTransporte ?? ""}");
            var estadoTransportista = ordenCargaConsumer.GetOrdenCargaControlEstadoTransportista(CUITTransporte);
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest Result " + estadoTransportista);

            return estadoTransportista == ControlEstadoResEnum.TransportistaOK;
        }
        private bool? CuitExisteScato(string cuit)
        {
            if (cuit is null)
                return null;

            return ValidarCuitExisteScato(cuit).Existe;
        }
        private DetallesActualizarOrdenDeCargaFason ObtenerDetallesActualizar(OrdenDeCargaFason orden,
            bool? existeTransportePreRevisado = null,
            bool? existeIntermediarioFletePreRevisado = null)
        {
            var existeTransporte = existeTransportePreRevisado != null ? (bool)existeTransportePreRevisado : TransporteExiste(orden.CUITTransporte);
            var existeIntermediarioFlete = existeIntermediarioFletePreRevisado != null ? (bool)existeIntermediarioFletePreRevisado : string.IsNullOrEmpty(orden.CUITIntermediarioFlete) || TransporteExiste(orden.CUITIntermediarioFlete);
            return new DetallesActualizarOrdenDeCargaFason
            {
                orden = orden,
                existeTransporte = existeTransporte,
                existeIntermediarioFlete = existeIntermediarioFlete,
            };
        }
        private void NotificacionesNecesarias(DetallesActualizarOrdenDeCargaFason detallesOrden)
        {
            if (!detallesOrden.existeTransporte)
            {
                emailFasonService.EnviarMailTransporteNoExiste(detallesOrden.orden);
            }
        }
    }
}
