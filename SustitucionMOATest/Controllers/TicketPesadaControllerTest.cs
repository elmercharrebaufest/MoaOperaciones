using Moq;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class TicketPesadaControllerTest
    {
        private TicketPesadaController target;
        private Mock<ITicketPesadaService> ticketPesadaServiceMock;

        [SetUp]
        public void SetUp()
        {
            ticketPesadaServiceMock = new Mock<ITicketPesadaService>();
            target = new TicketPesadaController(ticketPesadaServiceMock.Object);
        }

        [Test()]
        public void ObtenerTest()
        {
            var ticketPesadaJson = "{'NumeroCartaPorte':'12345678910','PatenteCamion':'ABC123','Mail':'mail@mail.com'}";

            var resultadoByte = new byte[] { 1, 2, 3 };

            ticketPesadaServiceMock
                    .Setup(s => s.ObtenerTicket(It.IsAny<ConsultaTicketPesada>()))
                    .Returns(resultadoByte);


            var result = target.Obtener(ticketPesadaJson);

            var expected = resultadoByte;

            Assert.AreEqual(result, expected);

        }
    }
}