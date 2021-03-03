using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class TicketPesadaServiceTest
    {
        private TicketPesadaService target;
        private Mock<IScatoComandosConsumer> scatoComandosConsumerMock;

        [SetUp]
        public void SetUp()
        {
            scatoComandosConsumerMock = new Mock<IScatoComandosConsumer>();
            target = new TicketPesadaService(scatoComandosConsumerMock.Object);
        }


        [Test()]
        public void ObtenerTicketCPNoEncontrada()
        {
            var resultado = new ResultadoTickets
            {
            };

            var consulta = new ConsultaTicketPesada
            {

            };

            scatoComandosConsumerMock.Setup(s => s.ObtenerTicketPesada(It.IsAny<string>())).Throws(new Exception());

            var ex = Assert.Throws<ValidationCustomException>(() => target.ObtenerTicket(consulta));

            var expected = "No se encontró una CCPP con el número ingresado.";

            var result = ex.Message;

            Assert.AreEqual(expected, result);
        }


        [Test()]
        public void ObtenerTicketPatenteNoCoincide()
        {
            var resultado = new ResultadoTickets
            {
                CP = "1234",
                Patente = "XYZ987"
            };

            var consulta = new ConsultaTicketPesada
            {
                NumeroCartaPorte = "1234",
                PatenteCamion = "ABC123"
            };

            scatoComandosConsumerMock.Setup(s => s.ObtenerTicketPesada(It.IsAny<string>())).Returns(resultado);

            var ex = Assert.Throws<ValidationCustomException>(() => target.ObtenerTicket(consulta));

            var expected = "La patente del cambio no coincide con la patente del camión.";

            var result = ex.Message;

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void ObtenerTicketCompleto()
        {
            const string falsoString = "R0lGODlhAQABAIAAAAAAAAAAACH5BAAAAAAALAAAAAABAAEAAAICTAEAOw==";

            byte[] bytes = Convert.FromBase64String(falsoString);

            var resultadoTickets = new ResultadoTickets
            {
                CP = "1234",
                Patente = "ABC123",
                TicketPesada = bytes,
                TicketReciboMunicipal = bytes,
                FotoCP = new FotosDto
                {
                    Fotos = new FotoDto[]
                    {
                        new FotoDto { Foto = bytes },
                        new FotoDto { Foto = bytes },
                    }
                }
            };

            var consulta = new ConsultaTicketPesada
            {
                NumeroCartaPorte = "1234",
                PatenteCamion = "ABC123",
                Mail = "mail@mail.com"
            };

            scatoComandosConsumerMock.Setup(s => s.ObtenerTicketPesada(It.IsAny<string>())).Returns(resultadoTickets);

            var resultado = target.ObtenerTicket(consulta) ;

            var expected = 768;

            Assert.AreEqual(expected, resultado.Length);
        }
    }
}