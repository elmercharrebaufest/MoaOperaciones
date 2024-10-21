using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using ScatoRepo = SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
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
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Models.WebApiMap.CNRT;
using SustitucionMOAWS.OrdenCargaControlSAP;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class OrdenDeCargaServiceTest
    {
        private OrdenDeCargaService target;
        private Mock<IRepositorioOrdenDeCarga> repositorioMock;
        private Mock<IOrdenCargaConsumerMOA> consumerOrdenCargaMOA;
        private OrdenDeCarga ordenDeCarga;
        private List<OrdenDeCargaCambiosHistorial> ordenDeCargaCambiosHistorial;
        private Mock<IFeriadoService> feriadoService;
        private Mock<IUsuarioService> usuarioService;
        private Mock<IEmailFasService> mIEmailFasService;
        private Mock<IFacturaAnticipadaService> mIFacturaAnticipadaService;
        private Mock<IScatoRepositorioClient> mIScatoRepositorioClient;
        private Mock<IScatoConsumer> mIScatoConsumer;
        private Mock<IKgDisponiblesFasService> mIKgDisponiblesFasService;
        private Mock<ICNRTClient> mICNRTClient;

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
            //repositorioMock = new Mock<IRepositorio>();
            repositorioMock = new Mock<IRepositorioOrdenDeCarga>();
            consumerOrdenCargaMOA = new Mock<IOrdenCargaConsumerMOA>();
            mIFacturaAnticipadaService = new Mock<IFacturaAnticipadaService>();
            feriadoService = new Mock<IFeriadoService>();
            mIScatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            mIScatoConsumer = new Mock<IScatoConsumer>();
            feriadoService.Setup(fs => fs.ObtenerFeriados()).Returns(new List<DateTime>());
            mIEmailFasService = new Mock<IEmailFasService>();
            mIKgDisponiblesFasService = new Mock<IKgDisponiblesFasService>();
            mICNRTClient = new Mock<ICNRTClient>();
            
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            target = new OrdenDeCargaService(repositorioMock.Object, consumerOrdenCargaMOA.Object, feriadoService.Object,
                mIScatoRepositorioClient.Object, mIScatoConsumer.Object, mIEmailFasService.Object, mIFacturaAnticipadaService.Object,
                mIKgDisponiblesFasService.Object, mICNRTClient.Object);
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

            var result = target.Agregar(ordenDeCarga, _mailSesionUsuario, NoSeGestionaNingunAlta());

            var expected = new Resultado { IdEntidad = 1, Mensaje = SuccessMsg.OrdenDeCargaAgregada };

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Agregar(It.IsAny<OrdenDeCarga>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(4));

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void ListarUsuarioComercialTest()
        {
            var fechaInicio = new DateTime(2023, 3, 10);
            var fechaFin = new DateTime(2023, 3, 17);
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
                Roles = new List<Rol>
                {
                    new Rol
                    {
                        Codigo = "COMERCIAL ",
                        PermisosAsociados = new List<PermisoPorRol>()
                        {
                            new PermisoPorRol { Permiso = "VER ORDENES DE CARGA PARA COMERCIALES" }
                        }
                    }
                }
            };

            var ordenesDeCarga = new List<OrdenDeCarga>()
            {
                new OrdenDeCarga
                {
                    Id = 1, Cliente_Id = 1, CUITCliente = "233333333333",
                    Cliente = new Proveedor { CodigoProveedor = "DS2345", RazonSocial = "Kefwen" },
                    Producto = new Material { Nombre = "mat1" }
                },
                new OrdenDeCarga
                {
                    Id = 2, Cliente_Id = 2, CUITCliente = "255555555555",
                    Cliente = new Proveedor { CodigoProveedor = "JRE6532", RazonSocial = "Mjerehd" },
                    Producto = new Material { Nombre = "mat2" }
                },
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

            //var result = target.Listar(mailUsuario, "", "");
            var result = target.Listar(mailUsuario, fechaInicio.ToString(), fechaFin.ToString());

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
            var fechaInicio = new DateTime(2023, 3, 10);
            var fechaFin = new DateTime(2023, 3, 17);
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
                new OrdenDeCarga
                {
                    Id = 1, Cliente_Id = 1, CUITCliente = "233333333333",
                    Cliente = new Proveedor { CodigoProveedor = "DS2345", RazonSocial = "Kefwen" },
                    Producto = new Material { Nombre = "mat1" }
                },
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

            var result = target.Listar(mailUsuario, fechaInicio.ToString(), fechaFin.ToString());

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
            var usuario = new Usuario
            {
                Mail = mailUsuario,
                Roles = new[] { new Rol { PermisosAsociados = new[] { new PermisoPorRol { Permiso = "ENVIAR A SAP" } } } }
            };
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.EntregaPendiente,
                InformadaSAP = false
            };

            repositorioMock.Setup(x =>
                x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(orden);

            repositorioMock.Setup(x =>
                x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            var result = target.AnularOrden(orderId, mailUsuario);

            var expected = SuccessMsg.OrdenDeCargaAnulada;

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoOrdenDeCarga.Anulada, orden.Estado);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test()]
        public void AnularOrdenNoAnulableTest()
        {
            int orderId = 1;
            var mailUsuario = "usuario@test.com";
            var orden = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Entregada,
                InformadaSAP = true
            };
            var mUsuario = new Mock<Usuario>();
            mUsuario.Setup(x => x.TienePermiso(It.Is<PermisoEnum>(p => p == PermisoEnum.EnviarASap))).Returns(true);

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);
            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(mUsuario.Object);

            var expected = "La orden no puede anularse debido a su estado actual.";

            var ex = Assert.Throws<ValidationCustomException>(() => target.AnularOrden(orderId, mailUsuario));

            var result = ex.Message;

            Assert.AreEqual(expected, result);
            Assert.AreEqual(EstadoOrdenDeCarga.Entregada, orden.Estado);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test()]
        public void EditarOrdenDeCargaInformadaInterno()
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
                CUITChofer = "33703558599",
                CUITTransporte = "20637698295",
                HistorialCambios = new List<OrdenDeCargaCambiosHistorial> { },
                Cliente = proveedor,
                Cliente_Id = proveedor.Id,
                NumeroPedido = "11",
                Producto = producto,
                Producto_Id = producto.Id,
                NumeroEntrega = "9834755",
                ChasisAcoplado = "ABC123"
            };

            var orden2 = new OrdenDeCarga
            {
                Id = orderId,
                Estado = EstadoOrdenDeCarga.Confirmado,
                InformadaSAP = true,
                ContratoIngresado = "212121",
                NombreChofer = "Enzo",
                CUITChofer = "20268774603",
                CUITTransporte = "20111698295",
                HistorialCambios = new List<OrdenDeCargaCambiosHistorial> { },
                Cliente = proveedor,
                Cliente_Id = proveedor.Id,
                NumeroPedido = "11",
                Producto = producto,
                Producto_Id = producto.Id,
                NumeroEntrega = "9834755",
                ChasisAcoplado = "DEF456"
            };
            var respuestaScato = new ScatoRepo.Respuesta<ScatoRepo.Chofer> { IsValid = true, Data = new ScatoRepo.Chofer() };
            var mensajesSap = new ZMPES7060[] { new ZMPES7060 { MENSAJE = "CC-00" } };
            var controlCargaResponseHandler = new ControlCargaResponseHandler(mensajesSap);

            repositorioMock.Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);
            repositorioMock.Setup(x => x.Obtener<Material>(It.IsAny<int>())).Returns(producto);
            repositorioMock
                .Setup(x => x.Listar(
                    It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>(),
                    It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
                .Returns(new List<OrdenDeCarga>());

            consumerOrdenCargaMOA
                .Setup(x => x.ControlarCarga(It.IsAny<ControlCargaRequest>()))
                .Returns(controlCargaResponseHandler);
            consumerOrdenCargaMOA
                .Setup(x => x.OrdenCargaControlEstadoRequest(
                    It.Is<string>(entr => string.IsNullOrEmpty(entr)),
                    It.Is<string>(ped => string.IsNullOrEmpty(ped)),
                    It.Is<string>(tr => !string.IsNullOrEmpty(tr))))
                .Returns("CE-07");

            mIScatoRepositorioClient
                .Setup(x => x.ObtenerChoferPorCuil(It.IsAny<string>()))
                .Returns(respuestaScato);

            var result = target.Editar(orden2, mailUsuario, NoSeGestionaNingunAlta());

            Assert.AreEqual(orderId, result.IdEntidad);
            Assert.AreEqual(SuccessMsg.OrdenDeCargaActualizada, result.Mensaje);
            Assert.AreEqual(EstadoOrdenDeCarga.Confirmado, orden.Estado);
            repositorioMock.Verify(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Exactly(1));
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
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
                Roles = new List<Rol>
                {
                    new Rol
                    {
                        Codigo = "COMERCIAL",
                        PermisosAsociados = new List<PermisoPorRol>
                        {
                            new PermisoPorRol { Permiso = "VER ORDENES DE CARGA PARA COMERCIALES" }
                        }
                    }
                }
            };

            var ordenId = 1;
            var ordenDeCarga = new OrdenDeCarga
            {
                Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333",
                Cliente = new Proveedor { CodigoProveedor = "KJ387" },
                Producto = new Material { Nombre = "mat1" }
            };

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

            repositorioMock
                .Setup(x => x.Listar(
                    It.IsAny<Expression<Func<OrdenDeCargaCambiosHistorial, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>(),
                    It.IsAny<IEnumerable<Expression<Func<OrdenDeCargaCambiosHistorial, object>>>>()))
                .Returns(new List<OrdenDeCargaCambiosHistorial>());

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
                new OrdenDeCarga
                {
                    Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333",
                    Estado = EstadoOrdenDeCarga.EntregaGenerada, FechaEntregaGenerada =  DateTime.Now.AddHours(-90)
                }
            };

            repositorioMock
                .Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
                .Returns(ordenDeCargaLista);
            
            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(new HabilitacionJob { Habilitado = true });

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new Usuario { Id = 3 });

            var result = target.VerificarVencimientoOrdenDeCarga();

            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(result[0].Estado, EstadoOrdenDeCarga.Vencida);
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
            var ordenesDeCarga = new List<OrdenDeCarga>
            {
                new OrdenDeCarga
                {
                    Id = ordenId, Cliente_Id = 1, CUITCliente = "233333333333",
                    Cliente = new Proveedor { CodigoProveedor = "EN432" },
                    Producto = new Material { Nombre = "mat2" }
                }
            };

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

            repositorioMock
                .Setup(x => x.Listar(
                    It.IsAny<Expression<Func<OrdenDeCargaCambiosHistorial, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>(),
                    It.IsAny<IEnumerable<Expression<Func<OrdenDeCargaCambiosHistorial, object>>>>()))
                .Returns(new List<OrdenDeCargaCambiosHistorial>());

            var result = target.Obtener(mailUsuario, ordenId);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()), Times.Exactly(2));
            repositorioMock.Verify(x => x.Obtener<Proveedor>(It.IsAny<int>()), Times.Once);

            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.CUITCliente, result.CUITCliente);
        }

        [Test()]
        public void ObtenerPatentesTest()
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

            var patentesDropDown = new List<AutoCompleteDropdownElement>
            {
                new AutoCompleteDropdownElement { label = ordenDeCarga.ChasisAcoplado, value = ordenDeCarga.PatenteAcoplado }
            };

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
                .Returns(proveedor);

            repositorioMock
                .Setup(x => x.Listar(
                    It.IsAny<Expression<Func<OrdenDeCarga, AutoCompleteDropdownElement>>>(),
                    It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()))
                .Returns(patentesDropDown);

            var result = target.ObtenerPatentes(ordenDeCarga);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ordenes);
            Assert.AreEqual(1, result.ordenes.Count);
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
        public void AnularPedidoEnSap_PedidoTomadoEnSap()
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
                NumeroPedido = "001243898",
                Estado = EstadoOrdenDeCarga.EntregaPendiente
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


        [Test]
        public void Agregar_UsuarioNoPuedeModificarReventa_ThrowValidationCustomException()
        {
            ordenDeCarga.Reventa = true;
            SetupAgregarTests();

            var expected = $"Cliente {_proveedorUsuario.RazonSocial}({_proveedorUsuario.CUIT}) no es revendedor. No puede modificar campo reventa";

            var ex = Assert.Throws<ValidationCustomException>(() => target.Agregar(ordenDeCarga, _mailSesionUsuario, NoSeGestionaNingunAlta()));

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void Agregar_UsuarioPuedeModificarReventa_CreaNormalmente()
        {
            ordenDeCarga.Reventa = true;

            _proveedorUsuario.EsRevendedor = true;

            SetupAgregarTests();
            SetupAgregarSuccess();

            var result = target.Agregar(ordenDeCarga, _mailSesionUsuario, NoSeGestionaNingunAlta());

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

        [Test]
        public void ValidarCamion_ExisteCamionEscalable()
        {
            var chasisParam = "CHA135";
            var acopladoParam = "ACO246";

            var cnrtRes = new EquiposResponse
            {
                Data = new Equipo
                {
                    CategoriaEscalado = "D",
                    Dominios = new List<Dominio>
                    {
                        new Dominio { Rto = new Rto { CantEjes = 2 } },
                        new Dominio { Rto = new Rto { CantEjes = 3 } }
                    }
                }
            };

            mICNRTClient
                .Setup(x => x.ObtenerEquipos(chasisParam, acopladoParam))
                .Returns(cnrtRes);

            var result = target.ValidarCamion(chasisParam, acopladoParam);

            Assert.That(result.ExisteCamion, Is.True);
            Assert.That(result.EsCamionEscalable, Is.True);
            mICNRTClient.Verify(x => x.ObtenerEquipos(chasisParam, acopladoParam), Times.Once);
        }

        [Test]
        public void ValidarCamion_ExisteCamionNoEscalable()
        {
            var chasisParam = "CHA135";
            var acopladoParam = "ACO246";

            var cnrtRes = new EquiposResponse
            {
                Data = new Equipo
                {
                    CategoriaEscalado = "A",
                    Dominios = new List<Dominio>
                    {
                        new Dominio { Rto = new Rto { CantEjes = 2 } },
                        new Dominio { Rto = new Rto { CantEjes = 3 } }
                    }
                }
            };

            mICNRTClient
                .Setup(x => x.ObtenerEquipos(chasisParam, acopladoParam))
                .Returns(cnrtRes);

            var result = target.ValidarCamion(chasisParam, acopladoParam);

            Assert.That(result.ExisteCamion, Is.True);
            Assert.That(result.EsCamionEscalable, Is.False);
            mICNRTClient.Verify(x => x.ObtenerEquipos(chasisParam, acopladoParam), Times.Once);
        }

        [Test]
        public void ValidarCamion_NoExisteCamionEsBitren()
        {
            var chasisParam = "CHA135";
            var acopladoParam = "ACO246";

            var cnrtRes = new EquiposResponse
            {
                Data = new Equipo
                {
                    CategoriaEscalado = "A",
                    Dominios = new List<Dominio>
                    {
                        new Dominio { Rto = new Rto { CantEjes = 2 } },
                        new Dominio { Rto = new Rto { CantEjes = 0 } }
                    }
                }
            };

            mICNRTClient
                .Setup(x => x.ObtenerEquipos(chasisParam, acopladoParam))
                .Returns(cnrtRes);

            var result = target.ValidarCamion(chasisParam, acopladoParam);

            Assert.That(result.ExisteCamion, Is.False);
            mICNRTClient.Verify(x => x.ObtenerEquipos(chasisParam, acopladoParam), Times.Once);
        }

        [Test]
        public void ValidarCamion_NoExisteCamionSinTipoVehiculo()
        {
            var chasisParam = "CHA135";
            var acopladoParam = "ACO246";

            var cnrtRes = new EquiposResponse
            {
                Data = new Equipo
                {
                    CategoriaEscalado = "F",
                    Dominios = new List<Dominio>
                    {
                        new Dominio { Rto = new Rto { CantEjes = 2 } },
                        new Dominio { Rto = new Rto { CantEjes = 0 } }
                    }
                }
            };

            mICNRTClient
                .Setup(x => x.ObtenerEquipos(chasisParam, acopladoParam))
                .Returns(cnrtRes);

            var result = target.ValidarCamion(chasisParam, acopladoParam);

            Assert.That(result.ExisteCamion, Is.False);
            mICNRTClient.Verify(x => x.ObtenerEquipos(chasisParam, acopladoParam), Times.Once);
        }

        [Test]
        public void ValidarCamion_NoExisteCamionSinDominios()
        {
            var chasisParam = "CHA135";
            var acopladoParam = "ACO246";

            var cnrtRes = new EquiposResponse
            {
                Data = new Equipo
                {
                    CategoriaEscalado = "A",
                    Dominios = new List<Dominio>()
                }
            };

            mICNRTClient
                .Setup(x => x.ObtenerEquipos(chasisParam, acopladoParam))
                .Returns(cnrtRes);

            var result = target.ValidarCamion(chasisParam, acopladoParam);

            Assert.That(result.ExisteCamion, Is.False);
            mICNRTClient.Verify(x => x.ObtenerEquipos(chasisParam, acopladoParam), Times.Once);
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
            ConfigurationManager.AppSettings["UsuarioAutomaticoSAP"] = "moaoperaciones@baufest.com";
        }
        [Test()]
        public void GenerarEntregaSAPOkTest()
        {
            var nroEntrega = "001235";
            var ordenCargaEntreResponseHandler = new OrdenCargaEntreResponseHandler("OE-00", nroEntrega);
            ordenDeCarga.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;

            repositorioMock
                .Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);

            consumerOrdenCargaMOA
                .Setup(x => x.CrearEntrega(It.IsAny<CrearEntregaRequest>(), It.IsAny<bool>()))
                .Returns(ordenCargaEntreResponseHandler);

            consumerOrdenCargaMOA
                .Setup(x => x.OrdenCargaControlEstadoRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("CE-00");

            var response = target.VerificarSituacionCrediticia(ordenDeCarga.Id);

            var result = new Resultado()
            {
                Mensaje = string.Concat("Se ha generado la entrega ", nroEntrega, ".")
            };

            Assert.AreEqual(result.Mensaje, response.Mensaje);
        }

        [Test()]
        public void GenerarEntregaSAPTransporteNoExisteTest()
        {
            var respuestaSap = "OE-01";
            var nroEntrega = "";
            var ordenCargaEntreResponseHandler = new OrdenCargaEntreResponseHandler(respuestaSap, nroEntrega);
            ordenDeCarga.Estado = EstadoOrdenDeCarga.PendienteAprobacionCredito;
            
            repositorioMock
                .Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);
            
            consumerOrdenCargaMOA
                .Setup(x => x.CrearEntrega(It.IsAny<CrearEntregaRequest>(), It.IsAny<bool>()))
                .Returns(ordenCargaEntreResponseHandler);

            consumerOrdenCargaMOA
                .Setup(x => x.OrdenCargaControlEstadoRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("CE-00");

            var response = target.VerificarSituacionCrediticia(ordenDeCarga.Id);

            var result = new Resultado()
            {
                Mensaje = "No se pudo generar la entrega. No existe el transportista."
            };

            Assert.AreEqual(result.Mensaje, response.Mensaje);
        }

        [Test]
        public void ValidarChofer_NoExisteEnOrdenesPendientes()
        {
            var cuilChofer = "20343197072";
            var cuitCliente = "30500858628";
            var scatoRes = new ScatoRepo.Respuesta<ScatoRepo.Chofer>
            {
                IsValid = true,
                Data = new ScatoRepo.Chofer()
            };

            mIScatoRepositorioClient
                .Setup(x => x.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer)))
                .Returns(scatoRes);

            repositorioMock
                .Setup(x => x.ObtenerCuitsClientesDeOrdenesPendientesParaChofer(cuilChofer))
                .Returns(new List<string>());

            var resp = target.ValidarChofer(cuilChofer, cuitCliente);

            Assert.That(resp.EsCuilValido);
        }

        [Test]
        public void ValidarChofer_ExisteEnOrdenesPendientesMismoCliente()
        {
            var cuilChofer = "20343197072";
            var cuitCliente = "30500858628";
            var scatoRes = new ScatoRepo.Respuesta<ScatoRepo.Chofer>
            {
                IsValid = true,
                Data = new ScatoRepo.Chofer()
            };

            mIScatoRepositorioClient
                .Setup(x => x.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer)))
                .Returns(scatoRes);

            repositorioMock
                .Setup(x => x.ObtenerCuitsClientesDeOrdenesPendientesParaChofer(cuilChofer))
                .Returns(new List<string> { cuitCliente });

            var resp = target.ValidarChofer(cuilChofer, cuitCliente);

            Assert.That(resp.EsCuilValido);
        }

        [Test]
        public void ValidarChofer_ExisteEnOrdenesPendientesOtroCliente()
        {
            var cuilChofer = "20343197072";
            var cuitCliente = "30500858628";
            var cuitOtroCliente = "30716780291";
            var scatoRes = new ScatoRepo.Respuesta<ScatoRepo.Chofer>
            {
                IsValid = true,
                Data = new ScatoRepo.Chofer()
            };

            mIScatoRepositorioClient
                .Setup(x => x.ObtenerChoferPorCuil(DataFormatter.CuitConGuion(cuilChofer)))
                .Returns(scatoRes);

            repositorioMock
                .Setup(x => x.ObtenerCuitsClientesDeOrdenesPendientesParaChofer(cuilChofer))
                .Returns(new List<string> { cuitCliente, cuitOtroCliente });

            var resp = target.ValidarChofer(cuilChofer, cuitCliente);

            Assert.That(resp.EsCuilValido);
        }
        private GestionAltasFAS NoSeGestionaNingunAlta()
        {
            return new GestionAltasFAS
            {
                GestionaDestinatario = false,
                GestionaDestino = false,
                GestionaFlete = false
            };
        }
    }
}