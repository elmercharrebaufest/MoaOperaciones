using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Validadores.OrdenDeCarga;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Mod = SustitucionMOAModel.Models;
using ScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaService : OrdenDeCargaServiceBase, IOrdenDeCargaService
    {
        private readonly string _usuarioAutomaticoSAP = ConfigurationManager.AppSettings["UsuarioAutomaticoSAP"];
        private readonly string _transporteNoExiste = "El transporte no existe";

        protected readonly IEmailFasService emailFasService;
        protected readonly IFacturaAnticipadaService facturaAnticipadaService;
        protected readonly IKgDisponiblesFasService kgDisponiblesFasService;

        protected readonly List<EstadoOrdenDeCarga> estadosParaNoNotificarChasisRepetido = new List<EstadoOrdenDeCarga>
        {
            EstadoOrdenDeCarga.Vencida,
            EstadoOrdenDeCarga.Anulada,
            EstadoOrdenDeCarga.Entregada,
            EstadoOrdenDeCarga.AnuladaPorVencimiento,
            EstadoOrdenDeCarga.ErrorDeCarga
        };

        public IRepositorioOrdenDeCarga RepositorioOrdenDeCarga { get { return (IRepositorioOrdenDeCarga)repositorio; } }

        public OrdenDeCargaService(
            IRepositorioOrdenDeCarga repositorioOrdenDeCarga,
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IFeriadoService feriadoService,
            IScatoRepositorioClient scatoRepositorioClient,
            IScatoConsumer scatoConsumer,
            IEmailFasService emailFasService,
            IFacturaAnticipadaService facturaAnticipadaService,
            IKgDisponiblesFasService kgDisponiblesFasService,
            ICNRTClient cNRTClient,
            IUbicacionGeograficaService ubicacionGeograficaService
            ) : base(ordenCargaConsumer, scatoConsumer, scatoRepositorioClient, repositorioOrdenDeCarga, cNRTClient, feriadoService, ubicacionGeograficaService)
        {
            this.emailFasService = emailFasService;
            this.facturaAnticipadaService = facturaAnticipadaService;
            this.kgDisponiblesFasService = kgDisponiblesFasService;
        }

        public Resultado Agregar(CrearOrdenDeCargaRequest crearOrdenDeCargaRequest, string mailUsuario)
        {
            var ordenReq = crearOrdenDeCargaRequest.OrdenDeCarga;
            var gestionAltas = crearOrdenDeCargaRequest.GestionAltasFAS;

            Log.Info($"Agregar orden de carga con datos: {ordenReq.ToDto().ToJson()}. MailUsuario: {mailUsuario}");

            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

                LlenarOrdenAltaCorredorCliente(ordenReq, usuario);

                LlenarOrdenAlta(ordenReq, usuario);

                var kilosDisponibles = ObtenerKilosDisponiblesContrato(ordenReq, out Result contratoSAP);
                var kilosPorOrden = ordenReq.Cantidad;

                LlenarOrdenDeCargaFleteMOA(ordenReq, contratoSAP);

                OrdenDeCarga nuevaOrden = null;
                foreach (var unidadTransporte in crearOrdenDeCargaRequest.UnidadesTransporte)
                {
                    var ordenPuedeEnviarseDirectoSap = KilosAlcanzanParaConfirmarOrden(kilosDisponibles);

                    nuevaOrden = new OrdenDeCarga(ordenReq, unidadTransporte);

                    ValidarOrdenDeCargaAlta(nuevaOrden);

                    nuevaOrden.TransporteExiste = TransporteExiste(nuevaOrden);

                    var crearPedido = VerificarOrden(nuevaOrden, nuevaOrden.Cliente, false);

                    repositorio.Agregar(nuevaOrden);

                    var pedidoTieneKgDisponiblesEnFacturaAnticipada = true;

                    if (crearPedido && ordenPuedeEnviarseDirectoSap)
                    {
                        nuevaOrden.ContratoSAP = nuevaOrden.ContratoIngresado;
                        if (nuevaOrden.EsFacturaAnticipada)
                        {
                            pedidoTieneKgDisponiblesEnFacturaAnticipada = PedidoTieneKgDisponibles(nuevaOrden);
                            if (pedidoTieneKgDisponiblesEnFacturaAnticipada)
                            {
                                nuevaOrden.NumeroFacturaSeleccionada = nuevaOrden.NumeroFactura;
                                nuevaOrden.NumeroPedido = nuevaOrden.NumeroPedidoIngresado;
                            }
                            if (!nuevaOrden.SinSeleccionarFactura)
                                GenerarEntregaSAP(nuevaOrden);
                            else
                            {
                                nuevaOrden.DescripcionErrorInterno = "Orden con pedido entre 0 a 15Tn.";
                                nuevaOrden.Estado = EstadoOrdenDeCarga.Pendiente;
                            }
                        }
                        else
                        {
                            var creadaEnSAP = CrearPedidoEnSAP(nuevaOrden, nuevaOrden.Cliente, true, mailUsuario);
                            if (creadaEnSAP)
                            {
                                VerificarSituacionCrediticia(nuevaOrden, true);
                            }
                        }
                    }

                    if (nuevaOrden.Estado == EstadoOrdenDeCarga.SinEnviarASAP &&
                        (!ordenPuedeEnviarseDirectoSap || (nuevaOrden.EsFacturaAnticipada && !pedidoTieneKgDisponiblesEnFacturaAnticipada)))
                    {
                        nuevaOrden.Estado = EstadoOrdenDeCarga.Pendiente;
                        nuevaOrden.DescripcionErrorInterno = "Orden con contrato/pedido entre 0 a 15Tn.";
                    }
                    repositorio.GuardarCambios();

                    if (nuevaOrden.Estado == EstadoOrdenDeCarga.ContratoVencido)
                    {
                        emailFasService.EnviarMailContratoVencido(nuevaOrden);
                    }

                    if (!ordenPuedeEnviarseDirectoSap)
                    {
                        emailFasService.EnviarMailVariosContratos(nuevaOrden);
                    }

                    NotificarContratoSinKm(nuevaOrden);
                    NotificarTransporte(nuevaOrden.Id);
                    NotificarVariasFacturas(nuevaOrden);
                    NotificarCamionAutorizadoMultiplesOrdenes(nuevaOrden);

                    gestionAltas.OrdenId = nuevaOrden.Id;
                    EnviarMailAltaCuitTerceros(gestionAltas);

                    kilosDisponibles -= kilosPorOrden;
                }
                var resultado = new Resultado { IdEntidad = nuevaOrden.Id, Mensaje = SuccessMsg.OrdenDeCargaAgregada };
                return resultado;
            }
            catch (ValidationCustomException vcex)
            {
                throw vcex;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Resultado { error = ex.Message };
            }
        }

        public string NotificarTransporte(int ordenDeCargaId)
        {
            string mensaje = "";

            try
            {
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);

                if (!TransporteExiste(orden))
                {
                    emailFasService.EnviarMailTransporteNoExiste(orden);
                    mensaje = "Notificación enviada";
                }
                else
                {
                    mensaje = "El transporte ya fue creado";
                    orden.TransporteExiste = true;
                    repositorio.GuardarCambios();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return mensaje;
        }

        public Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario, GestionAltasFAS gestionAltas)
        {
            Log.Info($"Editar(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, mailUsuario: {mailUsuario})");
            var valoresAEditar = new List<string> { "NombreChofer", "CUITChofer", "PatenteAcoplado", "ChasisAcoplado", "ContratoIngresado", "NumeroPedido", "Observacion", "Cantidad", "RazonSocialTransporte", "CUITTransporte", "Producto_Id", "NumeroPedidoIngresado" };
            var historialCambios = new List<OrdenDeCargaCambiosHistorial>() { };
            var (cuilChoferValido, choferEnScato) = ValidarCuilChofer(ordenDeCarga.CUITChofer);
            if (!cuilChoferValido)
                throw new ValidationCustomException("Cuil de chofer inválido");

            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodasOrdenesDeCarga);
                var estadoPrevio = ordenDeCarga.Estado;
                var esInterno = EsUsuarioInterno(usuario);

                var cargarDatosOCEditar = CargarDatosOCEditar(ordenDeCarga, usuario);
                var ordenEditar = cargarDatosOCEditar.Item1;
                var listaValoresDiferentes = cargarDatosOCEditar.Item2;
                var contratoKgDisponibles = cargarDatosOCEditar.Item3;

                foreach (var prop in listaValoresDiferentes)
                {
                    if (!valoresAEditar.Contains(prop.PropertyName))
                        continue;

                    var anterior = !string.IsNullOrEmpty(prop.ValorAnterior?.ToString()) ? prop.ValorAnterior?.ToString() : "-";
                    var nuevo = !string.IsNullOrEmpty(prop.ValorNuevo?.ToString()) ? prop.ValorNuevo?.ToString() : "-";
                    if (anterior != "-" && nuevo != "-")
                    {
                        historialCambios.Add(new OrdenDeCargaCambiosHistorial
                        {
                            Id = 0,
                            Antes = anterior,
                            Despues = nuevo,
                            NombreColumnaCambio = prop.PropertyName,
                            FechaCambio = DateTime.Now,
                            Usuario_Id = usuario.Id,
                            OrdenDeCarga_Id = ordenDeCarga.Id,
                        });
                    }
                }

                foreach (var historialCambio in historialCambios)
                {
                    repositorio.Agregar(historialCambio);
                }
                ordenEditar.HistorialCambios.Concat(historialCambios);

                if (ordenEditar.NumeroEntrega != null)
                {
                    var resultadoSAP = ordenCargaConsumer.ModificarEntregaOrdenCarga(new ModificarEntregaRequest(ordenEditar));
                    if (resultadoSAP.HayError)
                    {
                        throw new InfoCustomException(resultadoSAP.Errores[0].Message);
                    }
                }
                if (historialCambios.Count > 0 && !esAdmin)
                {
                    emailFasService.EnviarMailSolicitudEdicion(ordenEditar, historialCambios);
                }

                repositorio.GuardarCambios();
                NotificarTransporte(ordenEditar.Id);

                if (!ordenEditar.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato)
                        && ordenEditar.TransporteExiste && string.IsNullOrEmpty(ordenEditar.NumeroEntrega)
                        && ordenEditar.AprobadoCredito && (!ordenEditar.SinSeleccionarFactura || !ordenEditar.EsFacturaAnticipada))
                {
                    GenerarEntregaSAP(ordenEditar);
                }
                else if (ordenEditar.Estado == EstadoOrdenDeCarga.SinEnviarASAP)
                {
                    if (estadoPrevio == EstadoOrdenDeCarga.PendienteCompensacion)
                    {
                        ordenEditar.Estado = EstadoOrdenDeCarga.PendienteCompensacion;
                    }
                    else if (!ordenEditar.EsFacturaAnticipada && !contratoKgDisponibles)
                    {
                        ordenEditar.Estado = EstadoOrdenDeCarga.Pendiente;
                        ordenEditar.DescripcionErrorInterno = "Orden con contrato entre 0 a 15Tn.";
                    }
                    else if (ordenEditar.EsFacturaAnticipada && ordenEditar.SinSeleccionarFactura)
                    {
                        ordenEditar.Estado = EstadoOrdenDeCarga.Pendiente;
                        ordenEditar.DescripcionErrorInterno = "Se debe seleccionar una factura.";
                    }
                    repositorio.GuardarCambios();
                }

                NotificarCamionAutorizadoMultiplesOrdenes(ordenEditar);

                var resultado = new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
                gestionAltas.OrdenId = ordenDeCarga.Id;
                EnviarMailAltaCuitTerceros(gestionAltas);
                Log.Info($"Result: {resultado.ToJson()}");
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Resultado { error = ex.Message };
            }
        }

        public CrearOrdenEnSAPResponse CrearOrdenEnSAP(CrearOrdenEnSAPRequest request, bool puedeEnviarASAP = false)
        {
            Log.Info($"CrearOrdenEnSAP(request: {request.ToJson()}, usuarioPuedeEnviarASAP: {puedeEnviarASAP})");
            var response = new CrearOrdenEnSAPResponse { ResultCreation = true };
            var creadaEnSAP = false;
            try
            {
                var ordenDeCarga = repositorio.Obtener<OrdenDeCarga>(q => q.Id == request.IdOrdenDeCarga);
                var mailUsuarioSAP = puedeEnviarASAP ? request.MailUsuarioSAP : _usuarioAutomaticoSAP;
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioSAP);

                var crearOrdenReq = new CrearOrdenRequest
                {
                    Cliente = ordenDeCarga.Cliente.CodigoProveedor,
                    CuitCliente = ordenDeCarga.Cliente.CUIT,
                    Contrato = ObtenerContratoDeOrden(ordenDeCarga),
                    Corredor = ordenDeCarga.CodigoCorredor,
                    Kilos = ordenDeCarga.Cantidad,
                    Material = ordenDeCarga.Producto.CodigoSap,
                    PedidoInput = ordenDeCarga.NumeroPedidoIngresado,
                    UsuarioSAP = usuario.UsuarioSap,
                    ValidaKg = true,
                    CuitDestino = ordenDeCarga.CUITDestino,
                    CuitDestinatario = ordenDeCarga.CUITDestinatario,
                    RazonSocialDestino = ordenDeCarga.RazonSocialDestino,
                    RazonSocialDestinatario = ordenDeCarga.RazonSocialDestinatario,
                    Reventa = ordenDeCarga.Reventa
                };

                var result = ordenCargaConsumer.CrearOrden(crearOrdenReq, out string numeroPedido, out string rawResult);
                ordenDeCarga.CodigoVerificacionSap = OrdenCargaCrearOrdenClass.GetCodigo(result);

                if (result == CrearOrdenResEnum.PedidoCreado || result == CrearOrdenResEnum.PedidoCreadoVerificarCredito)
                {
                    ordenDeCarga.InformadaSAP = true;
                    ordenDeCarga.NumeroPedido = numeroPedido;
                    ordenDeCarga.ContratoSAP = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                    ordenDeCarga.DescripcionErrorInterno = "";
                    ordenDeCarga.DescripcionCodigoVerificacionSap = "";
                    ordenDeCarga.CodigoVerificacionSap = "";
                    creadaEnSAP = true;
                }
                if (result == CrearOrdenResEnum.VerificarCantidadPendiente)
                {
                    ordenDeCarga.CodigoVerificacionSap = ResponseConverter.GetCodigoControlCarga(ControlCargaResEnum.MasDeUnContratoVigente);
                    ordenDeCarga.ContratoSinCantidadPendiente = true;
                    ordenDeCarga.DescripcionErrorInterno = "El contrato ingresado tiene menos de 15 toneladas disponibles. Puede elegir forzar la creación del pedido desde \"Crear pedido\" o anularlo.";
                }
                else if (result == CrearOrdenResEnum.ContratoSinKg)
                {
                    ordenDeCarga.ContratoSinCantidadPendiente = true;
                    ordenDeCarga.DescripcionErrorInterno = "El contrato ingresado no tiene kilogramos disponibles.";

                }
                else if (result == CrearOrdenResEnum.VerificarDatos)
                {
                    ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                }

                else if (result == CrearOrdenResEnum.Vacia)
                {
                    ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                }
                else if (result == CrearOrdenResEnum.NoEsperado)
                {
                    ordenDeCarga.DescripcionCodigoVerificacionSap = "Respuesta inesperada: " + rawResult;
                }

                var logCambioEstado = ordenDeCarga.ActualizarEstado();
                Log.Info(logCambioEstado);
                repositorio.GuardarCambios();
                if (creadaEnSAP)
                {
                    VerificarSituacionCrediticia(ordenDeCarga, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                response.ResultCreation = false;
                response.Error = $"Error al enviar la orden {request.IdOrdenDeCarga}";
            }
            return response;
        }

        public List<OrdenDeCargaDto> Listar(string mailUsuario, string fechaInicio, string fechaFin, string tipoOperacion, int? idProveedorSeleccionado = null)
        {
            Log.Info($"Listar(mailUsuario: {mailUsuario}, fechaInicio: {fechaInicio}, fechaFin: {fechaFin}, tipoOperacion: {tipoOperacion})");

            var fechaInicioDateTime = DataFormatter.StringToDateTime(fechaInicio, "inicio");
            var fechaFinDateTime = DataFormatter.StringToDateTime(fechaFin, "fin");

            var codigosEstadoConsultaHabilitados = new string[]
            {
                "INI",
                "GES",
                "GESRTA",
                "DOC"
            };

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esTercero = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaDeTerceros);
            var esInterno = EsUsuarioInterno(usuario);

            var descripcion = EstadoOrdenDeCarga.EdicionRechazada;
            fechaFinDateTime = fechaFinDateTime.AddDays(1);

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            var filtroEstados = ObtenerFiltroEstadosParaUsuario(usuario, esInterno);

            if (esInterno)
            {
                Expression<Func<OrdenDeCarga, bool>> filtro;
                if (tipoOperacion == "CyO")
                {
                    filtro = o => o.FechaCarga <= fechaFinDateTime
                        && o.FechaCarga >= fechaInicioDateTime
                        && filtroEstados.Contains(o.Estado)
                        && o.TipoContrato == TipoContratoFAS.CuentaYOrden;
                }
                else
                {
                    filtro = o => o.FechaCarga <= fechaFinDateTime
                        && o.FechaCarga >= fechaInicioDateTime
                        && filtroEstados.Contains(o.Estado) &&
                        o.TipoContrato != TipoContratoFAS.CuentaYOrden;
                }

                var listadoConFiltro = repositorio.ListarConsultable<OrdenDeCarga>(filtro);
                var hashPatentesCargadas = ObtenerHashPatentesCargadas(listadoConFiltro.AsEnumerable());
                var consultas = repositorio.Listar<ConsultaDetalle>(cd => listadoConFiltro.Any(orden => orden.Id == cd.Orden_Id)
                    && codigosEstadoConsultaHabilitados.Contains(cd.Consulta.EstadoConsulta.Code));
                listado = listadoConFiltro
                    .ToList()
                    .Select(x => new OrdenDeCargaDto
                    {
                        Id = x.Id,
                        Cliente = x.Cliente.CodigoProveedor,
                        RazonSocialCliente = x.Cliente.RazonSocial,
                        Fecha = x.FechaCarga.ToString("dd/MM/yyyy HH:mm"),
                        CUITCliente = x.CUITCliente,
                        Corredor = x.CodigoCorredor,
                        RazonSocialCorredor = string.IsNullOrWhiteSpace(x.Corredor?.RazonSocial) ? "-" : x.Corredor?.RazonSocial,
                        Contrato = !string.IsNullOrEmpty(x.ContratoSAP?.Trim()) ? x.ContratoSAP?.Trim() : x.ContratoIngresado?.Trim(),
                        Pedido = x.NumeroPedido ?? "-",
                        Entrega = x.NumeroEntrega ?? "-",
                        Material = x.Producto.Nombre,
                        DescripcionEstado = x.EdicionRechazada ? descripcion.ToFriendlyString() : x.Estado.ToFriendlyString(),
                        DescripcionEstadoListado = x.EdicionRechazada ? x.Estado.ToFriendlyString() + "(Edición Rechazada)" : x.Estado.ToFriendlyString(),
                        ColorSemaforo = x.Estado.ObtenerSemaforo(),
                        EsFacturaAnticipada = (x.NumeroPedidoIngresado != null),
                        PatenteChasis = x.ChasisAcoplado,
                        NoEstaEnSAP = x.Estado == EstadoOrdenDeCarga.SinEnviarASAP,
                        EstaSeleccionado = false,
                        EdicionRechazada = x.EdicionRechazada,
                        Escalable = x.Escalable,
                        TipoContrato = x.TipoContrato,
                        TienePatentesRepetidas = VerificarOrdenConPatentesRepetidas(x, hashPatentesCargadas),
                        IdsConsultasRealizadas = consultas.Where(cd => cd.Orden_Id == x.Id).Select(cd => cd.Id),
                        FleteMOA = x.FleteMOA ?? false,
                        TienePatenteMultiplesAutorizaciones = VerificarChasisConMultiplesAutorizaciones(x, hashPatentesCargadas)
                    }).OrderByDescending(y => y.Id).ToList();
            }
            else
            {
                var usuariosConMismoCuit = repositorio.Listar<Usuario, int>(x => x.Id, x => x.CUITRegistro == usuario.CUITRegistro);
                var proveedor = ObtenerProveedorSeleccionado(usuario, idProveedorSeleccionado);

                Expression<Func<OrdenDeCarga, bool>> filtro;
                if (tipoOperacion == "CyO")
                {
                    filtro = o =>
                        (usuariosConMismoCuit.Contains(o.UsuarioCreacion_Id) || o.Cliente.CodigoProveedor == proveedor.CodigoProveedor)
                        && o.FechaCarga <= fechaFinDateTime
                        && o.FechaCarga >= fechaInicioDateTime
                        && filtroEstados.Contains(o.Estado)
                        && o.TipoContrato == TipoContratoFAS.CuentaYOrden;
                }
                else
                {
                    filtro = o =>
                        (usuariosConMismoCuit.Contains(o.UsuarioCreacion_Id) || o.Cliente.CodigoProveedor == proveedor.CodigoProveedor)
                        && o.FechaCarga <= fechaFinDateTime
                        && o.FechaCarga >= fechaInicioDateTime
                        && filtroEstados.Contains(o.Estado)
                        && o.TipoContrato != TipoContratoFAS.CuentaYOrden;
                }

                var listadoConFiltro = repositorio.ListarConsultable<OrdenDeCarga>(filtro);
                var consultas = repositorio.Listar<ConsultaDetalle>(cd => listadoConFiltro.Any(orden => orden.Id == cd.Orden_Id)
                    && codigosEstadoConsultaHabilitados.Contains(cd.Consulta.EstadoConsulta.Code));

                listado = listadoConFiltro.ToList()
                    .Select(x => new OrdenDeCargaDto
                    {
                        Id = x.Id,
                        CUITCliente = x.CUITCliente,
                        Fecha = x.FechaCarga.ToString("dd/MM/yyyy HH:mm"),
                        Contrato = !string.IsNullOrEmpty(x.ContratoSAP?.Trim()) ? x.ContratoSAP?.Trim() : x.ContratoIngresado?.Trim(),
                        Cliente = x.Cliente.CodigoProveedor,
                        RazonSocialCliente = x.Cliente.RazonSocial,
                        Pedido = x.NumeroPedido ?? "-",
                        Entrega = x.NumeroEntrega ?? "-",
                        Material = x.Producto.Nombre,
                        DescripcionEstado = x.EdicionRechazada ? descripcion.ToUserFriendlyString() : x.Estado.ToUserFriendlyString(),
                        DescripcionEstadoListado = x.EdicionRechazada ? x.Estado.ToUserFriendlyString() + "(Edición Rechazada)" : x.Estado.ToUserFriendlyString(),
                        EsFacturaAnticipada = (x.NumeroPedidoIngresado != null),
                        PatenteChasis = x.ChasisAcoplado,
                        NoEstaEnSAP = x.Estado == EstadoOrdenDeCarga.SinEnviarASAP,
                        EstaSeleccionado = false,
                        EdicionRechazada = x.EdicionRechazada,
                        TipoContrato = x.TipoContrato,
                        IdsConsultasRealizadas = consultas.Where(cd => cd.Orden_Id == x.Id).Select(cd => cd.Id)
                    }).OrderByDescending(y => y.Id).ToList();
            }
            return listado;
        }

        public OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esInterno = EsUsuarioInterno(usuario);

            OrdenDeCarga orden;
            if (esInterno)
            {
                orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.CUIT);
                orden = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.CUITCliente) && n.Id == ordenId).FirstOrDefault();
            }

            if (orden == null) throw new InfoCustomException("No se encontró ninguna orden de carga");

            Proveedor cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);

            var ordenDeCargaCambiosHistorial = ObtenerCambiosHistorial(orden);

            var contratoSAP = ordenCargaConsumer.ObtenerContratoSAP(orden.ContratoIngresado, TipoContratoFAS.Todos);

            var ordenesPendientes = ObtenerOrdenesPendientesDeCliente(orden.Cliente.CodigoProveedor);

            var ordenDto = new OrdenDeCargaDetalleDto(orden, ordenDeCargaCambiosHistorial, cliente)
            {
                ContratoSeleccionado = new ContratoOrdenFas(orden)
                {
                    KgDisponibles = kgDisponiblesFasService.ObtenerKgDisponiblesContrato(contratoSAP, ordenesPendientes)
                },
                OrdenesConPatentesRepetidas = esInterno ? ObtenerOrdenesConPatentesRepetidas(orden) : null
            };

            return ordenDto;
        }

        public List<OrdenDeCarga> VerificarVencimientoOrdenDeCarga()
        {
            var dayOfWeek = DateTime.Now.DayOfWeek;
            var feriados = feriadoService.ObtenerFeriados();
            var fechaActual = DateTime.Now.Date;
            foreach (var diasFeriados in feriados)
            {
                if (diasFeriados.Date == fechaActual)
                {
                    return null;
                }
            }

            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesDeCargaSapJob").Habilitado == false)
                return null;

            if ((dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday))
            {
                return null;
            }

            int Usuario_Id = repositorio.Obtener<Usuario>(a => a.Mail == "moaoperaciones@molinosagro.com.ar").Id;

            var ordenes = repositorio.Listar<OrdenDeCarga>(o => o.FechaVencimiento < fechaActual && (o.Estado == EstadoOrdenDeCarga.EntregaGenerada || o.Estado == EstadoOrdenDeCarga.Vencida));
            foreach (var orden in ordenes)
            {
                if (orden.Estado == EstadoOrdenDeCarga.EntregaGenerada)
                {
                    orden.Estado = EstadoOrdenDeCarga.Vencida;
                    orden.HistorialCambios.Add(new OrdenDeCargaCambiosHistorial
                    {
                        Antes = EstadoOrdenDeCarga.EntregaGenerada.ToFriendlyString(),
                        Despues = EstadoOrdenDeCarga.Vencida.ToFriendlyString(),
                        FechaCambio = DateTime.Now,
                        NombreColumnaCambio = "Estado",
                        Usuario_Id = Usuario_Id
                    });
                }
            }
            repositorio.GuardarCambios();
            emailFasService.EnviarMailVencieronOrdenesDeCarga(ordenes);

            return ordenes;
        }

        public string NotificarVencimientoOrdenCarga(int ordenId, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var puedeEnviarASAP = usuario.TienePermiso(PermisoEnum.EnviarASap);
            var orden = repositorio.Obtener<OrdenDeCarga>(x => x.Id == ordenId);

            if (puedeEnviarASAP)
            {
                AnularOrdenSap(orden);
            }

            emailFasService.EnviarMailOrdenDeCargaVencida(orden);

            orden.Estado = EstadoOrdenDeCarga.AnuladaPorVencimiento;
            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAnulada;
        }

        public OrdenDeCargaEditarDto ObtenerEditar(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var ordenDto = new OrdenDeCargaEditarDto(orden);

            return ordenDto;
        }

        public List<OrdenDeCargaHistorialDto> ObtenerEditarHistorial(string mailUsuario, int ordenId)
        {
            var ordenHistorialDtoLista = new List<OrdenDeCargaHistorialDto>();
            foreach (var ordenDeCargaHistorial in repositorio.Listar<OrdenDeCargaCambiosHistorial>(o => o.OrdenDeCarga_Id == ordenId))
            {
                var ordenHistorialDto = new OrdenDeCargaHistorialDto(ordenDeCargaHistorial);
                ordenHistorialDtoLista.Add(ordenHistorialDto);
            }
            return ordenHistorialDtoLista;
        }

        public string AnularOrden(int ordenId, string mailUsuario)
        {
            var estadosPuedeAnular = new List<EstadoOrdenDeCarga>
            {
                EstadoOrdenDeCarga.Pendiente,
                EstadoOrdenDeCarga.Confirmado,
                EstadoOrdenDeCarga.ContratoVencido,
                EstadoOrdenDeCarga.EntregaGenerada,
                EstadoOrdenDeCarga.EntregaPendiente,
                EstadoOrdenDeCarga.PendienteCompensacion,
                EstadoOrdenDeCarga.PendienteAprobacionCredito,
                EstadoOrdenDeCarga.Vencida,
                EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion,
                EstadoOrdenDeCarga.ErrorDeCarga,
            };
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            if (!estadosPuedeAnular.Contains(orden.Estado))
                throw new ValidationCustomException("La orden no puede anularse debido a su estado actual.");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            //var puedeEnviarASAP = usuario.TienePermiso(PermisoEnum.EnviarASap);

            //if (puedeEnviarASAP)
            //{
            //    AnularOrdenSap(orden);
            //}

            AnularOrdenSap(orden);

            var ordenHistorial = new OrdenDeCargaCambiosHistorial()
            {
                Id = 0,
                Antes = orden.Estado.ToFriendlyString(),
                Despues = EstadoOrdenDeCarga.Anulada.ToFriendlyString(),
                NombreColumnaCambio = "estado",
                FechaCambio = DateTime.Now,
                Usuario_Id = usuario.Id,
                OrdenDeCarga_Id = orden.Id
            };
            repositorio.Agregar(ordenHistorial);
            orden.Estado = EstadoOrdenDeCarga.Anulada;
            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAnulada;
        }

        public OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga ordenDeCarga)
        {
            OrdenDeCargaDto result = new OrdenDeCargaDto();
            if (ordenDeCarga.CUITCliente == null)
            {
                result.ordenes.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return result;
            }
            var cliente = repositorio.Obtener<Proveedor>(x =>
                x.CUIT == ordenDeCarga.CUITCliente &&
                x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return new OrdenDeCargaDto();
            }
            result.ordenes = repositorio.Listar<OrdenDeCarga, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.ChasisAcoplado,
                value = x.PatenteAcoplado
            }, x => x.Cliente_Id == cliente.Id);
            result.ordenes = result.ordenes.Distinct().ToList();
            return result;
        }

        public VisualizarClienteResponse VisualizarCliente(VisualizarClienteRequest request)
        {
            Log.Info($"VisualizarCliente(request: {request.ToJson()})");
            try
            {
                var validator = new VisualizarClienteRequestValidator();
                validator.ValidateAndThrow(request);

                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(string.Empty, string.Empty, request.Corredor, request.FechaInicio, request.FechaFin, string.Empty, request.Pendiente, TipoContratoFAS.Todos, 1);

                var response = new VisualizarClienteResponse
                {
                    Clientes = GetClientesFromVisualizarClienteProducto(ordenCargaVisualizarClienteWSMOAResponse, request)
                };

                if (!string.IsNullOrEmpty(request.Corredor))
                {
                    SincronizarRelacionesCorredorCliente(request.Corredor, response);
                }
                return response;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public VisualizarProductoResponse VisualizarProducto(VisualizarProductoRequest request)
        {
            Log.Info($"VisualizarCliente(request: {request.ToJson()})");
            try
            {
                var validator = new VisualizarProductoRequestValidator();
                validator.ValidateAndThrow(request);

                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(string.Empty, request.Contrato, string.Empty, request.FechaInicio, request.FechaFin, string.Empty, request.Pendiente, TipoContratoFAS.Todos, 2);

                var response = new VisualizarProductoResponse
                {
                    Productos = GetProductosFromVisualizarClienteProducto(ordenCargaVisualizarClienteWSMOAResponse)
                };
                return response;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public ValidarCorredorClienteContratoProductoResponse ValidarCorredorClienteContratoProducto(ValidarCorredorClienteContratoProductoRequest request)
        {
            request.Contrato = request.Contrato?.Trim();
            request.ClienteCodigo = request.ClienteCodigo?.Trim();
            request.Corredor = request.Corredor?.Trim();
            request.ProductoId = request.ProductoId?.Trim();
            request.UsuarioEmail = request.UsuarioEmail?.Trim();
            Log.Info($"ValidarCorredorClienteContratoProducto(request: {request.ToJson()})");
            try
            {
                var validator = new ValidarCorredorClienteContratoProductoRequestValidator();
                validator.ValidateAndThrow(request);
                var producto = repositorio.Obtener<Material>(Convert.ToInt32(request.ProductoId));
                if (producto == null)
                {
                    var error = new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Producto"));
                    Log.Error(error);
                    throw error;
                }
                Log.Debug(this.GetType().Name, "ValidarCorredorClienteContratoProducto", $" producto: {producto?.Id.ToJson()}");
                var cliente = repositorio.Obtener<Proveedor>(x =>
                    (x.CUIT == request.ClienteCuit || x.CodigoProveedor == request.ClienteCodigo) &&
                    x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                    x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
                if (cliente == null)
                {
                    var error = new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Cliente"));
                    Log.Error(error);
                    throw error;
                }
                Log.Debug(this.GetType().Name, "ValidarCorredorClienteContratoProducto", $" cliente: {cliente?.Id.ToJson()}");
                request.ClienteCuit = cliente.CUIT;
                request.ClienteCodigo = cliente.CodigoProveedor;
                request.Contrato = request.Contrato.TrimStart(new Char[] { '0' });
                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(request.ClienteCodigo, request.Contrato, request.Corredor, request.FechaInicio, request.FechaFin, producto.CodigoSap, request.Pendiente, TipoContratoFAS.Todos, 3);
                var response = new ValidarCorredorClienteContratoProductoResponse();
                if (ordenCargaVisualizarClienteWSMOAResponse.Resultados.Count == 0)
                {
                    response.ResultValidation = false;
                }
                else
                {
                    var corredorCodigo = ordenCargaVisualizarClienteWSMOAResponse.Resultados[0].Corredor;
                    var corredorEmail = request.UsuarioEmail;
                    var corredor = repositorio.Obtener<Proveedor>(x =>
                        x.CodigoProveedor == corredorCodigo &&
                        x.Mail == corredorEmail &&
                        x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                        x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor);
                    if (corredor == null)
                    {
                        corredor = repositorio.Obtener<Proveedor>(x =>
                            x.CodigoProveedor == corredorCodigo &&
                            x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                            x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor);
                    }
                    response.ResultValidation = GetResultFromValidarCorredorClienteContratoProducto(ordenCargaVisualizarClienteWSMOAResponse, request, producto);
                    if (response.ResultValidation && corredor != null)
                    {
                        CrearRelacionCorredorCliente(corredor, cliente);
                    }
                }
                Log.Info($" response: {response.ToJson()}");
                return response;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public Resultado SeleccionarContrato(int ordenId, string contratoSAP, string mailUsuario)
        {
            string resultado = SuccessMsg.OrdenDeCargaActualizada;
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            if (orden != null && !string.IsNullOrEmpty(orden.ContratoSAP))
            {
                return new Resultado { info = "La orden ya tiene un contrato seleccionado." };
            }
            orden.ContratoSAP = contratoSAP;
            orden.DescripcionErrorInterno = "";
            var logCambioEstado = orden.ActualizarEstado();
            Log.Info("SeleccionarContrato. " + logCambioEstado);
            var contratoEnSAP = ordenCargaConsumer.ObtenerContratoSAP(contratoSAP, null);
            orden.TipoContrato = contratoEnSAP.TipoContrato;
            if (!ValidarVencimientoContrato(contratoSAP, orden.Cliente))
            {
                orden.Estado = EstadoOrdenDeCarga.ContratoVencido;
                repositorio.GuardarCambios();
                emailFasService.EnviarMailContratoVencido(orden);
                return new Resultado { error = "El contrato seleccionado está vencido." };
            }

            NotificarVariasFacturas(orden);

            if (orden.TipoContrato == TipoContratoFAS.Anticipado &&
                string.IsNullOrEmpty(orden.NumeroFacturaSeleccionada))
            {
                orden.Estado = EstadoOrdenDeCarga.Pendiente;
                orden.DescripcionErrorInterno = "Hay más de una factura para seleccionar.";
                repositorio.GuardarCambios();
                return new Resultado { error = "El contrato tiene más de una factura para seleccionar" };
            }

            if (!orden.TransporteExiste)
            {
                resultado = VerificarTransporte(orden);
                if (resultado == _transporteNoExiste)
                {
                    orden.DescripcionCodigoVerificacionSap = _transporteNoExiste;
                    emailFasService.EnviarMailTransporteNoExiste(orden);
                }
            }
            if (orden.TransporteExiste && !string.IsNullOrEmpty(orden.ContratoSAP))
            {
                VerificarOrden(orden, orden.Cliente, false);

                if (!orden.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    if (orden.TipoContrato == TipoContratoFAS.Anticipado)
                        return GenerarEntregaSAP(orden);

                    var creadaEnSaP = !string.IsNullOrEmpty(orden.NumeroPedido) || CrearPedidoEnSAP(orden, orden.Cliente, true, mailUsuario);
                    if (creadaEnSaP)
                    {
                        return VerificarSituacionCrediticia(orden, true);
                    }
                }
            }
            repositorio.GuardarCambios();
            return new Resultado { Mensaje = resultado };
        }

        public Resultado SeleccionarFactura(int ordenId, string numeroFacturaSeleccionada)
        {
            string resultado = SuccessMsg.OrdenDeCargaActualizada;
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            facturaAnticipadaService.SeleccionarFactura(orden, numeroFacturaSeleccionada);

            var logCambioEstado = orden.ActualizarEstado();
            Log.Info("SeleccionarFactura. " + logCambioEstado);
            var contrato = ObtenerContratoDeOrden(orden);

            if (string.IsNullOrEmpty(orden.ContratoSAP))
            {
                orden.Estado = EstadoOrdenDeCarga.Pendiente;
                orden.DescripcionErrorInterno = "Falta seleccionar un contrato.";
                repositorio.GuardarCambios();
                return new Resultado { error = "Falta seleccionar un contrato." };
            }
            if (!ValidarVencimientoContrato(contrato, orden.Cliente))
            {
                orden.Estado = EstadoOrdenDeCarga.ContratoVencido;
                repositorio.GuardarCambios();
                emailFasService.EnviarMailContratoVencido(orden);
                return new Resultado { error = "El contrato seleccionado está vencido" };
            }

            if (!orden.TransporteExiste)
            {
                resultado = VerificarTransporte(orden);
                if (resultado == _transporteNoExiste)
                {
                    orden.DescripcionCodigoVerificacionSap = _transporteNoExiste;
                    emailFasService.EnviarMailTransporteNoExiste(orden);
                }
            }
            VerificarOrden(orden, orden.Cliente, false);

            if (!orden.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
            {
                return GenerarEntregaSAP(orden);
            }

            repositorio.GuardarCambios();

            return new Resultado { Mensaje = resultado };
        }

        public string ForzarCreacionOrden(int ordenId, string mailUsuario)
        {
            Log.Info($"ForzarCreacionOrden " + ordenId.ToJson());
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var creadaEnSaP = CrearPedidoEnSAP(orden, orden.Cliente, false, mailUsuario);

            if (creadaEnSaP)
            {
                VerificarSituacionCrediticia(orden, true);
            }

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        public List<string> ObtenerContratos(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var consumerReq = new OrdenCargaVisualizarClienteWSMOARequest
            {
                Cliente = orden.Cliente.CodigoProveedor,
                Contrato = string.Empty,
                Corredor = orden.Corredor != null ? orden.Corredor.CodigoProveedor : string.Empty,
                Material = orden.Producto.CodigoSap,
                Pendiente = true,
                TipoContrato = Constante.FAS_FILTRO_TIPO_CONTRATO
            };

            var consumerRes = new OrdenCargaConsumerMOA().OrdenCargaVisualizarClienteExecute(consumerReq);

            if (consumerRes == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }
            if (consumerRes.Resultados == null || consumerRes.Resultados.Count == 0)
            {
                throw new ValidationCustomException("No se encontraron contratos abiertos para la orden");
            }
            return consumerRes.Resultados.Select(x => x.Contrato).ToList();
        }

        public string VerificarTransporte(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return VerificarTransporte(orden, true, mailUsuario);
        }

        public void VerificarTransporteBulk()
        {
            var estadosNoVerificaTransporte = new List<EstadoOrdenDeCarga>
            {
                EstadoOrdenDeCarga.AnuladaPorVencimiento,
                EstadoOrdenDeCarga.Anulada
            };

            var estadoJob = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VerificarTransporteOrdenesDeCargaJob");
            if (!estadoJob.Habilitado)
                return;

            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(o => !estadosNoVerificaTransporte.Contains(o.Estado) && !o.TransporteExiste))
            {
                VerificarTransporte(ordenDeCarga);
            }

            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(o => o.Estado == EstadoOrdenDeCarga.Vencida || o.Estado == EstadoOrdenDeCarga.EntregaGenerada || (o.Estado == EstadoOrdenDeCarga.EdicionRechazada && !string.IsNullOrEmpty(o.NumeroEntrega))))
            {
                VerificarEstadoEntrega(ordenDeCarga);
            }
        }

        public void CrearOrdenEnSAPBulk()
        {
            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnviarASAPOrdenDeCargaJob").Habilitado == false)
                return;

            foreach (var ordenDeCarga in repositorio
                .Listar<OrdenDeCarga>(q => q.TipoContrato == TipoContratoFAS.Normal && q.Estado == EstadoOrdenDeCarga.SinEnviarASAP))
            {
                if (ContratoConMas15TN(ordenDeCarga))
                {
                    var crearOrdenEnSAPRequest = new CrearOrdenEnSAPRequest()
                    {
                        IdOrdenDeCarga = ordenDeCarga.Id,
                        ClienteCodigo = ordenDeCarga.Cliente?.CodigoProveedor,
                        ContratoSAP = ordenDeCarga.ContratoSAP,
                        CorredorCodigo = ordenDeCarga.Corredor?.CodigoProveedor,
                        Cantidad = ordenDeCarga.Cantidad,
                        MaterialCodigoSAP = ordenDeCarga.Producto?.CodigoSap,
                        NumeroPedidoIngresado = ordenDeCarga.NumeroPedidoIngresado,
                        MailUsuarioSAP = string.Empty
                    };
                    Log.Info($"CrearOrdenEnSAPBulk - CrearOrdenEnSAP -> OrdenId: {ordenDeCarga.Id}");
                    CrearOrdenEnSAP(crearOrdenEnSAPRequest);
                }
            }
        }

        public Resultado VerificarSituacionCrediticia(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            return VerificarSituacionCrediticia(orden, false);
        }

        public string ActivarOC(int ordenId, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var Usuario_Id = usuario.Id;
            var orden = repositorio.Obtener<OrdenDeCarga>(x => x.Id == ordenId);

            var fechaVencimientoOriginal = orden.FechaVencimiento.Value;
            var fechaVencimientoNueva = CalcularFechaVencimiento(fechaVencimientoOriginal);

            if (!EsUsuarioInterno(usuario))
            {
                fechaVencimientoNueva = fechaVencimientoNueva.AddDays(-1);
            }

            orden.Estado = EstadoOrdenDeCarga.EntregaGenerada;
            orden.FechaVencimiento = fechaVencimientoNueva;
            orden.FechaVencimientoAmpliada = true;

            orden.HistorialCambios.Add(new OrdenDeCargaCambiosHistorial
            {
                Antes = EstadoOrdenDeCarga.Vencida.ToFriendlyString(),
                Despues = EstadoOrdenDeCarga.EntregaGenerada.ToFriendlyString(),
                FechaCambio = DateTime.Now,
                NombreColumnaCambio = "Estado",
                Usuario_Id = Usuario_Id
            });
            orden.HistorialCambios.Add(new OrdenDeCargaCambiosHistorial
            {
                Antes = "No",
                Despues = "Si",
                FechaCambio = DateTime.Now,
                NombreColumnaCambio = "FechaVencimientoAmpliada",
                Usuario_Id = Usuario_Id
            });
            orden.HistorialCambios.Add(new OrdenDeCargaCambiosHistorial
            {
                Antes = fechaVencimientoOriginal.ToString(),
                Despues = orden.FechaVencimiento.ToString(),
                FechaCambio = DateTime.Now,
                NombreColumnaCambio = "FechaVencimiento",
                Usuario_Id = Usuario_Id
            });
            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        public void VerificarSituacionCrediticiaJob()
        {
            if (!repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VerificarSituacionCrediticiaJob").Habilitado)
                return;

            var ordenes = repositorio.Listar<OrdenDeCarga>(oc => oc.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito);
            foreach (OrdenDeCarga orden in ordenes)
            {
                VerificarSituacionCrediticia(orden, false);
            }
        }

        public OrdenDeCargaDetalleDto ObtenerPorNroEntrega(string mailUsuario, string nroEntrega)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(oc => oc.NumeroEntrega == nroEntrega);
            if (orden == null) throw new InfoCustomException("No se ha encontrado ninguna orden de carga");
            return Obtener(mailUsuario, orden.Id);
        }

        public ObtenerContratosDisponiblesResponse ObtenerContratosDisponibles(ObtenerContratosDisponiblesRequest req, string mailUsuario, string tipoOperacion)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var esInterno = EsUsuarioInterno(usuario);
                var rangoFechas = string.IsNullOrEmpty(req.FechaDesde) || string.IsNullOrEmpty(req.FechaHasta) ? null :
                    CommonUtil.toDateList(req.FechaDesde, req.FechaHasta);

                var consumerReq = new OrdenCargaVisualizarClienteWSMOARequest
                {
                    Cliente = req.ClienteCodigo,
                    Contrato = string.Empty,
                    Corredor = req.CorredorCodigo,
                    Fechas = rangoFechas,
                    Material = string.Empty,
                    Pendiente = true, // Contratos ABIERTOS
                    TipoContrato = tipoOperacion == "CyO"? TipoContratoFAS.CuentaYOrden : TipoContratoFAS.Todos
                };

                var ordenCargaConsumer = new OrdenCargaConsumerMOA();
                var consumerRes = ordenCargaConsumer.OrdenCargaVisualizarClienteExecute(consumerReq);

                if (consumerRes == null)
                {
                    throw new ValidationCustomException(ErrorMsg.Error);
                }

                RemoverContratosConBloqueo(consumerRes);

                if(tipoOperacion != "CyO")
                {
                    consumerRes.Resultados = consumerRes.Resultados.Where(r => r.TipoContrato != TipoContratoFAS.CuentaYOrden).ToList();
                }

                if (consumerRes.Resultados == null || consumerRes.Resultados.Count == 0)
                {
                    throw new ValidationCustomException("No se encontraron contratos abiertos para los datos ingresados");
                }

                var productosCodigosSap = consumerRes.Resultados.Select(p => p.Producto.Trim().TrimStart('0')).Distinct().ToList();

                var productosBD = repositorio
                    .Listar<Material>(m =>
                        m.TablaSeccionMaterial == TablaSeccionMaterial.OrdenDeCarga &&
                        productosCodigosSap.Contains(m.CodigoSap));

                var ordenesPendientes = ObtenerOrdenesPendientesDeCliente(req.ClienteCodigo);

                var contratos =
                    consumerRes.Resultados
                        .Select(contratoSap =>
                            new ContratoOrdenFas(contratoSap, productosBD)
                            {
                                KgDisponibles = kgDisponiblesFasService.ObtenerKgDisponiblesContrato(contratoSap, ordenesPendientes)
                            })
                        .OrderBy(contrato => contrato.DescripcionProducto)
                        .ToList();

                if (!esInterno)
                {
                    contratos = contratos.Where(c => c.CondicionRetiro == CondicionRetiro.RetiroEnPlanta).ToList();
                }

                return new ObtenerContratosDisponiblesResponse { Contratos = contratos };
            }
            catch (InfoCustomException) { throw; }
            catch (ValidationCustomException) { throw; }
            catch (Exception ex)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, ex);
            }
        }

        private static void RemoverContratosConBloqueo(OrdenCargaVisualizarClienteWSMOAResponse consumerRes)
        {
            consumerRes.Resultados = consumerRes.Resultados
                .Where(c =>
                    !c.BloqueoEntrega &&
                    (!c.Detalles.Any() || c.Detalles.Any(d => !d.BloqueoEntrega)))
                .ToList();
            consumerRes.Resultados.ForEach(c => c.Detalles = c.Detalles.Where(d => !d.BloqueoEntrega).ToList());
        }

        public string EnviarOrdenesASAP(List<int> ordenesId, string mailUsuario)
        {
            var ordenes = repositorio.Listar<OrdenDeCarga>(a => ordenesId.Contains(a.Id) && a.Estado == EstadoOrdenDeCarga.SinEnviarASAP);
            var errores = new List<int>();
            foreach (var ordenDeCarga in ordenes)
            {
                if (ordenDeCarga.TipoContrato == TipoContratoFAS.Normal || ordenDeCarga.TipoContrato == TipoContratoFAS.CuentaYOrden)
                {
                    var crearOrdenEnSAPRequest = new CrearOrdenEnSAPRequest()
                    {
                        IdOrdenDeCarga = ordenDeCarga.Id,
                        ClienteCodigo = ordenDeCarga.Cliente?.CodigoProveedor,
                        ContratoSAP = ordenDeCarga.ContratoSAP,
                        CorredorCodigo = ordenDeCarga.Corredor?.CodigoProveedor,
                        Cantidad = ordenDeCarga.Cantidad,
                        MaterialCodigoSAP = ordenDeCarga.Producto?.CodigoSap,
                        NumeroPedidoIngresado = ordenDeCarga.NumeroPedidoIngresado,
                        MailUsuarioSAP = mailUsuario
                    };
                    var response = CrearOrdenEnSAP(crearOrdenEnSAPRequest, true);
                    if (response.Error != null)
                    {
                        errores.Add(ordenDeCarga.Id);
                    }
                }
                else
                {
                    if (ordenDeCarga.TipoContrato == TipoContratoFAS.Anticipado)
                    {
                        var resultado = GenerarEntregaSAP(ordenDeCarga);
                        if (resultado.error != null)
                            errores.Add(ordenDeCarga.Id);
                    }
                }
            }
            if (errores.Any())
                throw new InfoCustomException($"Las siguientes órdenes no pudieron enviarse correctamente: {string.Join(", ", errores)}");
            return "Se han enviado las órdenes";
        }

        public ValidarSisaCorredorClienteResponse ValidarSisaCorredorCliente(string corredorCodigo, string clienteCodigo)
        {
            var materialCodigo = ObtenerMaterialValidaSisa();
            var controlarCargaReq = new ControlCargaRequest
            {
                Cliente = clienteCodigo,
                Corredor = corredorCodigo.StartsWith("C") ? corredorCodigo : "",
                Material = materialCodigo,
                SoloSisa = true
            };

            var responseHandler = ordenCargaConsumer.ControlarCarga(controlarCargaReq);

            var res = new ValidarSisaCorredorClienteResponse
            {
                ClienteHabilitadoEnSisa = !responseHandler.TieneRespuesta(ControlCargaResEnum.ClienteInhabilitadoEnSisa),
                CorredorHabilitadoEnSisa = !responseHandler.TieneRespuesta(ControlCargaResEnum.CorredorInhabilitadoEnSisa)
            };
            return res;
        }

        public bool ValidarSisaCuit(string cuit, string campo)
        {
            var validaSISA = new ValidaSisaCuit(campo);

            var cuitDestinatario = validaSISA.Destinatario ? cuit : null;
            var cuitDestino = validaSISA.Destino ? cuit : null;
            var codigoMaterial = ObtenerMaterialValidaSisa();

            return ValidarSisa(cuitDestinatario, cuitDestino, codigoMaterial);
        }

        public void VerificarCompensacion(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            Log.Info($"Verificar Compensacion: orden: {ordenId}");
            VerificarCompensacion(orden);
        }

        public List<AutoCompleteDropdownElement> ObtenerCuilsChofer(OrdenDeCarga ordenDeCarga)
        {
            var cuils = new List<AutoCompleteDropdownElement>();
            if (ordenDeCarga.Cliente == null && ordenDeCarga.PatenteAcoplado == null)
            {
                cuils.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return cuils;
            }
            var cliente = repositorio.Obtener<Proveedor>(x =>
                x.CUIT == ordenDeCarga.CUITCliente &&
                x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return cuils;
            }
            cuils = repositorio.Listar<OrdenDeCarga, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.CUITChofer.Substring(0, 2) + "-" + x.CUITChofer.Substring(2, 8) + "-" + x.CUITChofer.Substring(10, 1),
                value = x.CUITChofer
            }, x => x.Cliente_Id == cliente.Id && x.PatenteAcoplado == ordenDeCarga.PatenteAcoplado);

            return cuils.Distinct().ToList();
        }

        public List<AutoCompleteDropdownElement> ObtenerCuitsTransporte(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            var cuits = new List<AutoCompleteDropdownElement>();
            if (ordenDeCarga.CUITCliente == null && ordenDeCarga.PatenteAcoplado == null)
            {
                cuits.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return cuits;
            }
            var cliente = repositorio.Obtener<Proveedor>(x =>
                x.CUIT == ordenDeCarga.CUITCliente &&
                x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
            if (cliente == null)
            {
                return cuits;
            }
            cuits = repositorio.Listar<OrdenDeCarga, AutoCompleteDropdownElement>(x => new AutoCompleteDropdownElement
            {
                label = x.CUITTransporte.Substring(0, 2) + "-" + x.CUITTransporte.Substring(2, 8) + "-" + x.CUITTransporte.Substring(10, 1),
                value = x.CUITTransporte
            }, x => x.Cliente_Id == cliente.Id && x.PatenteAcoplado == ordenDeCarga.PatenteAcoplado);

            return cuits.Distinct().ToList();
        }

        public bool ValidarOrdenActivaScato(string ordenId)
        {
            var orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id.ToString() == ordenId);
            if (string.IsNullOrEmpty(orden.NumeroEntrega))
                return false;
            Log.Info($"Obteniendo estado de la orden {orden.Id} en Scato con nro Entrega: " + orden.NumeroEntrega);
            var result = this.scatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroDocumento(orden.NumeroEntrega).FirstOrDefault();
            if (result == null)
                return false;
            Log.Info($"ScatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroDocumento Params => OrdenId: {orden.Id}, NroEntrega: {orden.NumeroEntrega}, Response => Terminado:{result.Terminado}");
            return result.Terminado != true;
        }

        public Resultado VerificarCuitsTerceros(int ordenId)
        {
            string resultado = SuccessMsg.OrdenDeCargaActualizada;
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            ValidarExistenciaCuitsTerceros(orden);
            if (!orden.CuitTerceroExisteScato)
            {
                return ResultadoCuitsTercerosNoExisten(orden);
            }

            var contrato = ObtenerContratoDeOrden(orden);

            if (string.IsNullOrEmpty(orden.ContratoSAP))
            {
                orden.Estado = EstadoOrdenDeCarga.Pendiente;
                orden.DescripcionErrorInterno = "Falta seleccionar un contrato.";
                repositorio.GuardarCambios();
                return new Resultado { error = "Falta seleccionar un contrato." };
            }
            if (!ValidarVencimientoContrato(contrato, orden.Cliente))
            {
                orden.Estado = EstadoOrdenDeCarga.ContratoVencido;
                repositorio.GuardarCambios();
                emailFasService.EnviarMailContratoVencido(orden);
                return new Resultado { error = "El contrato seleccionado está vencido" };
            }
            if (!orden.TransporteExiste)
            {
                resultado = VerificarTransporte(orden);
                if (resultado == _transporteNoExiste)
                {
                    orden.DescripcionCodigoVerificacionSap = _transporteNoExiste;
                    emailFasService.EnviarMailTransporteNoExiste(orden);
                }
            }

            NotificarVariasFacturas(orden);
            if (orden.TipoContrato == TipoContratoFAS.Anticipado &&
                string.IsNullOrEmpty(orden.NumeroFacturaSeleccionada))
            {
                orden.Estado = EstadoOrdenDeCarga.Pendiente;
                orden.DescripcionErrorInterno = "Hay más de una factura para seleccionar.";
                repositorio.GuardarCambios();
                return new Resultado { error = "El contrato tiene más de una factura para seleccionar" };
            }

            VerificarOrden(orden, orden.Cliente, false);

            if (!orden.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                return GenerarEntregaSAP(orden);

            repositorio.GuardarCambios();

            return new Resultado { Mensaje = resultado };
        }

        public List<DestinatarioDto> ObtenerDestinatariosConsultaFas(int ordenId)
        {
            var destinatarios = new List<DestinatarioDto>();

            if (ordenId == 0)
            {
                return destinatarios;
            }

            var orden = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == ordenId);

            var mailCreador = this.repositorio.Obtener<Usuario>(u => u.Id == orden.UsuarioCreacion_Id);
            if (mailCreador != null)
            {
                destinatarios.Add(new DestinatarioDto { Campo = "Usuario creador", Mail = mailCreador.Mail, UsuarioId = mailCreador.Id, NombreTipoUsuario = mailCreador.TipoUsuario.NombreCorto });
            }

            var mailCliente = this.repositorio.Obtener<Usuario>(u => u.Mail == orden.Cliente.Mail && u.TipoUsuario.Id == orden.Cliente.TipoProveedor.Id);
            if (mailCliente != null)
            {
                destinatarios.Add(new DestinatarioDto { Campo = "Cliente", Mail = mailCliente.Mail, UsuarioId = mailCliente.Id, NombreTipoUsuario = mailCliente.TipoUsuario.NombreCorto });
            }

            return destinatarios.ToList();
        }

        public void VerificarOrdenesFacturaCompensadaJob()
        {
            var estadoJob = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VerificarOrdenesFacturaCompensadaJob");
            if (estadoJob == null || !estadoJob.Habilitado)
                return;

            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(oc => oc.TipoContrato == TipoContratoFAS.Anticipado && oc.Estado == EstadoOrdenDeCarga.PendienteCompensacion))
            {
                VerificarCompensacion(ordenDeCarga);
            }
        }

        public List<ClienteSAPResponse> GetClientesVigentesSAP(string fechaInicio, string fechaFin)
        {
            List<Mod.FechaWS> fechas = null;
            var ordenCargaConsumerMOA = new OrdenCargaConsumerMOA();

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
            }

            var request = new OrdenCargaVisualizarClienteWSMOARequest()
            {
                Cliente = string.Empty,
                Contrato = string.Empty,
                Corredor = string.Empty,
                Fechas = fechas,
                Material = string.Empty,
                Pendiente = true,
                TipoContrato = TipoContratoFAS.Todos,
            };

            Log.Info($"GetClientesVigentesSAP(request: {request.ToJson()})");
            var ordenCargaVisualizarClienteWSMOAResponse = ordenCargaConsumerMOA.OrdenCargaVisualizarClienteExecute(request);

            if (ordenCargaVisualizarClienteWSMOAResponse == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            List<ClienteSAPResponse> response = ordenCargaVisualizarClienteWSMOAResponse.Resultados
                .Select(c => new ClienteSAPResponse
                {
                    RazonSocial = c.NombreCliente,
                    CuitCliente = c.CuitCliente,
                    CodigoProveedor = c.Cliente
                }).Distinct(new CustomComparerClienteSap()).ToList();

            Log.Info("GetClientesVigentesSAP result " + response.ToJson());

            return response;
        }

        public List<Proveedor> FiltrarNoExistentesWeb(List<ClienteSAPResponse> clientes)
        {
            var clientesNuevos = new List<Proveedor>();

            foreach (ClienteSAPResponse c in clientes)
            {
                var clienteBd = this.repositorio.Obtener<Proveedor>(p =>
                    p.CUIT == c.CuitCliente &&
                    p.CodigoProveedor == c.CodigoProveedor &&
                    p.RazonSocial.Trim().ToUpper() == (c.RazonSocial.Trim().ToUpper()));
                if (clienteBd == null)
                {
                    var clienteNuevo = new Proveedor
                    {
                        CUIT = c.CuitCliente,
                        RazonSocial = c.RazonSocial,
                        CodigoProveedor = c.CodigoProveedor,
                        Mail = c.CuitCliente + "@altaclientejob.com",
                        EstadoAprobacion = 0,
                        Observaciones = "Carga masiva - " + DateTime.Now.Date,
                    };
                    clientesNuevos.Add(clienteNuevo);
                }
            }
            return clientesNuevos;
        }

        public ValidarChoferResponse ValidarChofer(string cuilChofer, string cuitCliente)
        {
            var esCuilValido = ValidarCuilChoferDigito(cuilChofer);

            return new ValidarChoferResponse
            {
                EsCuilValido = esCuilValido,
            };
        }

        public bool ValidarExistenciaPatente(string patenteChasis, string cuitCliente)
        {
            return OrdenesConPatentesRepetidas(patenteChasis)
                .Any(oc => oc.CUITCliente != cuitCliente);
        }

        public bool ValidarClienteSolicitaAnulacion(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId) ?? throw new Exception("No se encontró Orden de carga " + ordenId);
            var camionEstaEnPlanta = CamionEstaEnPlanta(orden);
            if (camionEstaEnPlanta)
            {
                try
                {
                    emailFasService.EnviarMailSolicitudAnulacionCamionEnPlanta(orden);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error al enviar mail de cliente que intenta anular orden {ordenId} estando el camión en planta", ex);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidarClienteSolicitaEdicion(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId) ?? throw new Exception("No se encontró Orden de carga " + ordenId);
            var camionEstaEnPlanta = CamionEstaEnPlanta(orden);
            if (camionEstaEnPlanta)
            {
                try
                {
                    emailFasService.EnviarMailSolicitudEdicionCamionEnPlanta(orden);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error al enviar mail de cliente que intenta editar orden {ordenId} estando el camión en planta", ex);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private void EnviarMailAltaCuitTerceros(GestionAltasFAS gestionAltas)
        {
            if (gestionAltas.OrdenId is null)
            {
                throw new Exception("No se ha especificado una orden de carga");
            }
            Log.Info($"EnviarMailAltaCuitTerceros params => gestionaFlete: {gestionAltas.GestionaFlete}, " +
                $"gestionaDestino: {gestionAltas.GestionaDestino}, gestionaDestinatario: {gestionAltas.GestionaDestinatario}, ordenId:{gestionAltas.OrdenId}");

            var ordenDeCarga = this.repositorio.Obtener<OrdenDeCarga>(o => o.Id == gestionAltas.OrdenId);

            if (gestionAltas.GestionaFlete)
            {
                emailFasService.EnviarMailAltaIntermediarioFlete(
                    ordenDeCarga.CUITIntermediarioFlete,
                    ordenDeCarga.RazonSocialIntermediarioFlete,
                    ordenDeCarga.Id);
            }

            if (gestionAltas.GestionaDestino || gestionAltas.GestionaDestinatario)
            {
                emailFasService.EnviarMailAltaTempranaCuit(
                    ordenDeCarga, gestionAltas.OrdenId ?? 0,
                    gestionAltas.GestionaDestino,
                    gestionAltas.GestionaDestinatario);
            }
        }

        private List<OrdenDeCarga> OrdenesConPatentesRepetidas(string patenteChasis)
        {
            return repositorio.Listar<OrdenDeCarga>(oc =>
                !estadosParaNoNotificarChasisRepetido.Contains(oc.Estado) &&
                oc.ChasisAcoplado == patenteChasis
            );
        }

        private void LlenarOrdenAltaCorredorCliente(OrdenDeCarga ordenDeCarga, Usuario usuario)
        {
            var esComercial = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaComerciales);
            var esMultifirma = usuario.TienePermiso(PermisoEnum.SeleccionarVendedor);

            Proveedor cliente = null;
            if (esComercial || (esMultifirma && ordenDeCarga.CUITCliente != usuario.CUITRegistro))
            {
                cliente = repositorio.Obtener<Proveedor>(x =>
                    x.CUIT == ordenDeCarga.CUITCliente &&
                    x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                    x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
                if (!esComercial)
                {
                    ordenDeCarga.CodigoCorredor = "";
                    ordenDeCarga.CUITCorredor = "";
                    ordenDeCarga.Corredor_Id = null;
                }

                if (!string.IsNullOrEmpty(ordenDeCarga.CUITCorredor) && esComercial)
                {
                    var corredor = repositorio.Obtener<Proveedor>(x =>
                        x.CUIT == ordenDeCarga.CUITCorredor &&
                        x.EstadoAprobacion == EstadoAprobacion.Aprobado &&
                        x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor)
                    ?? throw new Exception("No se encontró el corredor seleccionado");

                    ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
                    ordenDeCarga.Corredor_Id = corredor.Id;
                }
            }
            else
            {
                if (usuario.EsCorredor())
                {
                    var corredor = usuario.ObtenerCorredor();
                    ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
                    ordenDeCarga.Corredor_Id = corredor.Id;
                    ordenDeCarga.CUITCorredor = corredor.CUIT;

                    cliente = usuario.Proveedores.FirstOrDefault(prov =>
                        prov.CUIT == ordenDeCarga.CUITCliente &&
                        prov.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente)
                    ?? throw new Exception("Su usuario no está habilitado para operar con esa CUIT");
                }
                else
                {
                    cliente = usuario.ObtenerProveedor();
                    ordenDeCarga.CodigoCorredor = "";
                    ordenDeCarga.CUITCorredor = "";
                    ordenDeCarga.Corredor_Id = null;
                }
            }
            ordenDeCarga.Cliente = cliente;
            ordenDeCarga.Cliente_Id = cliente.Id;
            ordenDeCarga.CUITCliente = cliente.CUIT;
        }

        private void ValidarOrdenDeCargaAlta(OrdenDeCarga orden)
        {
            if (orden.PatenteAcoplado == orden.ChasisAcoplado)
            {
                throw new InfoCustomException("Las patentes de chasis y acoplado no pueden ser iguales.");
            }
            ValidarCuilChofer(orden);
        }

        private void ValidarCuilChofer(OrdenDeCarga orden)
        {
            var cuilChofer = orden.CUITChofer;
            var choferRes = scatoRepositorioClient.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer));
            if (!choferRes.IsValid)
            {
                Log.Info("Error al obtener chofer de Scato " + cuilChofer);
                foreach (var err in choferRes.Messages)
                {
                    Log.Info($"Error Scato código {err.MessageCode}, descripción: {err.Message}");
                }
                if (choferRes.Messages.Any(msg => msg.MessageCode == ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido))
                {
                    throw new ValidationCustomException("CUIL de chofer inválido");
                }
                Log.Debug($"Chofer inválido ({cuilChofer}) en Scato: {choferRes.Data.ToJson()}");
            }
        }

        private void LlenarOrdenAlta(OrdenDeCarga ordenDeCarga, Usuario usuario)
        {
            ordenDeCarga.Estado = EstadoOrdenDeCarga.ErrorDeCarga;
            ordenDeCarga.UsuarioCreacion_Id = usuario.Id;
            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.ContratoSinCantidadPendiente = false;
            ordenDeCarga.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;

            var producto = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);
            ordenDeCarga.Producto = producto;

            if (ordenDeCarga.EsFacturaAnticipada)
            {
                ordenDeCarga.AprobadoCredito = true;
                if (!facturaAnticipadaService.OrdenConMultiplesFacturas(ordenDeCarga))
                {
                    ordenDeCarga.NumeroFacturaSeleccionada = ordenDeCarga.NumeroFactura;
                    ordenDeCarga.NumeroPedido = ordenDeCarga.NumeroPedidoIngresado;
                }
            }
            else
            {
                // ¿Esta condición está de más? (Porque, si no es Factura Anticipada, es Normal)
                if (ordenDeCarga.TipoContrato == TipoContratoFAS.Normal || ordenDeCarga.TipoContrato == TipoContratoFAS.CuentaYOrden)
                {
                    ordenDeCarga.NumeroPedido = string.IsNullOrEmpty(ordenDeCarga.NumeroPedidoIngresado) ? "" : ordenDeCarga.NumeroPedidoIngresado;
                }
            }

            var validaCPEDG = producto.ValidaSisaRuca;
            if (!validaCPEDG)
            {
                RemoverCamposCPEDG(ordenDeCarga);
            }
            else
            {
                if (string.IsNullOrEmpty(ordenDeCarga.CUITDestinatario))
                {
                    UsarCUITClienteParaDestinatario(ordenDeCarga);
                }
                ordenDeCarga.DestinoMercaderia = null;
                ordenDeCarga.Reventa = ordenDeCarga.CUITCliente != ordenDeCarga.CUITDestino;
                ordenDeCarga.KmsARecorrer = ObtenerDistanciaARecorrer(ordenDeCarga.DomicilioDescr);
            }
            ordenDeCarga.FechaVencimiento = CalcularFechaVencimiento(DateTime.Now);
        }

        private static void RemoverCamposCPEDG(OrdenDeCarga orden)
        {
            orden.CUITDestinatario = null;
            orden.CUITDestino = null;
            orden.RazonSocialDestinatario = null;
            orden.RazonSocialDestino = null;
            orden.Reventa = false;
            orden.CUITIntermediarioFlete = null;
            orden.DomicilioOrden = null;
        }

        private static void UsarCUITClienteParaDestinatario(OrdenDeCarga orden)
        {
            orden.CUITDestinatario = orden.CUITCliente;
            orden.RazonSocialDestinatario = orden.Cliente.RazonSocial;
        }

        private bool TransporteExiste(OrdenDeCarga orden)
        {
            var result = ordenCargaConsumer.OrdenCargaControlEstadoRequest("", "", orden.CUITTransporte);
            return ResponseConverter.GetOrdenCargaControlEstadoResponse(result) == ControlEstadoResEnum.TransportistaOK;
        }

        private decimal ObtenerKilosDisponiblesContrato(OrdenDeCarga orden, out Result contratoSAP)
        {
            var numeroContrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;

            contratoSAP = ordenCargaConsumer.ObtenerContratoSAP(numeroContrato, null) ?? throw new InfoCustomException("No se encontró el contrato en SAP");

            var ordenesPendientes = ObtenerOrdenesPendientesDeCliente(orden.Cliente.CodigoProveedor).Where(ordenPendiente => ordenPendiente.Id != orden.Id).ToList();

            var kilosDisponibles = kgDisponiblesFasService.ObtenerKgDisponiblesContrato(contratoSAP, ordenesPendientes);

            return kilosDisponibles;
        }

        private static bool KilosAlcanzanParaConfirmarOrden(decimal kilosDisponibles)
        {
            if (kilosDisponibles <= Constante.FAS_KILOS_LIMITE_INFERIOR)
            {
                throw new InfoCustomException($"No hay kilos disponibles para la orden");
            }
            if (kilosDisponibles < Constante.FAS_KILOS_LIMITE_SUPERIOR)
            {
                return false;
            }
            return true;
        }

        private List<OrdenDeCarga> ObtenerOrdenesPendientesDeCliente(string codigoCliente)
        {
            var estadosNoTieneOrdenPendienteEnvio = new List<EstadoOrdenDeCarga>
            {
                EstadoOrdenDeCarga.AnuladaPorVencimiento,
                EstadoOrdenDeCarga.Anulada
            };

            return repositorio.Listar<OrdenDeCarga>(x =>
                x.Cliente.CodigoProveedor == codigoCliente &&
                (
                    (string.IsNullOrEmpty(x.NumeroPedido) && x.TipoContrato == TipoContratoFAS.Normal) ||
                    (string.IsNullOrEmpty(x.NumeroEntrega) && x.TipoContrato == TipoContratoFAS.Anticipado)
                ) &&
                !estadosNoTieneOrdenPendienteEnvio.Contains(x.Estado));
        }

        private bool VerificarOrden(OrdenDeCarga ordenDeCarga, Proveedor cliente, bool esJob)
        {
            Log.Info($"VerificarOrden(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, cliente: {cliente?.Id.ToJson()}, esJob: {esJob})");

            var puedeCrearPedido = true;
            var existeTransporte = true;
            var codigoVerificacionSap = string.Empty;
            var descripcionCodigoVerificacionSap = string.Empty;

            if (!ValidarExistenciaIntermediarioFlete(ordenDeCarga))
            {
                existeTransporte = false;
                descripcionCodigoVerificacionSap = "Intermediario de flete no dado de alta";
            }

            var controlCargaResponse = ControlarCarga(ordenDeCarga, cliente.CodigoProveedor, false);

            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.OK))
            {
                ordenDeCarga.CorredorSeleccionado = true;
                descripcionCodigoVerificacionSap = "OK";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.OK);
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.MasDeUnContratoVigente))
            {
                descripcionCodigoVerificacionSap = "No se encontró ningún contrato con ese producto.";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.MasDeUnContratoVigente);
                puedeCrearPedido = false;
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.TransportistaNoDadoDeAlta))
            {
                existeTransporte = false;
                descripcionCodigoVerificacionSap = "Transportista no dado de alta";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.TransportistaNoDadoDeAlta);
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.VerificarPedido))
            {
                descripcionCodigoVerificacionSap = "El pedido informado no existe.";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.VerificarPedido);
                puedeCrearPedido = false;
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.VerificarCreditoDePedido))
            {
                ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                descripcionCodigoVerificacionSap = "Verificar crédito de pedido";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.VerificarCreditoDePedido);
                puedeCrearPedido = false;
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.PedidoEntregadoCompletamente))
            {
                descripcionCodigoVerificacionSap = "El pedido ingresado ya fue entregado completamente.";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.PedidoEntregadoCompletamente);
                puedeCrearPedido = false;
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.CC06IdemCC01))
            {
                descripcionCodigoVerificacionSap = "Error de carga.";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.CC06IdemCC01);
                puedeCrearPedido = false;
            }
            if (controlCargaResponse.TieneRespuesta(ControlCargaResEnum.FaltaCargarKmsEnContrato))
            {
                descripcionCodigoVerificacionSap = "Faltan cargar los Km en el contrato.";
                codigoVerificacionSap = controlCargaResponse.GetCodigoDeRespuesta(ControlCargaResEnum.FaltaCargarKmsEnContrato);
                puedeCrearPedido = false;
            }

            // Solo en el caso que el response dé ok para crear la orden tiene que verificar el vencimiento
            if (!esJob && puedeCrearPedido && string.IsNullOrEmpty(ordenDeCarga.NumeroEntrega))
            {
                if (!ValidarVencimientoContrato(ObtenerContratoDeOrden(ordenDeCarga), cliente))
                {
                    descripcionCodigoVerificacionSap = "";
                    ordenDeCarga.DescripcionErrorInterno = "El contrato está vencido.";
                    ordenDeCarga.Estado = EstadoOrdenDeCarga.ContratoVencido;
                    puedeCrearPedido = false;
                }
                else
                {
                    ordenDeCarga.Estado = EstadoOrdenDeCarga.SinEnviarASAP;
                }
            }

            ordenDeCarga.TransporteExiste = existeTransporte;
            ordenDeCarga.CodigoVerificacionSap = codigoVerificacionSap;
            ordenDeCarga.DescripcionCodigoVerificacionSap = descripcionCodigoVerificacionSap;

            if (!puedeCrearPedido)
            {
                var logCambioEstado = ordenDeCarga.ActualizarEstado();
                Log.Info(logCambioEstado);
            }
            return puedeCrearPedido;
        }

        private bool ValidarExistenciaIntermediarioFlete(OrdenDeCarga orden)
        {
            var cuilIF = orden.CUITIntermediarioFlete;

            if (string.IsNullOrEmpty(cuilIF))
            {
                return true; // No se ingresó Intermediario en la orden
            }

            var scatoRes = scatoRepositorioClient.ObtenerProveedorPorCuil(cuilIF);
            if (scatoRes.IsValid)
            {
                return true; // Se ingresó Intermediario y existe en Scato
            }
            else
            {
                // El Intermediario no existe o es inválido el cuil
                if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.ProveedorNoEncontrado))
                {
                    return false;
                }
                else
                {
                    throw new Exception("Error al validar existencia Intermediario. Validación inesperada con cuit " + cuilIF);
                }
            }
        }

        private ControlCargaResponseHandler ControlarCarga(OrdenDeCarga ordenDeCarga, string codigoProveedor, bool soloSisa)
        {
            var controlarCargaReq = new ControlCargaRequest
            {
                Cliente = codigoProveedor,
                Contrato = ObtenerContratoDeOrden(ordenDeCarga),
                Corredor = ordenDeCarga.CodigoCorredor,
                Cuit = ordenDeCarga.CUITTransporte,
                CuitDestino = ordenDeCarga.CUITDestino,
                CuitDestinatario = ordenDeCarga.CUITDestinatario,
                Material = ordenDeCarga.Producto.CodigoSap,
                Pedido = ObtenerPedidoDeOrden(ordenDeCarga),
                SoloSisa = soloSisa
            };
            return ordenCargaConsumer.ControlarCarga(controlarCargaReq);
        }

        private static string ObtenerContratoDeOrden(OrdenDeCarga ordenDeCarga)
        {
            if (ordenDeCarga.ContratoIngresado != null)
            {
                var contratos = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                return contratos.Split('|').First();
            }
            else
            {
                return null;
            }
        }

        private static string ObtenerPedidoDeOrden(OrdenDeCarga ordenDeCarga)
        {
            if (ordenDeCarga.EsFacturaAnticipada)
            {
                return !string.IsNullOrEmpty(ordenDeCarga.NumeroPedido) ? ordenDeCarga.NumeroPedido : ordenDeCarga.NumeroPedidoIngresado;
            }
            else
                return ordenDeCarga.NumeroPedido;
        }

        private bool ValidarVencimientoContrato(string contrato, Proveedor cliente)
        {
            var request = new OrdenCargaVisualizarClienteWSMOARequest()
            {
                Cliente = cliente.CodigoProveedor,
                Contrato = contrato,
                TipoContrato = TipoContratoFAS.Todos
            };
            Log.Info($"ValidarVencimientoContrato request: {request.ToJson()}");
            var result = ordenCargaConsumer.OrdenCargaVisualizarClienteExecute(request);
            Log.Info($"ValidarVencimientoContrato result count: {result.Resultados.Count}");

            var fechaContrato = result.Resultados.Select(d => d.FechaHasta).Distinct().FirstOrDefault();
            var fechaHoy = DateTime.Now.Date;
            Log.Info($"ValidarVencimientoContrato : {new { fechaContrato, fechaHoy }.ToJson()}");

            if (result.Resultados.Count == 0)
            {
                Log.Info($"ValidarVencimientoContrato return : true");
                return true;
            }
            if (fechaHoy > Convert.ToDateTime(fechaContrato))
            {
                Log.Info($"ValidarVencimientoContrato return : false");
                return false;
            }

            Log.Info($"ValidarVencimientoContrato return : true");
            return true;
        }

        private bool PedidoTieneKgDisponibles(OrdenDeCarga orden)
        {
            var numeroContrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
            var contratoSAP = ordenCargaConsumer.ObtenerContratoSAP(numeroContrato, null) ?? throw new InfoCustomException("No se encontró el contrato en SAP");
                
            var ordenesPendientes = ObtenerOrdenesPendientesDeCliente(orden.Cliente.CodigoProveedor).Where(ordenPendiente => ordenPendiente.Id != orden.Id).ToList();
            var kilosDisponibles = kgDisponiblesFasService.ObtenerKgDisponiblesPedido(contratoSAP, ordenesPendientes, orden.NumeroPedidoIngresado);
            
            Log.Info($"Validar kg pedido para contrato: {numeroContrato}. Kilos disponibles: {kilosDisponibles}");
            
            if (kilosDisponibles <= Constante.FAS_KILOS_LIMITE_INFERIOR)
            {
                throw new InfoCustomException($"El pedido seleccionado no tiene kg disponibles");
            }
            if (kilosDisponibles < Constante.FAS_KILOS_LIMITE_SUPERIOR)
            {
                return false;
            }

            return true;
        }

        private void NotificarContratoSinKm(OrdenDeCarga orden)
        {
            try
            {
                if (orden.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    emailFasService.EnviarMailContratoSinKm(orden);
                }
            }
            catch (Exception ex)
            {
                Log.Info("Error al enviar notificacion km");
                Log.Error(ex);
            }
        }

        private void NotificarVariasFacturas(OrdenDeCarga ordenDeCarga)
        {
            if (ordenDeCarga.EsFacturaAnticipada && ordenDeCarga.SinSeleccionarFactura && facturaAnticipadaService.OrdenConMultiplesFacturas(ordenDeCarga))
            {
                emailFasService.EnviarMailVariasFacturasPendientes(ordenDeCarga);
            }
        }

        private void NotificarCamionAutorizadoMultiplesOrdenes(OrdenDeCarga ordenDeCarga)
        {

            if (!estadosParaNoNotificarChasisRepetido.Contains(ordenDeCarga.Estado))
            {
                var ordenesConPatentesRepetidas = OrdenesConPatentesRepetidas(ordenDeCarga.ChasisAcoplado);
                if (ordenesConPatentesRepetidas.Any(oc => oc.CUITCliente != ordenDeCarga.CUITCliente))
                {
                    emailFasService.EnviarMailCamionAutorizadoEnVariasOrdenes(ordenDeCarga.ChasisAcoplado, ordenesConPatentesRepetidas
                        .Select(oc => oc.CUITCliente).Distinct().ToList());
                }
            }
        }

        private (OrdenDeCarga, List<Variance>, bool) CargarDatosOCEditar(OrdenDeCarga ordenDeCarga, Usuario usuario)
        {
            var estadosNoPuedeEditarCuitsTerceros = new List<EstadoOrdenDeCarga>
            {
                EstadoOrdenDeCarga.EntregaGenerada,
                EstadoOrdenDeCarga.Entregada,
                EstadoOrdenDeCarga.EntregaPendiente
            };

            var ordenEditar = repositorio.Obtener<OrdenDeCarga>(ordenDeCarga.Id);
            var listaValoresDiferentes = ordenEditar.Compare(ordenDeCarga);
            var product = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);

            ordenEditar.NombreChofer = ordenDeCarga.NombreChofer;
            ordenEditar.CUITChofer = ordenDeCarga.CUITChofer;
            ordenEditar.PatenteAcoplado = ordenDeCarga.PatenteAcoplado;
            ordenEditar.ChasisAcoplado = ordenDeCarga.ChasisAcoplado;
            ordenEditar.RazonSocialTransporte = ordenDeCarga.RazonSocialTransporte;
            ordenEditar.CUITTransporte = ordenDeCarga.CUITTransporte;
            ordenEditar.ContratoIngresado = ordenDeCarga.ContratoIngresado;
            ordenEditar.Cantidad = ordenDeCarga.Cantidad;
            ordenEditar.Producto = product;
            ordenEditar.NumeroPedidoIngresado = ordenDeCarga.NumeroPedidoIngresado;
            ordenEditar.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;
            ordenEditar.Escalable = ordenDeCarga.Escalable;

            var contratoKgDisponibles = ContratoConMas15TN(ordenEditar);
            var validaCPEDG = product.ValidaSisaRuca;

            if (validaCPEDG)
            {
                if (string.IsNullOrEmpty(ordenDeCarga.CUITDestinatario))
                {
                    UsarCUITClienteParaDestinatario(ordenEditar);
                }

                if ((!estadosNoPuedeEditarCuitsTerceros.Contains(ordenEditar.Estado)
                    || (ordenEditar.TipoContrato != TipoContratoFAS.Anticipado && !string.IsNullOrEmpty(ordenEditar.NumeroPedido))))
                {
                    ordenEditar.CUITDestinatario = ordenDeCarga.CUITDestinatario;
                    ordenEditar.CUITDestino = ordenDeCarga.CUITDestino;
                    ordenEditar.RazonSocialDestinatario = ordenDeCarga.RazonSocialDestinatario;
                    ordenEditar.RazonSocialDestino = ordenDeCarga.RazonSocialDestino;
                    ordenEditar.DomicilioDescr = ordenDeCarga.DomicilioDescr;
                    ordenEditar.DomicilioOrden = ordenDeCarga.DomicilioOrden;
                    ordenEditar.DomicilioTipo = ordenDeCarga.DomicilioTipo;
                    ordenEditar.PlantaCodigo = ordenDeCarga.PlantaCodigo;
                }
                ordenEditar.CUITIntermediarioFlete = ordenDeCarga.CUITIntermediarioFlete;
                ordenEditar.RazonSocialIntermediarioFlete = ordenDeCarga.RazonSocialIntermediarioFlete;
            }
            if (!ordenEditar.InformadaSAP || listaValoresDiferentes.Exists(x => x.PropertyName == "ContratoIngresado"))
            {
                var puedeCrearPedido = VerificarOrden(ordenEditar, ordenEditar.Cliente, false);
                if (ordenEditar.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    NotificarContratoSinKm(ordenEditar);
                }
                else
                {
                    var crearPedido = !string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP) || puedeCrearPedido;
                    if (crearPedido && !ordenEditar.EsFacturaAnticipada && contratoKgDisponibles)
                    {
                        if (string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP))
                        {
                            ordenEditar.ContratoSAP = ordenEditar.ContratoIngresado;
                        }
                        var creadaEnSaP = CrearPedidoEnSAP(ordenEditar, ordenEditar.Cliente, true, usuario.Mail);
                        if (creadaEnSaP)
                        {
                            VerificarSituacionCrediticia(ordenEditar, true);
                        }
                    }
                }
            }
            ordenEditar.Observacion = ordenDeCarga.Observacion;
            ordenEditar.TransporteExiste = TransporteExiste(ordenDeCarga);
            return (ordenEditar, listaValoresDiferentes, contratoKgDisponibles);
        }

        private bool ContratoConMas15TN(OrdenDeCarga orden)
        {
            var numeroContrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
            Log.Info($"Validar kg orden 0 a 15tn: patente={orden.PatenteAcoplado}, chasis={orden.ChasisAcoplado}, " +
                $"código cliente={orden.Cliente.CodigoProveedor}, número contrato={numeroContrato}");
            var contratoSAP = ordenCargaConsumer.ObtenerContratoSAP(numeroContrato, null);
            if (contratoSAP == null)
            {
                Log.Info($"ContratoEntre15a30Tn - No se encontró el contrato {numeroContrato} en SAP, ordenId: {orden.Id}");
                return false;
            }
            Log.Info($"Validar kg contrato 0 a 15tn: detalles={contratoSAP.Detalles} ");
            var ordenesPendientes = ObtenerOrdenesPendientesDeCliente(orden.Cliente.CodigoProveedor).Where(ordenPendiente => ordenPendiente.Id != orden.Id).ToList();
            var kilosDisponibles = kgDisponiblesFasService.ObtenerKgDisponiblesContrato(contratoSAP, ordenesPendientes);

            return kilosDisponibles >= Constante.FAS_KILOS_LIMITE_SUPERIOR;
        }

        private bool CrearPedidoEnSAP(OrdenDeCarga ordenDeCarga, Proveedor cliente, bool validaKg, string mailUsuario)
        {
            Log.Info($"CrearPedidoEnSAP(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, cliente: {cliente?.Id.ToJson()}, validaKg: {validaKg})");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var usuarioSapNombre = usuario?.UsuarioSap;
            if (string.IsNullOrEmpty(usuarioSapNombre))
            {
                var usuarioAutomatico = repositorio.Obtener<Usuario>(u => u.Mail == _usuarioAutomaticoSAP);
                usuarioSapNombre = usuarioAutomatico.UsuarioSap;
            }

            var crearOrdenReq = new CrearOrdenRequest
            {
                Cliente = cliente.CodigoProveedor,
                CuitCliente = cliente.CUIT,
                Contrato = ObtenerContratoDeOrden(ordenDeCarga),
                Corredor = ordenDeCarga.CodigoCorredor,
                Kilos = ordenDeCarga.Cantidad,
                Material = ordenDeCarga.Producto.CodigoSap,
                PedidoInput = ordenDeCarga.NumeroPedidoIngresado,
                UsuarioSAP = usuarioSapNombre,
                ValidaKg = validaKg,
                CuitDestino = ordenDeCarga.CUITDestino,
                CuitDestinatario = ordenDeCarga.CUITDestinatario,
                RazonSocialDestino = ordenDeCarga.RazonSocialDestino,
                RazonSocialDestinatario = ordenDeCarga.RazonSocialDestinatario,
                Reventa = ordenDeCarga.Reventa,
                PlantaCodigo = ordenDeCarga.PlantaCodigo,
                DomicilioDescr = ordenDeCarga.DomicilioDescr,
                DomicilioOrden = ordenDeCarga.DomicilioOrden,
                DomicilioTipo = ordenDeCarga.DomicilioTipo
            };

            var result = ordenCargaConsumer.CrearOrden(crearOrdenReq, out string numeroPedido, out string rawResult);

            var resultadoCrearOrden = false;
            ordenDeCarga.ContratoSinCantidadPendiente = false;
            ordenDeCarga.CodigoVerificacionSap = OrdenCargaCrearOrdenClass.GetCodigo(result);

            if (result == CrearOrdenResEnum.PedidoCreado || result == CrearOrdenResEnum.PedidoCreadoVerificarCredito)
            {
                ordenDeCarga.InformadaSAP = true;
                ordenDeCarga.NumeroPedido = numeroPedido;
                ordenDeCarga.ContratoSAP = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                ordenDeCarga.DescripcionErrorInterno = "";
                ordenDeCarga.DescripcionCodigoVerificacionSap = "";
                ordenDeCarga.CodigoVerificacionSap = "";
                resultadoCrearOrden = true;
            }
            if (result == CrearOrdenResEnum.VerificarCantidadPendiente)
            {
                ordenDeCarga.CodigoVerificacionSap = ResponseConverter.GetCodigoControlCarga(ControlCargaResEnum.MasDeUnContratoVigente);
                ordenDeCarga.ContratoSinCantidadPendiente = true;
                ordenDeCarga.DescripcionErrorInterno = "El contrato ingresado tiene menos de 15 toneladas disponibles. Puede elegir forzar la creación del pedido desde \"Crear pedido\" o anularlo.";
            }
            else if (result == CrearOrdenResEnum.ContratoSinKg)
            {
                ordenDeCarga.ContratoSinCantidadPendiente = true;
                ordenDeCarga.DescripcionErrorInterno = "El contrato ingresado no tiene kilogramos disponibles.";

            }
            else if (result == CrearOrdenResEnum.VerificarDatos)
            {
                ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningún contrato con ese producto.";
            }
            else if (result == CrearOrdenResEnum.Vacia)
            {
                ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningún contrato con ese producto.";
            }
            else if (result == CrearOrdenResEnum.NoEsperado)
            {
                ordenDeCarga.DescripcionCodigoVerificacionSap = "Respuesta inesperada: " + rawResult;
            }
            var logCrearOrden = ordenDeCarga.ActualizarEstado();
            Log.Info("CrearOrdenEnSAP. " + logCrearOrden);
            repositorio.GuardarCambios();
            return resultadoCrearOrden;
        }

        private Resultado VerificarSituacionCrediticia(OrdenDeCarga orden, bool notificar)
        {
            if (orden.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito || !orden.AprobadoCredito)
            {
                orden.AprobadoCredito = ObtenerSituacionCrediticia(orden);

                if (!orden.AprobadoCredito)
                {
                    if (notificar)
                    {
                        emailFasService.EnviarMailValidacionesCrediticias(orden);
                    }

                    var logActEstVerifCred = orden.ActualizarEstado();
                    Log.Info("VerificarSituacionCrediticia. " + logActEstVerifCred);
                    if (notificar)
                    {
                        return new Resultado { Mensaje = "Verifique el crédito del pedido" };
                    }
                    else
                    {
                        return new Resultado { info = "Verifique el crédito del pedido." };
                    }
                }
                else
                {
                    orden.DescripcionErrorInterno = "";
                    var logEstVerifCred = orden.ActualizarEstado();
                    Log.Info("VerificarSituacionCrediticia. " + logEstVerifCred);
                    return GenerarEntregaSAP(orden);
                }
            }
            else
            {
                return new Resultado { IdEntidad = orden.Id, Mensaje = "La orden no está pendiente de aprobación de crédito." };
            }
        }

        private bool ObtenerSituacionCrediticia(OrdenDeCarga orden)
        {
            if (orden.EsFacturaAnticipada)
            {
                return true;
            }
            var numeroPedido = orden.NumeroPedido;

            Log.Info("ObtenerSituacionCrediticia");
            var result = ordenCargaConsumer.OrdenCargaControlEstadoRequest("", numeroPedido, "");

            return result == "CE-00";
        }

        private Resultado GenerarEntregaSAP(OrdenDeCarga orden)
        {
            Log.Info("Ejecuta OrdenDeCargaService.GenerarEntregaSAP");

            if (string.IsNullOrEmpty(orden.NumeroPedido) && orden.EsFacturaAnticipada)
            {
                orden.Estado = EstadoOrdenDeCarga.Pendiente;
                orden.DescripcionErrorInterno = "Orden con pedido entre 0 a 15Tn";
                return new Resultado { Mensaje = "No se pudo generar la entrega. No se ha seleccionado una factura." };
            }
            if (!ValidarExistenciaIntermediarioFlete(orden))
            {
                orden.TransporteExiste = false;
                orden.DescripcionCodigoVerificacionSap = "No se pudo generar la entrega. No existe el Intermediario de flete.";
                orden.Estado = EstadoOrdenDeCarga.EntregaPendiente;
                repositorio.GuardarCambios();
                return new Resultado { Mensaje = "No se pudo generar la entrega. No existe el Intermediario de flete." };
            }
            if (orden.Producto.ValidaSisaRuca && !orden.CuitTerceroExisteScato)
            {
                ValidarExistenciaCuitsTerceros(orden);
                if (!orden.CuitTerceroExisteScato)
                {
                    return ResultadoCuitsTercerosNoExisten(orden);
                }
            }
            var numeroFactura = string.IsNullOrEmpty(orden.NumeroFacturaSeleccionada) ? orden.NumeroFactura : orden.NumeroFacturaSeleccionada;
            Log.Info($"GenerarEntregaSAP: numeroFactura -> {numeroFactura}");

            var req = new CrearEntregaRequest
            {
                Documento = orden.CUITChofer,
                Kilos = orden.Cantidad,
                NombreConductor = orden.NombreChofer,
                PatenteAcoplado = orden.PatenteAcoplado,
                PatenteChasis = orden.ChasisAcoplado,
                Pedido = orden.NumeroPedido,
                TipoDocumento = "CUIL",
                Transportista = orden.CUITTransporte,
                CuitDestinatario = orden.CUITDestinatario,
                RazonSocialDestinatario = orden.RazonSocialDestinatario,
                CuitDestino = orden.CUITDestino,
                RazonSocialDestino = orden.RazonSocialDestino,
                Reventa = orden.Reventa,
                TransportistaReal = orden.CUITIntermediarioFlete,
                PlantaCodigo = orden.PlantaCodigo,
                DomicilioTipo = orden.DomicilioTipo,
                DomicilioOrden = orden.DomicilioOrden,
                DomicilioDescr = orden.DomicilioDescr,
                DestinoMercaderia = orden.DestinoMercaderia
            };

            var respHandler = ordenCargaConsumer.CrearEntrega(req, !string.IsNullOrEmpty(numeroFactura));

            switch (respHandler.GetResultado())
            {
                case CrearEntregaResEnum.OK:
                case CrearEntregaResEnum.EntregaCreadaErrorAlInsertarOE02:
                case CrearEntregaResEnum.EntregaCreadaErrorAlInsertarOE03:
                    var numeroEntrega = respHandler.GetNumeroEntrega();
                    orden.TransporteExiste = true;
                    orden.FechaEntregaGenerada = DateTime.Now;
                    orden.NumeroEntrega = numeroEntrega;
                    orden.DescripcionCodigoVerificacionSap = "";
                    var logEstadoEntGen = orden.ActualizarEstado();
                    Log.Info(logEstadoEntGen);
                    repositorio.GuardarCambios();
                    return new Resultado { Mensaje = $"Se ha generado la entrega {numeroEntrega}." };

                case CrearEntregaResEnum.NoExisteTransportista:
                    orden.TransporteExiste = false;
                    orden.DescripcionCodigoVerificacionSap = "No se pudo generar la entrega. No existe el transportista.";
                    var logEstadoNoTransp = orden.ActualizarEstado();
                    Log.Info(logEstadoNoTransp);
                    repositorio.GuardarCambios();
                    return new Resultado { Mensaje = "No se pudo generar la entrega. No existe el transportista." };

                case CrearEntregaResEnum.FaltaCargarKmEnContrato:
                    orden.DescripcionCodigoVerificacionSap = "Falta cargar los Kms en el contrato";
                    repositorio.GuardarCambios();
                    return new Resultado { info = "No se pudo generar la entrega. Falta cargar los Kms en el contrato." };

                case CrearEntregaResEnum.FacturaNoCompensada:
                    orden.TransporteExiste = true;
                    orden.DescripcionCodigoVerificacionSap = "No se pudo generar la entrega. Factura no compensada.";
                    orden.Estado = EstadoOrdenDeCarga.PendienteCompensacion;
                    repositorio.GuardarCambios();
                    return new Resultado { Mensaje = "No se pudo generar la entrega. Factura no compensada." };

                case CrearEntregaResEnum.ErrorRespuestaInesperadaDeSap:
                    orden.DescripcionCodigoVerificacionSap = $"No se pudo generar la entrega. Respuesta inesperada de SAP ({respHandler.GetLogRespuestaSap()})";
                    repositorio.GuardarCambios();
                    return new Resultado { info = "No se pudo generar la entrega. Respuesta inesperada de SAP." };

                default:
                    throw new Exception("Respuesta SAP no manejada");
            }
        }

        private void ValidarExistenciaCuitsTerceros(OrdenDeCarga orden)
        {
            if (!(orden.DestinatarioExisteScato ?? false))
            {
                var validacionDestinatario = ValidarCuitExisteScato(orden.CUITDestinatario);
                orden.DestinatarioExisteScato = validacionDestinatario.Existe;
            }
            if (!(orden.DestinoExisteScato ?? false))
            {
                var validacionDestino = ValidarCuitExisteScato(orden.CUITDestino);
                orden.DestinoExisteScato = validacionDestino.Existe;
            }
            repositorio.GuardarCambios();
        }

        private Resultado ResultadoCuitsTercerosNoExisten(OrdenDeCarga orden)
        {
            var msg = orden.MsgCuitsTerceros;
            orden.DescripcionCodigoVerificacionSap = msg;
            orden.Estado = EstadoOrdenDeCarga.EntregaPendiente;
            repositorio.GuardarCambios();
            return new Resultado { Mensaje = msg };
        }

        private static List<EstadoOrdenDeCarga> ObtenerFiltroEstadosParaUsuario(Usuario usuario, bool esInterno)
        {
            if (esInterno)
            {
                var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodasOrdenesDeCarga);
                var esComercial = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaComerciales);
                var esMesaFas = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaMesaFas);
                var esPuerto = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaPuerto);

                var estadosListarInternos = new List<EstadoOrdenDeCarga>();

                if (esMesaFas)
                {
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Confirmado);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.PendienteAprobacionCredito);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaPendiente);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Entregada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EdicionRechazada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.ContratoVencido);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Vencida);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.SinEnviarASAP);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.PendienteCompensacion);
                }
                if (esComercial)
                {
                    estadosListarInternos.Add(EstadoOrdenDeCarga.ErrorDeCarga);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.AnuladaPorVencimiento);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Pendiente);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Vencida);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Anulada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Entregada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.ContratoVencido);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EdicionRechazada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.SinEnviarASAP);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.PendienteCompensacion);
                }
                if (esPuerto)
                {
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Entregada);
                }
                if (esAdmin)
                {
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Pendiente);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Confirmado);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.PendienteAprobacionCredito);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Anulada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Entregada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.Vencida);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaPendiente);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.AnuladaPorVencimiento);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.ErrorDeCarga);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.ContratoVencido);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EdicionRechazada);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.SinEnviarASAP);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
                    estadosListarInternos.Add(EstadoOrdenDeCarga.PendienteCompensacion);
                }
                return estadosListarInternos;
            }
            else
            {
                var estadosListarNoInternos = new List<EstadoOrdenDeCarga>
                {
                    EstadoOrdenDeCarga.Vencida,
                    EstadoOrdenDeCarga.ErrorDeCarga,
                    EstadoOrdenDeCarga.Pendiente,
                    EstadoOrdenDeCarga.Confirmado,
                    EstadoOrdenDeCarga.PendienteAprobacionCredito,
                    EstadoOrdenDeCarga.EntregaPendiente,
                    EstadoOrdenDeCarga.EntregaGenerada,
                    EstadoOrdenDeCarga.ContratoVencido,
                    EstadoOrdenDeCarga.EdicionRechazada,
                    EstadoOrdenDeCarga.SinEnviarASAP,
                    EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion,
                    EstadoOrdenDeCarga.PendienteCompensacion
                };
                return estadosListarNoInternos;
            }
        }

        private Hashtable ObtenerHashPatentesCargadas()
        {
            var diasPreviosParaCompararPatentes = -4;
            var hoy = DateTime.Now;
            var fechaTope = hoy.AddDays(diasPreviosParaCompararPatentes);

            return ObtenerHashPatentesCargadas(
                repositorio.ListarConsultable<OrdenDeCarga>(oc =>
                    oc.FechaCarga <= hoy &&
                    DbFunctions.TruncateTime(oc.FechaCarga) >= fechaTope)
                .AsEnumerable());
        }

        private Hashtable ObtenerHashPatentesCargadas(IEnumerable<OrdenDeCarga> ordenes)
        {
            var hashPatentesCargadas = new Hashtable();
            ordenes
                .Where(oc =>
                    !estadosParaNoNotificarChasisRepetido.Contains(oc.Estado)
                )
                .ToList()
                .ForEach(oc =>
                {
                    if (hashPatentesCargadas.ContainsKey(oc.ChasisAcoplado))
                    {
                        var ordenesCargadas = (List<(int, string)>)hashPatentesCargadas[oc.ChasisAcoplado];
                        ordenesCargadas.Add((oc.Id, oc.CUITCliente));
                        hashPatentesCargadas[oc.ChasisAcoplado] = ordenesCargadas;
                    }
                    else
                    {
                        hashPatentesCargadas.Add(oc.ChasisAcoplado, new List<(int, string)> { (oc.Id, oc.CUITCliente) });
                    }

                });
            return hashPatentesCargadas;
        }

        private static bool VerificarOrdenConPatentesRepetidas(OrdenDeCarga orden, Hashtable hashPatentesCargadas)
        {
            if (hashPatentesCargadas.ContainsKey(orden.ChasisAcoplado))
            {
                var ordenesCargadas = (List<(int, string)>)hashPatentesCargadas[orden.ChasisAcoplado];
                return ordenesCargadas.Count > 1;
            }
            return false;
        }

        private bool VerificarChasisConMultiplesAutorizaciones(OrdenDeCarga orden, Hashtable hashPatentesCargadas)
        {
            if (estadosParaNoNotificarChasisRepetido.Contains(orden.Estado))
            {
                return false;
            }

            if (hashPatentesCargadas.ContainsKey(orden.ChasisAcoplado))
            {
                var chasisAutorizados = (List<(int, string)>)hashPatentesCargadas[orden.ChasisAcoplado];
                return chasisAutorizados.Any(data => data.Item2 != orden.CUITCliente);
            }
            return false;
        }

        private List<OrdenDeCargaCambiosHistorialDto> ObtenerCambiosHistorial(OrdenDeCarga orden)
        {
            return repositorio.Listar<OrdenDeCargaCambiosHistorial>
                    (ordenes => ordenes.OrdenDeCarga_Id == orden.Id).Select(x => new OrdenDeCargaCambiosHistorialDto
                    {
                        Id = x.Id,
                        Antes = x.Antes,
                        Despues = x.Despues,
                        FechaCambio = Convert.ToDateTime(x.FechaCambio).ToString("dd/MM/yyyy HH:mm"),
                        NombreColumnaCambio = x.NombreColumnaCambio,
                        OrdenDeCarga_Id = x.OrdenDeCarga_Id,
                        Usuario = x.Usuario.Mail
                    }).ToList();
        }

        private List<int> ObtenerOrdenesConPatentesRepetidas(OrdenDeCarga orden)
        {
            var hashPatentesCargadas = ObtenerHashPatentesCargadas();
            if (!VerificarOrdenConPatentesRepetidas(orden, hashPatentesCargadas))
            {
                return null;
            }
            var ordenesConPatentesRepetidas = (List<(int, string)>)hashPatentesCargadas[orden.ChasisAcoplado];
            return ordenesConPatentesRepetidas.Where(data => data.Item1 != orden.Id)
                .Select(data => data.Item1)
                .ToList();
        }

        private void AnularOrdenSap(OrdenDeCarga orden)
        {
            var tieneNumeroEntrega = !string.IsNullOrEmpty(orden.NumeroEntrega);
            if (tieneNumeroEntrega)
            {
                AnularEntregaEnSap(orden);
            }
            if (!orden.EsFacturaAnticipada)
            {
                AnularPedidoEnSap(orden);
            }
        }

        private void AnularEntregaEnSap(OrdenDeCarga orden)
        {
            var respHandler = ordenCargaConsumer.AnularEntregaOrdenCarga(orden.NumeroEntrega);
            if (respHandler.EntregaTomadaEnSap)
            {
                //Pendiente revisión de los mensajes acorde a las verdaderas razones de error
                throw new InfoCustomException("La entrega está tomada en SAP");
            }
            if (respHandler.ActualizadoOK || respHandler.EntregaAnulada)
            {
                orden.Estado = EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion;
                orden.NumeroEntrega = null;
                orden.FechaEntregaGenerada = null;
                repositorio.GuardarCambios();
                Thread.Sleep(5000);
            }
            else
            {
                throw new Exception("No se reconoce respuesta SAP (Anular Entrega)");
            }
        }

        private void AnularPedidoEnSap(OrdenDeCarga orden)
        {
            var tieneNumeroPedido = !string.IsNullOrEmpty(orden.NumeroPedidoIngresado) || !string.IsNullOrEmpty(orden.NumeroPedido);
            if (tieneNumeroPedido)
            {
                var respHandler = ordenCargaConsumer.AnularOrdenCarga(orden);
                if (respHandler.PedidoTomadoEnSap)
                {
                    throw new InfoCustomException("El pedido está tomado en SAP");
                }
                if (!(respHandler.ActualizadoOK || respHandler.PedidoAnulado))
                {
                    throw new Exception("No se reconoce respuesta SAP (Anular Orden Carga)");
                }
            }
        }

        private OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarCliente(string cliente, string contrato, string corredor,
            string fechaInicio, string fechaFin, string material, bool pendiente, TipoContratoFAS tipoContrato, int type)
        {
            try
            {
                List<Mod.FechaWS> fechas = null;
                var ordenCargaConsumerMOA = new OrdenCargaConsumerMOA();
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
                {
                    fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                }
                var request = new OrdenCargaVisualizarClienteWSMOARequest()
                {
                    Cliente = cliente,
                    Contrato = contrato,
                    Corredor = corredor,
                    Fechas = fechas,
                    Material = material,
                    Pendiente = pendiente,
                    TipoContrato = tipoContrato,
                };
                Log.Info("OrdenCargaVisualizarCliente request " + request.ToJson());
                var ordenCargaVisualizarClienteWSMOAResponse = ordenCargaConsumerMOA.OrdenCargaVisualizarClienteExecute(request);

                if (ordenCargaVisualizarClienteWSMOAResponse == null)
                {
                    throw new ValidationCustomException(ErrorMsg.Error);
                }
                if (ordenCargaVisualizarClienteWSMOAResponse.Resultados == null || ordenCargaVisualizarClienteWSMOAResponse.Resultados.Count == 0)
                {
                    if (type == 1)
                    {
                        throw new ValidationCustomException("No se encontraron clientes para dicho corredor");
                    }
                    if (type == 2)
                    {
                        // buscar el nombre del cliente
                        var razonSocial = repositorio.Obtener<Proveedor, string>(a => a.CodigoProveedor == cliente, a => a.RazonSocial);
                        throw new ValidationCustomException($"El contrato {contrato} no corresponde al cliente {(string.IsNullOrEmpty(razonSocial) ? "" : razonSocial)}");
                    }
                }
                return ordenCargaVisualizarClienteWSMOAResponse;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private List<ProveedorDto> GetClientesFromVisualizarClienteProducto(OrdenCargaVisualizarClienteWSMOAResponse ordenCargaVisualizarClienteWSMOAResponse, VisualizarClienteRequest request)
        {
            var clientesDto = new List<ProveedorDto>();
            var clientesWS = ordenCargaVisualizarClienteWSMOAResponse.Resultados.Select(d => d.Cliente).Distinct().ToList();
            if (clientesWS != null && clientesWS.Count > 0)
            {
                var clientesBD = repositorio.Listar<Proveedor>()
                                            .Where(w => clientesWS.Contains(w.CodigoProveedor))
                                            .ToList();

                foreach (var clienteEnSap in ordenCargaVisualizarClienteWSMOAResponse.Resultados.GroupBy(x => x.Cliente).Select(g => g.First()))
                {
                    var proveedor = clientesBD.FirstOrDefault(x => x.CodigoProveedor == clienteEnSap.Cliente);
                    if (proveedor != null)
                    {
                        clientesDto.Add(new ProveedorDto(proveedor));
                    }
                    else
                    {
                        clientesDto.Add(new ProveedorDto
                        {
                            CodigoProveedor = clienteEnSap.Cliente,
                            CUIT = "",
                            RazonSocial = clienteEnSap.NombreCliente
                        });
                    }
                }
            }
            return clientesDto;
        }

        private void SincronizarRelacionesCorredorCliente(string codigoCorredor, VisualizarClienteResponse responseSap)
        {
            var corredor = repositorio.Obtener<Proveedor>(
                            cor => cor.CodigoProveedor == codigoCorredor && cor.EstadoAprobacion == EstadoAprobacion.Aprobado);
            foreach (var clienteDto in responseSap.Clientes.Where(c => c.Id > 0))
            {
                var cliente = repositorio.Obtener<Proveedor>(clienteDto.Id);
                CrearRelacionCorredorCliente(corredor, cliente);
            }
        }

        private void CrearRelacionCorredorCliente(Proveedor corredor, Proveedor cliente)
        {
            Log.Info($"CrearRelacionCorredorCliente(corredor: {corredor?.Id.ToJson()}, cliente: {cliente?.Id.ToJson()})");
            try
            {
                var usuariosCorredores = repositorio.Listar<Usuario>(q => q.CUITRegistro == corredor.CUIT && q.TipoUsuario.Id == (int)TipoUsuarioEnum.Corredor && q.Habilitado == true);
                if (usuariosCorredores != null && usuariosCorredores.Count > 0)
                {
                    foreach (var usuarioCorredor in usuariosCorredores)
                    {
                        var existProveedor = usuarioCorredor.Proveedores.Any(q => q.Id == cliente.Id);
                        if (!existProveedor)
                        {
                            usuarioCorredor.Proveedores.Add(cliente);
                            repositorio.GuardarCambios();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        private List<MaterialDto> GetProductosFromVisualizarClienteProducto(OrdenCargaVisualizarClienteWSMOAResponse ordenCargaVisualizarClienteWSMOAResponse)
        {
            var productosDto = new List<MaterialDto>();
            var productosWS = ordenCargaVisualizarClienteWSMOAResponse.Resultados.Select(d => d.Producto).Distinct().ToList();
            if (productosWS != null && productosWS.Count > 0)
            {
                for (int i = 0; i < productosWS.Count; i++)
                {
                    var producto = productosWS[i].Trim().TrimStart('0');
                    productosWS[i] = producto;
                }
                var productosBD = repositorio.Listar<Material>()
                                            .Where(w => productosWS.Contains(w.CodigoSap))
                                            .ToList();
                productosDto = productosBD.Select(prod => new MaterialDto
                {
                    MaterialId = prod.Id,
                    Descripcion = prod.Nombre,
                    CodigoSap = prod.CodigoSap
                }).ToList();
            }
            return productosDto;
        }

        private static bool GetResultFromValidarCorredorClienteContratoProducto(OrdenCargaVisualizarClienteWSMOAResponse ordenCargaVisualizarClienteWSMOAResponse, ValidarCorredorClienteContratoProductoRequest request, Material producto)
        {
            var result = false;
            if (ordenCargaVisualizarClienteWSMOAResponse.Resultados != null && ordenCargaVisualizarClienteWSMOAResponse.Resultados.Count > 0)
            {
                var res = ordenCargaVisualizarClienteWSMOAResponse.Resultados[0];
                if (!request.ClienteCuit.Equals(string.Empty) && !request.Contrato.Equals(string.Empty) && !request.Corredor.Equals(string.Empty) && !request.ProductoId.Equals(string.Empty))
                {
                    var clienteResult = res.Cliente.ToUpper().TrimStart(new Char[] { '0' });
                    var contratoResult = res.Contrato.ToUpper().TrimStart(new Char[] { '0' });
                    var corredorResult = res.Corredor.ToUpper().TrimStart(new Char[] { '0' });
                    var productoResult = res.Producto.ToUpper().Substring(13, 5);
                    if (request.Corredor.ToUpper().TrimStart(new Char[] { '0' }).Equals(corredorResult)
                        && request.Contrato.ToUpper().TrimStart(new Char[] { '0' }).Equals(contratoResult)
                        && request.ClienteCodigo.ToUpper().TrimStart(new Char[] { '0' }).Equals(clienteResult)
                        && producto.CodigoSap.ToUpper().Equals(productoResult))
                    {
                        result = true;
                    }
                }
                else
                {
                    if (!request.ClienteCodigo.Equals(string.Empty) &&
                        !request.Contrato.Equals(string.Empty) &&
                        !request.ProductoId.Equals(string.Empty))
                    {
                        var clienteResult = res.Cliente.ToUpper().TrimStart(new Char[] { '0' });
                        var contratoResult = res.Contrato.ToUpper().TrimStart(new Char[] { '0' });
                        var productoResult = res.Producto.ToUpper().TrimStart(new Char[] { '0' });
                        if (request.Contrato.Trim().ToUpper().TrimStart(new Char[] { '0' }).Equals(contratoResult.Trim())
                            && request.ClienteCodigo.Trim().ToUpper().TrimStart(new Char[] { '0' }).Equals(clienteResult.Trim())
                            && producto.CodigoSap.Trim().ToUpper().TrimStart(new Char[] { '0' }).Equals(productoResult.Trim()))
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        private string VerificarTransporte(OrdenDeCarga orden, bool crearPedido = false, string mailUsuario = null)
        {
            orden.TransporteExiste = TransporteExiste(orden);
            VerificarOrden(orden, orden.Cliente, true);

            if (orden.TransporteExiste)
            {
                Log.Info("VerificarTransporte ActualizarEstado " + orden.ToDto().ToJson());
                if (!string.IsNullOrEmpty(orden.ContratoSAP) && string.IsNullOrEmpty(orden.NumeroPedido) && crearPedido && !orden.EsFacturaAnticipada)
                {
                    CrearPedidoEnSAP(orden, orden.Cliente, true, mailUsuario);
                }
                var aprobadoCredito = orden.AprobadoCredito || ObtenerSituacionCrediticia(orden);
                if (aprobadoCredito && string.IsNullOrEmpty(orden.NumeroEntrega) && !string.IsNullOrEmpty(orden.NumeroPedido))
                {
                    GenerarEntregaSAP(orden);
                }
                var logCambioEstado = orden.ActualizarEstado();
                Log.Info("VerificarTransporte " + logCambioEstado);
                repositorio.GuardarCambios();
                return SuccessMsg.OrdenDeCargaActualizada;
            }
            else
            {
                return _transporteNoExiste;
            }
        }

        private void VerificarEstadoEntrega(OrdenDeCarga orden)
        {
            Log.Info("VerificarEstadoEntrega");
            var result = ordenCargaConsumer.OrdenCargaControlEstadoRequest(orden.NumeroEntrega, "", "");

            if (result == "CE-06")
            {
                orden.Estado = EstadoOrdenDeCarga.Entregada;
                repositorio.GuardarCambios();
            }
        }

        private string ObtenerMaterialValidaSisa()
        {
            var material = repositorio.Obtener<Material>(m => m.ValidaSisaRuca && m.TablaSeccionMaterial == TablaSeccionMaterial.OrdenDeCarga);
            return material?.CodigoSap;
        }

        private void VerificarCompensacion(OrdenDeCarga orden)
        {
            if (orden == null)
                throw new InfoCustomException("No se encontró la orden");

            Log.Info($"Verificar Compensacion: orden: {orden.ToDto().ToJson()}");
            if (orden.Estado != EstadoOrdenDeCarga.PendienteCompensacion)
                return;

            GenerarEntregaSAP(orden);
        }

        private static bool EsUsuarioInterno(Usuario usuario)
        {
            var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodasOrdenesDeCarga);
            var esComercial = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaComerciales);
            var esMesaFas = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaMesaFas);
            var esPuerto = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaPuerto);

            var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);

            return esInterno;
        }

        private Proveedor ObtenerProveedorSeleccionado(Usuario usuario, int? idProveedorSeleccionado = null)
        {
            var proveedor = usuario.ObtenerProveedorAsignado() ?? usuario.ObtenerProveedor();
            if (idProveedorSeleccionado != null && idProveedorSeleccionado != proveedor.Id)
            {
                proveedor = repositorio.Obtener<Proveedor>(idProveedorSeleccionado);
                if (!usuario.EsAdmin() && !usuario.TienePermiso(PermisoEnum.ElegirTodosVendedores) && !usuario.TieneProveedor(proveedor.CodigoProveedor))
                {
                    throw new ValidationCustomException("Proveedor incorrecto");
                }
            }
            return proveedor;
        }

        private static void LlenarOrdenDeCargaFleteMOA(OrdenDeCarga orden, Result contratoSAP)
        {
            orden.FleteMOA = contratoSAP.PrecioFlete > 0;
        }

        private bool CamionEstaEnPlanta(OrdenDeCarga orden)
        {
            if (string.IsNullOrEmpty(orden.NumeroEntrega))
            {
                return false;
            }
            var recorridosScato = scatoConsumer.ObtenerRecorridoNoRechazadoPorNumeroDocumento(orden.NumeroEntrega);
            return recorridosScato != null && recorridosScato.Any();
        }
    }
}
