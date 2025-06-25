using SustitucionMOAAssets;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.Factura;
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
        public ActionResult RegistrarCertificacion(string gruposCertificaciones, List<HttpPostedFileBase> files)
        {
            var gruposCertificacionesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GrupoCertificaciones>>(gruposCertificaciones);
            var mail = SessionPersister.Mail;
            var proveedorId = SessionPersister.ProveedorId;
            var cuit = SessionPersister.CUIT;
            var codigo = SessionPersister.Proveedor;

            var data = facturaService.RegistrarCertificaciones(gruposCertificacionesList, mail, proveedorId, files, cuit, codigo);
            
            return JsonCustom(new { data });
        }

        [CustomPermisoAuthorize(Roles = Permiso.CARGAR_FACT_PROV)]
        public ActionResult GuardarFacturaDiferenciaTasaDeCambio(HttpPostedFileBase archivoFactura)
        {
            var mailUsuario = SessionPersister.Mail;
            var cuitUsuario = SessionPersister.CUIT;
            var codigoProveedor = SessionPersister.Proveedor;
            facturaService.GuardarFacturaPorDiferenciaTasaDeCambio(archivoFactura, cuitUsuario, codigoProveedor, mailUsuario);
            return JsonCustom(new {});
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CARGAR_FACT_PROV)]
        public ActionResult DescargarDocumentoAdjunto(int archivoId)
        {
            var archivo = facturaService.ObtenerArchivo(archivoId);
            if (archivo == null)
            {
                return Json(new { error = "No existe archivo solicitado" }, JsonRequestBehavior.AllowGet);
            }

            string ruta = archivo.Ruta;
            byte[] fileBytes = System.IO.File.ReadAllBytes(ruta);
            string fileName = Path.GetFileName(ruta);

            return JsonCustom(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName));
        }

    }
}