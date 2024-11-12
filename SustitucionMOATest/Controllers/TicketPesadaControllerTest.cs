using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

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

            List<ArchivoDescargaDto> listado = new List<ArchivoDescargaDto>()
            {
                new ArchivoDescargaDto { Nombre = "Ticket Pesada CCPP 12345678910.zip",  Datos = resultadoByte}
            };

            ticketPesadaServiceMock
                    .Setup(s => s.ObtenerTicket(It.IsAny<ConsultaTicketPesada>()))
                    .Returns(listado);


            var resultado = target.Obtener(ticketPesadaJson);

            var resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = listado };

            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void ObtenerTestValidationCustomException()
        {
            var ticketPesadaJson = "{'NumeroCartaPorte':'12345678910','PatenteCamion':'ABC123','Mail':'mail@mail.com'}";

            ticketPesadaServiceMock.Setup(s => s.ObtenerTicket(It.IsAny<ConsultaTicketPesada>())).Throws(new ValidationCustomException("Mensaje de error"));

            try
            {
                target.Obtener(ticketPesadaJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }

        [Test()]
        public void ObtenerTestInfoCustomException()
        {
            var ticketPesadaJson = "{'NumeroCartaPorte':'12345678910','PatenteCamion':'ABC123','Mail':'mail@mail.com'}";

            ticketPesadaServiceMock.Setup(s => s.ObtenerTicket(It.IsAny<ConsultaTicketPesada>())).Throws(new InfoCustomException("Mensaje de error"));

            try
            {
                target.Obtener(ticketPesadaJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }

        [Test()]
        public void ObtenerTestException()
        {
            var ticketPesadaJson = "{'NumeroCartaPorte':'12345678910','PatenteCamion':'ABC123','Mail':'mail@mail.com'}";

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
                );

            ticketPesadaServiceMock.Setup(s => s.ObtenerTicket(It.IsAny<ConsultaTicketPesada>())).Throws(new Exception());


            var expected = "Se produjo una excepción de tipo 'System.Exception'.";
            try
            {
                target.Obtener(ticketPesadaJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual(expected, e.Message);
            }
        }
    }
}