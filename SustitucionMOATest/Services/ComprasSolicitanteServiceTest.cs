using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasSolicitanteServiceTest
    {
        private ComprasSolicitanteService target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new ComprasSolicitanteService(repositorioMock.Object);
        }

        [Test]
        public void ListarSolpOk()
        {
            var usuarioCompras = new List<TablaGeneralDto> { new TablaGeneralDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            var listaPaginada = new ListaPaginada<SolpDto>(new List<SolpDto> { new SolpDto { Id = 1, ItemsTotales = 2 } }, 1, 10, 5);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
            .Returns(new List<UsuarioCompras>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, PeticionDeOfertaDto>>>(), It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<PeticionDeOfertaDto>() { new PeticionDeOfertaDto { Id = 1, Solp_Id = 1, RegistroInfo = false, UsuarioCreador_Id = 1, FechaCreacion = new DateTime(), Observaciones = "",
              Usuarios = new List<PeticionDeOfertaUsarioDto>() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, AdjudicacionDto>>>(), It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<AdjudicacionDto>() { new AdjudicacionDto { Id = 1, Solp_Id = 1, UsuarioCreador_Id = 1, FechaCreacion = new DateTime(), NumeroOrdenDeCompra = "Nro",
              Proveedor = "Proveedor" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(), It.IsAny<Paginacion>(), It.IsAny<Expression<Func<Solp, bool>>>()))
              .Returns(listaPaginada);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario { Roles = new List<Rol> { new Rol { Codigo = "COMPRADOR" } } });

            var result = target.ListarSolp(new UsuarioDto { Id = 1, Permisos = new List<string> { "VER TODAS SOLPS" } }, new Paginacion(), "", "", new DateTime(), new DateTime(), true, false, true, false, false);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result, listaPaginada);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }
    }
}
