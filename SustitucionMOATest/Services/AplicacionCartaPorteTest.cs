using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class AplicacionCartaPorteTest
    {
        private Mock<IRepositorio> repositorio;
        private IAplicacionCartaPorteService aplicacionCCPPService;

        [SetUp]
        public void SetUp()
        {
            repositorio = new Mock<IRepositorio> ();
            aplicacionCCPPService = new AplicacionCartaPorteService(repositorio.Object);
        }

        [Test]
        public void EliminarAplicacion_IdNoExiste_ShouldThrowInfoError()
        {
            repositorio.Setup(repositorio =>
                repositorio.Obtener<AplicacionCartaPorte>(It.IsAny<int>())
            ).Returns(null as AplicacionCartaPorte);

            Assert.That(
                () => aplicacionCCPPService.EliminarAplicacion(It.IsAny<int>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        [TestCase(EstadoAplicacionCartaPorte.Aplicado)]
        [TestCase(EstadoAplicacionCartaPorte.Eliminado)]
        [TestCase(EstadoAplicacionCartaPorte.Error)]
        public void EliminarAplicacion_IdExisteAplicacionEstadoDiferenteAPendiente_ShouldThrowInfoError(EstadoAplicacionCartaPorte estado)
        {
            repositorio.Setup(repositorio =>
                repositorio.Obtener<AplicacionCartaPorte>(It.IsAny<int>())
            ).Returns(new AplicacionCartaPorte { Estado = estado});

            Assert.That(
                () => aplicacionCCPPService.EliminarAplicacion(It.IsAny<int>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void EliminarAplicacion_IdExisteAplicacionEstadoAPendiente_ShouldChangeEstadoToEliminado()
        {
            var aplicacionAEliminar = new AplicacionCartaPorte { Estado = EstadoAplicacionCartaPorte.Pendiente };
            repositorio.Setup(repositorio =>
                repositorio.Obtener<AplicacionCartaPorte>(It.IsAny<int>())
            ).Returns(aplicacionAEliminar);
            aplicacionCCPPService.EliminarAplicacion(It.IsAny<int>());
            Assert.That( aplicacionAEliminar.Estado, Is.EqualTo(EstadoAplicacionCartaPorte.Eliminado));
        }
    }
}
