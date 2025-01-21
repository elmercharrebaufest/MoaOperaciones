using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Usuario = SustitucionMOAModel.Entities.Usuario;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class FacturaServiceTest
    {
        private FacturaService target;
        private Mock<IAzureService> azureServiceMock;
        private Mock<IAnalisisDocumentoService> analisisDocumentoServiceMock;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IObtenerOrdenDeCompraConsumerMOA> obtenerOrdenDeCompraConsumerMOAMock;
        private Mock<IEmailService> emailServiceMock;

        [SetUp]
        public void SetUp()
        {
            azureServiceMock = new Mock<IAzureService>();
            analisisDocumentoServiceMock = new Mock<IAnalisisDocumentoService>();
            repositorioMock = new Mock<IRepositorio>();
            obtenerOrdenDeCompraConsumerMOAMock = new Mock<IObtenerOrdenDeCompraConsumerMOA>();
            emailServiceMock = new Mock<IEmailService>();

            target = new FacturaService(
                azureServiceMock.Object,
                analisisDocumentoServiceMock.Object,
                repositorioMock.Object,
                obtenerOrdenDeCompraConsumerMOAMock.Object,
                emailServiceMock.Object
            );

            ConfigurationManager.AppSettings["EmailFacturasES"] = "moaoperaciones@baufest.com";
            ConfigurationManager.AppSettings["FolderFacturasES"] = TestContext.CurrentContext.TestDirectory;
        }

        [Test]
        public void SubirPDF_ShouldReturnValidationResults()
        {
            // Arrange
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("test.pdf");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));
            var files = new List<HttpPostedFileBase> { file1.Object };

            var cuit = "123456789";
            var codigo = "ABC123";
            var mail = "test@example.com";
            var usuario = new Usuario { Id = 1, Mail = mail };
            var operacionOCRId = "operationId";
            var elementosLeidos = new List<string> { "element1", "element2" };
            var validationResult = new ValidationResult { IsValid = true, FileName = "test.pdf" };



            repositorioMock.Setup(r => r.Obtener<Usuario>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Usuario, bool>>>())).Returns(usuario);
            azureServiceMock.Setup(a => a.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>())).ReturnsAsync(operacionOCRId);
            azureServiceMock.Setup(a => a.ObtenerResultadoOCRAsync(operacionOCRId)).ReturnsAsync(elementosLeidos);
            analisisDocumentoServiceMock.Setup(a => a.AnalizarFacturaCertificacionServicios(It.IsAny<List<string>>(), cuit, It.IsAny<string>())).Returns(new List<ValidationResult> { validationResult });

            // Act
            var result = target.SubirPDF(files, cuit, codigo, mail);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.AreEqual(validationResult.FileName, result[0].FileName);
        }

        [Test]
        public void SubirPDF_ShouldSaveFileAndSendEmail_WhenValidationIsValid()
        {
            // Arrange
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("test.pdf");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));
            var files = new List<HttpPostedFileBase> { file1.Object };

            var cuit = "123456789";
            var codigo = "ABC123";
            var mail = "test@example.com";
            var usuario = new Usuario { Id = 1, Mail = mail };
            var operacionOCRId = "operationId";
            var elementosLeidos = new List<string> { "element1", "element2" };
            var validationResult = new ValidationResult { IsValid = true, FileName = "test.pdf" };

            repositorioMock.Setup(r => r.Obtener<Usuario>(It.IsAny<System.Linq.Expressions.Expression<System.Func<Usuario, bool>>>())).Returns(usuario);
            azureServiceMock.Setup(a => a.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>())).ReturnsAsync(operacionOCRId);
            azureServiceMock.Setup(a => a.ObtenerResultadoOCRAsync(operacionOCRId)).ReturnsAsync(elementosLeidos);
            analisisDocumentoServiceMock.Setup(a => a.AnalizarFacturaCertificacionServicios(It.IsAny<List<string>>(), cuit, It.IsAny<string>())).Returns(new List<ValidationResult> { validationResult });

            // Act
            target.SubirPDF(files, cuit, codigo, mail);

            // Assert
            repositorioMock.Verify(r => r.Agregar(It.IsAny<Archivo>()), Times.Once);
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once);
            emailServiceMock.Verify(e => e.EnviarMail(It.IsAny<SustitucionMOAUtils.Email.EmailSenderData>()), Times.Once);
        }
        [Test]
        public void EliminarFacturasAntiguasTest()
        {
            // Arrange
            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<ResultadoOcr, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<ResultadoOcr, object>>>>()))
               .Returns(new List<ResultadoOcr>());
            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<ResultadoAnalisisOcr, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<ResultadoAnalisisOcr, object>>>>()))
               .Returns(new List<ResultadoAnalisisOcr>());
            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<Archivo, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<Archivo, object>>>>()))
               .Returns(new List<Archivo>());

            // Act
            target.EliminarFacturasAntiguas();

            // Assert
            repositorioMock.Verify(r => r.Listar(It.IsAny<Expression<Func<ResultadoOcr, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<ResultadoOcr, object>>>>()), Times.Once);
            repositorioMock.Verify(r => r.Listar(It.IsAny<Expression<Func<ResultadoAnalisisOcr, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<ResultadoAnalisisOcr, object>>>>()), Times.Once);
            repositorioMock.Verify(r => r.Listar(It.IsAny<Expression<Func<Archivo, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<Archivo, object>>>>()), Times.Once);

            repositorioMock.Verify(r => r.RemoverTodos(It.IsAny<IEnumerable<ResultadoOcr>>()), Times.Once);
            repositorioMock.Verify(r => r.RemoverTodos(It.IsAny<IEnumerable<ResultadoAnalisisOcr>>()), Times.Once);
            repositorioMock.Verify(r => r.RemoverTodos(It.IsAny<IEnumerable<Archivo>>()), Times.Once);

            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once);
        }

    }
}
