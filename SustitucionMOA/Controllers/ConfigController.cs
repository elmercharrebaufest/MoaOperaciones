using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SustitucionMOACrypting;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ConfigController : Controller
    {
        public ActionResult encrypt(string value)
        {
            string ip = Request.Url.Host;
            if (ip != "localhost" && ip != "127.0.0.1") {
                return Json(new { error = "No valido" }, JsonRequestBehavior.AllowGet);
            }
            string encryptedValue = CryptoServiceProvider.Encrypt(value);
            return Json(new{ data = encryptedValue }, JsonRequestBehavior.AllowGet);
        }
    }
}