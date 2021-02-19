using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class OrdenDeCargaServiceTest
    {

        private OrdenDeCargaService target;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new OrdenDeCargaService(repositorioMock.Object);
        }


        [Test()]
        public void AgregarTest()
        {
            string mailUsuario = "usuario@test.com";
           
            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Aprobado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = mailUsuario,
                CUIT = "233333333333",
                TipoProveedor = new TipoUsuario { Id = 5, Nombre = "Cliente" , NombreCorto = "CLI"},
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>()
                {
                    proveedor
                }
            };

            repositorioMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);

            repositorioMock
                 .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
                 .Returns(proveedor);

            var ordenDeCarga = new OrdenDeCarga
            {
                Id= 1,
                CUITCliente = "233333333333",

            };

            ConfigurationManager.AppSettings["CantidadOrdenDeCarga"] = "30000";

            var result = target.Agregar(ordenDeCarga, mailUsuario);

            var expected = new Resultado { IdEntidad = 1, Mensaje = SuccessMsg.OrdenDeCargaAgregada };

            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void ListarTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void AnularOrdenTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void SeleccionarContratoTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void SeleccionarCorredorTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void SeleccionarCorredorContratoTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerContratosTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerContratosYCorredoresTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerContratosTest1()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerCorredoresTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void ObtenerCorredoresTest1()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void VerificarTransporteTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void VerificarTransporteTest1()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void NotificarTransporteTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void VerificarTransporteBulkTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void VerificarSituacionCrediticiaTest()
        {
            throw new NotImplementedException();
        }
    }
}