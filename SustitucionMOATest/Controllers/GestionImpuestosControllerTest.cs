using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class GestionImpuestosControllerTest
    {
        GestionImpuestosController target;

        Mock<IGestionImpuestosService> gestionImpuestosServiceMock;

        [SetUp]
        public void SetUp()
        {
            this.gestionImpuestosServiceMock = new Mock<IGestionImpuestosService>();

            this.target = new GestionImpuestosController(gestionImpuestosServiceMock.Object);
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

            var result = target.ListarCabeceras();

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarCabecerasValidationCustomException()
        {
            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarCabeceras())
                .Throws(excepcionTest);

            var result = target.ListarCabeceras();

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
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

            var result = target.ListarDetalles(idCabeceraTest);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
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

            var result = target.ListarDetalles(idCabeceraTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(idCabeceraTest), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraOk()
        {
            int idCabeceraTest = 3;

            var ingresosBrutosCoeficienteUnificadoDetalleDtoList = new List<IngresosBrutosCoeficienteUnificadoDetalleDto>();

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(It.IsAny<int>()))
                .Returns("Se autorizo ok");

            var result = target.AutorizarCabecera(idCabeceraTest);

            Assert.AreEqual("Se autorizo ok", result.Data);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraInfoCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(idCabeceraTest))
                .Throws(excepcionTest);

            var result = target.AutorizarCabecera(idCabeceraTest);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest), Times.Once);
        }

        [Test]
        public void AutorizarCabeceraValidationCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.AutorizarCabecera(idCabeceraTest))
                .Throws(excepcionTest);

            var result = target.AutorizarCabecera(idCabeceraTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleOk()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Returns("Se edito ok");
            
            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            Assert.AreEqual("Se edito ok", result.Data);

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

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
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

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }
    }
}
