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
using System.Collections;
using System.Data.Entity;
using System.Linq.Expressions;
using SustitucionMOAModel.Util;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaFasonService : OrdenDeCargaServiceBase, IOrdenDeCargaFasonService
    {
        private readonly IEnumerable<string> codigosRetiroEnPatagonia = new string[] { "98855", "99098" };
        private readonly IEmailFasonService emailFasonService;

        protected readonly List<EstadoOrdenDeCargaFason> estadosParaNoNotificarChasisRepetido = new List<EstadoOrdenDeCargaFason>
        {
            EstadoOrdenDeCargaFason.Vencida,
            EstadoOrdenDeCargaFason.Anulada,
            EstadoOrdenDeCargaFason.Entregada,
            EstadoOrdenDeCargaFason.SinEstado,
        };

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
            Log.Debug($"Listar(request: {request.ToJson()})");
            try
            {
                var fechaIncioDateTime = DataFormatter.StringToDateTime(request.FechaDesde, "");
                var fechaFinDateTime = DataFormatter.StringToDateTime(request.FechaHasta, "");

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == request.MailUsuario);
                var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);
                fechaFinDateTime = fechaFinDateTime.AddDays(1);


                var codigoProveedorClientesRelacionados = usuario.Proveedores.Select(c => c.CodigoProveedor);
                var tipoUsuarioId = usuario.TipoUsuario.Id;

                Expression<Func<OrdenDeCargaFason, bool>> filtro = x =>
                (esInterno || codigoProveedorClientesRelacionados.Contains(x.Cliente.CodigoProveedor))
                && x.FechaCreacion >= fechaIncioDateTime && x.FechaCreacion <= fechaFinDateTime
                && (esInterno || (tipoUsuarioId == 5 ? x.CorredorId == null : x.CorredorId != null));

                var listadoConFiltro = repositorio.ListarConsultable(filtro);

                if (listadoConFiltro.Count() == 0)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga fason"));
                }

                var hashPatentesCargadas = ObtenerHashPatentesCargadas(listadoConFiltro.AsEnumerable());

                var listado = listadoConFiltro.ToList().OrderByDescending(x => x.FechaCreacion)
                    .Select(x => new OrdenDeCargaFasonDto(x, esInterno)
                    {
                        TienePatentesRepetidas = VerificarOrdenConPatentesRepetidas(x, hashPatentesCargadas),
                        TienePatenteMultiplesAutorizaciones = VerificarChasisConMultiplesAutorizaciones(x, hashPatentesCargadas)
                    })
                    .ToList();

                var response = new ListarOrdenDeCargaFasonResponse { Response = listado };

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

                var response = new OrdenDeCargaFasonDto(orden, esInterno)
                {
                    OrdenesConPatentesRepetidas = esInterno ? ObtenerOrdenesConPatentesRepetidas(orden) : null
                };

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
            emailFasonService.EnviarMailVencieronOrdenesDeCarga(ordenes);

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
                    corr.Proveedores.Where(x => x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente && x.EsClienteDeCorredorFason)
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
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            ValidarRequest(request, usuario);
            ModificarDatosRequest(request, usuario);
            var existeTransporte = TransporteExiste(request.CUITTransporte);
            var existeIntermediarioFlete = string.IsNullOrEmpty(request.CUITIntermediarioFlete) || TransporteExiste(request.CUITIntermediarioFlete);

            var producto = repositorio.Obtener<Material>(request.Producto_Id);
            var localidad = ObtenerLocalidadDeLaOrden(request, producto);

            request.DestinatarioExisteScato = CuitExisteScato(request.CUITDestinatario);
            request.DestinoExisteScato = CuitExisteScato(request.CUITDestino);

            long ultimoId = 0;
            for (int i = 0; i < request.CantidadDeViajes; i++)
            {
                var ordenEntity = new OrdenDeCargaFason(request)
                {
                    Producto = producto,
                    LocalidadId = localidad.LocalidadId,
                    LocalidadDescripcion = localidad.LocalidadDescripcion,
                    KmARecorrer = localidad.KmARecorrer
                };
                var detalleActualizar = ObtenerDetallesActualizar(ordenEntity, existeTransporte, existeIntermediarioFlete);
                var enviaNotificacion = i == 0;
                ActualizarOrdenDeCarga(detalleActualizar, enviaNotificacion);
                if (enviaNotificacion)
                {
                    ordenEntity.Cliente = repositorio.Obtener<Proveedor>(ordenEntity.Cliente_Id);
                    NotificacionCamionAutorizadoMultiplesOrdenes(ordenEntity);
                }
                repositorio.Agregar(ordenEntity);

                repositorio.GuardarCambios();
                ultimoId = ordenEntity.Id;
            }

            var resultado = new Resultado { Mensaje = SuccessMsg.OrdenDeCargaAgregada, IdEntidad = (int)ultimoId };
            return resultado;
        }

        public Resultado Editar(EditarOrdenDeCargaFasonRequest request, string mailUsuario)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
                var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);
                ValidarRequest(request, usuario);
                var orden = repositorio.Obtener<OrdenDeCargaFason>(request.Id);

                if (ValidarOrdenActivaScato(request.Id) && !esAdmin)
                {
                    emailFasonService.EnviarMailIntentoEdicionActiva(orden, request);
                    throw new InfoCustomException("La orden no se puede editar por estar el camión en planta");
                }
                var listaValoresDiferentes = ObtenerListaValoresDiferentes(orden, request);

                orden.Cantidad = request.Cantidad;
                orden.Cliente_Id = request.Cliente;
                orden.CorredorId = request.CorredorId;
                orden.CUILChofer = request.CUILChofer;
                orden.CUITTransporte = request.CUITTransporte;
                orden.NombreChofer = request.NombreChofer;
                orden.ApellidoChofer = request.ApellidoChofer;
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
                NotificacionCamionAutorizadoMultiplesOrdenes(orden);
                if (!esAdmin)
                {
                    emailFasonService.EnviarMailNotificacionEdicion(orden, listaValoresDiferentes);
                }
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
            var proveedor = repositorio.Obtener<Proveedor>(a => a.Id == clienteId);
            return ObtenerDestinos(proveedor.CUIT);
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

        public OrdenDeCargaFasonDto AnularOrden(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);

            if (orden.Estado == EstadoOrdenDeCargaFason.Entregada)
            {
                throw new InfoCustomException("Esta orden no puede ser anulada, ya fue entregada.");
            }
            if (ValidarOrdenActivaScato(ordenId) && !esAdmin)
            {
                emailFasonService.EnviarMailIntentoAnulacionActiva(orden);
                throw new InfoCustomException("La orden no se puede anular por estar el camión en planta");
            }
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
            var result = this.scatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroIdFason(ordenId);
            if (result == null)
                return false;
            Log.Debug($"ScatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroDocumento Params => OrdenId: {ordenId}, Response => Terminado:{result.Terminado}");
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
        public bool ValidarExistenciaPatente(string patenteChasis, string cuitCliente)
        {
            return OrdenesConPatentesRepetidas(patenteChasis)
                .Any(oc => oc.Cliente.CUIT != cuitCliente);
        }

        private List<OrdenDeCargaFason> OrdenesConPatentesRepetidas(string patenteChasis)
        {
            return repositorio.Listar<OrdenDeCargaFason>(oc =>
                !estadosParaNoNotificarChasisRepetido.Contains(oc.Estado) &&
                oc.PatenteChasis == patenteChasis
            );
        }
        private bool VerificarOrdenConPatentesRepetidas(OrdenDeCargaFason orden, Hashtable hashPatentesCargadas)
        {

            if (estadosParaNoNotificarChasisRepetido.Contains(orden.Estado))
            {
                return false;
            }
            if (hashPatentesCargadas.ContainsKey(orden.PatenteChasis))
            {
                var ordenesCargadas = (List<(long, string)>)hashPatentesCargadas[orden.PatenteChasis];
                return ordenesCargadas.Count > 1;
            }
            return false;
        }

        private bool VerificarChasisConMultiplesAutorizaciones(OrdenDeCargaFason orden, Hashtable hashPatentesCargadas)
        {
            if (estadosParaNoNotificarChasisRepetido.Contains(orden.Estado))
            {
                return false;
            }

            if (hashPatentesCargadas.ContainsKey(orden.PatenteChasis))
            {
                var chasisAutorizados = (List<(long, string)>)hashPatentesCargadas[orden.PatenteChasis];
                return chasisAutorizados.Any(data => data.Item2 != orden.Cliente.CUIT);
            }
            return false;
        }
        private List<long> ObtenerOrdenesConPatentesRepetidas(OrdenDeCargaFason orden)
        {
            var hashPatentesCargadas = ObtenerHashPatentesCargadas();
            if (!VerificarOrdenConPatentesRepetidas(orden, hashPatentesCargadas))
            {
                return null;
            }
            var ordenesConPatentesRepetidas = (List<(long, string)>)hashPatentesCargadas[orden.PatenteChasis];
            return ordenesConPatentesRepetidas.Where(data => data.Item1 != orden.Id)
                .Select(data => data.Item1)
                .ToList();
        }
        private Hashtable ObtenerHashPatentesCargadas()
        {
            var diasPreviosParaCompararPatentes = -5;
            var hoy = DateTime.Now;
            var fechaTope = hoy.AddDays(diasPreviosParaCompararPatentes);

            return ObtenerHashPatentesCargadas(
                repositorio.ListarConsultable<OrdenDeCargaFason>(oc =>
                    oc.FechaCreacion <= hoy &&
                    DbFunctions.TruncateTime(oc.FechaCreacion) >= fechaTope)
                .AsEnumerable());
        }
        private Hashtable ObtenerHashPatentesCargadas(IEnumerable<OrdenDeCargaFason> ordenes)
        {

            var hashPatentesCargadas = new Hashtable();
            ordenes
                .Where(oc =>
                    !estadosParaNoNotificarChasisRepetido.Contains(oc.Estado)
                )
                .ToList()
                .ForEach(oc =>
                {
                    if (hashPatentesCargadas.ContainsKey(oc.PatenteChasis))
                    {
                        var ordenesCargadas = (List<(long, string)>)hashPatentesCargadas[oc.PatenteChasis];
                        ordenesCargadas.Add((oc.Id, oc.Cliente.CUIT));
                        hashPatentesCargadas[oc.PatenteChasis] = ordenesCargadas;
                    }
                    else
                    {
                        hashPatentesCargadas.Add(oc.PatenteChasis, new List<(long, string)> { (oc.Id, oc.Cliente.CUIT) });
                    }

                });
            return hashPatentesCargadas;
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

        private void ValidarRequest(OrdenDeCargaFasonRequest request, Usuario usuario)
        {
            if (request.CantidadDeViajes > 3)
            {
                throw new ValidationCustomException("No puede generar más de 3(tres) viajes.");
            }

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

        private ScatoWS.KmPorProveedorDto ObtenerLocalidadDeLaOrden(CrearOrdenDeCargaFasonRequest request, Material producto)
        {
            var localidades = producto.EsDerivadoGranario ?
                ObtenerDestinos(request.CUITDestino) :
                ObtenerDestinos(request.Cliente);

            if (localidades.Count == 0)
            {
                throw new ValidationCustomException("El cliente/destino no cuenta con ninguna localidad, imposible continuar con la carga.");
            }
            return localidades.First();
        }

        private void ModificarDatosRequest(OrdenDeCargaFasonRequest request, Usuario usuario)
        {
            var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);
            if (esAdmin)
            {
                return;
            }
            if (usuario.EsCorredor())
            {
                var corredor = usuario.Proveedores.FirstOrDefault(p => p.CodigoProveedor == request.CodigoCorredor);
                if (corredor is null)
                {
                    throw new ValidationCustomException("El corredor no existe.");
                }
                request.CorredorId = corredor.Id;
            }
        }

        private void NotificacionCamionAutorizadoMultiplesOrdenes(OrdenDeCargaFason ordenDeCarga)
        {

            if (!estadosParaNoNotificarChasisRepetido.Contains(ordenDeCarga.Estado))
            {
                var ordenesConPatentesRepetidas = OrdenesConPatentesRepetidas(ordenDeCarga.PatenteChasis);
                if (ordenesConPatentesRepetidas.Any(oc => oc.Cliente.CUIT != ordenDeCarga.Cliente.CUIT))
                {
                    var cuits =  ordenesConPatentesRepetidas
                        .Select(oc => oc.Cliente.CUIT).Distinct().ToList();
                    if (!cuits.Contains(ordenDeCarga.Cliente.CUIT))
                    {
                        cuits.Add(ordenDeCarga.Cliente.CUIT);
                    }
                    emailFasonService.EnviarMailCamionAutorizadoEnVariasOrdenes(ordenDeCarga.PatenteChasis, cuits);
                }
            }
        }
        private List<Variance> ObtenerListaValoresDiferentes(OrdenDeCargaFason orden, EditarOrdenDeCargaFasonRequest request)
        {
            var ordenEditada = new OrdenDeCargaFason {
                Cantidad = request.Cantidad,
                Cliente_Id = request.Cliente,
                CorredorId = request.CorredorId,
                CUILChofer = request.CUILChofer,
                CUITTransporte = request.CUITTransporte,
                NombreChofer = request.NombreChofer,
                ApellidoChofer = request.ApellidoChofer,
                Observacion = request.Observacion,
                PatenteAcoplado = request.PatenteAcoplado,
                PatenteChasis = request.PatenteChasis,
                Producto_Id = request.Producto_Id,
                RazonSocialTransporte = request.RazonSocialTransporte,
                FleteMOA = request.FleteMOA,
                CUITIntermediarioFlete = request.CUITIntermediarioFlete,
                RazonSocialIntermediarioFlete = request.RazonSocialIntermediarioFlete,
                ClienteComoRemitenteComercial = !string.IsNullOrEmpty(request.CUITDestino) &&
                    request.CUITDestino != request.CUITCliente.ToString(),
                PlantaCodigo = request.PlantaCodigo,
                DomicilioTipo = request.DomicilioTipo,
                DomicilioOrden = request.DomicilioOrden,
                DomicilioDescr = request.DomicilioDescr,
                RazonSocialDestino = request.RazonSocialDestino,
                Escalable = request.Escalable,
                //Campos Con valores que no cambian
                Producto = orden.Producto,
                Cliente = orden.Cliente,
                Corredor = orden.Corredor,
                Estado = orden.Estado,
                Id = orden.Id,
                LocalidadId = orden.LocalidadId,
                DestinoMercaderia = orden.DestinoMercaderia,
                FechaCreacion = orden.FechaCreacion,
                LocalidadDescripcion = orden.LocalidadDescripcion,
                KmARecorrer = orden.KmARecorrer
            };
            return orden.Compare(ordenEditada);
        }
    }
}
