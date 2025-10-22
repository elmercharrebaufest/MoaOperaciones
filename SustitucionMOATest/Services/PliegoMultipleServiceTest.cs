using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class PliegoMultipleServiceTest
    {
        private Mock<IRepositorio> repositorioMock;
        private Mock<IComprasService> comprasServiceMock;
        private PliegoMultipleService pliegoMultipleService;
        private Mock<IEmailComprasService> emailComprasServiceMock;

        [SetUp]
        public void Setup()
        {
            repositorioMock = new Mock<IRepositorio>();
            comprasServiceMock = new Mock<IComprasService>();
            emailComprasServiceMock = new Mock<IEmailComprasService>();
            pliegoMultipleService = new PliegoMultipleService(repositorioMock.Object, comprasServiceMock.Object, emailComprasServiceMock.Object);
        }

        [Test]
        public void GetPliegosMultiples_ShouldReturnPliegos_WhenNombrePliegoIsNullOrEmpty()
        {
            // Arrange
            var pliegos = new List<Pliego>
            {
                new Pliego { Multiple = true, NombreObra = "Obra1" },
                new Pliego { Multiple = true, NombreObra = "Obra2" }
            }.AsQueryable();

            repositorioMock.Setup(r => r.ListarConsultable<Pliego>(It.IsAny<Expression<Func<Pliego, bool>>>()))
                           .Returns(pliegos);

            // Act
            var result = pliegoMultipleService.GetPliegosMultiples(null);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Obra1", result[0].NombreObra);
            Assert.AreEqual("Obra2", result[1].NombreObra);
        }

        [Test]
        public void GetPliegosMultiples_ShouldReturnFilteredPliegos_WhenNombrePliegoIsProvided()
        {
            // Arrange
            var pliegos = new List<Pliego>
            {
                new Pliego { Multiple = true, NombreObra = "Obra1" },
                new Pliego { Multiple = true, NombreObra = "Obra2" }
            }.AsQueryable();

            repositorioMock.Setup(r => r.ListarConsultable<Pliego>(It.IsAny<Expression<Func<Pliego, bool>>>()))
                           .Returns(pliegos);

            // Act
            var result = pliegoMultipleService.GetPliegosMultiples("Obra1");

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Obra1", result[0].NombreObra);
        }

        [Test]
        public void CrearPliegoMultiple_ShouldCallGuardarPliego()
        {
            // Arrange
            var pliegoData = new SolpDto { Pliego_Id = null };
            var adjuntos = new Mock<HttpFileCollectionBase>().Object;
            var solpsAsociar = new List<int> { 1, 2, 3 };

            repositorioMock
                .Setup(r => r.Agregar(It.IsAny<Pliego>()))
                .Returns(new Pliego());
            comprasServiceMock.Setup(cs => cs.GuardarPliego(It.IsAny<SolpDto>(), It.IsAny<HttpFileCollectionBase>(), It.IsAny<bool>(), null, It.IsAny<Pliego>(), true))
                              .Returns(new Pliego());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                            .Returns(new List<Solp>() { new Solp { Id = 1, TipoSolpSap = (int)TipoSolpSap.Sap } });

            // Act
            pliegoMultipleService.CrearPliegoMultiple(pliegoData, adjuntos, solpsAsociar);

            // Assert
            comprasServiceMock.Verify(cs => cs.GuardarPliego(pliegoData, adjuntos, It.IsAny<bool>(), null, It.IsAny<Pliego>(), true), Times.Once);
        }

        [Test]
        public void EliminarPliegoMultiple_ShouldCallRemover()
        {
            // Arrange
            var pliego = new Pliego { Id = 1, Solps = new List<Solp>(), Archivos = new List<Archivo>() };
            repositorioMock.Setup(r => r.Obtener<Pliego>(1)).Returns(pliego);

            // Act
            pliegoMultipleService.EliminarPliegoMultiple(1);

            // Assert
            repositorioMock.Verify(r => r.Remover(pliego), Times.Once);
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GenerarZipPliego_ShouldReturnPdfFilePath_WhenNoArchivos()
        {
            // Arrange
            var pliego = new Pliego { Id = 1, NombreObra = "Obra1", Archivos = new List<Archivo>() };
            repositorioMock.Setup(r => r.Obtener<Pliego>(1)).Returns(pliego);
            comprasServiceMock.Setup(cs => cs.GenerarSolpPdf(1, true)).Returns(new byte[0]);

            // Act
            var result = pliegoMultipleService.GenerarZipPliego(1, AppDomain.CurrentDomain.BaseDirectory, out string mimeType);

            // Assert
            Assert.AreEqual(CustomMediaTypeNames.Application.Pdf, mimeType);
        }
    }

}
