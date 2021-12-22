using NLog.Fluent;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Util;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaService : IOrdenDeCargaService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IOrdenCargaConsumerMOA consumer;

        private static readonly string EMAIL_TEMPLATE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "AvisoEdicionOrdenDeCarga.html");

        public OrdenDeCargaService(IRepositorio repositorio, IOrdenCargaConsumerMOA consumer)
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
        }

        public Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Proveedor cliente;
            Proveedor corredor;
            ordenDeCarga.Estado = EstadoOrdenDeCarga.ErrorDeCarga;

            var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");

            if (esComercial)
            {
                cliente = repositorio.Obtener<Proveedor>(x => x.CUIT == ordenDeCarga.CUITCliente && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == 5);
                corredor = repositorio.Obtener<Proveedor>(x => x.CUIT == ordenDeCarga.CUITCorredor && x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == 4);

                if (corredor != null)
                {
                    ordenDeCarga.CodigoCorredor = corredor.CodigoProveedor;
                    ordenDeCarga.Corredor_Id = corredor.Id;
                }
                else
                {
                    if (ordenDeCarga.CUITCorredor != null)
                    {
                        throw new ValidationCustomException("No se encontró el corredor seleccionado");
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
                    cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);

                    if (cliente == null)
                    {
                        throw new ValidationCustomException("Su usuario no está habilitado para operar con ese CUIT");
                    }
                }
                else
                {
                    ordenDeCarga.CodigoCorredor = "";
                    cliente = usuario.ObtenerProveedor();
                    ordenDeCarga.CUITCliente = cliente.CUIT;
                    ordenDeCarga.CUITCorredor = "";
                }
            }

            ordenDeCarga.UsuarioCreacion_Id = usuario.Id;
            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.Cliente_Id = cliente.Id;
            ordenDeCarga.ContratoSinCantidadPendiente = false;

            var producto = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);

            ordenDeCarga.Producto = producto;
            ordenDeCarga.NumeroPedido = string.IsNullOrEmpty(ordenDeCarga.NumeroPedidoIngresado) ? "" : ordenDeCarga.NumeroPedidoIngresado;
            ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
            ordenDeCarga.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;

            ordenDeCarga.Cantidad = int.Parse(ConfigurationManager.AppSettings["CantidadOrdenDeCarga"]);

            ordenDeCarga.TransporteExiste = TransporteExiste(ordenDeCarga);

            var crearPedido = VerificarOrden(ordenDeCarga, cliente);

            repositorio.Agregar(ordenDeCarga);

            repositorio.GuardarCambios();

            if (crearPedido)
            {
                ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;

                var creadaEnSaP = CrearOrdenEnSAP(ordenDeCarga, cliente, false);

                if (creadaEnSaP)
                {
                    VerificarSituacionCrediticia(ordenDeCarga, true);
                }
            }

            NotificarTransporte(ordenDeCarga.Id);

            return new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaAgregada };
        }

        public Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var valoresAEditar = new List<string> { "NombreChofer", "ApellidoChofer", "CUITChofer", "PatenteAcoplado", "ChasisAcoplado", "ContratoIngresado",
            "NumeroPedido", "Observacion", "Cantidad", "RazonSocialTransporte", "CUITTransporte", "Producto_Id", "NumeroPedidoIngresado" };
            var ordenEditar = repositorio.Obtener<OrdenDeCarga>(ordenDeCarga.Id);
            var listaValoresDiferentes = ordenEditar.Compare(ordenDeCarga);
            var historialCambios = new List<OrdenDeCargaCambiosHistorial>() { };

            ordenEditar.NombreChofer = ordenDeCarga.NombreChofer;
            ordenEditar.ApellidoChofer = ordenDeCarga.ApellidoChofer;
            ordenEditar.CUITChofer = ordenDeCarga.CUITChofer;
            ordenEditar.PatenteAcoplado = ordenDeCarga.PatenteAcoplado;
            ordenEditar.ChasisAcoplado = ordenDeCarga.ChasisAcoplado;
            ordenEditar.RazonSocialTransporte = ordenDeCarga.RazonSocialTransporte;
            ordenEditar.CUITTransporte = ordenDeCarga.CUITTransporte;

            ordenEditar.ContratoIngresado = ordenDeCarga.ContratoIngresado;
            ordenEditar.Cantidad = ordenDeCarga.Cantidad;
            ordenEditar.Producto_Id = ordenDeCarga.Producto_Id;
            ordenEditar.NumeroPedidoIngresado = ordenDeCarga.NumeroPedidoIngresado;
            ordenEditar.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;

            if (!ordenEditar.InformadaSAP || listaValoresDiferentes.Exists(x => x.PropertyName == "ContratoIngresado"))
            {
                var crearPedido = VerificarOrden(ordenEditar, ordenEditar.Cliente);

                if (crearPedido)
                {
                    ordenEditar.ContratoSAP = ordenEditar.ContratoIngresado;

                    var creadaEnSaP = CrearOrdenEnSAP(ordenEditar, ordenEditar.Cliente, false);

                    if (creadaEnSaP)
                    {
                        VerificarSituacionCrediticia(ordenEditar, true);
                    }
                }
            }

            ordenEditar.Observacion = ordenDeCarga.Observacion;
            ordenDeCarga.TransporteExiste = TransporteExiste(ordenDeCarga);

            foreach (var prop in listaValoresDiferentes)
            {

                if (valoresAEditar.Contains(prop.PropertyName))
                {
                    var registroHistorial = new OrdenDeCargaCambiosHistorial();

                    registroHistorial.Id = 0;
                    registroHistorial.Antes = prop.valA != null ? prop.valA?.ToString() : "-";
                    registroHistorial.Despues = prop.valB != null ? prop.valB?.ToString() : "-";
                    registroHistorial.NombreColumnaCambio = prop.PropertyName;
                    registroHistorial.FechaCambio = DateTime.Now;
                    registroHistorial.Usuario_Id = usuario.Id;
                    registroHistorial.OrdenDeCarga_Id = ordenDeCarga.Id;

                    historialCambios.Add(registroHistorial);
                }
            }

            repositorio.AgregarTodos(historialCambios);
            ordenEditar.HistorialCambios.Concat(historialCambios);
            repositorio.GuardarCambios();

            NotificarTransporte(ordenEditar.Id);

            if (historialCambios.Count > 0)
            {
                EnviarMailEdicionOrdenDeCarga(historialCambios);
            }

            return new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
        }

        private void EnviarMailEdicionOrdenDeCarga(List<OrdenDeCargaCambiosHistorial> ordenDeCargaHistorial)
        {
            try
            {
                var cambios = new StringBuilder();

                foreach (var cambio in ordenDeCargaHistorial)
                {
                    cambios.AppendLine($"<tr><td>{cambio.NombreColumnaCambio}</td><td>{cambio.Antes}</td><td>{cambio.Despues}</td><td>{cambio.FechaCambio}</td></tr>");
                }

                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                var cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), ordenDeCargaHistorial[0].OrdenDeCarga_Id, cambios);
                string asunto = "Molinos Agro - Edición en su orden de carga n°: " + ordenDeCargaHistorial[0].OrdenDeCarga_Id;
                var copia = new List<string>() { };
                var Destinatario = ConfigurationManager.AppSettings["EmailToComerciales"].Split(';').ToList();

                EmailSender.EnviarMail(Destinatario, asunto, cuerpo, copia, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private bool CrearOrdenEnSAP(OrdenDeCarga orden, Proveedor cliente, bool forzarCreacion)
        {
            //OV-01   'Verificar Contrato, Material, Cliente'
            //OV-02   'Verificar cantidad pendiente de Contratada'
            //OV-03   'Pedido creado - Verificar Crédito de pedido'
            //OV-00   'OK'
            var forzarCreacionStr = forzarCreacion ? "" : "X";

            var result = consumer.CrearOrdenRequest(cliente.CodigoProveedor, orden.ContratoSAP, orden.CodigoCorredor, orden.Cantidad, orden.Producto.CodigoSap, orden.NumeroPedidoIngresado, forzarCreacionStr, out string numeroPedido);

            var resultadoCrearOrden = false;
            orden.ContratoSinCantidadPendiente = false;

            //var result2 = consumer.OrdenCargaEntregadaRequest(orden.CUITChofer, orden.Cantidad, orden.NombreChofer, orden.PatenteAcoplado, orden.ChasisAcoplado, "", "DNI", orden.CUITTransporte, out string mensaje);
            if (result == "OV-00" || result == "OV-03")
            {
                orden.InformadaSAP = true;
                orden.NumeroPedido = numeroPedido;
                orden.DescripcionErrorInterno = "";

                //if (result == "OV-03")
                //{
                //    orden.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
                //}

                resultadoCrearOrden = true;
            }
            else
            {
                orden.CodigoVerificacionSap = result;
                if (result == "OV-02")
                {
                    orden.ContratoSinCantidadPendiente = true;
                    orden.CodigoVerificacionSap = "CC-01";
                    orden.DescripcionErrorInterno = "El contrato ingresado tiene menos de 15 toneladas disponibles. Puede elegir forzar la creación del pedido desde \"Crear pedido\" o anularlo.";
                }
                else
                {
                    orden.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                }

            }

            orden.ActualizarEstado();
            repositorio.GuardarCambios();

            return resultadoCrearOrden;
        }

        private bool VerificarOrden(OrdenDeCarga ordenDeCarga, Proveedor cliente)
        {
            /* 
            CC-01	'Más de un contrato vigente para Cliente/Corredor'
            CC-02	'Transportista no dado de alta'
            CC-03	'Verificar Pedido' 
            CC-04	'Verificar Crédito de pedido'
            CC-05	'Pedido entregado completamente'
            CC-00	'OK'
            */
            var result = consumer.ControlCargaRequest(cliente.CodigoProveedor, ordenDeCarga.ContratoSAP, ordenDeCarga.CodigoCorredor, ordenDeCarga.CUITTransporte, ordenDeCarga.Producto.CodigoSap, ordenDeCarga.NumeroPedido);

            //Existe la posibilidad de que el cliente tenga varios contratos abiertos con molinos. En caso de tener una "," un comercial debe seeccionar
            //cual es el contrato correcto que le quiere entregar.
            if (result.Contains(','))
            {
                if (string.IsNullOrEmpty(ordenDeCarga.NumeroPedido))
                {
                    ordenDeCarga.ContratosRespuesta = result;
                    ordenDeCarga.ContratoSAP = "";
                    ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios contratos pendientes para el mismo cliente. Seleccione el contrato para generar entregas desde el botón \"Contratos\".";
                }
                else
                {
                    ordenDeCarga.PedidosRespuesta = result;
                    ordenDeCarga.NumeroPedido = "";
                    ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios pedidos pendientes para el mismo cliente. Seleccione el pedido para generar entregas desde el botón \"Pedidos\".";
                }

                ordenDeCarga.ActualizarEstado();
            }
            else
            {
                switch (result)
                {
                    case "CC-00":
                        ordenDeCarga.TransporteExiste = true;
                        ordenDeCarga.CorredorSeleccionado = true;
                        ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                        ordenDeCarga.CodigoVerificacionSap = "CC-00";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "OK";
                        return true;

                    case "CC-01":
                        //ordenDeCarga.CorredorSeleccionado = false;
                        //break;
                        ordenDeCarga.CodigoVerificacionSap = "CC-01";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "No se encontró ningun contrato con ese producto.";
                        break;


                    case "CC-02":
                        ordenDeCarga.TransporteExiste = false;
                        ordenDeCarga.CodigoVerificacionSap = "CC-02";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "Transportista no dado de alta";
                        return true;

                    case "CC-03":
                        ordenDeCarga.CodigoVerificacionSap = "CC-03";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "El pedido informado no existe.";
                        break;

                    case "CC-04":
                        ordenDeCarga.TransporteExiste = true;
                        ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                        ordenDeCarga.CodigoVerificacionSap = "CC-04";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "Verificar Crédito de pedido";
                        break;

                    case "CC-05":
                        ordenDeCarga.CodigoVerificacionSap = "CC-05";
                        ordenDeCarga.DescripcionCodigoVerificacionSap = "El pedido ingresado ya fue entregado completamente.";
                        break;
                }
            }

            ordenDeCarga.ActualizarEstado();
            return false;
        }

        public List<OrdenDeCargaDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
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

            var esInterno = (esAdmin || esComercial || esMesaFas || esPuerto);

            fechaFinDateTime = fechaFinDateTime.AddDays(1);

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            if (esInterno)
            {

                var filtrosEstados = new List<EstadoOrdenDeCarga>();
                if (esMesaFas)
                {
                    filtrosEstados.Add(EstadoOrdenDeCarga.Confirmado);
                    filtrosEstados.Add(EstadoOrdenDeCarga.PendienteAprobacionCredito);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaPendiente);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
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
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaGenerada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Anulada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Entregada);
                    filtrosEstados.Add(EstadoOrdenDeCarga.Vencida);
                    filtrosEstados.Add(EstadoOrdenDeCarga.EntregaPendiente);
                    filtrosEstados.Add(EstadoOrdenDeCarga.AnuladaPorVencimiento);
                    filtrosEstados.Add(EstadoOrdenDeCarga.ErrorDeCarga);
                }

                Expression<Func<OrdenDeCarga, bool>> filtro =
                    o => o.FechaCarga <= fechaFinDateTime
                    && o.FechaCarga >= fechaIncioDateTime
                    && filtrosEstados.Contains(o.Estado);

                listado = repositorio.Listar<OrdenDeCarga>
                        (filtro)
                    .Select(x => new OrdenDeCargaDto
                    {
                        Id = x.Id,
                        Cliente = x.Cliente.CodigoProveedor,
                        RazonSocialCliente = x.Cliente.RazonSocial,
                        Fecha = x.FechaCarga.ToString("dd/MM/yyyy HH:mm"),
                        CUITCliente = x.CUITCliente,
                        Corredor = x.CodigoCorredor,
                        RazonSocialCorredor = string.IsNullOrWhiteSpace(x.Corredor?.RazonSocial) ? "-" : x.Corredor?.RazonSocial,
                        Contrato = x.ContratoSAP ?? "-",
                        Pedido = x.NumeroPedido ?? "-",
                        Entrega = x.NumeroEntrega ?? "-",
                        Material = x.Producto.Nombre,
                        DescripcionEstado = x.Estado.ToFriendlyString(),
                        ColorSemaforo = x.Estado.ObtenerSemaforo(),
                        EsFacturaAnticipada = (x.NumeroPedidoIngresado != null),
                        PatenteChasis = x.ChasisAcoplado
                    }).OrderByDescending(y => y.Id).ToList();
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id) && n.FechaCarga <= fechaFinDateTime
                    && n.FechaCarga >= fechaIncioDateTime)
                    .Select(x => new OrdenDeCargaDto
                    {
                        Id = x.Id,
                        CUITCliente = x.CUITCliente,
                        Fecha = x.FechaCarga.ToString("dd/MM/yyyy HH:mm"),
                        Contrato = x.ContratoSAP ?? "-",
                        Pedido = x.NumeroPedido ?? "-",
                        Entrega = x.NumeroEntrega ?? "-",
                        Material = x.Producto.Nombre,
                        DescripcionEstado = x.Estado.ToUserFriendlyString(),
                        EsFacturaAnticipada = (x.NumeroPedidoIngresado != null),
                        PatenteChasis = x.ChasisAcoplado

                    }).OrderByDescending(y => y.Id).ToList();
            }

            if (listado == null || listado.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de cargas"));
            }

            return listado;
        }

        public OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            OrdenDeCargaDetalleDto ordenDto;
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

            Proveedor cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);

            ordenDto = new OrdenDeCargaDetalleDto
            {
                Id = orden.Id,
                CUITCliente = orden.CUITCliente,
                DescripcionEstado = orden.Estado.ToFriendlyString(),
                DescripcionEstadoUsuarioFinal = orden.Estado.ToUserFriendlyString(),
                ColorSemaforo = orden.Estado.ObtenerSemaforo(),
                ContratoIngresado = orden.ContratoIngresado,
                NumeroPedidoIngresado = orden.NumeroPedidoIngresado,
                Cliente = orden.Cliente.CodigoProveedor,
                AprobadoCredito = orden.AprobadoCredito,
                Cantidad = orden.Cantidad,
                ChasisAcoplado = orden.ChasisAcoplado,
                Chofer = $"{orden.ApellidoChofer}, {orden.NombreChofer} ({orden.CUITChofer})",
                ContratoSAP = string.IsNullOrEmpty(orden.ContratoSAP) ? "-" : orden.ContratoSAP,
                PedidoSAP = string.IsNullOrEmpty(orden.PedidoSAP) ? "-" : orden.PedidoSAP,
                Corredor = orden.CodigoCorredor,
                RazonSocialCorredor = string.IsNullOrWhiteSpace(orden.Corredor?.RazonSocial) ? "-" : orden.Corredor?.RazonSocial,
                CorredorSeleccionado = orden.CorredorSeleccionado,
                Estado = (int)orden.Estado,
                FechaCarga = orden.FechaCarga.ToString("dd/MM/yyyy hh:mm"),
                FechaEntregaGenerada = orden.FechaEntregaGenerada?.ToString("dd/MM/yyyy hh:mm"),
                InformadaSAP = orden.InformadaSAP,
                Observacion = orden.Observacion,
                PatenteAcoplado = orden.PatenteAcoplado,
                RazonSocialCliente = cliente.RazonSocial,
                Transporte = $"{orden.RazonSocialTransporte} ({orden.CUITTransporte})",
                TransporteExiste = orden.TransporteExiste,
                Producto = orden.Producto.Nombre,
                PedidosRespuesta = string.IsNullOrEmpty(orden.PedidosRespuesta) ? "-" : orden.PedidosRespuesta,
                NumeroEntrega = string.IsNullOrEmpty(orden.NumeroEntrega) ? "-" : orden.NumeroEntrega,
                NumeroPedido = string.IsNullOrEmpty(orden.NumeroPedido) ? "-" : orden.NumeroPedido,
                MensajeValidacionSAP = string.IsNullOrEmpty(orden.DescripcionCodigoVerificacionSap) ? "" : orden.DescripcionCodigoVerificacionSap,
                ContratoSinCantidadPendiente = orden.ContratoSinCantidadPendiente,
                DescripcionErrorInterno = string.IsNullOrEmpty(orden.DescripcionErrorInterno) ? "" : orden.DescripcionErrorInterno
            };

            return ordenDto;
        }

        public List<OrdenDeCarga> VerificarVencimientoOrdenDeCarga()
        {
            //TODO: Validar 72 horas exactas en cada momento.
            var fechaActualMenos72Horas = DateTime.Now.AddHours(-72);

            var ordenes = repositorio.Listar<OrdenDeCarga>(o => o.FechaEntregaGenerada <= fechaActualMenos72Horas && o.Estado == EstadoOrdenDeCarga.EntregaGenerada);

            foreach (var orden in ordenes)
            {
                orden.Estado = EstadoOrdenDeCarga.AnuladaPorVencimiento;
            }

            if (ordenes.Count > 0) repositorio.GuardarCambios();

            return ordenes;
        }

        public OrdenDeCargaEditarDto ObtenerEditar(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var ordenDto = new OrdenDeCargaEditarDto(orden);

            return ordenDto;
        }

        public string AnularOrden(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            if (orden.InformadaSAP)
            {
                throw new ValidationCustomException("La orden no puede anularse debido a que ya fue informada.");
            }

            orden.Estado = EstadoOrdenDeCarga.Anulada;

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAnulada;
        }

        #region Etapa1

        public string SeleccionarContrato(int ordenId, string contratoSAP)
        {
            string resultado = SuccessMsg.OrdenDeCargaActualizada;
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.ContratoSAP = contratoSAP;
            orden.DescripcionErrorInterno = "";
            orden.ActualizarEstado();

            if (!string.IsNullOrEmpty(orden.ContratoSAP) && orden.TransporteExiste)
            {
                var creadaEnSaP = CrearOrdenEnSAP(orden, orden.Cliente, false);

                if (creadaEnSaP)
                {
                    resultado = VerificarSituacionCrediticia(orden, true);
                }
            }
            else
            {
                resultado = VerificarTransporte(orden);
            }

            repositorio.GuardarCambios();

            return resultado;
        }


        public string ForzarCreacionOrden(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            var creadaEnSaP = CrearOrdenEnSAP(orden, orden.Cliente, true);

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

        public string SeleccionarPedido(int ordenId, string pedido)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.NumeroPedido = pedido;

            orden.ActualizarEstado();

            if (!string.IsNullOrEmpty(orden.NumeroPedido) && orden.TransporteExiste)
            {
                var creadaEnSaP = CrearOrdenEnSAP(orden, orden.Cliente, false);

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

            //var resultadoVerificarOrden = VerificarOrden(orden, orden.Cliente);

            if (orden.TransporteExiste)
            {
                orden.ActualizarEstado();
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
            var result = consumer.OrdenCargaControlEstadoRequest(orden.NumeroEntrega, "", "");

            if (result == "CE-06")
            {
                orden.Estado = EstadoOrdenDeCarga.Entregada;
                repositorio.GuardarCambios();
                return SuccessMsg.OrdenDeCargaActualizada;
            }
            else
            {
                return "La entrega sigue pendiente.";
            }
        }

        private bool TransporteExiste(OrdenDeCarga orden)
        {
            var result = consumer.OrdenCargaControlEstadoRequest("", "", orden.CUITTransporte);

            return result == "CE-07";
        }

        public string NotificarTransporte(int ordenDeCargaId)
        {
            string mensaje;

            var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);

            if (!TransporteExiste(orden))
            {
                string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

                var mails = mailsMesaVentaFas.Split(';').ToList();

                string asunto = "ALTA TTE";

                string cuerpo = string.Format("Razón Social: {0} <br> CUIT: {1}", orden.RazonSocialTransporte, orden.CUITTransporte);

                EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);

                mensaje = "Notificación enviada";
            }
            else
            {
                mensaje = "El transporte ya fue creado";
                orden.TransporteExiste = true;
                repositorio.GuardarCambios();
            }

            return mensaje;
        }

        public void VerificarTransporteBulk()
        {
            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(o => !o.TransporteExiste))
            {
                VerificarTransporte(ordenDeCarga);
            }

            foreach (var ordenDeCarga in repositorio.Listar<OrdenDeCarga>(o => o.Estado == EstadoOrdenDeCarga.EntregaGenerada))
            {
                VerificarEstadoEntrega(ordenDeCarga);
            }
        }

        #endregion

        #region Etapa2

        public string VerificarSituacionCrediticia(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return VerificarSituacionCrediticia(orden, false);
        }

        private string VerificarSituacionCrediticia(OrdenDeCarga orden, bool notificar)
        {
            if (orden.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito)
            {
                orden.AprobadoCredito = ObtenerSituacionCrediticia(orden);

                if (!orden.AprobadoCredito)
                {
                    if (notificar)
                    {
                        NotificarSituacionCrediticia(orden);
                    }

                    orden.ActualizarEstado();

                    if (notificar)
                    {
                        return "Verifique el crédito del pedido";
                    }
                    else
                    {
                        throw new InfoCustomException("Verifique el crédito del pedido.");
                    }

                }
                else
                {
                    orden.DescripcionErrorInterno = "";
                    orden.ActualizarEstado();

                    return GenerarEntregaSAP(orden);
                }
            }
            else
            {
                return "La orden no está pendiente de aprobación de crédito.";
            }
        }

        private bool ObtenerSituacionCrediticia(OrdenDeCarga orden)
        {
            var result = consumer.OrdenCargaControlEstadoRequest("", orden.NumeroPedido, "");

            return result == "CE-00";
        }

        private bool NotificarSituacionCrediticia(OrdenDeCarga orden)
        {
            string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            string mailsCobranzas = ConfigurationManager.AppSettings["EmailToCobranzas"];
            string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];

            var mails = new List<string>();

            mails.AddRange(mailsMesaVentaFas.Split(';').ToList());
            mails.AddRange(mailsCobranzas.Split(';').ToList());
            mails.AddRange(mailsComerciales.Split(';').ToList());

            var cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);

            string asunto = string.Concat("Orden de carga #", orden.Id);
            string cuerpo = string.Format("Orden de carga {0} de cliente {1} no pasó validaciones crediticias.", orden.Id, cliente.RazonSocial);

            EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);

            return true;
        }

        private string GenerarEntregaSAP(OrdenDeCarga orden)
        {
            orden.TransporteExiste = TransporteExiste(orden);

            if (!orden.TransporteExiste)
            {
                throw new ValidationCustomException("No existe el transportista");
            }

            var conductor = string.Concat(orden.ApellidoChofer, ", ", orden.NombreChofer);

            var tipoDocumento = "CUIL";
            var result = consumer.OrdenCargaEntregadaRequest(orden.CUITChofer, orden.Cantidad, conductor, orden.PatenteAcoplado, orden.ChasisAcoplado, orden.NumeroPedido, tipoDocumento, orden.CUITTransporte, out string respuesta);

            //OE-00   'OK'
            //OE-01   'No existe tranportista'
            //OE-02   'Entrega Creada - Error al insertar'

            switch (respuesta)
            {
                case "OE-00":
                case "OE-02":
                    orden.TransporteExiste = true;
                    orden.FechaEntregaGenerada = DateTime.Now;
                    orden.NumeroEntrega = result;
                    orden.ActualizarEstado();
                    repositorio.GuardarCambios();

                    return string.Concat("Se ha generado la entrega ", result, ".");

                case "OE-01":
                    orden.TransporteExiste = false;
                    orden.ActualizarEstado();
                    repositorio.GuardarCambios();
                    return string.Concat("No se pudo genera la entrega. No existe el transportista.");
            }

            return "Estado no conocido";
        }

        #endregion
    }
}
