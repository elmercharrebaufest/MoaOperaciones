using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOATest.Controllers
{
    public class CartaPorteControllerTest
    {
        private CartaPorteController target;
        private Mock<ICartaPorteService> cartaPorteServiceMock;
        private string expectedJson;
        private string resultJson;
        private JsonResult resultado;

        [SetUp]
        public void SetUp()
        {
            cartaPorteServiceMock = new Mock<ICartaPorteService>();

        }

        [Test]
        public void GetFotosSinImagenes()
        {
            cartaPorteServiceMock.Setup(s => s.GetFotos(It.IsAny<string>())).Returns(new List<CartaPorteFoto>());

            target = new CartaPorteController(cartaPorteServiceMock.Object);

            resultado = target.GetFotos("0");

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            expectedJson = JsonConvert.SerializeObject(new List<CartaPorteFoto>() { });

            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void GetFotosConImagenes()
        {
            List<CartaPorteFoto> listaFotos = new List<CartaPorteFoto>() { new CartaPorteFoto(new byte[] { 1, 2, 3 }, new byte[] { 1, 2 }) };

            cartaPorteServiceMock.Setup(s => s.GetFotos(It.Is<string>(i => i == "100"))).Returns(listaFotos);

            target = new CartaPorteController(cartaPorteServiceMock.Object);

            resultado = target.GetFotos("100");

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            expectedJson = JsonConvert.SerializeObject(listaFotos);

            Console.WriteLine(resultJson);

            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }
    }
}
