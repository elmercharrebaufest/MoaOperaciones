using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


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
        public void AgregarTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void EditarTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void BorrarTest()
        {
            throw new NotImplementedException();
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
        public void ListarTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerCampoTest()
        {
            throw new NotImplementedException();
        }
    }
}
