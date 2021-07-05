using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SustitucionMOATest.Controllers
{
    public class AltaEmpresaControllerTest
    {
        private AltaEmpresaController target;
        private Mock<IAltaEmpresaService> altaEmpresaServiceMock;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IDataAgroService> dataAgroServiceMock;
        private string expectedJson;
        private string resultJson;
        private string mailUsuario = "mail@mail.com";

        [SetUp]
        public void SetUp()
        {
            altaEmpresaServiceMock = new Mock<IAltaEmpresaService>();
            repositorioMock = new Mock<IRepositorio>();
            dataAgroServiceMock = new Mock<IDataAgroService>();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, mailUsuario));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

            target = new AltaEmpresaController(altaEmpresaServiceMock.Object, repositorioMock.Object, dataAgroServiceMock.Object);
        }

        [Test()]
        public void AgregarObservacionTest()
        {
            var expected = SuccessMsg.ObservacionAgregadaOK;

            var proveedorId = 1;
            var observacion = "Buen día!";

            altaEmpresaServiceMock.Setup(s => s.AgregarObservacion(
                                                        It.IsAny<int>(),
                                                        It.IsAny<string>(),
                                                        It.IsAny<string>()))
                                   .Returns(expected);

            var result = target.AgregarObservacion(proveedorId, observacion);

            expectedJson = JsonConvert.SerializeObject(new { data = expected });
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }
    }
}
