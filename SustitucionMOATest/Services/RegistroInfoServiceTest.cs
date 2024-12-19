// Ignore Spelling: Sustitucion

using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class RegistroInfoServiceTest
    {
        private RegistroInfoService target;
        private Mock<IObtenerRegistroInfoConsumerMOA> obtenerRegistroInfoConsumerMOAMock;

        [SetUp]
        public void Setup()
        {
            obtenerRegistroInfoConsumerMOAMock = new Mock<IObtenerRegistroInfoConsumerMOA>();

            target = new RegistroInfoService(obtenerRegistroInfoConsumerMOAMock.Object);
        }

        [Test]
        public void AutocompleteMaterialRFCOk()
        {
            RegistroInfoDto registroInfo = new RegistroInfoDto { Cantidad = 5, Moneda = "USDM", Centro = "1029", GrupoDeCompras = "" };
            obtenerRegistroInfoConsumerMOAMock.Setup(y => y.ObtenerRegistroInfoConsumer(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<RegistroInfoDto> { registroInfo });
            var result = target.ObtenerUltimoRegistroPorMaterialYProveedor("codigoMaterial", "codigoCentro", "codigoGrupoDeCompras");
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(registroInfo.GetType(), result.GetType());
        }
    }
}
