using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [Test]
        public void SetComprobantes_CasoExitoso_ProcesaContratosYRetornaJson()
        {
            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(f => f.FileName).Returns("contratos.csv");
            fileMock.Setup(f => f.ContentLength).Returns(100);

            var contratosDesdeCsv = new List<ContratoContenido>
            {
                new ContratoContenido { Contrato = "100", Fijacion = "1", Cantidad = 50, Correo = "test@test.com" },
                new ContratoContenido { Contrato = "200", Fijacion = "2", Cantidad = 70, Correo = "test@test.com" }
            };
            pesificacionServiceMock.Setup(s => s.LeerContratosCSV(It.IsAny<HttpPostedFileBase>())).Returns(contratosDesdeCsv);

            usuarioService.Setup(s => s.GetUsuario(It.IsAny<string>())).Returns(new UsuarioDto { Id = 1 });

            var logsGuardados = new List<LogPesificacionDto>
            {
                new LogPesificacionDto { Id = 1, Contrato = 100, Fijacion = 1 },
                new LogPesificacionDto { Id = 2, Contrato = 200, Fijacion = 2 }
            };
            logPesificacionMock.Setup(s => s.GuardarPesificaciones(It.IsAny<List<LogPesificacion>>())).Returns(logsGuardados);

            var respuestaServicio = new PesificacionSetContratosWSMOAResponse
            {
                ContratosOk = new List<string> { "100-1" },
                Log = new List<Item> { new Item { Mensaje = "Error en contrato 200" } }
            };
            pesificacionServiceMock.Setup(s => s.SetContratos(It.IsAny<string>(), It.IsAny<List<ContratoContenido>>())).Returns(respuestaServicio);

            var result = target.SetComprobantes(fileMock.Object) as JsonResult;

            Assert.IsNotNull(result, "El resultado no debe ser nulo.");

            var data = result.Data as PesificacionSetContratosWSMOAResponse;
            Assert.IsNotNull(data, "Los datos del JsonResult no deben ser nulos.");
            Assert.AreEqual(1, data.ContratosOk.Count, "Debería haber 1 contrato exitoso.");
            Assert.AreEqual("100-1", data.ContratosOk.First());
            Assert.AreEqual(1, data.Log.Count, "Debería haber 1 mensaje de error en el log.");

            pesificacionServiceMock.Verify(s => s.LeerContratosCSV(fileMock.Object), Times.Once);
            logPesificacionMock.Verify(s => s.GuardarPesificaciones(It.Is<List<LogPesificacion>>(l => l.Count == 2)), Times.Once);
            pesificacionServiceMock.Verify(s => s.SetContratos(It.IsAny<string>(), contratosDesdeCsv), Times.Once);
            logPesificacionMock.Verify(s => s.ActualizarEstadoLogPesificacion(It.Is<List<int>>(ids => ids.Count == 1 && ids.Contains(1))), Times.Once);
        }

        [Test]
        public void SetComprobantes_ArchivoNulo_LanzaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => target.SetComprobantes(null));
        }

        [Test]
        public void SetComprobantes_ExtensionIncorrecta_LanzaArgumentException()
        {
            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(f => f.FileName).Returns("documento.txt");
            fileMock.Setup(f => f.ContentLength).Returns(10);

            var ex = Assert.Throws<ArgumentException>(() => target.SetComprobantes(fileMock.Object));
            Assert.AreEqual("El archivo debe ser de tipo .csv", ex.Message);
        }

        [Test]
        public void SetComprobantes_ArchivoVacio_LanzaValidationCustomException()
        {
            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(f => f.FileName).Returns("vacios.csv");
            fileMock.Setup(f => f.ContentLength).Returns(10);
            pesificacionServiceMock.Setup(s => s.LeerContratosCSV(It.IsAny<HttpPostedFileBase>())).Returns(new List<ContratoContenido>());

            var ex = Assert.Throws<ValidationCustomException>(() => target.SetComprobantes(fileMock.Object));
            Assert.AreEqual("El archivo no contiene contratos válidos.", ex.Message);
        }
    }
}