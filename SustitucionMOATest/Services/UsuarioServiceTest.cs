using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
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


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new UsuarioService(repositorioMock.Object);
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
        }
    }
}
