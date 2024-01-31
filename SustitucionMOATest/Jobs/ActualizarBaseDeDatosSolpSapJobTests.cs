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
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.AgregarRegistroInfoServiceWebMOA;

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
            TablaSapDto tablaSapDto = new TablaSapDto {
                Id = -1,
                Codigo = "s",
                CodigoSap = "s",
                Descripcion = "s",
                Tabla = "s",
            };
            TablaSapDto tablaSapDto2 = new TablaSapDto
            {
                Id = 1,
                Codigo = "s",
                CodigoSap = "s",
                Descripcion = "s",
                Tabla = "s",
            };
            comprasServiceMock.Setup(x => x.ObtenerOrdenesSap(It.IsAny<string>()))
                .Returns(new List<TablaSapDto> { tablaSapDto, tablaSapDto2 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(new HabilitacionJob { Habilitado = true });
            comprasServiceMock.Setup(x => x.ObtenerCecoSap())
                .Returns(new List<TablaSapDto> { tablaSapDto, tablaSapDto2 });
            comprasServiceMock.Setup(x => x.ObtenerCuentasSap())
                .Returns(new List<TablaSapDto> { tablaSapDto, tablaSapDto2 });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            target.Execute();

            comprasServiceMock.Verify(x => x.ObtenerCecoSap(), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
        }
    }
}
