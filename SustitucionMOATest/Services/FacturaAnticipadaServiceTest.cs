using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
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
                .Setup(c => c.ObtenerContratoSAP(It.IsAny<string>(), TipoContratoFAS.Anticipado))
                .Returns(null as Result);

            Assert.That(
                () => _facturaAnticipadaService.ObtenerFacturasDeContrato(It.IsAny<string>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void ObtenerFacturasDeContrato_ListaFacturas_DebeFiltrarLasVacias()
        {
            var factura1 = "001246892";
            var factura2 = "00124091232";
            var detail1 = new Detail { FacturaLegal = factura1 };
            var detail2 = new Detail { FacturaLegal = factura2 };
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                new Detail {FacturaLegal= string.Empty},
                new Detail {FacturaLegal= string.Empty},
                detail1,
                new Detail {FacturaLegal= string.Empty},
                detail2,
                new Detail {FacturaLegal= string.Empty},
            });

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(It.IsAny<string>());

            Assert.That(result[0].NumeroFactura, Is.EqualTo(factura1));
            Assert.That(result[1].NumeroFactura, Is.EqualTo(factura2));
        }
        [Test]
        public void ObtenerFacturasDeContrato_OrdenSinContratoSAP_DebeLlamarObtenerFacturasConContratoIngresado()
        {
            var numeroContrato = "12345678910";
            var factura1 = "001246892";
            var orden = new OrdenDeCarga { ContratoSAP = "", ContratoIngresado = numeroContrato };
            var detail = new Detail { FacturaLegal = factura1 };
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                detail,
                new Detail {FacturaLegal= string.Empty},
            }, numeroContrato);

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(orden);

            Assert.That(result[0].NumeroFactura, Is.EqualTo(factura1));
        }
        [Test]
        public void ObtenerFacturasDeContrato_OrdenConContratoSAP_DebeLlamarObtenerFacturasConContratoSAP()
        {
            var numeroContrato = "12345678910";
            var contratoSAP = "345682943";
            var factura1 = "001246892";
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                new Detail {FacturaLegal= factura1},
                new Detail {FacturaLegal= string.Empty},
            }, contratoSAP);
            var orden = new OrdenDeCarga { ContratoSAP = contratoSAP, ContratoIngresado = numeroContrato };

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(orden);

            Assert.That(result[0].NumeroFactura, Is.EqualTo(factura1));
        }
        [Test]
        public void OrdenConMultiplesFacturas_ListaUnaSolaFactura_RetursFalse()
        {
            var contratoSAP = "1234567810";
            var orden = new OrdenDeCarga { ContratoSAP = contratoSAP };
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                new Detail {FacturaLegal= "001246892"},
                new Detail {FacturaLegal= string.Empty},
            }, contratoSAP);

            var result = _facturaAnticipadaService.OrdenConMultiplesFacturas(orden);

            Assert.That(result, Is.False);
        }
        [Test]
        public void OrdenConMultiplesFacturas_ListaVariasFactura_RetursTrue()
        {
            var contratoSAP = "1234567810";
            var orden = new OrdenDeCarga { ContratoSAP = contratoSAP };
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                new Detail {FacturaLegal= "001246892"},
                new Detail {FacturaLegal= string.Empty},
                new Detail {FacturaLegal= "001246892"},
            }, contratoSAP);


            var result = _facturaAnticipadaService.OrdenConMultiplesFacturas(orden);

            Assert.That(result, Is.True);
        }
        [Test]
        public void SeleccionarFactura_NumeroFacturaNull_ThrowInvalidCustomException()
        {
            Assert.That(
                () => _facturaAnticipadaService.SeleccionarFactura(It.IsAny<int>(), null),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void SeleccionarFactura_OrdenNoExiste_ThrowInvalidCustomException()
        {
            _repositorio.Setup(r => r.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(null as OrdenDeCarga);
            Assert.That(
                () => _facturaAnticipadaService.SeleccionarFactura(It.IsAny<int>(), It.IsAny<string>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        [TestCase("1234", "0012314153")]
        [TestCase("56789", "001231412")]
        public void SeleccionarFactura_OrdenExisteYNumeroFacturaValido_CambiarNumeroFacturaYLlamaActualizarEstado(string numeroFacturaSeleccionada, string pedido)
        {
            var contratoSAP = "123";
            var orden = new OrdenDeCarga { ContratoSAP = contratoSAP };
            _repositorio.Setup(r => r.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);
            SetupRespuestaResult(new List<Detail> { new Detail { FacturaLegal = numeroFacturaSeleccionada, Pedido = pedido } }, contratoSAP);

            _facturaAnticipadaService.SeleccionarFactura(0, numeroFacturaSeleccionada);
            Assert.That(
                orden.NumeroFacturaSeleccionada,
                Is.EqualTo(numeroFacturaSeleccionada));
            Assert.That(
                orden.NumeroPedido,
                Is.EqualTo(pedido));
            _ordenDeCargaEstadoService.Verify(es => es.ActualizarEstado(orden), Times.Once());
        }
        [Test]
        [TestCase("1234")]
        [TestCase("56789")]
        public void SeleccionarFactura_OrdenExisteYNumeroNoValida_ThrowsInfoCustomException(string numeroFacturaSeleccionada)
        {
            var contratoSAP = "123";
            var orden = new OrdenDeCarga { ContratoSAP = contratoSAP };
            _repositorio.Setup(r => r.Obtener<OrdenDeCarga>(It.IsAny<int>())).Returns(orden);
            SetupRespuestaResult(new List<Detail> { new Detail { FacturaLegal = "diferente" } }, contratoSAP);

            Assert.That(() => _facturaAnticipadaService.SeleccionarFactura(0, numeroFacturaSeleccionada),
                Throws.TypeOf<InfoCustomException>());
        }
        private void SetupRespuestaResult(List<Detail> detalles, string numeroContrato = null)
        {
            _consumerOrdenCarga
              .Setup(c => c.ObtenerContratoSAP(numeroContrato ?? It.IsAny<string>(), TipoContratoFAS.Anticipado))
              .Returns(
                  new Result
                  {
                      Detalles = detalles
                  });
        }
    }
}
