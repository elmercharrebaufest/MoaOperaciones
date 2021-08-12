using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
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

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            azureServiceMock = new Mock<IAzureService>();
            timeProviderMock = new Mock<ITimeProvider>();

            target = new ConsultaService(repositorioMock.Object, azureServiceMock.Object, timeProviderMock.Object);
        }

        [Test]
        public void ProcesarCM05Ok()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            var archivos = new Mock<HttpFileCollectionBase>();
            var archivo1 = new Mock<HttpPostedFileBase>();
            var archivo2 = new Mock<HttpPostedFileBase>();
            archivo2.Setup(a => a.FileName).Returns("C:/ArchivosProveedores/Consultas/CM05.pdf");
            var archivo3 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(3);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901",
                "Determinación del Coeficiente Unificado",
                "Coeficiente Unificado",
                "901", "Capital Federal", "15/05/2021", "18/06/2021", "0,2134", "0,0000", "0,9999",
                "903", "Catamarca", "0,0000", "0,0000", "0,0000",
                "904", "Cordoba", "23/12/2018", "0,0000", "0,7548", "0,3477",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            archivos.Setup(x => x[2]).Returns(archivo3.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo3.Object)).ReturnsAsync("3");

            IList<string> resultOCR3 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("3")).ReturnsAsync(resultOCR3);

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

            target.ProcesarCM05(archivos.Object, idComentarioTest);

            azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);
            azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo3.Object), Times.Never);

            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);
            azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("3"), Times.Never);

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
            Assert.AreEqual("20-12312312-1", ingresosBrutosCoeficienteUnificadosInsertados[0].CUIT);
            Assert.AreEqual(1234, ingresosBrutosCoeficienteUnificadosInsertados[0].Anticipo);
            Assert.AreEqual(901, ingresosBrutosCoeficienteUnificadosInsertados[0].Sede);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaCarga);
            Assert.AreEqual(hoy, ingresosBrutosCoeficienteUnificadosInsertados[0].FechaUltimaModificacion);
            Assert.AreEqual(3, ingresosBrutosCoeficienteUnificadosInsertados[0].Consulta_Id);
            Assert.AreEqual(2, ingresosBrutosCoeficienteUnificadosInsertados[0].Archivo_Id);
        }

        [Test]
        public void ProcesarCM05FaltanCoeficientes()
        {
            DateTime hoy = new DateTime(2021, 7, 19);
            timeProviderMock.Setup(x => x.Now()).Returns(hoy);

            var archivos = new Mock<HttpFileCollectionBase>();
            var archivo1 = new Mock<HttpPostedFileBase>();
            var archivo2 = new Mock<HttpPostedFileBase>();

            archivos.Setup(x => x.Count).Returns(2);

            archivos.Setup(x => x[0]).Returns(archivo1.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo1.Object)).ReturnsAsync("1");

            IList<string> resultOCR1 = new List<string>();
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("1")).ReturnsAsync(resultOCR1);

            archivos.Setup(x => x[1]).Returns(archivo2.Object);
            azureServiceMock.Setup(az => az.AnalizarImagenAsync(archivo2.Object)).ReturnsAsync("2");

            IList<string> resultOCR2 = new List<string> {
                "1", "2", "", "", "OSIRIS",
                "CUIT:", "20-12312312-1", "Anticipo:", "1234", "Sede:", "901",
            };
            azureServiceMock.Setup(az => az.ObtenerResultadoOCRAsync("2")).ReturnsAsync(resultOCR2);

            try
            {
                target.ProcesarCM05(archivos.Object, 32);
                Assert.Fail("Debió lanzar una excepción");
            }
            catch (ValidationCustomException vex)
            {
                Assert.AreEqual("No se pudieron obtener los coeficientes. Por favor, asegúrese de adjuntar el documento correspondiente.", vex.Message);

                azureServiceMock.Verify(x => x.AnalizarImagenAsync(It.IsAny<HttpPostedFileBase>()), Times.Exactly(2));
                azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo1.Object), Times.Once);
                azureServiceMock.Verify(x => x.AnalizarImagenAsync(archivo2.Object), Times.Once);

                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync(It.IsAny<string>()), Times.Exactly(2));
                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("1"), Times.Once);
                azureServiceMock.Verify(x => x.ObtenerResultadoOCRAsync("2"), Times.Once);

                repositorioMock.Verify(repo => repo.GuardarCambios(), Times.Never);
            }
            catch (Exception)
            {
                Assert.Fail("Debió lanzar una ValidationCustomException");
            }
        }
    }
}