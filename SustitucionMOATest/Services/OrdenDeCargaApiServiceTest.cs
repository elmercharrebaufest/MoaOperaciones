using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class OrdenDeCargaApiServiceTest
    {
        private OrdenDeCargaApiService target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new OrdenDeCargaApiService(repositorioMock.Object);
        }


        [Test()]
        public void InformarViajeOrdenesDeCargaFasOkTest()
        {
            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>()))
                .Returns(new OrdenDeCarga { });

            var result = target.InformarViajeOrdenesDeCargaFas(new SustitucionMOAModel.Dto.OrdenDeCargaFason.IngresosEgresosFas { Entrega = "00123456", PesoBruto = 123, PesoNeto = 456 });

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));

            Assert.AreEqual(0, result.Errores.Count);
        }

        [Test()]
        public void InformarViajeOrdenesDeCargaFasErrorTest()
        {
            var result = target.InformarViajeOrdenesDeCargaFas(new SustitucionMOAModel.Dto.OrdenDeCargaFason.IngresosEgresosFas { Entrega = "00123456", PesoBruto = 123, PesoNeto = 456 });

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.AreEqual(1, result.Errores.Count);
        }

        [Test()]
        public void InformarViajeOrdenesDeCargaFasonOkTest()
        {
            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<OrdenDeCargaFason, bool>>>()))
                .Returns(new OrdenDeCargaFason { });

            var result = target.InformarViajeOrdenesDeCargaFason(
                new SustitucionMOAModel.Dto.OrdenDeCargaFason.IngresosEgresosFasones
                {
                    Cantidad = 1,
                    FasonId = 1,
                    FechaEgreso = DateTime.Now,
                    FechaIngreso = DateTime.Now,
                    NroRemito = "00123456",
                    UniMedCant = "KG"
                });

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<OrdenDeCargaFason, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));

            Assert.AreEqual(0, result.Errores.Count);
        }

        [Test()]
        public void InformarViajeOrdenesDeCargaFasonErrorTest()
        {

            var result = target.InformarViajeOrdenesDeCargaFason(
                new SustitucionMOAModel.Dto.OrdenDeCargaFason.IngresosEgresosFasones
                {
                    Cantidad = 1,
                    FasonId = 1,
                    FechaEgreso = DateTime.Now,
                    FechaIngreso = DateTime.Now,
                    NroRemito = "00123456",
                    UniMedCant = "KG"
                });

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<OrdenDeCargaFason, bool>>>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);

            Assert.AreEqual(1, result.Errores.Count);
        }

        [Test()]
        public void ObtenerOrdenesTest()
        {
            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCargaFason, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>(),
                            It.IsAny<IEnumerable<Expression<Func<OrdenDeCargaFason, object>>>>()))
            .Returns(new List<OrdenDeCargaFason> { new OrdenDeCargaFason { PatenteChasis = "", Cantidad = 12312, Cliente = new Proveedor { RazonSocial = "" }, Cliente_Id = 1, Producto = new Material { Nombre = "" } } });

            repositorioMock.Setup(x => x.Listar(It.IsAny<Expression<Func<OrdenDeCarga, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>(),
                            It.IsAny<IEnumerable<Expression<Func<OrdenDeCarga, object>>>>()))
            .Returns(new List<OrdenDeCarga> { new OrdenDeCarga { ChasisAcoplado = "", Cliente_Id = 1, Cantidad = 1232, Producto = new Material { Nombre = "" }, Cliente = new Proveedor { RazonSocial = "" } } });
            var patente = "";
            var result = target.ObtenerOrdenes(patente, true, true);

            Assert.AreEqual(2, result.Count);
        }

    }
}