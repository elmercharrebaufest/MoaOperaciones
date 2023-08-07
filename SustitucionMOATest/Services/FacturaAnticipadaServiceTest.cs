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
using System.Linq;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class FacturaAnticipadaServiceTest
    {
        private Mock<IRepositorio> _repositorio;
        private Mock<IOrdenCargaConsumerMOA> _consumerOrdenCarga;
        private Mock<IOrdenDeCargaEstadoService> _ordenDeCargaEstadoService;
        private Mock<IKgDisponiblesFasService> _kgDisponiblesFasService;
        private IFacturaAnticipadaService _facturaAnticipadaService;
        [SetUp]
        public void Setup()
        {
            _consumerOrdenCarga = new Mock<IOrdenCargaConsumerMOA>();
            _repositorio = new Mock<IRepositorio>();
            _ordenDeCargaEstadoService = new Mock<IOrdenDeCargaEstadoService>();
            _kgDisponiblesFasService = new Mock<IKgDisponiblesFasService>();
            _facturaAnticipadaService =
                new FacturaAnticipadaService(
                    _consumerOrdenCarga.Object,
                    _repositorio.Object,
                    _ordenDeCargaEstadoService.Object,
                    _kgDisponiblesFasService.Object
                    );
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
            var pedido1 = "00124378";
            var pedido2 = "78431200";
            var detail1 = new Detail { FacturaLegal = factura1, Pedido = pedido1 };
            var detail3 = new Detail { Pedido = pedido1 };
            var detail1Vacio = new Detail { FacturaLegal = string.Empty, Pedido = pedido1 };

            var detail2 = new Detail { FacturaLegal = factura2, Pedido = pedido2 };
            var detail4 = new Detail { Pedido = pedido2 };
            var detail2Vacio = new Detail { FacturaLegal = string.Empty, Pedido = pedido2 };

            var list1 = new List<Detail> { detail1, detail1Vacio, detail3 };
            var list2 = new List<Detail> { detail2, detail4, detail2Vacio };
            SetupRespuestaResult(list1.Concat(list2).ToList());

            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                list1
            )).Returns(detail1);
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                list2
            )).Returns(detail2);

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
            var detail = new Detail { FacturaLegal = factura1, Pedido = "123456" };
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                detail,
                new Detail {FacturaLegal= string.Empty},
            }, numeroContrato);
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                new List<Detail> { detail }
            )).Returns(detail);

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(orden);

            Assert.That(result[0].NumeroFactura, Is.EqualTo(factura1));
        }
        [Test]
        public void ObtenerFacturasDeContrato_OrdenConContratoSAP_DebeLlamarObtenerFacturasConContratoSAP()
        {
            var numeroContrato = "12345678910";
            var contratoSAP = "345682943";
            var factura = "001246892";
            var pedido = "98765";
            var detail = new Detail { FacturaLegal = factura, Pedido = pedido };
            SetupRespuestaResult(new List<Detail>
            {
                new Detail {FacturaLegal= string.Empty},
                detail,
                new Detail {FacturaLegal= string.Empty},
            }, contratoSAP);
            var orden = new OrdenDeCarga { ContratoSAP = contratoSAP, ContratoIngresado = numeroContrato };
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                new List<Detail> { detail }
            )).Returns(detail);

            var result = _facturaAnticipadaService.ObtenerFacturasDeContrato(orden);

            Assert.That(result[0].NumeroFactura, Is.EqualTo(factura));
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
            var factura1 = "001246892";
            var factura2 = "654321";
            var pedido1 = "123456";
            var pedido2 = "8712456";
            var detail1 = new Detail { FacturaLegal = factura1, Pedido = pedido1 };
            var detail2 = new Detail { FacturaLegal = factura2, Pedido = pedido2 };
            SetupRespuestaResult(new List<Detail>
            {
                detail1,
                new Detail {FacturaLegal= string.Empty},
                detail2,
                new Detail {FacturaLegal= string.Empty},

            }, contratoSAP);
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                new List<Detail> { detail2 }
            )).Returns(detail1);
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                new List<Detail> { detail1 }
            )).Returns(detail2);

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
            var detail = new Detail { FacturaLegal = numeroFacturaSeleccionada, Pedido = pedido };
            var list = new List<Detail> { detail };
            SetupRespuestaResult(list, contratoSAP);
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                list
            )).Returns(detail);

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
            var detail = new Detail { FacturaLegal = "diferente", Pedido = "1234123" };
            var listReal = new List<Detail> { detail };
            SetupRespuestaResult(listReal, contratoSAP);
            _kgDisponiblesFasService.Setup(kgs => kgs.AuxObtenerDetallePedidoPrincipal(
                listReal
            )).Returns(detail);

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
        private Detail GetDetail(string numeroFactura, string numeroPedido)
        {
            return new Detail { NombreDestinatario = numeroFactura, Pedido = numeroPedido };
        }
    }
}
