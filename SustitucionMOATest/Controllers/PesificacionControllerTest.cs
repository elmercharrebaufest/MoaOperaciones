using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class PesificacionControllerTests
    {
        private PesificacionController target;
        private Mock<IPesificacionService> pesificacionServiceMock;
        private Mock<ILogPesificacionService> logPesificacionMock;
        private Mock<IUsuarioService> usuarioService;

        [SetUp]
        public void SetUp()
        {
            pesificacionServiceMock = new Mock<IPesificacionService>();
            logPesificacionMock = new Mock<ILogPesificacionService>();
            usuarioService = new Mock<IUsuarioService>();
            target = new PesificacionController(pesificacionServiceMock.Object, logPesificacionMock.Object, usuarioService.Object);
        }


        [Test()]
        public void GetFechaPesificacionTest()
        {
            Fecha fecha = new Fecha
            {
                DateTimeCorte = DateTime.Now,
                DateTimePesificacion = DateTime.Now,
                FechaPesificacion = "25/08/2021",
                HoraDeCorte = "12:12"
            };

            pesificacionServiceMock
                   .Setup(s => s.GetFechaPesificacion(It.IsAny<string>()))
                   .Returns(fecha);

            var result = target.GetFechaPesificacion();

            var resultJson = JsonConvert.SerializeObject(result.Data);

            var expectedJson = JsonConvert.SerializeObject(fecha);

            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void GetFechaPesificacionExceptionTest()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
                );

            pesificacionServiceMock
                   .Setup(s => s.GetFechaPesificacion(It.IsAny<string>()))
                   .Throws(new Exception());

            try
            {
                target.GetFechaPesificacion();
            }
            catch (Exception e)
            {
                var esperado1 = "Se produjo una excepción de tipo 'System.Exception'.";
                var esperado2 = "Exception of type 'System.Exception' was thrown.";
                Assert.Contains(e.Message, new[] { esperado1, esperado2 });

            }
        }

        [Test()]
        public void PesificacionesSAPTest()
        {
            List<PesificacionSapDto> pesificaciones = new List<PesificacionSapDto>
            {
                new PesificacionSapDto
                {
                    Contrato = "123",
                    FechaCarga = "25/08/2021",
                    FechaCargaDate = "25/08/2021",
                    FechaPesificacion = "25/08/2021",
                    FechaPesificacionDate = "25/08/2021",
                    Fijacion = "123",
                    Kilos = 100,
                    KilosString = "100",
                    Precio = 100,
                    PrecioString = "100",
                    //Ojala!
                    TipoCambio = "1"
                }
            };

            pesificacionServiceMock
                   .Setup(s => s.GetPesificacionesSAP(It.IsAny<string>()))
                   .Returns(pesificaciones);

            var result = target.PesificacionesSAP();

            var resultJson = JsonConvert.SerializeObject(result.Data);

            var expectedJson = JsonConvert.SerializeObject(new { data = pesificaciones });

            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void PesificacionesSAPTestInfoCustomException()
        {
            pesificacionServiceMock
                .Setup(s => s.GetPesificacionesSAP(It.IsAny<string>()))
                .Throws(new InfoCustomException("Mensaje de error"));

            try
            {
                target.PesificacionesSAP();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }

        [Test()]
        public void PesificacionesSAPTestValidationCustomException()
        {
            pesificacionServiceMock
                .Setup(s => s.GetPesificacionesSAP(It.IsAny<string>()))
                .Throws(new ValidationCustomException("Mensaje de error"));
            try
            {
                target.PesificacionesSAP();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }

        [Test()]
        public void PesificacionesSAPTestException()
        {
            HttpContext.Current = new HttpContext(
               new HttpRequest("", "http://tempuri.org", ""),
               new HttpResponse(new StringWriter())
               );

            pesificacionServiceMock
                .Setup(s => s.GetPesificacionesSAP(It.IsAny<string>()))
                .Throws(new Exception(""));

            try
            {
                target.PesificacionesSAP();
            }
            catch (Exception e)
            {
                Assert.AreEqual("", e.Message);
            }
        }
    }
}