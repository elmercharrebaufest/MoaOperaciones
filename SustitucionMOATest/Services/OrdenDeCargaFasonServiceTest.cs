using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;

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
            orden.CUITTransporte = cuitTransporte;
            ordenCargaConsumer
                .Setup(occ => occ.GetOrdenCargaControlEstadoTransportista(cuitTransporte))
                .Returns(ControlEstadoResEnum.TransportistaOK);
            var result = service.Crear(orden, "");
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
