using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class OrdenDeCargaServiceBaseTest
    {
        private Mock<OrdenDeCargaServiceBase> target;
        
        private Mock<IOrdenCargaConsumerMOA> mIOrdenCargaConsumerMOA;
        private Mock<IScatoConsumer> mIScatoConsumer;
        private Mock<IScatoRepositorioClient> mIScatoRepositorioClient;
        private Mock<IRepositorio> mIRepositorio;
        private Mock<ICNRTClient> mICNRTClient;
        private Mock<IFeriadoService> mIFeriadoService;
        private Mock<IUbicacionGeograficaService> mIUbicacionGeograficaService;

        [SetUp]
        public void SetUp()
        {
            mIOrdenCargaConsumerMOA = new Mock<IOrdenCargaConsumerMOA>();
            mIScatoConsumer = new Mock<IScatoConsumer>();
            mIScatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            mIRepositorio = new Mock<IRepositorio>();
            mICNRTClient = new Mock<ICNRTClient>();
            mIFeriadoService = new Mock<IFeriadoService>();
            mIUbicacionGeograficaService = new Mock<IUbicacionGeograficaService>();

            target = new Mock<OrdenDeCargaServiceBase>(
                new object[]
                {
                    mIOrdenCargaConsumerMOA.Object,
                    mIScatoConsumer.Object,
                    mIScatoRepositorioClient.Object,
                    mIRepositorio.Object,
                    mICNRTClient.Object,
                    mIFeriadoService.Object,
                    mIUbicacionGeograficaService.Object
                })
            {
                CallBase = true,
            };
        }

        [Test()]
        public void ValidarCuitExisteScato_Existe()
        {
            var inCuit = "30284849293";
            var outRazonSocial = "Cali hnos";
            var clientesScato = new ClienteDto[]
            {
                new ClienteDto { Cuit = inCuit, Descripcion = "CliExtra", Activo = true, Bloqueado = true },
                new ClienteDto { Cuit = inCuit, Descripcion = outRazonSocial, Activo = true, Bloqueado = false }
            };

            mIScatoConsumer
                .Setup(x => x.ObtenerClientesPorCuit(inCuit))
                .Returns(clientesScato);

            var resultado = target.Object.ValidarCuitExisteScato(inCuit);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Existe);
            Assert.AreEqual(outRazonSocial, resultado.RazonSocial);
        }

        [Test()]
        public void ValidarCuitExisteScato_NoExiste()
        {
            var inCuit = "30284849293";
            var outRazonSocial = "Cali hnos";
            var clientesScato = new ClienteDto[]
            {
                new ClienteDto { Cuit = inCuit, Descripcion = outRazonSocial, Activo = true, Bloqueado = true },
                new ClienteDto { Cuit = inCuit, Descripcion = outRazonSocial, Activo = false, Bloqueado = false }
            };

            mIScatoConsumer
                .Setup(x => x.ObtenerClientesPorCuit(inCuit))
                .Returns(clientesScato);

            var resultado = target.Object.ValidarCuitExisteScato(inCuit);

            Assert.IsNotNull(resultado);
            Assert.IsFalse(resultado.Existe);
            Assert.AreEqual("", resultado.RazonSocial);
        }

        [Test]
        [TestCase("30716928345")]
        [TestCase("23305842249")]
        [TestCase("20469978622")]
        [TestCase("20343197072")]
        [TestCase("20409255397")]
        [TestCase("20227860066")]
        public void ValidarDigitoCuit_CasosCorrecto(string cuitAValidar)
        {
            var scatoChoferRes = new SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio.Respuesta<SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio.Chofer> { IsValid = true };

            mIScatoRepositorioClient
                .Setup(x => x.ObtenerChoferPorCuil(It.IsAny<string>()))
                .Returns(scatoChoferRes);

            var resultado = target.Object.ValidarCuilChofer(cuitAValidar);

            Assert.IsTrue(resultado.Item1);
        }

        [Test]
        [TestCase("33716928345")]
        [TestCase("25305842249")]
        [TestCase("23469978622")]
        [TestCase("23343197072")]
        [TestCase("23409255397")]
        [TestCase("23227860066")]
        public void ValidarDigitoCuit_CasosErroneos(string cuitAValidar)
        {
            var scatoChoferRes = new SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio.Respuesta<SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio.Chofer> { IsValid = true };

            mIScatoRepositorioClient
                .Setup(x => x.ObtenerChoferPorCuil(It.IsAny<string>()))
                .Returns(scatoChoferRes);

            var resultado = target.Object.ValidarCuilChofer(cuitAValidar);

            Assert.IsFalse(resultado.Item1);
        }

        [Test]
        public void ValidarDigitoCuit_NoCumpleFormato_Exception()
        {
            Assert.Throws<ValidationCustomException>(() => target.Object.ValidarCuilChofer(""));
        }

        [Test]
        public void ObtenerProveedor_Ok()
        {
            var proveedorBD = new Proveedor
            {
                Id = 1,
                RazonSocial = "Prov1",
                CodigoProveedor = "PRV1",
                TipoProveedor = new TipoUsuario { Id = 7 }
            };

            mIRepositorio
                .Setup(x => x.Obtener<Proveedor>(1))
                .Returns(proveedorBD);

            var provResult = target.Object.ObtenerProveedor(1);

            Assert.IsNotNull(provResult);
            Assert.That(provResult.Id, Is.EqualTo(1));
            Assert.That(provResult.RazonSocial, Is.EqualTo("Prov1"));
            Assert.That(provResult.CodigoProveedor, Is.EqualTo("PRV1"));
            mIRepositorio
                .Verify(x => x.Obtener<Proveedor>(1), Times.Once);
        }

        [Test]
        public void ObtenerProveedor_NoExiste()
        {
            mIRepositorio
                .Setup(x => x.Obtener<Proveedor>(3))
                .Returns((Proveedor)null);

            Assert.Throws<Exception>(() => target.Object.ObtenerProveedor(3));

            mIRepositorio
                .Verify(x => x.Obtener<Proveedor>(3), Times.Once);
        }
    }
}
