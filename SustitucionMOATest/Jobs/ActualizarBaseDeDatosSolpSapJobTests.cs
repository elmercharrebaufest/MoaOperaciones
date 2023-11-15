using NUnit.Framework;
using Moq;
using SustitucionMOA.Jobs;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using SustitucionMOAModel.Consultas;
using System.Linq.Expressions;

namespace SustitucionMOATest.Jobs
{
    [TestFixture]
    public class ActualizarBaseDeDatosSolpSapJobTests
    {
        private ActualizarBaseDeDatosSolpSapJob target;
        private Mock<IComprasService> comprasServiceMock;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            comprasServiceMock = new Mock<IComprasService>();
            repositorioMock = new Mock<IRepositorio>();
            target = new ActualizarBaseDeDatosSolpSapJob(comprasServiceMock.Object, repositorioMock.Object);
        }

        [Test]
        public void Execute_DebeActualizarTablasSapCorrectamente()
        {
            repositorioMock.Setup(x => x.Listar<TablaSap>(
                It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DirOrden>(),
                It.IsAny<IEnumerable<Expression<Func<TablaSap, object>>>>())
                ).Returns(new List<TablaSap>());

            comprasServiceMock.Setup(x => x.ObtenerOrdenesSap(It.IsAny<string>())).Returns(new List<SustitucionMOAModel.Dto.TablaSapDto>());

            target.Execute();

            comprasServiceMock.Verify(x => x.ObtenerCecoSap(), Times.Once);
            repositorioMock.Verify(x => x.Listar<TablaSap>(
                It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DirOrden>(),
                It.IsAny<IEnumerable<Expression<Func<TablaSap, object>>>>()), Times.Exactly(3));

        }
    }
}
