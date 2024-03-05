
using Moq;
using NUnit.Framework;
using SustitucionMOA.Jobs;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOATest.Jobs
{
    [TestFixture]
    public class ActualizarLocalidadesTest
    {

        private Mock<IRepositorio> repositorio;
        private Mock<IDataAgroApiService> dataAgroApiService;

        private IActualizarLocalidades actualizarLocalidades;

        private string NombreHabilitacion = "ActualizarLocalidades";

        private HabilitacionJob habilitacionJob;

        private LocalidadDto localidadDA1;
        private LocalidadDto localidadDA2;

        private Localidad localidadGuardada;

        [SetUp]
        public void Setup()
        {
            repositorio = new Mock<IRepositorio>();
            dataAgroApiService = new Mock<IDataAgroApiService>();
            habilitacionJob = new HabilitacionJob { Nombre = NombreHabilitacion };

            actualizarLocalidades = new ActualizarLocalidades(
                repositorio.Object,
                dataAgroApiService.Object
                );

            localidadDA1 = new LocalidadDto { LocalidadId = 1, CodLocalidad = "1", PartidoId = 1, ProvinciaId = 1 };
            localidadDA2 = new LocalidadDto { LocalidadId = 2, CodLocalidad = "2", PartidoId = 2, ProvinciaId = 2 };
            localidadGuardada = new Localidad { CodLocalidad = localidadDA1.LocalidadId };
        }

        [Test]
        public void Execute_VerificarHabilitacion_Deshabilitado()
        {
            SetHabilitacion(false);

            actualizarLocalidades.Execute();

            Assert.That(!actualizarLocalidades.Habilitado());
        }

        [Test]
        public void Execute_VerificarHabilitacion_Habilitado()
        {
            SetHabilitacion(true);

            actualizarLocalidades.Execute();

            Assert.That(actualizarLocalidades.Habilitado());
        }

        [Test]
        public void Execute_LocalidadesSinExistir_LasAgrega()
        {
            SetHabilitacion(true);

            SetListaLocalidadesGuardadas(new List<Localidad> { }.AsQueryable());

            dataAgroApiService.Setup(das => das.ListarLocalidades()).Returns(
                new List<LocalidadDto> {  localidadDA1, localidadDA2} 
                );


            actualizarLocalidades.Execute();

            repositorio.Verify(rep => rep.Agregar(It.IsAny<Localidad>()), Times.Exactly(2));
        }

        [Test]
        public void Execute_AlgunasLocalidadesExisten_AgregaLasQueFalta()
        {
            SetHabilitacion(true);
            SetListaLocalidadesGuardadas(new List<Localidad> { localidadGuardada }.AsQueryable());

            dataAgroApiService.Setup(das => das.ListarLocalidades()).Returns(
                new List<LocalidadDto> { localidadDA1, localidadDA2 }
                );


            actualizarLocalidades.Execute();

            repositorio.Verify(rep => rep.Agregar(It.IsAny<Localidad>()), Times.Exactly(1));
        }

        [Test]
        public void Execute_AlgunasLocalidadesExisten_ModificaLaQueExiste()
        {
            SetHabilitacion(true);
            localidadGuardada.ProvinciaId = 20;
            localidadGuardada.PartidoId = 20;

            SetListaLocalidadesGuardadas(new List<Localidad> { localidadGuardada }.AsQueryable());

            dataAgroApiService.Setup(das => das.ListarLocalidades()).Returns(
                new List<LocalidadDto> { localidadDA1, localidadDA2 }
                );


            actualizarLocalidades.Execute();

            Assert.That(localidadGuardada.ProvinciaId == localidadDA1.ProvinciaId);
            Assert.That(localidadGuardada.PartidoId == localidadDA1.PartidoId);
        }

        private void SetListaLocalidadesGuardadas(IQueryable<Localidad> localidades)
        {
            repositorio.Setup(r=>r.ListarTodos<Localidad>()).Returns(localidades);
        }

        private void SetHabilitacion(bool estado)
        {
            habilitacionJob.Habilitado = estado;
            repositorio.Setup(r => r.Obtener(It.IsAny<
                Expression<Func<HabilitacionJob, bool>>>()))
                .Returns(habilitacionJob);
        }
    }
}
