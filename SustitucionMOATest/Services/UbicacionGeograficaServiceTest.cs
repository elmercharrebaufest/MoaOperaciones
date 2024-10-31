using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAUtils.Services.Email;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WebApi.OpenStreetMap;
using SustitucionMOAWS.WebApi.OpenStreetMap.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class UbicacionGeograficaServiceTest
    {
        private Mock<IRepositorioUbicacionGeografica> mIRepositorioUbicacionGeografica;
        private Mock<IOsrmApiClient> mIOsrmApiClient;
        private Mock<IOpenStreetMapClient> mIOpenStreetMapClient;
        private Mock<IEmailUbicacionGeograficaService> mIEmailUbicacionGeograficaService;
        private IUbicacionGeograficaService target;

        [SetUp]
        public void SetUp()
        {
            mIRepositorioUbicacionGeografica = new Mock<IRepositorioUbicacionGeografica>();
            mIOsrmApiClient = new Mock<IOsrmApiClient>();
            mIOpenStreetMapClient = new Mock<IOpenStreetMapClient>();
            mIEmailUbicacionGeograficaService = new Mock<IEmailUbicacionGeograficaService>();

            target = new UbicacionGeograficaService(
                mIRepositorioUbicacionGeografica.Object,
                mIOsrmApiClient.Object,
                mIOpenStreetMapClient.Object,
                mIEmailUbicacionGeograficaService.Object);
        }

        [Test]
        public void ObtenerDistanciaDePlantaMoaADestino_DestinoYaExisteLocalmente()
        {
            // Arrange
            var direccionDestino = "Belgrano 4750, Caseros, Buenos Aires";
            var distanciaDomicilioBD = new DistanciaDomicilio { DomicilioDescripcion = direccionDestino, DistanciaKm = 125 };

            mIRepositorioUbicacionGeografica
                .Setup(x => x.ObtenerDistanciaSegunDescripcionDomicilio(It.Is<string>(p => p == direccionDestino)))
                .Returns(distanciaDomicilioBD);

            // Act
            var resultado = target.ObtenerDistanciaDePlantaMoaADestino(direccionDestino);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(125, resultado.DistanciaKm);
        }

        [Test]
        public void ObtenerDistanciaDePlantaMoaADestino_NoSeEncuentraNingunLugar()
        {
            // Arrange
            var direccionDestino = "Belgrano 4750, Caseros, Buenos Aires";
            var direccionBuscada = "Caseros, Buenos Aires, Argentina";
            DistanciaDomicilio distanciaDomicilioBD = null;
            LugaresOSMResponse lugaresOSMResponse = null;

            mIRepositorioUbicacionGeografica
                .Setup(x => x.ObtenerDistanciaSegunDescripcionDomicilio(It.Is<string>(p => p == direccionDestino)))
                .Returns(distanciaDomicilioBD);

            mIOpenStreetMapClient
                .Setup(x => x.BuscarLugaresSegunDireccionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(lugaresOSMResponse));

            SetupReemplazosDeTextoEnDomicilio();

            // Act
            var resultado = target.ObtenerDistanciaDePlantaMoaADestino(direccionDestino);

            // Assert
            mIRepositorioUbicacionGeografica.Verify(x => x.Agregar(It.Is<DistanciaDomicilio>(d => !d.DistanciaKm.HasValue)), Times.Once);
            mIRepositorioUbicacionGeografica.Verify(x => x.GuardarCambios(), Times.Once);

            mIEmailUbicacionGeograficaService
                .Verify(x => x.EnviarMailDistanciaNoEncontrada(It.Is<string>(d => d == direccionDestino), It.Is<string>(d => d == direccionBuscada)), Times.Once);

            Assert.IsNotNull(resultado);
            Assert.IsNull(resultado.DistanciaKm);
        }

        private void SetupReemplazosDeTextoEnDomicilio()
        {
            mIRepositorioUbicacionGeografica
                .Setup(x => x.ObtenerReemplazosParaDomicilios())
                .Returns(new List<DistanciaDomicilioReemplazos>
                {
                    new DistanciaDomicilioReemplazos { Anterior = "CAP.FEDERAL", Nuevo = "CAPITAL FEDERAL"},
                    new DistanciaDomicilioReemplazos { Anterior = "SGO.DEL ESTERO", Nuevo = "SANTIAGO DEL ESTERO"}
                });
        }
    }
}
