using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class TablaSapServiceTest
    {
        TablaSapService target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();

            target = new TablaSapService(repositorioMock.Object);
        }

        [Test]
        public void ListarTablaSapOk()
        {
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "0011", Tabla = "OrdenSolpSap", CodigoSap = "11", Descripcion = "Limpiar rotor" },
            new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" }};
            var tablaSap = new List<TablaSap> { new TablaSap { Id = 11, Codigo = "0011", Tabla = "OrdenSolpSap", CodigoSap = "11", Descripcion = "Limpiar rotor" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpPosicion> { new SolpPosicion { Id = 1, ValorTipoImputacion_Id = 11 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpSubposicion> { new SolpSubposicion { Id = 1, TipoImputacion_Id = 11 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            var result = target.ListarTablaSap(new List<string> { "OrdenSolpSap" });

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test()]
        public void AutocompleteServiciosSapTest()
        {
            List<TablaSapDto> ListaSap = new List<TablaSapDto>
            {
                new TablaSapDto {Id=1, Descripcion = "Prueba 1", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=2, Descripcion = "Prueba 2", CodigoSap="Otro", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=3, Descripcion = "Prueba 3", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=4, Descripcion = "Prueba 1", CodigoSap="MOA Operaciones", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=5, Descripcion = "Prueba 1", CodigoSap="MOA", Tabla = TablasSap.CecoSolpSap},
            };

            repositorioMock.Setup(x => x.Listar(
                It.IsAny<Expression<Func<TablaSap, TablaSapDto>>>(),
                It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DirOrden>()))
                .Returns(ListaSap);

            var expected = new List<TablaSapDto>
            {
                new TablaSapDto { Id = 1,  Descripcion = "Prueba 2", CodigoSap="Otro", Tabla = TablasSap.CodigoServicioSap }
            };

            var result = target.AutocompleteTablaSap(TablasSap.CodigoServicioSap, "ot");

            Assert.AreEqual(expected[0].Id, result[0].Id);
        }
    }
}
