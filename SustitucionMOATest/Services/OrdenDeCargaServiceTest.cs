using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using ScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq.Expressions;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class OrdenDeCargaServiceTest
    {
        private OrdenDeCargaService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IOrdenCargaConsumerMOA> consumerOrdenCargaMOA;
        private OrdenDeCarga ordenDeCarga;
        private List<OrdenDeCargaCambiosHistorial> ordenDeCargaCambiosHistorial;
        private Mock<IFeriadoService> feriadoService;
        private Mock<IEmailFasService> mIEmailFasService;
        private Mock<IScatoRepositorioClient> mIScatoRepositorioClient;
        private Mock<IScatoConsumer> mIScatoConsumer;

        private ScatoRepo.Respuesta<ScatoRepo.Chofer> _respuestaChofer;
        private ScatoRepo.Respuesta<ScatoRepo.Chofer> _respuestaTransporte;

        private Proveedor _proveedorUsuario;
        private Usuario _usuario;
        private string _mailSesionUsuario;
        private List<Rol> _rolesUsuario;

        private Rol _rolAdministracion;
        private PermisoPorRol _permisoVerOrdenesComerciales;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            consumerOrdenCargaMOA = new Mock<IOrdenCargaConsumerMOA>();
            feriadoService = new Mock<IFeriadoService>();
            mIScatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            mIScatoConsumer = new Mock<IScatoConsumer>();
            feriadoService.Setup(fs => fs.ObtenerFeriados()).Returns(new List<DateTime>());
            mIEmailFasService = new Mock<IEmailFasService>();
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            target = new OrdenDeCargaService(repositorioMock.Object, consumerOrdenCargaMOA.Object, feriadoService.Object,
                mIScatoRepositorioClient.Object, mIScatoConsumer.Object, mIEmailFasService.Object);
            ordenDeCarga = new OrdenDeCarga
            {
                Id = 1,
                CUITCliente = "20266044993",
                NombreChofer = "Martin",
                CUITChofer = "20391666687",
                PatenteAcoplado = "ABC123",
                ChasisAcoplado = "ABBSM1231412",
                RazonSocialTransporte = "ORLANDI LUIS EDUARDO",
                CUITTransporte = "20391666687",
                Producto_Id = 1,
                Cantidad = 30000,
                Observacion = "Comentarios",
                ContratoIngresado = "33012251",
                CodigoCorredor = "",
                Producto = new Material()
                {
                    Id = 1,
                    CodigoSap = ""
                },
                NumeroPedido = "",
                Cliente = new Proveedor
                {
                    RazonSocial = "ClientePrueba",
                    CodigoProveedor = "498097000"
                },
                NumeroEntrega = ""

            };
            _respuestaChofer = new ScatoRepo.Respuesta<ScatoRepo.Chofer>
            {
                Data = new ScatoRepo.Chofer { },
                Messages = new ScatoRepo.MessageItem[] { },
                IsValid = true
            };

            _mailSesionUsuario = "usuario@test.com";

            _proveedorUsuario = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = _mailSesionUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" },
            };

            _permisoVerOrdenesComerciales = new PermisoPorRol()
            {
                Id = 95,
                Permiso = "VER ORDENES DE CARGA PARA COMERCIALES"

            };

            _rolAdministracion = new Rol
            {
                Id = 1,
                Nombre = "Administracion",
                PermisosAsociados = new List<PermisoPorRol>()
                {
                   _permisoVerOrdenesComerciales
                }

            };
            _rolesUsuario = new List<Rol> { _rolAdministracion };

            _usuario = new Usuario
            {
                Id = 1,
                Mail = _mailSesionUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    _proveedorUsuario
                },

                Roles = _rolesUsuario
            };
            _respuestaTransporte = new ScatoRepo.Respuesta<ScatoRepo.Chofer>
            {
                Data = new ScatoRepo.Chofer { },
                Messages = new ScatoRepo.MessageItem[] { },
                IsValid = false
            };
        }


        [Test()]
        public void AgregarTest()
        {

            SetupAgregarTests();

            SetupAgregarSuccess();

            var result = target.Agregar(ordenDeCarga, _mailSesionUsuario);

            var expected = new Resultado { IdEntidad = 1, Mensaje = SuccessMsg.OrdenDeCargaAgregada };

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<OrdenDeCarga>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));

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

            var result = target.Listar(mailUsuario, "", "");

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

            var result = target.Listar(mailUsuario, "", "");

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
            var mailUsuario = "usuario@test.com";
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Pendiente,
                InformadaSAP = false
            };

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);

            var result = target.AnularOrden(orderId, mailUsuario);

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
            var mailUsuario = "usuario@test.com";
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true
            };

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);

            var expected = "La orden no puede anularse debido a que ya fue informada.";

            var ex = Assert.Throws<ValidationCustomException>(() => target.AnularOrden(orderId, mailUsuario));

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
            string mailUsuario = "usuario@test.com";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                CodigoProveedor = "2323232323",
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

            var producto = new Material
            {
                Id = 1,
                CodigoSap = "23",
                Nombre = "Maiz"
            };

            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true,
                ContratoIngresado = "1111111",
                NombreChofer = "Enzo V.",
                HistorialCambios = new List<OrdenDeCargaCambiosHistorial> { },
                Cliente = proveedor,
                Cliente_Id = proveedor.Id,
                NumeroPedido = "11",
                Producto = producto,
                Producto_Id = producto.Id
            };

            var orden2 = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true,
                ContratoIngresado = "212121",
                NombreChofer = "Enzo",
                HistorialCambios = new List<OrdenDeCargaCambiosHistorial> { },
                Cliente = proveedor,
                Cliente_Id = proveedor.Id,
                NumeroPedido = "11",
                Producto = producto,
                Producto_Id = producto.Id
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

        [Test()]
        public void ObtenerPedidosValidosTest()
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
            var ordenDeCarga = new OrdenDeCarga { Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333", PedidosRespuesta = "1234,123,12,1" };
            var expected = new List<string> { "1234", "123", "12", "1" };

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);

            var result = target.ObtenerPedidos(ordenId);

            Assert.AreEqual(result, expected);
        }

        [Test()]
        public void SeleccionarPedidoValidoTest()
        {
            string mailUsuario = "usuario@test.com";
            string pedido = "1";

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
            var ordenDeCarga = new OrdenDeCarga { Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333", PedidosRespuesta = "" };
            var expected = SuccessMsg.OrdenDeCargaActualizada;

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);

            var result = target.SeleccionarPedido(ordenId, pedido, "");

            Assert.AreEqual(result, expected);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }


        [Test()]
        public void ObtenerPatentesComercialTest()
        {
            string mailUsuario = "usuario@test.com";
            OrdenDeCargaDto ordenCargaDto = new OrdenDeCargaDto();
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

            var permisos = new PermisoPorRol()
            {
                Id = 95,
                Permiso = "VER ORDENES DE CARGA PARA COMERCIALES"

            };

            var roles = new Rol
            {
                Id = 1,
                Nombre = "Administracion",
                PermisosAsociados = new List<PermisoPorRol>()
                {
                   permisos
                }

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
                {
                    roles
                },
                TipoUsuario = new TipoUsuario()
                {
                    Id = 5,
                    Nombre = "Cliente",
                    NombreCorto = "CLI"
                }

            };

            var ordenesDeCarga = new List<OrdenDeCarga> { new OrdenDeCarga

                    {
                      Id = ordenDeCarga.Id,
                      Cliente_Id = 1,
                      CUITCliente = "233333333333",
                      ChasisAcoplado = ordenDeCarga.ChasisAcoplado,
                      PatenteAcoplado = ordenDeCarga.PatenteAcoplado
                    }
            };

            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(usuario);

            repositorioMock
                .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(proveedor);

            repositorioMock
                .Setup(x => x.Obtener<Rol>(It.IsAny<int>()))
                .Returns(roles);

            repositorioMock
               .Setup(x => x.Obtener<PermisoPorRol>(It.IsAny<int>()))
               .Returns(permisos);

            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
               .Returns(ordenesDeCarga);

            var result = target.ObtenerPatentes(ordenDeCarga, mailUsuario);
            Assert.IsTrue(result.ordenes.Count == 1);

        }

        [Test()]
        public void ObtenerPatentesClienteTest()
        {
            string mailUsuario = "usuario@test.com";
            OrdenDeCargaDto ordenCargaDto = new OrdenDeCargaDto();
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

            var permisos = new PermisoPorRol()
            {
                Id = 95,
                Permiso = ""

            };

            var roles = new Rol
            {
                Id = 1,
                Nombre = "Administracion",
                PermisosAsociados = new List<PermisoPorRol>()
                {
                   permisos
                }

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
                {
                    roles
                },
                TipoUsuario = new TipoUsuario()
                {
                    Id = 5,
                    Nombre = "Cliente",
                    NombreCorto = "CLI"
                }

            };

            var ordenesDeCarga = new List<OrdenDeCarga> { new OrdenDeCarga

                    {
                      Id = ordenDeCarga.Id,
                      Cliente_Id = 1,
                      CUITCliente = "233333333333",
                      ChasisAcoplado = ordenDeCarga.ChasisAcoplado,
                      PatenteAcoplado = ordenDeCarga.PatenteAcoplado
                    }
            };

            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(usuario);

            repositorioMock
                .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(proveedor);

            repositorioMock
                .Setup(x => x.Obtener<Rol>(It.IsAny<int>()))
                .Returns(roles);

            repositorioMock
               .Setup(x => x.Obtener<PermisoPorRol>(It.IsAny<int>()))
               .Returns(permisos);

            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
               .Returns(ordenesDeCarga);

            var result = target.ObtenerPatentes(ordenDeCarga, mailUsuario);
            Assert.IsTrue(result.ordenes.Count == 1);

        }

        [Test()]
        public void ObtenerPatentesCorredorTest()
        {
            string mailUsuario = "usuario@test.com";
            OrdenDeCargaDto ordenCargaDto = new OrdenDeCargaDto();
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

            var permisos = new PermisoPorRol()
            {
                Id = 95,
                Permiso = ""

            };

            var roles = new Rol
            {
                Id = 1,
                Nombre = "Administracion",
                PermisosAsociados = new List<PermisoPorRol>()
                {
                   permisos
                }

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
                {
                    roles
                },
                TipoUsuario = new TipoUsuario()
                {
                    Id = 5,
                    Nombre = "Cliente",
                    NombreCorto = "CORR"
                }

            };

            var ordenesDeCarga = new List<OrdenDeCarga> { new OrdenDeCarga

                    {
                      Id = ordenDeCarga.Id,
                      Cliente_Id = 1,
                      CUITCliente = "233333333333",
                      ChasisAcoplado = ordenDeCarga.ChasisAcoplado,
                      PatenteAcoplado = ordenDeCarga.PatenteAcoplado
                    }
            };

            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(usuario);

            repositorioMock
                .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                .Returns(proveedor);

            repositorioMock
                .Setup(x => x.Obtener<Rol>(It.IsAny<int>()))
                .Returns(roles);

            repositorioMock
               .Setup(x => x.Obtener<PermisoPorRol>(It.IsAny<int>()))
               .Returns(permisos);

            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
               .Returns(ordenesDeCarga);

            var result = target.ObtenerPatentes(ordenDeCarga, mailUsuario);
            Assert.IsTrue(result.ordenes.Count == 1);

        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoSAPPedidoSAP()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = "10000000";
            ordenDeCarga.ContratoIngresado = string.Empty;
            ordenDeCarga.PedidoSAP = "25250000";
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = string.Empty;
            ordenDeCarga.NumeroEntrega = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            var result = new EmailSenderData()
            {
                Asunto = "Orden de carga #1  Pedido Bloqueado ClientePrueba",
                Cuerpo = "Orden de carga 1 de cliente RS no pasó validaciones crediticias. <br> Número de Contrato: 10000000 <br> Número de Pedido: 25250000"
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoIngresadoPedidoSAP()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = "25250000";
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = string.Empty;
            ordenDeCarga.NumeroEntrega = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            var result = new EmailSenderData()
            {
                Asunto = "Orden de carga #1",
                Cuerpo = "Orden de carga 1 de cliente RS no pasó validaciones crediticias. <br> Número de Contrato: 10000000 <br> Número de Pedido: 25250000"
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoSAPNumeroPedido()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = "10000000";
            ordenDeCarga.ContratoIngresado = string.Empty;
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = "25250000";
            ordenDeCarga.NumeroPedidoIngresado = string.Empty;
            ordenDeCarga.NumeroEntrega = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            var result = new EmailSenderData()
            {
                Asunto = "Orden de carga #1",
                Cuerpo = "Orden de carga 1 de cliente RS no pasó validaciones crediticias. <br> Número de Contrato: 10000000 <br> Número de Pedido: 25250000"
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoIngresadoNumeroPedido()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = "25250000";
            ordenDeCarga.NumeroPedidoIngresado = string.Empty;
            ordenDeCarga.NumeroEntrega = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            var result = new EmailSenderData()
            {
                Asunto = "Orden de carga #1",
                Cuerpo = "Orden de carga 1 de cliente RS no pasó validaciones crediticias. <br> Número de Contrato: 10000000 <br> Número de Pedido: 25250000"
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoSAPNumeroPedidoIngresado()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = "10000000";
            ordenDeCarga.ContratoIngresado = string.Empty;
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            ordenDeCarga.NumeroEntrega = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            var result = new EmailSenderData()
            {
                Asunto = "Orden de carga #1",
                Cuerpo = "Orden de carga 1 de cliente RS no pasó validaciones crediticias. <br> Número de Contrato: 10000000 <br> Número de Pedido: 25250000"
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoIngresadoNumeroPedidoIngresado()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            var result = new EmailSenderData()
            {
                Asunto = "Orden de carga #1",
                Cuerpo = CrearAsuntoNotificacionValidacionCrediticia()
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }
        [Test()]
        public void ConstruirCuerpoMailSolicitudAnulacionTest()
        {
            ConfigurationManager.AppSettings["EmailToComerciales"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "ariera@baufest.com";
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            var response = target.ConstruirCuerpoMailSolicitudAnulacion(ordenDeCarga.Id);
            var result = new EmailSenderData()
            {
                Asunto = "Solicitud de anulación, Orden de carga N° 1",
                Cuerpo = CrearAsuntoNotificacionSolicitudAnulacion()
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoMailNotificacionVariosContratosTest()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToMesaENTSL"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "ariera@baufest.com";
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            var response = target.ConstruirCuerpoMailNotificacionVariosContratos(ordenDeCarga.Id);
            var result = new EmailSenderData()
            {
                Asunto = "Varios ctto pendientes",
                Cuerpo = CrearAsuntoNotificacionVariosContratos()
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void NotificarVariosContratosTest()
        {
            EjecutarServidoMail();
            var expected = "Notificación enviada";
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToMesaENTSL"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["HostEmail"] = "127.0.0.1";
            ConfigurationManager.AppSettings["PortEmail"] = "1025";
            ConfigurationManager.AppSettings["EmailFrom"] = "moaoperaciones@molinosagro.com.ar";
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            var emailSender = target.ConstruirCuerpoMailNotificacionVariosContratos(ordenDeCarga.Id);
            var response = target.NotificarVariosContratos(emailSender);
            Assert.AreEqual(expected, response);
        }

        [Test()]
        public void ConstruirCuerpoMailNotificacionVariosPedidosTest()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToMesaENTSL"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "ariera@baufest.com";
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            var response = target.ConstruirCuerpoMailNotificacionVariosPedidos(ordenDeCarga.Id);
            var result = new EmailSenderData()
            {
                Asunto = "Varios pedidos pendientes",
                Cuerpo = CrearAsuntoNotificacionVariosPedidos()
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void NotificarVariosPedidosTest()
        {
            EjecutarServidoMail();
            var expected = "Notificación enviada";
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToMesaENTSL"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "ariera@baufest.com";
            ConfigurationManager.AppSettings["HostEmail"] = "127.0.0.1";
            ConfigurationManager.AppSettings["PortEmail"] = "1025";
            ConfigurationManager.AppSettings["EmailFrom"] = "moaoperaciones@molinosagro.com.ar";
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = "10000000";
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = "25250000";
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            var response = target.NotificarVariosPedidos(ordenDeCarga.Id);
            Assert.AreEqual(expected, response);
        }
        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestAppSettingsNull()
        {
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            Assert.IsNull(response);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestClientNotFound()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            ordenDeCarga.Cliente_Id = 1;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = string.Empty;
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            Assert.IsNull(response);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestContratoNull()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = string.Empty;
            ordenDeCarga.ContratoIngresado = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            Assert.IsNull(response);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaTestPedidoNull()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            ordenDeCarga.Cliente_Id = 301301301;
            ordenDeCarga.ContratoSAP = "10000000";
            ordenDeCarga.ContratoIngresado = string.Empty;
            ordenDeCarga.PedidoSAP = string.Empty;
            ordenDeCarga.NumeroPedido = string.Empty;
            ordenDeCarga.NumeroPedidoIngresado = string.Empty;
            var response = target.ConstruirCuerpoEmail(ordenDeCarga);
            Assert.IsNull(response);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaHistorialCrediticiaTest()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);


            ordenDeCargaCambiosHistorial = new List<OrdenDeCargaCambiosHistorial>
            {
                new OrdenDeCargaCambiosHistorial
                {
                    Id = 1,
                    OrdenDeCarga_Id = 636,
                    NombreColumnaCambio = "PatenteAcoplado",
                    Antes = "ABC123",
                    Despues = "123ABC"
                }
            };
            var numeroEntrega = "E1020";
            var numeroPedido = "25250000";
            var response = target.ConstruirCuerpoEmail(ordenDeCargaCambiosHistorial, numeroEntrega, numeroPedido);
            var result = new EmailSenderData()
            {
                Asunto = "Molinos Agro - Edición en su orden de carga n°: 636",
                Cuerpo = CrearAsuntoEdicionOrdenDeCarga()
            };
            result.Cuerpo = result.Cuerpo + "";
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo.Trim(), response.Cuerpo.Trim());


        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaHistorialCrediticiaTestAppSettingsNull()
        {
            var response = target.ConstruirCuerpoEmail(new List<OrdenDeCargaCambiosHistorial>(), "E1020", "25250000");
            Assert.IsNull(response);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaHistorialCrediticiaTestListEmpty()
        {
            ConfigurationManager.AppSettings["EmailToMesaVentaFas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToCobranzas"] = "dylopez@baufest.com";
            ConfigurationManager.AppSettings["EmailToComerciales"] = "dylopez@baufest.com";
            var response = target.ConstruirCuerpoEmail(new List<OrdenDeCargaCambiosHistorial>(), "E1020", "25250000");
            Assert.IsNull(response);
        }
        [Test]
        public void ValidarCuilChoferDigito_CuilNoExiste_ReturnsTrue()
        {
            _respuestaChofer.IsValid = false;
            _respuestaChofer.Messages = new ScatoRepo.MessageItem[]
            {
                new ScatoRepo.MessageItem
                {
                    MessageCode = ScatoRepo.CodigoMensajeObtenerChoferPorCuil.ChoferNoEncontrado
                }
            };
            mIScatoRepositorioClient.Setup(src => src.ObtenerChoferPorCuil(It.IsAny<string>())).Returns(
                _respuestaChofer
                );

            var result = target.ValidarCuilChoferDigito("11111111111");

            Assert.That(result, Is.True);

        }
        [Test]
        public void ValidarCuilChoferDigito_CuilDigitoVerificadorNoValido_ReturnsFalse()
        {
            _respuestaChofer.IsValid = false;
            _respuestaChofer.Messages = new ScatoRepo.MessageItem[]
            {
                new ScatoRepo.MessageItem
                {
                    MessageCode = ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido
                }
            };
            mIScatoRepositorioClient.Setup(src => src.ObtenerChoferPorCuil(It.IsAny<string>())).Returns(
                _respuestaChofer
                );

            var result = target.ValidarCuilChoferDigito("11111111111");

            Assert.That(result, Is.False);

        }
        [Test]
        public void ValidarCuitTransporteDigito_CuitDigitoVerificadorNoValido_ReturnsFalse()
        {
            _respuestaTransporte.Messages = new ScatoRepo.MessageItem[]
            {
                new ScatoRepo.MessageItem
                {
                    MessageCode = ScatoRepo.CodigoMensajeObtenerChoferPorCuil.DigitoVerificadorNoValido
                }
            };
            mIScatoRepositorioClient.Setup(src => src.ObtenerTransportePorCuit(It.IsAny<string>())).Returns(
                _respuestaTransporte
                );
            var result = target.ValidarCuitTransporteDigito("11111111111");

            Assert.That(result, Is.False);
        }
        [Test]
        public void AnularPedidoEnSap_RespuestaSinMapear_ThrowInfoCustomException()
        {
            var usuario = new Usuario
            {
                Roles = new List<Rol>
                {
                    new Rol {
                        PermisosAsociados= new List<PermisoPorRol>
                        {
                           new PermisoPorRol{ Permiso="ENVIAR A SAP" }
                        }
                    }
                }
            };
            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(usuario);
            repositorioMock
            .Setup(y => y.Obtener<OrdenDeCarga>(It.IsAny<int>()))
            .Returns(new OrdenDeCarga
            {
                NumeroPedido = "001243898"
            });
            var handler = new ModOrdenCargaResponseHandler("Pedido tomado en SAP");

            consumerOrdenCargaMOA.Setup(c => c.AnularOrdenCarga(It.IsAny<OrdenDeCarga>())).Returns(handler);

            Assert.That(() => target.AnularOrden(3, ""), Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void AnularPedidoEnSap_PedidoEntregaYaAnulados_OrdenEstadoAnulada()
        {
            var orden = new OrdenDeCarga
            {
                NumeroPedido = "001243898",
                NumeroEntrega = "001243898",
                Estado = EstadoOrdenDeCarga.EntregaGenerada
            };
            var usuario = new Usuario
            {
                Roles = new List<Rol>
                {
                    new Rol {
                        PermisosAsociados= new List<PermisoPorRol>
                        {
                           new PermisoPorRol{ Permiso="ENVIAR A SAP" }
                        }
                    }
                }
            };
            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(usuario);
            repositorioMock
            .Setup(y => y.Obtener<OrdenDeCarga>(It.IsAny<int>()))
            .Returns(orden);
            var handlerPedido = new ModOrdenCargaResponseHandler("Pedido ya anulado");
            var handlerEntrega = new ModEntregaResponseHandler("Entrega anulada en SAP");

            consumerOrdenCargaMOA.Setup(c => c.AnularOrdenCarga(It.IsAny<OrdenDeCarga>())).Returns(handlerPedido);
            consumerOrdenCargaMOA.Setup(c => c.AnularEntregaOrdenCarga(It.IsAny<string>())).Returns(handlerEntrega);

            target.AnularOrden(1, "");

            Assert.That(orden.Estado, Is.EqualTo(EstadoOrdenDeCarga.Anulada));

        }

            Assert.That(result, Is.False);
        }

        [Test]
        public void Agregar_UsuarioNoPuedeModificarReventa_ThrowValidationCustomException()
        {
            ordenDeCarga.Reventa = true;
            SetupAgregarTests();

            var result = target.Agregar(ordenDeCarga, _mailSesionUsuario);

            Assert.That(result.error, Is.EqualTo("Usuario sin permiso para modificar campo reventa"));
        }

        [Test]
        public void Agregar_UsuarioPuedeModificarReventa_CreaNormalmente()
        {
            ordenDeCarga.Reventa = true;
            var permisoModificarReventa = new PermisoPorRol { Id = 123, Permiso = Permisos.FasModificarCampoReventa };
            var rolRevendedor = new Rol { Id = 123, Nombre = "Revendedor", PermisosAsociados = new List<PermisoPorRol> { permisoModificarReventa } };

            _usuario.Roles.Add(rolRevendedor);

            SetupAgregarTests();
            SetupAgregarSuccess();

            var result = target.Agregar(ordenDeCarga, _mailSesionUsuario);

            Assert.That(result.Mensaje, Is.EqualTo(SuccessMsg.OrdenDeCargaAgregada));

        }
        [Test]
        public void ValidarCuitTransporteDigito_CuitTransporteNoExiste_ReturnsTrue()
        {
            _respuestaTransporte.Messages = new ScatoRepo.MessageItem[]
            {
                new ScatoRepo.MessageItem
                {
                    MessageCode = ScatoRepo.CodigoMensajeObtenerChoferPorCuil.ChoferNoEncontrado
                }
            };
            mIScatoRepositorioClient.Setup(src => src.ObtenerTransportePorCuit(It.IsAny<string>())).Returns(
                _respuestaTransporte
                );

            var result = target.ValidarCuitTransporteDigito("11111111111");

            Assert.That(result, Is.True);

        }
        [Test]
        public void ValidarCuitTransporteDigito_CuitTransporteExiste_ReturnsTrue()
        {
            _respuestaTransporte.IsValid = true;

            mIScatoRepositorioClient.Setup(src => src.ObtenerTransportePorCuit(It.IsAny<string>())).Returns(
                _respuestaTransporte
                );

            var result = target.ValidarCuitTransporteDigito("11111111111");

            Assert.That(result, Is.True);

        }
        private void AddProvider(int id, EstadoAprobacion estadoAprobacion, string observaciones, string razonSocial, string mail, string cUIT, TipoUsuario tipoProveedor)
        {
            var proveedor = new Proveedor
            {
                Id = id,
                EstadoAprobacion = estadoAprobacion,
                Observaciones = observaciones,
                RazonSocial = razonSocial,
                Mail = mail,
                CUIT = cUIT,
                TipoProveedor = tipoProveedor,
            };
            repositorioMock
                .Setup(x => x.Obtener<Proveedor>(It.IsIn<int>(id)))
                .Returns(proveedor);
        }

        private string CrearAsuntoEdicionOrdenDeCarga()
        {
            var fecha = DateTime.Now.Date;
            string asunto = string.Empty;
            asunto += $"<!DOCTYPE html>\r\n";
            asunto += $"<html>\r\n";
            asunto += $"<head>\r\n    ";
            asunto += $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n";
            asunto += $"</head>\r\n";
            asunto += $"<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n    ";
            asunto += $"<p>Buenos d&iacute;as,</p>\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Se informa que el día {fecha} se han realizado las siguientes modificaciones para la orden de carga 636 con numero de entrega E1020:</p>\r\n    ";
            asunto += $"<br />\r\n\r\n    ";
            asunto += $"<table>\r\n        ";
            asunto += $"<caption>Cambios:</caption>\r\n\r\n        ";
            asunto += $"<thead style=\"background-color: #adacac;\">\r\n            ";
            asunto += $"<tr>\r\n                ";
            asunto += $"<td scope=\"col\">Nombre de la Columna</td>\r\n                ";
            asunto += $"<td scope=\"col\">Antes del cambio</td>\r\n                ";
            asunto += $"<td scope=\"col\">Despues del cambio</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha</td>\r\n            ";
            asunto += $"</tr>\r\n        ";
            asunto += $"</thead>\r\n\r\n        ";
            asunto += $"<tbody>\r\n            ";
            asunto += $"<tr>";
            asunto += $"<td>PatenteAcoplado</td>";
            asunto += $"<td>ABC123</td>";
            asunto += $"<td>123ABC</td>";
            asunto += $"<td></td>";
            asunto += $"</tr>\r\n\r\n        ";
            asunto += $"</tbody>\r\n    ";
            asunto += $"</table>\r\n\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Saludos,</p>\r\n    ";
            asunto += $"<p>Moa Operaciones</p>\r\n";
            asunto += $"</body>\r\n";
            asunto += $"</html>";
            return asunto;
        }

        private string CrearAsuntoNotificacionValidacionCrediticia()
        {
            var fecha = DateTime.Now.ToString();
            string asunto = string.Empty;
            asunto += $"<!DOCTYPE html>\r\n";
            asunto += $"<html>\r\n";
            asunto += $"<head>\r\n    ";
            asunto += $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n";
            asunto += $"</head>\r\n";
            asunto += $"<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n    ";
            asunto += $"<p>Buenos d&iacute;as,</p>\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Se informa que la siguiente orden de carga no pasó las validaciones crediticias.</p>\r\n    ";
            asunto += $"<br />\r\n\r\n    ";
            asunto += $"<table cellspacing=\"5\" cellpadding=\"5\" border=\"3\">\r\n        ";
            asunto += $"<caption>Orden :</caption>\r\n        ";
            asunto += $"<thead style=\"background-color: #adacac;\">\r\n            ";
            asunto += $"<tr>\r\n                ";
            asunto += $"<td scope=\"col\">Número de orden</td>\r\n                ";
            asunto += $"<td scope=\"col\">Número de contrato</td>\r\n                ";
            asunto += $"<td scope=\"col\">Cliente</td>\r\n                ";
            asunto += $"<td scope=\"col\">Corredor</td>\r\n                ";
            asunto += $"<td scope=\"col\">Chofer</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente acoplado</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente Chasis</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de pedido</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de entrega</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha carga</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha Vencimiento</td>\r\n            ";
            asunto += $"</tr>\r\n        ";
            asunto += $"</thead>\r\n\r\n        ";
            asunto += $"<tbody>\r\n            ";
            asunto += $"<tr>";
            asunto += $"<td>1</td>";
            asunto += $"<td>10000000</td>";
            asunto += $"<td>RS</td>";
            asunto += $"<td></td>";
            asunto += $"<td>Martin</td>";
            asunto += $"<td>ABC123</td>";
            asunto += $"<td>ABBSM1231412</td>";
            asunto += $"<td></td>";
            asunto += $"<td></td>";
            asunto += $"<td>1/1/0001 00:00:00</td>";
            asunto += $"<td></td>";
            asunto += $"</tr>\r\n        ";
            asunto += $"</tbody>\r\n    ";
            asunto += $"</table>\r\n\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Saludos,</p>\r\n    ";
            asunto += $"<p>Moa Operaciones</p>\r\n";
            asunto += $"</body>\r\n";
            asunto += $"</html>";
            return asunto;
        }


        private string CrearAsuntoNotificacionSolicitudAnulacion()
        {
            var fecha = DateTime.Now;
            string asunto = string.Empty;
            asunto += $"<!DOCTYPE html>\r\n";
            asunto += $"<html>\r\n";
            asunto += $"<head>\r\n    ";
            asunto += $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n";
            asunto += $"</head>\r\n";
            asunto += $"<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n    ";
            asunto += $"<p>Buenos d&iacute;as,</p>\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Se informa que el día {fecha} se ha solicitado la anulación de la siguiente orden de carga:</p>\r\n    ";
            asunto += $"<br />\r\n\r\n    ";
            asunto += $"<table cellspacing=\"5\" cellpadding=\"5\" border=\"3\">\r\n        ";
            asunto += $"<caption>Orden :</caption>\r\n        ";
            asunto += $"<thead style=\"background-color: #adacac;\">\r\n            ";
            asunto += $"<tr>\r\n                ";
            asunto += $"<td scope=\"col\">Número de orden</td>\r\n                ";
            asunto += $"<td scope=\"col\">Número de contrato</td>\r\n                ";
            asunto += $"<td scope=\"col\">Cliente</td>\r\n                ";
            asunto += $"<td scope=\"col\">Corredor</td>\r\n                ";
            asunto += $"<td scope=\"col\">Chofer</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente Chasis</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente acoplado</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de pedido</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de entrega</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha carga</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha Vencimiento</td>\r\n            ";
            asunto += $"</tr>\r\n        ";
            asunto += $"</thead>\r\n\r\n        ";
            asunto += $"<tbody>\r\n            ";
            asunto += $"<tr>";
            asunto += $"<td>1</td>";
            asunto += $"<td>10000000</td>";
            asunto += $"<td>ClientePrueba</td>";
            asunto += $"<td></td>";
            asunto += $"<td>Martin</td>";
            asunto += $"<td>ABBSM1231412</td>";
            asunto += $"<td>ABC123</td>";
            asunto += $"<td></td>";
            asunto += $"<td></td>";
            asunto += $"<td>1/1/0001 00:00:00</td>";
            asunto += $"<td></td>";
            asunto += $"</tr>\r\n        ";
            asunto += $"</tbody>\r\n    ";
            asunto += $"</table>\r\n\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Saludos,</p>\r\n    ";
            asunto += $"<p>Moa Operaciones</p>\r\n";
            asunto += $"</body>\r\n";
            asunto += $"</html>";
            return asunto;
        }
        private string CrearAsuntoNotificacionVariosContratos()
        {
            var fecha = DateTime.Now.Date;
            string asunto = string.Empty;
            asunto += $"<!DOCTYPE html>\r\n";
            asunto += $"<html>\r\n";
            asunto += $"<head>\r\n    ";
            asunto += $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n";
            asunto += $"</head>\r\n";
            asunto += $"<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n    ";
            asunto += $"<p>Buenos d&iacute;as,</p>\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Se econtraron varios contratos para el mismo cliente</p>\r\n    ";
            asunto += $"<br />\r\n\r\n    ";
            asunto += $"<table cellspacing=\"5\" cellpadding=\"5\" border=\"3\">\r\n        ";
            asunto += $"<caption>Orden :</caption>\r\n        ";
            asunto += $"<thead style=\"background-color: #adacac;\">\r\n            ";
            asunto += $"<tr>\r\n                ";
            asunto += $"<td scope=\"col\">Número de orden</td>\r\n                ";
            asunto += $"<td scope=\"col\">Número de contrato</td>\r\n                ";
            asunto += $"<td scope=\"col\">Cliente</td>\r\n                ";
            asunto += $"<td scope=\"col\">Corredor</td>\r\n                ";
            asunto += $"<td scope=\"col\">Chofer</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente acoplado</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente Chasis</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de pedido</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de entrega</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha carga</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha Vencimiento</td>\r\n            ";
            asunto += $"</tr>\r\n        ";
            asunto += $"</thead>\r\n\r\n        ";
            asunto += $"<tbody>\r\n            ";
            asunto += $"<tr>";
            asunto += $"<td>1</td>";
            asunto += $"<td>10000000</td>";
            asunto += $"<td>ClientePrueba</td>";
            asunto += $"<td></td>";
            asunto += $"<td>Martin</td>";
            asunto += $"<td>ABC123</td>";
            asunto += $"<td>ABBSM1231412</td>";
            asunto += $"<td></td>";
            asunto += $"<td></td>";
            asunto += $"<td>1/1/0001 00:00:00</td>";
            asunto += $"<td></td>";
            asunto += $"</tr>\r\n        ";
            asunto += $"</tbody>\r\n    ";
            asunto += $"</table>\r\n\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Saludos,</p>\r\n    ";
            asunto += $"<p>Moa Operaciones</p>\r\n";
            asunto += $"</body>\r\n";
            asunto += $"</html>";
            return asunto;
        }

        private string CrearAsuntoNotificacionVariosPedidos()
        {
            var fecha = DateTime.Now.Date;
            string asunto = string.Empty;
            asunto += $"<!DOCTYPE html>\r\n";
            asunto += $"<html>\r\n";
            asunto += $"<head>\r\n    ";
            asunto += $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n";
            asunto += $"</head>\r\n";
            asunto += $"<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n    ";
            asunto += $"<p>Buenos d&iacute;as,</p>\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Se encontraron varios pedidos pendientes para el mismo cliente</p>\r\n    ";
            asunto += $"<br />\r\n\r\n    ";
            asunto += $"<table cellspacing=\"5\" cellpadding=\"5\" border=\"3\">\r\n        ";
            asunto += $"<caption>Orden :</caption>\r\n        ";
            asunto += $"<thead style=\"background-color: #adacac;\">\r\n            ";
            asunto += $"<tr>\r\n                ";
            asunto += $"<td scope=\"col\">Número de orden</td>\r\n                ";
            asunto += $"<td scope=\"col\">Número de contrato</td>\r\n                ";
            asunto += $"<td scope=\"col\">Cliente</td>\r\n                ";
            asunto += $"<td scope=\"col\">Corredor</td>\r\n                ";
            asunto += $"<td scope=\"col\">Chofer</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente acoplado</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente Chasis</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de pedido</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de entrega</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha carga</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha Vencimiento</td>\r\n            ";
            asunto += $"</tr>\r\n        ";
            asunto += $"</thead>\r\n\r\n        ";
            asunto += $"<tbody>\r\n            ";
            asunto += $"<tr>";
            asunto += $"<td>1</td>";
            asunto += $"<td>10000000</td>";
            asunto += $"<td>ClientePrueba</td>";
            asunto += $"<td></td>";
            asunto += $"<td>Martin</td>";
            asunto += $"<td>ABC123</td>";
            asunto += $"<td>ABBSM1231412</td>";
            asunto += $"<td></td>";
            asunto += $"<td></td>";
            asunto += $"<td>1/1/0001 00:00:00</td>";
            asunto += $"<td></td>";
            asunto += $"</tr>\r\n        ";
            asunto += $"</tbody>\r\n    ";
            asunto += $"</table>\r\n\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Saludos,</p>\r\n    ";
            asunto += $"<p>Moa Operaciones</p>\r\n";
            asunto += $"</body>\r\n";
            asunto += $"</html>";
            return asunto;
        }

        private string CrearAsuntoNotificacionContratoVencido()
        {
            var fecha = DateTime.Now.Date;
            string asunto = string.Empty;
            asunto += $"<!DOCTYPE html>\r\n";
            asunto += $"<html>\r\n";
            asunto += $"<head>\r\n    ";
            asunto += $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n";
            asunto += $"</head>\r\n";
            asunto += $"<body style=\"width: 100%; font-family: Helvetica; font-size: 14px; line-height: 1.4; margin: 0; padding: 0; -ms-text-size-adjust: 100%; -webkit-text-size-adjust: 100%;\">\r\n    ";
            asunto += $"<p>Buenos d&iacute;as,</p>\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Contrato vencido Nro :10000000</p>\r\n    ";
            asunto += $"<br />\r\n\r\n    ";
            asunto += $"<table cellspacing=\"5\" cellpadding=\"5\" border=\"3\">\r\n        ";
            asunto += $"<caption>Orden :</caption>\r\n        ";
            asunto += $"<thead style=\"background-color: #adacac;\">\r\n            ";
            asunto += $"<tr>\r\n                ";
            asunto += $"<td scope=\"col\">Número de orden</td>\r\n                ";
            asunto += $"<td scope=\"col\">Número de contrato</td>\r\n                ";
            asunto += $"<td scope=\"col\">Cliente</td>\r\n                ";
            asunto += $"<td scope=\"col\">Corredor</td>\r\n                ";
            asunto += $"<td scope=\"col\">Chofer</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente acoplado</td>\r\n                ";
            asunto += $"<td scope=\"col\">Patente Chasis</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de pedido</td>\r\n                ";
            asunto += $"<td scope=\"col\">Numero de entrega</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha carga</td>\r\n                ";
            asunto += $"<td scope=\"col\">Fecha Vencimiento</td>\r\n            ";
            asunto += $"</tr>\r\n        ";
            asunto += $"</thead>\r\n\r\n        ";
            asunto += $"<tbody>\r\n            ";
            asunto += $"<tr>";
            asunto += $"<td>1</td>";
            asunto += $"<td>10000000</td>";
            asunto += $"<td>ClientePrueba</td>";
            asunto += $"<td></td>";
            asunto += $"<td>Martin</td>";
            asunto += $"<td>ABC123</td>";
            asunto += $"<td>ABBSM1231412</td>";
            asunto += $"<td></td>";
            asunto += $"<td></td>";
            asunto += $"<td>1/1/0001 00:00:00</td>";
            asunto += $"<td></td>";
            asunto += $"</tr>\r\n        ";
            asunto += $"</tbody>\r\n    ";
            asunto += $"</table>\r\n\r\n    ";
            asunto += $"<br />\r\n    ";
            asunto += $"<p>Saludos,</p>\r\n    ";
            asunto += $"<p>Moa Operaciones</p>\r\n";
            asunto += $"</body>\r\n";
            asunto += $"</html>";
            return asunto;
        }
        public void EjecutarServidoMail()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = true;
            info.FileName = "MailHog_windows_amd64.exe";
            info.WorkingDirectory = "C:/Repositorios/Web Operaciones/SustitucionMOATest/bin/Debug/Templates";
            Process.Start(info);

        }

        public void DetenerServidorMail()
        {
            Process[] proc = Process.GetProcessesByName("MailHog_windows_amd64");
            proc[0].Kill();
        }
        private void SetupAgregarTests()
        {

            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(_usuario);

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                 .Returns(_proveedorUsuario);

            repositorioMock
                .Setup(x => x.Obtener<Rol>(It.IsAny<int>()))
                .Returns(_rolAdministracion);

            repositorioMock
               .Setup(x => x.Obtener<PermisoPorRol>(It.IsAny<int>()))
               .Returns(_permisoVerOrdenesComerciales);

            repositorioMock
               .Setup(x => x.Obtener<Material>(It.IsAny<int>()))
               .Returns(ordenDeCarga.Producto);

            repositorioMock
               .Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
               .Returns(_proveedorUsuario);
            mIScatoRepositorioClient.Setup(src => src.ObtenerChoferPorCuil(It.IsAny<string>())).Returns(
               _respuestaChofer
               );
        }
        private void SetupAgregarSuccess()
        {

            consumerOrdenCargaMOA
                .Setup(x => x.ControlarCarga(It.IsAny<ControlCargaRequest>()))
                .Returns(new SustitucionMOAWS.ResponseHandler.OrdenCarga.ControlCargaResponseHandler(
                    new SustitucionMOAWS.OrdenCargaControlSAP.ZMPES7060[]
                    {
                        new SustitucionMOAWS.OrdenCargaControlSAP.ZMPES7060 { MENSAJE = "CC-00" }
                    }));

            repositorioMock
                .Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);

            consumerOrdenCargaMOA
               .Setup(x => x.OrdenCargaVisualizarClienteExecute(It.IsAny<OrdenCargaVisualizarClienteWSMOARequest>()))
               .Returns(new OrdenCargaVisualizarClienteWSMOAResponse
               {
                   Resultados = new List<Result> {
               new Result { FechaHasta = DateTime.Now.AddDays(1).ToString()}
               }
               });

            consumerOrdenCargaMOA
               .Setup(x => x.OrdenCargaControlEstadoRequest(It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>()))
               .Returns("CE-07");

            ConfigurationManager.AppSettings["CantidadOrdenDeCarga"] = "30000";
        }
        [Test()]
        public void GenerarEntregaSAPOkTest()
        {
            var ordenCargaEntreResponseHandlerMock = new Mock<OrdenCargaEntreResponseHandler>();
            var respuesta = "OE-00";
            ordenDeCarga.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            ordenCargaEntreResponseHandlerMock.Setup(x => x.GetResultado()).Returns(CrearEntregaResEnum.OK);
            consumerOrdenCargaMOA
                .Setup(x => x.CrearEntrega(It.IsAny<CrearEntregaRequest>(), It.IsAny<bool>()))
                .Returns(ordenCargaEntreResponseHandlerMock.Object); //"OE-00");

            consumerOrdenCargaMOA.Setup(x => x.OrdenCargaControlEstadoRequest(
              It.IsAny<string>(),
              It.IsAny<string>(),
              It.IsAny<string>())).Returns("CE-00");


            var response = target.VerificarSituacionCrediticia(ordenDeCarga.Id);

            var result = new Resultado()
            {
                Mensaje = string.Concat("Se ha generado la entrega ", respuesta, ".")
            };

            Assert.AreEqual(result.Mensaje, response.Mensaje);


        }

        [Test()]
        public void GenerarEntregaSAPTransporteNoExisteTest()
        {
            var ordenCargaEntreResponseHandlerMock = new Mock<OrdenCargaEntreResponseHandler>();
            ordenDeCarga.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(ordenDeCarga);
            ordenCargaEntreResponseHandlerMock.Setup(x => x.GetResultado()).Returns(CrearEntregaResEnum.NoExisteTransportista);
            consumerOrdenCargaMOA
                .Setup(x => x.CrearEntrega(It.IsAny<CrearEntregaRequest>(), It.IsAny<bool>()))
                .Returns(ordenCargaEntreResponseHandlerMock.Object); //"OE-01");

            consumerOrdenCargaMOA.Setup(x => x.OrdenCargaControlEstadoRequest(
              It.IsAny<string>(),
              It.IsAny<string>(),
              It.IsAny<string>())).Returns("CE-00");


            var response = target.VerificarSituacionCrediticia(ordenDeCarga.Id);

            var result = new Resultado()
            {
                Mensaje = "No se pudo generar la entrega. No existe el transportista."
            };

            Assert.AreEqual(result.Mensaje, response.Mensaje);


        }


    }
}