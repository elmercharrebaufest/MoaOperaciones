using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
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

        private Mock<HttpContextBase> Context = new Mock<HttpContextBase>();
        private Mock<HttpRequestBase> Request = new Mock<HttpRequestBase>();
        private string filePath = Path.GetFullPath(TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png");

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

        [Test]
        public void AutocompleteMaterialRFCTest()
        {

            string material = "MaterialEjemplo";
            string centro = "CentroEjemplo";
            string grupoDeCompras = "GrupoEjemplo";

            var expected = new RegistroInfoDto
            {
                Id = "1",
                Precio = 100.00m,
                Cantidad = 50,
                Vendedor = "Vendedor Ejemplo",
                NombreProveedor = "Proveedor de Ejemplo",
                Unidad = "Unidad de Medida",
                Moneda = "USD",
                Centro = "Centro de Operaciones",
                Fecha = "2023-09-29",
                Codigo = "ABC123",
                PosicionId = 1,
                DescripcionPosicion = "Descripción de la Posición",
                CantidadAdjudicacion = 25,
                Cuit = "123456789",
                Indice = 2,
                ProveedorId = 789,
                Numero = 9876,
                Deshabilitado = false,
                MonedaId = 1,
                UnidadId = 2,
                FechaUltimaCompra = "2023-09-28",
                FechaVigencia = "2023-10-15",
                MaterialCodigo = "MATERIAL123",
                NumeroOrdenDeCompra = "OC-12345",
                GrupoDeCompras = "Grupo de Compras A",
                OrganizacionDeCompra = "Organización de Compra B",
            };

            comprasServiceMock.Setup(s => s.ObtenerUltimoRegistroMaterial(material, centro, grupoDeCompras))
                            .Returns(expected);


            var result = target.AutocompleteMaterialRFC(material, centro, grupoDeCompras) as JsonResult;

            string expectedjson = JsonConvert.SerializeObject(expected);

            string resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.AreEqual(expectedjson, resultJson);
            // Verificar que el resultado sea un JsonResult
            Assert.IsNotNull(result);
            Assert.IsTrue(result is JsonResult);
        }

        [Test]
        public void ObtenerUltimaSolpTest()
        {
            usuarioServiceMock.Setup(s => s.GetUsuario(It.IsAny<string>())).Returns(new UsuarioDto());
            comprasServiceMock.Setup(s => s.ObtenerUltimaSolp(It.IsAny<int>())).Returns(new DatosUltimaSolpDto());

            var result = target.ObtenerUltimaSolp() as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);

            var expected = "{\"ContentEncoding\":null,\"ContentType\":null,\"Data\":{\"data\":{\"FiscalContrato\":\"Fiscal\",\"EmailFiscalContrato\":\"email@email.com\",\"Telefono\":\"12121212\",\"ClaseDocumento\":{\"Id\":12,\"Tabla\":\"ClaseDocumento\",\"Codigo\":\"21\",\"CodigoSap\":\"21\",\"Descripcion\":\"ClaseDocumento\",\"IdPadre\":1,\"FiltroComprador\":null,\"CodigoDescripcion\":\"21 - ClaseDocumento\"},\"TipoPosicion\":{\"Id\":12,\"Tabla\":\"TipoPosicion\",\"Codigo\":\"21\",\"Descripcion\":\"TipoPosicion\",\"IdPadre\":1,\"Padre\":null,\"CodigoVisualizacion\":null},\"Almacen\":{\"Id\":12,\"Tabla\":\"Almacen\",\"Codigo\":\"21\",\"CodigoSap\":\"21\",\"Descripcion\":\"Almacen\",\"IdPadre\":1,\"FiltroComprador\":null,\"CodigoDescripcion\":\"21 - Almacen\"},\"CuentaMayor\":{\"Id\":12,\"Tabla\":\"CuentaMayor\",\"Codigo\":\"21\",\"CodigoSap\":\"21\",\"Descripcion\":\"CuentaMayor\",\"IdPadre\":1,\"FiltroComprador\":null,\"CodigoDescripcion\":\"21 - CuentaMayor\"},\"CuentaMayorSP\":{\"Id\":12,\"Tabla\":\"CuentaMayorSP\",\"Codigo\":\"21\",\"CodigoSap\":\"21\",\"Descripcion\":\"CuentaMayorSP\",\"IdPadre\":1,\"FiltroComprador\":null,\"CodigoDescripcion\":\"21 - CuentaMayorSP\"},\"GrupoCompras\":{\"Id\":12,\"Tabla\":\"GrupoCompras\",\"Codigo\":\"21\",\"CodigoSap\":\"21\",\"Descripcion\":\"GrupoCompras1\",\"IdPadre\":1,\"FiltroComprador\":null,\"CodigoDescripcion\":\"21 - GrupoCompras1\"},\"Centro\":{\"Id\":12,\"Tabla\":\"Centro\",\"Codigo\":\"21\",\"CodigoSap\":\"21\",\"Descripcion\":\"Centro\",\"IdPadre\":1,\"FiltroComprador\":null,\"CodigoDescripcion\":\"21 - Centro\"}}},\"JsonRequestBehavior\":0,\"MaxJsonLength\":2147483647,\"RecursionLimit\":null}";
            var resultText = serializer.Serialize(result);

            Assert.AreEqual(expected.Normalize(), resultText.Normalize());
        }

        [Test]
        public void ObtenerReporteOrdenDeCompra_CasoExitoso()
        {
            // Arrange
            var nroOC = "12345";
            var fechaDesde = "2023-01-01";
            var fechaHasta = "2023-02-01";
            var codigoProveedor = "PROV123";

            comprasServiceMock.Setup(x => x.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor))
                .Returns(new List<OrdenDeCompraSAPDto>
                {
                new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = "OC123",
                        CodigoProveedor = "PROV456",
                        RazonSocialProveedor = "Proveedor XYZ",
                        Moneda = "USD",
                        MontoTotal = 1000,
                        CreadoPor = "Usuario123",
                        ClaseDocumento = "DocumentoClase123",
                        FechaCreacion = DateTime.Now,
                        Tipo = "Materiales",
                    },
                    Mensaje = "Mensaje de prueba",
                }

                });

            var result = target.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public void ObtenerReporteOrdenDeCompra_InfoCustomException()
        {
            // Arrange
            var nroOC = "12345";
            var fechaDesde = "2023-01-01";
            var fechaHasta = "2023-02-01";
            var codigoProveedor = "PROV123";

            comprasServiceMock.Setup(x => x.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor))
                .Throws(new InfoCustomException("Información personalizada"));

            var result = target.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public void GrabarPeticionDeOfertaVisualizacionPrecioOK()
        {

            var json = "{\"Id\":1,\"PeticionDeOferta_Id\":2,\"Archivo_Id\":3,\"UsuarioCreador_Id\":4,\"FechaCreacion\":\"2023-10-27T12:00:00\",\"Observacion\":\"Esta es una observación\",\"Adjuntos\":[{\"Id\":101,\"NombreArchivo\":\"archivo1.pdf\",\"Tamaño\":1024},{\"Id\":102,\"NombreArchivo\":\"archivo2.docx\",\"Tamaño\":2048}]}";

            var archivoMock = new Mock<HttpPostedFileBase>();
            archivoMock.Setup(a => a.FileName).Returns(filePath); // Configura el nombre del archivo
            archivoMock.Setup(a => a.ContentLength).Returns(1024); // Configura el tamaño del archivo       
            var files = new Mock<HttpFileCollectionBase>();
            files.Setup(f => f.Count).Returns(1); // Número de archivos en la colección
            files.Setup(f => f.Get(0)).Returns(archivoMock.Object); // Obtiene el archivo en la colección
            Request.Setup(r => r.Files).Returns(files.Object);
            Context.Setup(c => c.Request).Returns(Request.Object);
            target.ControllerContext = new ControllerContext(Context.Object, new System.Web.Routing.RouteData(), target);
            // Arrange      
            comprasServiceMock.Setup(s => s.GrabarPeticionDeOfertaVisualizacionPrecio(It.IsAny<PeticionDeOfertaVisualizacionPrecioDto>(), It.IsAny<HttpFileCollectionBase>()))
            .Returns(new Resultado { IdEntidad = 1 });
            usuarioServiceMock.Setup(s => s.GetUsuario(It.IsAny<string>()))
            .Returns(new UsuarioDto { Id = 1 });

            var result = target.GrabarPeticionDeOfertaVisualizacionPrecio(json) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void DevolverMonedaProveedorOK()
        {

            comprasServiceMock.Setup(x => x.DevolverMonedaProveedor(It.IsAny<string>())).Returns(new ProveedorComprasDto { Moneda = "ARP" });

            var result = target.DevolverMonedaProveedor(It.IsAny<string>()) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public void ListarTablaSapOK()
        {
            comprasServiceMock.Setup(x => x.ListarTablaSap(It.IsAny<List<string>>())).Returns(new List<TablaSapDto> { new TablaSapDto() });

            var result = target.ListarTablaSap(It.IsAny<string>()) as JsonResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public void ModificarOrdenDeCompraOkTest()
        {
            var json = "{\"Id\":0,\"Cotizacion_Id\":0,\"Solp_Id\":31745,\"NumeroOrdenDeCompra\":\"4123002078\",\"FechaCreacion\":\"/Date(1705028400000)/\",\"UsuarioCreador_Id\":0,\"UsuarioCreador\":null,\"Moneda_Id\":225,\"MonedaDescripcion\":\"Franco suizo\",\"MontoTotal\":0,\"AdjudicacionPosiciones\":[{\"Id\":0,\"Adjudicacion_Id\":0,\"CotizacionPosicion_Id\":0,\"Cantidad\":1,\"SolpPosicion_Id\":0,\"SubposicionesCompras\":null,\"MaterialComprasCodigo\":\"000000000050224392\",\"Indice\":1,\"Tarea\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"CentroComprasDescripcion\":null,\"TextoSuministro\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"Modelo\":\"\",\"UnidadDescripcion\":\"UNI\",\"MonedaDescripcion\":\"CHF\",\"PrecioUnidad\":0.6,\"MonedaId\":225,\"PrecioTotal\":0.6,\"FechaEntregaServicio\":\"2024-01-29T03:00:00.000Z\",\"PlazoDeOferta\":18,\"MaterialComprasDescripcion\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"MonedaCodigo\":\"CHF\",\"CentroComprasCodigo\":\"1029\",\"MaterialTextoAmpliado\":\"BULON CABEZA HEXAGONAL RW 5/8\",\"FechaEntregaServicioFormateado\":\"29.01.2024\",\"PlazoDeEntrega\":null,\"Eliminado\":false,\"UnidadId\":214,\"UnidadesDeMedida\":[{\"Id\":43,\"Tabla\":\"Unidad\",\"Codigo\":\"CEN\",\"CodigoSap\":\"CEN\",\"Descripcion\":\"CEN\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"CEN - CEN\"},{\"Id\":51,\"Tabla\":\"Unidad\",\"Codigo\":\"DOC\",\"CodigoSap\":\"DOC\",\"Descripcion\":\"DOC\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"DOC - DOC\"},{\"Id\":66,\"Tabla\":\"Unidad\",\"Codigo\":\"GRU\",\"CodigoSap\":\"GRU\",\"Descripcion\":\"GRU\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"GRU - GRU\"},{\"Id\":147,\"Tabla\":\"Unidad\",\"Codigo\":\"MIL\",\"CodigoSap\":\"MIL\",\"Descripcion\":\"MIL\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"MIL - MIL\"},{\"Id\":185,\"Tabla\":\"Unidad\",\"Codigo\":\"PAR\",\"CodigoSap\":\"PAR\",\"Descripcion\":\"PAR\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"PAR - PAR\"},{\"Id\":214,\"Tabla\":\"Unidad\",\"Codigo\":\"UNI\",\"CodigoSap\":\"UNI\",\"Descripcion\":\"UNI\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"UNI - UNI\"}],\"UnidadCodigo\":\"UNI\",\"UnidadMedida\":{\"Id\":214,\"Tabla\":null,\"Codigo\":\"UNI\",\"CodigoSap\":null,\"Descripcion\":null,\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"  \"},\"DireccionDeEntrega\":{\"RegionSap\":\"04\",\"Id\":12,\"CodigoSap\":\"04\",\"Descripcion\":\"\",\"PaisSap\":\"\"},\"EntregaFinal\":false,\"Moneda\":{\"Id\":0,\"Tabla\":null,\"Codigo\":\"CHF\",\"CodigoSap\":null,\"Descripcion\":\"Franco suizo\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"  Franco suizo\"},\"RegionCodigo\":\"04\",\"RegionId\":12,\"PaisSap\":\"\"}],\"PrecioFinal\":574.115136,\"Proveedor\":\"0057984261\",\"TipoPosicionCodigo\":\"MATERIALES\",\"TextoDeCabecera\":\"\",\"CondicionesDeEntrega\":\"\",\"CondicionesDePago\":\"\",\"Garantias\":\"22\",\"Centro\":\"Planta San Lorenzo\",\"CalleEntrega\":\"Benielli 398\",\"CodigoPostal\":\"2200\",\"PrecioBruto\":0,\"EstadoLiberacionCodigo\":null,\"EstadoLiberacionDetalle\":null,\"EsMonedaProveedor\":false,\"RegionSap\":0,\"CreadoAutomatico\":false,\"MonedaCodigo\":null,\"CondicionDePago\":{\"CondicionDeImportacionCodigo\":null,\"CondicionDeImportacionComplemento\":null,\"Codigo\":\"0030\",\"Descripcion\":\"Pagadero dentro de los 30 días sin DPP\",\"Id\":121554,\"CodigoDescripcion\":\"0030 - Pagadero dentro de los 30 días sin DPP\"},\"CondicionDeImportacion\":{\"Codigo\":\"\",\"Descripcion\":\"\",\"Id\":null,\"CodigoDescripcion\":null},\"CondicionDePagoCodigo\":null,\"CondicionDePagoId\":null,\"CondicionDeImportacionCodigo\":null,\"CondicionDeImportacionId\":null,\"CondicionDeImportacionDescripcion\":\"\",\"PagoEn1\":30,\"PagoEn2\":0,\"PagoEn3\":0,\"PagoEn1Porcentaje\":0,\"PagoEn2Porcentaje\":0}";

            comprasServiceMock.Setup(s => s.EditarOrdenDeCompra(It.IsAny<AdjudicacionDto>())).Returns(new ResultadoGenerico() { Descripcion = "Ok" });

            var result = target.ModificarOrdenDeCompra(json) as JsonResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void ModificarOrdenDeCompraInfoCustomExceptionTest()
        {
            var json = "{\"Id\":0,\"Cotizacion_Id\":0,\"Solp_Id\":31745,\"NumeroOrdenDeCompra\":\"4123002078\",\"FechaCreacion\":\"/Date(1705028400000)/\",\"UsuarioCreador_Id\":0,\"UsuarioCreador\":null,\"Moneda_Id\":225,\"MonedaDescripcion\":\"Franco suizo\",\"MontoTotal\":0,\"AdjudicacionPosiciones\":[{\"Id\":0,\"Adjudicacion_Id\":0,\"CotizacionPosicion_Id\":0,\"Cantidad\":1,\"SolpPosicion_Id\":0,\"SubposicionesCompras\":null,\"MaterialComprasCodigo\":\"000000000050224392\",\"Indice\":1,\"Tarea\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"CentroComprasDescripcion\":null,\"TextoSuministro\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"Modelo\":\"\",\"UnidadDescripcion\":\"UNI\",\"MonedaDescripcion\":\"CHF\",\"PrecioUnidad\":0.6,\"MonedaId\":225,\"PrecioTotal\":0.6,\"FechaEntregaServicio\":\"2024-01-29T03:00:00.000Z\",\"PlazoDeOferta\":18,\"MaterialComprasDescripcion\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"MonedaCodigo\":\"CHF\",\"CentroComprasCodigo\":\"1029\",\"MaterialTextoAmpliado\":\"BULON CABEZA HEXAGONAL RW 5/8\",\"FechaEntregaServicioFormateado\":\"29.01.2024\",\"PlazoDeEntrega\":null,\"Eliminado\":false,\"UnidadId\":214,\"UnidadesDeMedida\":[{\"Id\":43,\"Tabla\":\"Unidad\",\"Codigo\":\"CEN\",\"CodigoSap\":\"CEN\",\"Descripcion\":\"CEN\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"CEN - CEN\"},{\"Id\":51,\"Tabla\":\"Unidad\",\"Codigo\":\"DOC\",\"CodigoSap\":\"DOC\",\"Descripcion\":\"DOC\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"DOC - DOC\"},{\"Id\":66,\"Tabla\":\"Unidad\",\"Codigo\":\"GRU\",\"CodigoSap\":\"GRU\",\"Descripcion\":\"GRU\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"GRU - GRU\"},{\"Id\":147,\"Tabla\":\"Unidad\",\"Codigo\":\"MIL\",\"CodigoSap\":\"MIL\",\"Descripcion\":\"MIL\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"MIL - MIL\"},{\"Id\":185,\"Tabla\":\"Unidad\",\"Codigo\":\"PAR\",\"CodigoSap\":\"PAR\",\"Descripcion\":\"PAR\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"PAR - PAR\"},{\"Id\":214,\"Tabla\":\"Unidad\",\"Codigo\":\"UNI\",\"CodigoSap\":\"UNI\",\"Descripcion\":\"UNI\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"UNI - UNI\"}],\"UnidadCodigo\":\"UNI\",\"UnidadMedida\":{\"Id\":214,\"Tabla\":null,\"Codigo\":\"UNI\",\"CodigoSap\":null,\"Descripcion\":null,\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"  \"},\"DireccionDeEntrega\":{\"RegionSap\":\"04\",\"Id\":12,\"CodigoSap\":\"04\",\"Descripcion\":\"\",\"PaisSap\":\"\"},\"EntregaFinal\":false,\"Moneda\":{\"Id\":0,\"Tabla\":null,\"Codigo\":\"CHF\",\"CodigoSap\":null,\"Descripcion\":\"Franco suizo\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"  Franco suizo\"},\"RegionCodigo\":\"04\",\"RegionId\":12,\"PaisSap\":\"\"}],\"PrecioFinal\":574.115136,\"Proveedor\":\"0057984261\",\"TipoPosicionCodigo\":\"MATERIALES\",\"TextoDeCabecera\":\"\",\"CondicionesDeEntrega\":\"\",\"CondicionesDePago\":\"\",\"Garantias\":\"22\",\"Centro\":\"Planta San Lorenzo\",\"CalleEntrega\":\"Benielli 398\",\"CodigoPostal\":\"2200\",\"PrecioBruto\":0,\"EstadoLiberacionCodigo\":null,\"EstadoLiberacionDetalle\":null,\"EsMonedaProveedor\":false,\"RegionSap\":0,\"CreadoAutomatico\":false,\"MonedaCodigo\":null,\"CondicionDePago\":{\"CondicionDeImportacionCodigo\":null,\"CondicionDeImportacionComplemento\":null,\"Codigo\":\"0030\",\"Descripcion\":\"Pagadero dentro de los 30 días sin DPP\",\"Id\":121554,\"CodigoDescripcion\":\"0030 - Pagadero dentro de los 30 días sin DPP\"},\"CondicionDeImportacion\":{\"Codigo\":\"\",\"Descripcion\":\"\",\"Id\":null,\"CodigoDescripcion\":null},\"CondicionDePagoCodigo\":null,\"CondicionDePagoId\":null,\"CondicionDeImportacionCodigo\":null,\"CondicionDeImportacionId\":null,\"CondicionDeImportacionDescripcion\":\"\",\"PagoEn1\":30,\"PagoEn2\":0,\"PagoEn3\":0,\"PagoEn1Porcentaje\":0,\"PagoEn2Porcentaje\":0}";

            comprasServiceMock.Setup(s => s.EditarOrdenDeCompra(It.IsAny<AdjudicacionDto>())).Throws(new InfoCustomException("Mensaje de información"));

            var result = target.ModificarOrdenDeCompra(json) as JsonResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void ModificarOrdenDeCompraValidationCustomExceptionTest()
        {
            var json = "{\"Id\":0,\"Cotizacion_Id\":0,\"Solp_Id\":31745,\"NumeroOrdenDeCompra\":\"4123002078\",\"FechaCreacion\":\"/Date(1705028400000)/\",\"UsuarioCreador_Id\":0,\"UsuarioCreador\":null,\"Moneda_Id\":225,\"MonedaDescripcion\":\"Franco suizo\",\"MontoTotal\":0,\"AdjudicacionPosiciones\":[{\"Id\":0,\"Adjudicacion_Id\":0,\"CotizacionPosicion_Id\":0,\"Cantidad\":1,\"SolpPosicion_Id\":0,\"SubposicionesCompras\":null,\"MaterialComprasCodigo\":\"000000000050224392\",\"Indice\":1,\"Tarea\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"CentroComprasDescripcion\":null,\"TextoSuministro\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"Modelo\":\"\",\"UnidadDescripcion\":\"UNI\",\"MonedaDescripcion\":\"CHF\",\"PrecioUnidad\":0.6,\"MonedaId\":225,\"PrecioTotal\":0.6,\"FechaEntregaServicio\":\"2024-01-29T03:00:00.000Z\",\"PlazoDeOferta\":18,\"MaterialComprasDescripcion\":\"BULON CAB.HEXAG.8.8 RW 5/8\",\"MonedaCodigo\":\"CHF\",\"CentroComprasCodigo\":\"1029\",\"MaterialTextoAmpliado\":\"BULON CABEZA HEXAGONAL RW 5/8\",\"FechaEntregaServicioFormateado\":\"29.01.2024\",\"PlazoDeEntrega\":null,\"Eliminado\":false,\"UnidadId\":214,\"UnidadesDeMedida\":[{\"Id\":43,\"Tabla\":\"Unidad\",\"Codigo\":\"CEN\",\"CodigoSap\":\"CEN\",\"Descripcion\":\"CEN\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"CEN - CEN\"},{\"Id\":51,\"Tabla\":\"Unidad\",\"Codigo\":\"DOC\",\"CodigoSap\":\"DOC\",\"Descripcion\":\"DOC\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"DOC - DOC\"},{\"Id\":66,\"Tabla\":\"Unidad\",\"Codigo\":\"GRU\",\"CodigoSap\":\"GRU\",\"Descripcion\":\"GRU\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"GRU - GRU\"},{\"Id\":147,\"Tabla\":\"Unidad\",\"Codigo\":\"MIL\",\"CodigoSap\":\"MIL\",\"Descripcion\":\"MIL\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"MIL - MIL\"},{\"Id\":185,\"Tabla\":\"Unidad\",\"Codigo\":\"PAR\",\"CodigoSap\":\"PAR\",\"Descripcion\":\"PAR\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"PAR - PAR\"},{\"Id\":214,\"Tabla\":\"Unidad\",\"Codigo\":\"UNI\",\"CodigoSap\":\"UNI\",\"Descripcion\":\"UNI\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"UNI - UNI\"}],\"UnidadCodigo\":\"UNI\",\"UnidadMedida\":{\"Id\":214,\"Tabla\":null,\"Codigo\":\"UNI\",\"CodigoSap\":null,\"Descripcion\":null,\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"  \"},\"DireccionDeEntrega\":{\"RegionSap\":\"04\",\"Id\":12,\"CodigoSap\":\"04\",\"Descripcion\":\"\",\"PaisSap\":\"\"},\"EntregaFinal\":false,\"Moneda\":{\"Id\":0,\"Tabla\":null,\"Codigo\":\"CHF\",\"CodigoSap\":null,\"Descripcion\":\"Franco suizo\",\"IdPadre\":null,\"FiltroComprador\":null,\"CodigoDescripcion\":\"  Franco suizo\"},\"RegionCodigo\":\"04\",\"RegionId\":12,\"PaisSap\":\"\"}],\"PrecioFinal\":574.115136,\"Proveedor\":\"0057984261\",\"TipoPosicionCodigo\":\"MATERIALES\",\"TextoDeCabecera\":\"\",\"CondicionesDeEntrega\":\"\",\"CondicionesDePago\":\"\",\"Garantias\":\"22\",\"Centro\":\"Planta San Lorenzo\",\"CalleEntrega\":\"Benielli 398\",\"CodigoPostal\":\"2200\",\"PrecioBruto\":0,\"EstadoLiberacionCodigo\":null,\"EstadoLiberacionDetalle\":null,\"EsMonedaProveedor\":false,\"RegionSap\":0,\"CreadoAutomatico\":false,\"MonedaCodigo\":null,\"CondicionDePago\":{\"CondicionDeImportacionCodigo\":null,\"CondicionDeImportacionComplemento\":null,\"Codigo\":\"0030\",\"Descripcion\":\"Pagadero dentro de los 30 días sin DPP\",\"Id\":121554,\"CodigoDescripcion\":\"0030 - Pagadero dentro de los 30 días sin DPP\"},\"CondicionDeImportacion\":{\"Codigo\":\"\",\"Descripcion\":\"\",\"Id\":null,\"CodigoDescripcion\":null},\"CondicionDePagoCodigo\":null,\"CondicionDePagoId\":null,\"CondicionDeImportacionCodigo\":null,\"CondicionDeImportacionId\":null,\"CondicionDeImportacionDescripcion\":\"\",\"PagoEn1\":30,\"PagoEn2\":0,\"PagoEn3\":0,\"PagoEn1Porcentaje\":0,\"PagoEn2Porcentaje\":0}";

            comprasServiceMock.Setup(s => s.EditarOrdenDeCompra(It.IsAny<AdjudicacionDto>())).Throws(new ValidationCustomException("Mensaje de validación"));

            var result = target.ModificarOrdenDeCompra(json) as JsonResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void ListarClaseDocumentoOK()
        {
            comprasServiceMock.Setup(x => x.ListarClaseDocumento(It.IsAny<int>())).Returns(new List<int> { 1 });

            var result = target.ListarClaseDocumento(It.IsAny<int>());

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public void ActualizarProveedorVisibleEnSolicitanteOk()
        {

            comprasServiceMock.Setup(s => s.ActualizarProveedorVisibleEnSolicitante(It.IsAny<int>(), It.IsAny<bool>())).Returns(new Resultado() { Mensaje = "Ok" });

            var result = target.ActualizarProveedorVisibleEnSolicitante(It.IsAny<int>(), It.IsAny<bool>()) as JsonResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void ListarUsuarioSolicitanteOk()
        {
            comprasServiceMock.Setup(x => x.ListarUsuarioSolicitante()).Returns(new List<UsuarioDto>
            { new UsuarioDto { Mail = "bmelgarejo@prueba.com", UsuarioSap = "BRISAM" } });

            var result = target.ListarUsuarioSolicitante();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

    }
}