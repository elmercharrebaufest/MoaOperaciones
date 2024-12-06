using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOA.Controllers;
using SustitucionMOA.Utils;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class CampoSustentableControllerTest
    {

        private CampoSustentableController target;
        private Mock<ICampoSustentableService> campoSustentableServiceMock;
        private Mock<IFileWrapper> fileWrapperMock;
        private Mock<IDataAgroService> dataAgroMock;
        private string expectedJson;
        private string resultJson;
        private string mailUsuario = "mail@mail.com";
        bool UsarArchivoId = false;

        [SetUp]
        public void SetUp()
        {
            campoSustentableServiceMock = new Mock<ICampoSustentableService>();
            fileWrapperMock = new Mock<IFileWrapper>();
            dataAgroMock = new Mock<IDataAgroService>();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, mailUsuario));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

            target = new CampoSustentableController(campoSustentableServiceMock.Object, fileWrapperMock.Object, dataAgroMock.Object);
        }

        [Test()]
        public void CampoProveedorAgregarTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            var expected = new Resultado
            {
                IdEntidad = 1,
                Mensaje = SuccessMsg.CampoSustentableAgregado
            };

            HttpPostedFileBase file = null;
            bool usarArchivo = false;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Agregar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>(), It.IsAny<bool>())).Returns(expected);

            var result = target.CampoProveedorAgregar(campoProveedorJson, file, usarArchivo);


            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorAgregarValidationCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Agregar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>(), It.IsAny<bool>())).Throws(new ValidationCustomException("Mensaje de error"));


            try
            {
                target.CampoProveedorAgregar(campoProveedorJson, file, UsarArchivoId);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void CampoProveedorAgregarInfoCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            var expected = "Mensaje de info";

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Agregar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>(), It.IsAny<bool>())).Throws(new InfoCustomException("Mensaje de info"));
            expectedJson = JsonConvert.SerializeObject(expected);

            try
            {
                target.CampoProveedorAgregar(campoProveedorJson, file, UsarArchivoId);
            }
            catch (InfoCustomException e)
            {
                resultJson = JsonConvert.SerializeObject(e.Message.ToString());
            }

            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorEditarTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 1, HectareasSoja = 100, HectareasTotales = 100 };

            var expected = new Resultado
            {
                IdEntidad = 1,
                Mensaje = SuccessMsg.CampoSustentableActualizado
            };

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            HttpPostedFileBase file = null;

            campoSustentableServiceMock.Setup(s => s.Editar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Returns(expected);

            var result = target.CampoProveedorEditar(campoProveedorJson, file);


            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorEditarValidationCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Editar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Throws(new ValidationCustomException("Mensaje de error"));

            try
            {
                target.CampoProveedorEditar(campoProveedorJson, file);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void CampoProveedorEditarInfoCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Editar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Throws(new InfoCustomException("Mensaje de info"));

            try
            {
                target.CampoProveedorEditar(campoProveedorJson, file);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de info", e.Message);
            }
        }

        [Test()]
        public void CamposProveedoresTest()
        {
            var camposProveedores = new List<CampoProveedorListadoDto>()
            {
                new CampoProveedorListadoDto{ NombreCampo = "Test" , HectareasSoja = 100, HectareasTotales = 100, NombreCosecha = "20-21", ToneladasAprobadas = 50 }
            };

            campoSustentableServiceMock.Setup(s => s.Listar(It.Is<string>(i => i == mailUsuario))).Returns(camposProveedores);

            var result = target.CamposProveedores();

            expectedJson = JsonConvert.SerializeObject(camposProveedores);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CamposProveedoresValidationCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.Listar(It.Is<string>(i => i == mailUsuario))).Throws(new ValidationCustomException("Mensaje de error"));

            try
            {
                target.CamposProveedores();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void CamposProveedoresInfoCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.Listar(It.Is<string>(i => i == mailUsuario))).Throws(new InfoCustomException("Mensaje de info"));

            try
            {
                target.CamposProveedores();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de info", e.Message);
            }
        }

        [Test()]
        public void CampoProveedorTest()
        {
            var campoProveedor = new CampoProveedorDto { NombreCampo = "Test", HectareasSoja = 100, HectareasTotales = 100, NombreCosecha = "20-21", ToneladasAprobadas = 50 };

            int proveedorId = 1;
            int campoCosechaId = 2;

            campoSustentableServiceMock.Setup(s => s.ObtenerCampo(It.Is<string>(i => i == mailUsuario),
                                                                  It.Is<int>(i => i == proveedorId),
                                                                  It.Is<int>(i => i == campoCosechaId))).Returns(campoProveedor);

            var result = target.CampoProveedor(proveedorId, campoCosechaId);

            expectedJson = JsonConvert.SerializeObject(campoProveedor);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorValidationCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.ObtenerCampo(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<int>(),
                                                                  It.IsAny<int>())).Throws(new ValidationCustomException("Mensaje de error"));


            try
            {
                target.CampoProveedor(1, 2);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void CampoProveedorInfoCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.ObtenerCampo(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<int>(),
                                                                  It.IsAny<int>())).Throws(new InfoCustomException("Mensaje de info"));


            try
            {
                target.CampoProveedor(1, 2);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de info", e.Message);
            }
        }

        [Test()]
        public void CosechasTest()
        {
            var cosechas = new List<Cosecha>
            {
                new Cosecha { Id = 1, Nombre="19-20", Inicio = DateTime.Now, Fin = DateTime.Now.AddDays(1)},
                new Cosecha { Id = 2, Nombre="20-21", Inicio = DateTime.Now, Fin = DateTime.Now.AddDays(1)}
            };

            campoSustentableServiceMock.Setup(s => s.ObtenerCosechas(true)).Returns(cosechas);

            var result = target.Cosechas(true);

            expectedJson = JsonConvert.SerializeObject(cosechas);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CosechasValidationCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.ObtenerCosechas(true)).Throws(new ValidationCustomException("Mensaje de error"));

            try
            {
                target.Cosechas(true);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void CosechasInfoCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.ObtenerCosechas(true)).Throws(new InfoCustomException("Mensaje de info"));

            try
            {
                target.Cosechas(true);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de info", e.Message);
            }
        }

        [Test()]
        public void VerificarDeclaracionTest()
        {
            var firmaDto = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = false,
            };

            int proveedorId = 1;
            int cosechaId = 1;
            string CUIT = "23333333333";

            campoSustentableServiceMock.Setup(s => s.VerificarDeclaracion(It.Is<int>(i => i == proveedorId), It.IsAny<int>(), It.IsAny<string>())).Returns(firmaDto);

            var result = target.VerificarDeclaracion(proveedorId, cosechaId, CUIT);

            expectedJson = JsonConvert.SerializeObject(firmaDto);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void VerificarDeclaracionValidationCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.VerificarDeclaracion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Throws(new ValidationCustomException("Mensaje de error"));

            try
            {
                target.VerificarDeclaracion(1, 1, "");

            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void VerificarDeclaracionInfoCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.VerificarDeclaracion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Throws(new InfoCustomException("Mensaje de info"));


            try
            {
                target.VerificarDeclaracion(1, 1, "");
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de info", e.Message);
            }
        }

        [Test()]
        public void FirmarDeclaracionTest()
        {
            int proveedorId = 1;

            var expected = SuccessMsg.DeclaracionCampoSustentableFirmada;

            campoSustentableServiceMock.Setup(s => s.AdjuntarDeclaracionFirmada(
                It.Is<string>(i => i == mailUsuario),
                It.Is<int>(i => i == proveedorId),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<HttpPostedFileBase>()
            )).Returns(expected);

            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.FileName).Returns("file1.pdf");

            var result = target.AdjuntarDeclaracionFirmada(1, 1, "2333333333", fileMock.Object);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void FirmarDeclaracionValidationCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.AdjuntarDeclaracionFirmada(
                It.Is<string>(i => i == mailUsuario),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<HttpPostedFileBase>())).Throws(new ValidationCustomException("Mensaje de error"));

            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.FileName).Returns("file1.pdf");

            try
            {
                target.AdjuntarDeclaracionFirmada(1, 1, "2333333333", fileMock.Object);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de error", e.Message);
            }
        }


        [Test()]
        public void FirmarDeclaracionInfoCustomExceptionTest()
        {
            campoSustentableServiceMock.Setup(s => s.AdjuntarDeclaracionFirmada(It.Is<string>(i => i == mailUsuario),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<HttpPostedFileBase>())).Throws(new InfoCustomException("Mensaje de info"));

            var fileMock = new Mock<HttpPostedFileBase>();
            fileMock.Setup(x => x.FileName).Returns("file1.pdf");

            try
            {
                target.AdjuntarDeclaracionFirmada(1, 1, "2333333333", fileMock.Object);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Mensaje de info", e.Message);
            }
        }

        [Test]
        public void CampoProveedorBorrarOk()
        {
            int campoCosechaIdTest = 32;
            int proveedorIdTest = 92;

            this.campoSustentableServiceMock.Setup(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest)).Returns("resultado");

            JsonResult result = target.CampoProveedorBorrar(campoCosechaIdTest, proveedorIdTest);

            Assert.AreEqual("resultado", result.Data);

            this.campoSustentableServiceMock.Verify(x => x.Borrar(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void CampoProveedorBorrarInfoCustomException()
        {
            int campoCosechaIdTest = 32;
            int proveedorIdTest = 92;

            InfoCustomException infoCustomExceptionTest = new InfoCustomException("mensaje excepcion");

            this.campoSustentableServiceMock.Setup(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest)).Throws(infoCustomExceptionTest);
            try
            {
                target.CampoProveedorBorrar(campoCosechaIdTest, proveedorIdTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("mensaje excepcion", e.Message);
            }
            this.campoSustentableServiceMock.Verify(x => x.Borrar(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void CampoProveedorBorrarValidationCustomException()
        {
            int campoCosechaIdTest = 32;
            int proveedorIdTest = 92;

            ValidationCustomException infoCustomExceptionTest = new ValidationCustomException("mensaje excepcion");

            this.campoSustentableServiceMock.Setup(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest)).Throws(infoCustomExceptionTest);

            try
            {
                target.CampoProveedorBorrar(campoCosechaIdTest, proveedorIdTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("mensaje excepcion", e.Message);
            }

            this.campoSustentableServiceMock.Verify(x => x.Borrar(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void CampoProveedorBorrarException()
        {
            int campoCosechaIdTest = 32;
            int proveedorIdTest = 92;

            Exception infoCustomExceptionTest = new Exception("mensaje excepcion");

            this.campoSustentableServiceMock.Setup(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest)).Throws(infoCustomExceptionTest);
            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.CampoProveedorBorrar(campoCosechaIdTest, proveedorIdTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("mensaje excepcion", e.Message);
            }

            this.campoSustentableServiceMock.Verify(x => x.Borrar(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.Borrar(mailUsuario, campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void DescargarArchivoKMZOk()
        {
            int campoCosechaIdTest = 123;
            int proveedorIdTest = 33;

            string rutaArchivoTest = "ruta/archivo.pdf";
            this.campoSustentableServiceMock.Setup(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest)).Returns(rutaArchivoTest);

            byte[] byteArrayTest = { 1, 2, 3 };
            this.fileWrapperMock.Setup(x => x.ReadAllBytes(rutaArchivoTest)).Returns(byteArrayTest);

            JsonResult result = target.DescargarArchivoKMZ(campoCosechaIdTest, proveedorIdTest);

            Assert.IsInstanceOf<FileContentResult>(result.Data);

            FileContentResult fileContentResultData = (FileContentResult)result.Data;

            Assert.AreEqual("application/octet-stream", fileContentResultData.ContentType);
            Assert.AreEqual(byteArrayTest, fileContentResultData.FileContents);
            Assert.AreEqual("archivo.pdf", fileContentResultData.FileDownloadName);

            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void DescargarArchivoKMZInfoCustomException()
        {
            int campoCosechaIdTest = 123;
            int proveedorIdTest = 33;

            InfoCustomException infoCustomExceptionTest = new InfoCustomException("excepcion");
            this.campoSustentableServiceMock.Setup(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest)).Throws(infoCustomExceptionTest);

            try
            {
                target.DescargarArchivoKMZ(campoCosechaIdTest, proveedorIdTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("excepcion", e.Message);
            }

            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void DescargarArchivoKMZValidationCustomException()
        {
            int campoCosechaIdTest = 123;
            int proveedorIdTest = 33;

            ValidationCustomException validationCustomExceptionTest = new ValidationCustomException("excepcion");
            this.campoSustentableServiceMock.Setup(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest)).Throws(validationCustomExceptionTest);

            try
            {
                target.DescargarArchivoKMZ(campoCosechaIdTest, proveedorIdTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("excepcion", e.Message);
            }

            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest), Times.Once);
        }

        [Test]
        public void DescargarArchivoKMZException()
        {
            int campoCosechaIdTest = 123;
            int proveedorIdTest = 33;

            Exception validationCustomExceptionTest = new Exception("excepcion");
            this.campoSustentableServiceMock.Setup(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest)).Throws(validationCustomExceptionTest);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(new StringWriter()));

            try
            {
                target.DescargarArchivoKMZ(campoCosechaIdTest, proveedorIdTest);
            }
            catch (Exception e)
            {
                Assert.AreEqual("excepcion", e.Message);
            }

            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            this.campoSustentableServiceMock.Verify(x => x.ObtenerRutaArchivoKMZ(campoCosechaIdTest, proveedorIdTest), Times.Once);
        }
    }
}
