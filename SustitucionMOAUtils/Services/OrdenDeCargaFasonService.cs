using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ScatoWS = SustitucionMOAWS.ScatoWebService;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaFasonService : OrdenDeCargaServiceBase, IOrdenDeCargaFasonService
    {
        private readonly IEnumerable<string> codigosRetiroEnPatagonia = new string[] { "98855", "99098" };
        private readonly string _codigoAceiteSojaNeutralizado = "98855";
        private readonly string _codigoAceiteMetiladoSoja = "99098";

        private readonly IEmailFasonService emailFasonService;
        private readonly IRepositorioOrdenDeCargaFason repositorioFason;

        protected readonly List<EstadoOrdenDeCargaFason> estadosParaNoNotificarChasisRepetido = new List<EstadoOrdenDeCargaFason>
        {
            EstadoOrdenDeCargaFason.Vencida,
            EstadoOrdenDeCargaFason.Anulada,
            EstadoOrdenDeCargaFason.Entregada,
            EstadoOrdenDeCargaFason.SinEstado,
        };

        public OrdenDeCargaFasonService(IRepositorioOrdenDeCargaFason repositorioFason,
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IScatoConsumer scatoConsumer,
            IScatoRepositorioClient scatoRepositorioClient,
            ICNRTClient cNRTClient,
            IEmailFasonService emailFasonService,
            IFeriadoService feriadoService,
            IUbicacionGeograficaService ubicacionGeograficaService
            ) : base(ordenCargaConsumer, scatoConsumer, scatoRepositorioClient, repositorioFason, cNRTClient, feriadoService, ubicacionGeograficaService)
        {
            this.emailFasonService = emailFasonService;
            this.repositorioFason = repositorioFason;
        }

        public ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request)
        {
            var fechaIncioDateTime = DataFormatter.StringToDateTime(request.FechaDesde, "");
            var fechaFinDateTime = DataFormatter.StringToDateTime(request.FechaHasta, "");

            var usuario = repositorioFason.Obtener<Usuario>(u => u.Mail == request.MailUsuario);
            var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);
            fechaFinDateTime = fechaFinDateTime.AddDays(1);

            var codigoProveedorClientesRelacionados = usuario.Proveedores.Select(c => c.CodigoProveedor);

            Expression<Func<OrdenDeCargaFason, bool>> filtro = x =>
                (esInterno || codigoProveedorClientesRelacionados.Contains(x.Cliente.CodigoProveedor))
                && x.FechaCreacion >= fechaIncioDateTime && x.FechaCreacion <= fechaFinDateTime
                && (esInterno || (request.EsCorredor ? x.CorredorId != null : x.CorredorId == null));

            var listadoConFiltro = repositorioFason.ListarConsultable(filtro);

            if (listadoConFiltro.Count() == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga fason"));
            }

            var hashPatentesCargadas = ObtenerHashPatentesCargadas(listadoConFiltro.AsEnumerable());

            var listado = listadoConFiltro.AsEnumerable().OrderByDescending(x => x.FechaCreacion)
                .Select(x => new OrdenDeCargaFasonDto(x, esInterno)
                {
                    TienePatentesRepetidas = VerificarOrdenConPatentesRepetidas(x, hashPatentesCargadas),
                    TienePatenteMultiplesAutorizaciones = VerificarChasisConMultiplesAutorizaciones(x, hashPatentesCargadas)
                })
                .ToList();

            var response = new ListarOrdenDeCargaFasonResponse { Response = listado };

            return response;
        }

        public DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario)
        {
            try
            {
                var usuario = repositorioFason.Obtener<Usuario>(u => u.Mail == mailUsuario.MailUsuario);
                var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);

                var orden = repositorioFason.Obtener<OrdenDeCargaFason>(IdOrdenCargaFason);

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
            var orden = repositorioFason.Obtener<OrdenDeCargaFason>(ordenId);
            var usuario = repositorioFason.Obtener<Usuario>(us => us.Mail == mailUsuario);
            ActualizarOrdenDeCarga(orden);

            repositorioFason.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }

        public List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason()
        {
            var fechaLimite = DateTime.Now.Date;
            var ordenesVencidas = new List<OrdenDeCargaFason>();

            if (repositorioFason.Obtener<HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesDeCargaFasonJob" && a.Habilitado) == null)
                return ordenesVencidas;

            var feriados = feriadoService.ObtenerFeriados();

            var ordenes = repositorioFason.Listar<OrdenDeCargaFason>((orden) => orden.Estado == EstadoOrdenDeCargaFason.Generada || orden.Estado == EstadoOrdenDeCargaFason.Pendiente);
            foreach (var orden in ordenes)
            {
                if (CalcularFechaVencimiento(orden.FechaCreacion, feriados) < fechaLimite)
                {
                    var camionEstaEnPlanta = ValidarOrdenActivaScato(orden.Id);
                    if (!camionEstaEnPlanta)
                    {
                        orden.Estado = EstadoOrdenDeCargaFason.Vencida;
                        ordenesVencidas.Add(orden);
                    }
                }
            }
            if (ordenesVencidas.Any())
            {
                repositorioFason.GuardarCambios();
            }
            emailFasonService.EnviarMailVencieronOrdenesDeCarga(ordenesVencidas);

            return ordenes;
        }

        public List<ProveedorDto> GetCorredores()
        {
            var corredoresBD = repositorioFason
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
                var corredor = repositorioFason.Obtener<Proveedor>(p => p.CodigoProveedor == codigoCorredor);

                var corredores = repositorioFason
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
                var corredores = repositorioFason
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
            var usuario = repositorioFason.Obtener<Usuario>(us => us.Mail == mailUsuario);
            ValidarRequest(request, usuario);
            SetearCorredor(request, usuario);

            var cliente = repositorioFason.Obtener<Proveedor>(request.Cliente);
            var producto = repositorioFason.Obtener<Material>(request.Producto_Id);
            var localidad = ObtenerLocalidadDeLaOrden(request, producto);

            request.DestinatarioExisteScato = CuitExisteScato(request.CUITDestinatario);
            request.DestinoExisteScato = CuitExisteScato(request.CUITDestino);

            var distanciaARecorrer = ObtenerDistanciaARecorrer(request.DomicilioDescr);

            var ordenEntity = new OrdenDeCargaFason { Id = 0 };
            var ordenesAgregadas = new List<OrdenDeCargaFason>();

            foreach (var unidadTransporte in request.UnidadesTransporte)
            {
                var existeTransporte = TransporteExiste(unidadTransporte.CUITTransporte);
                var existeIntermediarioFlete = string.IsNullOrEmpty(unidadTransporte.CUITIntermediarioFlete) || TransporteExiste(unidadTransporte.CUITIntermediarioFlete);

                for (int i = 0; i < unidadTransporte.CantidadDeViajes; i++)
                {
                    ordenEntity = new OrdenDeCargaFason(request, unidadTransporte)
                    {
                        Producto = producto,
                        LocalidadId = localidad?.LocalidadId,
                        LocalidadDescripcion = localidad?.LocalidadDescripcion,
                        KmARecorrer = distanciaARecorrer.HasValue ? distanciaARecorrer.ToString() : localidad?.KmARecorrer
                    };

                    var detalleActualizar = ObtenerDetallesActualizar(ordenEntity, existeTransporte, existeIntermediarioFlete);
                    var enviaNotificacion = i == 0;
                    ActualizarOrdenDeCarga(detalleActualizar, enviaNotificacion);
                    if (enviaNotificacion)
                    {
                        ordenEntity.Cliente = cliente;
                        NotificacionCamionAutorizadoMultiplesOrdenes(ordenEntity);
                    }
                    repositorioFason.Agregar(ordenEntity);
                    ordenesAgregadas.Add(ordenEntity);
                }
            }
            repositorioFason.GuardarCambios();
            var ultimoId = ordenEntity.Id;
            NotificarAutorizacionDeNomina(ordenesAgregadas, false);

            var resultado = new Resultado { Mensaje = SuccessMsg.OrdenDeCargaAgregada, IdEntidad = (int)ultimoId };
            return resultado;
        }

        public Resultado Editar(EditarOrdenDeCargaFasonRequest request, string mailUsuario)
        {
            try
            {
                var usuario = repositorioFason.Obtener<Usuario>(us => us.Mail == mailUsuario);
                var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);
                ValidarRequest(request, usuario);
                var orden = repositorioFason.Obtener<OrdenDeCargaFason>(request.Id);

                if (ValidarOrdenActivaScato(request.Id) && !esAdmin)
                {
                    emailFasonService.EnviarMailIntentoEdicionActiva(orden, request);
                    throw new InfoCustomException("La orden no se puede editar por estar el camión en planta");
                }
                var listaValoresDiferentes = ObtenerListaValoresDiferentes(orden, request);

                orden.Cantidad = request.Cantidad;
                orden.Cliente_Id = request.Cliente;
                orden.CorredorId = request.CorredorId;
                orden.CUILChofer = request.UnidadTransporte.CUILChofer;
                orden.CUITTransporte = request.UnidadTransporte.CUITTransporte;
                orden.NombreChofer = request.UnidadTransporte.NombreChofer;
                orden.ApellidoChofer = request.UnidadTransporte.ApellidoChofer;
                orden.Observacion = request.Observacion;
                orden.PatenteAcoplado = request.UnidadTransporte.PatenteAcoplado;
                orden.PatenteChasis = request.UnidadTransporte.PatenteChasis;
                orden.Producto_Id = request.Producto_Id;
                orden.RazonSocialTransporte = request.UnidadTransporte.RazonSocialTransporte;
                orden.FleteMOA = request.FleteMOA;
                orden.CUITIntermediarioFlete = request.UnidadTransporte.CUITIntermediarioFlete;
                orden.RazonSocialIntermediarioFlete = request.UnidadTransporte.RazonSocialIntermediarioFlete;
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
                repositorioFason.GuardarCambios();
                NotificarAutorizacionDeNomina(new List<OrdenDeCargaFason> { orden }, true);
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
            var proveedor = repositorioFason.Obtener<Proveedor>(a => a.Id == clienteId);
            return ObtenerDestinos(proveedor.CUIT);
        }

        public void VerificarTransporteJob()
        {
            if (repositorioFason.Obtener<HabilitacionJob>(hj => hj.Nombre == "VerificarTransporteOrdenesDeCargaFasonJob" && hj.Habilitado) == null)
                return;

            var ordenes = repositorioFason.Listar<OrdenDeCargaFason>(orden => !orden.TransporteExiste && orden.Estado == EstadoOrdenDeCargaFason.Pendiente);
            foreach (var orden in ordenes)
            {
                ActualizarOrdenDeCarga(orden);
            }
            repositorioFason.GuardarCambios();
        }

        public OrdenDeCargaFasonDto AnularOrden(int ordenId, string mailUsuario)
        {
            var orden = repositorioFason.Obtener<OrdenDeCargaFason>(ordenId) ?? throw new InfoCustomException("Orden no encontrada");
            var usuario = repositorioFason.Obtener<Usuario>(us => us.Mail == mailUsuario);
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
            repositorioFason.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }

        public List<AutoCompleteDropdownElement> ObtenerCuilsChofer(int clienteId, string patenteAcoplado, string mailUsuario)
        {
            if (clienteId == 0 || string.IsNullOrEmpty(patenteAcoplado))
            {
                return new List<AutoCompleteDropdownElement> { new AutoCompleteDropdownElement { label = "", value = "" } };
            }

            //var cliente = repositorioFason.Obtener<Proveedor>(x =>
            //    x.Id == clienteId &&
            //    x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
            //    x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);

            //if (cliente == null)
            //{
            //    return new List<AutoCompleteDropdownElement>();
            //}

            var cuilsElements = repositorioFason
                .ObtenerCuilsChofer(clienteId, patenteAcoplado)
                .Select(x =>
                    new AutoCompleteDropdownElement
                    {
                        label = x.Length >= 11 ? x.Substring(0, 2) + "-" + x.Substring(2, 8) + "-" + x.Substring(10, 1) : "",
                        value = x
                    })
                .ToList();

            return cuilsElements;
        }

        public List<AutoCompleteDropdownElement> ObtenerCuitsTransporte(int clienteId, string patenteAcoplado, string mailUsuario)
        {
            //List<AutoCompleteDropdownElement> cuits = new List<AutoCompleteDropdownElement>();
            //Proveedor cliente;

            if (clienteId == 0 || string.IsNullOrEmpty(patenteAcoplado))
            {
                return new List<AutoCompleteDropdownElement> { new AutoCompleteDropdownElement { label = "", value = "" } };
            }

            //cliente = repositorioFason.Obtener<Proveedor>(
            //    x => x.Id == orden.Cliente &&
            //    x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            //if (cliente == null)
            //{
            //    return cuits;
            //}

            var cuits = repositorioFason //.Listar<OrdenDeCargaFason, AutoCompleteDropdownElement>(
                .ObtenerCuitsTransporte(clienteId, patenteAcoplado)
                .Select(x =>
                    new AutoCompleteDropdownElement
                    {
                        label = x.Length >= 11 ? x.Substring(0, 2) + "-" + x.Substring(2, 8) + "-" + x.Substring(10, 1) : "",
                        value = x
                    })
                .ToList();

            return cuits;
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
            cliente = repositorioFason.Obtener<Proveedor>(
            x => x.Id == orden.Cliente &&
            x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return new OrdenDeCargaDto();
            }
            result.ordenes = repositorioFason.Listar<OrdenDeCargaFason, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.PatenteChasis,
                value = x.PatenteAcoplado
            }, x => x.Cliente_Id == cliente.Id);
            result.ordenes = result.ordenes.Distinct().ToList();
            return result;
        }

        public bool EnviarMailAltaCuitTerceros(bool gestionaFlete, bool gestionaDestino, bool gestionaDestinatario, string ordenId)
        {
            var ordenDeCarga = this.repositorioFason.Obtener<OrdenDeCargaFason>(o => o.Id.ToString() == ordenId);

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
            var orden = repositorioFason.Obtener<OrdenDeCargaFason>(ordenId);
            var usuario = repositorioFason.Obtener<Usuario>(us => us.Mail == mailUsuario);
            orden.DestinoExisteScato = CuitExisteScato(orden.CUITDestino);
            orden.DestinatarioExisteScato = CuitExisteScato(orden.CUITDestinatario);
            ActualizarOrdenDeCarga(orden);

            repositorioFason.GuardarCambios();
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
            return repositorioFason.Listar<OrdenDeCargaFason>(oc =>
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
                repositorioFason.ListarConsultable<OrdenDeCargaFason>(oc =>
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
            if (!orden.TransporteExiste ||
                !orden.CuitsTerceroExisten)
            {
                Log.Debug($"Orden fason id={orden.Id} queda pendiente. TransporteExiste={orden.TransporteExiste}, CuitsTerceroExisten={orden.CuitsTerceroExisten}, LocalidadId={orden.LocalidadId}.");
                return EstadoOrdenDeCargaFason.Pendiente;
            }

            var producto = orden.Producto ?? repositorioFason.Obtener<Material>(orden.Producto_Id);
            if (codigosRetiroEnPatagonia.Contains(producto.CodigoSap))
            {
                return EstadoOrdenDeCargaFason.PendienteCompensacion;
            }
            else
            {
                return EstadoOrdenDeCargaFason.Generada;
            }
        }

        private void ValidarRequest(CrearOrdenDeCargaFasonRequest request, Usuario usuario)
        {
            if (request.UnidadesTransporte == null || request.UnidadesTransporte.Count == 0)
            {
                throw new ValidationCustomException("No se ingresaron unidades de transporte");
            }
            if (request.UnidadesTransporte.Any(ut => ut.CantidadDeViajes > 3))
            {
                throw new ValidationCustomException("No puede generar más de 3 (tres) viajes.");
            }
            if (!request.ProductoSeleccionado.ValidaSisaRuca)
            {
                request.UnidadesTransporte.ForEach(ut =>
                {
                    ut.CUITIntermediarioFlete = null;
                    ut.RazonSocialIntermediarioFlete = null;
                });
            }
            ValidarRequest((OrdenDeCargaFasonRequest)request, usuario);
        }

        private void ValidarRequest(EditarOrdenDeCargaFasonRequest request, Usuario usuario)
        {
            if (request.UnidadTransporte == null)
            {
                throw new ValidationCustomException("No se ingresó unidad de transporte");
            }
            if (request.UnidadTransporte.CantidadDeViajes > 3)
            {
                throw new ValidationCustomException("No puede generar más de 3 (tres) viajes.");
            }
            if (!request.ProductoSeleccionado.ValidaSisaRuca)
            {
                request.UnidadTransporte.CUITIntermediarioFlete = null;
                request.UnidadTransporte.RazonSocialIntermediarioFlete = null;
            }
            ValidarRequest((OrdenDeCargaFasonRequest)request, usuario);
        }

        private void ValidarRequest(OrdenDeCargaFasonRequest request, Usuario usuario)
        {
            var fleteMOA = usuario.TieneRol(RolEnum.FleteMOA);

            request.FleteMOA = fleteMOA && request.FleteMOA;
            if (!request.ProductoSeleccionado.ValidaSisaRuca)
            {
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

        private void SetearCorredor(OrdenDeCargaFasonRequest request, Usuario usuario)
        {
            var esAdmin = usuario.TieneRol(RolEnum.FasonAdmin);
            if (esAdmin)
            {
                return;
            }
            if (usuario.EsCorredor())
            {
                var corredor = usuario.Proveedores.FirstOrDefault(p => p.CodigoProveedor == request.CodigoCorredor) ?? throw new ValidationCustomException("El corredor no existe.");
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
                    var cuits = ordenesConPatentesRepetidas
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
            var ordenEditada = new OrdenDeCargaFason
            {
                Cantidad = request.Cantidad,
                Cliente_Id = request.Cliente,
                CorredorId = request.CorredorId,
                CUILChofer = request.UnidadTransporte.CUILChofer,
                CUITTransporte = request.UnidadTransporte.CUITTransporte,
                NombreChofer = request.UnidadTransporte.NombreChofer,
                ApellidoChofer = request.UnidadTransporte.ApellidoChofer,
                Observacion = request.Observacion,
                PatenteAcoplado = request.UnidadTransporte.PatenteAcoplado,
                PatenteChasis = request.UnidadTransporte.PatenteChasis,
                Producto_Id = request.Producto_Id,
                RazonSocialTransporte = request.UnidadTransporte.RazonSocialTransporte,
                FleteMOA = request.FleteMOA,
                CUITIntermediarioFlete = request.UnidadTransporte.CUITIntermediarioFlete,
                RazonSocialIntermediarioFlete = request.UnidadTransporte.RazonSocialIntermediarioFlete,
                ClienteComoRemitenteComercial = !string.IsNullOrEmpty(request.CUITDestino) &&
                    request.CUITDestino != request.CUITCliente.ToString(),
                PlantaCodigo = request.PlantaCodigo,
                DomicilioTipo = request.DomicilioTipo,
                DomicilioOrden = request.DomicilioOrden,
                DomicilioDescr = request.DomicilioDescr,
                RazonSocialDestino = request.RazonSocialDestino,
                Escalable = request.Escalable,
                TransporteExiste = orden.TransporteExiste,
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

        private ScatoWS.KmPorProveedorDto ObtenerLocalidadDeLaOrden(CrearOrdenDeCargaFasonRequest request, Material producto)
        {
            if (producto.EsDerivadoGranario)
            {
                return null;
            }
            var localidades = ObtenerDestinos(request.Cliente);
            return localidades.Count > 0 ? localidades.First() : null;
        }

        private void NotificarAutorizacionDeNomina(List<OrdenDeCargaFason> ordenesAgregadas, bool esEdicionDeOrden)
        {
            try
            {
                if (!(ordenesAgregadas?.Any() ?? false))
                {
                    return;
                }

                if (DebeNotificarAutorizacionDeNomina(ordenesAgregadas[0]))
                {
                    var ordenesANotificar = ordenesAgregadas.Where(o => o.Estado == EstadoOrdenDeCargaFason.Generada);
                    emailFasonService.EnviarMailAutorizacionDeNomina(ordenesANotificar, esEdicionDeOrden);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al notificar autorización de nómina", ex);
                throw;
            }
        }

        private bool DebeNotificarAutorizacionDeNomina(OrdenDeCargaFason orden)
        {
            return orden.Estado == EstadoOrdenDeCargaFason.Generada &&
                (orden.Producto.CodigoSap == _codigoAceiteSojaNeutralizado || orden.Producto.CodigoSap == _codigoAceiteMetiladoSoja);
        }
    }
}
