
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
    public class ActualizarLocalidadesPartidosTest
    {

        private Mock<IRepositorio> repositorio;
        private Mock<IDataAgroApiService> dataAgroApiService;

        private IActualizarLocalidadesPartidos actualizarLocalidadesPartidos;

        private string NombreHabilitacion = "ActualizarLocalidades";

        private HabilitacionJob habilitacionJob;

        private LocalidadDto localidadDA1;
        private LocalidadDto localidadDA2;

        private Localidad localidadGuardada;

        private PartidoDto partidoDA1;
        private PartidoDto partidoDA2;

        private Partido partidoGuardado;

        [SetUp]
        public void Setup()
        {
            repositorio = new Mock<IRepositorio>();
            dataAgroApiService = new Mock<IDataAgroApiService>();
            habilitacionJob = new HabilitacionJob { Nombre = NombreHabilitacion };

            actualizarLocalidadesPartidos = new ActualizarLocalidadesPartidos(
                repositorio.Object,
                dataAgroApiService.Object
                );

            localidadDA1 = new LocalidadDto { LocalidadId = 1, CodLocalidad = "1", PartidoId = 1, ProvinciaId = 1 };
            localidadDA2 = new LocalidadDto { LocalidadId = 2, CodLocalidad = "2", PartidoId = 2, ProvinciaId = 2 };
            localidadGuardada = new Localidad { CodLocalidad = localidadDA1.LocalidadId };

            partidoDA1 = new PartidoDto { Id = 1, ProvinciaId = 1, Descripcion="Partido DA 1"};
            partidoDA2 = new PartidoDto { Id = 2, ProvinciaId = 2, Descripcion = "Partido DA 1" };
            partidoGuardado = new Partido { Id = partidoDA1.Id };

        }

        [Test]
        public void Execute_VerificarHabilitacion_Deshabilitado()
        {
            SetHabilitacion(false);

            actualizarLocalidadesPartidos.Execute();

            Assert.That(!actualizarLocalidadesPartidos.Habilitado());
        }

        [Test]
        public void Execute_VerificarHabilitacion_Habilitado()
        {
            SetHabilitacion(true);

            actualizarLocalidadesPartidos.Execute();

            Assert.That(actualizarLocalidadesPartidos.Habilitado());
        }

        [Test]
        public void Execute_LocalidadesSinExistir_LasAgrega()
        {
            SetHabilitacion(true);

            SetListaLocalidadesGuardadas(new List<Localidad> { }.AsQueryable());
            SetLocalidadesRespuestaDA(new List<LocalidadDto> { localidadDA1, localidadDA2 });

            SetPartidosRespuestaDA(new List<PartidoDto> { });
            SetListaPartidosGuardados(new List<Partido> { }.AsQueryable());


            actualizarLocalidadesPartidos.Execute();

            repositorio.Verify(rep => rep.Agregar(It.IsAny<Localidad>()), Times.Exactly(2));
        }

        [Test]
        public void Execute_AlgunasLocalidadesExisten_AgregaLasQueFalta()
        {
            SetHabilitacion(true);

            SetListaLocalidadesGuardadas(new List<Localidad> { localidadGuardada }.AsQueryable());
            SetLocalidadesRespuestaDA(new List<LocalidadDto> { localidadDA1, localidadDA2 });

            SetPartidosRespuestaDA(new List<PartidoDto> { });
            SetListaPartidosGuardados(new List<Partido> { }.AsQueryable());

            actualizarLocalidadesPartidos.Execute();

            repositorio.Verify(rep => rep.Agregar(It.IsAny<Localidad>()), Times.Exactly(1));
        }

        [Test]
        public void Execute_AlgunasLocalidadesExisten_ModificaLaQueExiste()
        {
            SetHabilitacion(true);
            localidadGuardada.ProvinciaId = 20;
            localidadGuardada.PartidoId = 20;

            SetListaLocalidadesGuardadas(new List<Localidad> { localidadGuardada }.AsQueryable());
            SetLocalidadesRespuestaDA(new List<LocalidadDto> { localidadDA1, localidadDA2 });

            SetPartidosRespuestaDA(new List<PartidoDto> { });
            SetListaPartidosGuardados(new List<Partido> { }.AsQueryable());


            actualizarLocalidadesPartidos.Execute();

            Assert.That(localidadGuardada.ProvinciaId == localidadDA1.ProvinciaId);
            Assert.That(localidadGuardada.PartidoId == localidadDA1.PartidoId);
        }

        [Test]
        public void Execute_PartidosSinExistir_LosAgrega()
        {
            SetHabilitacion(true);

            SetListaLocalidadesGuardadas(new List<Localidad> { }.AsQueryable());
            SetLocalidadesRespuestaDA(new List<LocalidadDto> {  });

            SetListaPartidosGuardados(new List<Partido> { }.AsQueryable());
            SetPartidosRespuestaDA(new List<PartidoDto> { partidoDA1, partidoDA2 });


            actualizarLocalidadesPartidos.Execute();

            repositorio.Verify(rep => rep.Agregar(It.IsAny<Partido>()), Times.Exactly(2));
        }

        [Test]
        public void Execute_AlgunosPartidosExisten_AgregaLosQueFalten()
        {
            SetHabilitacion(true);

            SetListaLocalidadesGuardadas(new List<Localidad> { }.AsQueryable());
            SetLocalidadesRespuestaDA(new List<LocalidadDto> { });

            SetListaPartidosGuardados(new List<Partido> { partidoGuardado }.AsQueryable());
            SetPartidosRespuestaDA(new List<PartidoDto> { partidoDA1, partidoDA2 });


            actualizarLocalidadesPartidos.Execute();

            repositorio.Verify(rep => rep.Agregar(It.IsAny<Partido>()), Times.Exactly(1));
        }

        [Test]
        public void Execute_AlgunosPartidosExisten_ModificaElQueExiste()
        {
            SetHabilitacion(true);
            partidoGuardado.ProvinciaID = 999;
            partidoGuardado.Descripcion = "Cambia";

            SetListaLocalidadesGuardadas(new List<Localidad> { }.AsQueryable());
            SetLocalidadesRespuestaDA(new List<LocalidadDto> { });

            SetListaPartidosGuardados(new List<Partido> { partidoGuardado }.AsQueryable());
            SetPartidosRespuestaDA(new List<PartidoDto> { partidoDA1, partidoDA2 });


            actualizarLocalidadesPartidos.Execute();

            Assert.That(partidoGuardado.ProvinciaID == partidoDA1.ProvinciaId);
            Assert.That(partidoGuardado.Descripcion == partidoDA1.Descripcion);
        }

        private void SetListaLocalidadesGuardadas(IQueryable<Localidad> localidades)
        {
            repositorio.Setup(r=>r.ListarTodos<Localidad>()).Returns(localidades);
        }

        private void SetListaPartidosGuardados(IQueryable<Partido> partidos)
        {
            repositorio.Setup(r => r.ListarTodos<Partido>()).Returns(partidos);
        }

        private void SetLocalidadesRespuestaDA(List<LocalidadDto> localidadesDA)
        {
            dataAgroApiService.Setup(das => das.ListarLocalidades()).Returns(
                localidadesDA
                );
        }

        private void SetPartidosRespuestaDA(List<PartidoDto> partidosDA)
        {
            dataAgroApiService.Setup(das => das.ListarPartidos()).Returns(
                partidosDA
                );
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
