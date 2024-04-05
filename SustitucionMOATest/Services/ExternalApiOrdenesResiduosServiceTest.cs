using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

            Expression<Func<OrdenesDeCargaApiDto, bool>> filter1 =
                or => string.IsNullOrEmpty(patente1) || or.PatenteChasis == patente1;

            Expression<Func<OrdenesDeCargaApiDto, bool>> filter2 =
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
                DistanciaKm = 45,
                Cliente = new Proveedor { RazonSocial = "default" },
                FechaCreacion = DateTime.Now,
                FechaRetiro = DateTime.Now,
                Producto = new Material { Nombre = "producto" },
                Localidad = new Localidad { Nombre = "localidad" }
            };
        }
    }
}
