using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
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
        //    Mock<ControllerContext> cc = new Mock<ControllerContext>();
        //    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();

        //   // string filePath = Path.GetFileName("/20362736251/15560/constanciaCUIT/AFIP - Administración Federal de Ingresos Públicos.pdf");
        //    FileStream fileStream = new FileStream(filePath, FileMode.Open);
        //    Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
        //    file1.Setup(d => d.FileName).Returns("Cotizacion.xlsx");
        //    file1.Setup(d => d.InputStream).Returns(fileStream);
        //    file1.Setup(d => d.ContentLength).Returns(Convert.ToInt32(fileStream.Length));

        //    cc.Setup(d => d.HttpContext.Request.Files.Count).Returns(1);
        //    cc.Setup(d => d.HttpContext.Request.Files[0]).Returns(file1.Object);
        //    target.ControllerContext = cc.Object;

        //    var result = target.GrabarCotizacion(json) as JsonResult;    
        //    var a = serializer.Serialize(result);          
        //    Assert.AreEqual(1, 1);
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(JsonRequestBehavior.AllowGet, result.JsonRequestBehavior);
        //}

        [Test]
        public void ObtenerServiciosSapTest()
        {
            var tablaSapDtoList = new List<TablaSapDto>
            {
                new TablaSapDto { Id = 1, Tabla = "Tabla1", Codigo = "3213213123", CodigoSap = "3213213123", Descripcion = "Tornillo 3/4", IdPadre = 2 },
                new TablaSapDto { Id = 2, Tabla = "Tabla2", Codigo = "3213213", CodigoSap = "3213213", Descripcion = "Balde", IdPadre = 2},
            };

            comprasServiceMock.Setup(servicio => servicio.ObtenerServiciosSap()).Returns(tablaSapDtoList);

            // Act
            var result = target.ObtenerServiciosSap();

            // Assert
            Assert.NotNull(result);
            var data = (dynamic)((JsonResult)result).Data;
            var propiedad = data.GetType().GetProperties()[0];
            var valor = (List<TablaSapDto>)propiedad.GetValue(data);
            Assert.AreEqual(valor.Count, 2);
            Assert.AreEqual(tablaSapDtoList.Count, valor.Count);
        }

        //[Test]
        //public void AutocompleteTablaSapTest()
        //{
        //    var tabla = "Tabla1";
        //    var valor = "valorBuscado";

        //    var tablaSapDtoList = new List<TablaSapDto>
        //    {
        //        new TablaSapDto { Id = 1, Tabla = "Tabla1", Codigo = "3213213123", CodigoSap = "3213213123", Descripcion = "Tornillo 3/4", IdPadre = 2 },
        //        new TablaSapDto { Id = 2, Tabla = "Tabla2", Codigo = "3213213", CodigoSap = "3213213", Descripcion = "Balde", IdPadre = 2},
        //    };

        //    comprasServiceMock.Setup(servicio => servicio.AutocompleteTablaSap(tabla, valor)).Returns(tablaSapDtoList);

        //    // Act
        //    var result = target.AutocompleteTablaSap(tabla, valor) as JsonResult;


        //    // Assert
        //    Assert.NotNull(result);
        //    var data = (dynamic)((JsonResult)result).Data;
        //    var propiedad = data.GetType().GetProperties()[0];
        //    var valor1 = (List<TablaSapDto>)propiedad.GetValue(data);
        //    Assert.AreEqual(valor1.Count, 2);
        //    Assert.AreEqual(tablaSapDtoList.Count, valor1.Count);
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

            comprasServiceMock.Setup(s => s.ObtenerAdjudicacion(It.IsAny<string>())).Returns(expected);

            var result = target.ObtenerAdjudicacion("");

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

        [Test]
        public void ObtenerOrdenDeCompraTest()
        {
            // Arrange
            var expected = new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = "OC123",
                    CodigoProveedor = "PROV456",
                    RazonSocialProveedor = "Proveedor Example",
                    CUITProveedor = "123456789",
                    Moneda = "USD",
                    MontoTotal = 1000.0m,
                    CreadoPor = "Usuario",
                    ClaseDocumento = "ClaseDoc",
                    Tipo = "TipoExample",
                    FechaCreacion = DateTime.Now,
                    TipoDocCompras = "TipoDocCompras",
                    Usuario_Id = 1
                },
                // Posiciones = () 
                // Error = ()
            };

            string nroOC = "yourTestNroOC";
            comprasServiceMock.Setup(s => s.ObtenerOrdenDeCompra(It.IsAny<string>())).Returns(expected);

            // Act
            var result = target.ObtenerOrdenDeCompra(nroOC);

            // Assert
            Assert.IsTrue(result is JsonResult);
            var data = (dynamic)((JsonResult)result).Data;
            var propiedad = data.GetType().GetProperties()[0];
            var valor = (OrdenDeCompraSAPDto)propiedad.GetValue(data);
            OrdenDeCompraSAPDto actual = valor;

            Assert.AreEqual(expected.Cabecera.OrdenDeCompra, actual.Cabecera.OrdenDeCompra);
            Assert.AreEqual(expected.Cabecera.CodigoProveedor, actual.Cabecera.CodigoProveedor);
            Assert.AreEqual(expected.Cabecera.RazonSocialProveedor, actual.Cabecera.RazonSocialProveedor);
            Assert.AreEqual(expected.Cabecera.CUITProveedor, actual.Cabecera.CUITProveedor);
            Assert.AreEqual(expected.Cabecera.Moneda, actual.Cabecera.Moneda);
            Assert.AreEqual(expected.Cabecera.MontoTotal, actual.Cabecera.MontoTotal);
            Assert.AreEqual(expected.Cabecera.CreadoPor, actual.Cabecera.CreadoPor);
            Assert.AreEqual(expected.Cabecera.ClaseDocumento, actual.Cabecera.ClaseDocumento);
            Assert.AreEqual(expected.Cabecera.Tipo, actual.Cabecera.Tipo);
            Assert.AreEqual(expected.Cabecera.FechaCreacion, actual.Cabecera.FechaCreacion);
            Assert.AreEqual(expected.Cabecera.TipoDocCompras, actual.Cabecera.TipoDocCompras);
            Assert.AreEqual(expected.Cabecera.Usuario_Id, actual.Cabecera.Usuario_Id);
            Assert.AreEqual(null, actual.Error);

            comprasServiceMock.Verify(s => s.ObtenerOrdenDeCompra(nroOC), Times.Once);
        }

        [Test]
        public void ListarProveedoresTest()
        {
            var filtro = "filtroBuscado";

            var proveedorDtoList = new List<ProveedorDto>
            {
                new ProveedorDto
                {
                    Id = 1,
                    CUIT = "11111111",
                    RazonSocial = "Proveedor 1",
                    CodigoProveedor = "COD1",
                    Mail = "proveedor1@example.com",
                    EstadoAprobacion = EstadoAprobacion.Aprobado,
                    Observaciones = "Observación proveedor 1",
                    IdDataAgro = 101,
                    IdComercialDataAgro = 201,
                    EstadoAprobacionDescripcion = "Aprobado",
                    HistorialAprobaciones = new List<ProveedorHistorialAprobacionDto>
                    {
                        new ProveedorHistorialAprobacionDto { Fecha = DateTime.Now.AddMonths(-1) },
                        new ProveedorHistorialAprobacionDto { Fecha = DateTime.Now.AddMonths(-2) }
                    },
                    Comercial = "Comercial 1",
                    SISAEstadoCuit = "Activo",
                    EstadoSIPER = "Activo en SIPER",
                    UltimaEdicion = DateTime.Now.AddDays(-10),
                    FechaSolicitud = DateTime.Now.AddMonths(-3),
                    RazonSocialCorredor = "Razón social corredor 1",
                    IdTipoUsuario = 101,
                    IdTipoProveedor = 201,
                    IngresoAPlanta = true,
                    AltaInterna = false,
                    ContieneDocumentacionFisica = true
                },
                new ProveedorDto
                {
                    Id = 2,
                    CUIT = "22222222",
                    RazonSocial = "Proveedor 2",
                    CodigoProveedor = "COD2",
                    Mail = "proveedor2@example.com",
                    EstadoAprobacion = EstadoAprobacion.Aprobado,
                    Observaciones = "Observación proveedor 2",
                    IdDataAgro = 102,
                    IdComercialDataAgro = 202,
                    EstadoAprobacionDescripcion = "Pendiente",
                    HistorialAprobaciones = new List<ProveedorHistorialAprobacionDto>
                    {
                        new ProveedorHistorialAprobacionDto { Fecha = DateTime.Now.AddMonths(-2) }
                    },
                    Comercial = "Comercial 2",
                    SISAEstadoCuit = "Activo",
                    EstadoSIPER = "Activo en SIPER",
                    UltimaEdicion = DateTime.Now.AddDays(-20),
                    FechaSolicitud = DateTime.Now.AddMonths(-4),
                    RazonSocialCorredor = "Razón social corredor 2",
                    IdTipoUsuario = 102,
                    IdTipoProveedor = 202,
                    IngresoAPlanta = false,
                    AltaInterna = true,
                    ContieneDocumentacionFisica = false
                }
            };


            usuarioServiceMock.Setup(servicio => servicio.ListarProveedores(filtro)).Returns(proveedorDtoList);

            // Act
            var result = target.ListarProveedores(filtro) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var data = (dynamic)((JsonResult)result).Data;
            var propiedad = data.GetType().GetProperties()[0];
            var valor = (List<ProveedorDto>)propiedad.GetValue(data);
            Assert.AreEqual(valor.Count, 2);
            // Realiza más aserciones según sea necesario.
        }

    }
}