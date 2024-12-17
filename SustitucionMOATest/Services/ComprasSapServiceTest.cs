using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasSapServiceTest
    {
        private ComprasSapService target;
        private Mock<IObtenerCecoSolpConsumerMOA> cecoConsumerMock;
        private Mock<IObtenerSolpConsumerMOA> solpConsumerMock;

        [SetUp]
        public void Setup()
        {
            cecoConsumerMock = new Mock<IObtenerCecoSolpConsumerMOA>();
            solpConsumerMock = new Mock<IObtenerSolpConsumerMOA>();

            target = new ComprasSapService(cecoConsumerMock.Object,
                                           solpConsumerMock.Object);
        }

        [Test()]
        public void ObtenerCecoSapTest()
        {
            var rfcResultMock = new CecoWSMOAResponse()
            {
                Cecos = new List<Ceco>()
                {
                    new Ceco() { CostCenter = "MOA", CO_A = "MOA", Descripcion = "MOA"}
                }
            };

            cecoConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CecoSolpSap}
            };

            var result = target.ObtenerCecoSap();

            Assert.AreEqual(expected.Count, result.Count);
        }
    }
}
