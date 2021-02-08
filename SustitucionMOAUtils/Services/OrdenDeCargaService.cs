using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAUtils.Services
{

    public class OrdenDeCargaService : IOrdenDeCargaService
    {
        protected readonly IRepositorio repositorio;

        public OrdenDeCargaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public string Agregar(OrdenDeCarga ordenDeCarga, string mailUsuario)
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

            VerificarContrato(ordenDeCarga);

            VerificarCorredor(ordenDeCarga);

            VerificarTransporte(ordenDeCarga);

            EnviarASAP(ordenDeCarga);

            VerificarSituacionCrediticia(ordenDeCarga);

            repositorio.Agregar(ordenDeCarga);

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaAgregada;
        }

        public List<OrdenDeCargaDto> Listar(string mailUsuario)
        {

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var clientes = usuario.Proveedores.Select(c => c.Id);

            List<OrdenDeCargaDto> listado = new List<OrdenDeCargaDto>();
            if (usuario.EsAdmin())
            {
                listado = repositorio.Listar<OrdenDeCarga>(n => clientes.Contains(n.Cliente_Id)).Select(x => new OrdenDeCargaDto
                {
                    Id = x.Id,
                    CUITCliente = x.CUITCliente,
                    DescripcionEstado = x.Estado.ToString(),
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),

                }).ToList();
            }
            else
            {
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


        #region Etapa1
        private void VerificarContrato(OrdenDeCarga orden)
        {
            var contratosSAP = ObtenerContratos(orden.CUITCliente);

            if (contratosSAP.Count == 1)
            {
                orden.ContratoSAP = contratosSAP.First();
            }
            else
            {
                orden.ContratoSAP = "";
            }

            orden.ActualizarEstado();
        }

        private string SeleccionarContrato(int ordenId, string contratoSAP)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.ContratoSAP = contratoSAP;

            orden.ActualizarEstado();

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        private string SeleccionarCorredor(int ordenId, string corredor)
        {
            var orden = repositorio.Obtener<OrdenDeCarga>(ordenId);

            orden.Corredor = corredor;
            orden.CorredorSeleccionado = true;

            orden.ActualizarEstado();

            repositorio.GuardarCambios();

            return SuccessMsg.OrdenDeCargaActualizada;
        }

        private List<string> ObtenerContratos(string CUIT)
        {
            return new List<string> { "1231231", "515121" };
        }

        private List<string> ObtenerCorredores(string CUIT)
        {
            return new List<string> { "PEPE", "LUIS" };
        }

        private void VerificarCorredor(OrdenDeCarga orden)
        {
            var corredoresCliente = ObtenerCorredores(orden.CUITCliente);

            if (corredoresCliente.Count == 1)
            {
                orden.Corredor = corredoresCliente.First();
                orden.CorredorSeleccionado = true;
            }
            else
            {
                orden.CorredorSeleccionado = false;
            }

            orden.ActualizarEstado();
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
            return true;
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
        #endregion

        #region Etapa2

        private void VerificarSituacionCrediticia(OrdenDeCarga orden)
        {
            if (orden.Estado == EstadoOrdenDeCarga.PendienteAprobacionCredito)
            {
                orden.AprobadoCredito = ObtenerSituacionCrediticia(orden.CUITCliente);

                if (!orden.AprobadoCredito)
                {
                    NotificarSituacionCrediticia(orden);
                }

                orden.ActualizarEstado();
            }
        }

        private bool ObtenerSituacionCrediticia(string CUIT)
        {
            return true;
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

            var cliente = repositorio.Obtener<Cliente>(orden.Cliente_Id);

            string asunto = string.Concat("Orden de carga #", orden.Id);
            string cuerpo = string.Format("Orden de carga {0} de cliente {1} no pasó validaciones crediticias.", orden.Id, cliente.RazonSocial);

            EmailSender.EnviarMail(mails, asunto, cuerpo, null, null, null, null);

            return true;
        }
        #endregion


        private void EnviarASAP(OrdenDeCarga orden)
        {
            if (orden.Estado == EstadoOrdenDeCarga.Confirmado)
            {
                orden.InformadaSAP = true;

                orden.ActualizarEstado();
            }
        }


    }
}
