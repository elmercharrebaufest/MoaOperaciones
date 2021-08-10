using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Helpers;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class GestionImpuestosServiceTest
    {
        private GestionImpuestosService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<ITimeProvider> timeProviderMock;
        private Mock<IConsultaService> consultaServiceMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            timeProviderMock = new Mock<ITimeProvider>();
            consultaServiceMock = new Mock<IConsultaService>();

            target = new GestionImpuestosService(repositorioMock.Object, timeProviderMock.Object, consultaServiceMock.Object);
        }

        [Test]
        public void ListarCabecerasOk()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 29);

            EstadoIngresosBrutosCoeficienteUnificado pendiente = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente };
            EstadoIngresosBrutosCoeficienteUnificado autorizado = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado };
            EstadoIngresosBrutosCoeficienteUnificado completado = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado };

            List<IngresosBrutosCoeficienteUnificado> cabeceras = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 1, Anticipo = 1, CUIT = "1", Sede = 1, FechaCarga = hoy, FechaUltimaModificacion = ayer, EstadoIngresosBrutosCoeficienteUnificado = pendiente },
                new IngresosBrutosCoeficienteUnificado { Id = 2, Anticipo = 2, CUIT = "2", Sede = 2, FechaCarga = hoy, FechaUltimaModificacion = ayer, EstadoIngresosBrutosCoeficienteUnificado = autorizado },
                new IngresosBrutosCoeficienteUnificado { Id = 3, Anticipo = 23, CUIT = "33", Sede = 3, FechaCarga = ayer, FechaUltimaModificacion = hoy, EstadoIngresosBrutosCoeficienteUnificado = completado },
            };

            this.repositorioMock
                .Setup(repo => repo.Listar(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(), null, 0, null, DirOrden.Asc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => 
                    cabeceras.Select(x => proy.Compile().Invoke(x)).ToList());

            var result = target.ListarCabeceras();

            this.repositorioMock.Verify(
                repo => repo.Listar(
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()),
                Times.Once);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(1, result[0].EstadoId);
            Assert.AreEqual(1, result[0].Anticipo);
            Assert.AreEqual("1", result[0].CUIT);
            Assert.AreEqual(hoy, result[0].FechaCarga);
            Assert.AreEqual(ayer, result[0].FechaUltimaModificacion);
            Assert.AreEqual(1, result[0].Sede);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(2, result[1].EstadoId);
            Assert.AreEqual(2, result[1].Anticipo);
            Assert.AreEqual("2", result[1].CUIT);
            Assert.AreEqual(hoy, result[1].FechaCarga);
            Assert.AreEqual(ayer, result[1].FechaUltimaModificacion);
            Assert.AreEqual(2, result[1].Sede);

            Assert.AreEqual(3, result[2].Id);
            Assert.AreEqual(3, result[2].EstadoId);
            Assert.AreEqual(23, result[2].Anticipo);
            Assert.AreEqual("33", result[2].CUIT);
            Assert.AreEqual(ayer, result[2].FechaCarga);
            Assert.AreEqual(hoy, result[2].FechaUltimaModificacion);
            Assert.AreEqual(3, result[2].Sede);
        }

        [Test]
        public void ListarCabecerasListaVaciaNoNulaOk()
        {
            List<IngresosBrutosCoeficienteUnificado> cabeceras = new List<IngresosBrutosCoeficienteUnificado> {};

            this.repositorioMock
                .Setup(repo => repo.Listar(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(), null, 0, null, DirOrden.Asc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) =>
                    cabeceras.Select(x => proy.Compile().Invoke(x)).ToList());

            var result = target.ListarCabeceras();

            this.repositorioMock.Verify(
                repo => repo.Listar(
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()),
                Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ListarDetallesOk()
        {
            int idCabeceraTest = 921;
            
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 29);

            List<IngresosBrutosCoeficienteUnificadoDetalle> detalles = new List<IngresosBrutosCoeficienteUnificadoDetalle>
            {
                new IngresosBrutosCoeficienteUnificadoDetalle 
                {
                    Id = 1,
                    Jurisdiccion = "Capital Federal",
                    NumeroJurisdiccion = 901,
                    FechaInicio = null,
                    FechaCese = null,
                    CoeficienteGastos = 0,
                    CoeficienteIngresos = 1,
                    CoeficienteUnificado = 2,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 2,
                    Jurisdiccion = "San Juan",
                    NumeroJurisdiccion = 918,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 3,
                    Jurisdiccion = "Jujuy",
                    NumeroJurisdiccion = 910,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 923,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 4,
                    Jurisdiccion = "Tucuman",
                    NumeroJurisdiccion = 924,
                    FechaInicio = hoy,
                    FechaCese = ayer,
                    CoeficienteGastos = 0.099M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0.98989M,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                }
            };

            this.repositorioMock
                .Setup(repo => repo.Listar
                    (It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>>>(), 
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, bool>>>(),
                    0, 
                    null, 
                    DirOrden.Asc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>>, Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => 
                    detalles.Where(filtro.Compile()).Select(x => proy.Compile().Invoke(x)).ToList());

            var result = target.ListarDetalles(idCabeceraTest);

            this.repositorioMock.Verify(
                repo => repo.Listar(
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>>>(),
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()), 
                Times.Once);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Capital Federal", result[0].Jurisdiccion);
            Assert.AreEqual(901, result[0].NumeroJurisdiccion);
            Assert.IsNull(result[0].FechaInicio);
            Assert.IsNull(result[0].FechaCese);
            Assert.AreEqual(0, result[0].CoeficienteGastos);
            Assert.AreEqual(1, result[0].CoeficienteIngresos);
            Assert.AreEqual(2, result[0].CoeficienteUnificado);
            Assert.AreEqual(hoy, result[0].FechaUltimaModificacion);
            Assert.AreEqual(921, result[0].IdCabecera);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("San Juan", result[1].Jurisdiccion);
            Assert.AreEqual(918, result[1].NumeroJurisdiccion);
            Assert.AreEqual(hoy, result[1].FechaInicio);
            Assert.IsNull(result[1].FechaCese);
            Assert.AreEqual(0.1243M, result[1].CoeficienteGastos);
            Assert.AreEqual(0.0001M, result[1].CoeficienteIngresos);
            Assert.AreEqual(0, result[1].CoeficienteUnificado);
            Assert.AreEqual(ayer, result[1].FechaUltimaModificacion);
            Assert.AreEqual(921, result[1].IdCabecera);

            Assert.AreEqual(4, result[2].Id);
            Assert.AreEqual("Tucuman", result[2].Jurisdiccion);
            Assert.AreEqual(924, result[2].NumeroJurisdiccion);
            Assert.AreEqual(hoy, result[2].FechaInicio);
            Assert.AreEqual(ayer, result[2].FechaCese);
            Assert.AreEqual(0.099M, result[2].CoeficienteGastos);
            Assert.AreEqual(0.0001M, result[2].CoeficienteIngresos);
            Assert.AreEqual(0.98989M, result[2].CoeficienteUnificado);
            Assert.AreEqual(hoy, result[2].FechaUltimaModificacion);
            Assert.AreEqual(921, result[2].IdCabecera);
        }

        [Test]
        public void ListarDetallesListaVaciaNoNulaOk()
        {
            int idCabeceraTest = 931;

            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 29);

            List<IngresosBrutosCoeficienteUnificadoDetalle> detalles = new List<IngresosBrutosCoeficienteUnificadoDetalle>
            {
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 1,
                    Jurisdiccion = "Capital Federal",
                    NumeroJurisdiccion = 901,
                    FechaInicio = null,
                    FechaCese = null,
                    CoeficienteGastos = 0,
                    CoeficienteIngresos = 1,
                    CoeficienteUnificado = 2,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 2,
                    Jurisdiccion = "San Juan",
                    NumeroJurisdiccion = 918,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 3,
                    Jurisdiccion = "Jujuy",
                    NumeroJurisdiccion = 910,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 923,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 4,
                    Jurisdiccion = "Tucuman",
                    NumeroJurisdiccion = 924,
                    FechaInicio = hoy,
                    FechaCese = ayer,
                    CoeficienteGastos = 0.099M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0.98989M,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                }
            };

            this.repositorioMock
                .Setup(repo => repo.Listar
                    (It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>>>(),
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, bool>>>(),
                    0,
                    null,
                    DirOrden.Asc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>>, Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) =>
                    detalles.Where(filtro.Compile()).Select(x => proy.Compile().Invoke(x)).ToList());

            var result = target.ListarDetalles(idCabeceraTest);

            this.repositorioMock.Verify(
                repo => repo.Listar(
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>>>(),
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificadoDetalle, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()),
                Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleOk()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 27);

            timeProviderMock.Setup(t => t.Now()).Returns(hoy);

            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 3,
                Jurisdiccion = "CABA",
                NumeroJurisdiccion = 901,
                FechaCese = null,
                FechaInicio = null,
                CoeficienteGastos = 12,
                CoeficienteIngresos = 23,
                CoeficienteUnificado = 2,
                FechaUltimaModificacion = hoy,
            };

            var ingresosBrutosCoeficienteUnificadoDetalleList = new List<IngresosBrutosCoeficienteUnificadoDetalle>
            {
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 1,
                    Jurisdiccion = "Capital Federal",
                    NumeroJurisdiccion = 901,
                    FechaInicio = null,
                    FechaCese = null,
                    CoeficienteGastos = 0,
                    CoeficienteIngresos = 1,
                    CoeficienteUnificado = 2,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 2,
                    Jurisdiccion = "San Juan",
                    NumeroJurisdiccion = 918,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 3,
                    Jurisdiccion = "Jujuy",
                    NumeroJurisdiccion = 910,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 923,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 4,
                    Jurisdiccion = "Tucuman",
                    NumeroJurisdiccion = 924,
                    FechaInicio = hoy,
                    FechaCese = ayer,
                    CoeficienteGastos = 0.099M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0.98989M,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                }
            };

            IngresosBrutosCoeficienteUnificadoDetalle entidadModificada = new IngresosBrutosCoeficienteUnificadoDetalle();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificadoDetalle>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    entidadModificada = ingresosBrutosCoeficienteUnificadoDetalleList.SingleOrDefault(x => x.Id == (int)id);
                    return entidadModificada;
                });

            EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            Assert.AreEqual("El detalle de ingresos brutos se ha actualizado correctamente.", result.Mensaje);
            Assert.AreEqual(hoy, result.FechaUltimaModificacion);

            this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificadoDetalle>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            Assert.AreEqual(3, entidadModificada.Id);
            Assert.AreEqual(923, entidadModificada.IngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual(hoy, entidadModificada.FechaUltimaModificacion);

            Assert.IsNull(entidadModificada.FechaInicio);
            Assert.IsNull(entidadModificada.FechaCese);
            Assert.AreEqual(12, entidadModificada.CoeficienteGastos);
            Assert.AreEqual(23, entidadModificada.CoeficienteIngresos);
            Assert.AreEqual(2, entidadModificada.CoeficienteUnificado);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleErrorSinRegistros()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 27);

            timeProviderMock.Setup(t => t.Now()).Returns(hoy);

            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 6,
                Jurisdiccion = "CABA",
                NumeroJurisdiccion = 901,
                FechaCese = null,
                FechaInicio = null,
                CoeficienteGastos = 12,
                CoeficienteIngresos = 23,
                CoeficienteUnificado = 2,
                FechaUltimaModificacion = hoy,
            };

            var ingresosBrutosCoeficienteUnificadoDetalleList = new List<IngresosBrutosCoeficienteUnificadoDetalle>
            {
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 1,
                    Jurisdiccion = "Capital Federal",
                    NumeroJurisdiccion = 901,
                    FechaInicio = null,
                    FechaCese = null,
                    CoeficienteGastos = 0,
                    CoeficienteIngresos = 1,
                    CoeficienteUnificado = 2,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 2,
                    Jurisdiccion = "San Juan",
                    NumeroJurisdiccion = 918,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 3,
                    Jurisdiccion = "Jujuy",
                    NumeroJurisdiccion = 910,
                    FechaInicio = hoy,
                    FechaCese = null,
                    CoeficienteGastos = 0.1243M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0,
                    FechaUltimaModificacion = ayer,
                    IngresosBrutosCoeficienteUnificado_Id = 923,
                },
                new IngresosBrutosCoeficienteUnificadoDetalle
                {
                    Id = 4,
                    Jurisdiccion = "Tucuman",
                    NumeroJurisdiccion = 924,
                    FechaInicio = hoy,
                    FechaCese = ayer,
                    CoeficienteGastos = 0.099M,
                    CoeficienteIngresos = 0.0001M,
                    CoeficienteUnificado = 0.98989M,
                    FechaUltimaModificacion = hoy,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                }
            };

            IngresosBrutosCoeficienteUnificadoDetalle entidadModificada = new IngresosBrutosCoeficienteUnificadoDetalle();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificadoDetalle>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    entidadModificada = ingresosBrutosCoeficienteUnificadoDetalleList.SingleOrDefault(x => x.Id == (int)id);
                    return entidadModificada;
                });

            try
            {
                var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch(InfoCustomException icex)
            {
                this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificadoDetalle>(It.IsAny<int>()), Times.Once);
                this.repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);

                Assert.AreEqual("No se encontraron registros de detalles de coeficientes unificados ", icex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una InfoCustomException");
            }
        }

        [Test]
        public void AutorizarCabeceraOk()
        {
            int idCabeceraTest = 221;
            string mailTest = "mail";

            DateTime hoy = new DateTime(2021, 8, 2);
            DateTime ayer = new DateTime(2021, 8, 1);

            timeProviderMock.Setup(t => t.Now()).Returns(hoy);

            int pendiente = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente;
            int autorizado = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado;
            int completado = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado;

            var ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 221, EstadoIngresosBrutosCoeficienteUnificado_Id = pendiente, Consulta_Id = 132, FechaUltimaModificacion = ayer, Anticipo = 1, CUIT = "1", FechaCarga = ayer, Sede = 912 },
                new IngresosBrutosCoeficienteUnificado { Id = 222, EstadoIngresosBrutosCoeficienteUnificado_Id = pendiente, Consulta_Id = 244, FechaUltimaModificacion = hoy, Anticipo = 2, CUIT = "2", FechaCarga = ayer, Sede = 913 },
                new IngresosBrutosCoeficienteUnificado { Id = 223, EstadoIngresosBrutosCoeficienteUnificado_Id = autorizado, Consulta_Id = 323, FechaUltimaModificacion = ayer, Anticipo = 3, CUIT = "3", FechaCarga = ayer, Sede = 914 },
                new IngresosBrutosCoeficienteUnificado { Id = 224, EstadoIngresosBrutosCoeficienteUnificado_Id = completado, Consulta_Id = 466, FechaUltimaModificacion = ayer, Anticipo = 4, CUIT = "4", FechaCarga = ayer, Sede = 915 },
            };

            IngresosBrutosCoeficienteUnificado ingresosBrutosCoeficienteUnificadoAModificar = new IngresosBrutosCoeficienteUnificado();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    ingresosBrutosCoeficienteUnificadoAModificar = ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == (int)id);
                    return ingresosBrutosCoeficienteUnificadoAModificar;
                });

            List<Usuario> usuarioList = new List<Usuario>
            {
                new Usuario { Id = 1, Mail = "mail2" },
                new Usuario { Id = 2, Mail = null },
                new Usuario { Id = 3, Mail = "mail" },
            };
            this.repositorioMock
                .Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns<Expression<Func<Usuario, bool>>>(q => usuarioList.SingleOrDefault(q.Compile()));

            string result = target.AutorizarCabecera(idCabeceraTest, mailTest);

            Assert.AreEqual("Se autorizaron correctamente los coeficientes unificados de ingresos brutos", result);

            this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(repo => repo.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
            this.repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            this.consultaServiceMock.Verify(c => c.AgregarComentario(It.IsAny<int>(), It.IsAny<ComentarioDto>(), It.IsAny<System.Web.HttpFileCollectionBase>()), Times.Once);
            this.consultaServiceMock.Verify(c => c.AgregarComentario(
                132,
                It.Is<ComentarioDto>(comentarioDto => comentarioDto.Detalle == "Autorizado" && comentarioDto.Fecha == hoy && comentarioDto.UsuarioId == 3),
                null),
                Times.Once);

            Assert.AreEqual(221, ingresosBrutosCoeficienteUnificadoAModificar.Id);
            Assert.AreEqual(1, ingresosBrutosCoeficienteUnificadoAModificar.Anticipo);
            Assert.AreEqual("1", ingresosBrutosCoeficienteUnificadoAModificar.CUIT);
            Assert.AreEqual(ayer, ingresosBrutosCoeficienteUnificadoAModificar.FechaCarga);
            Assert.AreEqual(912, ingresosBrutosCoeficienteUnificadoAModificar.Sede);

            Assert.AreEqual(autorizado, ingresosBrutosCoeficienteUnificadoAModificar.EstadoIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoAModificar.FechaUltimaModificacion);
        }

        [Test]
        public void AutorizarCabeceraErrorSinRegistros()
        {
            int idCabeceraTest = 229;
            string mailTest = "mail";

            var ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 221, },
                new IngresosBrutosCoeficienteUnificado { Id = 222, },
                new IngresosBrutosCoeficienteUnificado { Id = 223, },
                new IngresosBrutosCoeficienteUnificado { Id = 224, },
            };

            IngresosBrutosCoeficienteUnificado ingresosBrutosCoeficienteUnificadoAModificar = new IngresosBrutosCoeficienteUnificado();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    ingresosBrutosCoeficienteUnificadoAModificar = ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == (int)id);
                    return ingresosBrutosCoeficienteUnificadoAModificar;
                });

            try
            {
                string result = target.AutorizarCabecera(idCabeceraTest, mailTest);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch(InfoCustomException icex)
            {
                repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
                repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);

                Assert.AreEqual("No se encontraron registros de cabecera de coeficientes unificados ", icex.Message);
            }
            catch(Exception)
            {
                Assert.Fail("Debió lanzar una InfoCustomException");
            }

        }

        [Test]
        public void ObtenerRutaArchivoFormularioCM05Ok()
        {
            int idCabeceraTest = 1332;

            var ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 1331, Archivo = new Archivo { Ruta = "ruta1331" } },
                new IngresosBrutosCoeficienteUnificado { Id = 1332, Archivo = new Archivo { Ruta = "ruta1332" } },
                new IngresosBrutosCoeficienteUnificado { Id = 1333, Archivo = new Archivo { Ruta = "ruta1333" } },
                new IngresosBrutosCoeficienteUnificado { Id = 1334, Archivo = new Archivo { Ruta = "ruta1334" } },
            };
            repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<int>(idCabecera => ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == idCabecera));

            var result = target.ObtenerRutaArchivoFormularioCM05(idCabeceraTest);

            Assert.AreEqual("ruta1332", result);
            this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ObtenerRutaArchivoFormularioCM05Exception()
        {
            int idCabeceraTest = 1332;

            Exception exceptionTest = new NullReferenceException("random exception");
            repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Throws(exceptionTest);

            try
            {
                var result = target.ObtenerRutaArchivoFormularioCM05(idCabeceraTest);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch(NullReferenceException nrex)
            {
                Assert.AreEqual(exceptionTest, nrex);
                this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
            }
            catch(Exception)
            {
                Assert.Fail("Debió lanzar una NullReferenceException");
            }
        }
    }
}