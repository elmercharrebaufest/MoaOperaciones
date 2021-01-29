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
using SustitucionMOAModel.Models;

namespace SustitucionMOA.Controllers
{
    public class TicketPesadaController : BaseController
    {
        //readonly INotificacionService notificacionService;

        public TicketPesadaController()
        {
            //this.notificacionService = notificacionService;
        }
        //notificacionService.ObtenerNotificacion(notificacionId)
        public ActionResult Obtener(string ticketPesadaJson)
        {
            try
            {
                var notificacion = JsonConvert.DeserializeObject<ConsultaTicketPesada>(ticketPesadaJson);

                return JsonCustom(new { data = "" });
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