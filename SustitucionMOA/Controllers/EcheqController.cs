using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
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
            try
            {
                return JsonCustom(new { data = service.ObtenerPendientePago(SessionPersister.Proveedor, fechaInicio, fechaFin, "") });

            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e) {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult MarcarContrato(string contrato, string pedido)
        {
            try
            {
                EcheqRequestModel request = new EcheqRequestModel(pedido, contrato);
                request.ProveedorId = SessionPersister.ProveedorId;
                request.CodigoProveedor = SessionPersister.Proveedor;
                request.UsuarioCreacionId = ObtenerUsuarioActual().Id;

                return JsonCustom(service.MarcarContrato(request));

            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DesmarcarContrato(string contrato, string pedido)
        {
            try
            {
                EcheqRequestModel request = new EcheqRequestModel(pedido, contrato);
                request.ProveedorId = SessionPersister.ProveedorId;
                request.CodigoProveedor = SessionPersister.Proveedor;
                request.UsuarioCreacionId = ObtenerUsuarioActual().Id;

                return JsonCustom(service.DesmarcarContrato(request));

            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult MarcarDocumento(string documento, string pedido, string contrato, string ejercicio)
        {
            try
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
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DesmarcarDocumento(string documento, string pedido, string contrato)
        {
            try
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
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AgregarApertura(string documento, string pedido, string contrato, string aperturaDtos)
        {
            try
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
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        public ActionResult ObtenerConfiguracion()
        {
            try
            {
                return JsonCustom(service.ObtenerConfiguracion());

            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ObtenerDatosReporte(string fechaInicio, string fechaFin)
        {
            try
            {
                string mailUsuario = ObtenerUsuarioActual().Mail;
                string codigoProveedor = SessionPersister.Proveedor; 

                return JsonCustom(new { data = service.ObtenerDatosReporte(fechaInicio, fechaFin, mailUsuario, codigoProveedor) });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}