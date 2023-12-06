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
        private Mock<IAzureService> azureServiceMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            timeProviderMock = new Mock<ITimeProvider>();
            azureServiceMock = new Mock<IAzureService>();

            target = new GestionImpuestosService(repositorioMock.Object, timeProviderMock.Object, azureServiceMock.Object);
        }

        [Test]
        public void ListarCabecerasOk()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 29);

            EstadoIngresosBrutosCoeficienteUnificado pendiente = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() };
            EstadoIngresosBrutosCoeficienteUnificado autorizado = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() };
            EstadoIngresosBrutosCoeficienteUnificado completado = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Completado.ToFriendlyString() };

            SecuenciaIngresosBrutosCoeficienteUnificado secuencia1 = new SecuenciaIngresosBrutosCoeficienteUnificado { Id = 1, Descripcion = "secuencia1" };
            SecuenciaIngresosBrutosCoeficienteUnificado secuencia2 = new SecuenciaIngresosBrutosCoeficienteUnificado { Id = 2, Descripcion = "secuencia2" };

            List<IngresosBrutosCoeficienteUnificado> cabeceras = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 1, RazonSocial = "razon social 1", Consulta_Id = 1, SecuenciaIngresosBrutosCoeficienteUnificado_Id = 1, SecuenciaIngresosBrutosCoeficienteUnificado = secuencia1, Anticipo = 1, CUIT = "1", Sede = 1, FechaCarga = hoy, FechaUltimaModificacion = ayer, EstadoIngresosBrutosCoeficienteUnificado = pendiente, MalCargada = false },
                new IngresosBrutosCoeficienteUnificado { Id = 2, RazonSocial = "razon social 2", Consulta_Id = 2, SecuenciaIngresosBrutosCoeficienteUnificado_Id = 2, SecuenciaIngresosBrutosCoeficienteUnificado = secuencia2, Anticipo = 2, CUIT = "2", Sede = 2, FechaCarga = hoy, FechaUltimaModificacion = ayer, EstadoIngresosBrutosCoeficienteUnificado = autorizado, MalCargada = false },
                new IngresosBrutosCoeficienteUnificado { Id = 3, RazonSocial = "razon social 3", Consulta_Id = 3, SecuenciaIngresosBrutosCoeficienteUnificado_Id = null, Anticipo = 23, CUIT = "33", Sede = 3, FechaCarga = ayer, FechaUltimaModificacion = hoy, EstadoIngresosBrutosCoeficienteUnificado = completado, MalCargada = true },
            };

            this.repositorioMock
                .Setup(repo => repo.Listar(
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>(),
                    0,
                    "Id",
                    DirOrden.Desc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResult, prop, dirOrden) =>
                    cabeceras.Select(x => proy.Compile().Invoke(x)).OrderByDescending(x => x.Id).ToList());

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

            Assert.AreEqual(1, result[2].Id);
            Assert.AreEqual(1, result[2].EstadoId);
            Assert.AreEqual(1, result[2].Anticipo);
            Assert.AreEqual("1", result[2].CUIT);
            Assert.AreEqual(hoy, result[2].FechaCarga);
            Assert.AreEqual(ayer, result[2].FechaUltimaModificacion);
            Assert.AreEqual(1, result[2].Sede);
            Assert.AreEqual(1, result[2].SecuenciaId);
            Assert.IsFalse(result[2].MalCargada);
            Assert.AreEqual(1, result[2].ConsultaId);
            Assert.AreEqual("razon social 1", result[2].RazonSocial);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual(2, result[1].EstadoId);
            Assert.AreEqual(2, result[1].Anticipo);
            Assert.AreEqual("2", result[1].CUIT);
            Assert.AreEqual(hoy, result[1].FechaCarga);
            Assert.AreEqual(ayer, result[1].FechaUltimaModificacion);
            Assert.AreEqual(2, result[1].Sede);
            Assert.AreEqual(2, result[1].SecuenciaId);
            Assert.IsFalse(result[1].MalCargada);
            Assert.AreEqual(2, result[1].ConsultaId);
            Assert.AreEqual("razon social 2", result[1].RazonSocial);

            Assert.AreEqual(3, result[0].Id);
            Assert.AreEqual(3, result[0].EstadoId);
            Assert.AreEqual(23, result[0].Anticipo);
            Assert.AreEqual("33", result[0].CUIT);
            Assert.AreEqual(ayer, result[0].FechaCarga);
            Assert.AreEqual(hoy, result[0].FechaUltimaModificacion);
            Assert.AreEqual(3, result[0].Sede);
            Assert.IsNull(result[0].SecuenciaId);
            Assert.IsTrue(result[0].MalCargada);
            Assert.AreEqual(3, result[0].ConsultaId);
            Assert.AreEqual("razon social 3", result[0].RazonSocial);
        }

        [Test]
        public void ListarCabecerasListaVaciaNoNulaOk()
        {
            List<IngresosBrutosCoeficienteUnificado> cabeceras = new List<IngresosBrutosCoeficienteUnificado> { };

            this.repositorioMock
                .Setup(repo => repo.Listar(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(), null, 0, "Id", DirOrden.Desc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) =>
                    cabeceras.Select(x => proy.Compile().Invoke(x)).OrderByDescending(x => x.Id).ToList());

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

            Assert.AreEqual("El detalle de coeficientes unificados de ingresos brutos se ha actualizado correctamente.", result.Mensaje);
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
            catch (InfoCustomException icex)
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
            catch (InfoCustomException icex)
            {
                repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
                repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);

                Assert.AreEqual("No se encontraron registros de cabecera de coeficientes unificados ", icex.Message);
            }
            catch (Exception)
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
            catch (NullReferenceException nrex)
            {
                Assert.AreEqual(exceptionTest, nrex);
                this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una NullReferenceException");
            }
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoOk()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 27);

            timeProviderMock.Setup(t => t.Now()).Returns(hoy);

            var ingresosBrutosCoeficienteUnificadoDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 3,
                Anticipo = 201,
                CUIT = "cuitcuilt",
                EstadoId = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                FechaCarga = ayer,
                Sede = 123,
                SecuenciaId = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original,
                RazonSocial = "razon social numero 3",
                MalCargada = false,
                FechaUltimaModificacion = hoy,
                ConsultaId = 333,
            };

            var ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 1,
                    Archivo_Id = 12,
                    Consulta_Id = 3,
                    Anticipo = 202,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                    RazonSocial = "razon social 1",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa,
                    MalCargada = true,
                    FechaUltimaModificacion = hoy,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 2,
                    Archivo_Id = 12,
                    Consulta_Id = 663,
                    Anticipo = 2012,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    RazonSocial = "razon social 2",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa,
                    MalCargada = false,
                    FechaUltimaModificacion = ayer,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 3,
                    Archivo_Id = 62,
                    Consulta_Id = 334,
                    Anticipo = 2012,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    RazonSocial = "razon social 3",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = null,
                    MalCargada = true,
                    FechaUltimaModificacion = ayer,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 4,
                    Archivo_Id = 122,
                    Consulta_Id = 4,
                    Anticipo = 201,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado,
                    RazonSocial = "razon social 4",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original,
                    MalCargada = false,
                    FechaUltimaModificacion = hoy,
                }
            };
            EstadoIngresosBrutosCoeficienteUnificado estado = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() };
            EstadoIngresosBrutosCoeficienteUnificadoDto estadoDto = new EstadoIngresosBrutosCoeficienteUnificadoDto(estado);

            IngresosBrutosCoeficienteUnificado entidadModificada = new IngresosBrutosCoeficienteUnificado();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    entidadModificada = ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == (int)id);
                    return entidadModificada;
                });

            this.repositorioMock
                .Setup(repo => repo.Obtener<EstadoIngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    return estado;
                });

            EditarIngresosBrutosCoeficienteUnificadoResponseDto result = target.EditarIngresosBrutosCoeficienteUnificado(ingresosBrutosCoeficienteUnificadoDtoTest);

            Assert.AreEqual("El detalle de coeficientes unificados de ingresos brutos se ha actualizado correctamente.", result.Mensaje);
            Assert.AreEqual(hoy, result.FechaUltimaModificacion);

            this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(repo => repo.Obtener<EstadoIngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            Assert.AreEqual(3, entidadModificada.Id);
            Assert.AreEqual(62, entidadModificada.Archivo_Id);
            Assert.AreEqual(334, entidadModificada.Consulta_Id);
            Assert.AreEqual(ayer, entidadModificada.FechaCarga);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, entidadModificada.EstadoIngresosBrutosCoeficienteUnificado_Id);

            Assert.AreEqual(hoy, entidadModificada.FechaUltimaModificacion);

            Assert.AreEqual(201, entidadModificada.Anticipo);
            Assert.AreEqual("cuitcuilt", entidadModificada.CUIT);
            Assert.AreEqual(123, entidadModificada.Sede);
            Assert.AreEqual((int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original, entidadModificada.SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("razon social numero 3", entidadModificada.RazonSocial);
            Assert.IsFalse(entidadModificada.MalCargada);

            Assert.AreEqual(estadoDto.Id, result.estadoCabecera.Id);
            Assert.AreEqual(estadoDto.Descripcion, result.estadoCabecera.Descripcion);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoSecuenciaNulaOk()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 27);

            timeProviderMock.Setup(t => t.Now()).Returns(hoy);

            var ingresosBrutosCoeficienteUnificadoDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 3,
                Anticipo = 201,
                CUIT = "cuitcuilt",
                EstadoId = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                RazonSocial = "razon social 3",
                FechaCarga = ayer,
                Sede = 123,
                SecuenciaId = null,
                MalCargada = false,
                FechaUltimaModificacion = hoy,
            };

            var ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 1,
                    Archivo_Id = 12,
                    Consulta_Id = 3,
                    Anticipo = 202,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                    RazonSocial = "razon social 1",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa,
                    MalCargada = true,
                    FechaUltimaModificacion = hoy,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 2,
                    Archivo_Id = 12,
                    Consulta_Id = 663,
                    Anticipo = 2012,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    RazonSocial = "razon social 2",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original,
                    MalCargada = false,
                    FechaUltimaModificacion = ayer,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 3,
                    Archivo_Id = 62,
                    Consulta_Id = 334,
                    Anticipo = 2012,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    RazonSocial = "razon social 3",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa,
                    MalCargada = true,
                    FechaUltimaModificacion = ayer,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 4,
                    Archivo_Id = 122,
                    Consulta_Id = 4,
                    Anticipo = 201,
                    CUIT = "cuit",
                    EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado,
                    RazonSocial = "razon social 4",
                    FechaCarga = ayer,
                    Sede = 123,
                    SecuenciaIngresosBrutosCoeficienteUnificado_Id = (int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original,
                    MalCargada = false,
                    FechaUltimaModificacion = hoy,
                }
            };

            EstadoIngresosBrutosCoeficienteUnificado estado = new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() };
            EstadoIngresosBrutosCoeficienteUnificadoDto estadoDto = new EstadoIngresosBrutosCoeficienteUnificadoDto(estado);

            IngresosBrutosCoeficienteUnificado entidadModificada = new IngresosBrutosCoeficienteUnificado();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    entidadModificada = ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == (int)id);
                    return entidadModificada;
                });

            this.repositorioMock
                .Setup(repo => repo.Obtener<EstadoIngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    return estado;
                });

            EditarIngresosBrutosCoeficienteUnificadoResponseDto result = target.EditarIngresosBrutosCoeficienteUnificado(ingresosBrutosCoeficienteUnificadoDtoTest);

            Assert.AreEqual("El detalle de coeficientes unificados de ingresos brutos se ha actualizado correctamente.", result.Mensaje);
            Assert.AreEqual(hoy, result.FechaUltimaModificacion);

            this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            Assert.AreEqual(3, entidadModificada.Id);
            Assert.AreEqual(62, entidadModificada.Archivo_Id);
            Assert.AreEqual(334, entidadModificada.Consulta_Id);
            Assert.AreEqual(ayer, entidadModificada.FechaCarga);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, entidadModificada.EstadoIngresosBrutosCoeficienteUnificado_Id);

            Assert.AreEqual(hoy, entidadModificada.FechaUltimaModificacion);

            Assert.AreEqual(201, entidadModificada.Anticipo);
            Assert.AreEqual("cuitcuilt", entidadModificada.CUIT);
            Assert.AreEqual(123, entidadModificada.Sede);
            Assert.IsNull(entidadModificada.SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.IsFalse(entidadModificada.MalCargada);

            Assert.AreEqual(estadoDto.Id, result.estadoCabecera.Id);
            Assert.AreEqual(estadoDto.Descripcion, result.estadoCabecera.Descripcion);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoErrorSinRegistros()
        {
            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 27);

            timeProviderMock.Setup(t => t.Now()).Returns(hoy);

            var ingresosBrutosCoeficienteUnificadoDtoTest = new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = 6,
                FechaUltimaModificacion = hoy,
            };

            var ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 1,
                    FechaUltimaModificacion = hoy,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 2,
                    FechaUltimaModificacion = ayer,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 3,
                    FechaUltimaModificacion = ayer,
                },
                new IngresosBrutosCoeficienteUnificado
                {
                    Id = 4,
                    FechaUltimaModificacion = hoy,
                }
            };

            IngresosBrutosCoeficienteUnificado entidadModificada = new IngresosBrutosCoeficienteUnificado();
            this.repositorioMock
                .Setup(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()))
                .Returns<object>(id =>
                {
                    entidadModificada = ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == (int)id);
                    return entidadModificada;
                });

            try
            {
                var result = target.EditarIngresosBrutosCoeficienteUnificado(ingresosBrutosCoeficienteUnificadoDtoTest);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (InfoCustomException icex)
            {
                this.repositorioMock.Verify(repo => repo.Obtener<IngresosBrutosCoeficienteUnificado>(It.IsAny<int>()), Times.Once);
                this.repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);

                Assert.AreEqual("No se encontraron registros de coeficientes unificados ", icex.Message);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una InfoCustomException");
            }
        }

        [Test]
        public void ListarMovimientosOk()
        {
            int idCabeceraTest = 921;

            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 29);

            List<MovimientoIngresosBrutosCoeficienteUnificado> movimientos = new List<MovimientoIngresosBrutosCoeficienteUnificado>
            {
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 1,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.Autorizacion,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 23,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.AutorizacionRevertida,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 33,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.Autorizacion,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 4343,
                    IngresosBrutosCoeficienteUnificado_Id = 922,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.SAP,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.Error,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 233333,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = hoy,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.ExportacionExitosa,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado,
                },
            };

            this.repositorioMock
                .Setup(repo => repo.Listar
                    (It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, bool>>>(),
                    0,
                    null,
                    DirOrden.Asc))
                .Returns<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) =>
                    movimientos.Where(filtro.Compile()).Select(x => proy.Compile().Invoke(x)).ToList());

            var result = target.ListarMovimientos(idCabeceraTest);

            this.repositorioMock.Verify(
                repo => repo.Listar(
                    It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()),
                Times.Once);

            Assert.AreEqual(4, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(idCabeceraTest, result[0].IngresosBrutosCoeficienteUnificado.Id);
            Assert.AreEqual(ayer, result[0].Fecha);
            Assert.AreEqual("Observaciones che", result[0].Observaciones);
            Assert.AreEqual(1, result[0].OrigenId);
            Assert.AreEqual(2, result[0].TipoId);
            Assert.AreEqual(1, result[0].EstadoAnteriorId);
            Assert.AreEqual(2, result[0].EstadoPosteriorId);

            Assert.AreEqual(23, result[1].Id);
            Assert.AreEqual(idCabeceraTest, result[1].IngresosBrutosCoeficienteUnificado.Id);
            Assert.AreEqual(ayer, result[1].Fecha);
            Assert.AreEqual("Observaciones che", result[1].Observaciones);
            Assert.AreEqual(1, result[1].OrigenId);
            Assert.AreEqual(3, result[1].TipoId);
            Assert.AreEqual(2, result[1].EstadoAnteriorId);
            Assert.AreEqual(1, result[1].EstadoPosteriorId);

            Assert.AreEqual(33, result[2].Id);
            Assert.AreEqual(idCabeceraTest, result[2].IngresosBrutosCoeficienteUnificado.Id);
            Assert.AreEqual(ayer, result[2].Fecha);
            Assert.AreEqual("Observaciones che", result[2].Observaciones);
            Assert.AreEqual(1, result[2].OrigenId);
            Assert.AreEqual(2, result[2].TipoId);
            Assert.AreEqual(1, result[2].EstadoAnteriorId);
            Assert.AreEqual(2, result[2].EstadoPosteriorId);

            Assert.AreEqual(233333, result[3].Id);
            Assert.AreEqual(idCabeceraTest, result[3].IngresosBrutosCoeficienteUnificado.Id);
            Assert.AreEqual(hoy, result[3].Fecha);
            Assert.AreEqual("Observaciones che", result[3].Observaciones);
            Assert.AreEqual(1, result[3].OrigenId);
            Assert.AreEqual(4, result[3].TipoId);
            Assert.AreEqual(2, result[3].EstadoAnteriorId);
            Assert.AreEqual(3, result[3].EstadoPosteriorId);
        }

        [Test]
        public void ListarMovimientosListaVaciaNoNulaOk()
        {
            int idCabeceraTest = 931;

            DateTime hoy = new DateTime(2021, 07, 28);
            DateTime ayer = new DateTime(2021, 07, 29);

            List<MovimientoIngresosBrutosCoeficienteUnificado> movimientos = new List<MovimientoIngresosBrutosCoeficienteUnificado>
            {
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 1,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.Autorizacion,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 23,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.AutorizacionRevertida,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 33,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.Autorizacion,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 4343,
                    IngresosBrutosCoeficienteUnificado_Id = 922,
                    Fecha = ayer,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.SAP,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.Error,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                },
                new MovimientoIngresosBrutosCoeficienteUnificado
                {
                    Id = 233333,
                    IngresosBrutosCoeficienteUnificado_Id = 921,
                    Fecha = hoy,
                    Observaciones = "Observaciones che",
                    OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumOrigenIngresosBrutosCoeficienteUnificado.WEB,
                    TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = (int)EnumTipoIngresosBrutosCoeficienteUnificado.ExportacionExitosa,
                    EstadoAnterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado,
                    EstadoPosterior_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Completado,
                },
            };

            this.repositorioMock
                .Setup(repo => repo.Listar
                    (It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, bool>>>(),
                    0,
                    null,
                    DirOrden.Asc))
                .Returns<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) =>
                    movimientos.Where(filtro.Compile()).Select(x => proy.Compile().Invoke(x)).ToList());

            var result = target.ListarMovimientos(idCabeceraTest);

            this.repositorioMock.Verify(
                repo => repo.Listar(
                    It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>>>(),
                    It.IsAny<Expression<Func<MovimientoIngresosBrutosCoeficienteUnificado, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>()),
                Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ListarEstados()
        {
            var estados = new List<EstadoIngresosBrutosCoeficienteUnificado>
            {
                  new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                new EstadoIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, object>>>>()))
               .Returns(estados);

            List<EstadoIngresosBrutosCoeficienteUnificadoDto> expected = new List<EstadoIngresosBrutosCoeficienteUnificadoDto>() { };
            estados.ForEach(e => expected.Add(new EstadoIngresosBrutosCoeficienteUnificadoDto(e)));
            expected.OrderBy(x => x.Descripcion);

            var result = target.ListarEstados();

            repositorioMock.Verify(
                repo => repo.Listar(It.IsAny<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, object>>>>()),
                Times.Once);

            Assert.AreEqual(result.Count, expected.Count);
            Assert.AreEqual(result[0].Descripcion, expected[0].Descripcion);
            Assert.AreEqual(result[0].Id, expected[0].Id);
            Assert.AreEqual(result[1].Descripcion, expected[1].Descripcion);
            Assert.AreEqual(result[1].Id, expected[1].Id);
        }

        [Test]
        public void ListarEstadosVacios()
        {
            Exception exceptionTest = new NullReferenceException("random exception");

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, object>>>>()))
               .Throws(exceptionTest);

            try
            {
                var result = target.ListarEstados();
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (NullReferenceException nrex)
            {
                Assert.AreEqual(exceptionTest, nrex);
                repositorioMock.Verify(repo => repo.Listar(It.IsAny<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<EstadoIngresosBrutosCoeficienteUnificado, object>>>>()), Times.Once);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una NullReferenceException");
            }
        }

        [Test]
        public void ListarSecuenciaIngresosBrutosCoeficientesUnificador()
        {
            var secuencias = new List<SecuenciaIngresosBrutosCoeficienteUnificado>
            {
                  new SecuenciaIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado.ToFriendlyString() },
                  new SecuenciaIngresosBrutosCoeficienteUnificado { Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, Descripcion = EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente.ToFriendlyString() },
            };

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, object>>>>()))
               .Returns(secuencias);

            List<SecuenciaIngresosBrutosCoeficienteUnificadoDto> expected = new List<SecuenciaIngresosBrutosCoeficienteUnificadoDto>() { };
            secuencias.ForEach(e => expected.Add(new SecuenciaIngresosBrutosCoeficienteUnificadoDto(e)));
            expected.OrderBy(x => x.Descripcion);

            var result = target.ListarSecuenciaIngresosBrutosCoeficientesUnificador();

            repositorioMock.Verify(
                repo => repo.Listar(It.IsAny<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, object>>>>()),
                Times.Once);

            Assert.AreEqual(result.Count, expected.Count);
            Assert.AreEqual(result[0].Descripcion, expected[0].Descripcion);
            Assert.AreEqual(result[0].Id, expected[0].Id);
            Assert.AreEqual(result[1].Descripcion, expected[1].Descripcion);
            Assert.AreEqual(result[1].Id, expected[1].Id);
        }

        [Test]
        public void ListarSecuenciaIngresosBrutosCoeficientesUnificadorExcepcion()
        {
            Exception exceptionTest = new NullReferenceException("random exception");

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, object>>>>()))
               .Throws(exceptionTest);

            try
            {
                var result = target.ListarSecuenciaIngresosBrutosCoeficientesUnificador();
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (NullReferenceException nrex)
            {
                Assert.AreEqual(exceptionTest, nrex);
                repositorioMock.Verify(repo => repo.Listar(It.IsAny<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<SecuenciaIngresosBrutosCoeficienteUnificado, object>>>>()), Times.Once);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una NullReferenceException");
            }
        }

    }
}