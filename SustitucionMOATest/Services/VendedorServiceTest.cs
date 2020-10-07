using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
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
    public class VendedorServiceTest
    {
        private VendedorService target;
        private Mock<IRepositorio> repositorioMock;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new VendedorService(repositorioMock.Object);
        }

        [Test()]
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


            var result = target.GetVendedoresPendientes(mailUsuario);

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



            var result = target.AgregarVendedor(mailUsuario, CUIT, razonSocial);


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



            var ex = Assert.Throws<ValidationCustomException>(() => target.AgregarVendedor(mailUsuario, CUIT, razonSocial));

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
    }
}
