using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaService : IOrdenDeCargaService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IOrdenCargaConsumerMOA consumer;

        public OrdenDeCargaService(IRepositorio repositorio, IOrdenCargaConsumerMOA consumer)
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
        }

        public Resultado Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Proveedor cliente;
            ordenDeCarga.Estado = EstadoOrdenDeCarga.Pendiente;

            if (usuario.EsCorredor())
            {
                var corredor = usuario.ObtenerCorredor();
                ordenDeCarga.Corredor = corredor.CodigoProveedor;
                cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);
            }
            else
            {
                ordenDeCarga.Corredor = "";
                cliente = usuario.ObtenerProveedor();
            }

            //if (ordenDeCarga.CUITCliente != "")
            //    cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);
            //else
            //    cliente = usuario.ObtenerProveedor();

            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.Cliente_Id = cliente.Id;

            var producto = repositorio.Obtener<Material>(ordenDeCarga.Producto_Id);

            ordenDeCarga.Producto = producto;
            ordenDeCarga.NumeroPedido = string.IsNullOrEmpty(ordenDeCarga.NumeroPedidoIngresado) ? "" : ordenDeCarga.NumeroPedidoIngresado;
            ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;

            ordenDeCarga.Cantidad = int.Parse(ConfigurationManager.AppSettings["CantidadOrdenDeCarga"]);

            ordenDeCarga.TransporteExiste = TransporteExiste(ordenDeCarga);

            var crearPedido = VerificarOrden(ordenDeCarga, cliente);

            repositorio.Agregar(ordenDeCarga);

            repositorio.GuardarCambios();

            if (crearPedido)
            {
                ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;

                var creadaEnSaP = CrearOrdenEnSAP(ordenDeCarga, cliente);

                if (creadaEnSaP)
                {
                    VerificarSituacionCrediticia(ordenDeCarga, notificar: true);
                }
            }

            return new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaAgregada };
        }

        public Resultado Editar(OrdenDeCarga ordenDeCarga, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var ordenEditar = repositorio.Obtener<OrdenDeCarga>(ordenDeCarga.Id);

            if (ordenEditar.FechaEntregaGenerada != null)
            {
                ordenEditar.NombreChofer = ordenDeCarga.NombreChofer;
                ordenEditar.ApellidoChofer = ordenDeCarga.ApellidoChofer;
                ordenEditar.CUITChofer = ordenDeCarga.CUITChofer;
                ordenEditar.PatenteAcoplado = ordenDeCarga.PatenteAcoplado;
                ordenEditar.ChasisAcoplado = ordenDeCarga.ChasisAcoplado;
            }

            if (!ordenEditar.TransporteExiste)
            {
                ordenEditar.RazonSocialTransporte = ordenDeCarga.RazonSocialTransporte;
                ordenEditar.CUITTransporte = ordenDeCarga.CUITTransporte;
            }

            if (!ordenEditar.InformadaSAP)
            {
                ordenEditar.ContratoIngresado = ordenDeCarga.ContratoIngresado;
                ordenEditar.Cantidad = ordenDeCarga.Cantidad;
                ordenEditar.Producto_Id = ordenDeCarga.Producto_Id;
                ordenEditar.NumeroPedidoIngresado = ordenDeCarga.NumeroPedidoIngresado;

                var crearPedido = VerificarOrden(ordenEditar, ordenEditar.Cliente);

                if (crearPedido)
                {
                    ordenEditar.ContratoSAP = ordenEditar.ContratoIngresado;

                    var creadaEnSaP = CrearOrdenEnSAP(ordenEditar, ordenEditar.Cliente);

                    if (creadaEnSaP)
                    {
                        VerificarSituacionCrediticia(ordenEditar, notificar: true);
                    }
                }
            }

            ordenEditar.Observacion = ordenDeCarga.Observacion;

            repositorio.GuardarCambios();

            return new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
        }

        private bool CrearOrdenEnSAP(OrdenDeCarga orden, Proveedor cliente)
        {
            //OV-01   'Verificar Contrato, Material, Cliente'
            //OV-02   'Verificar cantidad pendiente de Contratada'
            //OV-03   'Pedido creado - Verificar Crédito de pedido'
            //OV-00   'OK'
            var result = consumer.CrearOrdenRequest(cliente.CodigoProveedor, orden.ContratoSAP, orden.Corredor, orden.Cantidad, orden.Producto.CodigoSap, orden.NumeroPedidoIngresado, out string numeroPedido);

            //var result2 = consumer.OrdenCargaEntregadaRequest(orden.CUITChofer, orden.Cantidad, orden.NombreChofer, orden.PatenteAcoplado, orden.ChasisAcoplado, "", "DNI", orden.CUITTransporte, out string mensaje);
            if (result == "OV-00" || result == "OV-03")
            {
                orden.InformadaSAP = true;
                orden.NumeroPedido = numeroPedido;

                if (result == "OV-03")
                {
                    orden.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
                }

                orden.ActualizarEstado();
                repositorio.GuardarCambios();

                return true;
            }
            else
            {
                return false;
            }
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
            var result = consumer.ControlCargaRequest(cliente.CodigoProveedor, ordenDeCarga.ContratoSAP, ordenDeCarga.Corredor, ordenDeCarga.CUITTransporte, ordenDeCarga.Producto.CodigoSap, ordenDeCarga.NumeroPedido);

            if (result.Contains(','))
            {
                ordenDeCarga.ContratosRespuesta = result;
                ordenDeCarga.ContratoSAP = "";
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
                        return true;

                    case "CC-01":
                        //ordenDeCarga.CorredorSeleccionado = false;
                        //break;
                        throw new ValidationCustomException("No se encontró ningun contrato con ese producto.");


                    case "CC-02":
                        ordenDeCarga.TransporteExiste = false;
                        return true;

                    case "CC-03":
                        throw new ValidationCustomException("El pedido informado no existe.");

                    case "CC-04":
                        ordenDeCarga.TransporteExiste = true;
                        ordenDeCarga.ContratoSAP = ordenDeCarga.ContratoIngresado;
                        break;

                    case "CC-05":
                        throw new ValidationCustomException("El pedido ingresado ya fue entregado completamente.");
                }
            }

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
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "incio"), e);
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
            var mostrarListadoCompleto = usuario.TienePermiso("VER TODAS ORDENES DE CARGA");

            fechaFinDateTime = fechaFinDateTime.AddDays(1);

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            if (mostrarListadoCompleto)
            {
                listado = repositorio.Listar<OrdenDeCarga>(o => o.FechaCarga <= fechaFinDateTime && o.FechaCarga >= fechaIncioDateTime)
                    .Select(x => new OrdenDeCargaDto
                    {
                        Id = x.Id,
                        Cliente = x.Cliente.CodigoProveedor,
                        Corredor = x.Corredor,
                        Contrato = x.ContratoSAP ?? "-",
                        Pedido = x.NumeroPedido ?? "-",
                        Entrega = x.NumeroEntrega ?? "-",
                        Material = x.Producto.Nombre,
                        DescripcionEstado = x.Estado.ToFriendlyString(),
                        ColorSemaforo = x.Estado.ObtenerSemaforo(),
                        EsFacturaAnticipada = (x.NumeroPedidoIngresado != null)
                    }).ToList();
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id) && n.FechaCarga <= fechaFinDateTime
                    && n.FechaCarga >= fechaIncioDateTime)
                    .Select(x => new OrdenDeCargaDto
                    {
                        Id = x.Id,
                        Contrato = x.ContratoSAP ?? "-",
                        Pedido = x.NumeroPedido ?? "-",
                        Entrega = x.NumeroEntrega ?? "-",
                        Material = x.Producto.Nombre,
                        DescripcionEstado = x.Estado.ToUserFriendlyString(),
                        EsFacturaAnticipada = (x.NumeroPedidoIngresado != null)
                    }).ToList();
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
            var mostrarListadoCompleto = usuario.TienePermiso("VER TODAS ORDENES DE CARGA");

            OrdenDeCargaDetalleDto ordenDto;
            OrdenDeCarga orden;

            if (mostrarListadoCompleto)
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
                Cliente = orden.Cliente.CodigoProveedor,
                AprobadoCredito = orden.AprobadoCredito,
                Cantidad = orden.Cantidad,
                ChasisAcoplado = orden.ChasisAcoplado,
                Chofer = $"{orden.ApellidoChofer}, {orden.NombreChofer} ({orden.CUITChofer})",
                ContratoSAP = string.IsNullOrEmpty(orden.ContratoSAP) ? "-" : orden.ContratoSAP,
                Corredor = orden.Corredor,
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
                NumeroEntrega = string.IsNullOrEmpty(orden.NumeroEntrega) ? "-" : orden.NumeroEntrega,
                NumeroPedido = string.IsNullOrEmpty(orden.NumeroPedido) ? "-" : orden.NumeroPedido
            };

            return ordenDto;
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
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.ContratoSAP = contratoSAP;

            orden.ActualizarEstado();

            if (!string.IsNullOrEmpty(orden.ContratoSAP) && orden.TransporteExiste)
            {
                var creadaEnSaP = CrearOrdenEnSAP(orden, orden.Cliente);

                if (creadaEnSaP)
                {
                    VerificarSituacionCrediticia(orden, notificar: true);
                }
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

            return VerificarSituacionCrediticia(orden, notificar: false);
        }

        private string VerificarSituacionCrediticia(OrdenDeCarga orden, bool notificar)
        {
            if (orden.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito)
            {
                orden.AprobadoCredito = ObtenerSituacionCrediticia(orden);

                if (!orden.AprobadoCredito)
                {
                    if (notificar && false)
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

            var tipoDocumento = "DNI";
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
