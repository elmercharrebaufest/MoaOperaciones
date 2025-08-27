using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto.OrdenDeCargaCommon;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ScatoConsumerWS = SustitucionMOAWS.ScatoWebService;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class OrdenDeCargaFasonServiceTest
    {
        private IOrdenDeCargaFasonService service;
        private Mock<IRepositorioOrdenDeCargaFason> repositorioOrdenDeCargaFason;
        private Mock<IOrdenCargaConsumerMOA> ordenCargaConsumer;
        private Mock<IScatoConsumer> scatoConsumer;
        private Mock<IScatoRepositorioClient> scatoRepositorioClient;
        private Mock<ICNRTClient> cnrtClient;
        private Mock<IEmailFasonService> emailFasonService;
        private Mock<IFeriadoService> feriadoService;
        private Mock<IUbicacionGeograficaService> mIUbicacionGeograficaService;
        [SetUp]
        public void Setup()
        {
            repositorioOrdenDeCargaFason = new Mock<IRepositorioOrdenDeCargaFason>();
            ordenCargaConsumer = new Mock<IOrdenCargaConsumerMOA>();
            scatoConsumer = new Mock<IScatoConsumer>();
            scatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            cnrtClient = new Mock<ICNRTClient>();
            emailFasonService = new Mock<IEmailFasonService>();
            feriadoService = new Mock<IFeriadoService>();
            mIUbicacionGeograficaService = new Mock<IUbicacionGeograficaService>();
            service = new OrdenDeCargaFasonService(
                repositorioOrdenDeCargaFason.Object,
                ordenCargaConsumer.Object,
                scatoConsumer.Object,
                scatoRepositorioClient.Object,
                cnrtClient.Object,
                emailFasonService.Object,
                feriadoService.Object,
                mIUbicacionGeograficaService.Object
            );
        }

        [Test]
        [Ignore("")]
        public void Crear_OrdenTransporteExiste_EnEstadoGenerada()
        {
            var orden = ObtenerCrearRequest();
            var cuitTransporte = "";
            var mailUsuario = "mail@user.com";
            var productoId = 12;
            var producto = new Material { Id = productoId, CodigoSap = "8088", ValidaSisaRuca = false };
            orden.ProductoSeleccionado = new CrearOrdenDeCargaFasonRequestProducto
            {
                MaterialId = productoId,
                Descripcion = "",
                Codigo = "",
                CodigoSap = 8088,
                ValidaSisaRuca = false,
            };
            orden.Producto_Id = productoId;
            orden.UnidadesTransporte.First().CUITTransporte = cuitTransporte;
            var clientes = new ScatoConsumerWS.ClienteDto[] { };
            var usuario = new Usuario { Mail = mailUsuario, Roles = new List<Rol>() { new Rol { Nombre = "FLETE MOA", Codigo = "FLETE MOA" } } };
            ordenCargaConsumer
                .Setup(occ => occ.GetOrdenCargaControlEstadoTransportista(cuitTransporte))
                .Returns(ControlEstadoResEnum.TransportistaOK);
            scatoConsumer.Setup(sc => sc.ObtenerClientesPorCuit(It.IsAny<string>())).Returns(clientes);
            repositorioOrdenDeCargaFason.Setup(r => r.Obtener<Usuario>(us => us.Mail == mailUsuario)).Returns(usuario);
            repositorioOrdenDeCargaFason.Setup(r => r.Obtener<Material>(It.IsAny<int>())).Returns(producto);
            repositorioOrdenDeCargaFason.Setup(y => y.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(new Proveedor
            {
                Id = 1,
                CUIT = "12345678909"
            });
            scatoConsumer.Setup(r => r.BuscarDestinos(It.IsAny<string>())).Returns(new List<ScatoConsumerWS.KmPorProveedorDto> { new ScatoConsumerWS.KmPorProveedorDto { } });

            var result = service.Crear(orden, mailUsuario);

            Assert.That(result.Mensaje, Is.EqualTo(SuccessMsg.OrdenDeCargaAgregada));
        }

        [Test]
        public void VerificarVencimientoOrdenDeCargaFason_OrdenesVencidas_CorrectlyUpdated()
        {
            // Arrange
            var fechaCreacion = DateTime.Now.AddDays(-10);
            var ordenes = new List<OrdenDeCargaFason>
            {
                new OrdenDeCargaFason { Id = 1, Estado = EstadoOrdenDeCargaFason.Generada, FechaCreacion = fechaCreacion },
                new OrdenDeCargaFason { Id = 2, Estado = EstadoOrdenDeCargaFason.Pendiente, FechaCreacion = fechaCreacion }
            };
            var habilitacionJob = new HabilitacionJob { Nombre = "VencimientoOrdenesDeCargaFasonJob", Habilitado = true };

            repositorioOrdenDeCargaFason.Setup(r => r.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>())).Returns(habilitacionJob);
            repositorioOrdenDeCargaFason.Setup(r => r.Listar<OrdenDeCargaFason>(It.IsAny<Expression<Func<OrdenDeCargaFason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<OrdenDeCargaFason, object>>>>())).Returns(ordenes);
            feriadoService.Setup(a => a.ObtenerFeriados()).Returns(new List<DateTime> { new DateTime(1900, 01, 01) });

            // Act
            var result = service.VerificarVencimientoOrdenDeCargaFason();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(EstadoOrdenDeCargaFason.Vencida, result[0].Estado);
            Assert.AreEqual(EstadoOrdenDeCargaFason.Vencida, result[1].Estado);
            emailFasonService.Verify(e => e.EnviarMailVencieronOrdenesDeCarga(It.Is<List<OrdenDeCargaFason>>(o => o.Count == 2)), Times.Once);
            repositorioOrdenDeCargaFason.Verify(r => r.GuardarCambios(), Times.Once);
        }

        [Test]
        public void VerificarVencimientoOrdenDeCargaFason_NoHabilitacionJob_ReturnsEmptyList()
        {
            // Arrange
            repositorioOrdenDeCargaFason.Setup(r => r.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>())).Returns((HabilitacionJob)null);

            // Act
            var result = service.VerificarVencimientoOrdenDeCargaFason();

            // Assert
            Assert.IsEmpty(result);
            emailFasonService.Verify(e => e.EnviarMailVencieronOrdenesDeCarga(It.IsAny<List<OrdenDeCargaFason>>()), Times.Never);
            repositorioOrdenDeCargaFason.Verify(r => r.GuardarCambios(), Times.Never);
        }

        [Test]
        public void VerificarVencimientoOrdenDeCargaFason_NoOrdenesToVencidas_ReturnsEmptyList()
        {
            // Arrange
            var habilitacionJob = new HabilitacionJob { Nombre = "VencimientoOrdenesDeCargaFasonJob", Habilitado = true };
            var ordenes = new List<OrdenDeCargaFason>();

            repositorioOrdenDeCargaFason.Setup(r => r.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>())).Returns(habilitacionJob);
            repositorioOrdenDeCargaFason.Setup(r => r.Listar<OrdenDeCargaFason>(It.IsAny<Expression<Func<OrdenDeCargaFason, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<OrdenDeCargaFason, object>>>>())).Returns(ordenes);
            feriadoService.Setup(a => a.ObtenerFeriados()).Returns(new List<DateTime> { new DateTime(1900, 01, 01) });

            // Act
            var result = service.VerificarVencimientoOrdenDeCargaFason();

            // Assert
            Assert.IsEmpty(result);
            emailFasonService.Verify(e => e.EnviarMailVencieronOrdenesDeCarga(It.IsAny<List<OrdenDeCargaFason>>()), Times.Once);
            repositorioOrdenDeCargaFason.Verify(r => r.GuardarCambios(), Times.Never);
        }

        private CrearOrdenDeCargaFasonRequest ObtenerCrearRequest()
        {
            return new CrearOrdenDeCargaFasonRequest
            {
                Cantidad = 30000,
                CUITCliente = 22001100553,
                Producto_Id = 4,
                UnidadesTransporte = new List<UnidadTransporteCarga>
                {
                    new UnidadTransporteCarga
                    {
                        CantidadDeViajes = 3,
                        PatenteAcoplado = "",
                        PatenteChasis = ""
                    }
                }
            };
        }
    }
}
