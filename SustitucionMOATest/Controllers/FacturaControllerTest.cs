using Moq;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class FacturaControllerTest
    {
        private Mock<IFacturaService> mockFacturaService;
        private FacturaController controller;

        [SetUp]
        public void SetUp()
        {
            mockFacturaService = new Mock<IFacturaService>();
            controller = new FacturaController(mockFacturaService.Object);
        }

        [Test]
        public void SubirPDF_FilesProvided_ReturnsJsonResultWithData()
        {
            // Arrange
            var files = new List<HttpPostedFileBase> { new Mock<HttpPostedFileBase>().Object };
            var expectedData = new List<ValidationResult>();
            mockFacturaService.Setup(s => s.SubirPDF(It.IsAny<List<HttpPostedFileBase>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(expectedData);

            // Act
            var result = controller.subirPDF("factura", files) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
        }

        [Test]
        public void SubirPDF_NoFilesProvided_ReturnsJsonResultWithError()
        {
            // Arrange
            var files = new List<HttpPostedFileBase>();

            // Act
            var result = controller.subirPDF("factura", files) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Data.ToJson();
            Assert.IsTrue(data.Contains("El archivo Factura es requerido"));
        }
    }
}
