using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Export.CampoSustentable;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.GoogleDrive.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class CampoSustentableServiceTest
    {
        private CampoSustentableService target;
        private Mock<IRepositorioCampoSustentable> repositorioMock;
        private Mock<IExcelExportWrapper> excelExportWrapperMock;
        private Mock<IDataAgroService> dataAgroServiceMock;
        private Mock<ICampoSustentableGoogleDrive> googleDriveMock;
        private Mock<ICampoSustentablePdfGenerator> campoSustentablePdfGeneratorMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorioCampoSustentable>();
            excelExportWrapperMock = new Mock<IExcelExportWrapper>();
            dataAgroServiceMock = new Mock<IDataAgroService>();
            googleDriveMock = new Mock<ICampoSustentableGoogleDrive>(MockBehavior.Strict);
            campoSustentablePdfGeneratorMock = new Mock<ICampoSustentablePdfGenerator>();

            target = new CampoSustentableService(
                repositorioMock.Object,
                excelExportWrapperMock.Object,
                dataAgroServiceMock.Object,
                googleDriveMock.Object,
                campoSustentablePdfGeneratorMock.Object);
        }

        [Test()]
        public void AgregarConCampoNuevoTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var cuitProveedor = "233333333333";
            var campoSustentableId = 654;
            var cosechaId = 367;
            var campoCosechaId = 287;
            var UsarArchivoId = false;
            var nombreArchivo = $"{cuitProveedor}_{campoSustentableId}";

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = cuitProveedor,
                RazonSocial = "Test"
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };

            var cosecha = new Cosecha
            {
                Id = cosechaId,
                Nombre = "20-21",
                Inicio = DateTime.Now
            };

            repositorioMock
               .Setup(x => x.ObtenerUsuarioPorMail(It.Is<string>(m => m == mailUsuario)))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Obtener<Cosecha>(It.IsAny<int>()))
               .Returns(cosecha);

            var declaracion = new DeclaracionCampoSustentable
            {
                CUIT = cuitProveedor,
                RazonSocial = "Test",
                Cosecha_Id = 1,
                Cosecha = cosecha,
                FechaFirma = DateTime.Now.AddDays(2),
                Archivo = new Archivo
                {
                    Id = 1,
                    Ruta = "C:/ArchivosCampoSustentableTest/declaracion.pdf"
                }
            };

            repositorioMock
               .Setup(x => x.ObtenerDeclaracionDeProveedor(
                   It.Is<string>(cp => cp == cuitProveedor),
                   It.Is<int>(c => c == cosechaId)))
               .Returns(declaracion);

            var reporteCertificadorDto = new CampoReporteDTO
            {
                CUIT = cuitProveedor,
                Id = 654,
                NombreCosecha = "CosechaX",
                RazonSocial = "proveedor ASD"
            };

            repositorioMock
                .Setup(x => x.ObtenerReporteCertificador(
                    It.Is<int>(y => y == campoCosechaId),
                    It.Is<int>(y => y == proveedorId)))
                .Returns(reporteCertificadorDto);

            repositorioMock
                .Setup(x => x.Existe<CarpetasUCROPIT>(
                It.IsAny<Expression<Func<CarpetasUCROPIT, bool>>>()))
                .Returns(true);

            googleDriveMock
                .Setup(x => x.UploadFile(It.IsAny<GoogleDriveFileUploadRequest>(), It.IsAny<string>()))
                .Returns(string.Empty);

            repositorioMock
                .Setup(x => x.Obtener<CarpetasUCROPIT>(
                It.IsAny<Expression<Func<CarpetasUCROPIT, bool>>>()))
                .Returns(new CarpetasUCROPIT
                {
                    UrlSubida = "https://drive.google.com/drive/folders/abc123"
                });

            repositorioMock
                .Setup(r => r.Listar<CampoProveedor>(
                    It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>(),
                    It.IsAny<IEnumerable<Expression<Func<CampoProveedor, object>>>>()
                ))
                .Returns(new List<CampoProveedor>());

            repositorioMock
            .Setup(r => r.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()))
            .Returns<Expression<Func<Cosecha, bool>>>(expr =>
            {
                return expr.Compile().Invoke(cosecha) ? cosecha : null;
            });

            var campoCreado = new CampoProveedor
            {
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                Proveedor = proveedor,
                CUIT = cuitProveedor,
                RazonSocial = "Test",
                CampoCosecha_Id = campoCosechaId,
                CampoCosecha = new CampoCosecha
                {
                    Id = campoCosechaId,
                    CampoSustentable_Id = campoSustentableId,
                    Campo = new CampoSustentable { Nombre = "Test", Id = campoSustentableId },
                    Cosecha_Id = cosechaId,
                    Cosecha = new Cosecha { Id = cosechaId, Nombre = "20-21" }
                },
                EPA = true,
                BSVS2 = true,
                EUDR = false,
            };

            var kmzPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", "ValidExample.kmz");
            Assert.IsTrue(File.Exists(kmzPath), $"El archivo de test KMZ no existe: {kmzPath}");
            var fileStream = new FileStream(kmzPath, FileMode.Open, FileAccess.Read);
            var uploadedFileMock = new Mock<HttpPostedFileBase>();

            var fileEpaStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".\\TestFiles\\EPA.png"), FileMode.Open, FileAccess.Read);
            var uploadedFileEPAMock = new Mock<HttpPostedFileBase>();

            uploadedFileMock
                .Setup(f => f.ContentLength)
                .Returns(100);

            uploadedFileMock
                .Setup(f => f.FileName)
                .Returns("ValidExample.kmz");

            uploadedFileMock
                .Setup(f => f.InputStream)
                .Returns(fileStream);

            uploadedFileEPAMock
                .Setup(f => f.InputStream)
                .Returns(fileEpaStream);

            ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"] = TestContext.CurrentContext.TestDirectory;
            ConfigurationManager.AppSettings["EmailCarpetasUcropIt"] = TestContext.CurrentContext.TestDirectory;

            var result = target.Agregar(mailUsuario, campoCreado, uploadedFileMock.Object, UsarArchivoId, uploadedFileEPAMock.Object);

            repositorioMock
                .Verify(
                    x => x.ObtenerUsuarioPorMail(It.Is<string>(m => m == mailUsuario)),
                    Times.Once());

            repositorioMock
                .Verify(
                    x => x.ObtenerDeclaracionDeProveedor(
                        It.Is<string>(cp => cp == cuitProveedor),
                        It.Is<int>(c => c == cosecha.Id)),
                    Times.Exactly(2));

            repositorioMock
               .Verify(x => x.GuardarCambios(), Times.Exactly(3));

            repositorioMock
                 .Verify(x => x.Agregar(It.IsAny<CampoProveedor>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Agregar(It.IsAny<ArchivoCampoSustentable>()), Times.Once);

            repositorioMock
                .Verify(
                    x => x.ObtenerReporteCertificador(
                        It.Is<int>(y => y == campoCosechaId),
                        It.Is<int>(y => y == proveedorId)),
                    Times.Once);

            googleDriveMock.Verify(x => x.UploadFile(It.IsAny<GoogleDriveFileUploadRequest>(), It.IsAny<string>()), Times.Exactly(2));
            googleDriveMock.VerifyAll();

            var expected = new Resultado { IdEntidad = campoCosechaId, Mensaje = SuccessMsg.CampoSustentableAgregado };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EditarTest()
        {
            // Arrange
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var campoCosechaId = 3;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333"
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
            new Rol
            {
                Codigo = "TestRol",
                PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" }, new PermisoPorRol { Id = 1, Permiso = "EDICION CAMPOS CREADOS" } }
            }
        }
            };

            var tipoNormativaEPA = new TipoNormativa { Id = 3, Descripcion = "EPA" };
            var normativaEPA = new CampoCosechaNormativa
            {
                TipoNormativa = tipoNormativaEPA,
                TipoNormativa_Id = 3,
                ToneladasAprobadas = 10,
                MotivoRechazo = null
            };

            var campoCosecha = new CampoCosecha
            {
                Campo = new CampoSustentable { Nombre = "Test", Renspa = "123", Localidad_Id = 5 },
                Cosecha = new Cosecha { Nombre = "20-21" },
                CampoCosechaNormativas = new List<CampoCosechaNormativa> { normativaEPA }
            };

            var campoProveedor = new CampoProveedor
            {
                CampoCosecha_Id = campoCosechaId,
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Longitud = "1.1",
                Latitud = "2.2",
                Borrado = false,
                Proveedor = proveedor,
                Archivo = new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" },
                CampoCosecha = campoCosecha,
                EPA = true,
                BSVS2 = false,
                EUDR = false,
                EvidenciaPresentada = false
            };

            var campoEditado = new CampoProveedor
            {
                CampoCosecha_Id = campoCosechaId,
                Proveedor_Id = proveedorId,
                HectareasSoja = 200,
                HectareasTotales = 300,
                Longitud = "3.3",
                Latitud = "4.4",
                Borrado = false,
                EPA = true,
                BSVS2 = false,
                EUDR = false,
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Editado", Renspa = "456", Localidad_Id = 6 },
                    Cosecha = new Cosecha { Nombre = "21-22" },
                },
                EvidenciaPresentada = false
            };

            var fileStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var uploadedFile = new Mock<HttpPostedFileBase>();
            uploadedFile.Setup(f => f.ContentLength).Returns(100);
            uploadedFile.Setup(f => f.FileName).Returns("Example.kmz");
            uploadedFile.Setup(f => f.InputStream).Returns(fileStream);

            var fileEpaStream = new MemoryStream(new byte[] { 4, 5, 6 });
            var uploadedFileEpa = new Mock<HttpPostedFileBase>();
            uploadedFileEpa.Setup(f => f.FileName).Returns("EPA.pdf");
            uploadedFileEpa.Setup(f => f.InputStream).Returns(fileEpaStream);

            ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"] = TestContext.CurrentContext.TestDirectory;

            repositorioMock
                .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
                .Returns(usuario);

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
                .Returns(campoProveedor);

            repositorioMock
                .Setup(x => x.Listar<TipoNormativa>(null, 0, null, DirOrden.Asc, null))
                .Returns(new List<TipoNormativa> { tipoNormativaEPA });

            repositorioMock
                .Setup(x => x.Obtener<TipoNormativa>(It.IsAny<Expression<Func<TipoNormativa, bool>>>()))
                .Returns(tipoNormativaEPA);

            repositorioMock
                .Setup(x => x.Agregar(It.IsAny<CampoCosechaNormativa>()));

            repositorioMock
                .Setup(x => x.GuardarCambios());

            // Act
            var result = target.Editar(mailUsuario, campoEditado, uploadedFile.Object, uploadedFileEpa.Object);

            // Assert
            repositorioMock.Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(200, campoProveedor.HectareasSoja);
            Assert.AreEqual(300, campoProveedor.HectareasTotales);
            Assert.AreEqual("3.3", campoProveedor.Longitud);
            Assert.AreEqual("4.4", campoProveedor.Latitud);
            Assert.AreEqual("Editado", campoProveedor.CampoCosecha.Campo.Nombre);
            Assert.AreEqual("456", campoProveedor.CampoCosecha.Campo.Renspa);
            Assert.AreEqual(6, campoProveedor.CampoCosecha.Campo.Localidad_Id);
            Assert.AreEqual(true, campoProveedor.EPA);

            var expected = new Resultado { IdEntidad = campoCosechaId, Mensaje = SuccessMsg.CampoSustentableActualizado };
            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void BorrarSinToneladasAprobadasTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var campoCosechaId = 3;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333"
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };

            var campoProveedor = new CampoProveedor
            {
                CampoCosecha_Id = campoCosechaId,
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" },
                    CampoCosechaNormativas = new List<CampoCosechaNormativa>
                    {
                        new CampoCosechaNormativa
                        {
                            Id = 1,
                            CampoCosecha_Id = 100,
                            TipoNormativa_Id = 2,
                            ToneladasAprobadas = 0,
                            MotivoRechazo = null,
                            Validado = false,
                        }
                    }
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
               .Returns(campoProveedor);

            var expected = SuccessMsg.CampoSustentableBorrado;

            var result = target.Borrar(mailUsuario, campoCosechaId, proveedorId);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()), Times.Once);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(true, campoProveedor.Borrado);
        }

        [Test()]
        public void BorrarConToneladasAprobadasTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var campoCosechaId = 3;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333"
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };

            var campoProveedor = new CampoProveedor
            {
                CampoCosecha_Id = campoCosechaId,
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" },
                    CampoCosechaNormativas = new List<CampoCosechaNormativa>
                    {
                        new CampoCosechaNormativa
                        {
                            Id = 1,
                            CampoCosecha_Id = 100,
                            TipoNormativa_Id = 2,
                            ToneladasAprobadas = 50.5,
                            MotivoRechazo = null,
                            Validado = false,
                        }
                    }
                },
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
               .Returns(campoProveedor);

            var expected = "No se puede eliminar el campo debido a que ya tiene toneladas aprobadas";

            var ex = Assert.Throws<ValidationCustomException>(() => target.Borrar(mailUsuario, campoCosechaId, proveedorId));

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()), Times.Once);

            Assert.AreEqual(expected, ex.Message);
            Assert.AreEqual(false, campoProveedor.Borrado);
        }

        [Test()]
        public void VerificarDeclaracionAceptadaTest()
        {
            var cosechaId = 1;
            var proveedorId = 1;
            var CUIT = "233333333333";

            var cosecha = new Cosecha { Id = cosechaId, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = CUIT,
                RazonSocial = "Test",
            };

            var declaracion = new DeclaracionCampoSustentable
            {
                CUIT = CUIT,
                RazonSocial = "Test",
                Cosecha_Id = cosechaId,
                Cosecha = cosecha,
                FechaFirma = DateTime.Now,
                HectareasDeclaradas = 0,
                OpcionDeclarada = OpcionesDeclaracionCampoSustentable.Totalidad,
                Archivo = new Archivo
                {
                    Id = 1,
                    Ruta = "C:/ArchivosCampoSustentableTest/declaracion.pdf"
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<DeclaracionCampoSustentable, bool>>>()))
               .Returns(declaracion);

            repositorioMock
                 .Setup(x => x.Obtener<Cosecha>(It.IsAny<int>()))
                 .Returns(cosecha);

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            var result = target.VerificarDeclaracion(proveedorId, cosechaId, CUIT);

            var expected = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = true,
                CosechaActual = "19-20",
                CUIT = CUIT,
                RazonSocial = "Test",
                HectareasDeclaracionCampoSustentable = 0,
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad,
            };

            repositorioMock
                 .Verify(x => x.Obtener<Cosecha>(It.IsAny<int>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)), Times.Once);

            Assert.AreEqual(expected.CUIT, result.CUIT);
        }

        [Test()]
        public void VerificarDeclaracionVencidaTest()
        {
            var cosechaId = 1;
            var proveedorId = 1;
            var CUIT = "233333333333";

            var cosecha = new Cosecha { Id = cosechaId, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = CUIT,
                RazonSocial = "Test",
            };

            var declaracion = new DeclaracionCampoSustentable
            {
                CUIT = CUIT,
                RazonSocial = "Test",
                Cosecha_Id = cosechaId,
                Cosecha = cosecha,
                FechaFirma = DateTime.Now.AddDays(-10),
                HectareasDeclaradas = 0,
                OpcionDeclarada = OpcionesDeclaracionCampoSustentable.Totalidad,
                Archivo = new Archivo
                {
                    Id = 1,
                    Ruta = "C:/ArchivosCampoSustentableTest/declaracion.pdf"
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<DeclaracionCampoSustentable, bool>>>()))
               .Returns(declaracion);

            repositorioMock
                 .Setup(x => x.Obtener<Cosecha>(It.IsAny<int>()))
                 .Returns(cosecha);

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            var result = target.VerificarDeclaracion(proveedorId, cosechaId, CUIT);

            var expected = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = false,
                CosechaActual = "19-20",
                CUIT = CUIT,
                RazonSocial = "",
                HectareasDeclaracionCampoSustentable = 0,
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad,
            };

            repositorioMock
                 .Verify(x => x.Obtener<Cosecha>(It.IsAny<int>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void VerificarDeclaracionNulaTest()
        {
            var cosechaId = 1;
            var proveedorId = 1;
            var CUIT = "233333333333";

            var cosecha = new Cosecha { Id = cosechaId, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = CUIT,
                RazonSocial = "Test",
            };

            repositorioMock
                 .Setup(x => x.Obtener<Cosecha>(It.IsAny<int>()))
                 .Returns(cosecha);

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            var result = target.VerificarDeclaracion(proveedorId, cosechaId, CUIT);

            var expected = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = false,
                CosechaActual = "19-20",
                CUIT = CUIT,
                RazonSocial = "",
                HectareasDeclaracionCampoSustentable = 0,
                OpcionDeclaracionCampoSustentable = OpcionesDeclaracionCampoSustentable.Totalidad,
            };

            repositorioMock
                 .Verify(x => x.Obtener<Cosecha>(It.IsAny<int>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void AdjuntarArchivoSinDeclaracionTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";

            var cosechaId = 1;
            var CUIT = "233333333333";

            var cosecha = new Cosecha { Id = cosechaId, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = CUIT
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.FileName).Returns("file1.pdf");

            var ex = Assert.Throws<ValidationCustomException>(() => target.AdjuntarDeclaracionFirmada(mailUsuario, proveedorId, cosechaId, CUIT, fileMock.Object));

            var expected = "Debe haber imprimido la declaración antes de adjuntarla.";

            Assert.AreEqual(ex.Message, expected);
        }

        [Test()]
        public void AdjuntarArchivoConDeclaracionTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";

            var cosechaId = 1;
            var CUIT = "233333333333";

            var cosecha = new Cosecha { Id = cosechaId, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = CUIT
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var declaracion = new DeclaracionCampoSustentable
            {
                CUIT = CUIT,
                RazonSocial = "Test",
                Cosecha_Id = cosechaId,
                Cosecha = cosecha,
                FechaFirma = DateTime.Now,
                HectareasDeclaradas = 0,
                OpcionDeclarada = OpcionesDeclaracionCampoSustentable.Totalidad,
                Archivo = new Archivo
                {
                    Id = 1,
                    Ruta = ""
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<DeclaracionCampoSustentable, bool>>>()))
               .Returns(declaracion);

            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.FileName).Returns("file1.pdf");

            var result = target.AdjuntarDeclaracionFirmada(mailUsuario, proveedorId, cosechaId, CUIT, fileMock.Object);

            var expected = SuccessMsg.DeclaracionCampoSustentableFirmada;

            repositorioMock
                .Verify((x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId))), Times.Exactly(2));

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void ObtenerCosechasTest()
        {
            var cosechas = new List<Cosecha>
            {
                new Cosecha { Id = 1, Nombre="19-20", Inicio = DateTime.Now, Fin = DateTime.Now.AddDays(1)},
                new Cosecha { Id = 2, Nombre="20-21", Inicio = DateTime.Now, Fin = DateTime.Now.AddDays(1)}
            };

            repositorioMock
                 .Setup(x => x.Listar(
                                It.IsAny<Expression<Func<Cosecha, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<Cosecha, object>>>>()))
                 .Returns(cosechas);


            var result = target.ObtenerCosechas(true);


            repositorioMock.Verify(x => x.Listar(
                                It.IsAny<Expression<Func<Cosecha, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<Cosecha, object>>>>()), Times.Once);

            Assert.NotNull(result);
            CollectionAssert.AreEquivalent(cosechas, result);

        }

        [Test()]
        public void ObtenerCampoTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var campoCosechaId = 3;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333"
            };

            var usuario = new Usuario
            {
                Id = usuarioId,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };

            var campoProveedor = new CampoProveedor
            {
                CampoCosecha_Id = campoCosechaId,
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" }
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var expected = new CampoProveedorDto { NombreCampo = "Test", HectareasSoja = 100, HectareasTotales = 100, NombreCosecha = "20-21", ToneladasAprobadas = 0 };

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>(), It.IsAny<Expression<Func<CampoProveedor, CampoProveedorDto>>>()))
               .Returns(expected);


            var result = target.ObtenerCampo(mailUsuario, proveedorId, campoCosechaId);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>(), It.IsAny<Expression<Func<CampoProveedor, CampoProveedorDto>>>()), Times.Once);


            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ListarTienePermisoTodosCamposSustentableOk()
        {
            string mailUsuarioTest = "mail";

            DateTime hoy = new DateTime(2021, 8, 24);
            DateTime ayer = new DateTime(2021, 8, 23);

            Mock<Usuario> usuario1Mock = new Mock<Usuario>();
            usuario1Mock.Setup(x => x.Mail).Returns("mailmail");
            usuario1Mock.Setup(x => x.TienePermiso(PermisoEnum.VerTodosCamposSustentable)).Returns(false);

            Mock<Usuario> usuario2Mock = new Mock<Usuario>();
            usuario2Mock.Setup(x => x.Mail).Returns("mail");
            usuario2Mock.Setup(x => x.TienePermiso(PermisoEnum.VerTodosCamposSustentable)).Returns(true);

            Mock<Usuario> usuario3Mock = new Mock<Usuario>();
            usuario3Mock.Setup(x => x.Mail).Returns((string)null);
            usuario3Mock.Setup(x => x.TienePermiso(PermisoEnum.VerTodosCamposSustentable)).Returns(false);

            var usuariosList = new List<Usuario>
    {
        usuario1Mock.Object,
        usuario2Mock.Object,
        usuario3Mock.Object,
    };
            this.repositorioMock.Setup(repo => repo.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns<Expression<Func<Usuario, bool>>>(q => usuariosList.SingleOrDefault(q.Compile()));

            CampoSustentable campoSustentable1 = new CampoSustentable { IdScato = 1, Nombre = "Campo sustentable 1" };
            CampoSustentable campoSustentable2 = new CampoSustentable { IdScato = 2, Nombre = "Campo sustentable 2" };

            var campoCosecha1 = new CampoCosecha
            {
                Campo = campoSustentable1,
                Cosecha = new Cosecha { Nombre = "cosecha 1" },
                Cosecha_Id = 1,
                CampoCosechaNormativas = new List<CampoCosechaNormativa>
        {
            new CampoCosechaNormativa
            {
                Id = 1,
                CampoCosecha_Id = 1,
                TipoNormativa_Id = 2,
                TipoNormativa = new TipoNormativa
                {
                    Id = 2,
                    Descripcion = "2BSVS"
                },
                ToneladasAprobadas = 29,
                MotivoRechazo = "motivo rechazo 1",
                Validado = false,
            }
        }
            };
            var campoCosecha2 = new CampoCosecha
            {
                Campo = campoSustentable2,
                Cosecha = new Cosecha { Nombre = "cosecha 2" },
                Cosecha_Id = 2,
                CampoCosechaNormativas = new List<CampoCosechaNormativa>
        {
            new CampoCosechaNormativa
            {
                Id = 1,
                CampoCosecha_Id = 2,
                TipoNormativa_Id = 2,
                TipoNormativa = new TipoNormativa
                {
                    Id = 2,
                    Descripcion = "2BSVS"
                },
                ToneladasAprobadas = 80,
                MotivoRechazo = "motivo rechazo 2",
                Validado = false,
            }
        }
            };

            Proveedor proveedor1 = new Proveedor { Id = 1, CodigoProveedor = "prov1", RazonSocial = "Proveedor 1", CUIT = "cuit1" };
            Proveedor proveedor2 = new Proveedor { Id = 2, CodigoProveedor = "prov2", RazonSocial = "Proveedor 2", CUIT = "cuit2" };

            var campoProveedor1 = new CampoProveedor
            {
                CampoCosecha = campoCosecha1,
                CampoCosecha_Id = 1,
                HectareasSoja = 23,
                HectareasTotales = 93,
                Proveedor_Id = 1,
                Proveedor = proveedor1,
                CUIT = "cuit1",
                RazonSocial = "razon social 1",
                FechaCreacion = ayer,
                Borrado = false,
                BSVS2 = true,
                EPA = false,
                EUDR = false
            };
            var campoProveedor2 = new CampoProveedor
            {
                CampoCosecha = campoCosecha2,
                CampoCosecha_Id = 2,
                HectareasSoja = 39,
                HectareasTotales = 41,
                Proveedor_Id = 2,
                Proveedor = proveedor2,
                CUIT = "cuit2",
                RazonSocial = "razon social 2",
                FechaCreacion = hoy,
                Borrado = false,
                BSVS2 = true,
                EPA = false,
                EUDR = false
            };
            var campoProveedor3 = new CampoProveedor { Borrado = true, };

            var campoProveedorList = new List<CampoProveedor>
    {
        campoProveedor1,
        campoProveedor2,
        campoProveedor3,
    };

            this.repositorioMock
                .Setup(repo => repo.Listar(
                    It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(),
                    It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                    0, "FechaCreacion", DirOrden.Desc))
                .Returns<Expression<Func<CampoProveedor, CampoProveedor>>, Expression<Func<CampoProveedor, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => campoProveedorList.Where(filtro.Compile()).Select(proy.Compile()).OrderByDescending(x => x.FechaCreacion).ToList());

            var result = target.Listar(mailUsuarioTest);

            usuario2Mock.Verify(u => u.TienePermiso(PermisoEnum.ComercialCamposSustentables), Times.Once);
            usuario2Mock.Verify(u => u.TienePermiso(PermisoEnum.VerTodosCamposSustentable), Times.Once);

            repositorioMock.Verify(repo => repo.Listar(It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(), It.IsAny<Expression<Func<CampoProveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(2, result[0].IdScato);
            Assert.AreEqual("cosecha 2", result[0].NombreCosecha);
            Assert.AreEqual(39, result[0].HectareasSoja);
            Assert.AreEqual(41, result[0].HectareasTotales);
            Assert.AreEqual("Campo sustentable 2", result[0].NombreCampo);
            Assert.AreEqual(80, result[0].ToneladasAprobadas);
            Assert.AreEqual(2, result[0].CampoCosechaId);
            Assert.AreEqual(2, result[0].Proveedor.Id);
            Assert.AreEqual("prov2", result[0].Proveedor.CodigoProveedor);
            Assert.AreEqual("Proveedor 2", result[0].Proveedor.RazonSocial);
            Assert.AreEqual("prov2", result[0].CodigoProveedor);
            Assert.AreEqual("cuit2", result[0].CUITProveedor);
            Assert.AreEqual("razon social 2", result[0].RazonSocialProveedor);
            Assert.AreEqual(2, result[0].CosechaId);
            Assert.AreEqual("motivo rechazo 2", result[0].MotivoRechazo);
            Assert.AreEqual(hoy, result[0].FechaCreacion);

            Assert.AreEqual(1, result[1].IdScato);
            Assert.AreEqual("cosecha 1", result[1].NombreCosecha);
            Assert.AreEqual(23, result[1].HectareasSoja);
            Assert.AreEqual(93, result[1].HectareasTotales);
            Assert.AreEqual("Campo sustentable 1", result[1].NombreCampo);
            Assert.AreEqual(29, result[1].ToneladasAprobadas);
            Assert.AreEqual(1, result[1].CampoCosechaId);
            Assert.AreEqual(1, result[1].Proveedor.Id);
            Assert.AreEqual("prov1", result[1].Proveedor.CodigoProveedor);
            Assert.AreEqual("Proveedor 1", result[1].Proveedor.RazonSocial);
            Assert.AreEqual("prov1", result[1].CodigoProveedor);
            Assert.AreEqual("cuit1", result[1].CUITProveedor);
            Assert.AreEqual("razon social 1", result[1].RazonSocialProveedor);
            Assert.AreEqual(1, result[1].CosechaId);
            Assert.AreEqual("motivo rechazo 1", result[1].MotivoRechazo);
            Assert.AreEqual(ayer, result[1].FechaCreacion);
        }

        [Test]
        public void ListarNoTienePermisoTodosCamposSustentableOk()
        {
            string mailUsuarioTest = "mailmail";

            DateTime hoy = new DateTime(2021, 8, 24);
            DateTime ayer = new DateTime(2021, 8, 23);

            Proveedor proveedor1 = new Proveedor { Id = 1, CodigoProveedor = "prov1", RazonSocial = "Proveedor 1", CUIT = "cuit1" };
            Proveedor proveedor2 = new Proveedor { Id = 2, CodigoProveedor = "prov2", RazonSocial = "Proveedor 2", CUIT = "cuit2" };
            Proveedor proveedor3 = new Proveedor { Id = 3, CodigoProveedor = "prov3", RazonSocial = "Proveedor 3", CUIT = "cuit3" };

            Mock<Usuario> usuario1Mock = new Mock<Usuario>();
            usuario1Mock.Setup(x => x.Mail).Returns("mailmail");
            usuario1Mock.Setup(x => x.Proveedores).Returns(new List<Proveedor> { proveedor1, proveedor2 });
            usuario1Mock.Setup(x => x.TienePermiso(PermisoEnum.VerTodosCamposSustentable)).Returns(false);

            Mock<Usuario> usuario2Mock = new Mock<Usuario>();
            usuario2Mock.Setup(x => x.Mail).Returns("mail");
            usuario2Mock.Setup(x => x.TienePermiso(PermisoEnum.VerTodosCamposSustentable)).Returns(true);

            Mock<Usuario> usuario3Mock = new Mock<Usuario>();
            usuario3Mock.Setup(x => x.Mail).Returns((string)null);
            usuario3Mock.Setup(x => x.TienePermiso(PermisoEnum.VerTodosCamposSustentable)).Returns(true);

            var usuariosList = new List<Usuario>
            {
                usuario1Mock.Object,
                usuario2Mock.Object,
                usuario3Mock.Object,
            };
            this.repositorioMock.Setup(repo => repo.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns<Expression<Func<Usuario, bool>>>(q => usuariosList.SingleOrDefault(q.Compile()));

            CampoSustentable campoSustentable1 = new CampoSustentable { IdScato = 1, Nombre = "Campo sustentable 1" };
            CampoSustentable campoSustentable2 = new CampoSustentable { IdScato = 2, Nombre = "Campo sustentable 2" };

            var campoCosecha1 = new CampoCosecha { Campo = campoSustentable1, Cosecha = new Cosecha { Nombre = "cosecha 1" }, Cosecha_Id = 1,
                CampoCosechaNormativas = new List<CampoCosechaNormativa>
                    {
                        new CampoCosechaNormativa
                        {
                            Id = 1,
                            CampoCosecha_Id = 1,
                            TipoNormativa_Id = 3,
                            TipoNormativa = new TipoNormativa
                            {
                                Id = 3,
                                Descripcion = "2BSVS"
                            },
                            ToneladasAprobadas = 10,
                            MotivoRechazo = null,
                            Validado = false,
                        }
                    }
            };
            var campoCosecha2 = new CampoCosecha { Campo = campoSustentable2, Cosecha = new Cosecha { Nombre = "cosecha 2" }, Cosecha_Id = 2,
                CampoCosechaNormativas = new List<CampoCosechaNormativa>
                    {
                        new CampoCosechaNormativa
                        {
                            Id = 1,
                            CampoCosecha_Id = 1,
                            TipoNormativa_Id = 3,
                            TipoNormativa = new TipoNormativa
                            {
                                Id = 3,
                                Descripcion = "2BSVS"
                            },
                            ToneladasAprobadas = 10,
                            MotivoRechazo = null,
                            Validado = false,
                        }
                    }
            };

            var campoProveedor1 = new CampoProveedor { CampoCosecha = campoCosecha1, CampoCosecha_Id = 1, HectareasSoja = 23, HectareasTotales = 93, Proveedor_Id = 1, Proveedor = proveedor1, CUIT = "cuit1", RazonSocial = "razon social 1", FechaCreacion = ayer, Borrado = false,
                BSVS2 = true,
                EPA = false,
                EUDR = false
            };
            var campoProveedor2 = new CampoProveedor { CampoCosecha = campoCosecha2, CampoCosecha_Id = 2, HectareasSoja = 39, HectareasTotales = 41, Proveedor_Id = 2, Proveedor = proveedor2, CUIT = "cuit2", RazonSocial = "razon social 2", FechaCreacion = hoy, Borrado = false,
                BSVS2 = true,
                EPA = false,
                EUDR = false
            };

            var campoProveedorList = new List<CampoProveedor>
            {
                campoProveedor1,
                campoProveedor2,
                new CampoProveedor { Proveedor = proveedor3 },
                new CampoProveedor { Borrado = true, },
            };

            /*this.repositorioMock
                .Setup(repo => repo.Listar(
                    It.IsAny<Expression<Func<CampoProveedor, CampoProveedorListadoDto>>>(),
                    It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                    0, "FechaCreacion", DirOrden.Desc))
                                .Returns<Expression<Func<CampoProveedor, CampoProveedorListadoDto>>, Expression<Func<CampoProveedor, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => campoProveedorList.Where(filtro.Compile()).Select(proy.Compile()).OrderByDescending(x => x.FechaCreacion).ToList());
            */
            this.repositorioMock
       .Setup(repo => repo.Listar(
           It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(),
           It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
           0, "FechaCreacion", DirOrden.Desc))
       .Returns<Expression<Func<CampoProveedor, CampoProveedor>>, Expression<Func<CampoProveedor, bool>>, int, string, DirOrden>
           ((proy, filtro, maxResultados, orden, dirOrden) => campoProveedorList.Where(filtro.Compile()).Select(proy.Compile()).OrderByDescending(x => x.FechaCreacion).ToList());


            var result = target.Listar(mailUsuarioTest);

            usuario1Mock.Verify(u => u.TienePermiso(PermisoEnum.ComercialCamposSustentables), Times.Once);
            usuario1Mock.Verify(u => u.TienePermiso(PermisoEnum.VerTodosCamposSustentable), Times.Once);

            repositorioMock.Verify(repo => repo.Listar(It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(), It.IsAny<Expression<Func<CampoProveedor, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>()), Times.Once);

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(2, result[0].IdScato);
            Assert.AreEqual("cosecha 2", result[0].NombreCosecha);
            Assert.AreEqual(39, result[0].HectareasSoja);
            Assert.AreEqual(41, result[0].HectareasTotales);
            Assert.AreEqual("Campo sustentable 2", result[0].NombreCampo);
            Assert.AreEqual(10, result[0].ToneladasAprobadas);
            Assert.AreEqual(2, result[0].CampoCosechaId);
            Assert.AreEqual(2, result[0].Proveedor.Id);
            Assert.AreEqual("prov2", result[0].Proveedor.CodigoProveedor);
            Assert.AreEqual("Proveedor 2", result[0].Proveedor.RazonSocial);
            Assert.AreEqual("prov2", result[0].CodigoProveedor);
            Assert.AreEqual("cuit2", result[0].CUITProveedor);
            Assert.AreEqual("razon social 2", result[0].RazonSocialProveedor);
            Assert.AreEqual(2, result[0].CosechaId);
            Assert.AreEqual(null, result[0].MotivoRechazo);
            Assert.AreEqual(hoy, result[0].FechaCreacion);

            Assert.AreEqual(1, result[1].IdScato);
            Assert.AreEqual("cosecha 1", result[1].NombreCosecha);
            Assert.AreEqual(23, result[1].HectareasSoja);
            Assert.AreEqual(93, result[1].HectareasTotales);
            Assert.AreEqual("Campo sustentable 1", result[1].NombreCampo);
            Assert.AreEqual(10, result[1].ToneladasAprobadas);
            Assert.AreEqual(1, result[1].CampoCosechaId);
            Assert.AreEqual(1, result[1].Proveedor.Id);
            Assert.AreEqual("prov1", result[1].Proveedor.CodigoProveedor);
            Assert.AreEqual("Proveedor 1", result[1].Proveedor.RazonSocial);
            Assert.AreEqual("prov1", result[1].CodigoProveedor);
            Assert.AreEqual("cuit1", result[1].CUITProveedor);
            Assert.AreEqual("razon social 1", result[1].RazonSocialProveedor);
            Assert.AreEqual(1, result[1].CosechaId);
            Assert.AreEqual(null, result[1].MotivoRechazo);
            Assert.AreEqual(ayer, result[1].FechaCreacion);
        }

        [Test]
        public void ExportarCamposProveedoresAdminCampos()
        {
            // Arrange
            string mailUsuarioTest = "mail";
            DateTime hoy = new DateTime(2021, 8, 24);
            DateTime ayer = new DateTime(2021, 8, 23);

            var permisoEsAdminCampos = PermisoEnum.VerTodosCamposSustentable;

            var usuarioAdminMock = new Mock<Usuario>();
            usuarioAdminMock.Setup(x => x.Mail).Returns(mailUsuarioTest);
            usuarioAdminMock.Setup(x => x.TienePermiso(permisoEsAdminCampos)).Returns(true);

            var usuariosList = new List<Usuario>
            {
                usuarioAdminMock.Object
            };
            repositorioMock.Setup(repo => repo.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns<Expression<Func<Usuario, bool>>>(q => usuariosList.SingleOrDefault(q.Compile()));

            CampoSustentable campoSustentable1 = new CampoSustentable { IdScato = 1, Nombre = "Campo sustentable 1" };
            CampoSustentable campoSustentable2 = new CampoSustentable { IdScato = 2, Nombre = "Campo sustentable 2" };

            var tipoNormativa = new TipoNormativa { Id = 2, Descripcion = "2BSVS" };

            var normativa1 = new CampoCosechaNormativa
            {
                TipoNormativa = tipoNormativa,
                TipoNormativa_Id = 2,
                ToneladasAprobadas = 29,
                MotivoRechazo = "motivo rechazo 1"
            };
            var normativa2 = new CampoCosechaNormativa
            {
                TipoNormativa = tipoNormativa,
                TipoNormativa_Id = 2,
                ToneladasAprobadas = 80,
                MotivoRechazo = "motivo rechazo 2"
            };

            var campoCosecha1 = new CampoCosecha
            {
                Campo = campoSustentable1,
                Cosecha = new Cosecha { Nombre = "cosecha 1" },
                Cosecha_Id = 1,
                CampoCosechaNormativas = new List<CampoCosechaNormativa> { normativa1 }
            };
            var campoCosecha2 = new CampoCosecha
            {
                Campo = campoSustentable2,
                Cosecha = new Cosecha { Nombre = "cosecha 2" },
                Cosecha_Id = 2,
                CampoCosechaNormativas = new List<CampoCosechaNormativa> { normativa2 }
            };

            Proveedor proveedor1 = new Proveedor { Id = 1, CodigoProveedor = "prov1", RazonSocial = "Proveedor 1", CUIT = "cuit1" };
            Proveedor proveedor2 = new Proveedor { Id = 2, CodigoProveedor = "prov2", RazonSocial = "Proveedor 2", CUIT = "cuit2" };

            var campoProveedorList = new List<CampoProveedor>
            {
                new CampoProveedor { CampoCosecha = campoCosecha1, CampoCosecha_Id = 1, HectareasSoja = 23, HectareasTotales = 93, Proveedor_Id = 1, Proveedor = proveedor1, CUIT = "cuit1", RazonSocial ="razon social 1", FechaCreacion = ayer, Borrado = false, BSVS2 = true },
                new CampoProveedor { CampoCosecha = campoCosecha2, CampoCosecha_Id = 2, HectareasSoja = 39, HectareasTotales = 41, Proveedor_Id = 2, Proveedor = proveedor2, CUIT = "cuit2", RazonSocial ="razon social 2", FechaCreacion = hoy, Borrado = false, BSVS2 = true },
                new CampoProveedor { Borrado = true }
            };

            repositorioMock
                .Setup(repo => repo.Listar(It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(), It.IsAny<Expression<Func<CampoProveedor, bool>>>(), 0, "FechaCreacion", DirOrden.Desc))
                .Returns<Expression<Func<CampoProveedor, CampoProveedor>>, Expression<Func<CampoProveedor, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => campoProveedorList.Where(filtro.Compile()).Select(proy.Compile()).OrderByDescending(x => x.FechaCreacion).ToList());

            excelExportWrapperMock
                .Setup(excelExport => excelExport.ToExcel(It.IsAny<List<CampoSustentableExportDTO>>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .Returns("excel-content");

            // Act
            var result = target.ExportarCamposProveedores(mailUsuarioTest);

            // Assert
            excelExportWrapperMock.Verify(excelExport => excelExport.ToExcel(
                It.Is<List<CampoSustentableExportDTO>>(x => x.Count == 2 &&
                    x[0].IdScato == 2 &&
                    x[1].IdScato == 1),
                It.Is<string[]>(headers => headers.Contains("Id Scato")),
                "Reporte Campos Sustentables"),
                Times.Once);

            repositorioMock.Verify(repo => repo.Listar(It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(), It.IsAny<Expression<Func<CampoProveedor, bool>>>(), 0, "FechaCreacion", DirOrden.Desc), Times.Once);

            Assert.AreEqual("excel-content", result);
        }

        [Test]
        public void ExportarCamposProveedores()
        {
            // Arrange
            string mailUsuarioTest = "mail";
            DateTime hoy = new DateTime(2021, 8, 24);
            DateTime ayer = new DateTime(2021, 8, 23);

            var permisoEsAdminCampos = PermisoEnum.VerTodosCamposSustentable;

            Proveedor proveedor1 = new Proveedor { Id = 1, CodigoProveedor = "prov1", RazonSocial = "Proveedor 1" };
            Proveedor proveedor2 = new Proveedor { Id = 2, CodigoProveedor = "prov2", RazonSocial = "Proveedor 2" };

            // Usuario SIN permiso de admin campos
            var usuarioMock = new Mock<Usuario>();
            usuarioMock.Setup(x => x.Mail).Returns(mailUsuarioTest);
            usuarioMock.Setup(x => x.Proveedores).Returns(new List<Proveedor> { proveedor1, proveedor2 });
            usuarioMock.Setup(x => x.TienePermiso(permisoEsAdminCampos)).Returns(false);

            var usuariosList = new List<Usuario>
            {
                usuarioMock.Object
            };
            repositorioMock.Setup(repo => repo.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns<Expression<Func<Usuario, bool>>>(q => usuariosList.SingleOrDefault(q.Compile()));

            CampoSustentable campoSustentable1 = new CampoSustentable { IdScato = 1, Nombre = "Campo sustentable 1" };
            CampoSustentable campoSustentable2 = new CampoSustentable { IdScato = 2, Nombre = "Campo sustentable 2" };

            var tipoNormativaEPA = new TipoNormativa { Id = 3, Descripcion = "EPA" };
            var tipoNormativaBSVS2 = new TipoNormativa { Id = 2, Descripcion = "2BSVS" };

            var normativaEPA = new CampoCosechaNormativa
            {
                TipoNormativa = tipoNormativaEPA,
                TipoNormativa_Id = 3,
                ToneladasAprobadas = 10,
                MotivoRechazo = null
            };
            var normativaBSVS2 = new CampoCosechaNormativa
            {
                TipoNormativa = tipoNormativaBSVS2,
                TipoNormativa_Id = 2,
                ToneladasAprobadas = 20,
                MotivoRechazo = null
            };

            var campoCosecha1 = new CampoCosecha
            {
                Campo = campoSustentable1,
                Cosecha = new Cosecha { Nombre = "cosecha 1" },
                Cosecha_Id = 1,
                CampoCosechaNormativas = new List<CampoCosechaNormativa> { normativaEPA }
            };
            var campoCosecha2 = new CampoCosecha
            {
                Campo = campoSustentable2,
                Cosecha = new Cosecha { Nombre = "cosecha 2" },
                Cosecha_Id = 2,
                CampoCosechaNormativas = new List<CampoCosechaNormativa> { normativaBSVS2 }
            };

            var campoProveedorList = new List<CampoProveedor>
            {
                new CampoProveedor { CampoCosecha = campoCosecha1, CampoCosecha_Id = 1, HectareasSoja = 23, HectareasTotales = 93, Proveedor_Id = 1, Proveedor = proveedor1, CUIT = "cuit1", RazonSocial ="razon social 1", FechaCreacion = ayer, Borrado = false, EPA = true },
                new CampoProveedor { CampoCosecha = campoCosecha2, CampoCosecha_Id = 2, HectareasSoja = 39, HectareasTotales = 41, Proveedor_Id = 2, Proveedor = proveedor2, CUIT = "cuit2", RazonSocial ="razon social 2", FechaCreacion = hoy, Borrado = false, BSVS2 = true },
                new CampoProveedor { Borrado = true }
            };

            repositorioMock
                .Setup(repo => repo.Listar(It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(), It.IsAny<Expression<Func<CampoProveedor, bool>>>(), 0, "FechaCreacion", DirOrden.Desc))
                .Returns<Expression<Func<CampoProveedor, CampoProveedor>>, Expression<Func<CampoProveedor, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => campoProveedorList.Where(filtro.Compile()).Select(proy.Compile()).OrderByDescending(x => x.FechaCreacion).ToList());

            excelExportWrapperMock
                .Setup(excelExport => excelExport.ToExcel(It.IsAny<List<CampoSustentableExportBaseDTO>>(), It.IsAny<string[]>(), It.IsAny<string>()))
                .Returns("excel-content");

            // Act
            var result = target.ExportarCamposProveedores(mailUsuarioTest);

            // Assert
            excelExportWrapperMock.Verify(excelExport => excelExport.ToExcel(
                It.Is<List<CampoSustentableExportBaseDTO>>(x => x.Count == 2 &&
                    x[0].RazonSocial == "razon social 2" &&
                    x[1].RazonSocial == "razon social 1"),
                It.IsAny<string[]>(),
                "Reporte Campos Sustentables"),
                Times.Once);

            repositorioMock.Verify(repo => repo.Listar(It.IsAny<Expression<Func<CampoProveedor, CampoProveedor>>>(), It.IsAny<Expression<Func<CampoProveedor, bool>>>(), 0, "FechaCreacion", DirOrden.Desc), Times.Once);

            Assert.AreEqual("excel-content", result);
        }

        [Test]
        public void ObtenerRutaArchivoKMZOk()
        {
            int campoCosechaIdTest = 3;
            int proveedorIdTest = 2;

            var campoProveedorList = new List<CampoProveedor>
            {
                new CampoProveedor { CampoCosecha_Id = 1, Proveedor_Id = 1, Archivo = new Archivo { Ruta = "ruta1" } },
                new CampoProveedor { CampoCosecha_Id = 2, Proveedor_Id = 1, Archivo = new Archivo { Ruta = "ruta2" } },
                new CampoProveedor { CampoCosecha_Id = 3, Proveedor_Id = 1, Archivo = new Archivo { Ruta = "ruta3" } },
                new CampoProveedor { CampoCosecha_Id = 1, Proveedor_Id = 2, Archivo = new Archivo { Ruta = "ruta4" } },
                new CampoProveedor { CampoCosecha_Id = 2, Proveedor_Id = 2, Archivo = new Archivo { Ruta = "ruta5" } },
                new CampoProveedor { CampoCosecha_Id = 3, Proveedor_Id = 2, Archivo = new Archivo { Ruta = "ruta6" } },
            };

            this.repositorioMock
                .Setup(r => r.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
                .Returns<Expression<Func<CampoProveedor, bool>>>(q => campoProveedorList.SingleOrDefault(q.Compile()));

            string result = target.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest);

            Assert.AreEqual("ruta6", result);

            this.repositorioMock.Verify(r => r.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()), Times.Once);
        }
        [Test]
        public void DescargarArchivosGoogle_ArchivoYaProcesado_ReturnAntes()
        {
            target.DescargarArchivosDeGoogleDrive(new ArchivoCampoSustentable { ProcesadoUcropit = true });
            googleDriveMock.Verify(drive => drive.DownloadFile(It.IsAny<GoogleDriveFileDownloadRequest>()), Times.Never);
        }
        [Test]
        public void DescargarArchivosGoogle_ArchivoSinProcesar_DescargaArchivo()
        {
            this.repositorioMock
                .Setup(r => r.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
                .Returns(
                new CampoProveedor
                {
                    CUIT = "222222222",
                    Proveedor_Id = 1,
                    BSVS2 = true,
                    EPA = false,
                    EUDR = false,
                    CampoCosecha_Id = 10,
                    CampoCosecha = new CampoCosecha
                    {
                        Campo = new CampoSustentable { Id = 2 },
                        Cosecha = new Cosecha { Nombre = "adsf", Id =1 },
                        Cosecha_Id = 1,
                        CampoCosechaNormativas = new List<CampoCosechaNormativa>
                            {
                                new CampoCosechaNormativa
                                {
                                    TipoNormativa = new TipoNormativa { Id=1, Descripcion = "2BSVS" },
                                    TipoNormativa_Id = 1
                                }
                            }

                    }
                });

            repositorioMock
               .Setup(r => r.Listar<CampoProveedor>(
                   It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                   It.IsAny<int>(),
                   It.IsAny<string>(),
                   It.IsAny<DirOrden>(),
                   It.IsAny<IEnumerable<Expression<Func<CampoProveedor, object>>>>()
               ))
               .Returns(new List<CampoProveedor>
               {
                   new CampoProveedor
                    {
                        Proveedor_Id = 1,
                        CampoCosechaSuperposicion_Id = 10,
                        CampoCosecha = new CampoCosecha
                        {
                            Campo = new CampoSustentable { Id = 2 },
                            Cosecha = new Cosecha { Nombre = "adsf", Id =1 },
                            Cosecha_Id = 1,
                            CampoCosechaNormativas = new List<CampoCosechaNormativa>
                            {
                                new CampoCosechaNormativa
                                {
                                    TipoNormativa = new TipoNormativa { Id=1, Descripcion = "2BSVS" },
                                    TipoNormativa_Id = 1
                                }
                            }
                            
                        }
                    }
               });

            var archivoSinDescargar = new ArchivoCampoSustentable
            {
                ProcesadoUcropit = false,
                Proveedor = new Proveedor { CUIT = "cuit" },
                CampoCosecha = new CampoCosecha { Id= 10, Campo = new CampoSustentable { Id = 3 }, Cosecha = new Cosecha { Nombre = "ads", Id= 1 }, CampoSustentable_Id = 3,
                    Cosecha_Id = 1,
                    CampoCosechaNormativas = new List<CampoCosechaNormativa>
                            {
                                new CampoCosechaNormativa
                                {
                                    TipoNormativa = new TipoNormativa { Descripcion = "2BSVS" }
                                }
                            }
                }
            };
            googleDriveMock.Setup(drive => drive.DownloadFileAs<ReporteProcesoUcropit>(It.IsAny<GoogleDriveFileDownloadRequest>()))
                .ReturnsAsync(new ReporteProcesoUcropit());

            repositorioMock
            .Setup(r => r.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()))
            .Returns<Expression<Func<Cosecha, bool>>>(expr =>
            {
                var cosecha = new Cosecha { Id = 1, Nombre = "CosechaTest" };
                // Evalúa la expresión para ver si coincide con el Id
                return expr.Compile().Invoke(cosecha) ? cosecha : null;
            });

            target.DescargarArchivosDeGoogleDrive(archivoSinDescargar);

            googleDriveMock.Verify(drive => drive.DownloadFileAs<ReporteProcesoUcropit>(It.IsAny<GoogleDriveFileDownloadRequest>()), Times.Once);
            repositorioMock.Verify(repositorio => repositorio.GuardarCambios(), Times.Once);
        }

        [Test]
        public void Rechazar_CampoCosechaNormativaExiste_ActualizaMotivoRechazoYGuarda()
        {
            // Arrange
            var mailUsuario = "usuario@test.com";
            var campoCosechaId = 10;
            var proveedorId = 20;
            var tipoNormativaId = 30;
            var motivoRechazo = "Motivo rechazo de prueba";

            var usuario = new Usuario
            {
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { new Proveedor { Id = proveedorId } },
                Roles = new List<Rol>{
            new Rol
            {
                Codigo = "TestRol",
                PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
            }
        }
            };
            CampoSustentable campoSustentable = new CampoSustentable { IdScato = 1, Nombre = "Campo sustentable 1" };

            var campoCosecha = new CampoCosecha
            {
                Campo = campoSustentable,
                Cosecha = new Cosecha { Nombre = "cosecha 1" },
                Cosecha_Id = 1,
                CampoCosechaNormativas = new List<CampoCosechaNormativa>
                {
                    new CampoCosechaNormativa
                    {
                        Id = 1,
                        CampoCosecha_Id = 100,
                        TipoNormativa_Id = 30,
                        ToneladasAprobadas = 50.5,
                        MotivoRechazo = null,
                        Validado = false,
                    }
                }
            };
            var campoProveedor = new CampoProveedor { Proveedor_Id = proveedorId, CampoCosecha_Id = campoCosechaId, CampoCosecha = campoCosecha };
            var campoCosechaNormativa = campoProveedor.CampoCosecha.CampoCosechaNormativas.First();

            repositorioMock
                .Setup(x => x.Obtener<CampoProveedor>(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
                .Returns(campoProveedor);

            repositorioMock
                .Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);      

            // Act
            var result = target.Rechazar(mailUsuario, campoCosechaId, proveedorId, tipoNormativaId, motivoRechazo);

            // Assert
            Assert.AreEqual(motivoRechazo, campoCosechaNormativa.MotivoRechazo);
            Assert.AreEqual(SuccessMsg.CampoSustentableRechazado, result);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
        [Test]
        public void Aprobar_CampoCosechaNormativaExiste_ValidaYGuarda()
        {
            // Arrange
            var mailUsuario = "usuario@test.com";
            var campoCosechaId = 10;
            var proveedorId = 20;
            var tipoNormativaId = 2;

            var usuario = new Usuario { Id = 99, Mail = mailUsuario, Proveedores = new List<Proveedor> { new Proveedor { Id = proveedorId } },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };
            CampoSustentable campoSustentable = new CampoSustentable { IdScato = 1, Nombre = "Campo sustentable 1" };

            var campoCosecha = new CampoCosecha
            {
                Campo = campoSustentable,
                Cosecha = new Cosecha { Nombre = "cosecha 1" },
                Cosecha_Id = 1,
                CampoCosechaNormativas = new List<CampoCosechaNormativa>
                {
                    new CampoCosechaNormativa
                    {
                        Id = 1,
                        CampoCosecha_Id = 100,
                        TipoNormativa_Id = 2,
                        ToneladasAprobadas = 50.5,
                        MotivoRechazo = null,
                        Validado = false,
                    }
                }
            };

            var campoProveedor = new CampoProveedor { Proveedor_Id = proveedorId, CampoCosecha_Id = campoCosechaId, CampoCosecha = campoCosecha };

            repositorioMock
                .Setup(x => x.Obtener<CampoProveedor>(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
                .Returns(campoProveedor);

            repositorioMock
                .Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            //repositorioMock
              //  .Setup(x => x.Obtener<CampoCosechaNormativa>(It.IsAny<Expression<Func<CampoCosechaNormativa, bool>>>()))
               // .Returns(campoProveedor.CampoCosecha.CampoCosechaNormativas.First());

            // Act
            var result = target.Aprobar(mailUsuario, campoCosechaId, proveedorId, tipoNormativaId);

            var campoCosechaNormativa = campoProveedor.CampoCosecha.CampoCosechaNormativas.First();
            // Assert
            Assert.IsTrue(campoCosechaNormativa.Validado);
            Assert.AreEqual(usuario.Id, campoCosechaNormativa.ValidadoPor);
            Assert.IsNotNull(campoCosechaNormativa.ValidadoFecha);
            Assert.AreEqual(SuccessMsg.CampoSustentableAprobado, result);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AdjuntarEPAValidado_ActualizaEvidenciaEPA_YGuarda()
        {
            var mailUsuario = "usuario@test.com";
            var campoCosechaId = 10;
            var proveedorId = 20;
            var tipoNormativaId = 30;
            
            var usuario = new Usuario
            {
                Id = 99,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { new Proveedor { Id = proveedorId } },
                Roles = new List<Rol>{
                    new Rol
                    {
                        Codigo = "TestRol",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Id = 1, Permiso = "VER TODOS CAMPOS SUSTENTABLE" } }
                    }
                }
            };
            var campoProveedor = new CampoProveedor { Proveedor_Id = proveedorId, CampoCosecha_Id = campoCosechaId };
            var campoCosechaNormativa = new CampoCosechaNormativa { TipoNormativa_Id = tipoNormativaId };


            var archivoEPAStream = new MemoryStream(new byte[] { 1, 2, 3 });
            var archivoEPAMock = new Mock<HttpPostedFileBase>();
            archivoEPAMock.Setup(f => f.FileName).Returns("EPA.pdf");
            archivoEPAMock.Setup(f => f.InputStream).Returns(archivoEPAStream);

            repositorioMock
                .Setup(x => x.Obtener<CampoProveedor>(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
                .Returns(campoProveedor);

            repositorioMock
                .Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            var result = target.AdjuntarEPAValidado(mailUsuario, campoCosechaId, proveedorId, archivoEPAMock.Object);

            Assert.IsNotNull(campoProveedor.EvidenciaEPA);
            Assert.AreEqual(FileKeys.ArchivoEPA, campoProveedor.EvidenciaEPA.FileKey);
            Assert.IsTrue(campoProveedor.EvidenciaEPA.Ruta.EndsWith("EPA.pdf"));
            Assert.AreEqual(SuccessMsg.CampoSustentableActualizado, result);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

    }
}