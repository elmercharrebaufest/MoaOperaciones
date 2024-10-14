using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class ExternalApiOrdenesResiduosServiceTest
    {
        private Mock<IRepositorio> repositorio { get; set; }
        private IExternalApiOrdenesResiduosService service { get; set; }
        [SetUp]
        public void Setup()
        {
            repositorio = new Mock<IRepositorio>();
            service = new ExternalApiOrdenesResiduosService(repositorio.Object);
        }
        [Test]
        public void ObtenerOrdenes_FiltraPorPatente()
        {
            var patente1 = "123456";
            var patente2 = "654321";

            Expression<Func<OrdenResiduosApiDto, bool>> filter1 =
                or => string.IsNullOrEmpty(patente1) || or.PatenteChasis == patente1;

            Expression<Func<OrdenResiduosApiDto, bool>> filter2 =
                or => string.IsNullOrEmpty(patente2) || or.PatenteChasis == patente2;

            var listaRaw1 = new OrdenResiduos[]
            {
                CrearOrden(patente1),
                CrearOrden(patente1),
                CrearOrden(patente1),
            };

            var listaRaw2 = new OrdenResiduos[]
            {
                CrearOrden(patente2),
                CrearOrden(patente2),
                CrearOrden(patente2),
            };

            SetupRespuestaLista(listaRaw1);

            var result1 = service.ObtenerOrdenes(patente1);

            Assert.That(result1.All(or => filter1.Compile()(or)));


            SetupRespuestaLista(listaRaw2);

            var result2 = service.ObtenerOrdenes(patente1);

            Assert.That(result2.All(or => filter2.Compile()(or)));
        }
        [Test]
        [TestCase(FlujoActualizacionOrdenResiduos.Ingreso,EstadoOrdenResiduosEnum.Ingresada)]
        [TestCase(FlujoActualizacionOrdenResiduos.Rechazo, EstadoOrdenResiduosEnum.Rechazada)]
        [TestCase(FlujoActualizacionOrdenResiduos.Salida, EstadoOrdenResiduosEnum.Retirada)]
        public void ActualizarOrden_DatosCorrectos_ActulizarOrdenAcordeFlujo(FlujoActualizacionOrdenResiduos flujo, EstadoOrdenResiduosEnum estadoFinalEsperado)
        {
            var id = 1;
            var orden = CrearOrden("123456");
            SetupObtencionOrden(id, orden);

            service.ActualizarOrden(new ActualizarOrdenResiduosExternalDto { TipoActualizacion = flujo, Id=id });

            Assert.That(orden.EstadoId == (int)estadoFinalEsperado);
        }
        [Test]
        public void ActualizarOrden_OrdenSinEncontrar_ThrowException()
        {
            var id = 1;
            SetupObtencionOrden(id, null);

            Assert.That(
                ()=> service.ActualizarOrden(new ActualizarOrdenResiduosExternalDto { Id = id }), 
                Throws.InstanceOf<InfoCustomException>());
        }
        private void SetupRespuestaLista(IEnumerable<OrdenResiduos> lista)
        {

            repositorio.Setup(
                expression: r => r.Listar(
                    It.IsAny<Expression<Func<OrdenResiduos, bool>>>(),
                    It.IsAny<int>(),
                    null,
                    DirOrden.Asc,
                    null
                ))
               .Returns(lista.ToList());
        }

        private OrdenResiduos CrearOrden(string patente)
        {
            return new OrdenResiduos
            {
                PatenteChasis = patente,
                Cliente = new Proveedor { RazonSocial = "default" },
                FechaCreacion = DateTime.Now,
                Producto = new Material { Nombre = "producto" },
                Localidad = new Localidad { Nombre = "localidad" }
            };
        }
        private void SetupObtencionOrden(int id,OrdenResiduos ordenRecibida)
        {
            repositorio.Setup(r=>r.Obtener<OrdenResiduos>(id)).Returns(ordenRecibida);
        }
    }
}
