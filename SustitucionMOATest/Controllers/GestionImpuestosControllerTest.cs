using Microsoft.QualityTools.Testing.Fakes;
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
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace SustitucionMOATest.Controllers
{
    [TestFixture()]
    public class GestionImpuestosControllerTest
    {
        GestionImpuestosController target;

        Mock<IGestionImpuestosService> gestionImpuestosServiceMock;

        [SetUp]
        public void SetUp()
        {
            this.gestionImpuestosServiceMock = new Mock<IGestionImpuestosService>();

            this.target = new GestionImpuestosController(gestionImpuestosServiceMock.Object);
        }

        [Test]
        public void ListarCabecerasOk()
        {
            var ingresosBrutosCoeficienteUnificadoDtoList = new List<IngresosBrutosCoeficienteUnificadoDto>
            {
                new  IngresosBrutosCoeficienteUnificadoDto { Id = 1, },
                new  IngresosBrutosCoeficienteUnificadoDto { Id = 2, },
                new  IngresosBrutosCoeficienteUnificadoDto { Id = 3, },
            };

            this.gestionImpuestosServiceMock.Setup(g => g.ListarCabeceras()).Returns(ingresosBrutosCoeficienteUnificadoDtoList);

            var result = target.ListarCabeceras();

            Assert.IsInstanceOf<List<IngresosBrutosCoeficienteUnificadoDto>>(result.Data);

            List<IngresosBrutosCoeficienteUnificadoDto> resultData = (List<IngresosBrutosCoeficienteUnificadoDto>)result.Data;
            Assert.AreEqual(3, resultData.Count);

            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDtoList[0], resultData[0]);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDtoList[1], resultData[1]);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDtoList[2], resultData[2]);

            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarCabecerasInfoCustomException()
        {
            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarCabeceras())
                .Throws(excepcionTest);

            var result = target.ListarCabeceras();

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        [Test]
        public void ListarCabecerasValidationCustomException()
        {
            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarCabeceras())
                .Throws(excepcionTest);

            var result = target.ListarCabeceras();

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        }

        //[Test]
        //public void ListarCabecerasException()
        //{
        //    var excepcionTest = new NullReferenceException("exploto molinos");

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.ListarCabeceras())
        //        .Throws(excepcionTest);

        //    Exception excepcionResultante = null;

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(null));
        //        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);

        //        SustitucionMOAUtils.Logger.Fakes.ShimLog.ErrorStringStringStringStringException = (s1, s2, s3, s4, ex) => { excepcionResultante = ex; };

        //        result = target.ListarCabeceras();
        //    }

        //    Assert.IsNotNull(result.Data);

        //    string resultDataError = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();
        //    Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", resultDataError);
        //    Assert.AreEqual("exploto molinos", excepcionResultante.Message);

        //    this.gestionImpuestosServiceMock.Verify(g => g.ListarCabeceras(), Times.Once);
        //}

        [Test]
        public void ListarDetallesOk()
        {
            int idCabeceraTest = 3;

            var ingresosBrutosCoeficienteUnificadoDetalleDtoList = new List<IngresosBrutosCoeficienteUnificadoDetalleDto>
            {
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 1, IdCabecera = 1, },
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 2, IdCabecera = 2, },
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 3, IdCabecera = 3, },
                new  IngresosBrutosCoeficienteUnificadoDetalleDto { Id = 4, IdCabecera = 3, },
            };

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(It.IsAny<int>()))
                .Returns<int>(idCabecera => ingresosBrutosCoeficienteUnificadoDetalleDtoList.Where(x => x.IdCabecera == idCabecera).ToList());

            var result = target.ListarDetalles(idCabeceraTest);

            Assert.IsInstanceOf<List<IngresosBrutosCoeficienteUnificadoDetalleDto>>(result.Data);

            List<IngresosBrutosCoeficienteUnificadoDetalleDto> resultData = (List<IngresosBrutosCoeficienteUnificadoDetalleDto>)result.Data;
            Assert.AreEqual(2, resultData.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetalleDtoList[2], resultData[0]);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetalleDtoList[3], resultData[1]);

            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ListarDetallesInfoCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new InfoCustomException("Algo");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(idCabeceraTest))
                .Throws(excepcionTest);

            var result = target.ListarDetalles(idCabeceraTest);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(idCabeceraTest), Times.Once);
        }

        [Test]
        public void ListarDetallesValidationCustomException()
        {
            int idCabeceraTest = 1;

            var excepcionTest = new ValidationCustomException("Error de validacion");

            this.gestionImpuestosServiceMock
                .Setup(g => g.ListarDetalles(idCabeceraTest))
                .Throws(excepcionTest);

            var result = target.ListarDetalles(idCabeceraTest);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(idCabeceraTest), Times.Once);
        }

        //[Test]
        //public void ListarDetallesException()
        //{
        //    int idCabeceraTest = 1;

        //    var excepcionTest = new NullReferenceException("exploto molinos");

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.ListarDetalles(1))
        //        .Throws(excepcionTest);

        //    Exception excepcionResultante = null;

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(null));
        //        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);

        //        SustitucionMOAUtils.Logger.Fakes.ShimLog.ErrorStringStringStringStringException = (s1, s2, s3, s4, ex) => { excepcionResultante = ex; };

        //        result = target.ListarDetalles(idCabeceraTest);
        //    }

        //    Assert.IsNotNull(result.Data);

        //    string resultDataError = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();
        //    Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", resultDataError);
        //    Assert.AreEqual("exploto molinos", excepcionResultante.Message);

        //    this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(It.IsAny<int>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.ListarDetalles(1), Times.Once);
        //}

        //[Test]
        //public void AutorizarCabeceraOk()
        //{
        //    int idCabeceraTest = 3;
        //    string mailUsuarioTest = "mail";

        //    var ingresosBrutosCoeficienteUnificadoDetalleDtoList = new List<IngresosBrutosCoeficienteUnificadoDetalleDto>();

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
        //        .Returns("Se autorizo ok");

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        SustitucionMOASecurity.Fakes.ShimSessionPersister.getUsername = () => mailUsuarioTest;

        //        result = target.AutorizarCabecera(idCabeceraTest);
        //    }

        //    Assert.AreEqual("Se autorizo ok", result.Data);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        //}

        //[Test]
        //public void AutorizarCabeceraInfoCustomException()
        //{
        //    int idCabeceraTest = 1;
        //    string mailUsuarioTest = "mail";

        //    var excepcionTest = new InfoCustomException("Algo");

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
        //        .Throws(excepcionTest);

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        SustitucionMOASecurity.Fakes.ShimSessionPersister.getUsername = () => mailUsuarioTest;

        //        result = target.AutorizarCabecera(idCabeceraTest);
        //    }

        //    string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

        //    Assert.AreEqual("Algo", infoResultData);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        //}

        //[Test]
        //public void AutorizarCabeceraValidationCustomException()
        //{
        //    int idCabeceraTest = 1;
        //    string mailUsuarioTest = "mail";

        //    var excepcionTest = new ValidationCustomException("Error de validacion");

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
        //        .Throws(excepcionTest);

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        SustitucionMOASecurity.Fakes.ShimSessionPersister.getUsername = () => mailUsuarioTest;

        //        result = target.AutorizarCabecera(idCabeceraTest);
        //    }

        //    string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

        //    Assert.AreEqual("Error de validacion", errorResultData);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        //}

        //[Test]
        //public void AutorizarCabeceraException()
        //{
        //    int idCabeceraTest = 3;
        //    string mailUsuarioTest = "mail";

        //    var excepcionTest = new NullReferenceException("exploto molinos");

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest))
        //        .Throws(excepcionTest);

        //    Exception excepcionResultante = null;

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(null));
        //        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);
                
        //        SustitucionMOASecurity.Fakes.ShimSessionPersister.getUsername = () => mailUsuarioTest;

        //        SustitucionMOAUtils.Logger.Fakes.ShimLog.ErrorStringStringStringStringException = (s1, s2, s3, s4, ex) => { excepcionResultante = ex; };

        //        result = target.AutorizarCabecera(idCabeceraTest);
        //    }

        //    Assert.IsNotNull(result.Data);

        //    string resultDataError = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();
        //    Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", resultDataError);
        //    Assert.AreEqual("exploto molinos", excepcionResultante.Message);

        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.AutorizarCabecera(idCabeceraTest, mailUsuarioTest), Times.Once);
        //}

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleOk()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            DateTime hoy = new DateTime(2021, 8, 2);

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Returns(new EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto { Mensaje = "Se edito ok", FechaUltimaModificacion = hoy });
            
            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            Assert.IsInstanceOf<EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto>(result.Data);

            var resultData = (EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto)result.Data;

            Assert.AreEqual("Se edito ok", resultData.Mensaje);
            Assert.AreEqual(hoy, resultData.FechaUltimaModificacion);

            gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleInfoCustomException()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            var excepcionTest = new InfoCustomException("Algo");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            string infoResultData = result.Data.GetType().GetProperty("info").GetValue(result.Data).ToString();

            Assert.AreEqual("Algo", infoResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        [Test]
        public void EditarIngresosBrutosCoeficienteUnificadoDetalleValidationCustomException()
        {
            var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = 1,
            };

            var excepcionTest = new ValidationCustomException("Error de validacion");

            gestionImpuestosServiceMock
                .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
                .Throws(excepcionTest);

            string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);

            var result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);

            string errorResultData = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();

            Assert.AreEqual("Error de validacion", errorResultData);
            this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        }

        //[Test]
        //public void EditarIngresosBrutosCoeficienteUnificadoDetalleException()
        //{
        //    var ingresosBrutosCoeficienteUnificadoDetalleDtoTest = new IngresosBrutosCoeficienteUnificadoDetalleDto
        //    {
        //        Id = 1,
        //    };

        //    var excepcionTest = new NullReferenceException("exploto molinos");

        //    gestionImpuestosServiceMock
        //        .Setup(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)))
        //        .Throws(excepcionTest);

        //    string parametroJson = JsonConvert.SerializeObject(ingresosBrutosCoeficienteUnificadoDetalleDtoTest);
        //    Exception excepcionResultante = null;

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(null));
        //        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);

        //        SustitucionMOAUtils.Logger.Fakes.ShimLog.ErrorStringStringStringStringException = (s1, s2, s3, s4, ex) => { excepcionResultante = ex; };

        //        result = target.EditarIngresosBrutosCoeficienteUnificadoDetalle(parametroJson);
        //    }

        //    Assert.IsNotNull(result.Data);

        //    string resultDataError = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();
        //    Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", resultDataError);
        //    Assert.AreEqual("exploto molinos", excepcionResultante.Message);

        //    this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalleDto>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.EditarIngresosBrutosCoeficienteUnificadoDetalle(It.Is<IngresosBrutosCoeficienteUnificadoDetalleDto>(x => x.Id == 1)), Times.Once);
        //}

        [Test]
        public void DescargarFormularioCM05Ok()
        {
            int idCabeceraTest = 123;

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadoList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 1,    Archivo = new Archivo { Ruta = "C:/ArchivosProveedores/Consultas/5776/47/46323061565409 CM05.jpg" }, },
                new IngresosBrutosCoeficienteUnificado { Id = 12,   Archivo = new Archivo { Ruta = "C:/CarpetaLoca/Consultas/5776/47/46_3061565409 CM05.pdf" }, },
                new IngresosBrutosCoeficienteUnificado { Id = 123,  Archivo = new Archivo { Ruta = "C:/ArchivosProveedores/Consultas/5776/47/46_3061565409 CM05.pdf" }, },
                new IngresosBrutosCoeficienteUnificado { Id = 1234, Archivo = new Archivo { Ruta = "C:/ArchivosProveedores/Consultas/5776/47/46_3061565409 CM07.pdf" }, },
            };
            this.gestionImpuestosServiceMock
                .Setup(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()))
                .Returns<int>(idCabecera => ingresosBrutosCoeficienteUnificadoList.SingleOrDefault(x => x.Id == idCabecera).Archivo.Ruta);
            
            var result = target.DescargarFormularioCM05(idCabeceraTest);

            Assert.IsNotNull(result.Data);
            Assert.IsInstanceOf<FileContentResult>(result.Data);
            
            FileContentResult resultData = (FileContentResult)result.Data;
            Assert.AreEqual("46_3061565409 CM05.pdf", resultData.FileDownloadName);

            this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()), Times.Once);
            this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(123), Times.Once);
        }

        //[Test]
        //public void DescargarFormularioCM05Exception()
        //{
        //    int idCabeceraTest = 123;

        //    this.gestionImpuestosServiceMock
        //        .Setup(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()))
        //        .Throws(new Exception("exploto molinos"));

        //    Exception excepcionResultante = null;

        //    JsonResult result;
        //    using (ShimsContext.Create())
        //    {
        //        HttpContext.Current = new HttpContext(new HttpRequest("", "http://tempuri.org", ""), new HttpResponse(null));
        //        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("username"), new string[0]);

        //        SustitucionMOAUtils.Logger.Fakes.ShimLog.ErrorStringStringStringStringException = (s1, s2, s3, s4, ex) => { excepcionResultante = ex; };

        //        result = target.DescargarFormularioCM05(idCabeceraTest);
        //    }

        //    Assert.IsNotNull(result.Data);

        //    string resultDataError = result.Data.GetType().GetProperty("error").GetValue(result.Data).ToString();
        //    Assert.AreEqual("Ha ocurrido un error, por favor intente nuevamente", resultDataError);
        //    Assert.AreEqual("exploto molinos", excepcionResultante.Message);

        //    this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(It.IsAny<int>()), Times.Once);
        //    this.gestionImpuestosServiceMock.Verify(g => g.ObtenerRutaArchivoFormularioCM05(123), Times.Once);
        //}
    }
}
