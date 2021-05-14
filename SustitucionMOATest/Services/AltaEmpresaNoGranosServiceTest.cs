using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
   public class AltaEmpresaNoGranosServiceTest
   {
       private AltaEmpresaNoGranosService target;
       private Mock<IRepositorio> repositorioMock;
       private Mock<IDataAgroService> dataAgroServiceMock;
        private Mock<IAltaEmpresaNoGranosService> consultaServiceMock;

       [SetUp]
       public void SetUp()
       {
           repositorioMock = new Mock<IRepositorio>();
           dataAgroServiceMock = new Mock<IDataAgroService>();
           target = new AltaEmpresaNoGranosService(repositorioMock.Object, dataAgroServiceMock.Object);
       }

       [Test]
       public void EditarAltaEmpresaNoGranosTest()
       {

            var mailUsuario = "existente@mail.com";

            var proveedorMock = new Proveedor
            {
                Id = 1,
                RazonSocial = "Test",
                CUIT = "1111111111",
                Mail = mailUsuario,
                CodigoProveedor = "11111111"
            };

            var usuarioMock = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG", EsEditable = true } },
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "No Granos", NombreCorto = "NGRAN" },
                SeccionesVisitadas = "descargas-prueba",
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                 .Returns(proveedorMock);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                 .Returns(usuarioMock);

            var expected = "No se puede modificar el Mail porque ya existe un Usuario vinculado.";

            var result = target.EditarAltaEmpresaNoGranos(1, "X", "3333333", mailUsuario, "2494576721", true, 1, "algo", "SWDEV", "??", "TEST", 1150, true, false, false, false);

            Assert.AreEqual(expected, result);
        }
        /*
        [Test]
        public void EditarAltaEmpresaNoGranosTestIdInexistente()
        {

            var mailUsuario = "existente@mail.com";

            var proveedorMock = new Proveedor
            {
                Id = 1,
                RazonSocial = "Test",
                CUIT = "1111111111",
                Mail = mailUsuario,
                CodigoProveedor = "11111111"
            };

            var usuarioMock = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG", EsEditable = true } },
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "No Granos", NombreCorto = "NGRAN" },
                SeccionesVisitadas = "descargas-prueba",
            };

            consultaServiceMock.Setup(x => x.EditarAltaEmpresaNoGranos(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Throws(new InfoCustomException("Id invalido."));

            var expectedJson = JsonConvert.SerializeObject(new { info = "Id invalido." });
            var result = target.EditarAltaEmpresaNoGranos(0, "X", "3333333", mailUsuario, "2494576721", true, 1, "algo", "SWDEV", "??", "TEST", 1150, true, false, false, false);

            Assert.AreEqual(expectedJson, result);
        }*/

        [Test]
        public void EditarAltaEmpresaNoGranosSinUsuarioTest()
        {
            var mailUsuario = "existente@mail.com";

            var proveedorMock = new Proveedor
            {
                Id = 1,
                RazonSocial = "Test",
                CUIT = "1111111111",
                Mail = mailUsuario,
                CodigoProveedor = "11111111"
            };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                 .Returns(proveedorMock);

            var expected = "Editado correctamente";
            var result = target.EditarAltaEmpresaNoGranos(1, "X", "3333333", mailUsuario, "2494576721", true, 1, "algo", "SWDEV", "??", "TEST", 1150, true, false, false, false);

            Assert.AreEqual(expected, result);
        }
    }
}

