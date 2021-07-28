using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOA.Controllers
{
    public class GestionImpuestosController : BaseController
    {
        private readonly IGestionImpuestosService gestionImpuestosService;

        public GestionImpuestosController(IGestionImpuestosService gestionImpuestosService)
        {
            this.gestionImpuestosService = gestionImpuestosService;
        }

        public JsonResult ListarCabeceras()
        {
            return JsonCustom(gestionImpuestosService.ListarCabeceras());
        }

        public JsonResult ListarDetalles(int idCabecera)
        {
            return JsonCustom(gestionImpuestosService.ListarDetalles(idCabecera));
        }

        public JsonResult Editar(string detalleJson)
        {
            try
            {
                var detalle = JsonConvert.DeserializeObject<IngresosBrutosCoeficienteUnificadoDetalle>(detalleJson);

                return JsonCustom(gestionImpuestosService.EditarDetalles(detalle));
            }
            catch (InfoCustomException e)
            {
                return Json(new { info = e }, JsonRequestBehavior.AllowGet);
            }
            catch (ValidationCustomException e)
            {
                return Json(new { error = e }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}