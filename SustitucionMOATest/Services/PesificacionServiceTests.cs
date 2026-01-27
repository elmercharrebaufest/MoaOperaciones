using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class PesificacionServiceTests
    {

        private IPesificacionService target;
        private Mock<IListarPesificacionesConsumer> pesificacionConsumerMock;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IDataAgroService> dataAgroService;


        [SetUp]
        public void SetUp()
        {
            pesificacionConsumerMock = new Mock<IListarPesificacionesConsumer>();
            repositorioMock = new Mock<IRepositorio>();
            target = new PesificacionService(pesificacionConsumerMock.Object, repositorioMock.Object, dataAgroService.Object);
        }

        [Test()]
        public void GetPesificacionesSAPTest()
        {
            List<PesificacionSapDto> pesificaciones = new List<PesificacionSapDto>
            {
                new PesificacionSapDto
                {
                    Contrato = "123",
                    FechaCarga = "25/08/2021",
                    FechaCargaDate = "25/08/2021",
                    FechaPesificacion = "25/08/2021",
                    FechaPesificacionDate = "25/08/2021",
                    Fijacion = "123",
                    Kilos = 100,
                    KilosString = "100",
                    Precio = 100,
                    PrecioString = "100",
                    //Ojala!
                    TipoCambio = "1"
                }
            };

            ListarPesificacionesWSMOAResponse response = new ListarPesificacionesWSMOAResponse { Pesificaciones = pesificaciones };

            pesificacionConsumerMock
                .Setup(s => s.Request(It.IsAny<string>()))
                .Returns(response);

            var result = target.GetPesificacionesSAP("C00000001");

            Assert.AreEqual(pesificaciones, response.Pesificaciones);
        }

        [Test()]
        public void GetPesificacionesSAPTestProveedorNull()
        {
            var ex = Assert.Throws<ValidationCustomException>(() => target.GetPesificacionesSAP(null));

            var expected = "Debe ingresar un proveedor";

            var result = ex.Message;

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void GetPesificacionesSAPTestListaVacia()
        {
            List<PesificacionSapDto> pesificaciones = new List<PesificacionSapDto>();

            ListarPesificacionesWSMOAResponse response = null;

            pesificacionConsumerMock
                .Setup(s => s.Request(It.IsAny<string>()))
                .Returns(response);

            var ex = Assert.Throws<InfoCustomException>(() => target.GetPesificacionesSAP("C00000001"));

            var result = ex.Message;

            var expected = "No se encontraron pesificaciones";

            Assert.AreEqual(expected, result);
        }
    }
}