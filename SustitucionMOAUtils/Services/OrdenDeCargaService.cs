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
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Helpers;

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
            Log.Info($"OdenDeCargaService Agregar: {ordenDeCarga.ToJson()}");

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
                        //throw new ValidationCustomException("No se encontró el corredor seleccionado");
                        return new Resultado { error = "No se encontró el corredor seleccionado" };
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
                        //throw new ValidationCustomException("Su usuario no está habilitado para operar con ese CUIT");
                        return new Resultado { error = "Su usuario no está habilitado para operar con ese CUIT" };
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
            ordenDeCarga.PedidoSAP = ordenDeCarga.NumeroPedidoIngresado;

            ordenDeCarga.Cantidad = int.Parse(ConfigurationManager.AppSettings["CantidadOrdenDeCarga"]);

            ordenDeCarga.TransporteExiste = TransporteExiste(ordenDeCarga);

            var crearPedido = VerificarOrden(ordenDeCarga, cliente);

            //if (ordenDeCarga.ContratoSAP == null)
            //{
            //    ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
            //}

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
            Log.Info($"OdenDeCargaService Editar: {ordenDeCarga.ToJson()}");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var valoresAEditar = new List<string> { "NombreChofer", "CUITChofer", "PatenteAcoplado", "ChasisAcoplado", "ContratoIngresado",
            "NumeroPedido", "Observacion", "Cantidad", "RazonSocialTransporte", "CUITTransporte", "Producto_Id", "NumeroPedidoIngresado" };
            var ordenEditar = repositorio.Obtener<OrdenDeCarga>(ordenDeCarga.Id);
            var listaValoresDiferentes = ordenEditar.Compare(ordenDeCarga);
            var historialCambios = new List<OrdenDeCargaCambiosHistorial>() { };

            ordenEditar.NombreChofer = ordenDeCarga.NombreChofer;
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
                var crearPedido = !string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP) || VerificarOrden(ordenEditar, ordenEditar.Cliente);

                if (crearPedido)
                {
                    if (string.IsNullOrWhiteSpace(ordenEditar.ContratoSAP))
                    {
                        ordenEditar.ContratoSAP = ordenEditar.ContratoIngresado;
                    }

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

            // error de base de datos al usar agregar todos
            //repositorio.AgregarTodos(historialCambios);
            foreach (var historialCambio in historialCambios)
            {
                repositorio.Agregar(historialCambio);
            }
            ordenEditar.HistorialCambios.Concat(historialCambios);
            repositorio.GuardarCambios();

            NotificarTransporte(ordenEditar.Id);
            
            if (historialCambios.Count > 0)
            {
                //Aviso de Edición de Orden de Carga
                var emailSenderData = ConstruirCuerpoEmail(historialCambios, ordenDeCarga.NumeroEntrega);
                if (emailSenderData != null)
                {
                    EmailSender.EnviarMail(emailSenderData);
                }
			}

            return new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
        }

        private bool CrearOrdenEnSAP(OrdenDeCarga orden, Proveedor cliente, bool forzarCreacion)
        {
            //OV-01   'Verificar Contrato, Material, Cliente'
            //OV-02   'Verificar cantidad pendiente de Contratada'
            //OV-03   'Pedido creado - Verificar Crédito de pedido'
            //OV-00   'OK'
            var forzarCreacionStr = forzarCreacion ? "" : "X";
            string contrato = null;
            if(orden.ContratoSAP !=null)
            {
                contrato = orden.ContratoSAP.Split('|').First();
            }           
            Log.Info("CrearOrdenEnSAP CrearOrdenRequest" + $"cliente.CodigoProveedor {cliente.CodigoProveedor ?? ""}, orden.ContratoSAP {orden.ContratoSAP ?? ""}, orden.CodigoCorredor {orden.CodigoCorredor ?? ""}, orden.Cantidad {orden.Cantidad}, orden.Producto.CodigoSap {orden.Producto.CodigoSap ?? ""}, orden.NumeroPedidoIngresado {orden.NumeroPedidoIngresado ?? ""}, forzarCreacionStr {forzarCreacionStr ?? ""}");
            var result = consumer.CrearOrdenRequest(cliente.CodigoProveedor, contrato, orden.CodigoCorredor, orden.Cantidad, orden.Producto.CodigoSap, orden.NumeroPedidoIngresado, forzarCreacionStr, out string numeroPedido);
            Log.Info("CrearOrdenEnSAP CrearOrdenRequest Result " + result);

            var resultadoCrearOrden = false;
            orden.ContratoSinCantidadPendiente = false;

            //var result2 = consumer.OrdenCargaEntregadaRequest(orden.CUITChofer, orden.Cantidad, orden.NombreChofer, orden.PatenteAcoplado, orden.ChasisAcoplado, "", "DNI", orden.CUITTransporte, out string mensaje);
            if (result == "OV-00" || result == "OV-03")
            {
                orden.InformadaSAP = true;
                orden.NumeroPedido = numeroPedido;
                orden.DescripcionErrorInterno = "";
                orden.DescripcionCodigoVerificacionSap = "";
                orden.CodigoVerificacionSap = "";

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
            string contrato = null;
            if (ordenDeCarga.ContratoIngresado != null) 
            {
                contrato = string.IsNullOrEmpty(ordenDeCarga.ContratoSAP) ? ordenDeCarga.ContratoIngresado : ordenDeCarga.ContratoSAP;
                contrato = contrato.Split('|').First();
            }
            
            Log.Info("VerificarOrden ControlCargaRequest " + $"cliente.CodigoProveedor {cliente.CodigoProveedor ?? ""}, contrato {contrato ?? ""}, ordenDeCarga.CodigoCorredor {ordenDeCarga.CodigoCorredor ?? ""}, ordenDeCarga.CUITTransporte {ordenDeCarga.CUITTransporte ?? ""}, ordenDeCarga.Producto.CodigoSap {ordenDeCarga.Producto.CodigoSap ?? ""}, ordenDeCarga.NumeroPedido {ordenDeCarga.NumeroPedido ?? ""}");
            var result = consumer.ControlCargaRequest(cliente.CodigoProveedor, contrato, ordenDeCarga.CodigoCorredor, ordenDeCarga.CUITTransporte, ordenDeCarga.Producto.CodigoSap, ordenDeCarga.NumeroPedido);
            Log.Info("VerificarOrden ControlCargaRequest Result " + result);

            //Existe la posibilidad de que el cliente tenga varios contratos abiertos con molinos. En caso de tener una "," un comercial debe seeccionar
            //cual es el contrato correcto que le quiere entregar.
            if (result.Contains(','))
            {
                result = string.Join(",", result.Split(',').Select(a => a.Split('|')[0]).ToList());
                if (string.IsNullOrEmpty(ordenDeCarga.NumeroPedido))
                {
                    if (string.IsNullOrEmpty(ordenDeCarga.ContratoSAP))
                    {
                        ordenDeCarga.ContratosRespuesta = result;
                        ordenDeCarga.ContratoSAP = "";
                        ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios contratos pendientes para el mismo cliente. Seleccione el contrato para generar entregas desde el botón \"Contratos\".";
                        NotificarVariosContratos(ordenDeCarga.Id);

                    }
                }
                else
                {
                    ordenDeCarga.PedidosRespuesta = result;
                    ordenDeCarga.NumeroPedido = "";
                    ordenDeCarga.DescripcionErrorInterno = "Se encontraron varios pedidos pendientes para el mismo cliente. Seleccione el pedido para generar entregas desde el botón \"Pedidos\".";
                    NotificarVariosPedidos(ordenDeCarga.Id);
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
                    filtrosEstados.Add(EstadoOrdenDeCarga.AnulacionSolicitada);
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
                    && n.FechaCarga >= fechaIncioDateTime
                    && (n.Estado == EstadoOrdenDeCarga.Vencida
                        || n.Estado == EstadoOrdenDeCarga.ErrorDeCarga
                        || n.Estado == EstadoOrdenDeCarga.Pendiente
                        || n.Estado == EstadoOrdenDeCarga.Confirmado
                        || n.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito
                        || n.Estado == EstadoOrdenDeCarga.EntregaPendiente
                        || n.Estado == EstadoOrdenDeCarga.EntregaGenerada)
                    )
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

            Proveedor cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);
            //  var ordenDeCargaCambiosHistorial = repositorio.Listar<OrdenDeCargaCambiosHistorial>(ordenes => ordenes.OrdenDeCarga_Id == orden.Id);

            var ordenDeCargaCambiosHistorial = repositorio.Listar<OrdenDeCargaCambiosHistorial>
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
                Chofer = $"{orden.NombreChofer} ({orden.CUITChofer})",
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
                ContratosRespuesta = string.IsNullOrEmpty(orden.ContratosRespuesta) ? "-" : orden.ContratosRespuesta,
                NumeroEntrega = string.IsNullOrEmpty(orden.NumeroEntrega) ? "-" : orden.NumeroEntrega,
                NumeroPedido = string.IsNullOrEmpty(orden.NumeroPedido) ? "-" : orden.NumeroPedido,
                MensajeValidacionSAP = string.IsNullOrEmpty(orden.DescripcionCodigoVerificacionSap) ? "" : orden.DescripcionCodigoVerificacionSap,
                ContratoSinCantidadPendiente = orden.ContratoSinCantidadPendiente,
                DescripcionErrorInterno = string.IsNullOrEmpty(orden.DescripcionErrorInterno) ? "" : orden.DescripcionErrorInterno,
                OrdenDeCargaCambiosHistorial = ordenDeCargaCambiosHistorial
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

        public string SolicitarAnulacionOrden(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            if (orden.InformadaSAP)
            {
                throw new ValidationCustomException("La orden no puede anularse debido a que ya fue informada.");
            }

            orden.Estado = EstadoOrdenDeCarga.AnulacionSolicitada;

            repositorio.GuardarCambios();

            NotificarSolicitudAnulacion(ordenId);

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        public string NotificarSolicitudAnulacion(int ordenDeCargaId)
        {
            string mensaje;
            try
            {                

                var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);

                string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];

                string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];

                var mails = new List<string>
                {
                    mailsMesaVentaFas,
                    mailsComerciales,
                };

                string asunto = "Solicitud de anulación, Orden de carga N° " + ordenDeCargaId.ToString();

                string cuerpo = string.Format("Razón Social: {0} <br> CUIT: {1}", orden.RazonSocialTransporte, orden.CUITTransporte);

                EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);

                mensaje = "Notificación enviada";
            }
            catch(Exception ex)
            {
                mensaje = "Error al enviar la notificación : " + ex.Message;
            }
            return mensaje;
        }

        public OrdenDeCargaDto ObtenerPatentes(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            Proveedor cliente;
            OrdenDeCargaDto result = new OrdenDeCargaDto();
            List<OrdenDeCarga> listOrden = new List<OrdenDeCarga>();
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esComercial = usuario.TienePermiso("VER ORDENES DE CARGA PARA COMERCIALES");

            if (esComercial || usuario.EsCorredor())
            {
                if (ordenDeCarga.CUITCliente == null)
                {
                    result.ordenes.Add(new AutoCompleteDropdownElement() { label = " ", value = " " });
                    return result;
                }
                if (usuario.EsCorredor())
                {
                    cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);
                    listOrden = repositorio.Listar<OrdenDeCarga>(x => x.Cliente_Id == cliente.Id).ToList();
                }
                else
                {
                    cliente = repositorio.Obtener<Proveedor>(
                    x => x.CUIT == ordenDeCarga.CUITCliente &&
                    x.EstadoAprobacion == EstadoAprobacion.Aprobado && x.TipoProveedor.Id == 5);
                    listOrden = repositorio.Listar<OrdenDeCarga>(x => x.Cliente_Id == cliente.Id).ToList();
                }

            }
            else
            {
                cliente = usuario.ObtenerProveedor();
                listOrden = repositorio.Listar<OrdenDeCarga>(x => x.Cliente_Id == cliente.Id).ToList();
            }

            foreach (var ordenes in listOrden)
            {
                result.ordenes.Add(new AutoCompleteDropdownElement()
                {
                    label = ordenes.ChasisAcoplado,
                    value = ordenes.PatenteAcoplado

                });
            }

            result.ordenes = result.ordenes.Distinct().ToList();
            return result;
        }

        #region Etapa1

        public Resultado SeleccionarContrato(int ordenId, string contratoSAP)
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
                    return VerificarSituacionCrediticia(orden, true);
                }
            }
            else
            {
                resultado = VerificarTransporte(orden);
            }

            repositorio.GuardarCambios();

            return new Resultado { Mensaje = resultado };
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
            VerificarOrden(orden, orden.Cliente);

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
            Log.Info("VerificarEstadoEntrega OrdenCargaControlEstadoRequest " + $"orden.NumeroEntrega {orden.NumeroEntrega ?? ""}");
            var result = consumer.OrdenCargaControlEstadoRequest(orden.NumeroEntrega, "", "");
            Log.Info("VerificarEstadoEntrega OrdenCargaControlEstadoRequest Result " + result);

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
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest " + $"orden.CUITTransporte {orden.CUITTransporte ?? ""}");
            var result = consumer.OrdenCargaControlEstadoRequest("", "", orden.CUITTransporte);
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest Result " + result);

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

                    var mails = mailsMesaVentaFas.Split(';').ToList();
                    mails.AddRange(mailsMesaENTSL.Split(';').ToList());

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
            }
            catch (Exception e)
            {
                Log.Error(e);
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

        public Resultado VerificarSituacionCrediticia(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return VerificarSituacionCrediticia(orden, false);
        }

        private Resultado VerificarSituacionCrediticia(OrdenDeCarga orden, bool notificar)
        {
            if (orden.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito)
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
                            EmailSender.EnviarMail(emailSenderData);
                        }
                    }

                    orden.ActualizarEstado();

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
                    orden.ActualizarEstado();
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
            Log.Info("ObtenerSituacionCrediticia OrdenCargaControlEstadoRequest " + $"orden.NumeroPedido {orden.NumeroPedido ?? ""}");
            var result = consumer.OrdenCargaControlEstadoRequest("", orden.NumeroPedido, "");
            Log.Info("ObtenerSituacionCrediticia OrdenCargaControlEstadoRequest Result " + result);

            return result == "CE-00";
        }

        public EmailSenderData ConstruirCuerpoEmail(List<OrdenDeCargaCambiosHistorial> ordenDeCargaHistorial, string numeroPedido)
        {
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

                emailSenderData.Mails.AddRange(mailsMesaVentaFas.Split(';').ToList());
                emailSenderData.Mails.AddRange(mailsComerciales.Split(';').ToList());
                if (emailSenderData.Mails.Count == 0)
                {
                    return null;
                }

                var cambios = new StringBuilder();
                foreach (var cambio in ordenDeCargaHistorial)
                {
                    cambios.AppendLine($"<tr><td>{cambio.NombreColumnaCambio}</td><td>{cambio.Antes}</td><td>{cambio.Despues}</td><td>{cambio.FechaCambio}</td></tr>");
                }
                var cuerpoTemplate = File.ReadAllText(EMAIL_TEMPLATE);
                emailSenderData.Asunto = $"Molinos Agro - Edición en su orden de carga n°: {ordenDeCargaHistorial[0].OrdenDeCarga_Id}";
                emailSenderData.Cuerpo = string.Format(cuerpoTemplate, DateTime.Now.ToString(), ordenDeCargaHistorial[0].OrdenDeCarga_Id, numeroPedido, cambios);
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
            var mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
            var mailsCobranzas = ConfigurationManager.AppSettings["EmailToCobranzas"];
            var mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
            try
			{
                if (string.IsNullOrEmpty(mailsMesaVentaFas) &&
                    string.IsNullOrEmpty(mailsCobranzas) &&
                    string.IsNullOrEmpty(mailsComerciales))
				{
                    return null;
				}

                emailSenderData.Mails.AddRange(mailsMesaVentaFas.Split(';').ToList());
                emailSenderData.Mails.AddRange(mailsCobranzas.Split(';').ToList());
                emailSenderData.Mails.AddRange(mailsComerciales.Split(';').ToList());
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

                emailSenderData.Asunto = $"Orden de carga #{orden.Id}";
                emailSenderData.Cuerpo = $"Orden de carga {orden.Id} de cliente {cliente.RazonSocial} no pasó validaciones crediticias. <br> Número de Contrato: {contrato} <br> Número de Pedido: {pedido}";
                return emailSenderData;
            }
            catch
			{
                return null;
			}
        }

        private Resultado GenerarEntregaSAP(OrdenDeCarga orden)
        {
            orden.TransporteExiste = TransporteExiste(orden);

            if (!orden.TransporteExiste)
            {
                return new Resultado { error = "No existe el transportista" };
            }

            var conductor = orden.NombreChofer;

            var tipoDocumento = "CUIL";

            Log.Info("GenerarEntregaSAP OrdenCargaEntregadaRequest " + $"orden.CUITChofer {orden.CUITChofer ?? ""}, orden.Cantidad {orden.Cantidad}, conductor {conductor ?? ""}, orden.PatenteAcoplado {orden.PatenteAcoplado ?? ""}, orden.ChasisAcoplado {orden.ChasisAcoplado ?? ""}, orden.NumeroPedido {orden.NumeroPedido ?? ""}, tipoDocumento {tipoDocumento ?? ""}, orden.CUITTransporte {orden.CUITTransporte ?? ""}");
            var result = consumer.OrdenCargaEntregadaRequest(orden.CUITChofer, orden.Cantidad, conductor, orden.PatenteAcoplado, orden.ChasisAcoplado, orden.NumeroPedido, tipoDocumento, orden.CUITTransporte, out string respuesta);
            Log.Info("GenerarEntregaSAP OrdenCargaEntregadaRequest Result " + result);

            //OE-00   'OK'
            //OE-01   'No existe tranportista
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
                    return new Resultado { Mensaje = string.Concat("Se ha generado la entrega ", result, ".") };
                //return string.Concat("Se ha generado la entrega ", result, ".");

                case "OE-01":
                    orden.TransporteExiste = false;
                    orden.ActualizarEstado();
                    repositorio.GuardarCambios();
                    return new Resultado { Mensaje = "No se pudo genera la entrega. No existe el transportista." };
                    //return string.Concat("No se pudo genera la entrega. No existe el transportista.");
            }

            return new Resultado { info = "Estado no conocido" };
        }

        public string NotificarVariosPedidos(int ordenDeCargaId)
        {
            string mensaje = "";

            try
            {
                var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);
                string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
                string mailsMesaENTSL = ConfigurationManager.AppSettings["EmailToMesaENTSL"];
                string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
                var mails = mailsMesaVentaFas.Split(';').ToList();
                mails.AddRange(mailsMesaENTSL.Split(';').ToList());
                mails.AddRange(mailsComerciales.Split(';').ToList());
                string asunto = "Varios pedidos pendientes para el mismo cliente";
                string cuerpo = string.Format("Se encontraron varios Pedidos pendientes para el mismo cliente. Orden de carga {0} de cliente {1} <br> Numero de Pedido: {2}",
                     orden.Id, orden.Cliente.RazonSocial, (string.IsNullOrEmpty(orden.PedidoSAP) ? orden.NumeroPedidoIngresado : orden.PedidoSAP) ?? "");
                EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);

                mensaje = "Notificación enviada";

            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return mensaje;
        }

        public string NotificarVariosContratos(int ordenDeCargaId)
        {
            string mensaje = "";
            try
            {

                var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);
                string mailsMesaVentaFas = ConfigurationManager.AppSettings["EmailToMesaVentaFas"];
                string mailsMesaENTSL = ConfigurationManager.AppSettings["EmailToMesaENTSL"];
                string mailsComerciales = ConfigurationManager.AppSettings["EmailToComerciales"];
                var mails = mailsMesaVentaFas.Split(';').ToList();
                mails.AddRange(mailsMesaENTSL.Split(';').ToList());
                mails.AddRange(mailsComerciales.Split(';').ToList());
                string asunto = "Varios ctto pendientes";
                string cuerpo = string.Format("Se encontraron varios contratos pendientes para el mismo cliente. Orden de carga {0} de cliente {1} <br> Numero de Contrato: {2}",
                    orden.Id, orden.Cliente.RazonSocial, string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP);
                EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);

                mensaje = "Notificación enviada";

            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return mensaje;
        }

        #endregion
    }
}
