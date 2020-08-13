using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOARepositorio;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Services.Description;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace SustitucionMOA.Controllers
{
    public class AltaEmpresaController : BaseController
    {
        protected readonly IRepositorio repositorio;
        readonly IAltaEmpresaService altaEmpresaService;

        public AltaEmpresaController(IAltaEmpresaService altaEmpresaService, IRepositorio repositorio)
        {
            this.altaEmpresaService = altaEmpresaService;
            this.repositorio = repositorio;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult getEmpresas()
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.getEmpresas(EstadoAprobacion.AprobacionPendiente) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.ABM_EMPRESAS)]
        public ActionResult setEstadoAprobacion(int empresaId, EstadoAprobacion estado, string observacion)
        {
            try
            {
                return JsonCustom(new { data = altaEmpresaService.setEstadoAprobacion(empresaId, estado, observacion) });
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
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
