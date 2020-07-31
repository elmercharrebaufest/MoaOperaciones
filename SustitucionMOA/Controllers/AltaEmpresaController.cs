using Microsoft.Ajax.Utilities;
using SustitucionMOASecurity;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace SustitucionMOA.Controllers
{
    public class AltaEmpresaController : Controller
    {


        public class ParamInformeComercial
        {
            public int ProveedorId { get; set; }
            public List<ParamInformeComercialMaterial> Materiales { get; set; }
            public int CampañaId { get; set; }
            public string Campaña { get; set; }
            public bool EmplRelDep { get; set; }
            public string EmplRelDepCant { get; set; }
            public int? Rodados { get; set; }
            public string RodadosOtros { get; set; }
            public int? Chacra { get; set; }
            public string ChacraOtros { get; set; }
            public string AntigActividad { get; set; }
            public string ActuacionProd { get; set; }
            public string ClienteAnt { get; set; }
            public string Comentarios { get; set; }
            public string Domicilio { get; set; }
            public int? InformeComercialId { get; set; }

            public ParamInformeComercial()
            {
                Materiales = new List<ParamInformeComercialMaterial>();
            }

        }

        public class ParamInformeComercialMaterial
        {
            public int MaterialId { get; set; }
            public float? Toneladas { get; set; }
        }


        [HttpPost]
        public ActionResult TEST(ParamInformeComercial oParam, int ComercialID)
        {
            //var model = new ReportesModel();

            //var ComercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            //var entityError = mobjInformeComercialManager.GrabarInformeComercial(oParam, ComercialId);

            //if (!entityError.HayErrores)
            //{
            //    var oLstInformeComercial = new LstInformeComercial(reportesManager);

            //    var datos = mobjInformeComercialManager.GenerarInformeComercial(oParam, (int)entityError.InformeId);

            //    var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

            //    model.DownloadKey = Util.GetDownloadKey(identif);
            //}
            //else
            //{
            //    model.Errores = entityError.Errores;
            //}

            return new JsonResult()
            {
                Data = "TEST",
                MaxJsonLength = Int32.MaxValue
            };
        }



        public void GenerarInformeComercial(string EmplRelDep, string EmplRelDepCant, string Rodados, string RodadosOtros, string Chacra, string ChacraOtros, string AntigActividad, string ActuacionProd, string ClienteAnt, string Comentarios, string Domicilio)
        {
            var url = "http://test.dataagro.com.ar:9000/InformeComercial/Listar";
            //var url = "http://localhost:58280/api/AltaEmpresa/TEST";

            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential("dataagrop", "D@t@@gro2020.*", "MOLINOSAGRO"),
            };

            string ProveedorId = "1239";
            string CampañaId = "8";
            string Campaña = "19-20";
            string ComercialID = "10";

           /* EmplRelDep = EmplRelDep.IsNullOrWhiteSpace() ? "" : EmplRelDep;
            EmplRelDepCant = EmplRelDepCant.IsNullOrWhiteSpace() ? "" : EmplRelDepCant;
            Rodados = Rodados.IsNullOrWhiteSpace() ? "" : Rodados;
            RodadosOtros = RodadosOtros.IsNullOrWhiteSpace() ? "" : RodadosOtros;
            Chacra = Chacra.IsNullOrWhiteSpace() ? "" : Chacra;
            ChacraOtros = ChacraOtros.IsNullOrWhiteSpace() ? "" : ChacraOtros;
            AntigActividad = AntigActividad.IsNullOrWhiteSpace() ? "" : AntigActividad;
            ActuacionProd = ActuacionProd.IsNullOrWhiteSpace() ? "" : ActuacionProd;
            ClienteAnt = ClienteAnt.IsNullOrWhiteSpace() ? "" : ClienteAnt;
            Comentarios = Comentarios.IsNullOrWhiteSpace() ? "" : Comentarios;
            Domicilio = Domicilio.IsNullOrWhiteSpace() ? "" : Domicilio;*/

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ProveedorId", ProveedorId),
                new KeyValuePair<string, string>("CampañaId", CampañaId),
                new KeyValuePair<string, string>("Campaña", Campaña),
                new KeyValuePair<string, string>("EmplRelDep", EmplRelDep),
                new KeyValuePair<string, string>("EmplRelDepCant", EmplRelDepCant),
                new KeyValuePair<string, string>("Rodados", Rodados),
                new KeyValuePair<string, string>("RodadosOtros", RodadosOtros),
                new KeyValuePair<string, string>("Chacra", Chacra),
                new KeyValuePair<string, string>("ChacraOtros", ChacraOtros),
                new KeyValuePair<string, string>("AntigActividad", AntigActividad),
                new KeyValuePair<string, string>("ActuacionProd", ActuacionProd),
                new KeyValuePair<string, string>("ClienteAnt", ClienteAnt),
                new KeyValuePair<string, string>("Comentarios", Comentarios),
                new KeyValuePair<string, string>("Domicilio", Domicilio),
                new KeyValuePair<string, string>("InformeComercialId", "0"),
                new KeyValuePair<string, string>("ComercialID", ComercialID)

            });

            string downloadKey = "";
            using (var client = new HttpClient(httpClientHandler))
            //using (var client = new HttpClient())
            {
                try
                {

                    var task = client.PostAsync(url, formContent);

                    task.Wait();

                    var response = task.Result;

                    var stringContent = response.Content.ReadAsStringAsync();

                    downloadKey = stringContent.Result;

                }
                catch
                {

                }
            }

            using (var client = new HttpClient(httpClientHandler))
            //using (var client = new HttpClient())
            {
                try
                {

                    var task = client.PostAsync(url, formContent);

                    task.Wait();

                    var response = task.Result;

                    var stringContent = response.Content.ReadAsStringAsync();

                    downloadKey = stringContent.Result;

                }
                catch
                {

                }
            }




        }

        [HttpPost]
        public ActionResult SubirArchivo(FormCollection collection)
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                            //Use the following properties to get file's name, size and MIMEType
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                file.SaveAs(Server.MapPath("~/") + fileName); //File will be saved in application root
            }

            return RedirectToAction("UploadDocument");
        }

        [HttpPost]
        public ActionResult UploadFile(string fileKey)
        {
            try
            {
                var fileSubido = Request.Files[0];
                var fileName = Request.Form.Get("fileKey");
                if (fileSubido.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(fileSubido.FileName);
                    string _path = Path.Combine(Server.MapPath("~/"), _FileName);
                   
                    fileSubido.SaveAs(_path);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return View();
            }
        }
    }
}
