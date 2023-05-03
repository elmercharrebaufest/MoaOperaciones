using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Validadores.OrdenDeCarga;
using SustitucionMOAWS.Enum.OrdenCargaConsumer;
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
using Mod = SustitucionMOAModel.Models;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaService : IOrdenDeCargaService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IOrdenCargaConsumerMOA consumer;
        protected readonly IScatoConsumer scatoConsumer;
        readonly FeriadoService _feriadoService = new FeriadoService();
        protected readonly IFeriadoService feriadoService;
        protected readonly IScatoRepositorioClient scatoRepositorioClient;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AvisoEdicionOrdenDeCarga.html");
        private static readonly string EMAIL_TEMPLATE_ORDENES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "NotificacionOrdenesDeCarga.html");
        private readonly string _usuarioAutomaticoSAP;

        private readonly string _errorAnulacion = "Error al anular orden de carga, pero la entrega si ha sido anulada";
        private readonly string _entregaEstadoPendiente = "La entrega sigue pendiente.";

        public OrdenDeCargaService(
            IRepositorio repositorio,
            IOrdenCargaConsumerMOA consumer,
            IFeriadoService feriadoService,
            IScatoRepositorioClient scatoRepositorioClient,
            IScatoConsumer scatoConsumer)
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
            this.feriadoService = feriadoService;
            this.scatoConsumer = scatoConsumer;
            _usuarioAutomaticoSAP = ConfigurationManager.AppSettings["UsuarioAutomaticoSAP"];
            this.scatoRepositorioClient = scatoRepositorioClient;
        }

        public Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            Log.Info($"Agregar(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, mailUsuario: {mailUsuario})");
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");
                var puedeEnviarASAP = usuario.TienePermiso("ENVIAR A SAP");
                var llenarOrdenDeCarga = LlenarOrdenDeCarga(ordenDeCarga, usuario, esComercial);
                ordenDeCarga = llenarOrdenDeCarga;
                Log.Debug(this.GetType().Name, "Agregar", $" esComercial: {esComercial}");
                Log.Debug(this.GetType().Name, "Agregar", $" puedeEnviarASAP: {puedeEnviarASAP}");

                var crearPedido = VerificarOrden(ordenDeCarga, ordenDeCarga.Cliente, false, puedeEnviarASAP);
                Log.Debug(this.GetType().Name, "Agregar", $" crearPedido: {crearPedido}");
                repositorio.Agregar(ordenDeCarga);
                repositorio.GuardarCambios();
                ValidarCuilChoferEnScato(ordenDeCarga.CUITChofer);
                ValidarCuilChoferEnScato(ordenDeCarga.CUITIntermediarioFlete);
                NotificarContratoSinKm(ordenDeCarga);
                NotificarTransporte(ordenDeCarga.Id);


                if (ordenDeCarga.Estado == EstadoOrdenDeCarga.ContratoVencido)
                {
                    NotificacionContratoVencido(ConstruirCuerpoMailNotificacionContratoVencido(ordenDeCarga));
                }
                if (crearPedido && puedeEnviarASAP)
                {
                    ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                    var creadaEnSAP = CrearPedidoEnSAP(ordenDeCarga, ordenDeCarga.Cliente, true, puedeEnviarASAP, mailUsuario);
                    if (creadaEnSAP)
                    {
                        VerificarSituacionCrediticia(ordenDeCarga, true);
                    }
                }
                if (!string.IsNullOrEmpty(ordenDeCarga.PedidosRespuesta))
                {
                    NotificarVariosPedidos(ordenDeCarga.Id);
                }
                if (!string.IsNullOrEmpty(ordenDeCarga.ContratosRespuesta))
                {
                    NotificarVariosContratos(ConstruirCuerpoMailNotificacionVariosContratos(ordenDeCarga.Id));
                }

                var resultado = new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaAgregada };
                Log.Info($"Result: {resultado.ToJson()}");
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Resultado { error = ex.Message };
            }
        }
        private OrdenDeCarga LlenarOrdenDeCarga(OrdenDeCarga ordenDeCarga, Usuario usuario, bool esComercial)
        {
            Log.Info($"LlenarOrdenDeCarga(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, usuario: {usuario?.Id.ToJson()}, esComercial: {esComercial})");
            Proveedor cliente = null;
            Proveedor corredor = null;
            //var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var producto = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);
            ordenDeCarga.Estado = EstadoOrdenDeCarga.ErrorDeCarga;
            if (esComercial)
            {
                cliente = repositorio.Obtener<Proveedor>(x => x.CUIT == ordenDeCarga.CUITCliente && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente);
                corredor = repositorio.Obtener<Proveedor>(x => x.CUIT == ordenDeCarga.CUITCorredor && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor);
                if (corredor != null && !string.IsNullOrEmpty(ordenDeCarga.CUITCorredor))
                {
                    ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
                    ordenDeCarga.Corredor_Id = corredor.Id;
                }
                else
                {
                    if (ordenDeCarga.CUITCorredor != null)
                    {
                        throw new Exception("No se encontró el corredor seleccionado");
                        //return new Resultado { error = "No se encontró el corredor seleccionado" };
                    }
                }
            }
            else
            {
                if (usuario.EsCorredor())
                {
                    cliente = GetClienteParaCorredor(usuario, corredor, ordenDeCarga);
                    if (cliente == null)
                        throw new Exception("Su usuario no está habilitado para operar con ese CUIT");
                }
                else
                {
                    cliente = usuario.ObtenerProveedor();
                    ordenDeCarga.CodigoCorredor = "";
                    ordenDeCarga.CUITCorredor = "";
                    ordenDeCarga.Corredor_Id = null;
                }
            }
            var validaCPEDG = producto.ValidaSisaRuca;
            if (!validaCPEDG)
                RemoverCamposCPEDG(ordenDeCarga);
            ordenDeCarga.UsuarioCreacion_Id = usuario.Id;
            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.Cliente = cliente;
            ordenDeCarga.Cliente_Id = cliente.Id;
            ordenDeCarga.CUITCliente = cliente.CUIT;
            ordenDeCarga.ContratoSinCantidadPendiente = false;
            ordenDeCarga.Producto = producto;
            ordenDeCarga.NumeroPedido = string.IsNullOrEmpty(ordenDeCarga.NumeroPedidoIngresado) ? "" : ordenDeCarga.NumeroPedidoIngresado;
            ordenDeCarga.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;
            ordenDeCarga.TransporteExiste = TransporteExiste(ordenDeCarga);
            var dayOfWeek = ordenDeCarga.FechaCarga.DayOfWeek;
            ordenDeCarga.FechaVencimiento = (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Thursday) ? CalcularFechaVencimiento(4, DateTime.Now) : CalcularFechaVencimiento(2, DateTime.Now);
            return ordenDeCarga;
        }
        public Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            Log.Info($"Editar(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, mailUsuario: {mailUsuario})");
            var valoresAEditar = new List<string> { "NombreChofer", "CUITChofer", "PatenteAcoplado", "ChasisAcoplado", "ContratoIngresado", "NumeroPedido", "Observacion", "Cantidad", "RazonSocialTransporte", "CUITTransporte", "Producto_Id", "NumeroPedidoIngresado" };
            var historialCambios = new List<OrdenDeCargaCambiosHistorial>() { };
            try
            {
                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
                var esAdmin = usuario.TienePermiso("VER TODAS ORDENES DE CARGA");
                var puedeEnviarASAP = usuario.TienePermiso("ENVIAR A SAP");
                var esTercero = usuario.TienePermiso("VER ORDENES DE CARGA DE TERCEROS");
                var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");
                var esMesaFas = usuario.TienePermiso("VER ORDENES DE CARGA PARA MESA FAS");
                var esPuerto = usuario.TienePermiso("VER ORDENES DE CARGA PARA PUERTO");
                var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);

                Log.Debug(this.GetType().Name, "Editar", $" puedeEnviarASAP: {puedeEnviarASAP}");
                var cargarDatosOCEditar = CargarDatosOCEditar(ordenDeCarga, usuario);
                var ordenEditar = cargarDatosOCEditar.Item1;
                var listaValoresDiferentes = cargarDatosOCEditar.Item2;

                ValidarCuilChoferEnScato(ordenDeCarga.CUITChofer);
                ValidarCuilChoferEnScato(ordenDeCarga.CUITIntermediarioFlete);
                //Solicitud de edición
                if (!esInterno)
                {
                    SolicitarEdicionOrden(ordenDeCarga.Id, mailUsuario);
                }
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

                // error de base de datos al usar agregar todos
                //repositorio.AgregarTodos(historialCambios);
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

                repositorio.GuardarCambios();
                NotificarTransporte(ordenEditar.Id);

                if (puedeEnviarASAP && ordenEditar.CodigoVerificacionSap != "CC-07")
                {
                    if (ordenEditar.TransporteExiste && string.IsNullOrEmpty(ordenEditar.NumeroEntrega) && ordenEditar.AprobadoCredito)
                    {
                        GenerarEntregaSAP(ordenEditar);
                    }

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
            var puedeEnviarASAP = usuario.TienePermiso("ENVIAR A SAP");
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
            if (!ordenEditar.InformadaSAP || listaValoresDiferentes.Exists(x => x.PropertyName == "ContratoIngresado"))
            {
                var verificarOrden = VerificarOrden(ordenEditar, ordenEditar.Cliente, false, puedeEnviarASAP);
                if (ordenEditar.CodigoVerificacionSap == "CC-07")
                {
                    NotificarContratoSinKm(ordenEditar);
                }
                else
                {
                    var crearPedido = !string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP) || verificarOrden;
                    if (puedeEnviarASAP && crearPedido)
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
                if (orden.CodigoVerificacionSap != "CC-07")
                    return;

                var emailSenderData = new EmailSenderData();
                var ordenVencidas = new StringBuilder();
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
                var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
                var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
                emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas });
                string titulo = $"Se informa que el día {DateTime.Now.ToString()} el contrato de la siguiente orden no tiene los Km cargados:";
                var cabecera = "Orden :";
                var contrato = !string.IsNullOrEmpty(orden.ContratoSAP?.Trim()) ? orden.ContratoSAP?.Trim() : orden.ContratoIngresado?.Trim();
                ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{contrato}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
                emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, ordenVencidas, titulo, cabecera);
                emailSenderData.Asunto = $"Faltan cargar los Km en el contrato, Orden de carga N° {orden.Id}";
                if (emailSenderData != null)
                {
                    EmailSender.EnviarMail(emailSenderData);
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
            Log.Info($"CrearOrdenEnSAP(request: {request.ToJson()}, puedeEnviarASAP: {puedeEnviarASAP})");
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

                var result = consumer.CrearOrden(crearOrdenReq, out string numeroPedido);
                
                if (!string.IsNullOrEmpty(result))
                {
                    if (result == "OV-00" || result == "OV-03")
                    {
                        ordenDeCarga.InformadaSAP = true;
                        ordenDeCarga.NumeroPedido = numeroPedido;
                        ordenDeCarga.ContratoSAP = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                        ordenDeCarga.DescripcionErrorInterno = "";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "";
                        ordenDeCarga.CodigoVerificacionSap = "";
                        creadaEnSAP = true;
                    }
                    else
                    {
                        ordenDeCarga.CodigoVerificacionSap = result;
                        if (result == "OV-02")
                        {
                            ordenDeCarga.ContratoSinCantidadPendiente = true;
                            ordenDeCarga.CodigoVerificacionSap = "CC-01";
                            ordenDeCarga.DescripcionErrorInterno = "El contrato ingresado tiene menos de 15 toneladas disponibles. Puede elegir forzar la creación del pedido desde \"Crear pedido\" o anularlo.";
                        }
                        else if (result == "OV-01")
                        {
                            ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                        }
                        else
                        {
                            ordenDeCarga.DescripcionCodigoVerificacionSap = result;
                        }
                    }
                }
                else
                {
                    ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
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
            Log.Info($"CrearPedidoEnSAP(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, cliente: {cliente?.Id.ToJson()}, validaKg: {validaKg}, puedeEnviarASAP: {puedeEnviarASAP})");
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

            var result = consumer.CrearOrden(crearOrdenReq, out string numeroPedido);

            var resultadoCrearOrden = false;
            ordenDeCarga.ContratoSinCantidadPendiente = false;
            //var result2 = consumer.CrearEntrega(orden.CUITChofer, orden.Cantidad, orden.NombreChofer, orden.PatenteAcoplado, orden.ChasisAcoplado, "", "DNI", orden.CUITTransporte, out string mensaje);
            if (result == "OV-00" || result == "OV-03")
            {
                ordenDeCarga.InformadaSAP = true;
                ordenDeCarga.NumeroPedido = numeroPedido;
                ordenDeCarga.ContratoSAP = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                ordenDeCarga.DescripcionErrorInterno = "";
                ordenDeCarga.DescripcionCodigoVerificacionSap = "";
                ordenDeCarga.CodigoVerificacionSap = "";

                //if (result == "OV-03")
                //{
                //    orden.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
                //}

                resultadoCrearOrden = true;
            }
            else
            {
                ordenDeCarga.CodigoVerificacionSap = result;
                if (result == "OV-02")
                {
                    ordenDeCarga.ContratoSinCantidadPendiente = true;
                    ordenDeCarga.CodigoVerificacionSap = "CC-01";
                    ordenDeCarga.DescripcionErrorInterno = "El contrato ingresado tiene menos de 15 toneladas disponibles. Puede elegir forzar la creación del pedido desde \"Crear pedido\" o anularlo.";
                }
                else if (result == "OV-01")
                {
                    ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                }
                else
                {
                    ordenDeCarga.DescripcionCodigoVerificacionSap = result;
                }
            }
            Log.Debug(this.GetType().Name, "CrearOrdenEnSAP", $" actualizarEstado, inicial: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
            ordenDeCarga.ActualizarEstado();
            Log.Debug(this.GetType().Name, "CrearOrdenEnSAP", $" actualizarEstado, final: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
            repositorio.GuardarCambios();
            return resultadoCrearOrden;
        }

        private bool VerificarOrden(OrdenDeCarga ordenDeCarga, Proveedor cliente, bool esJob, bool puedeEnviarASAP = false)
        {
            /* 
            CC-01	'Más de un contrato vigente para Cliente/Corredor'
            CC-02	'Transportista no dado de alta'
            CC-03	'Verificar Pedido' 
            CC-04	'Verificar Crédito de pedido'
            CC-05	'Pedido entregado completamente'
            CC-06	'Considerar como error CC-01'
            CC-07	'Faltan cargar los Km en el contrato'
            CC-08	'Cliente inhabilitado en SISA'
            CC-09	'Corredor inhabilitado en SISA'
            CC-00	'OK'
            */

            Log.Info($"VerificarOrden(ordenDeCarga: {ordenDeCarga.ToDto().ToJson()}, cliente: {cliente?.Id.ToJson()}, esJob: {esJob}, puedeEnviarASAP: {puedeEnviarASAP})");
            string contrato = null;
            if (ordenDeCarga.ContratoIngresado != null)
            {
                contrato = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                contrato = contrato.Split('|').First();
            }

            var controlarCargaReq = new ControlCargaRequest
            {
                Cliente = cliente.CodigoProveedor,
                Contrato = contrato,
                Corredor = ordenDeCarga.CodigoCorredor,
                Cuit = ordenDeCarga.CUITTransporte,
                CuitDestino = ordenDeCarga.CUITDestino,
                CuitDestinatario = ordenDeCarga.CUITDestinatario,
                Material = ordenDeCarga.Producto.CodigoSap,
                Pedido = ordenDeCarga.NumeroPedido,
                SoloSisa = false
            };
            var responseHandler = consumer.ControlarCarga(controlarCargaReq);

            //Existe la posibilidad de que el cliente tenga varios contratos abiertos con molinos. En ese caso,
            //un comercial debe seleccionar cual es el contrato correcto que le quiere entregar.
            if (responseHandler.TieneMultiplesContratos)
            {
                ordenDeCarga.CodigoVerificacionSap = "";
                ordenDeCarga.DescripcionCodigoVerificacionSap = "";
                var numerosContratos = responseHandler.ObtenerNumerosContratos();
                var contratosAbiertos = ObtenerContratosAbiertos(numerosContratos);
                //Pendiente deficinición queda como si siempre tuviera muchos contratos abiertos

                TieneVariosContratosAbiertos(ordenDeCarga, contratosAbiertos);

                Log.Debug(this.GetType().Name, "VerificarOrden", $" actualizarEstado, inicial: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
                ordenDeCarga.ActualizarEstado();
                Log.Debug(this.GetType().Name, "VerificarOrden", $" actualizarEstado, final: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));

                return false;
            }
            else
            {
                var respuestaCC = responseHandler.ObtenerRespuestaUnica();
                //solo en el caso que el response de ok para crear la orden tiene que verificar el vencimiento
                if (!esJob &&
                    (respuestaCC == OrdenCargaControlCarga.OK || respuestaCC == OrdenCargaControlCarga.TransportistaNoDadoDeAlta))
                {
                    if (!ValidarVencimientoContrato(ordenDeCarga.ContratoIngresado, cliente))
                    {
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "";
                        ordenDeCarga.Estado = EstadoOrdenDeCarga.ContratoVencido;
                        return false;
                    }
                    else
                    {
                        ordenDeCarga.Estado = EstadoOrdenDeCarga.SinEnviarASAP;
                    }
                }
                ordenDeCarga.CodigoVerificacionSap = responseHandler.GetCodigoDeRespuesta(respuestaCC);
                switch (respuestaCC)
                {
                    case OrdenCargaControlCarga.OK:
                        ordenDeCarga.TransporteExiste = true;
                        ordenDeCarga.CorredorSeleccionado = true;
                        ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;

                        ordenDeCarga.DescripcionCodigoVerificacionSap = "OK";
                        return true;

                    case OrdenCargaControlCarga.MasDeUnContratoVigente:
                        //ordenDeCarga.CorredorSeleccionado = false;
                        //break;
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                        break;


                    case OrdenCargaControlCarga.TransportistaNoDadoDeAlta:
                        ordenDeCarga.TransporteExiste = false;
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "Transportista no dado de alta";
                        return true;

                    case OrdenCargaControlCarga.VerificarPedido:
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "El pedido informado no existe.";
                        break;

                    case OrdenCargaControlCarga.VerificarCreditoDePedido:
                        ordenDeCarga.TransporteExiste = true;
                        ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "Verificar Crédito de pedido";
                        break;

                    case OrdenCargaControlCarga.PedidoEntregadoCompletamente:
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "El pedido ingresado ya fue entregado completamente.";
                        break;

                    case OrdenCargaControlCarga.CC06IdemCC01:
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "Error de carga.";
                        break;

                    case OrdenCargaControlCarga.FaltaCargarKmsEnContrato:
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "Faltan cargar los Km en el contrato.";
                        break;
                }
            }
            Log.Debug(this.GetType().Name, "VerificarOrden", $" actualizarEstado, inicial: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
            ordenDeCarga.ActualizarEstado();
            Log.Debug(this.GetType().Name, "VerificarOrden", $" actualizarEstado, final: " + EstadoOrdenDeCargaExtensions.ToFriendlyString(ordenDeCarga.Estado));
            return false;
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
                }
                if (esPuerto)
                {
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion);
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
                }
                Expression<Func<OrdenDeCarga, bool>> filtro = o => o.FechaCarga <= fechaFinDateTime
                    && o.FechaCarga >= fechaIncioDateTime
                    && filtrosEstados.Contains(o.Estado);
                var listadoConFiltro = repositorio.Listar<OrdenDeCarga>(filtro);


                //foreach (var item in listadoConFiltro)
                //{
                //    Log.Debug(this.GetType().Name, "Listar", $" item:[Id: {item.Id}, Cliente: {item.Cliente.CodigoProveedor}, RazonSocialCliente: {item.Cliente.RazonSocial}, Fecha: {item.FechaCarga}, CUITCliente: {item.CUITCliente}, Corredor: {item.CodigoCorredor}, RazonSocialCorredor: {item.Corredor?.RazonSocial}], ContratoSAP: {item.ContratoSAP}, ContratoIngresado: {item.ContratoIngresado}");
                //}

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
                        EdicionRechazada = x.EdicionRechazada
                    }).OrderByDescending(y => y.Id).ToList();
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                var listadoSinFiltro = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id) && n.FechaCarga <= fechaFinDateTime
                    && n.FechaCarga >= fechaIncioDateTime
                    && (n.Estado == EstadoOrdenDeCarga.Vencida
                        || n.Estado == EstadoOrdenDeCarga.ErrorDeCarga
                        || n.Estado == EstadoOrdenDeCarga.Pendiente
                        || n.Estado == EstadoOrdenDeCarga.Confirmado
                        || n.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito
                        || n.Estado == EstadoOrdenDeCarga.EntregaPendiente
                        || n.Estado == EstadoOrdenDeCarga.EntregaGenerada
                        || n.Estado == EstadoOrdenDeCarga.EdicionSolicitada
                        || n.Estado == EstadoOrdenDeCarga.AnulacionSolicitada
                        || n.Estado == EstadoOrdenDeCarga.ContratoVencido
                        || n.Estado == EstadoOrdenDeCarga.EdicionRechazada
                        || n.Estado == EstadoOrdenDeCarga.SinEnviarASAP
                        || n.Estado == EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion
                        )
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

            OrdenDeCargaDetalleDto ordenDto;

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

            if (orden == null) throw new InfoCustomException("No se encontro ningún orden de carga");

            Proveedor cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);
            //  var ordenDeCargaCambiosHistorial = repositorio.Listar<OrdenDeCargaCambiosHistorial>(ordenes => ordenes.OrdenDeCarga_Id == orden.Id);

            var ordenDeCargaCambiosHistorial = ObtenerCambiosHistorial(orden);

            ordenDto = OrdenDeCargaDetalleDto.DeOrdenDeCarga(orden, ordenDeCargaCambiosHistorial, cliente);

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
            NotificarVencimientoOrdenCarga(ordenes);

            return ordenes;
        }

        public void NotificarVencimientoOrdenCarga(List<OrdenDeCarga> ordenes)
        {

            var emailSenderData = ConstruirCuerpoOrdenesVencidas(ordenes);

            if (emailSenderData != null)
            {
                //if (!HttpContext.Current.IsDebuggingEnabled)
                //{
                EmailSender.EnviarMail(emailSenderData);
                //}
            }

        }

        public string NotificarVencimientoOrdenCarga(int ordenId, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var puedeEnviarASAP = usuario.TienePermiso("ENVIAR A SAP");
            var emailSenderData = new EmailSenderData();
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            var orden = repositorio.Obtener<OrdenDeCarga>(x => x.Id == ordenId);
            var mail = orden.Cliente.Mail;
            var titulo = $"Se informa que el día {DateTime.Now.ToString()} se ha vencido la siguiente orden de carga:";
            var cabecera = "Orden :";
            var ordenVencidas = new StringBuilder();

            if (puedeEnviarASAP)
                AnularOrdenSap(orden);

            ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
            emailSenderData.Asunto = $"Molinos Agro - Notificación de orden vencida- {orden.Cliente.RazonSocial}";
            emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, ordenVencidas, titulo, cabecera);

            emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mail, mailsComerciales, mailsMesaVentaFas });

            if (emailSenderData != null)
            {
                //if (!HttpContext.Current.IsDebuggingEnabled)
                //{
                EmailSender.EnviarMail(emailSenderData);
                //}
            }
            orden.Estado = EstadoOrdenDeCarga.AnuladaPorVencimiento;
            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAnulada;
        }
        public EmailSenderData ConstruirCuerpoOrdenesVencidas(List<OrdenDeCarga> ordenes)
        {
            var emailSenderData = new EmailSenderData();
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            var mailsAuditoriaOrdenesVencidas = ConfigurationManager.AppSettings["EmailToAuditoriaOrdenesVencidas"];
            var titulo = $"Se informa que el día {DateTime.Now.ToString()} se han vencido las siguientes ordenes de carga:";
            var cabecera = "Ordenes:";
            try
            {
                if (string.IsNullOrEmpty(mailsComerciales))
                {
                    return null;
                }
                emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsAuditoriaOrdenesVencidas });
                if (emailSenderData.Mails.Count == 0)
                {
                    return null;
                }
                var ordenVencidas = new StringBuilder();
                foreach (var orden in ordenes)
                {
                    ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
                }
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
                emailSenderData.Asunto = $"Molinos Agro - Notificación de ordenes Vencidas";
                if (ordenes.Count == 0)
                {
                    emailSenderData.Cuerpo = $"Se informa que para el dia {DateTime.Now} no hay ordenes de carga vencidas";
                }
                else
                {
                    emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), ordenes[0].Id, ordenVencidas, titulo, cabecera);
                }

                return emailSenderData;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
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
                repositorio.GuardarCambios();

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
                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(string.Empty, string.Empty, request.Corredor, request.FechaInicio, request.FechaFin, string.Empty, request.Pendiente, string.Empty, 1);
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
        //private string ValidateVisualizarClienteRequest(VisualizarClienteRequest request)
        //{
        //    string validation = string.Empty;
        //    if (string.IsNullOrEmpty(request.Corredor))
        //    {
        //        validation = "Corredor";
        //    }
        //    if (string.IsNullOrEmpty(request.Pendiente))
        //    {
        //        validation = "Pendiente";
        //    }
        //    return validation;
        //}
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
                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(string.Empty, request.Contrato, string.Empty, request.FechaInicio, request.FechaFin, string.Empty, request.Pendiente, string.Empty, 2);
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
        //private string ValidateVisualizarProductoRequest(VisualizarProductoRequest request)
        //{
        //    string validation = string.Empty;
        //    if (string.IsNullOrEmpty(request.Contrato))
        //    {
        //        validation = "Contrato";
        //    }
        //    return validation;
        //}
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
                //var validateValidarCorredorClienteContratoProductoRequest = ValidateValidarCorredorClienteContratoProductoRequest(request);
                //if (!string.IsNullOrEmpty(validateValidarCorredorClienteContratoProductoRequest))
                //{
                //    throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, validateValidarCorredorClienteContratoProductoRequest));
                //}
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
                var ordenCargaVisualizarClienteWSMOAResponse = OrdenCargaVisualizarCliente(request.ClienteCodigo, request.Contrato, request.Corredor, request.FechaInicio, request.FechaFin, producto.CodigoSap, request.Pendiente, string.Empty, 3);
                //Log.Debug(this.GetType().Name, "ValidarCorredorClienteContratoProducto", $" ordenCargaVisualizarClienteWSMOAResponse: { ordenCargaVisualizarClienteWSMOAResponse.ToJson() }");
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
        //private string ValidateValidarCorredorClienteContratoProductoRequest(ValidarCorredorClienteContratoProductoRequest request)
        //{
        //    string validation = string.Empty;
        //    if (string.IsNullOrEmpty(request.ClienteCuit))
        //    {
        //        validation = "ClienteCuit";
        //    }
        //    if (string.IsNullOrEmpty(request.Contrato))
        //    {
        //        validation = "Contrato";
        //    }
        //    if (string.IsNullOrEmpty(request.ProductoId))
        //    {
        //        validation = "ProductoId";
        //    }
        //    return validation;
        //}
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

        private OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarCliente(string cliente, string contrato, string corredor, string fechaInicio, string fechaFin, string material, string pendiente, string tipoContrato, int type)
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
            Log.Info("SeleccionarContrato ActualizarEstado " + orden.ToDto().ToJson());
            orden.ActualizarEstado();
            Log.Info("SeleccionarContrato ActualizarEstado Nuevo " + orden.Estado.ToString());
            if (!ValidarVencimientoContrato(contratoSAP, orden.Cliente))
            {
                orden.Estado = EstadoOrdenDeCarga.ContratoVencido;
                repositorio.GuardarCambios();
                return new Resultado { error = "El contrato seleccionado esta vencido" };
            }
            if (!string.IsNullOrEmpty(orden.ContratoSAP))
            {
                var puedeCrear = VerificarOrden(orden, orden.Cliente, false, true);

                if (orden.CodigoVerificacionSap != "CC-07")
                {
                    var creadaEnSaP = CrearPedidoEnSAP(orden, orden.Cliente, true, false, mailUsuario);

                    if (creadaEnSaP)
                    {
                        return VerificarSituacionCrediticia(orden, true);
                    }
                }

            }
            else
            {
                resultado = VerificarTransporte(orden);
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

            if (string.IsNullOrEmpty(orden.ContratosRespuesta))
            {
                throw new ValidationCustomException("La orden no tiene contratos disponibles para seleccionar.");
            }

            var listadoContratos = orden.ContratosRespuesta.Split(',').ToList();

            return listadoContratos;
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
                var aprobadoCredito = ObtenerSituacionCrediticia(orden);
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
                return "El transporte no existe";
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
            Log.Info("TransporteExiste");
            var result = consumer.OrdenCargaControlEstadoRequest("", "", orden.CUITTransporte);

            return result == "CE-07";
        }
        public string NotificarTransporte(int ordenDeCargaId)
        {
            string mensaje = "";

            try
            {
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);

                if (!TransporteExiste(orden))
                {
                    string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
                    string mailsMesaENTSL = ConfigurationManager.AppSettings["EmailToMesaENTSL"];

                    var mails = CargarYObtenerMailsDestino(new List<string> { }, new List<string> { mailsMesaVentaFas, mailsMesaENTSL });

                    string asunto = "ALTA TTE";

                    string cuerpo = string.Format("Razón Social: {0} <br> CUIT: {1}", orden.RazonSocialTransporte, orden.CUITTransporte);

                    //if (!HttpContext.Current.IsDebuggingEnabled)
                    //{
                    EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);
                    //}
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
                        //Notificar situacion crediticia
                        var emailSenderData = ConstruirCuerpoEmail(orden);
                        if (emailSenderData != null)
                        {
                            //if (!HttpContext.Current.IsDebuggingEnabled)
                            //{
                            EmailSender.EnviarMail(emailSenderData);
                            //}
                        }
                    }

                    Log.Info("VerificarSituacionCrediticia ActualizarEstado " + orden.ToDto().ToJson());
                    orden.ActualizarEstado();
                    Log.Info("VerificarSituacionCrediticia ActualizarEstado Nuevo " + orden.Estado.ToString());
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
                    Log.Info("VerificarSituacionCrediticia ActualizarEstado " + orden.ToDto().ToJson());
                    orden.ActualizarEstado();
                    Log.Info("VerificarSituacionCrediticia ActualizarEstado Nuevo " + orden.Estado.ToString());
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
            Log.Info("ObtenerSituacionCrediticia");
            var result = consumer.OrdenCargaControlEstadoRequest("", orden.NumeroPedido, "");

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
        public EmailSenderData ConstruirCuerpoEmail(OrdenDeCarga orden)
        {
            var emailSenderData = new EmailSenderData();
            var ordenVencidas = new StringBuilder();
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            var mailsCobranzas = ConfigurationManager.AppSettings["EmailToCobranzas"];
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            string titulo = $"Se informa que la siguiente orden de carga no pasó las validaciones crediticias.";
            var cabecera = "Orden :";
            try
            {
                if (string.IsNullOrEmpty(mailsMesaVentaFas) &&
                    string.IsNullOrEmpty(mailsCobranzas) &&
                    string.IsNullOrEmpty(mailsComerciales))
                {
                    return null;
                }

                emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas, mailsCobranzas });

                if (emailSenderData.Mails.Count == 0)
                {
                    return null;
                }

                var cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);
                if (cliente == null)
                {
                    return null;
                }

                var contrato = (string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP) ?? string.Empty;
                if (string.IsNullOrEmpty(contrato))
                {
                    return null;
                }

                var pedido = (string.IsNullOrEmpty(orden.PedidoSAP) ? (string.IsNullOrEmpty(orden.NumeroPedido) ? orden.NumeroPedidoIngresado : orden.NumeroPedido) : orden.PedidoSAP) ?? string.Empty;
                if (string.IsNullOrEmpty(pedido))
                {
                    return null;
                }


                ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
                emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, ordenVencidas, titulo, cabecera);
                emailSenderData.Asunto = $"Orden de carga #{orden.Id}  Pedido Bloqueado {orden.Cliente.RazonSocial}";
                return emailSenderData;
            }
            catch
            {
                return null;
            }
        }
        private Resultado GenerarEntregaSAP(OrdenDeCarga orden)
        {
            Log.Info("GenerarEntregaSAP");

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

            var result = consumer.CrearEntrega(req, out string respuesta);

            Log.Info("GenerarEntregaSAP CrearEntrega Result " + result);
            Log.Info("GenerarEntregaSAP CrearEntrega Respuesta " + respuesta);

            //OE-00   'OK'
            //OE-01   'No existe tranportista
            //OE-02   'Entrega Creada - Error al insertar'
            //OE-03   'Entrega Creada - Error al insertar'
            //OE-04   'Falta cargars los Km en el contrato'

            switch (respuesta)
            {
                case "OE-00":
                case "OE-02":
                case "OE-03":
                    orden.TransporteExiste = true;
                    orden.FechaEntregaGenerada = DateTime.Now;
                    orden.NumeroEntrega = result;
                    orden.DescripcionCodigoVerificacionSap = "";
                    Log.Info("GenerarEntregaSAP ActualizarEstado " + orden.ToDto().ToJson());
                    orden.ActualizarEstado();
                    Log.Info("GenerarEntregaSAP ActualizarEstado Nuevo " + orden.Estado.ToString());
                    repositorio.GuardarCambios();
                    return new Resultado { Mensaje = string.Concat("Se ha generado la entrega ", result, ".") };
                //return string.Concat("Se ha generado la entrega ", result, ".");

                case "OE-01":
                    orden.TransporteExiste = false;
                    orden.DescripcionCodigoVerificacionSap = "No se pudo generar la entrega. No existe el transportista.";
                    Log.Info("GenerarEntregaSAP ActualizarEstado " + orden.ToDto().ToJson());
                    orden.ActualizarEstado();
                    Log.Info("GenerarEntregaSAP ActualizarEstado Nuevo " + orden.Estado.ToString());
                    repositorio.GuardarCambios();
                    return new Resultado { Mensaje = "No se pudo generar la entrega. No existe el transportista." };
                    //return string.Concat("No se pudo genera la entrega. No existe el transportista.");
            }
            orden.DescripcionCodigoVerificacionSap = respuesta;
            return new Resultado { info = "Estado no conocido" };
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
        public string NotificarVariosContratos(EmailSenderData emailSenderData)
        {
            string mensaje = "";
            try
            {
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

        public EmailSenderData ConstruirCuerpoMailNotificacionVariosContratos(int ordenDeCargaId)
        {
            var emailSenderData = new EmailSenderData();
            var ordenVencidas = new StringBuilder();
            string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
            emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas });
            string titulo = "Se encontraron varios contratos para el mismo cliente";
            var cabecera = "Orden :";
            ordenVencidas.Append($"<tr><td>{orden.Id}</td><td>{orden.ContratoIngresado}</td><td>{orden.Cliente.RazonSocial}</td><td>{orden.CodigoCorredor}</td><td>{orden.NombreChofer}</td><td>{orden.ChasisAcoplado}</td><td>{orden.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedido : orden.PedidoSAP)}</td><td>{orden.NumeroEntrega}</td><td>{orden.FechaCarga}</td><td>{orden.FechaVencimiento}</td></tr>");
            emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), orden.Id, ordenVencidas, titulo, cabecera);
            emailSenderData.Asunto = $"Varios ctto pendientes - {orden.Cliente.RazonSocial}";
            return emailSenderData;
        }


        public string NotificacionContratoVencido(EmailSenderData emailSenderData)
        {
            if (emailSenderData != null)
            {
                EmailSender.EnviarMail(emailSenderData);
            }

            return "Notificación enviada";
        }

        public EmailSenderData ConstruirCuerpoMailNotificacionContratoVencido(OrdenDeCarga ordenDeCarga)
        {
            var emailSenderData = new EmailSenderData();
            var ordenVencidas = new StringBuilder();
            var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE_ORDENES);
            string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            emailSenderData.Mails = CargarYObtenerMailsDestino(emailSenderData.Mails, new List<string>() { mailsComerciales, mailsMesaVentaFas });
            string titulo = "Contrato vencido Nro :" + ordenDeCarga.ContratoIngresado;
            var cabecera = "Orden :";
            ordenVencidas.Append($"<tr><td>{ordenDeCarga.Id}</td><td>{ordenDeCarga.ContratoIngresado}</td><td>{ordenDeCarga.Cliente.RazonSocial}</td><td>{ordenDeCarga.CodigoCorredor}</td><td>{ordenDeCarga.NombreChofer}</td><td>{ordenDeCarga.ChasisAcoplado}</td><td>{ordenDeCarga.PatenteAcoplado}</td><td>{(string.IsNullOrEmpty(ordenDeCarga.PedidoSAP) ? ordenDeCarga.NumeroPedido : ordenDeCarga.PedidoSAP)}</td><td>{ordenDeCarga.NumeroEntrega}</td><td>{ordenDeCarga.FechaCarga}</td><td>{ordenDeCarga.FechaVencimiento}</td></tr>");
            emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), ordenDeCarga.Id, ordenVencidas, titulo, cabecera);
            emailSenderData.Asunto = $"Contrato Vencido - {ordenDeCarga.Cliente.RazonSocial}";
            return emailSenderData;
        }

        public bool ValidarVencimientoContrato(string contrato, Proveedor cliente)
        {

            var request = new OrdenCargaVisualizarClienteWSMOARequest()
            {
                Cliente = cliente.CodigoProveedor,
                Contrato = contrato
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

        public ObtenerContratosDisponiblesResponse ObtenerContratosDisponibles(ObtenerContratosDisponiblesRequest req)
        {
            try
            {
                var rangoFechas = string.IsNullOrEmpty(req.FechaDesde) || string.IsNullOrEmpty(req.FechaHasta) ? null :
                    CommonService.toDateList(req.FechaDesde, req.FechaHasta);

                var consumerReq = new OrdenCargaVisualizarClienteWSMOARequest
                {
                    Cliente = req.ClienteCodigo,
                    Contrato = string.Empty,
                    Corredor = req.CorredorCodigo,
                    Fechas = rangoFechas,
                    Material = "",
                    Pendiente = "X", // "X" es para Contratos ABIERTOS
                    TipoContrato = "N"
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

                var contratosDisponiblesResp = new ObtenerContratosDisponiblesResponse
                {
                    Contratos = consumerRes.Resultados.Select(x => new ContratoOrdenFas
                    {
                        NumeroContrato = x.Contrato,
                        Producto = productosBD
                            .Where(p => p.CodigoSap == x.Producto.Trim().TrimStart('0'))
                            .Select(p => new MaterialDto
                            {
                                MaterialId = p.Id,
                                Descripcion = p.Nombre,
                                CodigoSap = p.CodigoSap
                            })
                            .Single()
                    }).ToList()
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
            var controlarCargaReq = new ControlCargaRequest
            {
                Cliente = clienteCodigo,
                Corredor = corredorCodigo.StartsWith("C") ? corredorCodigo : "",
                SoloSisa = true
            };

            var responseHandler = consumer.ControlarCarga(controlarCargaReq);

            var res = new ValidarSisaCorredorClienteResponse
            {
                ClienteHabilitadoEnSisa = !responseHandler.TieneRespuesta(OrdenCargaControlCarga.ClienteInhabilitadoEnSisa),
                CorredorHabilitadoEnSisa = !responseHandler.TieneRespuesta(OrdenCargaControlCarga.CorredorInhabilitadoEnSisa)
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
                result = !responseHandler.TieneRespuesta(OrdenCargaControlCarga.DestinatarioInhabilitadoEnSisa);
            else if (validaDestino)
                result = !responseHandler.TieneRespuesta(OrdenCargaControlCarga.DestinoInhabilitadoEnSisa);

            return result;
        }
        public ValidarCuitExisteScatoResponse ValidarCuitExisteScato(string cuit)
        {

            var result = scatoConsumer.ExisteCuitDestinoDestinatario(cuit);

            return result;
        }
        public bool EmailGestionarAlta(string cuit, string razonSocial)
        {
            string mailsGestion = ConfigurationManager.AppSettings["EmailToGestionAltaCuit"];

            string mailsCopiaGestion = ConfigurationManager.AppSettings["CopiaEmailToGestionAltaCuit"];
            var mails = CargarYObtenerMailsDestino(new List<string> { }, new List<string> { mailsGestion });
            var copias = CargarYObtenerMailsDestino(new List<string> { }, new List<string> { mailsCopiaGestion });

            string asunto = "ALTA TEMPRANA CUIT";

            string cuerpo = string.Format("Se solicita el alta temprana del CUIT: {0} , Razón Social: {1}", cuit, razonSocial);

            EmailSender.EnviarMail(mails, asunto, cuerpo, copias, null, null, null);

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
        private Proveedor GetClienteParaCorredor(Usuario usuario, Proveedor corredor, OrdenDeCarga ordenDeCarga)
        {
            corredor = usuario.ObtenerCorredor();
            ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
            ordenDeCarga.Corredor_Id = corredor.Id;
            ordenDeCarga.CUITCorredor = corredor.CUIT;
            var cliente = usuario.Proveedores.Where(prov => prov.CUIT == ordenDeCarga.CUITCliente && prov.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente).First();

            return cliente;
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
            var resultadoAnularEntrega = consumer.AnularEntregaOrdenCarga(orden.NumeroEntrega);
            if (resultadoAnularEntrega.HayError)
                //Pendiente revisión de los mensajes acorde a las verdaderas razones de error
                throw new InfoCustomException("La entrega está tomada en SAP");
            else
            {
                orden.Estado = EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion;
                orden.NumeroEntrega = null;
                orden.FechaEntregaGenerada = null;
                repositorio.GuardarCambios();
                Thread.Sleep(5000);
            }
        }
        private void AnularPedidoEnSap(OrdenDeCarga orden, bool tieneNumeroEntrega)
        {
            var tieneNumeroPedido = !string.IsNullOrEmpty(orden.NumeroPedidoIngresado) || !string.IsNullOrEmpty(orden.NumeroPedido);
            if (!tieneNumeroPedido)
                return;
            var resultadoAnularOrden = consumer.AnularOrdenCarga(orden);
            if (resultadoAnularOrden.HayError)
                //Pendiente revisión de los mensajes acorde a las verdaderas razones de error
                throw new InfoCustomException("El pedido está tomado en SAP");
        }
        private List<string> ObtenerContratosAbiertos(List<string> contratos)
        {
            return contratos.Where(contrato => consumer.VerificarContratoAbierto(contrato)).ToList();
        }
        private void TieneVariosContratosAbiertos(OrdenDeCarga ordenDeCarga, List<string> contratosAbiertos)
        {
            var result = string.Join(",", contratosAbiertos);
            if (string.IsNullOrEmpty(ordenDeCarga.NumeroPedido))
            {
                if (string.IsNullOrEmpty(ordenDeCarga.ContratoSAP))
                {
                    ordenDeCarga.ContratosRespuesta = result;
                    ordenDeCarga.ContratoSAP = "";
                    ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios contratos pendientes para el mismo cliente. Seleccione el contrato para generar entregas desde el botón \"Contratos\".";
                }
            }
            else
            {
                ordenDeCarga.PedidosRespuesta = result;
                ordenDeCarga.NumeroPedido = "";
                ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios pedidos pendientes para el mismo cliente. Seleccione el pedido para generar entregas desde el botón \"Pedidos\".";

            }
        }
        private void ValidarCuilChoferEnScato(string cuil)
        {
            try
            {
                var result = scatoConsumer.CuilChoferExiste(cuil);
            }
            catch (Exception err)
            {
                Log.Debug("OrdenDeCarga Controller", string.Format("Error al Validar CUIL: {0}", cuil), err.Message);
            }


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
        }
    }
}
