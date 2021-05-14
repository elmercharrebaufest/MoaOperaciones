using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class CampoSustentableServiceTest
    {
        private CampoSustentableService target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new CampoSustentableService(repositorioMock.Object);
        }


        [Test()]
        public void AgregarConCampoNuevoTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333",
                RazonSocial = "Test",

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
                Id = 1,
                Nombre = "20-21",
                Inicio = DateTime.Now
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Obtener<Cosecha>(It.IsAny<int>()))
               .Returns(cosecha);


            var declaracion = new DeclaracionCampoSustentable
            {
                CUIT = "233333333333",
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
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<DeclaracionCampoSustentable, bool>>>()))
               .Returns(declaracion);

            var campoCreado = new CampoProveedor
            {
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                Proveedor = proveedor,
                CUIT="23333333333",
                RazonSocial = "Test",
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" }
                }
            };

            FileStream fileStream = null;
            Mock<HttpPostedFileBase> uploadedFile = new Mock<HttpPostedFileBase>();

            uploadedFile
                .Setup(f => f.ContentLength)
                .Returns(100);

            uploadedFile
                .Setup(f => f.FileName)
                .Returns("Example.kmz");

            uploadedFile
                .Setup(f => f.InputStream)
                .Returns(fileStream);

            ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"] = "C:/ArchivosCampoSustentableTest";

            var result = target.Agregar(mailUsuario, campoCreado, uploadedFile.Object);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
               .Verify(x => x.GuardarCambios(), Times.Exactly(2));

            repositorioMock
                 .Verify(x => x.Agregar(It.IsAny<CampoProveedor>()), Times.Once);

            var expected = new Resultado { IdEntidad = 0, Mensaje = SuccessMsg.CampoSustentableAgregado };

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void EditarTest()
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
                Proveedor = proveedor,
                Archivo = new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" },
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" }
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
               .Returns(campoProveedor);

            var campoEditado = new CampoProveedor
            {
                CampoCosecha_Id = campoCosechaId,
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" }
                }
            };

            FileStream fileStream = null;
            Mock<HttpPostedFileBase> uploadedFile = new Mock<HttpPostedFileBase>();

            uploadedFile
                .Setup(f => f.ContentLength)
                .Returns(100);

            uploadedFile
                .Setup(f => f.FileName)
                .Returns("Example.kmz");

            uploadedFile
                .Setup(f => f.InputStream)
                .Returns(fileStream);

            ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"] = "C:/ArchivosCampoSustentableTest";

            var result = target.Editar(mailUsuario, campoEditado, uploadedFile.Object);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()), Times.Once);

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
                    Cosecha = new Cosecha { Nombre = "20-21" }
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
                    ToneladasAprobadas = 50
                }
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

            var expected = new EstadoDeclaracionSustentableDto { 
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

            Assert.AreEqual(expected, result);
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


            var result = target.ObtenerCosechas();


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
    }
}