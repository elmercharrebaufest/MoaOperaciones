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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class CampoSustentableControllerTest
    {

        private CampoSustentableController target;
        private Mock<ICampoSustentableService> campoSustentableServiceMock;
        private string expectedJson;
        private string resultJson;
        private string mailUsuario = "mail@mail.com";

        [SetUp]
        public void SetUp()
        {
            campoSustentableServiceMock = new Mock<ICampoSustentableService>();

            var fakeIdentity = new GenericIdentity("User");

            var claims = (ClaimsIdentity)fakeIdentity;

            claims.AddClaim(new Claim(Globals.ClaimsUserNameType, mailUsuario));
            claims.AddClaim(new Claim(Globals.ClaimsNombreType, "mail"));

            var principal = new GenericPrincipal(fakeIdentity, null);

            Thread.CurrentPrincipal = principal;

            target = new CampoSustentableController(campoSustentableServiceMock.Object);
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

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Agregar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Returns(expected);

            var result = target.CampoProveedorAgregar(campoProveedorJson, file);


            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorAgregarValidationCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            var expected = @"{ error = Mensaje de error }";

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Agregar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.CampoProveedorAgregar(campoProveedorJson, file);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CampoProveedorAgregarInfoCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            var expected = @"{ info = Mensaje de info }";

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Agregar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.CampoProveedorAgregar(campoProveedorJson, file);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
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

            var expected = @"{ error = Mensaje de error }";

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Editar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.CampoProveedorEditar(campoProveedorJson, file);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CampoProveedorEditarInfoCustomExceptionTest()
        {
            var campoProveedor = new CampoProveedor { Proveedor_Id = 1, CampoCosecha_Id = 0, HectareasSoja = 100, HectareasTotales = 100 };

            var expected = @"{ info = Mensaje de info }";

            HttpPostedFileBase file = null;

            string campoProveedorJson = JsonConvert.SerializeObject(campoProveedor);

            campoSustentableServiceMock.Setup(s => s.Editar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<CampoProveedor>(),
                                                                  It.IsAny<HttpPostedFileBase>())).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.CampoProveedorEditar(campoProveedorJson, file);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorBorrarTest()
        {
            int proveedorId = 1;
            int campoCosechaId = 2;
            var expected = SuccessMsg.CampoSustentableBorrado;

            campoSustentableServiceMock.Setup(s => s.Borrar(It.Is<string>(i => i == mailUsuario),
                                                                  It.Is<int>(i => i == campoCosechaId),
                                                                  It.Is<int>(i => i == proveedorId))).Returns(expected);

            var result = target.CampoProveedorBorrar(campoCosechaId, proveedorId);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CampoProveedorBorrarValidationCustomExceptionTest()
        {
            var expected = @"{ error = Mensaje de error }";

            campoSustentableServiceMock.Setup(s => s.Borrar(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<int>(),
                                                                  It.IsAny<int>())).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.CampoProveedorBorrar(1, 1);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CampoProveedorBorrarInfoCustomExceptionTest()
        {
            var expected = @"{ info = Mensaje de info }";

            campoSustentableServiceMock.Setup(s => s.Borrar(It.Is<string>(i => i == mailUsuario),
                                                                 It.IsAny<int>(),
                                                                 It.IsAny<int>())).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.CampoProveedorBorrar(1, 1);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
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
            var expected = @"{ error = Mensaje de error }";

            campoSustentableServiceMock.Setup(s => s.Listar(It.Is<string>(i => i == mailUsuario))).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.CamposProveedores();

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CamposProveedoresInfoCustomExceptionTest()
        {
            var expected = @"{ info = Mensaje de info }";

            campoSustentableServiceMock.Setup(s => s.Listar(It.Is<string>(i => i == mailUsuario))).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.CamposProveedores();

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
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
            var expected = @"{ error = Mensaje de error }";

            campoSustentableServiceMock.Setup(s => s.ObtenerCampo(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<int>(),
                                                                  It.IsAny<int>())).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.CampoProveedor(1, 2);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CampoProveedorInfoCustomExceptionTest()
        {
            var expected = @"{ info = Mensaje de info }";

            campoSustentableServiceMock.Setup(s => s.ObtenerCampo(It.Is<string>(i => i == mailUsuario),
                                                                  It.IsAny<int>(),
                                                                  It.IsAny<int>())).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.CampoProveedor(1, 2);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void CosechasTest()
        {
            var cosechas = new List<Cosecha>
            {
                new Cosecha { Id = 1, Nombre="19-20", Inicio = DateTime.Now, Fin = DateTime.Now.AddDays(1)},
                new Cosecha { Id = 2, Nombre="20-21", Inicio = DateTime.Now, Fin = DateTime.Now.AddDays(1)}
            };

            campoSustentableServiceMock.Setup(s => s.ObtenerCosechas()).Returns(cosechas);

            var result = target.Cosechas();

            expectedJson = JsonConvert.SerializeObject(cosechas);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CosechasValidationCustomExceptionTest()
        {
            var expected = @"{ error = Mensaje de error }";

            campoSustentableServiceMock.Setup(s => s.ObtenerCosechas()).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.Cosechas();

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void CosechasInfoCustomExceptionTest()
        {
            var expected = @"{ info = Mensaje de info }";

            campoSustentableServiceMock.Setup(s => s.ObtenerCosechas()).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.Cosechas();

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void VerificarDeclaracionTest()
        {
            var firmaDto = new EstadoDeclaracionSustentableDto
            {
                DeclaracionFirmada = false,
            };

            int proveedorId = 1;

            campoSustentableServiceMock.Setup(s => s.VerificarDeclaracion(It.Is<int>(i => i == proveedorId))).Returns(firmaDto);

            var result = target.VerificarDeclaracion(proveedorId);

            expectedJson = JsonConvert.SerializeObject(firmaDto);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void VerificarDeclaracionValidationCustomExceptionTest()
        {
            var expected = @"{ error = Mensaje de error }";

            campoSustentableServiceMock.Setup(s => s.VerificarDeclaracion(It.IsAny<int>())).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.VerificarDeclaracion(1);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void VerificarDeclaracionInfoCustomExceptionTest()
        {
            var expected = @"{ info = Mensaje de info }";

            campoSustentableServiceMock.Setup(s => s.VerificarDeclaracion(It.IsAny<int>())).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.VerificarDeclaracion(1);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void FirmarDeclaracionTest()
        {
            int proveedorId = 1;
            double hectareasTotales = 100;

            var expected = SuccessMsg.DeclaracionCampoSustentableFirmada;

            campoSustentableServiceMock.Setup(s => s.FirmarDeclaracion(
                It.Is<string>(i => i == mailUsuario),
                It.Is<int>(i => i == proveedorId),
                It.Is<double>(i => i == hectareasTotales))).Returns(expected);

            var result = target.FirmarDeclaracion(proveedorId, hectareasTotales);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data);

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }

        [Test()]
        public void FirmarDeclaracionValidationCustomExceptionTest()
        {
            var expected = @"{ error = Mensaje de error }";

            campoSustentableServiceMock.Setup(s => s.FirmarDeclaracion(It.Is<string>(i => i == mailUsuario),
                                                                       It.IsAny<int>(),
                                                                       It.IsAny<double>())).Throws(new ValidationCustomException("Mensaje de error"));

            var result = target.FirmarDeclaracion(1, 100);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }


        [Test()]
        public void FirmarDeclaracionInfoCustomExceptionTest()
        {
            var expected = @"{ info = Mensaje de info }";

            campoSustentableServiceMock.Setup(s => s.FirmarDeclaracion(It.Is<string>(i => i == mailUsuario),
                                                                      It.IsAny<int>(),
                                                                      It.IsAny<double>())).Throws(new InfoCustomException("Mensaje de info"));

            var result = target.FirmarDeclaracion(1, 100);

            expectedJson = JsonConvert.SerializeObject(expected);
            resultJson = JsonConvert.SerializeObject(result.Data.ToString());

            Assert.NotNull(result);
            Assert.AreEqual(expectedJson, resultJson);
        }
    }
}
