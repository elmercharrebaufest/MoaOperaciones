// Ignore Spelling: Sustitucion

using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasSapServiceTest
    {
        private ComprasSapService target;
        private Mock<IObtenerCecoSolpConsumerMOA> cecoConsumerMock;
        private Mock<IObtenerCuentasSolpConsumerMOA> cuentasConsumerMock;
        private Mock<ICrearSolpConsumerMOA> crearSolpMock;
        private Mock<IModificarSolpConsumerMOA> modificarSolpMock;
        private Mock<IObtenerOrdenSolpConsumerMOA> ordenesConsumerMock;
        private Mock<IObtenerServiciosSolpConsumerMOA> serviciosConsumerMock;
        private Mock<IObtenerSolpConsumerMOA> solpConsumerMock;
        private Mock<ICrearPedidoConsumerMOA> crearPedidoMock;
        private Mock<IModificarOrdenDeCompraConsumerMOA> modificarOrdenDeCompraConsumerMock;
        private Mock<IObtenerOrdenDeCompraConsumerMOA> obtenerOrdenDeCompraConsumerMock;
        private Mock<IObtenerFuenteAprovisionamientoConsumerMOA> obtenerFuenteAprovisionamientoConsumerMock;
        private Mock<IObtenerContratoSolpConsumerMOA> obtenerContratoSolpConsumerMock;
        private Mock<IListarSolpPendientesConsumerMOA> listarSolpPendientesConsumerMOAMock;
        private Mock<IReporteOrdenDeCompraConsumerMOA> reporteOrdenDeCompraConsumerMOAMock;
        private Mock<IObtenerPDFOrdenCompraConsumerMOA> obtenerPDFOrdenCompraConsumerMOAMock;
        private Mock<IObtenerAdjuntosSOLPEDConsumerMOA> obtenerAdjuntosSOLPEDConsumerMOAMock;
        private Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA> mIObtenerOrdenesDeCompraParaSOLPConsumerMOA;

        private Mock<ICentroDireccionService> centroDireccionServiceMock;
        private Mock<ITablaSapService> tablaSapServiceMock;
        private Mock<IUnidadMedidaService> unidadMedidaServiceMock;
        private Mock<IUsuarioService> usuarioServiceMock;
        private Mock<ITipoCambioService> cambioServiceMock;

        [SetUp]
        public void Setup()
        {
            cecoConsumerMock = new Mock<IObtenerCecoSolpConsumerMOA>();
            cuentasConsumerMock = new Mock<IObtenerCuentasSolpConsumerMOA>();
            ordenesConsumerMock = new Mock<IObtenerOrdenSolpConsumerMOA>();
            serviciosConsumerMock = new Mock<IObtenerServiciosSolpConsumerMOA>();
            solpConsumerMock = new Mock<IObtenerSolpConsumerMOA>();
            crearSolpMock = new Mock<ICrearSolpConsumerMOA>();
            modificarSolpMock = new Mock<IModificarSolpConsumerMOA>();
            crearPedidoMock = new Mock<ICrearPedidoConsumerMOA>();
            modificarOrdenDeCompraConsumerMock = new Mock<IModificarOrdenDeCompraConsumerMOA>();
            obtenerOrdenDeCompraConsumerMock = new Mock<IObtenerOrdenDeCompraConsumerMOA>();
            obtenerFuenteAprovisionamientoConsumerMock = new Mock<IObtenerFuenteAprovisionamientoConsumerMOA>();
            obtenerContratoSolpConsumerMock = new Mock<IObtenerContratoSolpConsumerMOA>();
            listarSolpPendientesConsumerMOAMock = new Mock<IListarSolpPendientesConsumerMOA>();
            reporteOrdenDeCompraConsumerMOAMock = new Mock<IReporteOrdenDeCompraConsumerMOA>();
            obtenerPDFOrdenCompraConsumerMOAMock = new Mock<IObtenerPDFOrdenCompraConsumerMOA>();
            obtenerAdjuntosSOLPEDConsumerMOAMock = new Mock<IObtenerAdjuntosSOLPEDConsumerMOA>();
            mIObtenerOrdenesDeCompraParaSOLPConsumerMOA = new Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA>();

            centroDireccionServiceMock = new Mock<ICentroDireccionService>();
            tablaSapServiceMock = new Mock<ITablaSapService>();
            unidadMedidaServiceMock = new Mock<IUnidadMedidaService>();
            usuarioServiceMock = new Mock<IUsuarioService>();
            cambioServiceMock = new Mock<ITipoCambioService>();

            target = new ComprasSapService(cecoConsumerMock.Object,
                                           crearPedidoMock.Object,
                                           crearSolpMock.Object,
                                           modificarOrdenDeCompraConsumerMock.Object,
                                           modificarSolpMock.Object,
                                           cuentasConsumerMock.Object,
                                           ordenesConsumerMock.Object,
                                           serviciosConsumerMock.Object,
                                           obtenerOrdenDeCompraConsumerMock.Object,
                                           solpConsumerMock.Object,
                                           obtenerFuenteAprovisionamientoConsumerMock.Object,
                                           obtenerContratoSolpConsumerMock.Object,
                                           listarSolpPendientesConsumerMOAMock.Object,
                                           reporteOrdenDeCompraConsumerMOAMock.Object,
                                           obtenerPDFOrdenCompraConsumerMOAMock.Object,
                                           obtenerAdjuntosSOLPEDConsumerMOAMock.Object,
                                           mIObtenerOrdenesDeCompraParaSOLPConsumerMOA.Object,
                                           centroDireccionServiceMock.Object,
                                           tablaSapServiceMock.Object,
                                           unidadMedidaServiceMock.Object,
                                           usuarioServiceMock.Object,
                                           cambioServiceMock.Object);
        }

        [Test()]
        public void ObtenerCentroDeCostoSapTest()
        {
            var rfcResultMock = new CecoWSMOAResponse()
            {
                Cecos = new List<Ceco>()
                {
                    new Ceco() { CostCenter = "MOA", CO_A = "MOA", Descripcion = "MOA"}
                }
            };

            cecoConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CecoSolpSap}
            };

            var result = target.ObtenerCentrosDeCostoSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void ObtenerCuentasSapTest()
        {
            var rfcResultMock = new CuentaWSMOAResponse()
            {
                Cuentas = new List<SustitucionMOAModel.Models.WSMapMOA.Compras.Cuenta>()
                {
                    new SustitucionMOAModel.Models.WSMapMOA.Compras.Cuenta() { Descripcion = "MOA", Codigo = "MOA", Comp = "MOA"}
                }
            };

            cuentasConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CuentasSolpSap}
            };

            var result = target.ObtenerCuentasSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void ObtenerOrdenesSapTest()
        {
            var rfcResultMock = new OrdenWSMOAResponse()
            {
                Ordenes = new List<Orden>()
                {
                    new Orden() { Descripcion = "MOA", Codigo = "MOA", CompCode = "MOA", Clase = "MOA", Tipo = "MOA"}
                }
            };

            ordenesConsumerMock.Setup(x => x.request("")).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.OrdenSolpSap}
            };

            var result = target.ObtenerOrdenesSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void ObtenerServiciosSapTest()
        {
            var rfcResultMock = new ServicioWSMOAResponse()
            {
                Servicios = new List<Servicio>()
                {
                    new Servicio() { Descripcion = "MOA", Codigo = "MOA", Serv = "MOA"}
                }
            };

            serviciosConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap}
            };

            var result = target.ObtenerServiciosSap();

            Assert.AreEqual(expected.Count, result.Count);
        }
    }
}
