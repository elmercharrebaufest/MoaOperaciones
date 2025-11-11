using Moq;
using Moq.Protected;
using NUnit.Framework;
using SustitucionMOAModel.Dto;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace SustitucionMOATests.Services
{
    [TestFixture]
    public class LogServiceTests
    {
        private LogService logService;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            logService = new LogService(repositorioMock.Object);
        }

        [Test]
        public void EliminarLogsAntiguosOk()
        {
            // Mockear método BorrarArchivosViejos para evitar IO real
            var archivosEliminados = new List<ArchivoLog>{
                new ArchivoLog { Nombre = "log1.txt", PesoKB = 10, FechaCreacion = DateTime.Now.AddDays(-40), FechaUltimaModificacion = DateTime.Now.AddDays(-40) }
            };
            var logServiceMock = new Mock<LogService>(repositorioMock.Object) { CallBase = true };
            logServiceMock
                .Protected()
                .Setup<List<ArchivoLog>>("BorrarArchivosViejos", ItExpr.IsAny<string>(), ItExpr.IsAny<TimeSpan>())
                .Returns(archivosEliminados);

            // Act
            logServiceMock.Object.EliminarLogsAntiguos();

            // Assert
            var expectedDate = DateTime.Today.AddDays(-30).ToString("yyyyMMdd");
            repositorioMock.Verify(r => r.ExecuteCommand($"delete logs.logtable where date <= '{expectedDate}'"), Times.Once);
        }

        [Test]
        public void BorrarArchivosViejos_DeletesOldFiles_ReturnsDeletedFiles()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var oldFile = Path.Combine(tempDir, "old.log");
            var newFile = Path.Combine(tempDir, "new.log");
            File.WriteAllText(oldFile, "old");
            File.WriteAllText(newFile, "new");

            File.SetLastWriteTime(oldFile, DateTime.Now.AddDays(-200));
            File.SetLastWriteTime(newFile, DateTime.Now);

            var testService = new TestableLogService(repositorioMock.Object);

            // Act
            var deleted = testService.BorrarArchivosViejosPublic(tempDir, TimeSpan.FromDays(180));

            // Assert
            Assert.AreEqual(1, deleted.Count);
            Assert.AreEqual(oldFile, deleted[0].Nombre);
            Assert.IsFalse(File.Exists(oldFile));
            Assert.IsTrue(File.Exists(newFile));

            // Cleanup
            File.Delete(newFile);
            Directory.Delete(tempDir);
        }

        [Test]
        public void BorrarArchivosViejos_DirectoryNotFound_LogsError()
        {
            var testService = new TestableLogService(repositorioMock.Object);
            var deleted = testService.BorrarArchivosViejosPublic(@"Z:\NonExistentPath", TimeSpan.FromDays(180));
            Assert.IsNotNull(deleted);
            Assert.AreEqual(0, deleted.Count);
        }

        private class TestableLogService : LogService
        {
            public TestableLogService(IRepositorio repositorio) : base(repositorio)
            {
            }

            public IList<ArchivoLog> BorrarArchivosViejosPublic(string path, TimeSpan antiguedad)
            {
                return base.BorrarArchivosViejos(path, antiguedad);
            }
        }
    }
}