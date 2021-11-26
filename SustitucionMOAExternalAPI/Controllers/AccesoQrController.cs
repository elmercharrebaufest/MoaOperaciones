using SustitucionMOAUtils.Logger;
using System;
using System.Web.Http;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Imaging;

namespace SustitucionMOAExternalAPI.Controllers
{
    public class AccesoQrController : ApiController
    {

        public AccesoQrController()
        {
        }

        [Authorize(Roles = "ACCESO QR")]
        public IHttpActionResult Get(string email)
        {
            try
            {
                Log.ExternalAPIInfo(string.Format("Se solicita qr para email {0}", email));

                //llamar al servicio de accesos para obtener el token del qr
                var tokenQr = email;

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(tokenQr, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                using (MemoryStream m = new MemoryStream())
                {
                    qrCodeImage.Save(m, ImageFormat.Png);
                    byte[] imageBytes = m.ToArray();

                    return Json<QrResult>(new QrResult { QrImage = Convert.ToBase64String(imageBytes) });
                }
            }
            catch(Exception ex)
            {
                Log.ExternalAPIError(ex);
                return Json<QrResult>(new QrResult { Errors = new List<string>() { string.Format("Hubo un error al generar el qr para {1}: {0}", ex.Message, email) } });
            }
        }
    }

    public class QrResult
    {
        public string QrImage { get; set; }
        public bool HasError { get { return this.Errors != null && this.Errors.Count > 0; } }
        public List<string> Errors { get; set; }
    }
}
