using Moq;
using NUnit.Framework;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.WSConsumers;
using System;

namespace SustitucionMOATest.Consumers
{
    [TestFixture]
    public class ScatoConsumerTests
    {
        private Mock<IServicioRepositorioChannel> _mServicioRepositorioClient;
        private IScatoConsumer _consumer;
        [SetUp]
        public void Setup()
        {
            _mServicioRepositorioClient = new Mock<IServicioRepositorioChannel>();
            _consumer = new ScatoConsumer(_mServicioRepositorioClient.Object);
        }

        [Test]
        public void CuilChoferExiste_CuilExiste_ReturnsTrue()
        {
            _mServicioRepositorioClient.Setup(spc => spc.ObtenerChoferPorCuit(It.IsAny<string>())).Returns(
                new ChoferDto()
                );
            var result = _consumer.CuilChoferExiste("11111111111");
            Assert.That(result, Is.True);
        }


        [Test]
        public void CuilChoferExiste_CuilNoExiste_ReturnsFalse()
        {

            _mServicioRepositorioClient.Setup(spc => spc.ObtenerChoferPorCuit(It.IsAny<string>())).Returns(
                null as ChoferDto
                );
            var result = _consumer.CuilChoferExiste("11111111111");
            Assert.That(result, Is.False);
        }
    }
}
