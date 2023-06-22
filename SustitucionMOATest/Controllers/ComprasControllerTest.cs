using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SustitucionMOATest.Controllers
{
    public class ComprasControllerTest
    {
        private ComprasController target;
        private Mock<IComprasService> comprasServiceMock;
        private Mock<IUsuarioService> usuarioServiceMock;
        private Mock<IRepositorio> repositorioMock;
        private string expectedJson;
        private string resultJson;
        private string mailUsuario = "mail@mail.com";
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            comprasServiceMock = new Mock<IComprasService>();
            usuarioServiceMock = new Mock<IUsuarioService>();
            repositorioMock = new Mock<IRepositorio>();
            
            this.serializer = new JavaScriptSerializer();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, mailUsuario));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

            target = new ComprasController(comprasServiceMock.Object, usuarioServiceMock.Object);
        }

        //[Test()]
        //public void GrabarCotizacionTest()
        //{
        //    string json = "{\"PeticionOfertaUsuarioId\":123,\"CotizacionPosiciones\":[{\"PeticionDeOfertaSolpPosicionId\":1,\"Precio\":10.5,\"MonedaId\":1,\"UnidadDeMedidaId\":2,\"Cantidad\":5,\"FechaDeEntrega\":\"2023-06-22T00:00:00\",\"PrecioTotal\":52.5,\"TotalPesos\":75.6,\"NoDisponible\":false},{\"PeticionDeOfertaSolpPosicionId\":2,\"Precio\":8.75,\"MonedaId\":1,\"UnidadDeMedidaId\":3,\"Cantidad\":3,\"FechaDeEntrega\":\"2023-06-25T00:00:00\",\"PrecioTotal\":26.25,\"TotalPesos\":39.8,\"NoDisponible\":true}],\"ObservacionTecnica\":\"Observación técnica\",\"ObservacionEconomica\":\"Observación económica\",\"ArchivosNuevos\":[{\"Id\":1,\"FileKey\":\"file1\",\"Nombre\":\"Archivo1.pdf\",\"Ruta\":\"/archivos/archivo1.pdf\"},{\"Id\":2,\"FileKey\":\"file2\",\"Nombre\":\"Archivo2.docx\",\"Ruta\":\"/archivos/archivo2.docx\"}],\"ArchivosGuardados\":[],\"CotizacionId\":456,\"EsFinalizado\":true,\"RespetaMateriales\":true,\"RespetaServicios\":false,\"CotizacionesHoras\":[{\"Id\":1,\"Cotizacion_Id\":456,\"Categoria\":\"Categoría 1\",\"CantidadPersonas\":2,\"HorasNormales\":8,\"HorasNocturnas\":2,\"HorasExtras\":4,\"Gremio\":\"Gremio 1\"},{\"Id\":2,\"Cotizacion_Id\":456,\"Categoria\":\"Categoría 2\",\"CantidadPersonas\":3,\"HorasNormales\":6,\"HorasNocturnas\":0,\"HorasExtras\":2,\"Gremio\":\"Gremio 2\"}],\"MonedaId\":1,\"UnidadDeMedidaId\":4,\"Cantidad\":10,\"FechaDeEntrega\":\"2023-06-30T00:00:00\",\"CotizacionSubposiciones\":[{\"CotizacionSubPosicionId\":1,\"Precio\":15.75,\"MonedaId\":1,\"CotizacionPosicionId\":1,\"UnidadDeMedidaId\":4,\"Cantidad\":2,\"SolpSubPosicionId\":10,\"PrecioTotal\":31.5},{\"CotizacionSubPosicionId\":2,\"Precio\":20.5,\"MonedaId\":1,\"CotizacionPosicionId\":1,\"UnidadDeMedidaId\":4,\"Cantidad\":3,\"SolpSubPosicionId\":11,\"PrecioTotal\":61.5}]}";
        //    var expected = JsonConvert.DeserializeObject<GuardarCotizacion>(json);

        //    usuarioServiceMock.Setup(u => u.GetUsuario(It.IsAny<string>())).Returns(new UsuarioDto { Id = 1 });
        //    comprasServiceMock.Setup(s => s.GrabarCotizacion(expected, null, true, 1)).Returns(new RespuestaGuardarSOLP());

        //    var files = new HttpFileCollectionBase(); // Agrega los archivos que deseas simular
        //    files.Add(new HttpPostedFileBaseMock("file1", "Archivo1.pdf", "/archivos/archivo1.pdf")); 
        //    files.Add(new HttpPostedFileBaseMock("file2", "Archivo2.pdf", "/archivos/archivo2.pdf")); // Establece el valor del Request.Files del controlador como el mock creado



        //    var result = target.GrabarCotizacion(json) as JsonResult;
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        //}

        [Test()]
        public void ObtenerPrecioTotalPosicionProveedorTest()
        {
            string json = "{\"PeticionOfertaUsuarioId\":123,\"CotizacionPosiciones\":[{\"PeticionDeOfertaSolpPosicionId\":1,\"Precio\":10.5,\"MonedaId\":1,\"UnidadDeMedidaId\":2,\"Cantidad\":5,\"FechaDeEntrega\":\"2023-06-22T00:00:00\",\"PrecioTotal\":52.5,\"TotalPesos\":75.6,\"NoDisponible\":false},{\"PeticionDeOfertaSolpPosicionId\":2,\"Precio\":8.75,\"MonedaId\":1,\"UnidadDeMedidaId\":3,\"Cantidad\":3,\"FechaDeEntrega\":\"2023-06-25T00:00:00\",\"PrecioTotal\":26.25,\"TotalPesos\":39.8,\"NoDisponible\":true}],\"ObservacionTecnica\":\"Observación técnica\",\"ObservacionEconomica\":\"Observación económica\",\"ArchivosNuevos\":[{\"Id\":1,\"FileKey\":\"file1\",\"Nombre\":\"Archivo1.pdf\",\"Ruta\":\"/archivos/archivo1.pdf\"},{\"Id\":2,\"FileKey\":\"file2\",\"Nombre\":\"Archivo2.docx\",\"Ruta\":\"/archivos/archivo2.docx\"}],\"ArchivosGuardados\":[],\"CotizacionId\":456,\"EsFinalizado\":true,\"RespetaMateriales\":true,\"RespetaServicios\":false,\"CotizacionesHoras\":[{\"Id\":1,\"Cotizacion_Id\":456,\"Categoria\":\"Categoría 1\",\"CantidadPersonas\":2,\"HorasNormales\":8,\"HorasNocturnas\":2,\"HorasExtras\":4,\"Gremio\":\"Gremio 1\"},{\"Id\":2,\"Cotizacion_Id\":456,\"Categoria\":\"Categoría 2\",\"CantidadPersonas\":3,\"HorasNormales\":6,\"HorasNocturnas\":0,\"HorasExtras\":2,\"Gremio\":\"Gremio 2\"}],\"MonedaId\":1,\"UnidadDeMedidaId\":4,\"Cantidad\":10,\"FechaDeEntrega\":\"2023-06-30T00:00:00\",\"CotizacionSubposiciones\":[{\"CotizacionSubPosicionId\":1,\"Precio\":15.75,\"MonedaId\":1,\"CotizacionPosicionId\":1,\"UnidadDeMedidaId\":4,\"Cantidad\":2,\"SolpSubPosicionId\":10,\"PrecioTotal\":31.5},{\"CotizacionSubPosicionId\":2,\"Precio\":20.5,\"MonedaId\":1,\"CotizacionPosicionId\":1,\"UnidadDeMedidaId\":4,\"Cantidad\":3,\"SolpSubPosicionId\":11,\"PrecioTotal\":61.5}]}";
            var expected = JsonConvert.DeserializeObject<GuardarCotizacion>(json);

            comprasServiceMock.Setup(s => s.ObtenerPrecioTotalPosicionProveedor(expected)).Returns(expected);

            var result = target.ObtenerPrecioTotalPosicionProveedor(json) as JsonResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        }

        [Test()]
        public void ObtenerAdjudicacionTest()
        {
            var expected = new AdjudicacionDto();
         
            comprasServiceMock.Setup(s => s.ObtenerAdjudicacion(It.IsAny<int>())).Returns(expected);

            var result = target.ObtenerAdjudicacion(1);

            Assert.NotNull(result);
            var data = (dynamic)((JsonResult)result).Data;
            var propiedad = data.GetType().GetProperties()[0];
            var valor = (AdjudicacionDto)propiedad.GetValue(data);
            Assert.AreEqual(valor, expected);
        }

        [Test()]
        public void ListarAdjudicacionesTest()
        {
            var expected = new List<AdjudicacionDto>
            {
                new AdjudicacionDto{ Id = 1}
            };

            comprasServiceMock.Setup(s => s.ListarAdjudicaciones(It.IsAny<int>())).Returns(expected);

            var result = target.ListarAdjudicaciones(1);

            Assert.NotNull(result);
            var data = (dynamic)((JsonResult)result).Data;
            var propiedad = data.GetType().GetProperties()[0];
            var valor = (List<AdjudicacionDto>)propiedad.GetValue(data);
            Assert.AreEqual(valor.Count, 1);
        }

    }
}

public class HttpPostedFileBaseMock : HttpPostedFileBase
{
    private readonly string _fileKey;
    private readonly string _nombre;
    private readonly string _ruta;
    public HttpPostedFileBaseMock(string fileKey, string nombre, string ruta)
    {
        _fileKey = fileKey;
        _nombre = nombre; _ruta = ruta;
    }

    public override string ContentType => throw new NotImplementedException();
    public override int ContentLength => throw new NotImplementedException();
    public override string FileName => _nombre;
    public override Stream InputStream => throw new NotImplementedException();
    public override void SaveAs(string filename) { throw new NotImplementedException(); }
    //public override string Key => _fileKey;
    //public override string Nombre => _nombre;
    //public override string Ruta => _ruta;
}
