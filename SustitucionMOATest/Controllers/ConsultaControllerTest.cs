using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
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
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    public class ConsultaControllerTest
    {
        private ConsultaController target;
        private Mock<IConsultaService> consultaServiceMock;
        private Mock<IUsuarioService> usuarioServiceMock;
        private string expectedJson;
        private string resultJson;
        private JsonResult resultado;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            consultaServiceMock = new Mock<IConsultaService>();
            usuarioServiceMock = new Mock<IUsuarioService>();

            repositorioMock = new Mock<IRepositorio>();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, "mail@mail.com"));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = "mail@mail.com"
            };

            var usuario = new Usuario
            {
                Id = 0,
                Mail = "mail@mail.com",
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } },
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Corredor", NombreCorto = "A"},
                Habilitado = true,
                CUITRegistro = "3030303030",
            };

            var usuarioDto = new UsuarioDto(usuario);

            usuarioServiceMock.Setup(x => x.GetUsuario(It.IsAny<string>())).Returns(usuarioDto);

            repositorioMock
             .Setup(y => y.Obtener<UsuarioDto>(It.IsAny<string>()))
             .Returns(usuarioDto);

            Thread.CurrentPrincipal = principal;
        }

        [Test]
        public void GetConsutaDetalle()
        {
            var mockedId = 1;
            var mockedCodigoCorredo = "C22002937";
            var mockedCodigoProveedor = "06422850";

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = "mail@mail.com"
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = "mail@mail.com",
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } }
            };

            var categoria = new Categoria
            {
                Id = 1,
                Code = "NOS",
                Nombre = "Nose",
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } }
            };

            var subcategoria = new SubCategoria
            {
                Id = 1,
                Code = "NOS",
                Nombre = "Nose",
                Categoria_Id = 1,
                Categoria = categoria
            };

            var estadoConsulta = new EstadoConsulta
            {
                Id = 1,
                Code = "NOS",
                Color = "#707070",
                Descripcion = "NOSE"
            };

            var consultaMock = new Consulta()
            {
                Id = 1,
                CodigoCorredor = "2020",
                RazonSocialCorredor = "TEST",
                CodigoProveedor = "2020",
                RazonSocialProveedor = "TEST",
                Usuario_Id = 1,
                Usuario = usuario,
                Asunto = "TEST",
                Categoria_Id = 1,
                Categoria = categoria,
                SubCategoria_Id = 1,
                SubCategoria = subcategoria,
                FechaCreacion = DateTime.Now,
                FechaUltimaModificacion = DateTime.Now,
                EstadoConsulta_Id = 1,
                EstadoConsulta = estadoConsulta,
                Detalle = null,
                Comentarios = null,
            };

            var consultaDto = new ConsultaDto(consultaMock);

            consultaServiceMock.Setup(x => x.ObtenerConsulta(It.IsAny<int>())).Returns(consultaDto);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(consultaDto);

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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id de consulta inválido" });
            
            resultado = target.Detalle(mockedId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostComentario()
        {
            var mockedConsultaId = 1;

            var proveedor = new Proveedor
            {
                Id = 1,
                EstadoAprobacion = EstadoAprobacion.Deshabilitado,
                Observaciones = "Test",
                RazonSocial = "RS",
                Mail = "mail@mail.com"
            };

            var usuario = new Usuario
            {
                Id = 1,
                Mail = "mail@mail.com",
                Proveedores = new List<Proveedor> { proveedor },
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } }
            };

            var categoria = new Categoria
            {
                Id = 1,
                Code = "NOS",
                Nombre = "Nose",
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } }
            };

            var subcategoria = new SubCategoria
            {
                Id = 1,
                Code = "NOS",
                Nombre = "Nose",
                Categoria_Id = 1,
                Categoria = categoria
            };

            var estadoConsulta = new EstadoConsulta
            {
                Id = 1,
                Code = "NOS",
                Color = "#707070",
                Descripcion = "NOSE"
            };

            var consultaMock = new Consulta()
            {
                Id = 1,
                CodigoCorredor = "2020",
                RazonSocialCorredor = "TEST",
                CodigoProveedor = "2020",
                RazonSocialProveedor = "TEST",
                Usuario_Id = 1,
                Usuario = usuario,
                Asunto = "TEST",
                Categoria_Id = 1,
                Categoria = categoria,
                SubCategoria_Id = 1,
                SubCategoria = subcategoria,
                FechaCreacion = DateTime.Now,
                FechaUltimaModificacion = DateTime.Now,
                EstadoConsulta_Id = 1,
                EstadoConsulta = estadoConsulta,
                Detalle = null,
                Comentarios = null,
            };

            Comentario comentario = new Comentario()
            {
                Id = 1,
                Detalle = "Comentario Test",
                Fecha = DateTime.Now,
                Usuario_Id = 1,
                Consulta_Id = 1,
                Consulta = consultaMock,
                Archivos = null,
                Usuario = usuario,
            };

            var comentarioDto = new ComentarioDto(comentario);

            consultaServiceMock.Setup(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<Comentario>())).Returns(comentarioDto);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(comentarioDto);

            resultado = target.Comentarios(mockedConsultaId, comentario);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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
            HttpFileCollectionBase files;

            consultaServiceMock.Setup(x => x.AgregarAdjuntoComentario(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<HttpFileCollectionBase>())).Throws(new InfoCustomException("No existe el comentario"));

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
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
            var mockedConsultaId = 2;
            var mockedComentarioId = 1;
            var cantidadArchivosMocked = 1;

            consultaServiceMock.Setup(x => x.AgregarAdjuntoComentario(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<HttpFileCollectionBase>())).Throws(new InfoCustomException("El comentario no corresponde a la consulta especificada"));

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "El comentario no corresponde a la consulta especificada" });

            resultado = target.Adjuntos(mockedConsultaId, mockedComentarioId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test]
        public void PostAdjuntoArchivoComentarioSinAdjuntos()
        {
            var mockedConsultaId = 1;
            var mockedComentarioId = 1;
            var cantidadArchivosMocked = 0;

            consultaServiceMock.Setup(x => x.AgregarAdjuntoComentario(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<HttpFileCollectionBase>())).Throws(new InfoCustomException("No se adjuntaron archivos"));

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "No se adjuntaron archivos" });

            resultado = target.Adjuntos(mockedConsultaId, mockedComentarioId);
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
