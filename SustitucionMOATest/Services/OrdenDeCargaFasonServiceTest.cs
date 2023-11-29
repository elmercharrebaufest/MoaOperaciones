using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;
using ScatoConsumerWS = SustitucionMOAWS.ScatoWebService;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class OrdenDeCargaFasonServiceTest
    {
        private IOrdenDeCargaFasonService service;
        private Mock<IRepositorio> repositorio;
        private Mock<IOrdenCargaConsumerMOA> ordenCargaConsumer;
        private Mock<IScatoConsumer> scatoConsumer;
        private Mock<IScatoRepositorioClient> scatoRepositorioClient;
        private Mock<ICNRTClient> cnrtClient;
        private Mock<IEmailFasonService> emailFasonService;
        [SetUp]
        public void Setup()
        {
            repositorio = new Mock<IRepositorio>();
            ordenCargaConsumer = new Mock<IOrdenCargaConsumerMOA>();
            scatoConsumer = new Mock<IScatoConsumer>();
            scatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            cnrtClient = new Mock<ICNRTClient>();
            emailFasonService = new Mock<IEmailFasonService>();
            service = new OrdenDeCargaFasonService(
                repositorio.Object,
                ordenCargaConsumer.Object,
                scatoConsumer.Object,
                scatoRepositorioClient.Object,
                cnrtClient.Object,
                emailFasonService.Object
            );
        }

        [Test]
        public void Crear_OrdenTransporteExiste_EnEstadoGenerada()
        {
            var orden = ObtenerCrearRequest();
            var cuitTransporte = "";
            var mailUsuario = "mail@user.com";
            var productoId = 12;
            var producto = new Material { Id= productoId, CodigoSap= "8088", ValidaSisaRuca= false };
            orden.ProductoSeleccionado = new CrearOrdenDeCargaFasonRequestProducto {
                MaterialId = productoId,
                Descripcion="",
                Codigo="",
                CodigoSap = 8088,
                ValidaSisaRuca=false,
            };
            orden.Destino = new CrearOrdenDeCargaFasonRequestDestino { LocalidadId=12, LocalidadDescripcion="Locale" };
            orden.Producto_Id = productoId;
            orden.CUITTransporte = cuitTransporte;
            var clientes = new ScatoConsumerWS.ClienteDto[] { };
            var usuario = new Usuario { Mail= mailUsuario, Roles = new List<Rol>() { new Rol { Nombre = "FLETE MOA", Codigo="FLETE MOA" } } };
            ordenCargaConsumer
                .Setup(occ => occ.GetOrdenCargaControlEstadoTransportista(cuitTransporte))
                .Returns(ControlEstadoResEnum.TransportistaOK);
            scatoConsumer.Setup(sc => sc.ObtenerClientesPorCuit(It.IsAny<string>())).Returns(clientes);
            repositorio.Setup(r=>r.Obtener<Usuario>(us=>us.Mail== mailUsuario)).Returns(usuario);
            repositorio.Setup(r => r.Obtener<Material>(It.IsAny<int>())).Returns(producto);

            var result = service.Crear(orden, mailUsuario);

            Assert.That(result.Mensaje, Is.EqualTo(SuccessMsg.OrdenDeCargaAgregada));
        }

        private CrearOrdenDeCargaFasonRequest ObtenerCrearRequest()
        {
            return new CrearOrdenDeCargaFasonRequest
            {
                Cantidad = 30000,
                CantidadDeViajes = 3,
                CUITCliente = 22001100553,
                Producto_Id = 4,
                PatenteAcoplado = "",
                PatenteChasis = "",
            };
        }
    }
}
