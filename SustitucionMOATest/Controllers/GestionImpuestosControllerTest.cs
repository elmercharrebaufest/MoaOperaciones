using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class GestionImpuestosControllerTest
    {
        GestionImpuestosController target;

        Mock<IGestionImpuestosService> gestionImpuestosServiceMock;
        Mock<IConsultaService> consultaServiceMock;

        [SetUp]
        public void SetUp()
        {
            this.gestionImpuestosServiceMock = new Mock<IGestionImpuestosService>();
            this.consultaServiceMock = new Mock<IConsultaService>();

            this.target = new GestionImpuestosController(gestionImpuestosServiceMock.Object, consultaServiceMock.Object);
        }

        [Test]
        public void ListarCabecerasOk()
        {
            var ingresosBrutosCoeficienteUnificadoDtoList = new List<IngresosBrutosCoeficienteUnificadoDto>
            {
                new  IngresosBrutosCoeficienteUnificadoDto { Id = 1, },
                new  IngresosBrutosCoeficienteUnificadoDto { Id = 2, },
                new  IngresosBrutosCoeficienteUnificadoDto { Id = 3, },
            };

            this.gestionImpuestosServiceMock.Setup(g => g.ListarCabeceras()).Returns(ingresosBrutosCoeficienteUnificadoDtoList);

            var result = target.ListarCabeceras();

            Assert.IsInstanceOf<List<IngresosBrutosCoeficienteUnificadoDto>>(result.Data);

            List<IngresosBrutosCoeficienteUnificadoDto> resultData = (List<IngresosBrutosCoeficienteUnificadoDto>)result.Data;
            Assert.AreEqual(3, resultData.Count);

            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDtoList[0], resultData[0]);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDtoList[1], resultData[1]);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDtoList[2], resultData[2]);

            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarCabecerasInfoCustomException()
        {
            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarCabeceras())
                .Throws(excepcionTest);

            try
            {
                target.ListarCabeceras();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarCabecerasValidationCustomException()
        {
            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarCabeceras())
                .Throws(excepcionTest);

            try
            {
                target.ListarCabeceras();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarCabecerasException()
        {
            var excepcionTest = new NullReferenceException("exploto molinos");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarCabeceras())
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.ListarCabeceras();
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarDetallesOk()
        {
            int idCabeceraTest = 3;

            var ingresosBrutosCoeficienteUnificadoDetalleDtoList = new List<IngresosBrutosCoeficienteUnificadoDetalleDto>
            {
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 1, IdCabecera = 1, },
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 2, IdCabecera = 2, },
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 3, IdCabecera = 3, },
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 4, IdCabecera = 3, },
            };

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(It.IsAny<int>()))
                .Returns<int>(idCabecera => ingresosBrutosCoeficienteUnificadoDetalleDtoList.Where(x => x.IdCabecera == idCabecera).ToList());

            var result = target.ListarDetalles(idCabeceraTest);

            Assert.IsInstanceOf<List<IngresosBrutosCoeficienteUnificadoDetalleDto>>(result.Data);

            List<IngresosBrutosCoeficienteUnificadoDetalleDto> resultData = (List<IngresosBrutosCoeficienteUnificadoDetalleDto>)result.Data;
            Assert.AreEqual(2, resultData.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetalleDtoList[2], resultData[0]);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetalleDtoList[3], resultData[1]);

            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ListarDetallesInfoCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(idCabeceraTest))
                .Throws(excepcionTest);

            try
            {
                target.ListarDetalles(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(idCabeceraTest), Times.Once);
        }

        [Test]
        public void ListarDetallesValidationCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(idCabeceraTest))
                .Throws(excepcionTest);

            try
            {
                target.ListarDetalles(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(idCabeceraTest), Times.Once);
        }

        [Test]
        public void ListarDetallesException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new NullReferenceException("exploto molinos");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(1))
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.ListarDetalles(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(It.IsAny<int>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(1), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraOk()
        {
            int idCabeceraTest = 3;
            string mailUsuarioTest = "mail";

            var ingresosBrutosCoeficienteUnificadoDetalleDtoList = new List<IngresosBrutosCoeficienteUnificadoDetalleDto>();

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
                .Returns("Se autorizo ok");

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim("userName", mailUsuarioTest) });
            ClaimsPrincipal.Current.AddIdentity(claimsIdentity);

            JsonResult result = target.AutorizarCabecera(idCabeceraTest);

            Assert.AreEqual("Se autorizo ok", result.Data);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraInfoCustomException()
        {
            int idCabeceraTest = 1;
            string mailUsuarioTest = "mail";

            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim("userName", mailUsuarioTest) });
            ClaimsPrincipal.Current.AddIdentity(claimsIdentity);

            try
            {
                target.AutorizarCabecera(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraValidationCustomException()
        {
            int idCabeceraTest = 1;
            string mailUsuarioTest = "mail";

            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim("userName", mailUsuarioTest) });
            ClaimsPrincipal.Current.AddIdentity(claimsIdentity);

            try
            {
                target.AutorizarCabecera(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraException()
        {
            int idCabeceraTest = 3;
            string mailUsuarioTest = "mail";

            var excepcionTest = new NullReferenceException("exploto molinos");

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            var claimsIdentity = new ClaimsIdentity(new List<Claim> { new Claim("userName", mailUsuarioTest) });
            ClaimsPrincipal.Current.AddIdentity(claimsIdentity);

            try
            {
                target.AutorizarCabecera(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleOk()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            DateTime hoy = new DateTime(2021, 8, 2);

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Returns(new EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto { Mensaje = "Se edito ok", FechaUltimaModificacion = hoy });

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            Assert.IsInstanceOf<EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto>(result.Data);

            var resultData = (EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto)result.Data;

            Assert.AreEqual("Se edito ok", resultData.Mensaje);
            Assert.AreEqual(hoy, resultData.FechaUltimaModificacion);

            gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalleDto>()), Times.Once);
            gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleInfoCustomException()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            var excepcionTest = new InfoCustomException("Algo");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            try
            {
                target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleValidationCustomException()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            var excepcionTest = new ValidationCustomException("Error de validacion");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            try
            {
                target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleException()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            var excepcionTest = new NullReferenceException("exploto molinos");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalleDto>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void DescargarFormularioCM05Ok()
        {
            int idCabeceraTest = 1;
            var testImagePath = TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png";
            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 1,    Archivo = new Archivo { Ruta = testImagePath }, },
            };
            this.gestionImpuestosServiceMock
                .Setup(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()))
                .Returns<int>(idCabecera => ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == idCabecera).Archivo.Ruta);

            var result = target.DescargarFormularioCM05(idCabeceraTest);

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<FileContentResult>(result.Data);

            FileContentResult resultData = (FileContentResult)result.Data;
            Assert.AreEqual("LogoBaufest.png", resultData.FileDownloadName);

            this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void DescargarFormularioCM05Exception()
        {
            int idCabeceraTest = 123;

            this.gestionImpuestosServiceMock
                .Setup(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()))
                .Throws(new Exception("exploto molinos"));

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.DescargarFormularioCM05(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(123), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoOk()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 1,
            };

            DateTime hoy = new DateTime(2021, 8, 2);

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)))
                .Returns(new EditarIngresosBrutosCoeficienteUnificadoResponseDto { Mensaje = "Se edito ok", FechaUltimaModificacion = hoy });

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            var result = target.EditarIngresosBrutosCoeficienteUnificado(parametroJson);

            Assert.IsInstanceOf<EditarIngresosBrutosCoeficienteUnificadoResponseDto>(result.Data);

            var resultData = (EditarIngresosBrutosCoeficienteUnificadoResponseDto)result.Data;

            Assert.AreEqual("Se edito ok", resultData.Mensaje);
            Assert.AreEqual(hoy, resultData.FechaUltimaModificacion);

            gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificado(It.IsAny<IngresosBrutosCoeficienteUnificadoDto>()), Times.Once);
            gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoInfoCustomException()
        {
            var ingresosBrutosCoeficienteUnificadoDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 1,
            };

            var excepcionTest = new InfoCustomException("Algo");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDtoTest);

            try
            {
                target.EditarIngresosBrutosCoeficienteUnificado(parametroJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoValidationCustomException()
        {
            var ingresosBrutosCoeficienteUnificadoDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 1,
            };

            var excepcionTest = new ValidationCustomException("Error de validacion");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDtoTest);

            try
            {
                target.EditarIngresosBrutosCoeficienteUnificado(parametroJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoException()
        {
            var ingresosBrutosCoeficienteUnificadoDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 1,
            };

            var excepcionTest = new NullReferenceException("exploto molinos");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDtoTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.EditarIngresosBrutosCoeficienteUnificado(parametroJson);
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificado(It.IsAny<IngresosBrutosCoeficienteUnificadoDto>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificado(It.Is<IngresosBrutosCoeficienteUnificadoDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void ListarMovimientosOk()
        {
            int idCabeceraTest = 3;

            var movimientoIngresosBrutosCoeficienteUnificadoDtoList = new List<MovimientoIngresosBrutosCoeficienteUnificadoDto>
            {
                new MovimientoIngresosBrutosCoeficienteUnificadoDto { Id = 1, IngresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificadoDto { Id = 1, } },
                new MovimientoIngresosBrutosCoeficienteUnificadoDto { Id = 2, IngresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificadoDto { Id = 2, } },
                new MovimientoIngresosBrutosCoeficienteUnificadoDto { Id = 3, IngresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificadoDto { Id = 3, } },
                new MovimientoIngresosBrutosCoeficienteUnificadoDto { Id = 4, IngresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificadoDto { Id = 3, } },
            };

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarMovimientos(It.IsAny<int>()))
                .Returns<int>(idCabecera => movimientoIngresosBrutosCoeficienteUnificadoDtoList.Where(x => x.IngresosBrutosCoeficienteUnificado.Id == idCabecera).ToList());

            var result = target.ListarMovimientos(idCabeceraTest);

            Assert.IsInstanceOf<List<MovimientoIngresosBrutosCoeficienteUnificadoDto>>(result.Data);

            List<MovimientoIngresosBrutosCoeficienteUnificadoDto> resultData = (List<MovimientoIngresosBrutosCoeficienteUnificadoDto>)result.Data;
            Assert.AreEqual(2, resultData.Count);
            Assert.AreEqual(movimientoIngresosBrutosCoeficienteUnificadoDtoList[2], resultData[0]);
            Assert.AreEqual(movimientoIngresosBrutosCoeficienteUnificadoDtoList[3], resultData[1]);

            this.gestionImpuestosServiceMock.Verify(g => g.ListarMovimientos(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ListarMovimientosInfoCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarMovimientos(idCabeceraTest))
                .Throws(excepcionTest);

            try
            {
                target.ListarMovimientos(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarMovimientos(idCabeceraTest), Times.Once);
        }

        [Test]
        public void ListarMovimientosValidationCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarMovimientos(idCabeceraTest))
                .Throws(excepcionTest);

            try
            {
                target.ListarMovimientos(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarMovimientos(idCabeceraTest), Times.Once);
        }

        [Test]
        public void ListarMovimientosException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new NullReferenceException("exploto molinos");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarMovimientos(1))
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.ListarMovimientos(idCabeceraTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.ListarMovimientos(It.IsAny<int>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarMovimientos(1), Times.Once);
        }

        [Test]
        public void GetCombosOk()
        {
            var estados = new List<EstadoIngresosBrutosCoeficienteUnificadoDto>
            {
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            var secuencias = new List<SecuenciaIngresosBrutosCoeficienteUnificadoDto>
            {
                  new SecuenciaIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                  new SecuenciaIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            this.gestionImpuestosServiceMock.Setup(g => g.ListarEstados()).Returns(estados);
            this.gestionImpuestosServiceMock.Setup(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador()).Returns(secuencias);

            var result = target.GetCombos();
            List<EstadoIngresosBrutosCoeficienteUnificadoDto> estadosList = (List<EstadoIngresosBrutosCoeficienteUnificadoDto>)result.Data.GetType().GetProperty("estados").GetValue(result.Data);
            List<SecuenciaIngresosBrutosCoeficienteUnificadoDto> secuenciasList = (List<SecuenciaIngresosBrutosCoeficienteUnificadoDto>)result.Data.GetType().GetProperty("secuencias").GetValue(result.Data);

            Assert.IsInstanceOf<List<EstadoIngresosBrutosCoeficienteUnificadoDto>>(result.Data.GetType().GetProperty("estados").GetValue(result.Data));
            Assert.IsInstanceOf<List<SecuenciaIngresosBrutosCoeficienteUnificadoDto>>(result.Data.GetType().GetProperty("secuencias").GetValue(result.Data));

            Assert.AreEqual(2, estadosList.Count);
            Assert.AreEqual(2, secuenciasList.Count);

            Assert.AreEqual(estados[0].Descripcion, estadosList[0].Descripcion);
            Assert.AreEqual(estados[1].Descripcion, estadosList[1].Descripcion);
            Assert.AreEqual(secuencias[0].Descripcion, secuenciasList[0].Descripcion);
            Assert.AreEqual(secuencias[1].Descripcion, secuenciasList[1].Descripcion);

            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador(), Times.Once);
        }

        [Test]
        public void GetCombosInfoCustomExceptionEstados()
        {
            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarEstados())
                .Throws(excepcionTest);

            try
            {
                target.GetCombos();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
        }

        [Test]
        public void GetCombosValidationCustomExceptionEstados()
        {
            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarEstados())
                .Throws(excepcionTest);

            try
            {
                target.GetCombos();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
        }

        [Test]
        public void GetCombosExceptionEstados()
        {
            var excepcionTest = new NullReferenceException("exploto molinos");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarEstados())
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.GetCombos();
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
        }

        [Test]
        public void GetCombosInfoCustomExceptionSecuencias()
        {
            var excepcionTest = new InfoCustomException("Algo");

            var estados = new List<EstadoIngresosBrutosCoeficienteUnificadoDto>
            {
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarEstados())
                .Returns(estados);

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador())
                .Throws(excepcionTest);

            try
            {
                target.GetCombos();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Algo", e.Message);
            }

            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador(), Times.Once);
        }

        [Test]
        public void GetCombosValidationCustomExceptionSecuencias()
        {
            var excepcionTest = new ValidationCustomException("Error de validacion");

            var estados = new List<EstadoIngresosBrutosCoeficienteUnificadoDto>
            {
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarEstados())
                .Returns(estados);

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador())
                .Throws(excepcionTest);

            try
            {
                target.GetCombos();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Error de validacion", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador(), Times.Once);
        }

        [Test]
        public void GetCombosExceptionSecuencias()
        {
            var excepcionTest = new NullReferenceException("exploto molinos");

            var estados = new List<EstadoIngresosBrutosCoeficienteUnificadoDto>
            {
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                new EstadoIngresosBrutosCoeficienteUnificadoDto { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarEstados())
                .Returns(estados);

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador())
                .Throws(excepcionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.GetCombos();
            }
            catch (Exception e)
            {
                Assert.AreEqual("exploto molinos", e.Message);
            }
            this.gestionImpuestosServiceMock.Verify(g => g.ListarEstados(), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarSecuenciaIngresosBrutosCoeficientesUnificador(), Times.Once);
        }
    }
}
