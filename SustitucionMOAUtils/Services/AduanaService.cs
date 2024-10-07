using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Credentials;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class AduanaService :IAduanaService
    {
        public AduanaService()
        {

        }

        public CamaraImageResponse ObtenerImagen(string url, string nombre)
        {

            byte[] buffer = new byte[100000];
            int read, total = 0;
            string user = "", pass = "";
            // create HTTP request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            // set user and pass
            string userConfig = CamCredential.getCamUser();
            string passConfig = CamCredential.getCamPass();
            if (userConfig != null && userConfig != "")
            {
                user = userConfig;
            }
            if (passConfig != null && passConfig != "")
            {
                pass = passConfig;
            }
            req.Credentials = new NetworkCredential(user, pass);
            try
            {
                // get response
                WebResponse resp = req.GetResponse();
                // get response stream
                Stream stream = resp.GetResponseStream();
                // read data from stream
                while ((read = stream.Read(buffer, total, 1000)) != 0)
                {
                    total += read;
                }
                // get bitmap
                Bitmap bmp = (Bitmap)Bitmap.FromStream(
                              new MemoryStream(buffer, 0, total));

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                return new CamaraImageResponse() { img = Convert.ToBase64String(ms.ToArray()), nombre = nombre };
            }
            catch(Exception e)
            {
                throw new InfoCustomException(InfoMsg.NoImagenCamara, e);
            }
        }

        public string ObtenerInfoMet()
        {

            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory + @"Template";
            string sourcePath = @"\\estmetsl\EMA";
            System.IO.Directory.CreateDirectory(startupPath);
            CopyFilesRecursively(new DirectoryInfo(sourcePath), new DirectoryInfo(startupPath));
            return "";
        }

        private void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                    CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
                foreach (FileInfo file in source.GetFiles())
                    if (!file.Name.Contains("LOGO_EMPR") && !file.Name.Contains("LOGO_MERC"))
                    {
                        file.CopyTo(Path.Combine(target.FullName, file.Name), true);
                    }
            }
            catch (Exception e) {
                Logger.Log.Error("Server", "Server", "Job", "Copy", e);
            }
        }
    }
}
