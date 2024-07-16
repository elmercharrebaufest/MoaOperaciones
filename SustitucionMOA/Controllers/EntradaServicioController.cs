using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
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
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SustitucionMOA.Controllers
{
    public class EntradaServicioController : BaseController
    {
        private readonly IEntradaServicioService EntradaServicioService;
        private readonly IUsuarioService usuarioService;

        public EntradaServicioController(IEntradaServicioService entradaServicio, IUsuarioService usuarioService)
        {
            this.EntradaServicioService = entradaServicio;
            this.usuarioService = usuarioService;
        }

        private UsuarioDto ObtenerUsuarioActual()
        {
            string userMail = SessionPersister.getUsername();
            return usuarioService.GetUsuario(userMail);
        }

        //[ValidateInput(false)]
        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_SOLP)]
        public async Task<ActionResult> GetByProveedorAsync(EntradaServicioParamsDto parametros)
        {
            try
            {

                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}
                UsuarioDto usuarioActual = ObtenerUsuarioActual();
                List<EntradaServicioCabeceraDto> result = await EntradaServicioService.ObtenerEntradasServicioCompleta(parametros, usuarioActual);

                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        
        /// <summary>
        /// Servicio para obtener las entradas de servicios guardadas en aprobaciones.
        /// </summary>
        /// <param name="parametros"></param>
        /// <returns></returns>
        public ActionResult ObtenerESLocales(EntradaServicioParamsDto parametros)
        {
            try
            {
                UsuarioDto usuarioActual = ObtenerUsuarioActual();
                List<EntradaServicioCabeceraDto> result =  EntradaServicioService.ServicioAprobaciones_EntradasServicioCabecera(parametros, usuarioActual);

                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult DeleteById(EntradaServicioParamsDto parametros)
        {
            try
            {
                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}

                string result = EntradaServicioService.BorrarEntradaServicio(parametros);

                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }


        //public ActionResult Create(EntradaServicioCreateParamsDto parametros)
        //{
        //    try
        //    {
        //        // Este debe combinarse con permisos de usuario.
        //        //if (parametros.vendedor == "" || parametros.vendedor == null)
        //        //{
        //        //    parametros.vendedor = SessionPersister.Proveedor;
        //        //}

        //        string result = EntradaServicioService.CrearEntradaServicio(parametros);

        //        return JsonCustom(new { data = result });
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonCustom(new { error = ex.Message });
        //    }
        //}

        [ValidateInput(false)]
        public async Task<ActionResult> CreateAsync(string request)
        {
            try
            {

                var payload = JsonConvert.DeserializeObject<CreateEntradaServicioDto>(request);

                // Este debe combinarse con permisos de usuario.
                //if (parametros.vendedor == "" || parametros.vendedor == null)
                //{
                //    parametros.vendedor = SessionPersister.Proveedor;
                //}
                //MMSN-601
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");
                List<EntradaServicioCreateRespuestaDto> ret = new List<EntradaServicioCreateRespuestaDto>();
                
                int index = 0;

                foreach(EntradaServicioCreateParamsDto parametro in payload.parametros)
                { 
                    string solpedNumber = parametro.EntrySheetHeader.SolPedNumber[index];

                    var validacion = EntradaServicioService.ValidarIngresante(parametro, userMail, solpedNumber);
                    var result = new EntradaServicioCreateRespuestaDto();
                    if (validacion.Message == "Auto")
                    {
                        result = await EntradaServicioService.CrearEntradaServicio(parametro, userMail, payload.report, payload.IdAdjuntos, solpedNumber, parametro.EntrySheetHeader.Proveedor);
                    }
                    else if (validacion.Message == "Temporal")
                    {
                        result = EntradaServicioService.CrearEntradaServicioTemporal(parametro, userMail, payload.report, payload.IdAdjuntos, solpedNumber, parametro.EntrySheetHeader.Proveedor);
                    }
                    else
                    {
                        result = validacion;
                    }
                    ret.Add(result);

                    index++;
                }


                return JsonCustom(new { data = ret });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult RechazarEntradaDeServicio(string json)
        {
            try
            {
                var motivoRechazo = JsonConvert.DeserializeObject<EmailDetailCertificateDto>(json);
                var toRet = EntradaServicioService.RechazarEntradaDeServicio(motivoRechazo);
                return JsonCustom(new { data = toRet });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AprobarEntradaDeServicio(string nro_es_local, string Moneda)
        {
            try
            {
                var result = await EntradaServicioService.AprobarEntradaDeServicio(nro_es_local, Moneda);
                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ReasignarSuplente(string nro_es_local, string suplente)
        {
            try
            {
                var result = EntradaServicioService.ReasignarSuplente(nro_es_local, suplente);
                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ActualizarInformacionIngresante(IngresanteInfoEditableDto info)
        {
            try
            {
                var result = EntradaServicioService.ActualizarInformacionIngresante(info);
                return JsonCustom(new { data = result });
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}