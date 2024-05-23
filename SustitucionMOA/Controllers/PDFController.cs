using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    [System.Web.Mvc.SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class PDFController : BaseController
    {
        protected readonly IPDFService pDFService;

        public PDFController(IPDFService pDFService)
        {   
            this.pDFService = pDFService;
        }

        [CustomPermisoAuthorizeAttribute(Roles = Permiso.DESCARGAR_DOCUMENTO)]
        public ActionResult downloadDocumentPDF(string documento, string ejercicio)
        {
            try
            {
                if (documento.Split('|').Length == 1)
                {
                    return JsonCustom(pDFService.DescargarDocumentPDF(documento, ejercicio, SessionPersister.Proveedor, SessionPersister.Sociedad));
                }
                else
                {
                    List<SustitucionMOAModel.Models.WSMapMOA.Pdf> pdfs = new List<SustitucionMOAModel.Models.WSMapMOA.Pdf>();
                    
                    var outputMemStream = new MemoryStream();

                    using (var zipStream = new ZipOutputStream(outputMemStream))
                    {
                        zipStream.SetLevel(3);

                        foreach (var doc in documento.Split('|'))
                        {
                            var pdf = pDFService.DescargarDocumentPDF(doc, ejercicio, SessionPersister.Proveedor, SessionPersister.Sociedad);
                            if (pdf != null)
                            {
                                MemoryStream fotoMemoryStream = new MemoryStream(pdf.data);

                                ZipEntry entry = new ZipEntry(string.Concat(doc, ".pdf"));
                                entry.DateTime = DateTime.Now;
                                zipStream.PutNextEntry(entry);
                                StreamUtils.Copy(fotoMemoryStream, zipStream, new byte[4096]);
                                zipStream.CloseEntry();
                            }

                        }

                        zipStream.IsStreamOwner = false;
                    }

                    outputMemStream.Position = 0;

                    return JsonCustom(File(outputMemStream.ToArray(), "application/zip", "Liquidaciones.zip"));
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
            catch (WSCustomException e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.ErrorWS }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, SessionPersister.getUsername(), this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
                return Json(new { error = ErrorMsg.Error }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}