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

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var campoCreado = new CampoProveedor
            {
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                Proveedor = proveedor,
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
        public void AgregarConCampoExistenteTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var campoCosechaId = 3;
            double toneladasAprobadas = 50;
            var campoSustentableId = 4;

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

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var campoCreado = new CampoProveedor
            {
                Proveedor_Id = proveedorId,
                HectareasSoja = 100,
                HectareasTotales = 100,
                Borrado = false,
                Proveedor = proveedor,
                CampoCosecha = new CampoCosecha
                {
                    Campo = new CampoSustentable { Nombre = "Test" },
                    Cosecha = new Cosecha { Nombre = "20-21" }
                }
            };


            var campoSustentable = new CampoSustentable
            {
                Id = campoSustentableId,
                Nombre = "Test",
                Localidad_Id = 5,
            };

            var campoCosechaExistente = new CampoCosecha
            {
                Id = campoCosechaId,
                Campo = campoSustentable,
                Cosecha = new Cosecha { Nombre = "20-21" },
                ToneladasAprobadas = toneladasAprobadas
            };

            repositorioMock
             .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoCosecha, bool>>>()))
             .Returns(campoCosechaExistente);

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoSustentable, bool>>>()))
               .Returns(campoSustentable);

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
            Assert.AreEqual(campoSustentableId, campoCreado.CampoCosecha.CampoSustentable_Id);
            Assert.AreEqual(toneladasAprobadas, campoCreado.CampoCosecha.ToneladasAprobadas);
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
            var cosecha = new Cosecha { Id = 1, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedorId = 1;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                FechaFirmaDeclaracionCampoSustentable = DateTime.Now,
            };

            repositorioMock
                 .Setup(x => x.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()))
                 .Returns(cosecha);


            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);


            var result = target.VerificarDeclaracion(proveedorId);

            var expected = true;

            repositorioMock
                 .Verify(x => x.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void VerificarDeclaracionVencidaTest()
        {
            var cosecha = new Cosecha { Id = 1, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedorId = 1;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                FechaFirmaDeclaracionCampoSustentable = DateTime.Now.AddDays(-5),
            };

            repositorioMock
                 .Setup(x => x.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()))
                 .Returns(cosecha);


            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);


            var result = target.VerificarDeclaracion(proveedorId);

            var expected = false;

            repositorioMock
                 .Verify(x => x.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void VerificarDeclaracionNulaTest()
        {
            var cosecha = new Cosecha { Id = 1, Nombre = "19-20", Inicio = DateTime.Now.AddDays(-1), Fin = DateTime.Now.AddDays(1) };

            var proveedorId = 1;

            var proveedor = new Proveedor
            {
                Id = proveedorId
            };

            repositorioMock
                 .Setup(x => x.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()))
                 .Returns(cosecha);


            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);


            var result = target.VerificarDeclaracion(proveedorId);

            var expected = false;

            repositorioMock
                 .Verify(x => x.Obtener(It.IsAny<Expression<Func<Cosecha, bool>>>()), Times.Once);

            repositorioMock
                 .Verify(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void FirmarDeclaracionTotalTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var hectareas = 0.00;

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

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var result = target.FirmarDeclaracion(mailUsuario, proveedorId, hectareas);

            var expected = SuccessMsg.DeclaracionCampoSustentableFirmada;

            repositorioMock
                .Verify((x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId))), Times.Exactly(2));

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            Assert.AreEqual(expected, result);

            Assert.AreEqual(OpcionesDeclaracionCampoSustentable.Totalidad, proveedor.OpcionDeclaracionCampoSustentable);
        }

        [Test()]
        public void FirmarDeclaracionParcialTest()
        {
            var proveedorId = 1;
            var usuarioId = 2;
            var mailUsuario = "mail@mail.com";
            var hectareas = 100.00;


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

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId)))
                 .Returns(proveedor);

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            var result = target.FirmarDeclaracion(mailUsuario, proveedorId, hectareas);

            var expected = SuccessMsg.DeclaracionCampoSustentableFirmada;

            repositorioMock
                .Verify((x => x.Obtener<Proveedor>(It.Is<int>(i => i == proveedorId))), Times.Exactly(2));

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            Assert.AreEqual(expected, result);

            Assert.AreEqual(OpcionesDeclaracionCampoSustentable.Parcial, proveedor.OpcionDeclaracionCampoSustentable);


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
                                It.IsAny<DirOrden>()))
                 .Returns(cosechas);


            var result = target.ObtenerCosechas();


            repositorioMock.Verify(x => x.Listar(
                                It.IsAny<Expression<Func<Cosecha, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>()), Times.Once);

            Assert.NotNull(result);
            CollectionAssert.AreEquivalent(cosechas, result);

        }

        [Test()]
        public void ListarCamposUsuarioAdminTest()
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

            var campoProveedor = new List<CampoProveedor> {
                new CampoProveedor
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
                },
                new CampoProveedor
                {
                    CampoCosecha_Id = campoCosechaId + 1,
                    Proveedor_Id = proveedorId,
                    HectareasSoja = 50,
                    HectareasTotales = 50,
                    CampoCosecha = new CampoCosecha
                    {
                        Campo = new CampoSustentable { Nombre = "Test2" },
                        Cosecha = new Cosecha { Nombre = "21-22" },
                        ToneladasAprobadas = 20
                    }
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                                    It.IsAny<int>(),
                                    It.IsAny<string>(),
                                    It.IsAny<DirOrden>()))
               .Returns(campoProveedor);

            var expected = new List<CampoProveedorListadoDto> {
                new CampoProveedorListadoDto {
                    NombreCampo = "Test",
                    HectareasSoja = 100,
                    HectareasTotales = 100,
                    NombreCosecha = "20-21",
                    ToneladasAprobadas = 0
                },
                new CampoProveedorListadoDto {
                    NombreCampo = "Test2",
                    HectareasSoja = 50,
                    HectareasTotales = 50,
                    NombreCosecha = "21-22",
                    ToneladasAprobadas = 20
                }
            };

            var result = target.Listar(mailUsuario);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Listar(It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                                        It.IsAny<int>(),
                                        It.IsAny<string>(),
                                        It.IsAny<DirOrden>()), Times.Once);


            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test()]
        public void ListarCamposUsuarioNormalTest()
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
                        PermisosAsociados = new List<PermisoPorRol> {}
                    }
                }
            };

            var campoProveedor = new List<CampoProveedor> {
                new CampoProveedor
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
                },
                new CampoProveedor
                {
                    CampoCosecha_Id = campoCosechaId + 1,
                    Proveedor_Id = proveedorId,
                    HectareasSoja = 50,
                    HectareasTotales = 50,
                    CampoCosecha = new CampoCosecha
                    {
                        Campo = new CampoSustentable { Nombre = "Test2" },
                        Cosecha = new Cosecha { Nombre = "21-22" },
                        ToneladasAprobadas = 20
                    }
                }
            };

            repositorioMock
               .Setup(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))))
               .Returns(usuario);

            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                                    It.IsAny<int>(),
                                    It.IsAny<string>(),
                                    It.IsAny<DirOrden>()))
               .Returns(campoProveedor);

            var expected = new List<CampoProveedorListadoDto> {
                new CampoProveedorListadoDto {
                    NombreCampo = "Test",
                    HectareasSoja = 100,
                    HectareasTotales = 100,
                    NombreCosecha = "20-21",
                    ToneladasAprobadas = 0
                },
                new CampoProveedorListadoDto {
                    NombreCampo = "Test2",
                    HectareasSoja = 50,
                    HectareasTotales = 50,
                    NombreCosecha = "21-22",
                    ToneladasAprobadas = 20
                }
            };

            var result = target.Listar(mailUsuario);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Listar(It.IsAny<Expression<Func<CampoProveedor, bool>>>(),
                                        It.IsAny<int>(),
                                        It.IsAny<string>(),
                                        It.IsAny<DirOrden>()), Times.Once);


            CollectionAssert.AreEquivalent(expected, result);
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

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()))
               .Returns(campoProveedor);

            var expected = new CampoProveedorDto { NombreCampo = "Test", HectareasSoja = 100, HectareasTotales = 100, NombreCosecha = "20-21", ToneladasAprobadas = 0 };

            var result = target.ObtenerCampo(mailUsuario, proveedorId, campoCosechaId);

            repositorioMock
                 .Verify(x => x.Obtener(It.Is<Expression<Func<Usuario, bool>>>(l => l.Compile().Invoke(usuario))), Times.Once);

            repositorioMock
                .Verify(x => x.Obtener(It.IsAny<Expression<Func<CampoProveedor, bool>>>()), Times.Once);


            Assert.AreEqual(expected, result);
        }
    }
}
