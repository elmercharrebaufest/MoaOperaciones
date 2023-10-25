using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Helpers;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ConsultaServiceTest
    {
        private ConsultaService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IAzureService> azureServiceMock;
        private Mock<ITimeProvider> timeProviderMock;
        private Mock<IConsultaContext> consultaContext;
        private Mock<IGestionImpuestosService> gestionImpuestoServiceMock;
        private Mock<IConsultaCommon> consultaCommon;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            azureServiceMock = new Mock<IAzureService>();
            timeProviderMock = new Mock<ITimeProvider>();
            consultaContext = new Mock<IConsultaContext>();
            gestionImpuestoServiceMock = new Mock<IGestionImpuestosService>();
            consultaCommon = new Mock<IConsultaCommon>();

            target = new ConsultaService(repositorioMock.Object, azureServiceMock.Object, timeProviderMock.Object, consultaContext.Object, gestionImpuestoServiceMock.Object, consultaCommon.Object);
        }

        [Test]
        public void ProcesarCM05CuitProveedorDistintoOk()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "99 99999 9999";

            var archivos = new Mock<HttpFileCollectionBase>();

            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("text");

            var archivo2 = new Mock<HttpPostedFileBase>();
            archivo2.Setup(a => a.ContentType).Returns("application/pdf");

            var archivo3 = new Mock<HttpPostedFileBase>();
            archivo3.Setup(a => a.ContentType).Returns("application/pdf");
            archivo3.Setup(a => a.FileName).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            var archivo4 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(4);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312313-1", "Anticipo:", "55", "Sede:", "903",
                "Determinación del Coeficiente Unificad0",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,323", "0,2221",
                "904", "Cordoba", "23/12/2018", "3,3", "223,4", "0,55",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            archivos.Setup(x => x[2]).Returns(archivo3.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo3.Object)).ReturnsAsync("3");

            IList<string> resultOCR3 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901", "Secuencia:", "Original",
                "Determinación del Coeficiente Unificado", "Contribuyente:", "razon social",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,0000", "0,9999",
                "903", "Catamarca", "0,0000", "0,0000", "0,0000",
                "904", "Cordoba", "23/12/2018", "0,0000", "0,7548", "0,3477",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("3")).ReturnsAsync(resultOCR3);

            archivos.Setup(x => x[3]).Returns(archivo4.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo4.Object)).ReturnsAsync("4");

            IList<string> resultOCR4 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("4")).ReturnsAsync(resultOCR4);

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetallesInsertados = new List<IngresosBrutosCoeficienteUnificadoDetalle>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalle>()))
                .Callback<IngresosBrutosCoeficienteUnificadoDetalle>(x => ingresosBrutosCoeficienteUnificadoDetallesInsertados.Add(x));

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadosInsertados = new List<IngresosBrutosCoeficienteUnificado>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificado>()))
                .Callback<IngresosBrutosCoeficienteUnificado>(x => ingresosBrutosCoeficienteUnificadosInsertados.Add(x));

            List<Archivo> archivoList = new List<Archivo>
            {
                new Archivo { Id = 1, Ruta = "C:/ArchivosProveedores/Consultas/CM05.pdf" },
                new Archivo { Id = 2, Ruta = "C:/ArchivosProveedores/Consultas/324_CM05.pdf" },
                new Archivo { Id = 3, Ruta = "C:/ArchivosProveedores/Consultas/324_CM06.pdf" },
            };

            List<Comentario> comentarioList = new List<Comentario>
            {
                new Comentario { Id = 322, Archivos = archivoList, Consulta_Id = 1 },
                new Comentario { Id = 323, Archivos = archivoList, Consulta_Id = 2 },
                new Comentario { Id = 324, Archivos = archivoList, Consulta_Id = 3 },
            };
            repositorioMock
                .Setup(repo => repo.Obtener<Comentario>(It.IsAny<int>()))
                .Returns<int>((id) => comentarioList.SingleOrDefault(c => c.Id == id));

            int idComentarioTest = 324;

            string result = target.ProcesarCM05(archivos.Object, cuitProveedorTest, idComentarioTest);

            Assert.AreEqual(SuccessMsg.AltaFormularioCM05DistintoCUITOK, result);

            azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Never);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo3.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo4.Object), Times.Never);

            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Never);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("3"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("4"), Times.Never);

            repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadoDetallesInsertados.Count);

            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].NumeroJurisdiccion);
            Assert.AreEqual("Capital Federal", ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].Jurisdiccion);
            Assert.AreEqual(new DateTime(2021, 05, 15), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaInicio);
            Assert.AreEqual(new DateTime(2021, 06, 18), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaCese);
            Assert.AreEqual(0.2134, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteGastos);
            Assert.AreEqual(0.9999, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaUltimaModificacion);

            Assert.AreEqual(903, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].NumeroJurisdiccion);
            Assert.AreEqual("Catamarca", ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].Jurisdiccion);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteGastos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaUltimaModificacion);

            Assert.AreEqual(904, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].NumeroJurisdiccion);
            Assert.AreEqual("Cordoba", ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].Jurisdiccion);
            Assert.AreEqual(new DateTime(2018, 12, 23), ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteIngresos);
            Assert.AreEqual(0.7548, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteGastos);
            Assert.AreEqual(0.3477, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaUltimaModificacion);

            Assert.AreEqual(1, ingresosBrutosCoeficienteUnificadosInsertados.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetallesInsertados, ingresosBrutosCoeficienteUnificadosInsertados[0].Detalle);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, ingresosBrutosCoeficienteUnificadosInsertados[0].EstadoIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("20123123121", ingresosBrutosCoeficienteUnificadosInsertados[0].CUIT);
            Assert.AreEqual(1234, ingresosBrutosCoeficienteUnificadosInsertados[0].Anticipo);
            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadosInsertados[0].Sede);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaCarga);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaUltimaModificacion);
            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadosInsertados[0].Consulta_Id);
            Assert.AreEqual(2, ingresosBrutosCoeficienteUnificadosInsertados[0].Archivo_Id);
            Assert.IsFalse(ingresosBrutosCoeficienteUnificadosInsertados[0].MalCargada);
            Assert.AreEqual((int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original, ingresosBrutosCoeficienteUnificadosInsertados[0].SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("razon social", ingresosBrutosCoeficienteUnificadosInsertados[0].RazonSocial);
        }

        [Test]
        public void ProcesarCM05MismoCuitProveedorOk()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "20123123121";

            var archivos = new Mock<HttpFileCollectionBase>();

            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("text");

            var archivo2 = new Mock<HttpPostedFileBase>();
            archivo2.Setup(a => a.ContentType).Returns("application/pdf");

            var archivo3 = new Mock<HttpPostedFileBase>();
            archivo3.Setup(a => a.ContentType).Returns("application/pdf");
            archivo3.Setup(a => a.FileName).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            var archivo4 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(4);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312313-1", "Anticipo:", "55", "Sede:", "903",
                "Determinación del Coeficiente Unificad0",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,323", "0,2221",
                "904", "Cordoba", "23/12/2018", "3,3", "223,4", "0,55",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            archivos.Setup(x => x[2]).Returns(archivo3.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo3.Object)).ReturnsAsync("3");

            IList<string> resultOCR3 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901", "Secuencia:", "Rectificativa 324",
                "Determinación del Coeficiente Unificado", "Contribuyente:", "razon social",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,0000", "0,9999",
                "903", "Catamarca", "0,0000", "0,0000", "0,0000",
                "904", "Cordoba", "23/12/2018", "0,0000", "0,7548", "0,3477",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("3")).ReturnsAsync(resultOCR3);

            archivos.Setup(x => x[3]).Returns(archivo4.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo4.Object)).ReturnsAsync("4");

            IList<string> resultOCR4 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("4")).ReturnsAsync(resultOCR4);

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetallesInsertados = new List<IngresosBrutosCoeficienteUnificadoDetalle>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalle>()))
                .Callback<IngresosBrutosCoeficienteUnificadoDetalle>(x => ingresosBrutosCoeficienteUnificadoDetallesInsertados.Add(x));

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadosInsertados = new List<IngresosBrutosCoeficienteUnificado>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificado>()))
                .Callback<IngresosBrutosCoeficienteUnificado>(x => ingresosBrutosCoeficienteUnificadosInsertados.Add(x));

            List<Archivo> archivoList = new List<Archivo>
            {
                new Archivo { Id = 1, Ruta = "C:/ArchivosProveedores/Consultas/CM05.pdf" },
                new Archivo { Id = 2, Ruta = "C:/ArchivosProveedores/Consultas/324_CM05.pdf" },
                new Archivo { Id = 3, Ruta = "C:/ArchivosProveedores/Consultas/324_CM06.pdf" },
            };

            List<Comentario> comentarioList = new List<Comentario>
            {
                new Comentario { Id = 322, Archivos = archivoList, Consulta_Id = 1 },
                new Comentario { Id = 323, Archivos = archivoList, Consulta_Id = 2 },
                new Comentario { Id = 324, Archivos = archivoList, Consulta_Id = 3 },
            };
            repositorioMock
                .Setup(repo => repo.Obtener<Comentario>(It.IsAny<int>()))
                .Returns<int>((id) => comentarioList.SingleOrDefault(c => c.Id == id));

            int idComentarioTest = 324;

            string result = target.ProcesarCM05(archivos.Object, cuitProveedorTest, idComentarioTest);

            Assert.AreEqual(string.Empty, result);

            azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Never);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo3.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo4.Object), Times.Never);

            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Never);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("3"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("4"), Times.Never);

            repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadoDetallesInsertados.Count);

            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].NumeroJurisdiccion);
            Assert.AreEqual("Capital Federal", ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].Jurisdiccion);
            Assert.AreEqual(new DateTime(2021, 05, 15), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaInicio);
            Assert.AreEqual(new DateTime(2021, 06, 18), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaCese);
            Assert.AreEqual(0.2134, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteGastos);
            Assert.AreEqual(0.9999, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaUltimaModificacion);

            Assert.AreEqual(903, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].NumeroJurisdiccion);
            Assert.AreEqual("Catamarca", ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].Jurisdiccion);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteGastos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaUltimaModificacion);

            Assert.AreEqual(904, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].NumeroJurisdiccion);
            Assert.AreEqual("Cordoba", ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].Jurisdiccion);
            Assert.AreEqual(new DateTime(2018, 12, 23), ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteIngresos);
            Assert.AreEqual(0.7548, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteGastos);
            Assert.AreEqual(0.3477, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaUltimaModificacion);

            Assert.AreEqual(1, ingresosBrutosCoeficienteUnificadosInsertados.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetallesInsertados, ingresosBrutosCoeficienteUnificadosInsertados[0].Detalle);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, ingresosBrutosCoeficienteUnificadosInsertados[0].EstadoIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("20123123121", ingresosBrutosCoeficienteUnificadosInsertados[0].CUIT);
            Assert.AreEqual(1234, ingresosBrutosCoeficienteUnificadosInsertados[0].Anticipo);
            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadosInsertados[0].Sede);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaCarga);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaUltimaModificacion);
            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadosInsertados[0].Consulta_Id);
            Assert.AreEqual(2, ingresosBrutosCoeficienteUnificadosInsertados[0].Archivo_Id);
            Assert.IsFalse(ingresosBrutosCoeficienteUnificadosInsertados[0].MalCargada);
            Assert.AreEqual((int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Rectificativa, ingresosBrutosCoeficienteUnificadosInsertados[0].SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("razon social", ingresosBrutosCoeficienteUnificadosInsertados[0].RazonSocial);
        }

        [Test]
        public void ProcesarCM05MalCargadoOk()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "20123123121";

            var archivos = new Mock<HttpFileCollectionBase>();

            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("text");

            var archivo2 = new Mock<HttpPostedFileBase>();
            archivo2.Setup(a => a.ContentType).Returns("application/pdf");

            var archivo3 = new Mock<HttpPostedFileBase>();
            archivo3.Setup(a => a.ContentType).Returns("application/pdf");
            archivo3.Setup(a => a.FileName).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            var archivo4 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(4);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312313-1", "Anticipo:", "55", "Sede:", "903", "Secuencia:",
                "Determinación del Coeficiente Unificad0",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,323", "0,2221",
                "904", "Cordoba", "23/12/2018", "3,3", "223,4", "0,55",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            archivos.Setup(x => x[2]).Returns(archivo3.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo3.Object)).ReturnsAsync("3");

            IList<string> resultOCR3 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901",
                "Determinación del Coeficiente Unificado", "Contribuyente:", "razon social",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,0000", "aas",
                "903", "Catamarca", "0,0000", "0,0000", "0,0000",
                "904", "Cordoba", "23/12/2018", "0,0000", "0,7548", "0,3477",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("3")).ReturnsAsync(resultOCR3);

            archivos.Setup(x => x[3]).Returns(archivo4.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo4.Object)).ReturnsAsync("4");

            IList<string> resultOCR4 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("4")).ReturnsAsync(resultOCR4);

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetallesInsertados = new List<IngresosBrutosCoeficienteUnificadoDetalle>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalle>()))
                .Callback<IngresosBrutosCoeficienteUnificadoDetalle>(x => ingresosBrutosCoeficienteUnificadoDetallesInsertados.Add(x));

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadosInsertados = new List<IngresosBrutosCoeficienteUnificado>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificado>()))
                .Callback<IngresosBrutosCoeficienteUnificado>(x => ingresosBrutosCoeficienteUnificadosInsertados.Add(x));

            List<Archivo> archivoList = new List<Archivo>
            {
                new Archivo { Id = 1, Ruta = "C:/ArchivosProveedores/Consultas/CM05.pdf" },
                new Archivo { Id = 2, Ruta = "C:/ArchivosProveedores/Consultas/324_CM05.pdf" },
                new Archivo { Id = 3, Ruta = "C:/ArchivosProveedores/Consultas/324_CM06.pdf" },
            };

            List<Comentario> comentarioList = new List<Comentario>
            {
                new Comentario { Id = 322, Archivos = archivoList, Consulta_Id = 1 },
                new Comentario { Id = 323, Archivos = archivoList, Consulta_Id = 2 },
                new Comentario { Id = 324, Archivos = archivoList, Consulta_Id = 3 },
            };
            repositorioMock
                .Setup(repo => repo.Obtener<Comentario>(It.IsAny<int>()))
                .Returns<int>((id) => comentarioList.SingleOrDefault(c => c.Id == id));

            int idComentarioTest = 324;

            string result = target.ProcesarCM05(archivos.Object, cuitProveedorTest, idComentarioTest);

            Assert.AreEqual(string.Empty, result);

            azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Never);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo3.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo4.Object), Times.Never);

            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Never);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("3"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("4"), Times.Never);

            repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Once);

            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadoDetallesInsertados.Count);

            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].NumeroJurisdiccion);
            Assert.AreEqual("Capital Federal", ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].Jurisdiccion);
            Assert.AreEqual(new DateTime(2021, 05, 15), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaInicio);
            Assert.AreEqual(new DateTime(2021, 06, 18), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaCese);
            Assert.AreEqual(0.2134, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteGastos);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaUltimaModificacion);

            Assert.AreEqual(903, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].NumeroJurisdiccion);
            Assert.AreEqual("Catamarca", ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].Jurisdiccion);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteGastos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaUltimaModificacion);

            Assert.AreEqual(904, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].NumeroJurisdiccion);
            Assert.AreEqual("Cordoba", ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].Jurisdiccion);
            Assert.AreEqual(new DateTime(2018, 12, 23), ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteIngresos);
            Assert.AreEqual(0.7548, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteGastos);
            Assert.AreEqual(0.3477, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaUltimaModificacion);

            Assert.AreEqual(1, ingresosBrutosCoeficienteUnificadosInsertados.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetallesInsertados, ingresosBrutosCoeficienteUnificadosInsertados[0].Detalle);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, ingresosBrutosCoeficienteUnificadosInsertados[0].EstadoIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("20123123121", ingresosBrutosCoeficienteUnificadosInsertados[0].CUIT);
            Assert.AreEqual(1234, ingresosBrutosCoeficienteUnificadosInsertados[0].Anticipo);
            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadosInsertados[0].Sede);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaCarga);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaUltimaModificacion);
            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadosInsertados[0].Consulta_Id);
            Assert.AreEqual(2, ingresosBrutosCoeficienteUnificadosInsertados[0].Archivo_Id);
            Assert.IsTrue(ingresosBrutosCoeficienteUnificadosInsertados[0].MalCargada);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadosInsertados[0].SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("razon social", ingresosBrutosCoeficienteUnificadosInsertados[0].RazonSocial);
        }

        [Test]
        public void ProcesarCM05FaltanCoeficientes()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "1232131231323";

            var archivos = new Mock<HttpFileCollectionBase>();
            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("application/pdf");
            var archivo2 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(2);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>{
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            try
            {
                target.ProcesarCM05(archivos.Object, cuitProveedorTest, 32);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (ValidationCustomException vex)
            {
                Assert.AreEqual("No se pudieron obtener los coeficientes. Por favor, asegúrese de adjuntar el documento correspondiente.", vex.Message);

                azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(1));
                azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Once);
                azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Never);

                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(1));
                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Once);
                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Never);

                repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una ValidationCustomException");
            }
        }

        [Test]
        public void ProcesarCM05Excepcion()
        {
            int idComentarioTest = 324;
            string cuitProveedorTest = "20123123121";

            var archivos = new Mock<HttpFileCollectionBase>();

            Exception exceptionTest = new Exception("excepcion loca");

            archivos.Setup(x => x.Count).Throws(exceptionTest);

            try
            {
                target.ProcesarCM05(archivos.Object, cuitProveedorTest, idComentarioTest);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (ValidationCustomException vex)
            {
                Assert.AreEqual(vex.Message, ErrorMsg.ErrorCargaCM05);
                Assert.AreEqual(vex.InnerException, exceptionTest);
                Assert.IsTrue(vex.LoguearExcepcion);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una ValidationCustomException");
            }
        }

        [Test]
        public void ProcesarCM05CargaInternaOk()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "99 99999 9999";

            var archivos = new Mock<HttpFileCollectionBase>();

            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("text");

            var archivo2 = new Mock<HttpPostedFileBase>();
            archivo2.Setup(a => a.ContentType).Returns("application/pdf");

            var archivo3 = new Mock<HttpPostedFileBase>();
            archivo3.Setup(a => a.ContentType).Returns("application/pdf");
            archivo3.Setup(a => a.FileName).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            var archivo4 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(4);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312313-1", "Anticipo:", "55", "Sede:", "903",
                "Determinación del Coeficiente Unificad0",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,323", "0,2221",
                "904", "Cordoba", "23/12/2018", "3,3", "223,4", "0,55",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            archivos.Setup(x => x[2]).Returns(archivo3.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo3.Object)).ReturnsAsync("3");

            IList<string> resultOCR3 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901", "Secuencia:", "Original",
                "Determinación del Coeficiente Unificado", "Contribuyente:", "razon social",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,0000", "0,9999",
                "903", "Catamarca", "0,0000", "0,0000", "0,0000",
                "904", "Cordoba", "23/12/2018", "0,0000", "0,7548", "0,3477",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("3")).ReturnsAsync(resultOCR3);

            archivos.Setup(x => x[3]).Returns(archivo4.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo4.Object)).ReturnsAsync("4");

            IList<string> resultOCR4 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("4")).ReturnsAsync(resultOCR4);

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetallesInsertados = new List<IngresosBrutosCoeficienteUnificadoDetalle>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalle>()))
                .Callback<IngresosBrutosCoeficienteUnificadoDetalle>(x => ingresosBrutosCoeficienteUnificadoDetallesInsertados.Add(x));

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadosInsertados = new List<IngresosBrutosCoeficienteUnificado>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificado>()))
                .Callback<IngresosBrutosCoeficienteUnificado>(x => ingresosBrutosCoeficienteUnificadosInsertados.Add(x));


            Mock<ConsultaService> targetMock = new Mock<ConsultaService>(this.repositorioMock.Object, this.azureServiceMock.Object, this.timeProviderMock.Object) { CallBase = true, };
            targetMock.Setup(x => x.ArmarRutaCarpetaCM05(It.IsAny<string>())).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");



            List<Archivo> archivoList = new List<Archivo>
            {
                new Archivo { Id = 1, Ruta = "C:/ArchivosProveedores/Consultas/CM05.pdf" },
                new Archivo { Id = 2, Ruta = "C:/ArchivosProveedores/Consultas/324_CM05.pdf" },
                new Archivo { Id = 3, Ruta = "C:/ArchivosProveedores/Consultas/324_CM06.pdf" },
            };

            string result = targetMock.Object.ProcesarCM05(archivos.Object, cuitProveedorTest, null, true);

            Assert.AreEqual(SuccessMsg.AltaFormularioCM05CargaInternaOK, result);

            azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Never);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo3.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo4.Object), Times.Never);

            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Never);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("3"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("4"), Times.Never);

            repositorioMock.Verify(repo => repo.GuardarCambios(), Times.AtLeast(2));

            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadoDetallesInsertados.Count);

            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].NumeroJurisdiccion);
            Assert.AreEqual("Capital Federal", ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].Jurisdiccion);
            Assert.AreEqual(new DateTime(2021, 05, 15), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaInicio);
            Assert.AreEqual(new DateTime(2021, 06, 18), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaCese);
            Assert.AreEqual(0.2134, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteGastos);
            Assert.AreEqual(0.9999, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaUltimaModificacion);

            Assert.AreEqual(903, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].NumeroJurisdiccion);
            Assert.AreEqual("Catamarca", ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].Jurisdiccion);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteGastos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaUltimaModificacion);

            Assert.AreEqual(904, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].NumeroJurisdiccion);
            Assert.AreEqual("Cordoba", ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].Jurisdiccion);
            Assert.AreEqual(new DateTime(2018, 12, 23), ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteIngresos);
            Assert.AreEqual(0.7548, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteGastos);
            Assert.AreEqual(0.3477, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaUltimaModificacion);

            Assert.AreEqual(1, ingresosBrutosCoeficienteUnificadosInsertados.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetallesInsertados, ingresosBrutosCoeficienteUnificadosInsertados[0].Detalle);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, ingresosBrutosCoeficienteUnificadosInsertados[0].EstadoIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("20123123121", ingresosBrutosCoeficienteUnificadosInsertados[0].CUIT);
            Assert.AreEqual(1234, ingresosBrutosCoeficienteUnificadosInsertados[0].Anticipo);
            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadosInsertados[0].Sede);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaCarga);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaUltimaModificacion);
            Assert.AreEqual(null, ingresosBrutosCoeficienteUnificadosInsertados[0].Consulta_Id);
            Assert.AreEqual(0, ingresosBrutosCoeficienteUnificadosInsertados[0].Archivo_Id);
            Assert.IsFalse(ingresosBrutosCoeficienteUnificadosInsertados[0].MalCargada);
            Assert.AreEqual((int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original, ingresosBrutosCoeficienteUnificadosInsertados[0].SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("razon social", ingresosBrutosCoeficienteUnificadosInsertados[0].RazonSocial);
        }

        [Test]
        public void ProcesarCM05CargaInternaMalCargadoOk()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "99 99999 9999";

            var archivos = new Mock<HttpFileCollectionBase>();

            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("text");

            var archivo2 = new Mock<HttpPostedFileBase>();
            archivo2.Setup(a => a.ContentType).Returns("application/pdf");

            var archivo3 = new Mock<HttpPostedFileBase>();
            archivo3.Setup(a => a.ContentType).Returns("application/pdf");
            archivo3.Setup(a => a.FileName).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            var archivo4 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(4);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312313-1", "Anticipo:", "55", "Sede:", "903",
                "Determinación del Coeficiente Unificad0",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,323", "0,2221",
                "904", "Cordoba", "23/12/2018", "3,3", "223,4", "0,55",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            archivos.Setup(x => x[2]).Returns(archivo3.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo3.Object)).ReturnsAsync("3");

            IList<string> resultOCR3 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901", "Secuencia:", "Original",
                "Determinación del Coeficiente Unificado", "Contribuyente:", "razon social",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,0000", "aas",
                "903", "Catamarca", "0,0000", "0,0000", "0,0000",
                "904", "Cordoba", "23/12/2018", "0,0000", "0,7548", "0,3477",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("3")).ReturnsAsync(resultOCR3);

            archivos.Setup(x => x[3]).Returns(archivo4.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo4.Object)).ReturnsAsync("4");

            IList<string> resultOCR4 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("4")).ReturnsAsync(resultOCR4);

            List<IngresosBrutosCoeficienteUnificadoDetalle> ingresosBrutosCoeficienteUnificadoDetallesInsertados = new List<IngresosBrutosCoeficienteUnificadoDetalle>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificadoDetalle>()))
                .Callback<IngresosBrutosCoeficienteUnificadoDetalle>(x => ingresosBrutosCoeficienteUnificadoDetallesInsertados.Add(x));

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadosInsertados = new List<IngresosBrutosCoeficienteUnificado>();
            repositorioMock
                .Setup(repo => repo.Agregar(It.IsAny<IngresosBrutosCoeficienteUnificado>()))
                .Callback<IngresosBrutosCoeficienteUnificado>(x => ingresosBrutosCoeficienteUnificadosInsertados.Add(x));

            Mock<ConsultaService> targetMock = new Mock<ConsultaService>(this.repositorioMock.Object, this.azureServiceMock.Object, this.timeProviderMock.Object) { CallBase = true, };
            targetMock.Setup(x => x.ArmarRutaCarpetaCM05(It.IsAny<string>())).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            List<Archivo> archivoList = new List<Archivo>
            {
                new Archivo { Id = 1, Ruta = "C:/ArchivosProveedores/Consultas/CM05.pdf" },
                new Archivo { Id = 2, Ruta = "C:/ArchivosProveedores/Consultas/324_CM05.pdf" },
                new Archivo { Id = 3, Ruta = "C:/ArchivosProveedores/Consultas/324_CM06.pdf" },
            };

            string result = targetMock.Object.ProcesarCM05(archivos.Object, cuitProveedorTest, null, true);

            Assert.AreEqual(SuccessMsg.AltaFormularioCM05CargaInternaMalCargadoOK, result);

            azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Never);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo3.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo4.Object), Times.Never);

            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Never);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("3"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("4"), Times.Never);

            repositorioMock.Verify(repo => repo.GuardarCambios(), Times.AtLeast(2));

            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadoDetallesInsertados.Count);

            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].NumeroJurisdiccion);
            Assert.AreEqual("Capital Federal", ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].Jurisdiccion);
            Assert.AreEqual(new DateTime(2021, 05, 15), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaInicio);
            Assert.AreEqual(new DateTime(2021, 06, 18), ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaCese);
            Assert.AreEqual(0.2134, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteGastos);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[0].FechaUltimaModificacion);

            Assert.AreEqual(903, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].NumeroJurisdiccion);
            Assert.AreEqual("Catamarca", ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].Jurisdiccion);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteIngresos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteGastos);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[1].FechaUltimaModificacion);

            Assert.AreEqual(904, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].NumeroJurisdiccion);
            Assert.AreEqual("Cordoba", ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].Jurisdiccion);
            Assert.AreEqual(new DateTime(2018, 12, 23), ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaInicio);
            Assert.IsNull(ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaCese);
            Assert.AreEqual(0.0, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteIngresos);
            Assert.AreEqual(0.7548, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteGastos);
            Assert.AreEqual(0.3477, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].CoeficienteUnificado);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadoDetallesInsertados[2].FechaUltimaModificacion);

            Assert.AreEqual(1, ingresosBrutosCoeficienteUnificadosInsertados.Count);
            Assert.AreEqual(ingresosBrutosCoeficienteUnificadoDetallesInsertados, ingresosBrutosCoeficienteUnificadosInsertados[0].Detalle);
            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente, ingresosBrutosCoeficienteUnificadosInsertados[0].EstadoIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("20123123121", ingresosBrutosCoeficienteUnificadosInsertados[0].CUIT);
            Assert.AreEqual(1234, ingresosBrutosCoeficienteUnificadosInsertados[0].Anticipo);
            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadosInsertados[0].Sede);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaCarga);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaUltimaModificacion);
            Assert.AreEqual(null, ingresosBrutosCoeficienteUnificadosInsertados[0].Consulta_Id);
            Assert.AreEqual(0, ingresosBrutosCoeficienteUnificadosInsertados[0].Archivo_Id);
            Assert.IsTrue(ingresosBrutosCoeficienteUnificadosInsertados[0].MalCargada);
            Assert.AreEqual((int)EnumSecuenciaIngresosBrutosCoeficienteUnificado.Original, ingresosBrutosCoeficienteUnificadosInsertados[0].SecuenciaIngresosBrutosCoeficienteUnificado_Id);
            Assert.AreEqual("razon social", ingresosBrutosCoeficienteUnificadosInsertados[0].RazonSocial);
        }

        [Test]
        public void ProcesarCM05CargaInternaFaltanCoeficientes()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            string cuitProveedorTest = "1232131231323";

            var archivos = new Mock<HttpFileCollectionBase>();
            var archivo1 = new Mock<HttpPostedFileBase>();
            archivo1.Setup(a => a.ContentType).Returns("application/pdf");
            var archivo2 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(2);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>{
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);


            Mock<ConsultaService> targetMock = new Mock<ConsultaService>(this.repositorioMock.Object, this.azureServiceMock.Object, this.timeProviderMock.Object) { CallBase = true, };
            targetMock.Setup(x => x.ArmarRutaCarpetaCM05(It.IsAny<string>())).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            try
            {
                targetMock.Object.ProcesarCM05(archivos.Object, cuitProveedorTest, null, true);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (ValidationCustomException vex)
            {
                Assert.AreEqual("No se pudieron obtener los coeficientes. Por favor, asegúrese de adjuntar el documento correspondiente.", vex.Message);

                azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(1));
                azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Once);
                azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Never);

                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(1));
                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Once);
                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Never);

                repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una ValidationCustomException");
            }
        }

        [Test]
        public void ProcesarCM05CargaInternaExcepcion()
        {
            int idComentarioTest = 324;
            string cuitProveedorTest = "20123123121";

            var archivos = new Mock<HttpFileCollectionBase>();

            Exception exceptionTest = new Exception("excepcion loca");

            archivos.Setup(x => x.Count).Throws(exceptionTest);
            Mock<ConsultaService> targetMock = new Mock<ConsultaService>(this.repositorioMock.Object, this.azureServiceMock.Object, this.timeProviderMock.Object) { CallBase = true, };
            targetMock.Setup(x => x.ArmarRutaCarpetaCM05(It.IsAny<string>())).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");

            try
            {
                targetMock.Object.ProcesarCM05(archivos.Object, cuitProveedorTest, idComentarioTest, true);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (ValidationCustomException vex)
            {
                Assert.AreEqual(vex.Message, ErrorMsg.ErrorCargaCM05);
                Assert.AreEqual(vex.InnerException, exceptionTest);
                Assert.IsTrue(vex.LoguearExcepcion);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una ValidationCustomException");
            }
        }

        [Test]
        public void AnularConsultaOk()
        {
            int consultaIdTest = 332;
            int usuarioIdTest = 98;
            string motivoRechazoTest = "rechazada pa";

            DateTime hoy = new DateTime(2021, 9, 14);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            Mock<ConsultaService> targetMock = new Mock<ConsultaService>(this.repositorioMock.Object, this.azureServiceMock.Object, this.timeProviderMock.Object) { CallBase = true, };
            targetMock.Setup(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<ComentarioDto>(), It.IsAny<HttpFileCollectionBase>())).Returns(new ComentarioDto());
            targetMock.Setup(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>())).Callback(() => { });

            Categoria actualizacion = new Categoria { Code = Categorias.Actualizacion };
            Categoria boletos = new Categoria { Code = Categorias.Boletos };

            SubCategoria cm05 = new SubCategoria { Code = SubCategorias.CM05 };
            SubCategoria contratos = new SubCategoria { Code = SubCategorias.Contratos };

            List<Consulta> consultaList = new List<Consulta>
            {
                new Consulta { Id = 248, Categoria = actualizacion, SubCategoria = contratos },
                new Consulta { Id = 332, Categoria = boletos, SubCategoria = cm05 },
            };

            this.repositorioMock
                .Setup(x => x.Obtener<Consulta>(It.IsAny<int>()))
                .Returns<int>(id => consultaList.SingleOrDefault(x => x.Id == id));

            var result = targetMock.Object.AnularConsulta(consultaIdTest, usuarioIdTest, motivoRechazoTest);

            targetMock.Verify(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            targetMock.Verify(x => x.ActualizarEstadoConsulta(consultaIdTest, (int)EstadosConsulta.Finalizado), Times.Once);

            targetMock.Verify(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<ComentarioDto>(), It.IsAny<HttpFileCollectionBase>()), Times.Once);
            targetMock.Verify(x => x.AgregarComentario(
                consultaIdTest,
                It.Is<ComentarioDto>(comentario => comentario.Detalle == motivoRechazoTest + ", consulta cerrada." && comentario.Fecha == hoy && comentario.UsuarioId == usuarioIdTest),
                It.IsAny<HttpFileCollectionBase>()), Times.Once);

            this.repositorioMock.Verify(x => x.Obtener<Consulta>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(x => x.Obtener<Consulta>(consultaIdTest), Times.Once);

            this.repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>()), Times.Never);
        }

        [Test]
        public void AnularConsultaCM05Ok()
        {
            int consultaIdTest = 332;
            int usuarioIdTest = 98;
            string motivoRechazoTest = "rechazada pa";

            DateTime hoy = new DateTime(2021, 9, 14);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            Mock<ConsultaService> targetMock = new Mock<ConsultaService>(this.repositorioMock.Object, this.azureServiceMock.Object, this.timeProviderMock.Object) { CallBase = true, };
            targetMock.Setup(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<ComentarioDto>(), It.IsAny<HttpFileCollectionBase>())).Returns(new ComentarioDto());
            targetMock.Setup(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>())).Callback(() => { });

            Categoria actualizacion = new Categoria { Code = Categorias.Actualizacion };
            Categoria boletos = new Categoria { Code = Categorias.Boletos };

            SubCategoria cm05 = new SubCategoria { Code = SubCategorias.CM05 };
            SubCategoria contratos = new SubCategoria { Code = SubCategorias.Contratos };

            List<Consulta> consultaList = new List<Consulta>
            {
                new Consulta { Id = 248, Categoria = boletos, SubCategoria = contratos },
                new Consulta { Id = 332, Categoria = actualizacion, SubCategoria = cm05 },
            };

            this.repositorioMock
                .Setup(x => x.Obtener<Consulta>(It.IsAny<int>()))
                .Returns<int>(id => consultaList.SingleOrDefault(x => x.Id == id));

            List<IngresosBrutosCoeficienteUnificado> ingresosBrutosCoeficienteUnificadosList = new List<IngresosBrutosCoeficienteUnificado>
            {
                new IngresosBrutosCoeficienteUnificado { Id = 1, Consulta_Id = 332, EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente },
                new IngresosBrutosCoeficienteUnificado { Id = 2, Consulta_Id = 248, EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Pendiente },
            };

            this.repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>()))
                .Returns<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>(q => ingresosBrutosCoeficienteUnificadosList.SingleOrDefault(q.Compile()));

            var result = targetMock.Object.AnularConsulta(consultaIdTest, usuarioIdTest, motivoRechazoTest);

            targetMock.Verify(x => x.ActualizarEstadoConsulta(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            targetMock.Verify(x => x.ActualizarEstadoConsulta(consultaIdTest, (int)EstadosConsulta.Finalizado), Times.Once);

            targetMock.Verify(x => x.AgregarComentario(It.IsAny<int>(), It.IsAny<ComentarioDto>(), It.IsAny<HttpFileCollectionBase>()), Times.Once);
            targetMock.Verify(x => x.AgregarComentario(
                consultaIdTest,
                It.Is<ComentarioDto>(comentario => comentario.Detalle == motivoRechazoTest + ", consulta cerrada." && comentario.Fecha == hoy && comentario.UsuarioId == usuarioIdTest),
                It.IsAny<HttpFileCollectionBase>()), Times.Once);

            this.repositorioMock.Verify(x => x.Obtener<Consulta>(It.IsAny<int>()), Times.Once);
            this.repositorioMock.Verify(x => x.Obtener<Consulta>(consultaIdTest), Times.Once);

            this.repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<IngresosBrutosCoeficienteUnificado, bool>>>()), Times.Once);

            Assert.AreEqual((int)EnumEstadoIngresosBrutosCoeficienteUnificado.RechazadoPorUsuario, ingresosBrutosCoeficienteUnificadosList[0].EstadoIngresosBrutosCoeficienteUnificado_Id);

            this.repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test()]
        public void ObtenerMaterialesContactoTest()
        {
            var materiales = new List<Material>() 
            { 
                new Material { Id=1, Nombre="Maiz", TablaSeccionMaterial=TablaSeccionMaterial.Contacto },
                new Material { Id=2, Nombre="Choclo", TablaSeccionMaterial=TablaSeccionMaterial.Contacto },
                new Material { Id=3, Nombre="Choclo algo", TablaSeccionMaterial=TablaSeccionMaterial.OrdenDeCarga },
                new Material { Id=4, Nombre="Maiz algo", TablaSeccionMaterial=TablaSeccionMaterial.OrdenDeCarga },
            };

            repositorioMock
            .Setup(x => x.Listar(It.IsAny<Expression<Func<Material, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>(),
                            It.IsAny<IEnumerable<Expression<Func<Material, object>>>>()))
            .Returns(materiales);

            var expected = new List<MaterialDto>()
            {
                new MaterialDto { MaterialId=2, Descripcion="Choclo" },
                new MaterialDto { MaterialId=1, Descripcion="Maiz" }
            };

            var result = target.ObtenerMaterial(TablaSeccionMaterial.Contacto);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(expected[0].Descripcion, result[0].Descripcion);
        }

        [Test()]
        public void ObtenerMaterialesSapTest()
        {
            var materiales = new List<Material>()
            {
                new Material { Id=1, Nombre="Maiz", TablaSeccionMaterial=TablaSeccionMaterial.Contacto },
                new Material { Id=2, Nombre="Choclo", TablaSeccionMaterial=TablaSeccionMaterial.Contacto },
                new Material { Id=3, Nombre="Choclo algo", TablaSeccionMaterial=TablaSeccionMaterial.OrdenDeCarga },
                new Material { Id=4, Nombre="Maiz algo", TablaSeccionMaterial=TablaSeccionMaterial.OrdenDeCarga },
            };

            repositorioMock
            .Setup(x => x.Listar(It.IsAny<Expression<Func<Material, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>(),
                            It.IsAny<IEnumerable<Expression<Func<Material, object>>>>()))
            .Returns(materiales);

            var expected = new List<MaterialDto>()
            {
                new MaterialDto { MaterialId=2, Descripcion="Choclo algo" },
                new MaterialDto { MaterialId=1, Descripcion="Maiz algo" }
            };

            var result = target.ObtenerMaterial(TablaSeccionMaterial.OrdenDeCarga);

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(expected[0].Descripcion, result[0].Descripcion);
        }
    }
}