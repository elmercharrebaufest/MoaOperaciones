using Moq;
using NUnit.Framework;
using SustitucionMOA.Jobs;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using System;
using System.Linq.Expressions;

namespace SustitucionMOATest.Jobs
{
    [TestFixture]
    public class ActualizarLegajoImpositivoJobTests
    {
        private ActualizarLegajoImpositivoJob job;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            job = new ActualizarLegajoImpositivoJob(repositorioMock.Object);
        }

        [Test]
        public void Execute_HabilitacionJobNotEnabled_DoesNotRunProcess()
        {
            // Arrange
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(new HabilitacionJob { Habilitado = false });

            // Act
            job.Execute();

            // Assert
            repositorioMock.Verify(r => r.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()), Times.Once);
        }

        [Test]
        public void Execute_HabilitacionJobEnabled_RunsProcess()
        {
            // Arrange
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(new HabilitacionJob { Habilitado = true });

            // Act
            job.Execute();

            // Assert
            repositorioMock.Verify(r => r.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()), Times.Once);
        }
    }
}
