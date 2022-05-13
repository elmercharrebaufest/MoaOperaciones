using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
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
        private OrdenDeCarga ordenDeCarga;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            consumerOrdenCargaMOA = new Mock<IOrdenCargaConsumerMOA>();
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
            target = new OrdenDeCargaService(repositorioMock.Object, consumerOrdenCargaMOA.Object);
            ordenDeCarga = new OrdenDeCarga
            {
                Id = 1,
                CUITCliente = "20266044993",
                NombreChofer = "Martin",
                ApellidoChofer = "Pfeiffer",
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
                    Id =1,
                    CodigoSap = ""
                },
                NumeroPedido = ""
              
            };
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
               .Setup(x => x.Obtener<Material>(It.IsAny<int>()))
               .Returns(ordenDeCarga.Producto);

            repositorioMock
               .Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>()))
               .Returns(proveedor);

            consumerOrdenCargaMOA
                .Setup(x => x.ControlCargaRequest(It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(),
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
                .Returns("CC-00");

            repositorioMock
                .Setup(x => x.Obtener<OrdenDeCarga>(It.IsAny<int>()))
                .Returns(ordenDeCarga);

            consumerOrdenCargaMOA
               .Setup(x => x.OrdenCargaControlEstadoRequest(It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>()))
               .Returns("CE-07");
            


            ConfigurationManager.AppSettings["CantidadOrdenDeCarga"] = "30000";

            var result = target.Agregar(ordenDeCarga, mailUsuario);

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

            var result = target.SeleccionarPedido(ordenId, pedido);

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
                Cuerpo = "Orden de carga 1 de cliente RS no pasó validaciones crediticias. <br> Número de Contrato: 10000000 <br> Número de Pedido: 25250000"
            };
            Assert.AreEqual(result.Asunto, response.Asunto);
            Assert.AreEqual(result.Cuerpo, response.Cuerpo);
        }

        [Test()]
        public void ConstruirCuerpoEmailOrdenDeCargaCrediticiaTestAppSettingsNull()
        {
            AddProvider(301301301, EstadoAprobacion.Aprobado, "Test", "RS", "dylopez@baufest.com", "233333333333", new TipoUsuario { Id = 5, Nombre = "Cliente", NombreCorto = "CLI" });
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
    }
}