using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    //[System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class BaseController : Controller
    {
        protected JsonResult JsonCustom(object data)
        {
            JsonResult json = Json(data, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        protected ContentResult ContentCustom(object data)
        {
            // Se serializa así para que tome bien los atributos JsonProperty
            return Content(JsonConvert.SerializeObject(data), "application/json");
        }
    }
}