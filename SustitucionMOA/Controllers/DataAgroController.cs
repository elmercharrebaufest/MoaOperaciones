using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class DataAgroController : BaseController
    {
        readonly IDataAgroService dataAgroService;


        public DataAgroController(IDataAgroService dataAgroService)
        {
            this.dataAgroService = dataAgroService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DATAAGROLOGIN)]
        public ActionResult GoToDataAgro()
        {
            return JsonCustom(new { data = dataAgroService.goToDataAgro(SessionPersister.Proveedor, SessionPersister.User.nombre) });
        }

        public ActionResult GetTipoCambiario()
        {
            return JsonCustom(new { data = dataAgroService.TraerTipoDeCambio() });
        }
    }
}