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
using System.Linq.Expressions;
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
        private Mock<IVendedorService> vendedorServiceMock;
        private Mock<IOrdenDeCargaService> ordenDeCargaServiceMock;
        private string expectedJson;
        private string resultJson;
        private JsonResult resultado;
        private Mock<IRepositorio> repositorioMock;

        [SetUp]
        public void SetUp()
        {
            consultaServiceMock = new Mock<IConsultaService>();
            usuarioServiceMock = new Mock<IUsuarioService>();
            vendedorServiceMock = new Mock<IVendedorService>();
            ordenDeCargaServiceMock = new Mock<IOrdenDeCargaService>();
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
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } },
                TipoUsuario = new TipoUsuario { NombreCorto = "G"} 
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id de consulta inválido" });
            
            resultado = target.Detalle(mockedId);
            resultJson = JsonConvert.SerializeObject(resultado.Data);

            Assert.NotNull(resultado);
            Assert.NotNull(resultado.Data);
            Assert.AreEqual(expectedJson, resultJson);
        }
        /*
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
                Roles = new List<Rol> { new Rol { Nombre = "GRANOS", Codigo = "GRAN" } },
                TipoUsuario = new TipoUsuario { NombreCorto = "G"}
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
                FechaRecordado = DateTime.Now,
                Recordado = false
            };

            var comentarioDto = new ComentarioDto(comentario);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                 .Returns(usuario);

            consultaServiceMock.Setup(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>())).Returns(comentarioDto);
            var comentarioJson = JsonConvert.SerializeObject(comentario).ToString();


            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(comentarioDto);

            resultado = target.Comentarios(mockedConsultaId, comentarioJson);
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
        }*/

        [Test]
        public void PatchRecategorizarConsulta()
        {
            var mockedConsultaId = 1;
            var mockedCategoriaId = 1;
            var mockedSubCategoriaId = 1;

            consultaServiceMock.Setup(x => x.RecategorizarConsulta(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            cargarFiles(cantidadArchivosMocked);
            expectedJson = JsonConvert.SerializeObject(new { info = "Id inválido" });

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

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
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

        [Test]
        public void ConsultaOk()
        {
            var consultaTest = new Consulta
            {
                Id = 13232,
            };
            string consultaJsonTest = JsonConvert.SerializeObject(consultaTest);

            var comentarioTest = new Comentario
            {
                Id = 423,
            };
            string comentarioJsonTest = JsonConvert.SerializeObject(comentarioTest);

            var httpFileCollectionMock = new Mock<HttpFileCollectionBase>();

            var httpRequestMock = new Mock<HttpRequestBase>();
            httpRequestMock.Setup(x => x.Files).Returns(httpFileCollectionMock.Object);
            
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            target.ControllerContext = new ControllerContext(httpContextMock.Object, new System.Web.Routing.RouteData(), target);

            consultaServiceMock
            .Setup(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()))
            .Returns<Consulta, Comentario, HttpFileCollectionBase>((cons, com, files) => 
            {
                return new AgregarConsultaResponseDto 
                {
                    Mensaje = "ok",
                    ConsultaDto = new ConsultaDto { 
                        Id = cons.Id, 
                        Comentarios = new List<ComentarioDto> { 
                            new ComentarioDto { Id = com.Id } 
                        },
                    },
                };
            });

            var result = target.Consulta(consultaJsonTest, comentarioJsonTest);

            Assert.IsInstanceOf<AgregarConsultaResponseDto>(result.Data);

            var resultData = (AgregarConsultaResponseDto)result.Data;

            Assert.AreEqual("ok", resultData.Mensaje);
            Assert.AreEqual(13232, resultData.ConsultaDto.Id);
            Assert.AreEqual(1, resultData.ConsultaDto.Comentarios.Count);
            Assert.AreEqual(423, resultData.ConsultaDto.Comentarios[0].Id);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Once);
            //this.consultaServiceMock.Verify(x => x.AgregarConsulta(consultaTest, comentarioTest, httpFileCollectionMock.Object), Times.Once);
        }

        [Test]
        public void ConsultaInfoCustomException()
        {
            var consultaTest = new Consulta
            {
                Id = 13232,
            };
            string consultaJsonTest = JsonConvert.SerializeObject(consultaTest);

            var comentarioTest = new Comentario
            {
                Id = 423,
            };
            string comentarioJsonTest = JsonConvert.SerializeObject(comentarioTest);

            InfoCustomException exceptionTest = new InfoCustomException("excepcion");

            this.usuarioServiceMock.Setup(x => x.GetUsuario("mail@mail.com")).Throws(exceptionTest);
            
            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);

            var result = target.Consulta(consultaJsonTest, comentarioJsonTest);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual(exceptionTest.Message, infoResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Never);
        }

        [Test]
        public void ConsultaValidationCustomException()
        {
            var consultaTest = new Consulta
            {
                Id = 13232,
            };
            string consultaJsonTest = JsonConvert.SerializeObject(consultaTest);

            var comentarioTest = new Comentario
            {
                Id = 423,
            };
            string comentarioJsonTest = JsonConvert.SerializeObject(comentarioTest);

            ValidationCustomException exceptionTest = new ValidationCustomException("excepcion");

            this.usuarioServiceMock.Setup(x => x.GetUsuario("mail@mail.com")).Throws(exceptionTest);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);

            var result = target.Consulta(consultaJsonTest, comentarioJsonTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual(exceptionTest.Message, errorResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Never);
        }

        [Test]
        public void ConsultaValidationCustomExceptionLoguearMensaje()
        {
            var consultaTest = new Consulta
            {
                Id = 13232,
            };
            string consultaJsonTest = JsonConvert.SerializeObject(consultaTest);

            var comentarioTest = new Comentario
            {
                Id = 423,
            };
            string comentarioJsonTest = JsonConvert.SerializeObject(comentarioTest);

            Exception innerExceptionTest = new Exception("excepcion interna");
            ValidationCustomException exceptionTest = new ValidationCustomException("excepcion", innerExceptionTest, true);

            this.usuarioServiceMock.Setup(x => x.GetUsuario("mail@mail.com")).Throws(exceptionTest);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new System.IO.StringWriter()));

            var result = target.Consulta(consultaJsonTest, comentarioJsonTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual(exceptionTest.Message, errorResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Never);
        }

        [Test]
        public void ConsultaException()
        {
            var consultaTest = new Consulta
            {
                Id = 13232,
            };
            string consultaJsonTest = JsonConvert.SerializeObject(consultaTest);

            var comentarioTest = new Comentario
            {
                Id = 423,
            };
            string comentarioJsonTest = JsonConvert.SerializeObject(comentarioTest);

            Exception exceptionTest = new Exception("excepcion");

            this.usuarioServiceMock.Setup(x => x.GetUsuario("mail@mail.com")).Throws(exceptionTest);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new System.IO.StringWriter()));

            var result = target.Consulta(consultaJsonTest, comentarioJsonTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", errorResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Never);
        }

        [Test]
        public void AnularConsultaOk()
        {
            int consultaIdTest = 123;
            string motivoRechazoTest = "Prueba loca";

            this.consultaServiceMock
                .Setup(x => x.AnularConsulta(consultaIdTest, 0, motivoRechazoTest))
                .Returns("Anulada ok");

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            var result = target.AnularConsulta(consultaIdTest, motivoRechazoTest);

            string mensajeResultData = result.Data.GetType().GetProperty("Mensaje").GetValue(result.Data).ToString();

            Assert.AreEqual("Anulada ok", mensajeResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AnularConsulta(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            this.consultaServiceMock.Verify(x => x.AnularConsulta(consultaIdTest, 0, motivoRechazoTest), Times.Once);
        }

        [Test]
        public void AnularConsultaInfoCustomException()
        {
            int consultaIdTest = 123;
            string motivoRechazoTest = "Prueba loca";

            InfoCustomException exceptionTest = new InfoCustomException("excepcion");

            this.consultaServiceMock
                .Setup(x => x.AnularConsulta(consultaIdTest, 0, motivoRechazoTest))
                .Throws(exceptionTest);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            var result = target.AnularConsulta(consultaIdTest, motivoRechazoTest);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual(exceptionTest.Message, infoResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AnularConsulta(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            this.consultaServiceMock.Verify(x => x.AnularConsulta(consultaIdTest, 0, motivoRechazoTest), Times.Once);
        }

        [Test]
        public void AnularConsultaValidationCustomException()
        {
            int consultaIdTest = 123;
            string motivoRechazoTest = "Prueba loca";

            ValidationCustomException exceptionTest = new ValidationCustomException("excepcion");

            this.consultaServiceMock
                .Setup(x => x.AnularConsulta(consultaIdTest, 0, motivoRechazoTest))
                .Throws(exceptionTest);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            var result = target.AnularConsulta(consultaIdTest, motivoRechazoTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual(exceptionTest.Message, errorResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Never);
        }

        [Test]
        public void AnularConsultaException()
        {
            int consultaIdTest = 123;
            string motivoRechazoTest = "Prueba loca";

            Exception exceptionTest = new NullReferenceException("excepcion random");

            this.consultaServiceMock
                .Setup(x => x.AnularConsulta(consultaIdTest, 0, motivoRechazoTest))
                .Throws(exceptionTest);

            target = new ConsultaController(consultaServiceMock.Object, usuarioServiceMock.Object, vendedorServiceMock.Object, ordenDeCargaServiceMock.Object);
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new System.IO.StringWriter()));
            
            var result = target.AnularConsulta(consultaIdTest, motivoRechazoTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", errorResultData);

            this.usuarioServiceMock.Verify(x => x.GetUsuario(It.IsAny<string>()), Times.Once);
            this.usuarioServiceMock.Verify(x => x.GetUsuario("mail@mail.com"), Times.Once);

            this.consultaServiceMock.Verify(x => x.AgregarConsulta(It.IsAny<Consulta>(), It.IsAny<Comentario>(), It.IsAny<HttpFileCollectionBase>()), Times.Never);
        }
    }
}
