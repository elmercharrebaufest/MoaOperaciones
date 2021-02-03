using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
namespace SustitucionMOA.Controllers
{
    public class OrdenDeCargaController : BaseController
    {
        readonly IOrdenDeCargaService ordenDeCargaService;
        public OrdenDeCargaController(IOrdenDeCargaService ordenDeCargaService)
        {
            this.ordenDeCargaService = ordenDeCargaService;
        }

        [HttpPost]
        public ActionResult Agregar(string ordenDeCargaJson)
        {
            try
            {
                var ordenDeCarga = JsonConvert.DeserializeObject<OrdenDeCarga>(ordenDeCargaJson);

                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.Agregar(ordenDeCarga, mailUsuario) });
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


        //[CustomPermisoAuthorizeAttribute(Roles = Permiso.c)]
        [HttpGet]
        public ActionResult GetListado()
        {
            try
            {
                var mailUsuario = SessionPersister.getUsername();

                return JsonCustom(new { data = ordenDeCargaService.Listar(mailUsuario) });
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