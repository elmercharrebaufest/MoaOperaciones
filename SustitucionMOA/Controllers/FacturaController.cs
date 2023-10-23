using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class FacturaController : BaseController
    {
        private readonly IFacturaService facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            this.facturaService = facturaService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CARGAR_FACT_PROV)]
        public ActionResult subirPDF(string factura, HttpPostedFileBase file)
        {
            try
            {
                try
                {
                    string extension = System.IO.Path.GetExtension(file.FileName);
                    if (extension.ToUpper() == ".PDF")
                    {
                        string folderPath = Server.MapPath("/") + "Facturas\\";

                        return JsonCustom(new { data = facturaService.SubirPDF(file, folderPath) });
                    }
                    else
                    {
                        return Json(new { error = ErrorMsg.ErrorArchivoFormato }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (ValidationCustomException e)
                {
                    return Json(new { error = e.Message }, JsonRequestBehavior.AllowGet);
                }
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