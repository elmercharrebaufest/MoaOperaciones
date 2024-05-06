using Moq;
using NUnit.Framework;
using SustitucionMOA.Jobs;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Jobs
{
    internal class CcSsObtenerArchivosUcropJobTests
    {
        private CcSsObtenerArchivosUcropJob target;
        private Mock<ICampoSustentableService> campoSustentableMock;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            campoSustentableMock = new Mock<ICampoSustentableService>();
            repositorioMock = new Mock<IRepositorio>();
            target = new CcSsObtenerArchivosUcropJob(campoSustentableMock.Object, repositorioMock.Object);
        }
        [Test]
        public void Execute_HabilitacionDeshabilitado_ReturnAntes()
        {
            repositorioMock
                .Setup(repositorio => repositorio.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(new HabilitacionJob { Habilitado = false});

            target.Execute();

            repositorioMock.Verify(expression: repositorio => 
                repositorio.Listar<ArchivoCampoSustentable>(It.IsAny<Expression<Func<ArchivoCampoSustentable, bool>>>(),0,null,DirOrden.Asc, null),
                times: Times.Never);
            Assert.That(repositorioMock.Invocations.Count == 1 );
        }
        [Test]
        public void Execute_HabilitacionHabilitado_DescargaPorCadaItemListado()
        {
            repositorioMock
                .Setup(repositorio => repositorio.Obtener<HabilitacionJob>(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(new HabilitacionJob { Habilitado = true });

            repositorioMock
                .Setup(repositorio => repositorio
                    .Listar<ArchivoCampoSustentable>(It.IsAny<Expression<Func<ArchivoCampoSustentable, bool>>>(), 0, null, DirOrden.Asc, null))
                .Returns(new List<ArchivoCampoSustentable> { new ArchivoCampoSustentable () , new ArchivoCampoSustentable { } });

            target.Execute();

            repositorioMock.Verify(expression: repositorio =>
                repositorio.Listar<ArchivoCampoSustentable>(It.IsAny<Expression<Func<ArchivoCampoSustentable, bool>>>(), 0, null, DirOrden.Asc, null),
                times: Times.Once);
            campoSustentableMock.Verify(servicio => servicio.DescargarArchivosDeGoogleDrive(It.IsAny<ArchivoCampoSustentable>()),
                Times.Exactly(2));
        }
    }
}
