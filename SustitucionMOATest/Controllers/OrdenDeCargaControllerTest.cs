using Moq;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
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
    public class OrdenDeCargaControllerTest
    {
        private OrdenDeCargaController target;
        private Mock<IOrdenDeCargaService> ordenDeCargaServiceMock;
        private Mock<IConsultaService> consultaServiceMock;

        [SetUp]
        public void SetUp()
        {
            ordenDeCargaServiceMock = new Mock<IOrdenDeCargaService>();
            consultaServiceMock = new Mock<IConsultaService>();
            target = new OrdenDeCargaController(consultaServiceMock.Object, ordenDeCargaServiceMock.Object);


            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, "mail@mail.com"));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

        }

        [Test()]
        public void AgregarTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void GetListadoTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void GetTest()
        {
            throw new NotImplementedException();
        }

        [Test()]
        public void AnularOrdenTest()
        {
            throw new NotImplementedException();
        }
    }
}