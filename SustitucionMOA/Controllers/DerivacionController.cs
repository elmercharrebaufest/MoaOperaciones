using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Web.Mvc;
using SustitucionMOA.Utils;


namespace SustitucionMOA.Controllers
{
    public class DerivacionController : BaseController
    {
        private readonly ILogicaDerivacionAutomaticaService logicaDerivacionAutomaticaService;

        public DerivacionController(ILogicaDerivacionAutomaticaService logicaDerivacionAutomaticaService)
        {
            this.logicaDerivacionAutomaticaService = logicaDerivacionAutomaticaService;
        }

        /// <summary>
        /// Correr reasignación Manual - El usuario debe tener rol Administración. 
        /// </summary>
        /// <returns></returns>
        public ActionResult CorrerReasignacionManual()
        {
            try
            {
                string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

                if (logicaDerivacionAutomaticaService.isUserAllowed(userMail))
                {
                    string res = "";
                    try
                    {
                        res = logicaDerivacionAutomaticaService.CorrerProcesoReasignacion();
                    }
                    catch (Exception ex)
                    {
                        return JsonCustom(new { error = ex.Message });
                    }

                    if(res == "SinRegistros")
                    {
                        return JsonCustom(new { error = "No hay registros en la tabla UsuarioReasignacion para procesar." });
                    }

                    return JsonCustom(new { data = "Proceso ejecutado con éxito" });
                }
                else
                {
                    return JsonCustom(new { data = "Usuario " + userMail + " no autorizado para correr el proceso" });
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
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}