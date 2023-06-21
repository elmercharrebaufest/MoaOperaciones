using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class FacturaAnticipadaServiceTest
    {
        private Mock<IOrdenCargaConsumerMOA> _consumerOrdenCargaFAS;
        private IFacturaAnticipadaService _facturaAnticipadaService;
        [SetUp]
        public void Setup()
        {
            _consumerOrdenCargaFAS = new Mock<IOrdenCargaConsumerMOA>();
            _facturaAnticipadaService = new FacturaAnticipadaService(_consumerOrdenCargaFAS.Object);

        }

        [Test]
        public void ObtenerFacturasDeContrato_NoEncuentraContrato_ThrowInfoCustomException()
        {
            _consumerOrdenCargaFAS
                .Setup(c => c.OrdenCargaVisualizarClienteExecute(It.IsAny<OrdenCargaVisualizarClienteWSMOARequest>()))
                .Returns(new OrdenCargaVisualizarClienteWSMOAResponse { Resultados = new List<Result>() });

            Assert.That(
                () => _facturaAnticipadaService.ObtenerFacturasDeContrato(It.IsAny<string>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void ObtenerFacturasDeContrato_ListaFacturasVacias_DebeFiltrarlas()
        {
            _consumerOrdenCargaFAS
                .Setup(c => c.OrdenCargaVisualizarClienteExecute(It.IsAny<OrdenCargaVisualizarClienteWSMOARequest>()))
                .Returns(new OrdenCargaVisualizarClienteWSMOAResponse
                {
                    Resultados = new List<Result> {
                    new Result
                    {
                        Detalles = new List<Detail>
                        {
                            new Detail {FacturaLegal= string.Empty},
                            new Detail {FacturaLegal= string.Empty},
                            new Detail {FacturaLegal= string.Empty},
                            new Detail {FacturaLegal= "001246892"},
                            new Detail {FacturaLegal= string.Empty},
                            new Detail {FacturaLegal= "001746592"},
                            new Detail {FacturaLegal= string.Empty},
                        }
                    }
                }
                });

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(It.IsAny<string>());

            var expected = new List<string> { "001246892", "001746592" };
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
