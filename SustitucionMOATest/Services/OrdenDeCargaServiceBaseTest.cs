using Moq;
using NUnit.Framework;
using SustitucionMOARepositorio;
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

        [SetUp]
        public void SetUp()
        {
            mIOrdenCargaConsumerMOA = new Mock<IOrdenCargaConsumerMOA>();
            mIScatoConsumer = new Mock<IScatoConsumer>();
            mIScatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            mIRepositorio = new Mock<IRepositorio>();
            mICNRTClient = new Mock<ICNRTClient>();

            target = new Mock<OrdenDeCargaServiceBase>(
                new object[]
                {
                    mIOrdenCargaConsumerMOA.Object,
                    mIScatoConsumer.Object,
                    mIScatoRepositorioClient.Object,
                    mIRepositorio.Object,
                    mICNRTClient.Object
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
    }
}
