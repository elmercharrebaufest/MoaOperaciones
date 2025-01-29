using SustitucionMOAAssets;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
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
        public ActionResult subirPDF(string factura, List<HttpPostedFileBase> files)
        {
            if (files.Any())
            {
                var cuit = SessionPersister.CUIT;
                var codigo = SessionPersister.Proveedor;
                var mail = SessionPersister.getUsername();
                var data = facturaService.SubirPDF(files, cuit, codigo, mail);
                return JsonCustom(new { data = data });
            }
            else
            {
                return Json(new { error = string.Format(ErrorMsg.ErrorArchivoRequerido, "Factura") }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}