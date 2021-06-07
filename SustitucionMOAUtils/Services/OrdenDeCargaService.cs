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

            if (ordenDeCarga.CUITCliente != "")
                cliente = usuario.ObtenerProveedorPorCUIT(ordenDeCarga.CUITCliente);
            else
                cliente = usuario.ObtenerProveedor();

            ordenDeCarga.FechaCarga = DateTime.Now;
            ordenDeCarga.Cliente_Id = cliente.Id;

            ordenDeCarga.Cantidad = int.Parse(ConfigurationManager.AppSettings["CantidadOrdenDeCarga"]);

            var crearPedido = VerificarOrden(ordenDeCarga, cliente);

            repositorio.Agregar(ordenDeCarga);

            repositorio.GuardarCambios();

            if (crearPedido)
            {
                EnviarASAP(ordenDeCarga, cliente);
            }
            else
            {

            }

            VerificarSituacionCrediticia(ordenDeCarga, notificar: true);



            return new Resultado { IdEntidad = ordenDeCarga.Id, Mensaje = SuccessMsg.OrdenDeCargaAgregada };
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
            var result = consumer.ControlCargaRequest(cliente.CodigoProveedor, ordenDeCarga.ContratoSAP, ordenDeCarga.Corredor, ordenDeCarga.CUITTransporte, ordenDeCarga.Producto, "");

            switch (result)
            {
                case "CC-00":
                    ordenDeCarga.TransporteExiste = true;
                    ordenDeCarga.CorredorSeleccionado = true;
                    ordenDeCarga.ContratoSAP = "11111";
                    return true;

                case "CC-01":
                    ordenDeCarga.TransporteExiste = false;
                    break;

                case "CC-02":
                    ordenDeCarga.CorredorSeleccionado = false;
                    break;

                case "CC-03":
                    throw new ValidationCustomException("El pedido informado no existe.");

                case "CC-04":
                    ordenDeCarga.TransporteExiste = true;
                    ordenDeCarga.CorredorSeleccionado = true;
                    ordenDeCarga.ContratoSAP = "11111";
                    break;

                case "CC-05":
                    ordenDeCarga.TransporteExiste = true;
                    ordenDeCarga.CorredorSeleccionado = true;
                    ordenDeCarga.ContratoSAP = "11111";
                    break;
            }

            return false;
        }

        public List<OrdenDeCargaDto> Listar(string mailUsuario)
        {

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esComercial = usuario.Roles.Where(r => r.Codigo == "COMERCIAL").Any();

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            if (esComercial)
            {
                listado = repositorio.Listar<OrdenDeCarga>().Select(x => new OrdenDeCargaDto
                {
                    Id = x.Id,
                    CUITCliente = x.CUITCliente,
                    DescripcionEstado = x.Estado.ToString(),
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),

                }).ToList();
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id)).Select(x => new OrdenDeCargaDto
                {
                    Id = x.Id,
                    CUITCliente = x.CUITCliente,
                    DescripcionEstado = x.Estado.ToString(),
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),
                }).ToList();
            }

            return listado;
        }

        public OrdenDeCargaDetalleDto Obtener(string mailUsuario, int ordenId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var esComercial = usuario.Roles.Where(r => r.Codigo == "COMERCIAL").Any();

            OrdenDeCargaDetalleDto ordenDto;
            OrdenDeCarga orden;

            if (esComercial)
            {
                orden = repositorio.Obtener<OrdenDeCarga>(ordenId);
            }
            else
            {
                var clientes = usuario.Proveedores.Select(c => c.Id);
                orden = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id) && n.Id == ordenId).FirstOrDefault();
            }

            var cliente = repositorio.Obtener<Proveedor>(orden.Cliente_Id);

            ordenDto = new OrdenDeCargaDetalleDto
            {
                Id = orden.Id,
                CUITCliente = orden.CUITCliente,
                DescripcionEstado = orden.Estado.ToString(),
                ColorSemaforo = orden.Estado.ObtenerSemaforo(),
                AprobadoCredito = orden.AprobadoCredito,
                Cantidad = orden.Cantidad,
                ChasisAcoplado = orden.ChasisAcoplado,
                Chofer = $"{orden.ApellidoChofer}, {orden.NombreChofer} ({orden.CUITChofer})",
                ContratoSAP = orden.ContratoSAP,
                Corredor = orden.Corredor,
                CorredorSeleccionado = orden.CorredorSeleccionado,
                FechaCarga = orden.FechaCarga.ToString("dd/MM/yyyy hh:mm"),
                FechaEntregaGenerada = orden.FechaEntregaGenerada?.ToString("dd/MM/yyyy hh:mm"),
                InformadaSAP = orden.InformadaSAP,
                Observacion = orden.Observacion,
                PatenteAcoplado = orden.PatenteAcoplado,
                RazonSocialCliente = cliente.RazonSocial,
                Transporte = $"{orden.RazonSocialTransporte} ({orden.CUITTransporte})",
                Producto = orden.Producto
            };

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
        private void VerificarContrato(OrdenDeCarga orden)
        {
            var contratosSAP = ObtenerContratos(orden.CUITCliente);

            if (contratosSAP.Count == 1)
            {
                orden.ContratoSAP = contratosSAP.First().Value;
            }
            else
            {
                orden.ContratoSAP = "";
            }

            orden.ActualizarEstado();
        }

        public string SeleccionarContrato(int ordenId, string contratoSAP)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.ContratoSAP = contratoSAP;

            orden.ActualizarEstado();

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        public string SeleccionarCorredor(int ordenId, string corredor)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.Corredor = corredor;
            orden.CorredorSeleccionado = true;

            orden.ActualizarEstado();

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }


        public string SeleccionarCorredorContrato(int ordenId, CorredorContratoDto corredorContrato)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.Corredor = corredorContrato.Corredor; ;
            orden.ContratoSAP = corredorContrato.Contrato;
            orden.CorredorSeleccionado = true;

            orden.ActualizarEstado();

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        public Dictionary<string, string> ObtenerContratos(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return ObtenerContratos(orden.CUITCliente);
        }

        public List<CorredorContratoDto> ObtenerContratosYCorredores(int ordenID)
        {
            var listado = new List<CorredorContratoDto>
            {
                new CorredorContratoDto(1, 1, "Pepe", "123"),
                new CorredorContratoDto(1, 2, "Pepe", "456"),
                new CorredorContratoDto(2, 3, "Luis", "789"),
                new CorredorContratoDto(2, 4, "Luis", "234")
            };


            return listado;
        }

        public Dictionary<string, string> ObtenerContratos(string CUIT)
        {
            return new Dictionary<string, string> { { "1", "1231231" }, { "2", "515121" } };
        }

        public Dictionary<string, string> ObtenerCorredores(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return ObtenerCorredores(orden.CUITCliente);
        }

        public Dictionary<string, string> ObtenerCorredores(string CUIT)
        {
            return new Dictionary<string, string> { { "1", "PEPE" }, { "2", "LUIS" } };
        }

        private void VerificarCorredor(OrdenDeCarga orden)
        {
            var corredoresCliente = ObtenerCorredores(orden.CUITCliente);

            if (corredoresCliente.Count == 1)
            {
                orden.Corredor = corredoresCliente.First().Value;
                orden.CorredorSeleccionado = true;
            }
            else
            {
                orden.CorredorSeleccionado = false;
            }

            orden.ActualizarEstado();
        }

        public string VerificarTransporte(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            return VerificarTransporte(orden);
        }

        public string VerificarTransporte(OrdenDeCarga orden)
        {
            orden.TransporteExiste = TransporteExiste(orden.CUITTransporte);

            if (orden.TransporteExiste)
            {
                orden.ActualizarEstado();
                return SuccessMsg.OrdenDeCargaActualizada;
            }
            else
            {
                return "El transporte no existe";
            }
        }

        private bool TransporteExiste(string CUIT)
        {
            return false;
        }

        public string NotificarTransporte(int ordenDeCargaId)
        {
            string mensaje;

            var orden = repositorio.Obtener<OrdenDeCarga>(ordenDeCargaId);

            if (!TransporteExiste(orden.CUITTransporte))
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
                orden.AprobadoCredito = ObtenerSituacionCrediticia(orden.CUITCliente);

                if (!orden.AprobadoCredito)
                {
                    if (notificar)
                    {
                        NotificarSituacionCrediticia(orden);
                    }

                    orden.ActualizarEstado();
                }
                else
                {
                    orden.ActualizarEstado();

                    //EnviarASAP(orden);
                }
            }

            return "Orden actualizada correctamente";
        }

        private bool ObtenerSituacionCrediticia(string CUIT)
        {
            return false;
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

        private void EnviarASAP(OrdenDeCarga orden, Proveedor cliente)
        {
            //OV-01   'Verificar Contrato, Material, Cliente'
            //OV-02   'Verificar cantidad pendiente de Contratada'
            //OV-03   'Pedido creado - Verificar Crédito de pedido'
            //OV-00   'OK'
            var result = consumer.CrearOrdenRequest(cliente.CodigoProveedor, orden.ContratoSAP, orden.Corredor, orden.Cantidad, orden.Producto, "", out string numeroPedido);

            if (result == "OK")
            {
                orden.InformadaSAP = true;
                //orden.numeroPedido = ""

                orden.ActualizarEstado();
            }
        }
        #endregion


    }
}
