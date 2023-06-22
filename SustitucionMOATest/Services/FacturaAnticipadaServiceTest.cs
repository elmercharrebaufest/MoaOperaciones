using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class FacturaAnticipadaServiceTest
    {
        private Mock<IRepositorio> _repositorio;
        private Mock<IOrdenCargaConsumerMOA> _consumerOrdenCarga;
        private Mock<IOrdenDeCargaEstadoService> _ordenDeCargaEstadoService;
        private IFacturaAnticipadaService _facturaAnticipadaService;
        [SetUp]
        public void Setup()
        {
            _consumerOrdenCarga = new Mock<IOrdenCargaConsumerMOA>();
            _repositorio = new Mock<IRepositorio>();
            _ordenDeCargaEstadoService = new Mock<IOrdenDeCargaEstadoService>();
            _facturaAnticipadaService =
                new FacturaAnticipadaService(_consumerOrdenCarga.Object, _repositorio.Object, _ordenDeCargaEstadoService.Object);

        }

        [Test]
        public void ObtenerFacturasDeContrato_NoEncuentraContrato_ThrowInfoCustomException()
        {
            _consumerOrdenCarga
                .Setup(c => c.ObtenerContratoSAP(It.IsAny<string>(), It.IsAny<TipoContratoFAS>()))
                .Returns(null as Result);

            Assert.That(
                () => _facturaAnticipadaService.ObtenerFacturasDeContrato(It.IsAny<string>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void ObtenerFacturasDeContrato_ListaFacturasVacias_DebeFiltrarlas()
        {
            _consumerOrdenCarga
                .Setup(c => c.ObtenerContratoSAP(It.IsAny<string>(), It.IsAny<TipoContratoFAS>()))
                .Returns(
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
                    });

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(It.IsAny<string>());

            var expected = new List<string> { "001246892", "001746592" };
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
