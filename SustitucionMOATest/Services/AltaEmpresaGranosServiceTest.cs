using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    public class AltaEmpresaGranosServiceTest
    {
        private AltaEmpresaGranosService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IDataAgroService> dataAgroServiceMock;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            dataAgroServiceMock = new Mock<IDataAgroService>();
            target = new AltaEmpresaGranosService(repositorioMock.Object, dataAgroServiceMock.Object);
        }

        [Test]
        public void ArmarRutaCarpetaTest()
        {
            string fileKey = "Prueba";
            string rutaArchivoProveedores = "C:/Archivos";
            UsuarioGranos usuarioGranos = new UsuarioGranos { CUITRegistro = "123", Id = 10};

            var expected = "C:/Archivos/123/10/Prueba";

            var result = target.ArmarRutaCarpeta(fileKey,rutaArchivoProveedores,usuarioGranos);

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void ValidarArchivosSubidosInformeComercialFaltante()
        {
            var usuarioGranos = new UsuarioGranos { Archivos = new List<Archivo>() };

            var infoProveedor = new InfoProveedorDataAgroDto { };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarArchivosSubidos(usuarioGranos, infoProveedor));

            var expected = string.Format(ErrorMsg.ErrorArchivoRequerido, "Informe comercial firmado");

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ValidarArchivosSubidosConstanciaCBUFaltante()
        {
            var usuarioGranos = new UsuarioGranos { Archivos = new List<Archivo>() };

            usuarioGranos.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });

            var infoProveedor = new InfoProveedorDataAgroDto { };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarArchivosSubidos(usuarioGranos, infoProveedor));

            var expected = string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CBU");

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ValidarArchivosSubidosSIPERRequeridoFaltante()
        {
            var usuarioGranos = new UsuarioGranos { Archivos = new List<Archivo>() };

            usuarioGranos.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            usuarioGranos.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            var infoProveedor = new InfoProveedorDataAgroDto { estadoSISA = "2" };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarArchivosSubidos(usuarioGranos, infoProveedor));

            var expected = string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER");

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ValidarArchivosSubidosSIPERNoRequeridoFaltante()
        {
            var usuarioGranos = new UsuarioGranos { Archivos = new List<Archivo>() };

            usuarioGranos.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            usuarioGranos.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });
            var infoProveedor = new InfoProveedorDataAgroDto { estadoSISA = "1" };

            var result = target.ValidarArchivosSubidos(usuarioGranos, infoProveedor);

            Assert.AreEqual(true, result);
        }
    }
}
