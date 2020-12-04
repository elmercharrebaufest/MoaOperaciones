using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
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
    [TestFixture()]
    public class AltaEmpresaServiceTest
    {
        private AltaEmpresaService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IDataAgroService>  dataAgroServiceMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            dataAgroServiceMock = new Mock<IDataAgroService>();
            target = new AltaEmpresaService(repositorioMock.Object, dataAgroServiceMock.Object);
        }

        [Test()]
        public void SetEstadoAprobacionAprobadoTest()
        {
            var mailUsuario = "existente@mail.com";


            int proveedorId = 1;

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario
            };

            var usuario = new Usuario
            {
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol> { new Rol { Nombre = "Nuevo granos", Codigo = "NUEG" } }
            };

            repositorioMock
                    .Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                    .Returns(new Proveedor
                    {
                        Id = proveedorId,
                        EstadoAprobacion = EstadoAprobacion.AprobacionPendiente,
                        RazonSocial = "RS",
                        Mail = mailUsuario
                    });

            repositorioMock
                    .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(new Usuario
                    {
                        Id = 1,
                        Mail = mailUsuario,
                        TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Granos" },
                        Roles = new List<Rol> { new Rol { Nombre = "Nuevo granos", Codigo = "NUEG" } }
                    });


            repositorioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
              .Returns(new Rol
              {
                  Id = 1,
                  Nombre = "Granos",
                  Codigo = "GRAN"
              });

            dataAgroServiceMock
                .Setup(y => y.ObtenerValidarCUITProveedorGranos(It.IsAny<string>()))
                .Returns(new SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial());


            var expected = string.Format(SuccessMsg.EmpresaCambioEstadoOK, proveedor.RazonSocial);

            var observacion = "";
            var observacionParaElProveedor = "";
            var estadoSIPER = "";

            var result = target.SetEstadoAprobacion(proveedorId, EstadoAprobacion.Aprobado, observacion, mailUsuario, observacionParaElProveedor, estadoSIPER, false);


            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()), Times.Once);


            repositorioMock.Verify(x => x.Agregar(It.IsAny<Rol>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);

        }

        [Test()]
        public void ObtenerRolPorCodigoTest()
        {
            var expected = new Rol { Id = 1, Nombre = "Todos", Codigo = "Todos" };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                .Returns(new Rol { Id = 1, Nombre = "Todos", Codigo = "Todos" });

            var result = target.ObtenerRolPorCodigo("Todos");

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()), Times.Once);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Rol>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void GetEstadoAprobacionTest()
        {
            var expected = new EstadoAprobacionDto
            {
                Estado = EstadoAprobacion.Aprobado,
                EstadoDescripcion = EstadoAprobacion.Aprobado.ToFriendlyString(),
                Observaciones = "Test"
            };

            var proveedor = new Proveedor { EstadoAprobacion = EstadoAprobacion.Aprobado, Observaciones = "Test" };
            var mailUsuario = "existente@mail.com";

            var usuario = new Usuario { Mail = mailUsuario, Proveedores = new List<Proveedor> { proveedor } };

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(usuario);

            var result = target.GetEstadoAprobacion(mailUsuario);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Usuario>()), Times.Never);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.AreEqual(expected, result);
        }
    }
}
