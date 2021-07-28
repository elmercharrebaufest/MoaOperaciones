using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
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

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            timeProviderMock = new Mock<ITimeProvider>();

            target = new GestionImpuestosService(repositorioMock.Object, timeProviderMock.Object);
        }

        [Test]
        public void Test()
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
            };

            this.repositorioMock
                .Setup(repo => repo.Listar(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>>(), null, 0, null, DirOrden.Asc))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>>, Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>, int, string, DirOrden>
                    ((proy, filtro, maxResultados, orden, dirOrden) => 
                    cabeceras.Where(x => filtro.Compile().Invoke(x))
                             .Select(x => proy.Compile().Invoke(x)).ToList());

             target.ListarCabeceras();
        }
    }
}