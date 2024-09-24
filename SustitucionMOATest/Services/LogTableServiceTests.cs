using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SustitucionMOATests.Services
{
    [TestFixture]
    public class LogTableServiceTests
    {
        private Mock<IRepositorio> repositorioMock;
        private LogTableService logTableService;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            logTableService = new LogTableService(repositorioMock.Object);
        }

        [Test]
        public void ObtenerLogs_SoloErrores_ReturnsOnlyErrorLogs()
        {
            // Arrange
            var desde = DateTime.Now.AddMinutes(-10);
            var logData = new List<LogTableDto>
            {
                new LogTableDto(new LogTable { Logger = "TestLogger1", Level = "error", Date = DateTime.Now }),
                new LogTableDto(new LogTable { Logger = "TestLogger2", Level = "info", Date = DateTime.Now }),
                new LogTableDto(new LogTable { Logger = "TestLogger3", Level = "error", Date = DateTime.Now.AddMinutes(-5) })
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<LogTable, LogTableDto>>>(), It.IsAny<Expression<Func<LogTable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
                .Returns(logData.Where(l => l.Level == "error").ToList());

            // Act
            var result = logTableService.ObtenerLogs(desde, true);

            // Assert
            Assert.AreEqual(2, result.Count); 
            Assert.IsTrue(result.TrueForAll(l => l.Level == "error"));
            repositorioMock.Verify(r => r.Listar(It.IsAny<Expression<Func<LogTable, LogTableDto>>>(), It.IsAny<Expression<Func<LogTable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc), Times.Once);
        }

        [Test]
        public void ObtenerLogs_NoSoloErrores_ReturnsAllLogs()
        {
            // Arrange
            var desde = DateTime.Now.AddMinutes(-10);
            var logData = new List<LogTableDto>
            {
                new LogTableDto(new LogTable { Logger = "TestLogger1", Level = "error", Date = DateTime.Now }),
                new LogTableDto(new LogTable { Logger = "TestLogger2", Level = "info", Date = DateTime.Now }),
                new LogTableDto(new LogTable { Logger = "TestLogger3", Level = "error", Date = DateTime.Now.AddMinutes(-5) })
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<LogTable, LogTableDto>>>(), It.IsAny<Expression<Func<LogTable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
               .Returns(logData);

            // Act
            var result = logTableService.ObtenerLogs(desde, false);

            // Assert
            Assert.AreEqual(3, result.Count); // Se deben retornar todos los logs (errores y no errores)
            repositorioMock.Verify(r => r.Listar(It.IsAny<Expression<Func<LogTable, LogTableDto>>>(), It.IsAny<Expression<Func<LogTable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc), Times.Once);
        }

        [Test]
        public void ObtenerLogs_ExcludesFrontLogger()
        {
            // Arrange
            var desde = DateTime.Now.AddMinutes(-10);
            var logData = new List<LogTableDto>
            {
                new LogTableDto(new LogTable { Logger = "TestLogger1", Level = "error", Date = DateTime.Now }),
                new LogTableDto(new LogTable { Logger = "frontLogger", Level = "error", Date = DateTime.Now }), // Debe ser excluido
                new LogTableDto(new LogTable { Logger = "TestLogger3", Level = "error", Date = DateTime.Now.AddMinutes(-5) })
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<LogTable, LogTableDto>>>(), It.IsAny<Expression<Func<LogTable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
                .Returns(logData.Where(l => l.Logger != "frontLogger").ToList());

            // Act
            var result = logTableService.ObtenerLogs(desde, true);

            // Assert
            Assert.AreEqual(2, result.Count); // El logger "frontLogger" debe ser excluido
            Assert.IsTrue(result.All(l => l.Logger != "frontLogger"));
            repositorioMock.Verify(r => r.Listar(It.IsAny<Expression<Func<LogTable, LogTableDto>>>(), It.IsAny<Expression<Func<LogTable, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc), Times.Once);
        }
    }
}
