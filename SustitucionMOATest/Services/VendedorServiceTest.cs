using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.WSConsumers;
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
        private Mock<IVendedorHabilitadoConsumerMOA> vendedorHabilitadoConsumerMock;
        private Mock<IVendedoresConsumerMOA> vendedoresConsumerMOAMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            dataAgroServiceMock = new Mock<IDataAgroService>();
            vendedoresConsumerMOAMock = new Mock<IVendedoresConsumerMOA>();
            vendedorHabilitadoConsumerMock = new Mock<IVendedorHabilitadoConsumerMOA>();
            target = new VendedorService(repositorioMock.Object, dataAgroServiceMock.Object,
                vendedorHabilitadoConsumerMock.Object, vendedoresConsumerMOAMock.Object);
        }

        public void GetVendedoresPendientesTest()
        {
            int proveedorIdUno = 1;

            var proveedorUno = new Proveedor
            {
                Id = proveedorIdUno,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            int proveedorIdDos = 2;

            var proveedorDos = new Proveedor
            {
                Id = proveedorIdDos,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333334",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            var expected = new List<ProveedorDto>
            {
                new ProveedorDto(proveedorUno),
                new ProveedorDto(proveedorDos)
            };

            var mailUsuario = "existente@mail.com";


            var usuarioCorredor = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Corredor" },
                Proveedores = new List<Proveedor> { proveedorUno, proveedorDos }
            };

            repositorioMock
                    .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(usuarioCorredor);


            var result = target.GetVendedoresPendientes(mailUsuario, codigoProveedor: "");

            CollectionAssert.AreEquivalent(expected, result);

        }

        [Test()]
        public void AgregarVendedorNuevoTest()
        {
            int proveedorIdUno = 1;

            var proveedorUno = new Proveedor
            {
                Id = proveedorIdUno,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                RazonSocial = "Razon Social 1",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            int proveedorId = 2;
            string CUIT = "233333333334";
            string razonSocial = "Razon Social 2";

            var proveedorDos = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = CUIT,
                RazonSocial = razonSocial,
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };



            var mailUsuario = "existente@mail.com";

            var usuarioCorredor = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Corredor" },
                Proveedores = new List<Proveedor> { proveedorUno }
            };

            repositorioMock
                    .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(usuarioCorredor);

            //Lo agrego para que no rompa, hay que cambiarlo para que tambien incluya clientes.
            var tipoProveedor = 2;

            var result = target.AgregarVendedor(mailUsuario, CUIT, tipoProveedor);


            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            var expected = SuccessMsg.AltaVendedorOK;

            Assert.AreEqual(expected, result);
        }


        [Test()]
        public void AgregarVendedorRepetidoTest()
        {
            int proveedorIdUno = 1;

            var proveedorUno = new Proveedor
            {
                Id = proveedorIdUno,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                RazonSocial = "Razon Social 1",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            int proveedorId = 2;
            string CUIT = "233333333334";
            string razonSocial = "Razon Social 2";

            var proveedorDos = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = CUIT,
                RazonSocial = razonSocial,
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };



            var mailUsuario = "existente@mail.com";

            var usuarioCorredor = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Corredor" },
                Proveedores = new List<Proveedor> { proveedorUno, proveedorDos }
            };

            repositorioMock
                    .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(usuarioCorredor);

            //Lo agrego para que no rompa, hay que cambiarlo para que tambien incluya clientes.
            var tipoProveedor = 2;

            var ex = Assert.Throws<ValidationCustomException>(() => target.AgregarVendedor(mailUsuario, CUIT, tipoProveedor));

            var expected = ErrorMsg.ErrorVendedorRepetido;

            var result = ex.Message;

            Assert.AreEqual(expected, result);

        }

        [Test()]
        public void EliminarVendedorTest()
        {
            int proveedorIdUno = 1;

            var proveedorUno = new Proveedor
            {
                Id = proveedorIdUno,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            int proveedorIdDos = 2;

            var proveedorDos = new Proveedor
            {
                Id = proveedorIdDos,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333334",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            var mailUsuario = "existente@mail.com";

            var usuarioCorredor = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Corredor" },
                Proveedores = new List<Proveedor> { proveedorUno, proveedorDos }
            };

            repositorioMock
                    .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(usuarioCorredor);


            var result = target.EliminarVendedor(mailUsuario, proveedorIdDos);

            var expected = SuccessMsg.VendedorBorradoOK;

            repositorioMock.Verify(x => x.Remover<Proveedor>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void EliminarVendedorConEstadoAprobadoTest()
        {
            int proveedorIdUno = 1;

            var proveedorUno = new Proveedor
            {
                Id = proveedorIdUno,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            int proveedorIdDos = 2;

            var proveedorDos = new Proveedor
            {
                Id = proveedorIdDos,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                CUIT = "233333333334",
                Archivos = new List<Archivo>(),
                UsuariosAsociados = new List<Usuario>(),
                HistorialAprobaciones = new List<ProveedorHistorialAprobacion>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
            };

            var mailUsuario = "existente@mail.com";

            var usuarioCorredor = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Corredor" },
                Proveedores = new List<Proveedor> { proveedorUno, proveedorDos }
            };

            repositorioMock
                    .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                    .Returns(usuarioCorredor);


            var ex = Assert.Throws<ValidationCustomException>(() => target.EliminarVendedor(mailUsuario, proveedorIdDos));

            var expected = "No se puede elimianr el vendedor debido a que su estado no es \"Documentación pendiente\".";

            var result = ex.Message;

            repositorioMock.Verify(x => x.Remover<Proveedor>(It.IsAny<int>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetVendedoresConUsuarioExistenteTest()
        {
            var mailUsuario = "existente@mail.com";
            var proveedores = new List<Proveedor>() {
                new Proveedor {
                    Id = 1,
                    CUIT = "23-102394598-7",
                    CodigoProveedor = "C12331234",
                    Mail = mailUsuario,
                    RazonSocial = "Test SA",
                    UsuariosAsociados = new List<Usuario>(),
                    TipoProveedor = new TipoUsuario { Id = 1 },
                    FechaSolicitud = new DateTime(),
                    EstadoSIPER = "EstadoSIPER",
                    Comercial = "Comercial",
                    IdDataAgro = 1,
                    IdComercialDataAgro = 1,
                    ContieneDocumentacionFisica = true
                }
            };
            var usuario = new Usuario()
            {
                Mail = mailUsuario,
                CUITRegistro = "23-123464943-9",
                Habilitado = true,
                Proveedores = proveedores,
                Roles = new List<Rol>()
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
                    Id = 1,
                    CUIT = "23-102394598-7",
                    CodigoProveedor = "C12331234",
                    Mail = mailUsuario,
                    RazonSocial = "Test SA",
                    EstadoAprobacion = EstadoAprobacion.AprobacionPendiente,
                    UsuariosAsociados = new List<Usuario>(),
                    TipoProveedor = new TipoUsuario { Id = 1 },
                    FechaSolicitud = new DateTime(),
                    EstadoSIPER = "EstadoSIPER",
                    Comercial = "Comercial",
                    IdDataAgro = 1,
                    IdComercialDataAgro = 1,
                    ContieneDocumentacionFisica = true
                }
            };
            var usuario = new Usuario()
            {
                Mail = mailUsuario,
                CUITRegistro = "23-123464943-9",
                Habilitado = true,
                Proveedores = proveedores,
                Roles = new List<Rol>()
            };

            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            var result = target.GetVendedoresPendientes(mailUsuario, codigoProveedor: "");

            repositorioMock.Verify(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.AtMost(2));

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
                Proveedores = proveedores,
                Roles = new List<Rol>()
            };
            var respuesta = string.Format(InfoMsg.SinRegistros, "Empresas");

            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(usuario);

            var ex = Assert.Throws<InfoCustomException>(() => target.GetVendedoresPendientes(mailUsuario, codigoProveedor: ""));
            Assert.AreEqual(ex.Message, respuesta);
            repositorioMock.Verify(x => x.Obtener<Usuario>(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.AtMost(2));
        }


        [Test]
        public void GetVendedorStatusExistente()
        {
            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string CUIT = "23123464943";
            string usuario = "";
            response.status = "Habilitado";
            vendedorHabilitadoConsumerMock.Setup(x => x.Request(CUIT, "MOA", usuario)).Returns(response);

            var result = target.GetVendedorStatus(CUIT, usuario);

            Assert.AreEqual("Habilitado", result.status);
        }

        [Test]
        public void GetVendedorStatusNoEncontrado()
        {
            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string CUIT = "23123464943";
            string usuario = "";
            response.status = "Habilitado";
            vendedorHabilitadoConsumerMock.Setup(x => x.Request("1111111", "MOA", usuario)).Returns(response);

            var respuesta = InfoMsg.ProveedorSinAlta;

            var ex = Assert.Throws<InfoCustomException>(() => target.GetVendedorStatus(CUIT, usuario));
            Assert.AreEqual(ex.Message, respuesta);
        }

        [Test]
        public void GetVendedorStatusEstadoNoExistente()
        {
            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string CUIT = "23123464943";
            string usuario = "";
            response.status = "Proveedor inexistente";
            vendedorHabilitadoConsumerMock.Setup(x => x.Request(CUIT, "MOA", usuario)).Returns(response);

            var respuesta = InfoMsg.ProveedorSinAlta;

            var ex = Assert.Throws<InfoCustomException>(() => target.GetVendedorStatus(CUIT, usuario));
            Assert.AreEqual(ex.Message, respuesta);
        }

        [Test]
        public void GetVendedorStatusCUITVacio()
        {
            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string CUIT = "23123464943";
            string usuario = "";
            response.status = "Habilitado";
            vendedorHabilitadoConsumerMock.Setup(x => x.Request(CUIT, "MOA", usuario)).Returns(response);

            var respuesta = string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT");

            var ex = Assert.Throws<ValidationCustomException>(() => target.GetVendedorStatus("", usuario));
            Assert.AreEqual(ex.Message, respuesta);
        }

        [Test]
        public void GetVariosVendedoresStatus()
        {
            List<string> cuitsVendedores = new List<string>
            {
                "23123464943",
                "30711160163"
            };

            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string usuario = "";
            response.status = "Habilitado";

            response.cabeceras = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle.Cabecera>
            {
                new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle.Cabecera
                {
                    proveedor = "Pepe",
                    descripcion = "Lui"
                }
            };

            vendedorHabilitadoConsumerMock.Setup(x => x.Request("23123464943", "MOA", usuario)).Returns(response);
            vendedorHabilitadoConsumerMock.Setup(x => x.Request("30711160163", "MOA", usuario)).Returns(response);

            var result = target.GetVariosVendedoresStatus(cuitsVendedores, usuario);

            var expected = new List<EstadoVendedorDto>() {
                new EstadoVendedorDto { CUIT = "23123464943", Estado = "Habilitado", CodigoProveedor ="Pepe", RazonSocial ="Lui" },
                new EstadoVendedorDto { CUIT = "30711160163", Estado = "Habilitado", CodigoProveedor ="Pepe", RazonSocial ="Lui"  }
            };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetVariosVendedoresStatusUnoInexistente()
        {
            List<string> cuitsVendedores = new List<string>
            {
                "23123464943",
                "22222"
            };

            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string usuario = "";
            response.status = "Habilitado";

            response.cabeceras = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle.Cabecera>
            {
                new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle.Cabecera
                {
                    proveedor = "Pepe",
                    descripcion = "Lui"
                }
            };

            vendedorHabilitadoConsumerMock.Setup(x => x.Request("23123464943", "MOA", usuario)).Returns(response);
            vendedorHabilitadoConsumerMock.Setup(x => x.Request("30711160163", "MOA", usuario)).Returns(response);

            var result = target.GetVariosVendedoresStatus(cuitsVendedores, usuario);

            var expected = new List<EstadoVendedorDto>() {
                new EstadoVendedorDto { CUIT = "23123464943", Estado = "Habilitado", CodigoProveedor ="Pepe", RazonSocial ="Lui" },
                new EstadoVendedorDto { CUIT = "22222", Estado =  InfoMsg.ProveedorSinAlta  }
            };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetVariosVendedoresStatusUnoInexistenteConException()
        {
            List<string> cuitsVendedores = new List<string>
            {
                "23123464943",
                "30711160163"
            };

            VendedorHabilitadoWSMOAResponse response = new VendedorHabilitadoWSMOAResponse();

            string usuario = "";
            response.status = "Habilitado";

            response.cabeceras = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle.Cabecera>
            {
                new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle.Cabecera
                {
                    proveedor = "Pepe",
                    descripcion = "Lui"
                }
            };

            var cuitNoEncontradoException = new InfoCustomException("CUIT No Encontrado");
            vendedorHabilitadoConsumerMock.Setup(x => x.Request("23123464943", "MOA", usuario)).Returns(response);
            vendedorHabilitadoConsumerMock.Setup(x => x.Request("30711160163", "MOA", usuario)).Throws(cuitNoEncontradoException);

            var result = target.GetVariosVendedoresStatus(cuitsVendedores, usuario);

            var expected = new List<EstadoVendedorDto>() {
                new EstadoVendedorDto { CUIT = "23123464943", Estado = "Habilitado", CodigoProveedor ="Pepe", RazonSocial ="Lui" },
                new EstadoVendedorDto { CUIT = "30711160163", Estado = cuitNoEncontradoException.Message  }
            };

            Assert.AreEqual(expected, result);
        }
    }
}