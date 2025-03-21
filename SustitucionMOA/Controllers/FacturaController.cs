using SustitucionMOAAssets;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SustitucionMOA.Controllers
{
    [Authorize]
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
                var mail = SessionPersister.Mail;
                var data = facturaService.SubirPDF(files, cuit, codigo, mail);
                return JsonCustom(new { data = data });
            }
            else
            {
                return Json(new { error = string.Format(ErrorMsg.ErrorArchivoRequerido, "Factura") }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CARGAR_FACT_PROV)]
        public ActionResult RegistrarCertificacion(string certificaciones, List<HttpPostedFileBase> files)
        {
            //parse the stringify certificaciones to a list of CertificacionDto
            var certificacionesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CertificacionDto>>(certificaciones);
            var mail = SessionPersister.Mail;
            var proveedorId = SessionPersister.ProveedorId;
            var cuit = SessionPersister.CUIT;
            var codigo = SessionPersister.Proveedor;
            var data = facturaService.RegistrarCertificacion(certificacionesList, mail, proveedorId, files, cuit, codigo);
            return JsonCustom(new { data = data });
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CARGAR_FACT_PROV)]
        public ActionResult VerificarSiExisteRegistro(string NRO_Certificacion)
        {
            var certificacion = facturaService.VerificarSiExisteRegistro(NRO_Certificacion);
            if (certificacion != null)
            {
                return Json(new { data = certificacion }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "No existe registro de certificacion" }, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CARGAR_FACT_PROV)]
        public ActionResult DescargarDocumentoAdjunto(string NRO_Certificacion)
        {
            var certificacion = facturaService.ObtenerCertificacion(NRO_Certificacion);
            if (certificacion == null)
            {
                return Json(new { error = "No existe registro de certificacion" }, JsonRequestBehavior.AllowGet);
            }

            string ruta = certificacion.Archivo.Ruta;
            byte[] fileBytes = System.IO.File.ReadAllBytes(ruta);
            string fileName = Path.GetFileName(ruta);

            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

    }
}