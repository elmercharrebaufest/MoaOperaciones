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
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class OrdenDeCargaServiceTest
    {

        private OrdenDeCargaService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IOrdenCargaConsumerMOA> consumerOrdenCargaMOA;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            consumerOrdenCargaMOA = new Mock<IOrdenCargaConsumerMOA>();
            target = new OrdenDeCargaService(repositorioMock.Object, consumerOrdenCargaMOA.Object);
        }


        [Test()]
        public void AgregarTest()
        {
            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                }
            };

            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                 .Returns(proveedor);

            var ordenDeCarga = new OrdenDeCarga
            {
                Id = 1,
                CUITCliente = "233333333333",

            };

            ConfigurationManager.AppSettings["CantidadOrdenDeCarga"] = "30000";

            var result = target.Agregar(ordenDeCarga, mailUsuario);

            var expected = new Resultado { IdEntidad = 1, Mensaje = SuccessMsg.OrdenDeCargaAgregada };

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<OrdenDeCarga>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void ListarUsuarioComercialTest()
        {
            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                },
                Roles = new List<Rol> { new Rol { Codigo = "COMERCIAL " } }
            };

            var ordenesDeCarga = new List<OrdenDeCarga>()
            {
                new OrdenDeCarga {Id = 1, Cliente_Id = 1, CUITCliente = "233333333333"},
                new OrdenDeCarga {Id = 2, Cliente_Id = 2, CUITCliente = "255555555555"},
            };

            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
               .Returns(ordenesDeCarga);

            var result = target.Listar(mailUsuario);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()), Times.Once);
            Assert.IsTrue(result.Count == 2);
        }


        [Test()]
        public void ListarUsuarioComunTest()
        {
            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                },
                Roles = new List<Rol>()
            };

            var ordenesDeCarga = new List<OrdenDeCarga>()
            {
                new OrdenDeCarga {Id = 1, Cliente_Id = 1, CUITCliente = "233333333333"},
            };

            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);
            
            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
               .Returns(ordenesDeCarga);

            var result = target.Listar(mailUsuario);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()), Times.Once);
            Assert.IsTrue(result.Count == 1);
        }

        [Test()]
        public void AnularOrdenTest()
        {
            int orderId = 1;
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Pendiente,
                InformadaSAP = false
            };

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);

            var result = target.AnularOrden(orderId);

            var expected = SuccessMsg.OrdenDeCargaAnulada;

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoOrdenDeCarga.Pendiente, orden.Estado);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test()]
        public void AnularOrdenInformadaTest()
        {
            int orderId = 1;
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true
            };

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);

            var expected = "La orden no puede anularse debido a que ya fue informada.";

            var ex = Assert.Throws<ValidationCustomException>(() => target.AnularOrden(orderId));

            var result = ex.Message;

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoOrdenDeCarga.Confirmado, orden.Estado);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test()]
        public void EditarOrdenDeCargaInformadaTest()
        {
            int orderId = 1;
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true,
                ContratoIngresado = "1111111",
                NombreChofer = "Enzo V.",
                HistorialCambios = new List<OrdenDeCargaCambiosHistorial> {}
            };

            var orden2 = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true,
                ContratoIngresado = "212121",
                NombreChofer = "Enzo",
                HistorialCambios = new List<OrdenDeCargaCambiosHistorial> { }
            };

            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                },
                Roles = new List<Rol>()
            };

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            var expected = new Resultado { IdEntidad = orden.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada }; ;

            var result = target.Editar(orden2, mailUsuario);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoOrdenDeCarga.Confirmado, orden.Estado);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test()]
        public void ObtenerOrdenPorComercialTest()
        {
            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                },
                Roles = new List<Rol> { new Rol { Codigo = "COMERCIAL" } }
            };

            var ordenId = 1;
            var ordenDeCarga = new OrdenDeCarga { Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333" };

            var expected = new OrdenDeCargaDetalleDto { Id = ordenId, CUITCliente = "233333333333" };

            repositorioMock
                .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(proveedor);

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            repositorioMock
                .Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);

            var result = target.Obtener(mailUsuario, ordenId);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);

            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.CUITCliente, result.CUITCliente);
        }

        [Test()]
        public void verificarVencimientoOrdenDeCargaTest()
        {
            var ordenId = 1;
            var ordenDeCargaLista = new List<OrdenDeCarga>
            {
                new OrdenDeCarga { Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333", Estado = EstadoOrdenDeCarga.EntregaGenerada, FechaEntregaGenerada =  DateTime.Now.AddHours(-90)},
            };

            repositorioMock
                .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
                .Returns(ordenDeCargaLista);

            var result = target.VerificarVencimientoOrdenDeCarga();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(result[0].Estado, EstadoOrdenDeCarga.AnuladaPorVencimiento);
        }

        [Test()]
        public void ObtenerOrdenPorComunTest()
        {
            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                },
                Roles = new List<Rol> { }
            };

            var ordenId = 1;
            var ordenesDeCarga = new List<OrdenDeCarga> { new OrdenDeCarga { Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333" } };

            var expected = new OrdenDeCargaDetalleDto { Id = ordenId, CUITCliente = "233333333333" };

            repositorioMock
                .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(proveedor);

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            
            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
               .Returns(ordenesDeCarga);

            var result = target.Obtener(mailUsuario, ordenId);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);
            
            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.CUITCliente, result.CUITCliente);
        }


    }
}