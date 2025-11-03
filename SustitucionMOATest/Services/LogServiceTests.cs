using NUnit.Framework;
using SustitucionMOAModel.Dto;
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

        [SetUp]
        public void SetUp()
        {
            logService = new LogService();
        }

        [Test]
        public void EliminarLogsAntiguosOk()
        {
            logService.EliminarLogsAntiguos();
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

            var testService = new TestableLogService();

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
            var testService = new TestableLogService();
            var deleted = testService.BorrarArchivosViejosPublic(@"Z:\NonExistentPath", TimeSpan.FromDays(180));
            Assert.IsNotNull(deleted);
            Assert.AreEqual(0, deleted.Count);
        }

        private class TestableLogService : LogService
        {
            public IList<ArchivoLog> BorrarArchivosViejosPublic(string path, TimeSpan antiguedad)
            {
                return base.BorrarArchivosViejos(path, antiguedad);
            }
        }
    }
}