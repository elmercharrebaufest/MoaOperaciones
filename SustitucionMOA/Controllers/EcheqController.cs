using Newtonsoft.Json;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class EcheqController : BaseController
    {
        private readonly IEcheqService service;
        private readonly IUsuarioService usuarioService;

        public EcheqController(IEcheqService echeqService, IUsuarioService usuarioService)
        {
            this.service = echeqService;
            this.usuarioService = usuarioService;
        }


        public ActionResult ObtenerPendientePago(string fechaInicio, string fechaFin)
        {
            return JsonCustom(new { data = service.ObtenerPendientePago(SessionPersister.Proveedor, fechaInicio, fechaFin, "") });
        }

        [HttpPost]
        public ActionResult MarcarContrato(string contrato, string pedido)
        {
            EcheqRequestModel request = new EcheqRequestModel(pedido, contrato);
            request.ProveedorId = SessionPersister.ProveedorId;
            request.CodigoProveedor = SessionPersister.Proveedor;
            request.UsuarioCreacionId = ObtenerUsuarioActual().Id;

            return JsonCustom(service.MarcarContrato(request));
        }

        [HttpPost]
        public ActionResult DesmarcarContrato(string contrato, string pedido)
        {
            EcheqRequestModel request = new EcheqRequestModel(pedido, contrato);
            request.ProveedorId = SessionPersister.ProveedorId;
            request.CodigoProveedor = SessionPersister.Proveedor;
            request.UsuarioCreacionId = ObtenerUsuarioActual().Id;

            return JsonCustom(service.DesmarcarContrato(request));
        }

        [HttpPost]
        public ActionResult MarcarDocumento(string documento, string pedido, string contrato, string ejercicio)
        {
            EcheqRequestModel request = new EcheqRequestModel()
            {
                Documento = documento,
                Pedido = pedido,
                Contrato = contrato,
                ProveedorId = SessionPersister.ProveedorId,
                UsuarioCreacionId = ObtenerUsuarioActual().Id,
                CodigoProveedor = SessionPersister.Proveedor,
                Ejercicio = ejercicio
            };

            return JsonCustom(service.MarcarDocumento(request));
        }

        [HttpPost]
        public ActionResult DesmarcarDocumento(string documento, string pedido, string contrato)
        {
            EcheqRequestModel request = new EcheqRequestModel()
            {
                Documento = documento,
                Pedido = pedido,
                ProveedorId = SessionPersister.ProveedorId,
                UsuarioCreacionId = ObtenerUsuarioActual().Id,
                CodigoProveedor = SessionPersister.Proveedor,
                Contrato = contrato
            };

            return JsonCustom(service.DesmarcarDocumento(request));
        }

        [HttpPost]
        public ActionResult AgregarApertura(string documento, string pedido, string contrato, string aperturaDtos)
        {
            List<EcheqAperturaDto> aperturas = JsonConvert.DeserializeObject<List<EcheqAperturaDto>>(aperturaDtos);

            EcheqRequestModel request = new EcheqRequestModel()
            {
                Documento = documento,
                Pedido = pedido,
                ProveedorId = SessionPersister.ProveedorId,
                UsuarioCreacionId = ObtenerUsuarioActual().Id,
                CodigoProveedor = SessionPersister.Proveedor,
                Contrato = contrato,
                Apertura = aperturas
            };
            return JsonCustom(service.AgregarApertura(request));
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        public ActionResult ObtenerConfiguracion()
        {
            return JsonCustom(service.ObtenerConfiguracion());
        }

        public ActionResult ObtenerDatosReporte(string fechaInicio, string fechaFin)
        {
            string mailUsuario = ObtenerUsuarioActual().Mail;
            string codigoProveedor = SessionPersister.Proveedor;

            return JsonCustom(new { data = service.ObtenerDatosReporte(fechaInicio, fechaFin, mailUsuario, codigoProveedor) });
        }

    }
}