using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            var mockedConsulta = new ConsultaDto(new Consulta()
            {
                Id = mockedId,
                Asunto = "Consulta Test"
            });

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
            var mockedSubCategoriaId = 1;

            consultaServiceMock.Setup(x => x.RecategorizarConsulta(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId, mockedSubCategoriaId);
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
            var mockedSubCategoriaId = 1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId, mockedSubCategoriaId);
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
            var mockedSubCategoriaId = 1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId, mockedSubCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchRecategorizarConsultaConIdConsultaInexistente()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;
            var mockedSubCategoriaId = 1;

            consultaServiceMock.Setup(x => x.RecategorizarConsulta(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new InfoCustomException("No existe la consulta"));

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe la consulta" });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId, mockedSubCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchRecategorizarConsultaConIdCategoriaInexistente()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;
            var mockedSubCategoriaId = 1;

            consultaServiceMock.Setup(x => x.RecategorizarConsulta(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(new InfoCustomException("No existe la categoria"));

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe la categoria" });

            resultado = target.Recategorizar(mockedConsultaId, mockedCategoriaId, mockedSubCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchEstadoConsulta()
        {
            var mockedConsultaId = 1;
            var mockedEstadoId = 1;

            consultaServiceMock.Setup(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedEstadoId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchEstadoConsultaConIdConsultaInvalido()
        {
            var mockedConsultaId = -1;
            var mockedEstadoId = 1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedEstadoId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchEstadoConsultaConIdEstadoInvalido()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = -1;

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchEstadoConsultaConIdConsultaInexistente()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;

            consultaServiceMock.Setup(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>())).Throws(new InfoCustomException("No existe la consulta"));

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe la consulta" });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PatchEstadoConsultaConIdEstadoInexistente()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;

            consultaServiceMock.Setup(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>())).Throws(new InfoCustomException("No existe el estado"));

            target = new ConsultaController(consultaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe el estado" });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoComentario()
        {
            var mockedConsultaId = 1;
            var mockedComentarioId = 1;
            var cantidadArchivosMocked = 1;

            consultaServiceMock.Setup(x => x.AgregarAdjuntoComentario(It.IsAny<int>(), It.IsAny<int>(),It.IsAny<HttpFileCollectionBase>())).Returns(SuccessMsg.ArchivoSubidoOK);

            target = new ConsultaController(consultaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { data = SuccessMsg.ArchivoSubidoOK });

            resultado = target.Adjuntos(mockedConsultaId, mockedComentarioId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoComentarioConIdConsultaInvalido()
        {
            var mockedConsultaId = -1;
            var mockedComentarioId = 1;
            var cantidadArchivosMocked = 1;

            target = new ConsultaController(consultaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.Adjuntos(mockedConsultaId, mockedComentarioId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoComentarioConIdComentarioInvalido()
        {
            var mockedConsultaId = 1;
            var mockedComentarioId = -1;
            var cantidadArchivosMocked = 1;

            target = new ConsultaController(consultaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

            resultado = target.Adjuntos(mockedConsultaId, mockedComentarioId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoComentarioConIdComentarioInexistente()
        {
            var mockedConsultaId = 1;
            var mockedComentarioId = 1;
            var cantidadArchivosMocked = 1;

            consultaServiceMock.Setup(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>())).Throws(new InfoCustomException("No existe el comentario"));

            target = new ConsultaController(consultaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "No existe el comentario" });

            resultado = target.Adjuntos(mockedConsultaId, mockedComentarioId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoArchivoComentarioConIdConsultaDiferente()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;
            var cantidadArchivosMocked = 1;

            consultaServiceMock.Setup(x => x.AgregarAdjuntoComentario(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<HttpFileCollectionBase>())).Throws(new InfoCustomException("El comentario no corresponde a la consulta especificada"));

            target = new ConsultaController(consultaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "El comentario no corresponde a la consulta especificada" });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoArchivoComentarioSinAdjuntos()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;
            var cantidadArchivosMocked = 0;

            consultaServiceMock.Setup(x => x.AgregarAdjuntoComentario(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<HttpFileCollectionBase>())).Throws(new InfoCustomException("No se adjuntaron archivos"));

            target = new ConsultaController(consultaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "No se adjuntaron archivos" });

            resultado = target.ActualizarEstado(mockedConsultaId, mockedCategoriaId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        private void cargarFiles(int cantidadArchivos)
        {
            var fakeHttpContext = new Mock<HttpContextBase>();
            var fakeRequest = new Mock<HttpRequestBase>();
            var fakeFiles = new Mock<HttpFileCollectionBase>();

            fakeFiles.Setup(f => f.Count).Returns(cantidadArchivos);

            var files = fakeFiles.Object;

            fakeRequest.Setup(req => req.Files).Returns(files);

            var request = fakeRequest.Object;

            fakeHttpContext.Setup(ctx => ctx.Request).Returns(request);

            target.ControllerContext = new ControllerContext(fakeHttpContext.Object, new System.Web.Routing.RouteData(), target);
        }
    }
}
