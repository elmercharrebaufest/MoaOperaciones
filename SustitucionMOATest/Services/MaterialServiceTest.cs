using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class MaterialServiceTest
    {
        private MaterialService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IObtenerMaterialesSolpConsumerMOA> obtenerMaterialesSolpConsumerMOAMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            obtenerMaterialesSolpConsumerMOAMock = new Mock<IObtenerMaterialesSolpConsumerMOA>();
            target = new MaterialService(repositorioMock.Object,
                                         obtenerMaterialesSolpConsumerMOAMock.Object);
        }

        [Test]
        public void ActualizarMaterialesSolpOk()
        {
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "Centro", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            obtenerMaterialesSolpConsumerMOAMock.Setup(y => y.request(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new MaterialWSMOAResponse
            {
                Materiales = new List<SustitucionMOAModel.Models.WSMapMOA.Compras.Material> { new SustitucionMOAModel.Models.WSMapMOA.Compras.Material {
                NroMaterial = "", NombreDeMaterial = "", TipoMaterial = "", TipoValoracion = "", GrupoCompras = "", PrecioDelMaterial = 500, CuentaDeMayor = "", TextoAmpliado = ""} }
            });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<MaterialSolp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<MaterialSolp> { new MaterialSolp { CodigoSap = "", Centro_Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<UnidadMedidaSap> { new UnidadMedidaSap { UM = "05", Comercial = "05", Tecnica = "05", Id = 1, TextoUM = "", TextoUM2 = "" } });
            target.ActualizarMaterialesSolp();

            repositorioMock.Verify(x => x.Agregar(It.IsAny<MaterialSolp>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }
    }
}
