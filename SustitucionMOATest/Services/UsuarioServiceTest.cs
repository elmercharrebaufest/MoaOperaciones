using Moq;
using NUnit.Framework;
using SustitucionMOA.Jobs;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    public class UsuarioServiceTest
    {

        private UsuarioService target;
        private Mock<IRepositorioUsuario> repositorioUsuarioMock;
        private Mock<IVendedorService> vendedorServiceMock;
        private Mock<IAzureADConsumer> azureADConsumerMock; 


        [SetUp]
        public void SetUp()
        {
            repositorioUsuarioMock = new Mock<IRepositorioUsuario>();
            vendedorServiceMock = new Mock<IVendedorService>();
            azureADConsumerMock = new Mock<IAzureADConsumer>();
            target = new UsuarioService(repositorioUsuarioMock.Object, vendedorServiceMock.Object, azureADConsumerMock.Object);
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

            repositorioUsuarioMock
                .Setup(y => y.Obtener<TipoUsuario>(It.IsAny<int>()))
                .Returns(new TipoUsuario
                {
                    Id = 2,
                    Nombre = "Granos",
                    NombreCorto = "GRAN"
                });

            repositorioUsuarioMock
                    .Setup(y => y.Obtener<Proveedor>(It.IsAny<int>()))
                    .Returns(new Proveedor
                    {
                        Id = 1,
                        EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                        RazonSocial = "RS",
                        Mail = mailUsuario
                    });


            repositorioUsuarioMock
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


            repositorioUsuarioMock
                    .Setup(y => y.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                    .Returns(new Rol { Id = 1, Nombre = "Granos", Codigo = "GRAN" });

            var expected = string.Format(SuccessMsg.UsuarioHabilitadoOK, mailUsuario);

            var result = target.HabilitarUsuario(mailUsuario);

            repositorioUsuarioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            var resultUser = repositorioUsuarioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            repositorioUsuarioMock.Verify(x => x.Agregar(It.IsAny<Rol>()), Times.Never);
            repositorioUsuarioMock.Verify(x => x.GuardarCambios(), Times.Once);

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

            repositorioUsuarioMock
                .Setup(y => y.Obtener<TipoUsuario>(It.IsAny<int>()))
                .Returns(new TipoUsuario
                {
                    Id = 2,
                    Nombre = "Granos",
                    NombreCorto = "GRAN"
                });

            repositorioUsuarioMock
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

            repositorioUsuarioMock
                    .Setup(y => y.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                    .Returns(new Rol { Id = 1, Nombre = "Granos", Codigo = "GRAN" });

            var expected = string.Format(SuccessMsg.UsuarioDeshabilitadoOK, mailUsuario);

            var result = target.DeshabilitarUsuario(mailUsuario);

            repositorioUsuarioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioUsuarioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()), Times.Once);

            var resultUser = repositorioUsuarioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            repositorioUsuarioMock.Verify(x => x.Agregar(It.IsAny<Rol>()), Times.Never);
            repositorioUsuarioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoAprobacion.Deshabilitado, resultUser.ObtenerProveedor().EstadoAprobacion);
            Assert.AreEqual(false, resultUser.Habilitado);
        }


        [Test]
        public void CambiarRolesUsuarioComunTest()
        {
            var mailUsuario = "existente@mail.com";
            string usuarioSap = "FOSSIM";

            repositorioUsuarioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(new Usuario
              {
                  Id = 1,
                  Mail = mailUsuario,
                  CUITRegistro = "23333333333",
                  Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG", EsEditable = true } },
                  TipoUsuario = new TipoUsuario { Id = 2, Nombre = "Granos", NombreCorto = "GRAN" },
                  Proveedores = new List<Proveedor>()
              });


            var rolesList = new List<Rol>
            {
                new Rol { Id = 1, Codigo = "Uno" },
                new Rol { Id = 2, Codigo = "Dos" }
            };

            repositorioUsuarioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                .Returns<Expression<Func<Rol, bool>>>(expr => rolesList.Where(expr.Compile()).FirstOrDefault());

            var expected = string.Format(SuccessMsg.RolesActualizadosOk, mailUsuario, "");

            List<int> idRoles = new List<int> { 1, 2 };

            int IdUsuario = 1;

            var result = target.GuardarRoles(idRoles, IdUsuario, usuarioSap,"");

            repositorioUsuarioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioUsuarioMock.Verify(x => x.GuardarCambios(), Times.Once);

            var resultUser = repositorioUsuarioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(2, resultUser.Roles.Count);
        }

        [Test]
        public void CambiarRolesUsuarioAdminTest()
        {
            var mailUsuario = "existente@mail.com";
            string usuarioSap = "FOSSIM";

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "23333333333",
                Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG", EsEditable = true } },
                TipoUsuario = new TipoUsuario { Id = 2, Nombre = "Granos", NombreCorto = "GRAN" },
                UsuarioSap = "",
                Proveedores = new List<Proveedor>
                      {
                        new Proveedor
                        {
                            Id = 1, 
                            EstadoAprobacion = EstadoAprobacion.DeshabilitadoEnDataAgro,
                            CUIT = "23333333333",
                            Mail = mailUsuario,
                            HistorialAprobaciones = new List<ProveedorHistorialAprobacion>
                            {
                                new ProveedorHistorialAprobacion
                                {
                                    Id = 1,
                                    Proveedor_Id = 1, Observacion = "TEST"
                                }
                            }
                        }
                      }
            };

            repositorioUsuarioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            var rolesList = new List<Rol>
            {
                new Rol { Id = 1, Codigo = "ADM" },
                new Rol { Id = 2, Codigo = "Otro" }
            };

            repositorioUsuarioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Rol, bool>>>()))
                .Returns<Expression<Func<Rol, bool>>>(expr => rolesList.Where(expr.Compile()).FirstOrDefault());

            var expected = string.Format(SuccessMsg.RolesActualizadosOk, mailUsuario, " ( CUIT: 23333333333)");

            List<int> idRoles = new List<int> { 1, 2 };

            int IdUsuario = 1;

            var result = target.GuardarRoles(idRoles, IdUsuario, usuarioSap,"");

            repositorioUsuarioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            repositorioUsuarioMock.Verify(x => x.GuardarCambios(), Times.Once);

            var resultUser = repositorioUsuarioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Assert.IsEmpty(usuario.Proveedores.First().HistorialAprobaciones);
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

        [Test]
        public void SeccionVisitadaTest()
        {
            var mailUsuario = "existente@mail.com";

            repositorioUsuarioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(new Usuario
              {
                  Id = 1,
                  Mail = mailUsuario,
                  Roles = new List<Rol> { new Rol { Nombre = "DESHABILITADO EN DATAAGRO", Codigo = "DDAG", EsEditable = true } },
                  TipoUsuario = new TipoUsuario { Id = 2, Nombre = "Granos", NombreCorto = "GRAN" },
                  SeccionesVisitadas = "descargas-prueba",
              });

            var expected = "descargas-prueba-noEstaba";

            target.SeccionVisitada(mailUsuario, "noEstaba");

            repositorioUsuarioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioUsuarioMock.Verify(x => x.GuardarCambios(), Times.Once);

            var resultUser = repositorioUsuarioMock.Object.Obtener<Usuario>(u => u.Mail == mailUsuario);

            Assert.AreEqual(expected, resultUser.SeccionesVisitadas);
        }
    }
}
