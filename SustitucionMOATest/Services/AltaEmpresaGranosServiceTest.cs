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
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

            var expected = true;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidarEstadoSolicitudHabilitadoTest()
        {
            var proveedor = new Proveedor { EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente };

            var result =  target.ValidarEstadoSolicitud(proveedor);

            var expected = true;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidarEstadoSolicitudDesHabilitadoTest()
        {
            var proveedor = new Proveedor { EstadoAprobacion = EstadoAprobacion.AprobacionPendiente };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarEstadoSolicitud(proveedor));

            var expected = ErrorMsg.EstadoIncorrectoSolicitud;

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ObtenerInfoProveedorTest()
        {
            var expected = new InfoProveedorDataAgroDto
            {
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                estadoSISA = "1"
            };

            var infoDataAgro = new ResultadoValidarProveedorComercial { 
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                ProveedorSISACodCategoria = "1"
            };

            var usuarioGranosOk = new UsuarioGranos { Archivos = new List<Archivo>(), Proveedores = new List<Proveedor>() };

            var proveedorOk = new Proveedor { EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente, CUIT = "233333333333" };

            usuarioGranosOk.Proveedores.Add(proveedorOk);

            usuarioGranosOk.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            usuarioGranosOk.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            var mailUsuario = "existente@mail.com";

            usuarioGranosOk.Mail = mailUsuario;

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<UsuarioGranos, bool>>>()))
                .Returns(usuarioGranosOk);


            dataAgroServiceMock.Setup(s => s.ObtenerValidarCUITProveedorGranos(It.IsAny<string>())).Returns(infoDataAgro);

            target = new AltaEmpresaGranosService(repositorioMock.Object, dataAgroServiceMock.Object);

            var result = target.ObtenerInfoProveedor(mailUsuario);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void EnviarSolicitudUsuarioTest()
        {
            var infoDataAgro = new ResultadoValidarProveedorComercial
            {
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                ProveedorSISACodCategoria = "1"
            };

            var usuarioGranosOk = new UsuarioGranos { Archivos = new List<Archivo>(), Proveedores = new List<Proveedor>() };

            var proveedorOk = new Proveedor { EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente, CUIT = "233333333333" };

            usuarioGranosOk.Proveedores.Add(proveedorOk);

            usuarioGranosOk.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            usuarioGranosOk.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            var mailUsuario = "existente@mail.com";

            usuarioGranosOk.Mail = mailUsuario;

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<UsuarioGranos, bool>>>()))
                .Returns(usuarioGranosOk);

            dataAgroServiceMock.Setup(s => s.ObtenerValidarCUITProveedorGranos(It.IsAny<string>())).Returns(infoDataAgro);

            var result = target.EnviarSolicitudUsuario(mailUsuario);

            var expected = SuccessMsg.ValidacionPendienteOK;

            Assert.AreEqual(expected, result);
        }
    }
}
