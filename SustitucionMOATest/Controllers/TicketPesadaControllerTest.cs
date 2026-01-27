using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class TicketPesadaControllerTest
    {
        private TicketPesadaController target;
        private Mock<ITicketPesadaService> ticketPesadaServiceMock;
        private Mock<IScatoConsumer> scatoConsumerMock;
        [SetUp]
        public void SetUp()
        {
            ticketPesadaServiceMock = new Mock<ITicketPesadaService>();
            scatoConsumerMock = new Mock<IScatoConsumer>();
            target = new TicketPesadaController(ticketPesadaServiceMock.Object, scatoConsumerMock.Object);
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

            try
            {
                target.Obtener(ticketPesadaJson);
            }
            catch (Exception e)
            {
                var esperado1 = "Se produjo una excepción de tipo 'System.Exception'.";
                var esperado2 = "Exception of type 'System.Exception' was thrown.";
                Assert.Contains(e.Message, new[] { esperado1, esperado2 });
            }
        }

        [Test()]
        public void ListarDatosTicketPesadaTest()
        {
            // Arrange
            var fechaInicio = new DateTime(2023, 1, 1);
            var fechaEgreso = new DateTime(2023, 1, 31);
            var cuitProveedor = "20304050607";
            var cuitTransportista = "20304050608";
            var ctg = "123456789";
            var patente = "ABC123";
            var cuitIntermediarioFlete = "20304050609";
            var esAdmin = true;

            var datosTicketPesada = new SustitucionMOAWS.ScatoWebService.TicketPesadaDto[]
            {
        new SustitucionMOAWS.ScatoWebService.TicketPesadaDto
        {
            CTG = "123456789",
            ChoferNombreApellido = "Juan Pérez",
            FechaHoraEgreso = new DateTime(2023, 1, 15, 14, 0, 0),
            FechaHoraIngreso = new DateTime(2023, 1, 15, 8, 0, 0),
            Intermediario = "Intermediario S.A.",
            IntermediarioCUIT = "20304050610",
            Material = "Soja",
            Patente = "ABC123",
            BrutoOrigen = 30000,
            NetoOrigen = 29000,
            TaraOrigen = 1000,
            BrutoPlanta = 31000,
            NetoPlanta = 30000,
            TaraPlanta = 1000,
            TitularCPCUIT = "20304050611",
            Procedencia = "Campo A",
            Transportista = "Transportes S.A.",
            TransportistaCUIT = "20304050612"
        }
            };

            scatoConsumerMock
                .Setup(s => s.ObtenerDatosTicketPesada(fechaInicio, fechaEgreso, cuitProveedor, cuitTransportista, ctg, patente, cuitIntermediarioFlete, esAdmin))
                .Returns(datosTicketPesada);

            // Act
            var resultado = target.ListarDatosTicketPesada(fechaInicio, fechaEgreso, cuitProveedor, cuitTransportista, ctg, patente, cuitIntermediarioFlete, esAdmin);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsInstanceOf<JsonResult>(resultado);

            var jsonResult = JsonConvert.SerializeObject(resultado.Data);
            var deserializedResult = JsonConvert.DeserializeObject<dynamic>(jsonResult);

            Assert.IsNotNull(deserializedResult.data);

            var listado = JsonConvert.DeserializeObject<List<TicketPesadaDto>>(deserializedResult.data.ToString());
            Assert.IsNotNull(listado);
            Assert.AreEqual(1, listado.Count);
            Assert.AreEqual("123456789", listado[0].CTG);
            Assert.AreEqual("Juan Pérez", listado[0].ChoferNombreApellido);
            Assert.AreEqual("Soja", listado[0].Material);
        }

    }
}