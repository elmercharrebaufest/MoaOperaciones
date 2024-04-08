using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class OrdenResiduosServiceTest
    {
        private Mock<IRepositorioOrdenResiduos> mIRepositorioOrdenResiduos;
        private IOrdenResiduosService target;

        [SetUp]
        public void SetUp()
        {
            mIRepositorioOrdenResiduos = new Mock<IRepositorioOrdenResiduos>();
            target = new OrdenResiduosService(mIRepositorioOrdenResiduos.Object);
        }

        [Test]
        public void ObtenerMateriales_Ok()
        {
            var materialesRepo = new MaterialDto[]
            {
                new MaterialDto { MaterialId = 3, Descripcion = "Residuo1" },
                new MaterialDto { MaterialId = 4, Descripcion = "Insumo1" }
            };

            mIRepositorioOrdenResiduos
                .Setup(x => x.ObtenerMateriales())
                .Returns(materialesRepo);

            var resp = target.ObtenerMateriales();

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            mIRepositorioOrdenResiduos
                .Verify(x => x.ObtenerMateriales(), Times.Once);
        }
    }
}
