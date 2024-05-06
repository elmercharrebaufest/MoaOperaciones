using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    public class OrdenDeCargaControllerTest
    {
        private OrdenDeCargaController target;
        private Mock<IOrdenDeCargaService> ordenDeCargaServiceMock;
        private Mock<IFacturaAnticipadaService> facturaAnticipadaServiceMock;
        private Mock<IConsultaService> consultaServiceMock;
        private OrdenDeCarga ordenDeCarga;
        private string expectedJson;
        private string resultJson;
        private string mailUsuario = "mail@mail.com";
        int ordenId = 2041;

        [SetUp]
        public void SetUp()
        {
            ordenDeCargaServiceMock = new Mock<IOrdenDeCargaService>();
            consultaServiceMock = new Mock<IConsultaService>();
            facturaAnticipadaServiceMock = new Mock<IFacturaAnticipadaService>();
            target = new OrdenDeCargaController(consultaServiceMock.Object, ordenDeCargaServiceMock.Object, facturaAnticipadaServiceMock.Object);


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
            var expected = new
            {
                data = new Resultado
                {
                    IdEntidad = 1,
                    Mensaje = SuccessMsg.OrdenDeCargaAgregada
                }
            };

            var ordenDeCargaJson = JsonConvert.SerializeObject(ordenDeCarga);
            ordenDeCargaServiceMock.Setup(s => s.Agregar(It.IsAny<OrdenDeCarga>(), It.Is<string>(i => i == mailUsuario))).Returns(expected.data);
            var result = (JsonResult)target.Agregar(ordenDeCargaJson);
            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);

        }

        [Test()]
        public void GetListadoTest()
        {
            var orden = new
            {
                data = new List<OrdenDeCargaDto>()
                {
                    new OrdenDeCargaDto { Id = 1, Cliente ="", PatenteChasis = "", CUITCliente ="" }
                }

            };

            ordenDeCargaServiceMock.Setup(x => x.Listar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(orden.data);
            var result = (JsonResult)target.GetListado(DateTime.Now.ToString(), DateTime.Now.ToString());
            expectedJson = JsonConvert.SerializeObject(orden.data);
            resultJson = JsonConvert.SerializeObject(result.Data);
            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);

        }

        [Test()]
        public void GetTest()
        {
            var orden = new
            {
                data = new OrdenDeCargaDetalleDto
                {
                    Id = 1,
                    Cliente = "",
                    PatenteAcoplado = "",
                    CUITCliente = ""
                },
                historial = new List<OrdenDeCargaHistorialDto>()
            };

            ordenDeCargaServiceMock.Setup(x => x.Obtener(It.IsAny<string>(), It.IsAny<int>())).Returns(orden.data);
            
            var result = (JsonResult)target.Get(ordenId);
            expectedJson = JsonConvert.SerializeObject(orden);
            resultJson = JsonConvert.SerializeObject(result.Data);
            dynamic expectedObject = JsonConvert.DeserializeObject(expectedJson);
            dynamic resultObject = JsonConvert.DeserializeObject(resultJson);
            Assert.NotNull(result);
            Assert.AreEqual(expectedObject.data, resultObject.data);
            Assert.AreEqual(expectedObject.historial, resultObject.historial);
        }

        [Test()]
        public void AnularOrdenTest()
        {
            var expected = new { data = SuccessMsg.OrdenDeCargaAnulada };
            ordenDeCargaServiceMock.Setup(s => s.AnularOrden(It.Is<int>(i => i == ordenId), It.IsAny<string>())).Returns(expected.data);
            var result = (JsonResult)target.AnularOrden(ordenId);
            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void EditarTest()
        {
            var expected = new
            {
                data = new Resultado
                {
                    IdEntidad = 1,
                    Mensaje = SuccessMsg.OrdenDeCargaActualizada
                }
            };
            var ordenDeCargaJson = JsonConvert.SerializeObject(ordenDeCarga);
            ordenDeCargaServiceMock.Setup(s => s.Editar(It.IsAny<OrdenDeCarga>(), It.Is<string>(i => i == mailUsuario))).Returns(expected.data);
            var result = (JsonResult)target.Editar(ordenDeCargaJson);
            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);
            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void NotificarTransporteTest()
        {
            var mensaje = new { data = "Notificación enviada" };
            ordenDeCargaServiceMock.Setup(s => s.NotificarTransporte(ordenId)).Returns(mensaje.data);
            var result = (JsonResult)target.NotificarTransporte(ordenId);
            expectedJson = JsonConvert.SerializeObject(mensaje);
            resultJson = JsonConvert.SerializeObject(result.Data);
            Assert.AreEqual(expectedJson, resultJson);

        }

        [Test()]
        public void VerificarSituacionCrediticiaTest()
        {
            var resultado = new
            {
                data = new Resultado { Mensaje = "Verifique el crédito del pedido" }
            };

            ordenDeCargaServiceMock.Setup(s => s.VerificarSituacionCrediticia(ordenId)).Returns(resultado.data);
            var result = (JsonResult)target.VerificarSituacionCrediticia(ordenId);
            expectedJson = JsonConvert.SerializeObject(result.Data);
            resultJson = JsonConvert.SerializeObject(result.Data);
            Assert.AreEqual(expectedJson, resultJson);

        }

        [Test]
        public void ValidarCamionTest()
        {
            var chasisParam = "CHA135";
            var acopladoParam = "ACO246";

            var serviceResponse = new ValidarCamionResponse
            {
                ExisteCamion = true,
                EsCamionEscalable = true
            };

            ordenDeCargaServiceMock
                .Setup(x => x.ValidarCamion(chasisParam, acopladoParam))
                .Returns(serviceResponse);

            var result = target.ValidarCamion(chasisParam, acopladoParam);

            var expected = new SustitucionMOAApiResponse<ValidarCamionResponse>
            {
                Data = serviceResponse,
                Error = null,
                Info = null,
                Logout = false
            };
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.AreEqual(expectedJson, result.Content);
        }
    }
}