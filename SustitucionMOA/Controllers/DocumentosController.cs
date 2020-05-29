using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class DocumentosController : Controller
    {
        // GET: Documentos
        public ActionResult index()
        {
            string fullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documentacion", "Legajos Molinos Agro S.A..pdf");

            byte[] fileBytes = GetFile(fullName);
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Legajos Molinos Agro S.A..pdf");
        }

        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
    }
}