using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SustitucionMOAUtils.Interfaces;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Logger;
using SustitucionMOASecurity;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.IO;
using SustitucionMOAModel.Entities;
using System.IO.Compression;

namespace SustitucionMOA.Controllers
{
    public class AdjuntosCertificacionesController : BaseController
    {
        private readonly IAdjuntosCertificacionesService _adjuntosCertificacionesService;

        public AdjuntosCertificacionesController(IAdjuntosCertificacionesService adjuntosCertificacionesService)
        {
            _adjuntosCertificacionesService = adjuntosCertificacionesService;
        }

        public async Task<ActionResult> Adjuntar()
        {
            try
            {
                var result = await _adjuntosCertificacionesService.AdjuntarAsync(Request.Files, "");

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
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


        public async Task<ActionResult> GetAdjuntos(string idES)
        {
            try
            {
                var result = await _adjuntosCertificacionesService.GetAdjuntos(idES);

                return Json(new { data = result }, JsonRequestBehavior.AllowGet);
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
