using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.PliegoMultiple;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    public class PliegoMultipleControllerTest
    {
        private Mock<IPliegoMultipleService> mockPliegoMultipleService;
        private Mock<IUsuarioService> mockUsuarioService;
        private PliegoMultipleController controller;

        [SetUp]
        public void SetUp()
        {
            mockPliegoMultipleService = new Mock<IPliegoMultipleService>();
            mockUsuarioService = new Mock<IUsuarioService>();
            controller = new PliegoMultipleController(mockPliegoMultipleService.Object, mockUsuarioService.Object);
        }

        [Test()]
        public void GetPliegosMultiples_ReturnsJsonResult()
        {
            // Arrange
            var pliegos = new List<PliegoPMDto>();
            mockPliegoMultipleService.Setup(service => service.GetPliegosMultiples(It.IsAny<string>())).Returns(pliegos);

            // Act
            var result = controller.GetPliegosMultiples("test");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pliegos, result.Data);
        }

        [Test()]
        public void GetSolpDisponiblesPliegosMultiple_ReturnsJsonResult()
        {
            // Arrange
            var solps = new List<SolpPMDto>();
            mockPliegoMultipleService.Setup(service => service.GetSolpDisponiblesPliegosMultiple(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<IEnumerable<int>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int?>())).Returns(solps);

            // Act
            var result = controller.GetSolpDisponiblesPliegosMultiple("test","testNombrePliego", null, null, "1,2", "fiscal", false, false, false, false, false, false, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(solps, result.Data);
        }

        [Test()]
        public void CrearPliegoMultiple_ReturnsJsonResult()
        {
            var httpFileCollectionMock = new Mock<HttpFileCollectionBase>();
            var httpRequestMock = new Mock<HttpRequestBase>();
            httpRequestMock.Setup(x => x.Files).Returns(httpFileCollectionMock.Object);
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            var pliegoData = JsonConvert.SerializeObject(new SolpPMDto());
            var solpsAsociar = JsonConvert.SerializeObject(new List<int> { 1, 2 });
            mockUsuarioService.Setup(service => service.GetUsuario(It.IsAny<string>())).Returns(new UsuarioDto());

            // Act
            controller = new PliegoMultipleController(mockPliegoMultipleService.Object, mockUsuarioService.Object);
            controller.ControllerContext = new ControllerContext(httpContextMock.Object, new System.Web.Routing.RouteData(), controller);
            var result = controller.CrearPliegoMultiple(pliegoData, solpsAsociar) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test()]
        public void EliminarPliegoMultiple_ReturnsJsonResult()
        {
            // Act
            var result = controller.EliminarPliegoMultiple(1);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test()]
        public void DescargarZipPliego_ReturnsFileResult()
        {
            // Arrange
            var mimeType = "application/zip";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".\\TestFiles\\Example.KMZ");
            mockPliegoMultipleService.Setup(service => service.GenerarZipPliego(It.IsAny<int>(), It.IsAny<string>(), out mimeType)).Returns(filePath);

            // Act
            var result = controller.DescargarZipPliego(1);

            // Assert
            Assert.IsNotNull(result);
        }

        [Test()]
        public void TraerPliegoId_InvalidId_ThrowsArgumentException()
        {
            // Act
            Assert.Throws<ArgumentException>(() => controller.TraerPliegoId(0));
        }

        [Test()]
        public void TraerPliegoId_ValidId_ReturnsJsonResult()
        {
            // Arrange
            var pliego = new TraerPliegoPMDto();
            mockPliegoMultipleService.Setup(service => service.TraerPliegoId(It.IsAny<int>())).Returns(pliego);

            // Act
            var result = controller.TraerPliegoId(1) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pliego, result.Data);
        }

    }
}