using Moq;
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
    public class VendedorServiceTest
    {
        private VendedorService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IDataAgroService> dataAgroServiceMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            dataAgroServiceMock = new Mock<IDataAgroService>();
            target = new VendedorService(repositorioMock.Object, dataAgroServiceMock.Object);
        }

        [Test]
        public void GetVendedoresConUsuarioExistenteTest()
        {
            var mailUsuario = "existente@mail.com";
            var proveedores = new List<Proveedor>() { 
                new Proveedor { 
                    CUIT = "23-102394598-7", 
                    CodigoProveedor = "C12331234", 
                    Mail = mailUsuario, 
                    RazonSocial = "Test SA",
                    UsuariosAsociados = new List<Usuario>()
                } 
            };
            var usuario = new Usuario()
            {
                Mail = mailUsuario,
                CUITRegistro = "23-123464943-9",
                Habilitado = true,
                Proveedores = proveedores
            };

            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            var result = target.GetVendedores(mailUsuario);

            repositorioMock.Verify(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(result.First().CUIT, proveedores.First().CUIT);
            Assert.AreEqual(result.First().Mail, proveedores.First().Mail);
            Assert.AreEqual(result.First().CodigoProveedor, proveedores.First().CodigoProveedor);
            Assert.AreEqual(result.First().RazonSocial, proveedores.First().RazonSocial);
        }

        [Test]
        public void GetVendedoresPendientesConUsuarioExistenteTest()
        {
            var mailUsuario = "existente@mail.com";
            var proveedores = new List<Proveedor>() {
                new Proveedor {
                    CUIT = "23-102394598-7",
                    CodigoProveedor = "C12331234",
                    Mail = mailUsuario,
                    RazonSocial = "Test SA",
                    EstadoAprobacion = EstadoAprobacion.AprobacionPendiente,
                    UsuariosAsociados = new List<Usuario>()
                }
            };
            var usuario = new Usuario()
            {
                Mail = mailUsuario,
                CUITRegistro = "23-123464943-9",
                Habilitado = true,
                Proveedores = proveedores
            };

            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            var result = target.GetVendedoresPendientes(mailUsuario);

            repositorioMock.Verify(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(result.First().CUIT, proveedores.First().CUIT);
            Assert.AreEqual(result.First().Mail, proveedores.First().Mail);
            Assert.AreEqual(result.First().CodigoProveedor, proveedores.First().CodigoProveedor);
            Assert.AreEqual(result.First().RazonSocial, proveedores.First().RazonSocial);
        }

        [Test]
        public void GetVendedoresPendientesConUsuarioExistenteYVendedorAprobadoTest()
        {
            var mailUsuario = "existente@mail.com";
            var proveedores = new List<Proveedor>() {
                new Proveedor {
                    CUIT = "23-102394598-7",
                    CodigoProveedor = "C12331234",
                    Mail = mailUsuario,
                    RazonSocial = "Test SA",
                    EstadoAprobacion = EstadoAprobacion.Aprobado,
                    UsuariosAsociados = new List<Usuario>()
                }
            };
            var usuario = new Usuario()
            {
                Mail = mailUsuario,
                CUITRegistro = "23-123464943-9",
                Habilitado = true,
                Proveedores = proveedores
            };
            var respuesta = string.Format(InfoMsg.SinRegistros, "Empresas");

            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            var ex = Assert.Throws<InfoCustomException>(() => target.GetVendedoresPendientes(mailUsuario));
            Assert.AreEqual(ex.Message, respuesta);
            repositorioMock.Verify(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
        }
    }
}
