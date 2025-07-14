using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.Compras.Factura;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.IO;
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

        [Test]
        public void RegistrarCertificacion_ValidInput_ReturnsJsonResult()
        {
            // Arrange
            var grupos = new List<GrupoCertificaciones>
            {
                new GrupoCertificaciones
                {
                    Items = new List<CertificacionDto> { new CertificacionDto { NRO_Certificacion = "123", NombreDeArchivo = "archivo1.pdf" } },
                    NombreArchivo = "archivo1.pdf",
                    EsFacturaPorDiferenciaTasaDeCambio = false
                },
                new GrupoCertificaciones
                {
                    Items = new List<CertificacionDto> { new CertificacionDto { NRO_Certificacion = "456", NombreDeArchivo = "archivo2.pdf" } },
                    NombreArchivo = "archivo2.pdf",
                    EsFacturaPorDiferenciaTasaDeCambio = false
                }
            };
            var gruposJson = JsonConvert.SerializeObject(grupos);
            var files = new List<HttpPostedFileBase>();

            mockFacturaService.Setup(s => s.RegistrarCertificaciones(It.IsAny<List<GrupoCertificaciones>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<HttpPostedFileBase>>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<CertificacionRegistrada>());

            // Act
            var result = controller.RegistrarCertificacion(gruposJson, files) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<JsonResult>(result);
            mockFacturaService.Verify(s => s.RegistrarCertificaciones(It.IsAny<List<GrupoCertificaciones>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<List<HttpPostedFileBase>>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
