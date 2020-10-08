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
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    public class UsuarioServiceTest
    {

        private UsuarioService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IVendedorService> vendedorServiceMock;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            vendedorServiceMock = new Mock<IVendedorService>();
            target = new UsuarioService(repositorioMock.Object, vendedorServiceMock.Object);
        }

        [Test]
        public void HabilitarUsuarioGranosTest()
        {

            var mailUsuario = "existente@mail.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } }
            };

            repositorioMock
                .Setup(y => y.Obtener<TipoUsuario>(It.IsAny<int>()))
                .Returns(new TipoUsuario
                {
                    Id = 2,
                    Nombre = "Granos",
                    NombreCorto = "GRAN"
                });

            repositorioMock
                    .Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                    .Returns(new Proveedor
                    {
                        Id = 1,
                        EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                        RazonSocial = "RS",
                        Mail = mailUsuario
                    });


            repositorioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(new Usuario
              {
                  Id = 1,
                  Mail = mailUsuario,
                  Habilitado = false,
                  Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG" } },
                  TipoUsuario = new TipoUsuario { Id = 2, Nombre = "Granos", NombreCorto = "GRAN" },
                  Proveedores = new List<Proveedor> { proveedor }
              });


            repositorioMock
                    .Setup(y => y.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                    .Returns(new Rol { Id = 1, Nombre = "Granos", Codigo = "GRAN" });

            var expected = string.Format(SuccessMsg.UsuarioHabilitadoOK, mailUsuario);

            var result = target.HabilitarUsuario(mailUsuario);


            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()), Times.Once);


            var resultUser = repositorioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Rol>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoAprobacion.Aprobado, resultUser.ObtenerProveedor().EstadoAprobacion);
            Assert.AreEqual(true, resultUser.Habilitado);

        }

        [Test]
        public void DeshabilitarUsuarioGranosTest()
        {

            var mailUsuario = "existente@mail.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } }
            };

            repositorioMock
                .Setup(y => y.Obtener<TipoUsuario>(It.IsAny<int>()))
                .Returns(new TipoUsuario
                {
                    Id = 2,
                    Nombre = "Granos",
                    NombreCorto = "GRAN"
                });

            repositorioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(new Usuario
              {
                  Id = 1,
                  Mail = mailUsuario,
                  Habilitado = true,
                  Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG" } },
                  TipoUsuario = new TipoUsuario { Id = 2, Nombre = "Granos", NombreCorto = "GRAN" },
                  Proveedores = new List<Proveedor> { proveedor }
              });


            repositorioMock
                    .Setup(y => y.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                    .Returns(new Rol { Id = 1, Nombre = "Granos", Codigo = "GRAN" });

            var expected = string.Format(SuccessMsg.UsuarioDeshabilitadoOK, mailUsuario);

            var result = target.DeshabilitarUsuario(mailUsuario);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()), Times.Once);

            var resultUser = repositorioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Rol>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoAprobacion.Deshabilitado, resultUser.ObtenerProveedor().EstadoAprobacion);
            Assert.AreEqual(false, resultUser.Habilitado);
        }


        [Test]
        public void CambiarRolesUsuarioTest()
        {
            var mailUsuario = "existente@mail.com";

            repositorioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(new Usuario
              {
                  Id = 1,
                  Mail = mailUsuario,
                  Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG" } },
                  TipoUsuario = new TipoUsuario { Id = 2, Nombre = "Granos", NombreCorto = "GRAN" },
              });

            var expected = string.Format(SuccessMsg.RolesActualizadosOk, mailUsuario);

            List<int> idRoles = new List<int> { 1, 2};

            int IdUsuario = 1;

            var result = target.GuardarRoles(idRoles, IdUsuario);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            var resultUser = repositorioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(2, resultUser.Roles.Count);
        }

        [Test]
        public void GetVendedoresUsuarioTest()
        {
            var mailUsuario = "existente@mail.com";
            var respuesta = new List<ProveedorDto>();

            vendedorServiceMock.Setup(x => x.GetVendedores(It.IsAny<string>())).Returns(respuesta);

            var result = target.GetVendedoresUsuario(mailUsuario);

            vendedorServiceMock.Verify(x => x.GetVendedores(It.IsAny<string>()), Times.Once);

            Assert.AreEqual(result, respuesta);
        }
    }
}
