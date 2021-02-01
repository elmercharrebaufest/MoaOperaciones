using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    public class ConsultaControllerTest
    {
        private ConsultaController target;
        private Mock<IConsultaService> consultaServiceMock;
        private string expectedJson;
        private string resultJson;
        private JsonResult resultado;

        [SetUp]
        public void SetUp()
        {
            consultaServiceMock = new Mock<IConsultaService>();
        }

        [Test]
        public void GetConsutaDetalle()
        {
            var mockedId = 1;
            var mockedConsulta = new ConsultaDto()
            {
                Id = mockedId,
                Asunto = "Consulta Test",
                CUIT = "11-111111-1"
            };

            consultaServiceMock.Setup(x => x.ObtenerConsulta(It.IsAny<int>())).Returns(mockedConsulta);

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(mockedConsulta);

            resultado = target.Detalle(mockedId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void GetConsutaDetalleConIdInvalido()
        {
            var mockedId = -1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id de consulta inválido" });
            
            resultado = target.Detalle(mockedId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void GetConsutaDetalleConIdInexistente()
        {
            var mockedId = 1;

            consultaServiceMock.Setup(x => x.ObtenerConsulta(It.IsAny<int>())).Returns((ConsultaDto)null);

            target = new ConsultaController(consultaServiceMock.Object);

            resultado = target.Detalle(mockedId);

            Assert.NotNull(resultado);
            Assert.IsNull(resultado.Data);
        }

        [Test]
        public void PostComentario()
        {
            var mockedConsultaId = 1;
            var mockedComentario = new Comentario()
            {
                Consulta_Id = mockedConsultaId,
                Detalle = "Comentario Test",
                Fecha = DateTime.Now
            };

            consultaServiceMock.Setup(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<Comentario>())).Verifiable();

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { });

            resultado = target.Comentarios(mockedConsultaId, mockedComentario);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostComentarioConIdConsultaInvalido()
        {
            var mockedConsultaId = 1;
            var mockedComentario = new Comentario()
            {
                Consulta_Id = mockedConsultaId,
                Detalle = "Comentario Test",
                Fecha = DateTime.Now
            };

            consultaServiceMock.Setup(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<Comentario>())).Throws(new InfoCustomException("No existe la consulta"));

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe la consulta" });
            
            resultado = target.Comentarios(mockedConsultaId, mockedComentario);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchRecategorizarConsulta()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;

            consultaServiceMock.Setup(x => x.RecategorizarConsulta(It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchRecategorizarConsultaConIdConsultaInvalido()
        {
            var mockedConsultaId = -1;
            var mockedCategoriaId = 1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchRecategorizarConsultaConIdCategoriaInvalido()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = -1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe la consulta" });

            resultado = target.Comentarios(mockedConsultaId, mockedComentario);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }
    }
}
