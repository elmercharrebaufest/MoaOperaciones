using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOASecurity;
using SustitucionMOAUtils.DBMethods;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using SustitucionMOAValidator;

//AGREGO CAMBIO DE PRUEBA

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class CartaPorteController : BaseController
    {
        ICartaPorteService _cartaPorteService;

        public CartaPorteController(ICartaPorteService cartaPorteService)
        {
            _cartaPorteService = cartaPorteService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CARTA_PORTE)]
        public ActionResult getAplicaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_cartaPorteService.GetAplicaciones(SessionPersister.Proveedor, fechaInicio, fechaFin));

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CARTA_PORTE)]
        public ActionResult getDescargas(string periodo, string fechaInicio, string fechaFin)
        {

            return JsonCustom(_cartaPorteService.GetDescargas(SessionPersister.Proveedor, fechaInicio, fechaFin));

        }

        public ActionResult downloadAplicaciones(string periodo, string fechaInicio, string fechaFin)
        {
            return JsonCustom(_cartaPorteService.DownloadAplicaciones(SessionPersister.Proveedor, fechaInicio, fechaFin));

        }

        public ActionResult downloadDescargas(string periodo, string fechaInicio, string fechaFin)
        {

            return JsonCustom(_cartaPorteService.DownloadDescargas(SessionPersister.Proveedor, fechaInicio, fechaFin));

        }

        public JsonResult GetFotos(string cartaPorteId)
        {

            return JsonCustom(_cartaPorteService.GetFotos(cartaPorteId));

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CARTA_PORTE)]
        public JsonResult GetListaFotos(string cartaPorteIds)
        {

            List<string> cartaPorteIdList = cartaPorteIds.Split(',').ToList();

            return JsonCustom(_cartaPorteService.GetFotos(cartaPorteIdList));

        }


        public ActionResult DescargarFotos(string cartaPorteIds)
        {

            var outputMemStream = new MemoryStream();

            List<string> cartaPorteIdList = cartaPorteIds.Split(',').ToList();

            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                zipStream.SetLevel(3);

                foreach (CartaPorteFoto foto in _cartaPorteService.GetFotos(cartaPorteIdList))
                {
                    MemoryStream fotoMemoryStream = new MemoryStream(Convert.FromBase64String(foto.Foto));

                    ZipEntry entry = new ZipEntry(string.Concat(foto.CartaPorteID, ".jpg"));
                    entry.DateTime = DateTime.Now;
                    zipStream.PutNextEntry(entry);
                    StreamUtils.Copy(fotoMemoryStream, zipStream, new byte[4096]);
                    zipStream.CloseEntry();
                }

                zipStream.IsStreamOwner = false;
            }

            outputMemStream.Position = 0;

            return JsonCustom(File(outputMemStream.ToArray(), "application/zip", "ImagenesCartaPorte.zip"));

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CONSULTAR_CARTA_PORTE_DETALLE)]
        public ActionResult getDetalle(string cartaPorteId)
        {

            return JsonCustom(new { data = _cartaPorteService.GetDetalle(SessionPersister.Proveedor, cartaPorteId) });

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CARTA_PORTE_DETALLE)]
        public ActionResult downloadDetalle(string cartaPorteId)
        {

            return JsonCustom(_cartaPorteService.DownloadDetalle(SessionPersister.Proveedor, cartaPorteId));

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_CARTA_PORTE_DETALLE)]
        public ActionResult exportPDFCalidad(string numeroCCPP)
        {

            return JsonCustom(_cartaPorteService.DownloadPDFCalidad(SessionPersister.Proveedor, numeroCCPP));

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CREAR_FORMULARIO_CCPP)]
        public ActionResult getFormularioDropdowns()
        {

            return JsonCustom(_cartaPorteService.GetFormularioDropdowns());

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CREAR_FORMULARIO_CCPP)]
        public ActionResult getDataCTG(string valor)
        {

            return JsonCustom(new { data = _cartaPorteService.GetDataCTG(valor) });

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CREAR_FORMULARIO_CCPP)]
        public ActionResult getCompletedPDFTemplate(string formularioString, int paginaSeleccionada, HttpPostedFileBase file)
        {
            string fileName = "";

            var archivoBytes = new byte[0];
            try
            {
                fileName = file.FileName;
                InputValidator.formularioPDF(file);
                var streamLength = file.InputStream.Length;
                archivoBytes = new byte[streamLength];
                file.InputStream.Read(archivoBytes, 0, archivoBytes.Length);
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch
            {
                throw new InfoCustomException(InfoMsg.NoPDF);
            }
            var formulario = JsonConvert.DeserializeObject<CCPPFormulario>(formularioString);

            return JsonCustom(new { data = _cartaPorteService.GetCompletedPDFTemplate(formulario, paginaSeleccionada, archivoBytes) });

        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.CREAR_FORMULARIO_CCPP)]
        public ActionResult getTemplate(string formularioString)
        {

            var formulario = JsonConvert.DeserializeObject<CCPPFormulario>(formularioString);
            return JsonCustom(new { data = _cartaPorteService.GetTemplate(formulario) });

        }
        public ActionResult DetalleCalidades(string cartaPorte)
        {

            var codigoProveedor = SessionPersister.Proveedor;
            return JsonCustom(new { data = _cartaPorteService.GetDetalleCalidades(cartaPorte, codigoProveedor) });

        }
    }
}