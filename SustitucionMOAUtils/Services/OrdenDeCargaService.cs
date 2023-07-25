using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using Mod = SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using ScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Validadores.OrdenDeCarga;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using Org.BouncyCastle.Ocsp;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaService : IOrdenDeCargaService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IOrdenCargaConsumerMOA consumer;
        protected readonly IScatoConsumer scatoConsumer;
        readonly FeriadoService _feriadoService = new FeriadoService();
        protected readonly IFeriadoService feriadoService;
        protected readonly IEmailFasService emailFasService;
        protected readonly IScatoRepositorioClient scatoRepositorioClient;
        protected readonly IFacturaAnticipadaService _facturaAnticipadaService;
        protected readonly IKgDisponiblesFasService _kgDisponiblesFasService;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AvisoEdicionOrdenDeCarga.html");
        private static readonly string EMAIL_TEMPLATE_ORDENES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesDeCarga.html");
        private readonly string _usuarioAutomaticoSAP;

        private readonly string _errorAnulacion = "Error al anular orden de carga, pero la entrega si ha sido anulada";
        private readonly string _entregaEstadoPendiente = "La entrega sigue pendiente.";
        private readonly string _transporteNoExiste = "El transporte no existe";

        private readonly List<EstadoOrdenDeCarga> _estadosListarNoInternos = new List<EstadoOrdenDeCarga>
        {
            EstadoOrdenDeCarga.Vencida,
            EstadoOrdenDeCarga.ErrorDeCarga,
            EstadoOrdenDeCarga.Pendiente,
            EstadoOrdenDeCarga.Confirmado,
            EstadoOrdenDeCarga.PendienteAprobacionCredito,
            EstadoOrdenDeCarga.EntregaPendiente,
            EstadoOrdenDeCarga.EntregaGenerada,
            EstadoOrdenDeCarga.EdicionSolicitada,
            EstadoOrdenDeCarga.AnulacionSolicitada,
            EstadoOrdenDeCarga.ContratoVencido,
            EstadoOrdenDeCarga.EdicionRechazada,
            EstadoOrdenDeCarga.SinEnviarASAP,
            EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion,
            EstadoOrdenDeCarga.PendienteCompensacion,
        };
        private readonly List<EstadoOrdenDeCarga> estadosNoTieneOrdenPendienteEnvio = new List<EstadoOrdenDeCarga> {
            EstadoOrdenDeCarga.AnuladaPorVencimiento,
            EstadoOrdenDeCarga.Anulada
        };

        public OrdenDeCargaService(
            IRepositorio repositorio,
            IOrdenCargaConsumerMOA consumer,
            IFeriadoService feriadoService,
            IScatoRepositorioClient scatoRepositorioClient,
            IScatoConsumer scatoConsumer,
            IEmailFasService emailFasService,
            IFacturaAnticipadaService facturaAnticipadaService,
            IKgDisponiblesFasService kgDisponiblesFasService
            )
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
            this.feriadoService = feriadoService;
            _usuarioAutomaticoSAP = ConfigurationManager.AppSettings["UsuarioAutomaticoSAP"];
            this.scatoRepositorioClient = scatoRepositorioClient;
            this.scatoConsumer = scatoConsumer;
            this.emailFasService = emailFasService;
            _facturaAnticipadaService = facturaAnticipadaService;
            _kgDisponiblesFasService = kgDisponiblesFasService;
        }

        public Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            Log.Info($"Agregar orden de carga con datos: {ordenDeCarga.ToDto().ToJson()}. MailUsuario: {mailUsuario}");
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                ValidarOrdenDeCargaAlta(ordenDeCarga, usuario);
                var ordenPuedeEnviarseDirectoSap = ValidarKgDisponiblesEnviaDirectamenteASAP(ordenDeCarga, usuario);
                LlenarOrdenAlta(ordenDeCarga, usuario);

                var usuarioPuedeEnviarASAP = usuario.TienePermiso(PermisoEnum.EnviarASap);

                var crearPedido = VerificarOrden(ordenDeCarga, ordenDeCarga.Cliente, false);
                Log.Debug(this.GetType().Name, "Agregar", $" crearPedido: {crearPedido}");
                repositorio.Agregar(ordenDeCarga);
                repositorio.GuardarCambios();

                NotificarContratoSinKm(ordenDeCarga);
                NotificarTransporte(ordenDeCarga.Id);

                if (ordenDeCarga.Estado == EstadoOrdenDeCarga.ContratoVencido)
                {
                    emailFasService.EnviarMailContratoVencido(ordenDeCarga);
                }
                if (crearPedido && (ordenPuedeEnviarseDirectoSap || usuarioPuedeEnviarASAP))
                {
                    ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                    if (ordenDeCarga.EsFacturaAnticipada)
                    {
                        if (!ordenDeCarga.SinSeleccionarFactura)
                            GenerarEntregaSAP(ordenDeCarga);
                    }
                    else
                    {
                        var creadaEnSAP = CrearPedidoEnSAP(ordenDeCarga, ordenDeCarga.Cliente, true, usuarioPuedeEnviarASAP, mailUsuario);
                        if (creadaEnSAP)
                        {
                            VerificarSituacionCrediticia(ordenDeCarga, true);
                        }
                    }

                }
                if (!ordenPuedeEnviarseDirectoSap)
                {
                    emailFasService.EnviarMailVariosContratos(ordenDeCarga);
                }
                NotificarVariasFacturas(ordenDeCarga);

                var resultado = new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaAgregada };
                Log.Info($"Result: {resultado.ToJson()}");
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

        private void LlenarOrdenAlta(OrdenDeCarga ordenDeCarga, Usuario usuario)
        {
            ordenDeCarga.Estado = EstadoOrdenDeCarga.ErrorDeCarga;
            LlenarOrdenAltaCorredorCliente(ordenDeCarga, usuario);

            ordenDeCarga.UsuarioCreacion_Id = usuario.Id;
            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.ContratoSinCantidadPendiente = false;
            ordenDeCarga.NumeroPedido = string.IsNullOrEmpty(ordenDeCarga.NumeroPedidoIngresado) ? "" : ordenDeCarga.NumeroPedidoIngresado;
            ordenDeCarga.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;

            var producto = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);
            ordenDeCarga.Producto = producto;

            if (ordenDeCarga.EsFacturaAnticipada)
            {
                ordenDeCarga.AprobadoCredito = true;
                if (!_facturaAnticipadaService.OrdenConMultiplesFacturas(ordenDeCarga))
                {
                    ordenDeCarga.NumeroFacturaSeleccionada = ordenDeCarga.NumeroFactura;
                    ordenDeCarga.NumeroPedido = ordenDeCarga.NumeroPedidoIngresado;
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
            }

            ordenDeCarga.TransporteExiste = TransporteExiste(ordenDeCarga);

            var dayOfWeek = ordenDeCarga.FechaCarga.DayOfWeek;
            ordenDeCarga.FechaVencimiento = (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Thursday) ? CalcularFechaVencimiento(4, DateTime.Now) : CalcularFechaVencimiento(2, DateTime.Now);
        }

        private void LlenarOrdenAltaCorredorCliente(OrdenDeCarga ordenDeCarga, Usuario usuario)
        {
            var esComercial = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaComerciales);
            Log.Info($"Llenar Orden de carga para alta. esComercial: {esComercial}.");

            Proveedor cliente = null;
            Proveedor corredor = null;
            if (esComercial)
            {
                cliente = repositorio.Obtener<Proveedor>(x => x.CUIT == ordenDeCarga.CUITCliente && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
                if (!string.IsNullOrEmpty(ordenDeCarga.CUITCorredor))
                {
                    corredor = repositorio.Obtener<Proveedor>(x => x.CUIT == ordenDeCarga.CUITCorredor && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor);
                    if (corredor != null)
                    {
                        ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
                        ordenDeCarga.Corredor_Id = corredor.Id;
                    }
                    else
                    {
                        throw new Exception("No se encontró el corredor seleccionado");
                    }
                }
            }
            else
            {
                if (usuario.EsCorredor())
                {
                    corredor = usuario.ObtenerCorredor();
                    ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
                    ordenDeCarga.Corredor_Id = corredor.Id;
                    ordenDeCarga.CUITCorredor = corredor.CUIT;
                    cliente = usuario.Proveedores.Where(prov => prov.CUIT == ordenDeCarga.CUITCliente && prov.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente).First();
                    if (cliente == null)
                        throw new Exception("Su usuario no está habilitado para operar con esa CUIT");
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

        public Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            Log.Info($"Editar(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, mailUsuario: {mailUsuario})");
            var valoresAEditar = new List<string> { "NombreChofer", "CUITChofer", "PatenteAcoplado", "ChasisAcoplado", "ContratoIngresado", "NumeroPedido", "Observacion", "Cantidad", "RazonSocialTransporte", "CUITTransporte", "Producto_Id", "NumeroPedidoIngresado" };
            var historialCambios = new List<OrdenDeCargaCambiosHistorial>() { };
            var (cuilChoferValido, choferEnScato) = ValidarCuilChofer(ordenDeCarga.CUITChofer);
            if (!cuilChoferValido)
                throw new ValidationCustomException("Cuil de chofer invalido");

            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodasOrdenesDeCarga);
                var puedeEnviarASAP = usuario.TienePermiso(PermisoEnum.EnviarASap);
                var esTercero = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaDeTerceros);
                var esComercial = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaComerciales);
                var esMesaFas = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaMesaFas);
                var esPuerto = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaPuerto);
                var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);

                Log.Debug(this.GetType().Name, "Editar", $" usuarioPuedeEnviarASAP: {puedeEnviarASAP}");
                Log.Debug(this.GetType().Name, "Editar", $" chofer en Scato: {choferEnScato.ToJson()}");

                var cargarDatosOCEditar = CargarDatosOCEditar(ordenDeCarga, usuario);
                var ordenEditar = cargarDatosOCEditar.Item1;
                var listaValoresDiferentes = cargarDatosOCEditar.Item2;

                foreach (var prop in listaValoresDiferentes)
                {
                    if (!valoresAEditar.Contains(prop.PropertyName))
                        continue;

                    var anterior = !string.IsNullOrEmpty(prop.valA?.ToString()) ? prop.valA?.ToString() : "-";
                    var nuevo = !string.IsNullOrEmpty(prop.valB?.ToString()) ? prop.valB?.ToString() : "-";
                    if (anterior != "-" && nuevo != "-")
                    {
                        var registroHistorial = new OrdenDeCargaCambiosHistorial();
                        registroHistorial.Id = 0;
                        registroHistorial.Antes = anterior;
                        registroHistorial.Despues = nuevo;
                        registroHistorial.NombreColumnaCambio = prop.PropertyName;
                        registroHistorial.FechaCambio = DateTime.Now;
                        registroHistorial.Usuario_Id = usuario.Id;
                        registroHistorial.OrdenDeCarga_Id = ordenDeCarga.Id;
                        historialCambios.Add(registroHistorial);
                    }
                }
                //Solicitud de edición
                if (!esInterno && historialCambios.Count > 0)
                {
                    SolicitarEdicionOrden(ordenDeCarga.Id, mailUsuario);
                }

                foreach (var historialCambio in historialCambios)
                {
                    repositorio.Agregar(historialCambio);
                }
                ordenEditar.HistorialCambios.Concat(historialCambios);

                if (puedeEnviarASAP && ordenEditar.NumeroEntrega != null)
                {

                    var resultadoSAP = consumer.ModificarEntregaOrdenCarga(new ModificarEntregaOrdenCargaSAP(ordenEditar));
                    if (resultadoSAP.HayError)
                        throw new InfoCustomException(resultadoSAP.Errores[0].Message);
                }
                if (historialCambios.Count > 0 && !esAdmin)
                {
                    //Aviso de Edición de Orden de Carga
                    var emailSenderData = ConstruirCuerpoEmail(historialCambios, ordenDeCarga.NumeroEntrega, ordenDeCarga.NumeroPedido);
                    if (emailSenderData != null)
                    {
                        EmailSender.EnviarMail(emailSenderData);
                    }
                }

                repositorio.GuardarCambios();
                NotificarTransporte(ordenEditar.Id);

                if (puedeEnviarASAP && !ordenEditar.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    if (ordenEditar.TransporteExiste && string.IsNullOrEmpty(ordenEditar.NumeroEntrega)
                        && ordenEditar.AprobadoCredito &&
                        (!ordenEditar.SinSeleccionarFactura || !ordenEditar.EsFacturaAnticipada)
                        )
                    {
                        GenerarEntregaSAP(ordenEditar);
                    }

                }

                var resultado = new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
                Log.Info($"Result: {resultado.ToJson()}");
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Resultado { error = ex.Message };
            }
        }
        private (OrdenDeCarga, List<Variance>) CargarDatosOCEditar(OrdenDeCarga ordenDeCarga, Usuario usuario)
        {
            Log.Info($"CargarDatosOCEditar(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, usuario: {usuario?.Id.ToJson()})");
            var ordenEditar = repositorio.Obtener<OrdenDeCarga>(ordenDeCarga.Id);
            var listaValoresDiferentes = ordenEditar.Compare(ordenDeCarga);
            var historialCambios = new List<OrdenDeCargaCambiosHistorial>() { };
            var puedeEnviarASAP = usuario.TienePermiso(PermisoEnum.EnviarASap);
            ordenEditar.NombreChofer = ordenDeCarga.NombreChofer;
            ordenEditar.CUITChofer = ordenDeCarga.CUITChofer;
            ordenEditar.PatenteAcoplado = ordenDeCarga.PatenteAcoplado;
            ordenEditar.ChasisAcoplado = ordenDeCarga.ChasisAcoplado;
            ordenEditar.RazonSocialTransporte = ordenDeCarga.RazonSocialTransporte;
            ordenEditar.CUITTransporte = ordenDeCarga.CUITTransporte;
            ordenEditar.ContratoIngresado = ordenDeCarga.ContratoIngresado;
            ordenEditar.Cantidad = ordenDeCarga.Cantidad;
            //ordenEditar.Producto_Id = ordenDeCarga.Producto_Id;
            var product = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);
            ordenEditar.Producto = product;
            ordenEditar.NumeroPedidoIngresado = ordenDeCarga.NumeroPedidoIngresado;
            ordenEditar.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;
            ordenEditar.Escalable = ordenDeCarga.Escalable;
            var validaCPEDG = product.ValidaSisaRuca;
            if (validaCPEDG && string.IsNullOrEmpty(ordenDeCarga.CUITDestinatario))
                UsarCUITClienteParaDestinatario(ordenEditar);

            if (!ordenEditar.InformadaSAP || listaValoresDiferentes.Exists(x => x.PropertyName == "ContratoIngresado"))
            {
                var verificarOrden = VerificarOrden(ordenEditar, ordenEditar.Cliente, false);//, puedeEnviarASAP);
                if (ordenEditar.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    NotificarContratoSinKm(ordenEditar);
                }
                else
                {
                    var crearPedido = !string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP) || verificarOrden;
                    if (crearPedido && puedeEnviarASAP)
                    {
                        if (string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP))
                        {
                            ordenEditar.ContratoSAP = ordenEditar.ContratoIngresado;
                        }
                        var creadaEnSaP = CrearPedidoEnSAP(ordenEditar, ordenEditar.Cliente, true, puedeEnviarASAP, usuario.Mail);
                        if (creadaEnSaP)
                        {
                            VerificarSituacionCrediticia(ordenEditar, true);
                        }
                    }
                }
            }
            ordenEditar.Observacion = ordenDeCarga.Observacion;
            ordenEditar.TransporteExiste = TransporteExiste(ordenDeCarga);
            return (ordenEditar, listaValoresDiferentes);
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

        public CrearOrdenEnSAPResponse CrearOrdenEnSAP(CrearOrdenEnSAPRequest request, bool puedeEnviarASAP = false)
        {
            Log.Info($"CrearOrdenEnSAP(request: {request.ToJson()}, usuarioPuedeEnviarASAP: {puedeEnviarASAP})");
            var response = new CrearOrdenEnSAPResponse();
            response.ResultCreation = true;
            var creadaEnSAP = false;
            try
            {
                var ordenDeCarga = repositorio.Obtener<OrdenDeCarga>(q => q.Id == request.IdOrdenDeCarga);
                var mailUsuarioSAP = puedeEnviarASAP ? request.MailUsuarioSAP : _usuarioAutomaticoSAP;
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioSAP);
                //var validarKg = "X";

                var crearOrdenReq = new CrearOrdenRequest
                {
                    Cliente = ordenDeCarga.Cliente.CodigoProveedor,
                    Contrato = ordenDeCarga.ContratoIngresado,
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

                var result = consumer.CrearOrden(crearOrdenReq, out string numeroPedido, out string rawResult);
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

                Log.Debug(this.GetType().Name, "CrearOrdenEnSAP", $" actualizarEstado, inicial: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
                ordenDeCarga.ActualizarEstado();
                Log.Debug(this.GetType().Name, "CrearOrdenEnSAP", $" actualizarEstado, final: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
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
        private bool CrearPedidoEnSAP(OrdenDeCarga ordenDeCarga, Proveedor cliente, bool validaKg, bool puedeEnviarASAP, string mailUsuario)
        {
            //OV-01   'Verificar Contrato, Material, Cliente'
            //OV-02   'Verificar cantidad pendiente de Contratada'
            //OV-03   'Pedido creado - Verificar Crédito de pedido'
            //OV-00   'OK'
            Log.Info($"CrearPedidoEnSAP(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, cliente: {cliente?.Id.ToJson()}, validaKg: {validaKg}, usuarioPuedeEnviarASAP: {puedeEnviarASAP})");
            string contrato = null;
            if (ordenDeCarga.ContratoSAP != null)
            {
                contrato = ordenDeCarga.ContratoSAP.Split('|').First();
            }
            var mailUsuarioSAP = puedeEnviarASAP ? mailUsuario : _usuarioAutomaticoSAP;
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuarioSAP);

            var crearOrdenReq = new CrearOrdenRequest
            {
                Cliente = cliente.CodigoProveedor,
                Contrato = contrato,
                Corredor = ordenDeCarga.CodigoCorredor,
                Kilos = ordenDeCarga.Cantidad,
                Material = ordenDeCarga.Producto.CodigoSap,
                PedidoInput = ordenDeCarga.NumeroPedidoIngresado,
                UsuarioSAP = usuario.UsuarioSap,
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

            var result = consumer.CrearOrden(crearOrdenReq, out string numeroPedido, out string rawResult);

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

        private bool VerificarOrden(OrdenDeCarga ordenDeCarga, Proveedor cliente, bool esJob)
        {
            Log.Info($"VerificarOrden(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, cliente: {cliente?.Id.ToJson()}, esJob: {esJob})");

            //Existe la posibilidad de que el cliente tenga varios contratos abiertos con molinos. En ese caso,
            //un comercial debe seleccionar cual es el contrato correcto que le quiere entregar.
            //if (controlCargaResponse.TieneMultiplesContratos)
            //{
            //    ordenDeCarga.CodigoVerificacionSap = "";
            //    ordenDeCarga.DescripcionCodigoVerificacionSap = "";
            //    var numerosContratos = controlCargaResponse.ObtenerNumerosContratos();
            //    var contratosAbiertos = ObtenerContratosAbiertos(numerosContratos);
            //    //Pendiente deficinición queda como si siempre tuviera muchos contratos abiertos

            //    SetTieneVariosContratosAbiertos(ordenDeCarga, contratosAbiertos);

            //    var logCambio = ordenDeCarga.ActualizarEstado();
            //    Log.Info(logCambio);

            //    return true;
            //}
            //else
            //{   
            //}

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
            if (!esJob && puedeCrearPedido)
            {
                if (!ValidarVencimientoContrato(ordenDeCarga.ContratoIngresado, cliente))
                {
                    descripcionCodigoVerificacionSap = "";
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

        public List<OrdenDeCargaDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
            Log.Info($"Listar(mailUsuario: {mailUsuario}, fechaInicio: {fechaInicio}, fechaFin: {fechaFin})");
            DateTime fechaIncioDateTime, fechaFinDateTime;
            try
            {
                fechaIncioDateTime = DateTime.Parse(fechaInicio);
            }
            catch
            {
                try
                {
                    fechaInicio = new string(fechaInicio.Where(c => c != '\u200E').ToArray());
                    fechaIncioDateTime = DateTime.Parse(fechaInicio);
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "inicio"), e);
                }
            }

            try
            {
                fechaFinDateTime = DateTime.Parse(fechaFin);
            }
            catch
            {
                try
                {
                    fechaFin = new string(fechaFin.Where(c => c != '\u200E').ToArray());
                    fechaFinDateTime = DateTime.Parse(fechaFin);
                }
                catch (Exception e)
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "fin"), e);
                }
            }

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var esAdmin = usuario.TienePermiso("VER TODAS ORDENES DE CARGA");
            var esTercero = usuario.TienePermiso("VER ORDENES DE CARGA DE TERCEROS");
            var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");
            var esMesaFas = usuario.TienePermiso("VER ORDENES DE CARGA PARA MESA FAS");
            var esPuerto = usuario.TienePermiso("VER ORDENES DE CARGA PARA PUERTO");
            var descripcion = EstadoOrdenDeCarga.EdicionRechazada;
            //Log.Debug(this.GetType().Name, "Listar", $" esAdmin: " + esAdmin);
            //Log.Debug(this.GetType().Name, "Listar", $" esTercero: " + esTercero);
            //Log.Debug(this.GetType().Name, "Listar", $" esComercial: " + esComercial);
            //Log.Debug(this.GetType().Name, "Listar", $" esMesaFas: " + esMesaFas);
            //Log.Debug(this.GetType().Name, "Listar", $" esPuerto: " + esPuerto);
            //Log.Debug(this.GetType().Name, "Listar", $" estadoOrdenDeCarga: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(descripcion));
            var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);
            //Log.Debug(this.GetType().Name, "Listar", $" esInterno: " + esInterno);
            fechaFinDateTime = fechaFinDateTime.AddDays(1);

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            if (esInterno)
            {
                var filtrosEstados = new List<EstadoOrdenDeCarga>();
                if (esMesaFas)
                {
                    filtrosEstados.Add(EstadoOrdenDeCarga.Confirmado);
                    filtrosEstados.Add(EstadoOrdenDeCarga.PendienteAprobacionCredito);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaPendiente);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.AnulacionSolicitada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EdicionSolicitada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EdicionRechazada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.ContratoVencido);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Vencida);
                    filtrosEstados.Add(EstadoOrdenDeCarga.SinEnviarASAP);
                    filtrosEstados.Add(EstadoOrdenDeCarga.PendienteCompensacion);
                }
                if (esComercial)
                {
                    filtrosEstados.Add(EstadoOrdenDeCarga.ErrorDeCarga);
                    filtrosEstados.Add(EstadoOrdenDeCarga.AnuladaPorVencimiento);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Pendiente);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Vencida);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Anulada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EdicionSolicitada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.ContratoVencido);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EdicionRechazada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.SinEnviarASAP);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
                    filtrosEstados.Add(EstadoOrdenDeCarga.PendienteCompensacion);
                }
                if (esPuerto)
                {
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
                }
                if (esAdmin)
                {
                    filtrosEstados.Add(EstadoOrdenDeCarga.Pendiente);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Confirmado);
                    filtrosEstados.Add(EstadoOrdenDeCarga.PendienteAprobacionCredito);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Anulada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Vencida);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaPendiente);
                    filtrosEstados.Add(EstadoOrdenDeCarga.AnuladaPorVencimiento);
                    filtrosEstados.Add(EstadoOrdenDeCarga.ErrorDeCarga);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EdicionSolicitada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.AnulacionSolicitada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.ContratoVencido);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EdicionRechazada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.SinEnviarASAP);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
                    filtrosEstados.Add(EstadoOrdenDeCarga.PendienteCompensacion);
                }
                Expression<Func<OrdenDeCarga, bool>> filtro = o => o.FechaCarga <= fechaFinDateTime
                    && o.FechaCarga >= fechaIncioDateTime
                    && filtrosEstados.Contains(o.Estado);
                var listadoConFiltro = repositorio.Listar<OrdenDeCarga>(filtro);

                listado = listadoConFiltro
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
                        NoEstaEnSAP = (x.Estado.ToFriendlyString() == "Sin Enviar a SAP"),
                        EstaSeleccionado = false,
                        EdicionRechazada = x.EdicionRechazada,
                        Escalable = x.Escalable
                    }).OrderByDescending(y => y.Id).ToList();
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                var listadoSinFiltro = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id) && n.FechaCarga <= fechaFinDateTime
                    && n.FechaCarga >= fechaIncioDateTime
                    && (_estadosListarNoInternos.Contains(n.Estado))
                    );
                //Log.Debug(this.GetType().Name, "Listar", $" listadoSinFiltro: {listadoSinFiltro.ToJson()}");
                listado = listadoSinFiltro
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
                        NoEstaEnSAP = (x.Estado.ToFriendlyString() == "Sin Enviar a SAP"),
                        EstaSeleccionado = false,
                        EdicionRechazada = x.EdicionRechazada
                    }).OrderByDescending(y => y.Id).ToList();
            }
            if (listado == null || listado.Count == 0)
            {
                var error = new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de cargas"));
                Log.Error(error);
                throw error;
            }
            //Log.Debug(this.GetType().Name, "Listar", $" listado: {listado.ToJson()}");
            return listado;
        }

        public OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            List<OrdenDeCargaCambiosHistorialDto> listado = new List<OrdenDeCargaCambiosHistorialDto>();
            OrdenDeCarga orden;


            var esAdmin = usuario.TienePermiso("VER TODAS ORDENES DE CARGA");
            var esTercero = usuario.TienePermiso("VER ORDENES DE CARGA DE TERCEROS");
            var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");
            var esMesaFas = usuario.TienePermiso("VER ORDENES DE CARGA PARA MESA FAS");
            var esPuerto = usuario.TienePermiso("VER ORDENES DE CARGA PARA PUERTO");

            var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);

            if (esInterno)
            {
                orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                orden = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id) && n.Id == ordenId).FirstOrDefault();
            }

            if (orden == null) throw new InfoCustomException("No se encontró ninguna orden de carga");

            Proveedor cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);
            //  var ordenDeCargaCambiosHistorial = repositorio.Listar<OrdenDeCargaCambiosHistorial>(ordenes => ordenes.OrdenDeCarga_Id == orden.Id);

            var ordenDeCargaCambiosHistorial = ObtenerCambiosHistorial(orden);

            var contratoSAP = consumer.ObtenerContratoSAP(orden.ContratoIngresado, TipoContratoFAS.Todos);

            var ordenDto = new OrdenDeCargaDetalleDto(orden, ordenDeCargaCambiosHistorial, cliente)
            {
                ContratoSeleccionado = new ContratoOrdenFas(orden, contratoSAP)
            };

            return ordenDto;
        }

        public string ObtenerDescripcionEstado(OrdenDeCarga orden, bool esUsuarioFinal)
        {

            var ordenDeCargaCambiosHistorial = repositorio.Listar<OrdenDeCargaCambiosHistorial>
             (ordenes => ordenes.OrdenDeCarga_Id == orden.Id).ToList();

            if (ordenDeCargaCambiosHistorial.Count > 0)
            {
                if (orden.Estado == EstadoOrdenDeCarga.EdicionRechazada)
                {
                    var estadoAnterior = repositorio.Listar<OrdenDeCargaCambiosHistorial>(o => o.NombreColumnaCambio == "estado" && o.OrdenDeCarga_Id == orden.Id)
                                                  .OrderByDescending(x => x.FechaCambio)
                                                  .Take(1)
                                                  .FirstOrDefault().Antes;
                    var descripcion = new EstadoOrdenDeCarga();
                    descripcion = (EstadoOrdenDeCarga)int.Parse(estadoAnterior);

                    return !esUsuarioFinal ? (EstadoOrdenDeCarga)int.Parse(estadoAnterior) + "(" + EstadoOrdenDeCarga.EdicionRechazada.ToFriendlyString() + ")" : descripcion.ToUserFriendlyString() + "(" + EstadoOrdenDeCarga.EdicionRechazada.ToUserFriendlyString() + ")";
                }
            }

            return !esUsuarioFinal ? orden.Estado.ToFriendlyString() : orden.Estado.ToUserFriendlyString();
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
                AnularOrdenSap(orden);

            emailFasService.EnviarMailOrdenDeCargaVencida(orden);

            orden.Estado = EstadoOrdenDeCarga.AnuladaPorVencimiento;
            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAnulada;
        }

        public OrdenDeCargaEditarDto ObtenerEditar(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var ordenDto = new OrdenDeCargaEditarDto(orden);

            return ordenDto;
        }

        public List<OrdenDeCargaHistorialDto> ObtenerEditarHistorial(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
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
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var puedeEnviarASAP = usuario.TienePermiso("ENVIAR A SAP");

            if (puedeEnviarASAP)
                AnularOrdenSap(orden);

            var ordenHistorial = new OrdenDeCargaCambiosHistorial()
            {
                Id = 0,
                Antes = EstadoOrdenDeCargaExtensions.ToFriendlyString(orden.Estado),
                Despues = EstadoOrdenDeCargaExtensions.ToFriendlyString(EstadoOrdenDeCarga.Anulada),
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

        public string SolicitarAnulacionOrden(int ordenId, string mailUsuario)
        {
            try
            {
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var ordenHistorial = new OrdenDeCargaCambiosHistorial()
                {
                    Id = 0,
                    Antes = EstadoOrdenDeCargaExtensions.ToFriendlyString(orden.Estado),
                    Despues = EstadoOrdenDeCargaExtensions.ToFriendlyString(EstadoOrdenDeCarga.AnulacionSolicitada),
                    NombreColumnaCambio = "estado",
                    FechaCambio = DateTime.Now,
                    Usuario_Id = usuario.Id,
                    OrdenDeCarga_Id = orden.Id
                };
                repositorio.Agregar(ordenHistorial);
                orden.Estado = EstadoOrdenDeCarga.AnulacionSolicitada;
                repositorio.GuardarCambios();
                NotificarSolicitudAnulacion(ordenId);
                return SuccessMsg.OrdenDeCargaActualizada;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        public string NotificarSolicitudAnulacion(int ordenDeCargaId)
        {
            try
            {
                var emailSenderData = ConstruirCuerpoMailSolicitudAnulacion(ordenDeCargaId);
                if (emailSenderData != null)
                {
                    EmailSender.EnviarMail(emailSenderData);
                }

                return "Notificación enviada";
            }
            catch (Exception ex)
            {
                return $"Error al enviar la notificación : {ex.Message}";
            }
        }
        public EmailSenderData ConstruirCuerpoMailSolicitudAnulacion(int ordenDeCargaId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);
            var emailSenderData = new EmailSenderData();
            var ordenVencidas = new StringBuilder();
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas });
            emailSenderData.Mails.AddRange(mailsComerciales.Split(';').ToList());
            string asunto = $"Solicitud de anulación, Orden de carga N° {ordenDeCargaId}";
            string titulo = $"Se informa que el día {DateTime.Now.ToString()} se ha solicitado la anulación de la siguiente orden de carga:";
            var cabecera = "Orden :";
            ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
            emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, ordenVencidas, titulo, cabecera);
            emailSenderData.Asunto = $"Solicitud de anulación, Orden de carga N° {ordenDeCargaId}";
            return emailSenderData;
        }
        public string RechazarSolicitudAnulacion(int ordenId, string mailUsuario)
        {
            try
            {
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

                var estadoAnterior = repositorio.Listar<OrdenDeCargaCambiosHistorial>(o => o.NombreColumnaCambio == "estado" && o.OrdenDeCarga_Id == ordenId)
                                                .OrderByDescending(x => x.FechaCambio)
                                                .Take(1)
                                                .FirstOrDefault().Antes;

                var estadoAnteriorDespues = repositorio.Listar<OrdenDeCargaCambiosHistorial>(o => o.NombreColumnaCambio == "estado" && o.OrdenDeCarga_Id == ordenId)
                                                .OrderByDescending(x => x.FechaCambio)
                                                .Take(1)
                                                .FirstOrDefault().Despues;

                orden.HistorialCambios.Add(new OrdenDeCargaCambiosHistorial
                {
                    Antes = estadoAnteriorDespues,
                    Despues = estadoAnterior,
                    FechaCambio = DateTime.Now,
                    NombreColumnaCambio = "estado",
                    Usuario_Id = usuario.Id
                });
                orden.Estado = EstadoOrdenDeCargaExtensions.ObtenerDescripcionEstado(estadoAnterior);
                repositorio.GuardarCambios();

                return SuccessMsg.OrdenDeCargaActualizada;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string SolicitarEdicionOrden(int ordenId, string mailUsuario)
        {
            try
            {
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var ordenHistorial = new OrdenDeCargaCambiosHistorial()
                {
                    Id = 0,
                    Antes = EstadoOrdenDeCargaExtensions.ToFriendlyString(orden.Estado),
                    Despues = EstadoOrdenDeCargaExtensions.ToFriendlyString(EstadoOrdenDeCarga.EdicionSolicitada),
                    NombreColumnaCambio = "estado",
                    FechaCambio = DateTime.Now,
                    Usuario_Id = usuario.Id,
                    OrdenDeCarga_Id = orden.Id
                };

                repositorio.Agregar(ordenHistorial);
                orden.Estado = EstadoOrdenDeCarga.EdicionSolicitada;

                return SuccessMsg.OrdenDeCargaActualizada;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string EdicionFinalizada(int ordenId, string mailUsuario)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var puedeEnviarASAP = usuario.TienePermiso("ENVIAR A SAP");
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

                var estadoAnterior = repositorio.Listar<OrdenDeCargaCambiosHistorial>(o => o.NombreColumnaCambio == "estado" && o.OrdenDeCarga_Id == ordenId)
                                                .OrderByDescending(x => x.FechaCambio)
                                                .Take(1)
                                                .FirstOrDefault().Antes;

                orden.Estado = EstadoOrdenDeCargaExtensions.ObtenerDescripcionEstado(estadoAnterior);
                orden.EdicionRechazada = false;

                if (puedeEnviarASAP && orden.NumeroEntrega != null)
                {
                    var resultado = consumer.ModificarEntregaOrdenCarga(new ModificarEntregaOrdenCargaSAP(orden));
                    if (resultado.HayError)
                        throw new InfoCustomException(resultado.Errores[0].Message);
                }

                repositorio.GuardarCambios();

                return SuccessMsg.OrdenDeCargaActualizada;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string RechazarSolicitudEdicion(int ordenId, string mailUsuario)
        {
            try
            {
                string fechaFormat = repositorio.Listar<OrdenDeCargaCambiosHistorial>(x => x.OrdenDeCarga_Id == ordenId && x.NombreColumnaCambio != "estado").OrderByDescending(x => x.Id).Take(1).FirstOrDefault().FechaCambio.ToString("yyyyMMddHHmm");
                var listaPrevia = repositorio.Listar<OrdenDeCargaCambiosHistorial>(x => x.OrdenDeCarga_Id == ordenId).Select(x => new
                {
                    FechaCambio = x.FechaCambio.ToString("yyyyMMddHHmm"),
                    x.NombreColumnaCambio,
                    x.Antes,
                    x.Despues
                });
                var datosAnteriores = listaPrevia.Where(x => x.FechaCambio == fechaFormat && x.NombreColumnaCambio != "estado").ToList();
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var estadoAnterior = repositorio.Listar<OrdenDeCargaCambiosHistorial>(o => o.NombreColumnaCambio == "estado" && o.OrdenDeCarga_Id == ordenId)
                                                .OrderByDescending(x => x.FechaCambio)
                                                .Take(1)
                                                .FirstOrDefault().Antes;
                var ordenHistorial = new OrdenDeCargaCambiosHistorial()
                {
                    Id = 0,
                    Antes = estadoAnterior,
                    Despues = EstadoOrdenDeCarga.EdicionRechazada.ToFriendlyString(),
                    NombreColumnaCambio = "estado",
                    FechaCambio = DateTime.Now,
                    Usuario_Id = usuario.Id,
                    OrdenDeCarga_Id = orden.Id
                };
                repositorio.Agregar(ordenHistorial);
                repositorio.GuardarCambios();
                foreach (var dato in datosAnteriores)
                {
                    orden.GetType().GetProperty(dato.NombreColumnaCambio).SetValue(orden, dato.Antes, null);
                    ordenHistorial = new OrdenDeCargaCambiosHistorial()
                    {
                        Id = 0,
                        Antes = dato.Despues,
                        Despues = dato.Antes,
                        NombreColumnaCambio = dato.NombreColumnaCambio,
                        FechaCambio = DateTime.Now,
                        Usuario_Id = usuario.Id,
                        OrdenDeCarga_Id = orden.Id
                    };
                    repositorio.Agregar(ordenHistorial);
                    repositorio.GuardarCambios();
                }
                orden.Estado = EstadoOrdenDeCargaExtensions.ObtenerDescripcionEstado(estadoAnterior);
                orden.EdicionRechazada = true;
                repositorio.GuardarCambios();
                return SuccessMsg.OrdenDeCargaActualizada;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            Proveedor cliente;
            OrdenDeCargaDto result = new OrdenDeCargaDto();
            if (ordenDeCarga.CUITCliente == null)
            {
                result.ordenes.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                return result;
            }
            cliente = repositorio.Obtener<Proveedor>(
            x => x.CUIT == ordenDeCarga.CUITCliente &&
            x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
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
            OrdenCargaConsumerMOA ordenCargaConsumerMOA;
            VisualizarClienteResponse response;
            List<Mod.FechaWS> fechas = null;
            try
            {
                var validator = new VisualizarClienteRequestValidator();
                validator.ValidateAndThrow(request);
                //var validateVisualizarClienteRequest = ValidateVisualizarClienteRequest(request);
                //if (!string.IsNullOrEmpty(validateVisualizarClienteRequest))
                //{
                //    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, validateVisualizarClienteRequest));
                //}
                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(string.Empty, string.Empty, request.Corredor, request.FechaInicio, request.FechaFin, string.Empty, request.Pendiente, TipoContratoFAS.Todos, 1);
                //Log.Debug(this.GetType().Name, "VisualizarCliente", $" ordenCargaVisualizarClienteWSMOAResponse: { ordenCargaVisualizarClienteWSMOAResponse.ToJson() }");
                response = new VisualizarClienteResponse();
                response.Clientes = GetClientesFromVisualizarClienteProducto(ordenCargaVisualizarClienteWSMOAResponse, request);
                if (!string.IsNullOrEmpty(request.Corredor))
                    SincronizarRelacionesCorredorCliente(request.Corredor, response);
                //Log.Info($" response: { response.ToJson() }");
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
                        clientesDto.Add(new ProveedorDto
                        {
                            CodigoProveedor = proveedor.CodigoProveedor ?? "",
                            CUIT = proveedor.CUIT,
                            EstadoAprobacion = proveedor.EstadoAprobacion,
                            EstadoAprobacionDescripcion = proveedor.EstadoAprobacion.ToFriendlyString(),
                            Id = proveedor.Id,
                            IdComercialDataAgro = proveedor.IdComercialDataAgro,
                            IdDataAgro = proveedor.IdDataAgro,
                            Mail = proveedor.Mail ?? "",
                            Observaciones = proveedor.Observaciones,
                            RazonSocial = !String.IsNullOrEmpty(proveedor.RazonSocial) ? proveedor.RazonSocial : proveedor.CUIT ?? "",
                            FechaSolicitud = proveedor.FechaSolicitud,
                            Comercial = proveedor.Comercial,
                            EstadoSIPER = proveedor.EstadoSIPER,
                            ContieneDocumentacionFisica = proveedor.ContieneDocumentacionFisica,
                            IdTipoProveedor = proveedor.TipoProveedor.Id
                        });
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

        public VisualizarProductoResponse VisualizarProducto(VisualizarProductoRequest request)
        {
            Log.Info($"VisualizarCliente(request: {request.ToJson()})");
            OrdenCargaConsumerMOA ordenCargaConsumerMOA;
            VisualizarProductoResponse response;
            List<Mod.FechaWS> fechas = null;
            try
            {
                var validator = new VisualizarProductoRequestValidator();
                validator.ValidateAndThrow(request);
                //var validateVisualizarProductoRequest = ValidateVisualizarProductoRequest(request);
                //if (!string.IsNullOrEmpty(validateVisualizarProductoRequest))
                //{
                //    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, validateVisualizarProductoRequest));
                //}
                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(string.Empty, request.Contrato, string.Empty, request.FechaInicio, request.FechaFin, string.Empty, request.Pendiente, TipoContratoFAS.Todos, 2);
                //Log.Debug(this.GetType().Name, "VisualizarProducto", $" ordenCargaVisualizarClienteWSMOAResponse: { ordenCargaVisualizarClienteWSMOAResponse.ToJson() }");
                response = new VisualizarProductoResponse();
                response.Productos = GetProductosFromVisualizarClienteProducto(ordenCargaVisualizarClienteWSMOAResponse);
                //Log.Info($" response: { response.ToJson() }");
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

        public ValidarCorredorClienteContratoProductoResponse ValidarCorredorClienteContratoProducto(ValidarCorredorClienteContratoProductoRequest request)
        {
            request.Contrato = request.Contrato?.Trim();
            request.ClienteCodigo = request.ClienteCodigo?.Trim();
            request.Corredor = request.Corredor?.Trim();
            request.ProductoId = request.ProductoId?.Trim();
            request.UsuarioEmail = request.UsuarioEmail?.Trim();
            Log.Info($"ValidarCorredorClienteContratoProducto(request: {request.ToJson()})");
            ValidarCorredorClienteContratoProductoResponse response;
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
                var cliente = repositorio.Obtener<Proveedor>(x => (x.CUIT == request.ClienteCuit || x.CodigoProveedor == request.ClienteCodigo) && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
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
                response = new ValidarCorredorClienteContratoProductoResponse();
                if (ordenCargaVisualizarClienteWSMOAResponse.Resultados.Count == 0)
                {
                    response.ResultValidation = false;
                }
                else
                {
                    var corredorCodigo = ordenCargaVisualizarClienteWSMOAResponse.Resultados[0].Corredor;
                    var corredorEmail = request.UsuarioEmail;
                    var corredor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == corredorCodigo && x.Mail == corredorEmail && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor);
                    if (corredor == null)
                    {
                        corredor = repositorio.Obtener<Proveedor>(x => x.CodigoProveedor == corredorCodigo && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor);
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

        private Boolean GetResultFromValidarCorredorClienteContratoProducto(OrdenCargaVisualizarClienteWSMOAResponse ordenCargaVisualizarClienteWSMOAResponse, ValidarCorredorClienteContratoProductoRequest request, Material producto)
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
                else if (!request.ClienteCodigo.Equals(string.Empty)
                    && !request.Contrato.Equals(string.Empty)
                    && !request.ProductoId.Equals(string.Empty))
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
            return result;
        }
        private void CrearRelacionCorredorCliente(Proveedor corredor, Proveedor cliente)
        {
            Log.Info($"CrearRelacionCorredorCliente(corredor: {corredor?.Id.ToJson()}, cliente: {cliente?.Id.ToJson()})");
            try
            {
                //var usuarioCorredor = repositorio.Obtener<Usuario>(q => q.CUITRegistro == corredor.CUIT && q.Mail == corredor.Mail && q.TipoUsuario.Id == (int) TipoUsuarioEnum.Corredor && q.Habilitado == true);
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

        private OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarCliente(string cliente, string contrato, string corredor, string fechaInicio, string fechaFin, string material, bool pendiente, TipoContratoFAS tipoContrato, int type)
        {
            try
            {
                List<Mod.FechaWS> fechas = null;
                OrdenCargaConsumerMOA ordenCargaConsumerMOA = new OrdenCargaConsumerMOA();
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
                {
                    fechas = CommonService.toDateList(fechaInicio, fechaFin);
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
                //Log.Info("OrdenCargaVisualizarCliente  result " + ordenCargaVisualizarClienteWSMOAResponse.ToJson());
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

        #region Etapa1
        public Resultado SeleccionarContrato(int ordenId, string contratoSAP, string mailUsuario)
        {
            string resultado = SuccessMsg.OrdenDeCargaActualizada;
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.ContratoSAP = contratoSAP;
            orden.DescripcionErrorInterno = "";
            var logCambioEstado = orden.ActualizarEstado();
            Log.Info("SeleccionarContrato. " + logCambioEstado);
            if (!ValidarVencimientoContrato(contratoSAP, orden.Cliente))
            {
                orden.Estado = EstadoOrdenDeCarga.ContratoVencido;
                repositorio.GuardarCambios();
                emailFasService.EnviarMailContratoVencido(orden);
                return new Resultado { error = "El contrato seleccionado está vencido." };
            }

            NotificarVariasFacturas(orden);

            if (
                orden.TipoContrato == TipoContratoFAS.Anticipado &&
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
            else if (!string.IsNullOrEmpty(orden.ContratoSAP))
            {
                var puedeCrear = VerificarOrden(orden, orden.Cliente, false);

                if (!orden.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    if (orden.TipoContrato == TipoContratoFAS.Anticipado)
                        return GenerarEntregaSAP(orden);

                    var creadaEnSaP = CrearPedidoEnSAP(orden, orden.Cliente, true, false, mailUsuario);
                    if (creadaEnSaP)
                    {
                        return VerificarSituacionCrediticia(orden, true);
                    }
                }
            }

            repositorio.GuardarCambios();

            return new Resultado { Mensaje = resultado };
        }
        public Resultado SeleccionarFactura(int ordenId, string numeroFacturaSeleccionada, string mailUsuario)
        {
            string resultado = SuccessMsg.OrdenDeCargaActualizada;
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            _facturaAnticipadaService.SeleccionarFactura(ordenId, numeroFacturaSeleccionada);

            var logCambioEstado = orden.ActualizarEstado();
            Log.Info("SeleccionarFactura. " + logCambioEstado);
            var contrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
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
            else if (string.IsNullOrEmpty(orden.ContratoSAP))
            {
                var puedeCrear = VerificarOrden(orden, orden.Cliente, false);//, true);

                if (!orden.TieneCodigoSap(ControlCargaResEnum.FaltaCargarKmsEnContrato))
                {
                    var result = GenerarEntregaSAP(orden);
                    return result;
                }
            }

            repositorio.GuardarCambios();

            return new Resultado { Mensaje = resultado };
        }
        public string ForzarCreacionOrden(int ordenId, string mailUsuario)
        {
            Log.Info($"ForzarCreacionOrden " + ordenId.ToJson());
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var creadaEnSaP = CrearPedidoEnSAP(orden, orden.Cliente, false, false, mailUsuario);

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
                TipoContrato = TipoContratoFAS.Normal
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


            //if (string.IsNullOrEmpty(orden.ContratosRespuesta))
            //{
            //    throw new ValidationCustomException("La orden no tiene contratos disponibles para seleccionar.");
            //}

            //var listadoContratos = orden.ContratosRespuesta.Split(',').ToList();

            //return listadoContratos;
        }
        public string SeleccionarPedido(int ordenId, string pedido, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.NumeroPedido = pedido;

            Log.Info("SeleccionarPedido ActualizarEstado " + orden.ToDto().ToJson());
            orden.ActualizarEstado();
            Log.Info("SeleccionarPedido ActualizarEstado Nuevo " + orden.Estado.ToString());

            if (!string.IsNullOrEmpty(orden.NumeroPedido) && orden.TransporteExiste)
            {
                var creadaEnSaP = CrearPedidoEnSAP(orden, orden.Cliente, true, false, mailUsuario);

                if (creadaEnSaP)
                {
                    VerificarSituacionCrediticia(orden, notificar: true);
                }
            }

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }
        public List<string> ObtenerPedidos(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            if (string.IsNullOrEmpty(orden.PedidosRespuesta))
            {
                throw new ValidationCustomException("La orden no tiene pedidos disponibles para seleccionar.");
            }

            var listadoPedidos = orden.PedidosRespuesta.Split(',').ToList();

            return listadoPedidos;
        }
        public string VerificarTransporte(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return VerificarTransporte(orden);
        }
        public string VerificarTransporte(OrdenDeCarga orden)
        {
            orden.TransporteExiste = TransporteExiste(orden);
            VerificarOrden(orden, orden.Cliente, true);

            if (orden.TransporteExiste)
            {
                Log.Info("VerificarTransporte ActualizarEstado " + orden.ToDto().ToJson());
                var aprobadoCredito = orden.AprobadoCredito ? orden.AprobadoCredito : ObtenerSituacionCrediticia(orden);
                if (aprobadoCredito && string.IsNullOrEmpty(orden.NumeroEntrega))
                {
                    GenerarEntregaSAP(orden);
                }
                orden.ActualizarEstado();
                Log.Info("VerificarTransporte ActualizarEstado Nuevo " + orden.Estado.ToString());
                repositorio.GuardarCambios();
                return SuccessMsg.OrdenDeCargaActualizada;
            }
            else
            {
                return _transporteNoExiste;
            }
        }
        public string VerificarEstadoEntrega(OrdenDeCarga orden)
        {
            Log.Info("VerificarEstadoEntrega");
            var result = consumer.OrdenCargaControlEstadoRequest(orden.NumeroEntrega, "", "");

            if (result == "CE-06")
            {
                orden.Estado = EstadoOrdenDeCarga.Entregada;
                repositorio.GuardarCambios();
                return SuccessMsg.OrdenDeCargaActualizada;
            }
            else
            {
                return _entregaEstadoPendiente;
            }
        }

        private bool TransporteExiste(OrdenDeCarga orden)
        {
            var result = consumer.OrdenCargaControlEstadoRequest("", "", orden.CUITTransporte);
            return ResponseConverter.GetOrdenCargaControlEstadoResponse(result) == ControlEstadoResEnum.TransportistaOK;
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
        public void VerificarTransporteBulk()
        {
            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VerificarTransporteOrdenesDeCargaJob").Habilitado == false)
                return;

            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(o => !o.TransporteExiste))
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

            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(q => q.Estado == EstadoOrdenDeCarga.SinEnviarASAP))
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
                    MailUsuarioSAP = String.Empty
                };
                CrearOrdenEnSAP(crearOrdenEnSAPRequest);
            }
        }
        #endregion

        #region Etapa2
        public Resultado VerificarSituacionCrediticia(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return VerificarSituacionCrediticia(orden, false);
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
                return true;
            var numeroPedido = orden.NumeroPedido;

            Log.Info("ObtenerSituacionCrediticia");
            var result = consumer.OrdenCargaControlEstadoRequest("", numeroPedido, "");

            return result == "CE-00";
        }
        public EmailSenderData ConstruirCuerpoEmail(List<OrdenDeCargaCambiosHistorial> ordenDeCargaHistorial, string numeroEntrega, string numeroPedido)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaHistorial[0].OrdenDeCarga_Id);
            var emailSenderData = new EmailSenderData();
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

            try
            {
                if (string.IsNullOrEmpty(mailsMesaVentaFas) &&
                    string.IsNullOrEmpty(mailsComerciales))
                {
                    return null;
                }

                if (ordenDeCargaHistorial.Count == 0)
                {
                    return null;
                }
                emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas });

                if (emailSenderData.Mails.Count == 0)
                {
                    return null;
                }

                var cambios = new StringBuilder();
                var contrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
                foreach (var cambio in ordenDeCargaHistorial.OrderByDescending(x => x.NombreColumnaCambio == "ChasisAcoplado").ThenByDescending(x => x.NombreColumnaCambio == "PatenteAcoplado"))
                {
                    cambios.AppendLine($"<tr><td>{(cambio.NombreColumnaCambio == "ChasisAcoplado" ? "PatenteChasis" : cambio.NombreColumnaCambio)}</td><td>{cambio.Antes}</td><td>{cambio.Despues}</td><td>{cambio.FechaCambio}</td></tr>");
                }
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                emailSenderData.Asunto = $"Molinos Agro - Edición en su orden de carga n°: {ordenDeCargaHistorial[0].OrdenDeCarga_Id}, {orden.Cliente.RazonSocial}, {contrato}";
                emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), ordenDeCargaHistorial[0].OrdenDeCarga_Id, String.IsNullOrEmpty(numeroEntrega) ? "N/G" : numeroEntrega, String.IsNullOrEmpty(numeroPedido) ? "N/G" : numeroPedido, cambios);
                return emailSenderData;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        private Resultado GenerarEntregaSAP(OrdenDeCarga orden)
        {
            Log.Info("Ejecuta OrdenDeCargaService.GenerarEntregaSAP");

            if (!ValidarExistenciaIntermediarioFlete(orden))
            {
                orden.TransporteExiste = false;
                orden.DescripcionCodigoVerificacionSap = "No se pudo generar la entrega. No existe el Intermediario de flete.";
                orden.Estado = EstadoOrdenDeCarga.Pendiente;
                repositorio.GuardarCambios();
                return new Resultado { Mensaje = "No se pudo generar la entrega. No existe el Intermediario de flete." };
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
                DomicilioDescr = orden.DomicilioDescr
            };

            var respHandler = consumer.CrearEntrega(req, !string.IsNullOrEmpty(numeroFactura));

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
        public string NotificarVariosPedidos(int ordenDeCargaId)
        {
            string mensaje = "";

            try
            {
                var emailSenderData = ConstruirCuerpoMailNotificacionVariosPedidos(ordenDeCargaId);
                if (emailSenderData != null)
                {
                    EmailSender.EnviarMail(emailSenderData);
                }

                mensaje = "Notificación enviada";

            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return mensaje;
        }

        public EmailSenderData ConstruirCuerpoMailNotificacionVariosPedidos(int ordenDeCargaId)
        {
            var emailSenderData = new EmailSenderData();
            var ordenVencidas = new StringBuilder();
            string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
            emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas });
            string titulo = "Se encontraron varios pedidos pendientes para el mismo cliente";
            var cabecera = "Orden :";
            ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
            emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, ordenVencidas, titulo, cabecera);
            emailSenderData.Asunto = "Varios pedidos pendientes";
            return emailSenderData;
        }

        private void NotificarVariasFacturas(OrdenDeCarga ordenDeCarga)
        {
            if (ordenDeCarga.EsFacturaAnticipada && _facturaAnticipadaService.OrdenConMultiplesFacturas(ordenDeCarga))
            {
                emailFasService.EnviarMailVariasFacturasPendientes(ordenDeCarga);
            }
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
            var result = consumer.OrdenCargaVisualizarClienteExecute(request);
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
        public DateTime CalcularFechaVencimiento(int dias, DateTime desde)
        {
            var feriados = feriadoService.ObtenerFeriados();
            var fechaHoy = desde;
            var fechaFinal = desde.AddDays(dias);
            foreach (var fechaFeriado in feriados)
            {
                if (fechaFeriado.DayOfWeek.ToString() == "Saturday" || fechaFeriado.DayOfWeek.ToString() == "Sunday")
                    continue;
                if ((fechaFeriado.Date >= fechaHoy) && (fechaFeriado.Date <= fechaFinal))
                {
                    dias++;
                }
            }

            var fechaVencimiento = desde.AddDays(dias);
            return fechaVencimiento;
        }
        #endregion

        public string ActivarOC(int ordenId, string mailUsuario)
        {
            var Usuario_Id = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario).Id;

            var orden = repositorio.Obtener<OrdenDeCarga>(x => x.Id == ordenId);
            var fechaVencimientoOriginal = orden.FechaVencimiento.Value;
            orden.Estado = EstadoOrdenDeCarga.EntregaGenerada;
            var dayOfWeek = orden.FechaVencimiento.Value.DayOfWeek;
            orden.FechaVencimiento = (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Thursday) ? CalcularFechaVencimiento(4, orden.FechaVencimiento.Value) : CalcularFechaVencimiento(2, orden.FechaVencimiento.Value);
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
            if (orden == null) throw new InfoCustomException("No se ha encontrado ningún orden de carga");
            return Obtener(mailUsuario, orden.Id);
        }

        public List<OrdenDeCargaCambiosHistorialDto> ObtenerCambiosHistorial(OrdenDeCarga orden)
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

        public ObtenerContratosDisponiblesResponse ObtenerContratosDisponibles(ObtenerContratosDisponiblesRequest req, string mailUsuario)
        {
            try
            {
                var rangoFechas = string.IsNullOrEmpty(req.FechaDesde) || string.IsNullOrEmpty(req.FechaHasta) ? null :
                    CommonService.toDateList(req.FechaDesde, req.FechaHasta);
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

                var material =
                    usuario.TieneRol(RolEnum.Administracion) || usuario.TieneRol(RolEnum.ClienteConCpedg)
                        ? string.Empty
                        : Constante.CODIGO_SOJA_HIPRO;

                var consumerReq = new OrdenCargaVisualizarClienteWSMOARequest
                {
                    Cliente = req.ClienteCodigo,
                    Contrato = string.Empty,
                    Corredor = req.CorredorCodigo,
                    Fechas = rangoFechas,
                    Material = material,
                    Pendiente = true, // Contratos ABIERTOS
                    TipoContrato = TipoContratoFAS.Todos
                };

                var ordenCargaConsumer = new OrdenCargaConsumerMOA();
                var consumerRes = ordenCargaConsumer.OrdenCargaVisualizarClienteExecute(consumerReq);

                if (consumerRes == null)
                {
                    throw new ValidationCustomException(ErrorMsg.Error);
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

                var contratosEnOrdenesPendientes = repositorio
                    .Listar<OrdenDeCarga>(
                        x =>
                            x.Cliente.CodigoProveedor == req.ClienteCodigo &&
                            ( 
                                (string.IsNullOrEmpty(x.NumeroPedido) && x.TipoContrato== TipoContratoFAS.Normal) ||
                                (string.IsNullOrEmpty(x.NumeroEntrega) && x.TipoContrato == TipoContratoFAS.Anticipado)
                            ) &&
                            !estadosNoTieneOrdenPendienteEnvio.Contains(x.Estado));

                var contratosDisponiblesResp = new ObtenerContratosDisponiblesResponse
                {
                    Contratos = consumerRes.Resultados
                        .Select(contratoSap =>
                            new ContratoOrdenFas(contratoSap, productosBD)
                            {
                                KgDisponibles = _kgDisponiblesFasService.ObtenerKgDisponiblesContrato(contratoSap,
                                    contratosEnOrdenesPendientes)
                            })
                        .OrderBy(contrato => contrato.DescripcionProducto)
                        .ToList()
                };

                return contratosDisponiblesResp;
            }
            catch (InfoCustomException) { throw; }
            catch (ValidationCustomException) { throw; }
            catch (Exception ex)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, ex);
            }
        }

        private List<string> CargarYObtenerMailsDestino(List<string> lista, List<string> mails)
        {
            foreach (string mail in mails)
            {
                if (!string.IsNullOrEmpty(mail))
                    lista.AddRange(mail.Split(';').ToList());
            }
            return lista.Distinct().ToList();
        }
        private void AnularOrdenSap(OrdenDeCarga orden)
        {
            var tieneNumeroEntrega = !string.IsNullOrEmpty(orden.NumeroEntrega);
            if (tieneNumeroEntrega)
                AnularEntregaEnSap(orden);
            if (orden.EsFacturaAnticipada)
                return;
            AnularPedidoEnSap(orden, tieneNumeroEntrega);
        }
        public string EnviarOrdenesASAP(List<int> ordenesId, string mailUsuario)
        {
            var ordenes = repositorio.Listar<OrdenDeCarga>(a => ordenesId.Contains(a.Id) && a.Estado == EstadoOrdenDeCarga.SinEnviarASAP);
            var errores = new List<int>();
            foreach (var ordenDeCarga in ordenes)
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
                    errores.Add(ordenDeCarga.Id);
            }
            if (errores.Count() > 0)
                throw new InfoCustomException($"Las siguientes ordenes no pudieron enviarse correctamente: {string.Join(", ", errores)}");
            return $"Se han enviado las ordenes";
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

            var responseHandler = consumer.ControlarCarga(controlarCargaReq);

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
            var validaDestinatario = validaSISA.Destinatario;
            var validaDestino = validaSISA.Destino;

            var controlarCargaReq = new ControlCargaRequest { SoloSisa = true, Material = ObtenerMaterialValidaSisa() };

            if (validaDestinatario)
                controlarCargaReq.CuitDestinatario = cuit;
            if (validaDestino)
                controlarCargaReq.CuitDestino = cuit;

            var responseHandler = consumer.ControlarCarga(controlarCargaReq);
            var result = false;

            if (validaDestinatario)
                result = !responseHandler.TieneRespuesta(ControlCargaResEnum.DestinatarioInhabilitadoEnSisa);
            else if (validaDestino)
                result = !responseHandler.TieneRespuesta(ControlCargaResEnum.DestinoInhabilitadoEnSisa);

            return result;
        }
        public ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit)
        {

            var result = scatoConsumer.ExisteCuitDestinoDestinatario(cuit);

            return result;
        }

        public bool EmailGestionarAlta(string cuit, string razonSocial, bool esIntermediarioFlete)
        {
            if (esIntermediarioFlete)
            {
                emailFasService.EnviarMailAltaIntermediarioFlete(cuit, razonSocial);
            }
            else
            {
                emailFasService.EnviarMailAltaTempranaCuit(cuit, razonSocial);
            }
            return true;
        }

        public List<PlantaDto> ObtenerPlantasDestino(string destinoCuit)
        {
            var plantasRes = scatoRepositorioClient.ObtenerPlantas(destinoCuit);
            if (!plantasRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Scato con CUIT " + destinoCuit);
                foreach (var err in plantasRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageType, err.Message));
                }
                throw new ValidationCustomException("Error al obtener Plantas");
            }
            else
            {
                return plantasRes.Data
                    .Select(x =>
                        new PlantaDto
                        {
                            Actividad = x.Actividad,
                            Codigo = x.NroPlanta
                        })
                    .ToList();
            }
        }

        public List<DomicilioDto> ObtenerDomiciliosDestino(string destinoCuit)
        {
            var domiciliosRes = scatoRepositorioClient.ObtenerDomicilios(destinoCuit);
            if (!domiciliosRes.IsValid)
            {
                Log.Info("Error al obtener Plantas Domicilios con CUIT " + destinoCuit);
                foreach (var err in domiciliosRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageType, err.Message));
                }
                throw new ValidationCustomException("Error al obtener Domicilios");
            }
            else
            {
                return domiciliosRes.Data
                    .Select(x =>
                        new DomicilioDto
                        {
                            Descripcion = x.Descripcion,
                            Orden = x.Orden,
                            Tipo = x.Tipo
                        })
                    .ToList();
            }
        }
        public bool ValidarCuitRuca(string cuit)
        {
            var tienePlantas = ObtenerPlantasDestino(cuit).Any();
            var tieneDomicilios = ObtenerDomiciliosDestino(cuit).Any();
            return tienePlantas && tieneDomicilios;
        }

        public ValidarIntermediarioFleteResponse ValidarIntermediarioFlete(string cuit)
        {
            var scatoRes = scatoRepositorioClient.ObtenerProveedorPorCuil(cuit);
            if (scatoRes.IsValid)
            {
                return new ValidarIntermediarioFleteResponse
                {
                    EsCuitValido = true,
                    ExisteIntermediario = true,
                    RazonSocial = scatoRes.Data.RazonSocial
                };
            }

            var response = new ValidarIntermediarioFleteResponse();
            if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.DigitoVerificadorNoValido))
            {
                response.EsCuitValido = false;
            }
            else
            {
                if (scatoRes.TieneError(ScatoRepo.ObtenerProveedorPorCuilError.ProveedorNoEncontrado))
                {
                    response.EsCuitValido = true;
                    response.ExisteIntermediario = false;
                }
                else
                {
                    throw new Exception("Error en ValidarIntermediarioFlete. Validación inesperada con cuit " + cuit);
                }
            }
            return response;
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
        private void AnularEntregaEnSap(OrdenDeCarga orden)
        {
            var respHandler = consumer.AnularEntregaOrdenCarga(orden.NumeroEntrega);
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
        private void AnularPedidoEnSap(OrdenDeCarga orden, bool tieneNumeroEntrega)
        {
            var tieneNumeroPedido = !string.IsNullOrEmpty(orden.NumeroPedidoIngresado) || !string.IsNullOrEmpty(orden.NumeroPedido);
            if (!tieneNumeroPedido)
                return;
            var respHandler = consumer.AnularOrdenCarga(orden);
            if (respHandler.PedidoTomadoEnSap)
                throw new InfoCustomException("El pedido está tomado en SAP");
            else if (!(respHandler.ActualizadoOK || respHandler.PedidoAnulado))
            {
                throw new Exception("No se reconoce respuesta SAP (Anular Orden Carga)");
            }
        }
        //private List<string> ObtenerContratosAbiertos(List<string> contratos)
        //{
        //    return contratos.Where(contrato => consumer.VerificarContratoAbierto(contrato)).ToList();
        //}

        //private void SetTieneVariosContratosAbiertos(OrdenDeCarga ordenDeCarga, List<string> contratosAbiertos)
        //{
        //    var result = string.Join(",", contratosAbiertos);
        //    if (string.IsNullOrEmpty(ordenDeCarga.NumeroPedido))
        //    {
        //        if (string.IsNullOrEmpty(ordenDeCarga.ContratoSAP))
        //        {
        //            ordenDeCarga.ContratosRespuesta = result;
        //            ordenDeCarga.ContratoSAP = "";
        //            ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios contratos pendientes para el mismo cliente. Seleccione el contrato para generar entregas desde el botón \"Contratos\".";
        //        }
        //    }
        //    else
        //    {
        //        ordenDeCarga.PedidosRespuesta = result;
        //        ordenDeCarga.NumeroPedido = "";
        //        ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios pedidos pendientes para el mismo cliente. Seleccione el pedido para generar entregas desde el botón \"Pedidos\".";
        //    }
        //}

        public (bool, ScatoRepo.Chofer) ValidarCuilChofer(string cuilChofer)
        {
            var choferRes = scatoRepositorioClient.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer));
            var chofer = choferRes.Data;
            if (!choferRes.IsValid)
            {
                Log.Info("Error al obtener chofer de Scato " + cuilChofer);
                foreach (var err in choferRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageCode, err.Message));
                }

                return (choferRes.Messages.All(msg => msg.MessageCode != ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido), chofer);
            }
            return (true, chofer);
        }
        public (bool, ScatoRepo.Chofer) ValidarCuitTransporte(string cuitTransporte)
        {
            var transporteRes = scatoRepositorioClient.ObtenerTransportePorCuit(DataFormatter.CuitConGuion(cuitTransporte));
            var transporte = transporteRes.Data;
            if (!transporteRes.IsValid)
            {
                Log.Info("Error al obtener transporte de Scato " + cuitTransporte);
                foreach (var err in transporteRes.Messages)
                {
                    Log.Info(string.Format("Error Scato código {0}, descripción: {1}", err.MessageCode, err.Message));
                }

                return (transporteRes.Messages.All(msg => msg.MessageCode != ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido), transporte);
            }
            return (true, transporte);
        }
        public bool ValidarCuilChoferDigito(string cuilChofer)
        {
            return ValidarCuilChofer(cuilChofer).Item1;
        }
        public bool ValidarCuitTransporteDigito(string cuitTransporte)
        {
            return ValidarCuitTransporte(cuitTransporte).Item1;
        }
        public void VerificarCompensacion(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            Log.Info($"Verificar Compensacion: orden: {ordenId}");

            if (orden == null)
                throw new InfoCustomException("No se encontró la orden");

            Log.Info($"Verificar Compensacion: orden: {orden.ToJson()}");
            if (orden.Estado != EstadoOrdenDeCarga.PendienteCompensacion)
                return;

            GenerarEntregaSAP(orden);
        }
        private string ObtenerMaterialValidaSisa()
        {
            var material = repositorio.Obtener<Material>(m => m.ValidaSisaRuca && m.TablaSeccionMaterial == TablaSeccionMaterial.OrdenDeCarga);

            return material?.CodigoSap;
        }
        private void RemoverCamposCPEDG(OrdenDeCarga orden)
        {
            orden.CUITDestinatario = null;
            orden.CUITDestino = null;
            orden.RazonSocialDestinatario = null;
            orden.RazonSocialDestino = null;
            orden.Reventa = false;
            orden.CUITIntermediarioFlete = null;
            orden.DomicilioOrden = null;
        }
        private void UsarCUITClienteParaDestinatario(OrdenDeCarga orden)
        {
            orden.CUITDestinatario = orden.CUITCliente;
            orden.RazonSocialDestinatario = orden.Cliente.RazonSocial;
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

        private void ValidarOrdenDeCargaAlta(OrdenDeCarga orden, Usuario usuario)
        {
            ValidarCuilChofer(orden);
            ValidarReventa(orden, usuario);
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
            }
            Log.Debug(this.GetType().Name, "Agregar", $" chofer en Scato: {choferRes.Data.ToJson()}");
        }

        private void ValidarReventa(OrdenDeCarga orden, Usuario usuario)
        {
            var puedeSeleccionarReventa = usuario.TienePermiso(PermisoEnum.Fas_ModificarCampoReventa);
            if (!puedeSeleccionarReventa && orden.Reventa)
            {
                throw new ValidationCustomException("Usuario sin permiso para modificar campo reventa");
            }
        }

        /// <summary>
        /// Validar la cantidad de kgs disponibles es la adecuada
        /// </summary>
        /// <param name="orden"></param>
        /// <param name="usuario"></param>
        private bool ValidarKgDisponiblesEnviaDirectamenteASAP(OrdenDeCarga orden, Usuario usuario)
        {
            var esAdmin = usuario.TienePermiso(PermisoEnum.VerTodasOrdenesDeCarga);
            var esComercial = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaComerciales);
            var esMesaFas = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaMesaFas);
            var esPuerto = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaParaPuerto);

            var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);
            if (!esInterno)
            {
                var numeroContrato = string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP;
                Log.Info($"Validar kg orden: {orden.ToJson()}");
                var contratoSAP = consumer.ObtenerContratoSAP(numeroContrato, null);
                Log.Info($"Validar kg contrato: {contratoSAP.ToJson()}");

                if (contratoSAP == null)
                    throw new InfoCustomException("No se encontró el contrato en SAP");
                var ordenesPendientes = repositorio
                    .Listar<OrdenDeCarga>(
                        x =>
                            ((!string.IsNullOrEmpty(x.ContratoSAP) && x.ContratoSAP == numeroContrato) ||
                            (string.IsNullOrEmpty(x.ContratoSAP) && x.ContratoIngresado == numeroContrato)) &&
                            (
                                (string.IsNullOrEmpty(x.NumeroPedido) && x.TipoContrato == TipoContratoFAS.Normal) ||
                                (string.IsNullOrEmpty(x.NumeroEntrega) && x.TipoContrato == TipoContratoFAS.Anticipado)
                            ) &&
                            !estadosNoTieneOrdenPendienteEnvio.Contains(x.Estado));

                var kilosDisponibles = _kgDisponiblesFasService.ObtenerKgDisponiblesContrato(contratoSAP, ordenesPendientes);
                if (kilosDisponibles <= Constante.FAS_KILOS_LIMITE_INFERIOR)
                    throw new InfoCustomException("El contrato seleccionado no tiene kg disponibles");
                if (kilosDisponibles < Constante.FAS_KILOS_LIMITE_SUPERIOR)
                    return false;
            }
            return true;
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
                Pedido = ordenDeCarga.NumeroPedido,
                SoloSisa = soloSisa
            };
            return consumer.ControlarCarga(controlarCargaReq);
        }

        private string ObtenerContratoDeOrden(OrdenDeCarga ordenDeCarga)
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

    }
}
