using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAUtils.Services.Email;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WebApi.OpenStreetMap;
using SustitucionMOAWS.WebApi.OpenStreetMap.Response;
using SustitucionMOAWS.WebApi.OSRM.Response;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            ConfigurationManager.AppSettings["CoordenadasPlantaMoaLatitud"] = "-32,773465";
            ConfigurationManager.AppSettings["CoordenadasPlantaMoaLongitud"] = "-60,728782";

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

        [Test]
        public void ObtenerDistanciaDePlantaMoaADestino_DireccionNoTieneLocalidadYProvincia()
        {
            // Arrange
            var direccionDestinoMalFormada = "Belgrano 4750";
            var direccionBuscada = "";
            DistanciaDomicilio distanciaDomicilioBD = null;

            mIRepositorioUbicacionGeografica
                .Setup(x => x.ObtenerDistanciaSegunDescripcionDomicilio(It.Is<string>(p => p == direccionDestinoMalFormada)))
                .Returns(distanciaDomicilioBD);

            SetupReemplazosDeTextoEnDomicilio();

            // Act
            var resultado = target.ObtenerDistanciaDePlantaMoaADestino(direccionDestinoMalFormada);

            // Assert
            mIRepositorioUbicacionGeografica.Verify(x => x.Agregar(It.Is<DistanciaDomicilio>(d => !d.DistanciaKm.HasValue)), Times.Once);
            mIRepositorioUbicacionGeografica.Verify(x => x.GuardarCambios(), Times.Once);

            mIEmailUbicacionGeograficaService
                .Verify(x => x.EnviarMailDistanciaNoEncontrada(It.Is<string>(d => d == direccionDestinoMalFormada), It.Is<string>(d => d == direccionBuscada)), Times.Once);

            Assert.IsNotNull(resultado);
            Assert.IsNull(resultado.DistanciaKm);
            Assert.AreEqual("", resultado.DireccionBuscada);
        }

        [Test]
        public void ObtenerDistanciaDePlantaMoaADestino_ConsultaApis_EncuentraLugarYRuta()
        {
            // Arrange
            var direccionDestino = "Belgrano 4750, Caseros, Buenos Aires";
            var direccionBuscada = "Caseros, Buenos Aires, Argentina";
            DistanciaDomicilio distanciaDomicilioBD = null;

            var lugaresOSMResponse = new LugaresOSMResponse
            {
                JsonResponseRaw = "{ \"PropiedadPrueba\": 4327.24 }",
                Lugares = new List<OSMPlace>
                {
                    new OSMPlace { Lat = -32.5434, Lon = -62.4235 }
                }
            };

            var rutaOsrmResponse = new RutaOSRMResponse
            {
                JsonResponseRaw = "{ \"PropiedadPrueba2\": 7543.67 }",
                RouteResponse = new RouteResponse<SustitucionMOAWS.WebApi.OSRM.Common.GeoJsonGeometry>
                {
                    Routes = new SustitucionMOAWS.WebApi.OSRM.Response.Common.Route<SustitucionMOAWS.WebApi.OSRM.Common.GeoJsonGeometry>[1]
                    {
                        new SustitucionMOAWS.WebApi.OSRM.Response.Common.Route<SustitucionMOAWS.WebApi.OSRM.Common.GeoJsonGeometry>
                        {
                            Distance = 319000
                        }
                    }
                }
            };

            mIRepositorioUbicacionGeografica
                .Setup(x => x.ObtenerDistanciaSegunDescripcionDomicilio(It.Is<string>(p => p == direccionDestino)))
                .Returns(distanciaDomicilioBD);

            mIOpenStreetMapClient
                .Setup(x => x.BuscarLugaresSegunDireccionAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(lugaresOSMResponse));

            mIOsrmApiClient
                .Setup(x => x.ObtenerRuta(It.IsAny<GeoCoordenada>(), It.IsAny<GeoCoordenada>()))
                .Returns(rutaOsrmResponse);

            SetupReemplazosDeTextoEnDomicilio();

            // Act
            var resultado = target.ObtenerDistanciaDePlantaMoaADestino(direccionDestino);

            // Assert
            mIRepositorioUbicacionGeografica.Verify(x => x.Agregar(It.Is<DistanciaDomicilio>(d => d.DistanciaKm == 319)), Times.Once);
            mIRepositorioUbicacionGeografica.Verify(x => x.GuardarCambios(), Times.Once);

            mIEmailUbicacionGeograficaService.Verify(x => x.EnviarMailDistanciaNoEncontrada(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(319, resultado.DistanciaKm);
            Assert.AreEqual(direccionBuscada, resultado.DireccionBuscada);
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
