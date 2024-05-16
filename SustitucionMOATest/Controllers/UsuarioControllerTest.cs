using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    public class UsuarioControllerTest
    {

        private UsuarioController target;
        private Mock<IUsuarioService> usuarioServiceMock;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IAltaEmpresaNoGranosService> altaEmpresaNoGranosServiceMock;
        private string expectedJson;
        private string resultJson;

        [SetUp]
        public void SetUp()
        {
            usuarioServiceMock = new Mock<IUsuarioService>();
            repositorioMock = new Mock<IRepositorio>();
            altaEmpresaNoGranosServiceMock = new Mock<IAltaEmpresaNoGranosService>();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, "mail@mail.com"));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

            target = new UsuarioController(usuarioServiceMock.Object, repositorioMock.Object, altaEmpresaNoGranosServiceMock.Object);
        }

        [Test]
        public void GetRolesTest()
        {


            List<RolDropdownDto> listaRoles = new List<RolDropdownDto>() {
                                                new RolDropdownDto { Id = 1, Nombre = "Rol 1" },
                                                new RolDropdownDto { Id = 2, Nombre = "Rol 2" },
                                                new RolDropdownDto { Id = 3, Nombre = "Rol 3" }};

            usuarioServiceMock.Setup(s => s.GetRoles()).Returns(listaRoles);

            JsonResult resultado = (JsonResult)target.GetRoles();

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = new { roles = listaRoles } };

            expectedJson = JsonConvert.SerializeObject(expected);

            Console.WriteLine(resultJson);
            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void GuardarRolesTest()
        {
            var successMessage = string.Format(SuccessMsg.RolesActualizadosOk, "mail@mail.com", "");

            usuarioServiceMock.
                Setup(s =>
                        s.GuardarRoles(
                                It.IsAny<List<int>>(),
                                It.Is<int>(i => i == 1),
                                It.IsAny<string>(),
                                It.IsAny<string>()
                                )
                    )
                .Returns(successMessage); ;

            var resultado = (JsonResult)target.GuardarRoles("1,2,3", 1, "","");

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = successMessage };

            expectedJson = JsonConvert.SerializeObject(expected);

            Console.WriteLine(resultJson);
            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test]
        public void HabilitarUsuarioTest()
        {
            var userMail = "mail@mail.com";

            var successMessage = string.Format(SuccessMsg.UsuarioHabilitadoOK, userMail);

            usuarioServiceMock.
                Setup(s =>
                        s.HabilitarUsuario(
                                It.Is<string>(i => i == userMail)
                                )
                    )
                .Returns(successMessage); ;

            var resultado = (JsonResult)target.Habilitar(userMail);

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = successMessage };

            expectedJson = JsonConvert.SerializeObject(expected);

            Console.WriteLine(resultJson);
            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test]
        public void DeshabilitarUsuarioTest()
        {
            var userMail = "mail@mail.com";

            var successMessage = string.Format(SuccessMsg.UsuarioDeshabilitadoOK, userMail);

            usuarioServiceMock.
                Setup(s =>
                        s.DeshabilitarUsuario(
                                It.Is<string>(i => i == userMail)
                                )
                    )
                .Returns(successMessage); ;

            var resultado = (JsonResult)target.Deshabilitar(userMail);

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = successMessage };

            expectedJson = JsonConvert.SerializeObject(expected);

            Console.WriteLine(resultJson);
            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void GetVendedoresTest()
        {
            var userMail = "mail@mail.com";
            var respuesta = new List<ProveedorDto>();
            usuarioServiceMock.
                Setup(s =>
                        s.GetVendedoresUsuario(
                                It.Is<string>(i => i == userMail)
                                )
                    )
                .Returns(respuesta);

            var resultado = (JsonResult)target.GetVendedores();

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = new { usuarios = respuesta } };

            expectedJson = JsonConvert.SerializeObject(expected);

            Console.WriteLine(resultJson);
            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }

    }
}
