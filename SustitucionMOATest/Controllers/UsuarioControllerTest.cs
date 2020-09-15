using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    public class UsuarioControllerTest
    {

        private UsuarioController target;
        private Mock<IUsuarioService> usuarioServiceMock;
        private string expectedJson;
        private string resultJson;

        [SetUp]
        public void SetUp()
        {
            usuarioServiceMock = new Mock<IUsuarioService>();
        }

        [Test]
        public void GetRolesTest()
        {
            List<RolDropdownDto> listaRoles = new List<RolDropdownDto>() {
                                                new RolDropdownDto { Id = 1, Nombre = "Rol 1" },
                                                new RolDropdownDto { Id = 2, Nombre = "Rol 2" },
                                                new RolDropdownDto { Id = 3, Nombre = "Rol 3" }};

            usuarioServiceMock.Setup(s => s.GetRoles()).Returns(listaRoles);

            target = new UsuarioController(usuarioServiceMock.Object);

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
            var successMessage = string.Format(SuccessMsg.RolesActualizadosOk, "mail@mail.com");

            usuarioServiceMock.
                Setup(s =>
                        s.GuardarRoles(
                                It.IsAny<List<int>>(),
                                It.Is<int>(i => i == 1)
                                )
                    )
                .Returns(successMessage); ;


            target = new UsuarioController(usuarioServiceMock.Object);

            var resultado = (JsonResult)target.GuardarRoles("1,2,3", 1);

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

            target = new UsuarioController(usuarioServiceMock.Object);

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

            target = new UsuarioController(usuarioServiceMock.Object);

            var resultado = (JsonResult)target.Deshabilitar(userMail);

            resultJson = JsonConvert.SerializeObject(resultado.Data);

            var expected = new { data = successMessage };

            expectedJson = JsonConvert.SerializeObject(expected);

            Console.WriteLine(resultJson);
            Assert.NotNull(resultado);
            Assert.AreEqual(expectedJson, resultJson);
        }


    }
}
