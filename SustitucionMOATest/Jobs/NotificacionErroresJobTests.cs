using Castle.Core.Logging;
using Moq;
using NUnit.Framework;
using SustitucionMOA.Jobs;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;

namespace SustitucionMOATests.Jobs
{
    [TestFixture]
    public class NotificacionErroresJobTests
    {
        private Mock<IRepositorio> repositorioMock;
        private Mock<ILogTableService> logTableServiceMock;
        private Mock<IEmailService> emailServiceMock;
        private Mock<IHttpContextService> httpContextServiceMock;
        private NotificacionErroresJob job;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            logTableServiceMock = new Mock<ILogTableService>();
            emailServiceMock = new Mock<IEmailService>();
            httpContextServiceMock = new Mock<IHttpContextService>();

            job = new NotificacionErroresJob(repositorioMock.Object, logTableServiceMock.Object, emailServiceMock.Object, httpContextServiceMock.Object);
            // Arrange
            var habilitacion = new HabilitacionJob { Nombre = "NotificacionErroresJob", Habilitado = true };
            repositorioMock.Setup(r => r.Obtener(It.IsAny<Expression<Func<HabilitacionJob, bool>>>())).Returns(habilitacion);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Configuracion, bool>>>(), It.IsAny<Expression<Func<Configuracion, string>>>()))
                .Returns("5");
            httpContextServiceMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png");

        }

        [Test]
        public void Execute_NoErrors_NoEmailSent()
        {
            logTableServiceMock.Setup(l => l.ObtenerLogs(It.IsAny<DateTime>(), true)).Returns(new List<LogTableCountErrors>());

            // Act
            job.Execute();

            // Assert
            emailServiceMock.Verify(e => e.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(), null, null, null, null, null), Times.Never);
        }

        [Test]
        public void Execute_ErrorsFound_EmailSent()
        {
            var errors = new List<LogTableCountErrors>
            {
                new LogTableCountErrors
                {
                    Logger = "TestLogger",
                    Level = "Error",
                    Count = 5,
                    Errors = new List<LogTableDto>
                    {
                        new LogTableDto { Date = DateTime.Now, Message = "Error 1", Exception = "Exception 1" },
                        new LogTableDto { Date = DateTime.Now, Message = "Error 2", Exception = "Exception 2" }
                    }
                }
            };
            logTableServiceMock.Setup(l => l.ObtenerLogs(It.IsAny<DateTime>(), true)).Returns(errors);

            // Act
            job.Execute();

            // Assert
            emailServiceMock.Verify(e => e.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<List<string>>(), It.IsAny<AlternateView>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<List<string>>(), It.IsAny<Dictionary<string, byte[]>>()),
                Times.Once);
        }
    }
}
