using SustitucionMOAAssets;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
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
    }
}