using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAExternalAPI.Controllers;
using SustitucionMOAExternalAPI.Jobs;
using SustitucionMOAModel.Dto;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SustitucionMOATest.Controllers
{
    public class ActualizarSolpControllerTest
    {
        private ActualizarSolpController target;
        private Mock<IJobService> jobService;
        private string mailUsuario = "mail@mail.com";
        private JavaScriptSerializer serializer;

        [SetUp]
        public void SetUp()
        {
            jobService = new Mock<IJobService>();

            this.serializer = new JavaScriptSerializer();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, mailUsuario));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

            target = new ActualizarSolpController(jobService.Object);
        }


        [Test()]
        public void ObtenerPrecioTotalPosicionProveedorTest()
        {
            // Arrange
            string nrosolp = "12345";

            // Act
            var result = target.Post(nrosolp);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkResult), result);
            jobService.Verify(c => c.ActualizarSolp(It.IsAny<string>()), Times.Once);
        }

    }
}