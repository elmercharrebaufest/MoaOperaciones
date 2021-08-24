using Moq;
using NUnit.Framework;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class PesificacionServiceTests
    {

        private IPesificacionService service;
        private Mock<IListarPesificacionesConsumer> pesificacionConsumer;

        [SetUp]
        public void SetUp()
        {
            pesificacionConsumer = new Mock<IListarPesificacionesConsumer>();
            service = new PesificacionService(pesificacionConsumer.Object);
        }

        //[Test()]
        //public void PesificacionServiceTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test()]
        //public void GetFechaPesificacionTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test()]
        //public void SetContratoTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test()]
        //public void SetContratosTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test()]
        //public void GetContratosTest()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test()]
        //public void GetPesificacionesSAPTest()
        //{
        //    throw new NotImplementedException();
        //}
    }
}