using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.Repositorios;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Repositorio
{
    [TestFixture]
    public class RepositorioOrdenResiduosTest
    {
        private Mock<MOAOperacionesDbContext> mContext;
        private IRepositorioOrdenResiduos target;

        [SetUp]
        public void SetUp()
        {
            mContext = new Mock<MOAOperacionesDbContext>();
            target = new RepositorioOrdenResiduos(mContext.Object);
        }

        [Test]
        public void ObtenerMateriales_Ok()
        {
            var materialesDB = new List<Material>
            {
                new Material
                {
                    TablaSeccionMaterial = SustitucionMOAModel.Enums.TablaSeccionMaterial.OrdenResiduos,
                    Id = 1,
                    Nombre = "MatResiduos1",
                    CodigoSap = "MR1",
                    ValidaSisaRuca = false,
                    Almacenes = new List<Almacen> { new Almacen { Id = 1, Nombre = "Alm1" } }
                },
                new Material
                {
                    TablaSeccionMaterial = SustitucionMOAModel.Enums.TablaSeccionMaterial.OrdenDeCargaFason,
                    Id = 2,
                    Nombre = "MatFason1",
                    CodigoSap = "MF1",
                    ValidaSisaRuca = true,
                    Almacenes = new List<Almacen> { new Almacen { Id = 8, Nombre = "Alm8" } }
                },
                new Material
                {
                    TablaSeccionMaterial = SustitucionMOAModel.Enums.TablaSeccionMaterial.OrdenResiduos,
                    Id = 3,
                    Nombre = "MatResiduos2",
                    CodigoSap = "MR2",
                    ValidaSisaRuca = true,
                    Almacenes = new List<Almacen> { new Almacen { Id = 1, Nombre = "Alm1" }, new Almacen { Id = 5, Nombre = "Alm5" } }
                }
            }.AsQueryable();

            var mSetMateriales = CrearSetMock(materialesDB);

            mContext.Setup(x => x.Set<Material>()).Returns(mSetMateriales.Object);

            var resMat = target.ObtenerMateriales();

            Assert.IsNotNull(resMat);
            Assert.That(resMat.Count() == 2);
            Assert.That(resMat.Where(x => x.ValidaSisaRuca).Count() == 1);
            Assert.That(resMat.Where(x => !x.ValidaSisaRuca).Count() == 1);
            Assert.That(resMat.Where(x => x.MaterialId == 3).Single().Almacenes.Count == 2);
        }

        [Test]
        public void ObtenerListadoOrdenes_Ok()
        {
            var fechaInicio = new DateTime(2024, 3, 1);
            var fechaFin = new DateTime(2024, 3, 15);

            var ordenesDB = new List<OrdenResiduos>
            {
                new OrdenResiduos
                {
                    FechaCreacion = new DateTime(2024, 2, 23),
                    Id = 1,
                    Estado = new EstadoOrdenResiduos { Nombre = "Pendiente", Semaforo = "yellow" },
                    LocalidadDescripcion = "Palomar",
                    Producto = new Material { Nombre = "Prod1" },
                    Cliente = new Proveedor { RazonSocial = "Clientelkj" }
                },
                new OrdenResiduos
                {
                    FechaCreacion = new DateTime(2024, 3, 8),
                    Id = 3,
                    Estado = new EstadoOrdenResiduos { Nombre = "Pendiente", Semaforo = "yellow" },
                    LocalidadDescripcion = "Palomar",
                    Producto = new Material { Nombre = "Prod5" },
                    Cliente = new Proveedor { RazonSocial = "Clientegfdsg" }
                },
                new OrdenResiduos
                {
                    FechaCreacion = new DateTime(2024, 3, 25),
                    Id = 5,
                    Estado = new EstadoOrdenResiduos { Nombre = "Pendiente", Semaforo = "yellow" },
                    LocalidadDescripcion = "Palomar",
                    Producto = new Material { Nombre = "Prod8" },
                    Cliente = new Proveedor { RazonSocial = "Clienteyutre" }
                }
            }.AsQueryable();

            var mSetOrdenes = CrearSetMock(ordenesDB);

            mContext.Setup(x => x.Set<OrdenResiduos>()).Returns(mSetOrdenes.Object);

            var resOrdenes = target.ObtenerListadoOrdenes(fechaInicio, fechaFin);

            Assert.IsNotNull(resOrdenes);
            Assert.That(resOrdenes.Count() == 1);
            Assert.That(resOrdenes.First().Id == 3);
            Assert.That(resOrdenes.First().ColorSemaforo == "yellow");
            Assert.AreEqual("08/03/2024", resOrdenes.First().FechaCreacion);
        }


        private Mock<DbSet<T>> CrearSetMock<T>(IQueryable<T> lista) where T : class
        {
            var mSet = new Mock<DbSet<T>>();
            mSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(lista.Provider);
            mSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(lista.Expression);
            mSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(lista.ElementType);
            mSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => lista.GetEnumerator());
            return mSet;
        }
    }
}
