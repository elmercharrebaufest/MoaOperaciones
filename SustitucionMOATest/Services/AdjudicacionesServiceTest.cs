using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class AdjudicacionesServiceTest
    {
        AdjudicacionesService target;
        private Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA> obtenerOrdenesDeCompraParaSOLPConsumerMOAMock;
        private Mock<ITablaSapService> tablaSapServiceMock;
        private Mock<ISolpService> solpServiceMock;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            obtenerOrdenesDeCompraParaSOLPConsumerMOAMock = new Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA>();
            tablaSapServiceMock = new Mock<ITablaSapService>();
            solpServiceMock = new Mock<ISolpService>();
            repositorioMock = new Mock<IRepositorio>();


            target = new AdjudicacionesService(obtenerOrdenesDeCompraParaSOLPConsumerMOAMock.Object,
                                               tablaSapServiceMock.Object,
                                               solpServiceMock.Object,
                                               repositorioMock.Object);
        }

        [Test]
        public void ListarAdjudicaciones()
        {
            const int adjudicacionId = 1;
            const string nroSolp = "0212303121";
            solpServiceMock.Setup(y => y.ObtenerNumeroSolp(1))
            .Returns(nroSolp);
            tablaSapServiceMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "1", Id = 1 } });
            obtenerOrdenesDeCompraParaSOLPConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<OrdenDeCompraSAPDto> {
                new OrdenDeCompraSAPDto { Cabecera = new OrdenDeCompraSAPCabecera { Tipo = "", OrdenDeCompra = "", FechaCreacion = new DateTime(), RazonSocialProveedor = "Proveedor", Moneda = "ARP", MontoTotal = 1500 } }
            });
            var result = target.ListarAdjudicaciones(adjudicacionId);
            solpServiceMock.Verify(y => y.ObtenerNumeroSolp(It.IsAny<int>()), Times.Once);
            solpServiceMock.Verify(y => y.ObtenerNumeroSolp(1), Times.Once);
        }

        [Test]
        public void ObtenerAdjudicacion()
        {
            const int adjudicacionId = 1;
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<Expression<Func<Adjudicacion, AdjudicacionDto>>>()))
            .Returns(new AdjudicacionDto { });
            var result = target.ObtenerAdjudicacion(adjudicacionId);
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<Expression<Func<Adjudicacion, AdjudicacionDto>>>()), Times.Once);
        }
    }
}
