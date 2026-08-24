using SustitucionMOA.Utils;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Web.Mvc;


namespace SustitucionMOA.Controllers
{
    [Authorize]
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
            string userMail = ClaimsPrincipalExtension.GetClaimValue("emails");

            if (logicaDerivacionAutomaticaService.isUserAllowedDerivacion(userMail))
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

                if (res == "SinRegistros")
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

    }
}