using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
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
            Proveedor proveedor = new Proveedor { CUIT = "123", Id = 10 };

            var expected = "C:/Archivos/123/10/Prueba";

            var result = target.ArmarRutaCarpeta(fileKey, rutaArchivoProveedores, proveedor);

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void ValidarArchivosSubidosInformeComercialFaltante()
        {
            var proveedor = new Proveedor { Archivos = new List<Archivo>() };

            var infoProveedor = new InfoProveedorDataAgroDto { };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarArchivosSubidos(proveedor, infoProveedor));

            var expected = string.Format(ErrorMsg.ErrorArchivoRequerido, "Informe comercial firmado");

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ValidarArchivosSubidosConstanciaCBUFaltante()
        {
            var proveedor = new Proveedor { Archivos = new List<Archivo>() };

            proveedor.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });

            var infoProveedor = new InfoProveedorDataAgroDto { };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarArchivosSubidos(proveedor, infoProveedor));

            var expected = string.Format(ErrorMsg.ErrorArchivoRequerido, "Constancia CBU");

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ValidarArchivosSubidosSIPERRequeridoFaltante()
        {
            var proveedor = new Proveedor { Archivos = new List<Archivo>() };

            proveedor.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            proveedor.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            var infoProveedor = new InfoProveedorDataAgroDto { EstadoSISA = "2" };

            var ex = Assert.Throws<ValidationCustomException>(() => target.ValidarArchivosSubidos(proveedor, infoProveedor));

            var expected = string.Format(ErrorMsg.ErrorArchivoRequerido, "SIPER");

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void ValidarArchivosSubidosSIPERNoRequeridoFaltante()
        {
            var proveedor = new Proveedor { Archivos = new List<Archivo>() };

            proveedor.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            proveedor.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });
            var infoProveedor = new InfoProveedorDataAgroDto { EstadoSISA = "1" };

            var result = target.ValidarArchivosSubidos(proveedor, infoProveedor);

            var expected = true;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GrabarProveedorAltaInternaGranosTest()
        {
            var mailUsuario = "existente@mail.com";
            var infoDataAgro = new SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial
            {
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                ProveedorSISAEstadoCuit = "1",
                ProveedorMails= new string[] {mailUsuario.ToUpper()}
            };

            int proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                RazonSocial = "test",
                CUIT = "333333333333",
                Archivos = new List<Archivo>(),
            };

            var proveedorOk2 = new Proveedor
            {
                Id = 2,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                RazonSocial = "test",
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
            };

            var usuarioGranosOk = new Usuario
            {
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>() { proveedorOk2 }
            };

            var tipoUsuarioMock = new TipoUsuario { Id = 1, Nombre = "Granos", NombreCorto = "G" }; 

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuarioGranosOk);

            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);

            repositorioMock
              .Setup(x => x.Obtener(It.IsAny<Expression<Func<TipoUsuario, bool>>>()))
                .Returns(tipoUsuarioMock);

            dataAgroServiceMock.Setup(s => s.ObtenerValidarCUITProveedorGranos(It.IsAny<string>(), It.IsAny<bool>())).Returns(infoDataAgro);

            var result = target.GrabarProveedorAltaInternaGranos("333333333333", mailUsuario, mailUsuario);
            var expected = SuccessMsg.AltaVendedorOK;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void NotificarSolicitudTest()
        {
            var infoDataAgro = new SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial
            {
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                ProveedorSISAEstadoCuit = "1"
            };

            var mailUsuario = "existente@mail.com";

            int proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                RazonSocial = "test",
                CUIT = "333333333333",
                Archivos = new List<Archivo>(),
                TipoProveedor = new TipoUsuario() { Id = 1, Nombre = "Granos", NombreCorto = "Granos" },
                AltaInterna = true
            };

            var proveedorOk2 = new Proveedor
            {
                Id = 2,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                RazonSocial = "test",
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
                TipoProveedor = new TipoUsuario() { Id = 1, Nombre = "Granos", NombreCorto = "Granos" },
                AltaInterna = true
            };

            var usuarioGranosOk = new Usuario
            {
                Mail = mailUsuario,
                CUITRegistro = "233333333333",
                Proveedores = new List<Proveedor>() { proveedorOk2 },
                TipoUsuario = new TipoUsuario() { Id = 1, Nombre = "Granos", NombreCorto = "Granos"}
            };

            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });
            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.DDJJ });
            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.SIPER });

            AltaEmpresaViewModel altaempresaMock = new AltaEmpresaViewModel() { VinculoConEmpleadosDeMolinos = false, VinculoConFuncionariosPublicos = false};

            var tipoUsuarioMock = new TipoUsuario { Id = 1, Nombre = "Granos", NombreCorto = "G" };

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuarioGranosOk);

            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);

            repositorioMock
              .Setup(x => x.Obtener(It.IsAny<Expression<Func<TipoUsuario, bool>>>()))
                .Returns(tipoUsuarioMock);          

            dataAgroServiceMock.Setup(s => s.ObtenerValidarCUITProveedorGranos(It.IsAny<string>(), It.IsAny<bool>())).Returns(infoDataAgro);

            var result = target.SolicitudAltaInterna(mailUsuario, proveedorId, altaempresaMock);
            var expected = SuccessMsg.ValidacionPendienteOK;

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ValidarEstadoSolicitudHabilitadoTest()
        {
            var proveedor = new Proveedor { EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente };

            var result = target.ValidarEstadoSolicitud(proveedor);

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
                EstadoSISA = "1",
                RazonSocial = "test",
                ProveedorCUIT = "23-33333333-33"
            };

            var infoDataAgro = new SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial
            {
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                ProveedorSISAEstadoCuit = "1"
            };

            var mailUsuario = "existente@mail.com";

            var usuarioGranosOk = new Usuario
            {
                Mail = mailUsuario,
                Proveedores = new List<Proveedor>()
            };

            int proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                RazonSocial = "test",
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
            };


            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            usuarioGranosOk.Proveedores.Add(proveedorOk);

            usuarioGranosOk.Mail = mailUsuario;

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuarioGranosOk);


            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);


            dataAgroServiceMock.Setup(s => s.ObtenerValidarCUITProveedorGranos(It.IsAny<string>(), It.IsAny<bool>())).Returns(infoDataAgro);

            target = new AltaEmpresaGranosService(repositorioMock.Object, dataAgroServiceMock.Object);

            var result = target.ObtenerInfoProveedor(mailUsuario, proveedorId);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetLocalidadTest()
        {
            var localidad = new Localidad
            {
                LocalidadId = 1,
                CodLocalidad = 1,
                Nombre = "TEST",
                Provincia = new Provincia() { Nombre = "BSAS", Orden = 1, ProvinciaId = 1 },
                ProvinciaId = 1,
                Partido = new Partido() { Id = 1, ProvinciaID = 1, Descripcion = "PartidoTest" },
                PartidoId = 1
            };

            repositorioMock
              .Setup(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()))
                .Returns(localidad);

            target = new AltaEmpresaGranosService(repositorioMock.Object, dataAgroServiceMock.Object);

            var result = target.GetLocalidad(localidad.LocalidadId);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()), Times.Once);

            Assert.AreEqual(localidad, result);
        }

        [Test]
        public void GerLocalidadConIdInvalido()
        {
            var id = 0;

            var ex = Assert.Throws<InfoCustomException>(() => target.GetLocalidad(id));

            var expected = "Id de localidad invalido";

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void GerLocalidadConLocalidadNoExistente()
        {
            var id = 2;

            var ex = Assert.Throws<InfoCustomException>(() => target.GetLocalidad(id));

            var expected = "No existe la localidad";

            Assert.AreEqual(expected, ex.Message);
        }

        [Test]
        public void EnviarSolicitudUsuarioTest()
        {
            var infoDataAgro = new SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial
            {
                ProveedorCBU = "1234",
                ProveedorClasificacion = "Productor",
                ProveedorSISAEstadoCuit = "1"
            };
            var mailUsuario = "existente@mail.com";

            var usuarioGranosOk = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Granos" },
                Proveedores = new List<Proveedor>()
            };

            int proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios>(),
                TipoProveedor = new TipoUsuario { Nombre = "Granos" , NombreCorto = "G" }
            };


            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            usuarioGranosOk.Proveedores.Add(proveedorOk);

            usuarioGranosOk.Mail = mailUsuario;

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuarioGranosOk);


            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);


            dataAgroServiceMock.Setup(s => s.ObtenerValidarCUITProveedorGranos(It.IsAny<string>(), It.IsAny<bool>())).Returns(infoDataAgro);
            List<AltaEmpresaEmpleadosViewModel> Empleados = new List<AltaEmpresaEmpleadosViewModel>();
            Empleados.Add(new AltaEmpresaEmpleadosViewModel { Vinculo = "", CargoProveedora = "", NombreMolinos = "", NombreProveedora = "" });
            List<AltaEmpresaFuncionariosViewModel> Funcionarios = new List<AltaEmpresaFuncionariosViewModel>();
            Funcionarios.Add(new AltaEmpresaFuncionariosViewModel { CargoFirma = "", CargoFuncionario = "", NombreFirma = "", NombreFuncionario = "", Vinculo = "" });

            var altaempresa = new AltaEmpresaViewModel
            {
                VinculoConEmpleadosDeMolinos = false,
                VinculoConFuncionariosPublicos = false,
                Empleados = Empleados,
                Funcionarios = Funcionarios
            };
            var result = target.EnviarSolicitudUsuario(mailUsuario, proveedorId, false, altaempresa);

            var expected = SuccessMsg.ValidacionPendienteOK;

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Proveedor>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ObtenerArchivosSubidos()
        {
            var mailUsuario = "existente@mail.com";

            var usuarioGranosOk = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Granos" },
                Proveedores = new List<Proveedor>()
            };

            int proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
            };

            var expected = new List<ArchivoDto>();

            Archivo archivoInfoComercial = new Archivo
            {
                FileKey = FileKeys.InformeComercialFirmado,
                Id = 1,
                Ruta = "C:/test.txt"
            };

            Archivo archivoConstanciaCBU = new Archivo
            {
                FileKey = FileKeys.ConstanciaCBU,
                Id = 2,
                Ruta = "C:/test2.txt"
            };

            expected.Add(new ArchivoDto(archivoInfoComercial));
            expected.Add(new ArchivoDto(archivoConstanciaCBU));

            proveedorOk.Archivos.Add(archivoInfoComercial);
            proveedorOk.Archivos.Add(archivoConstanciaCBU);

            usuarioGranosOk.Proveedores.Add(proveedorOk);

            repositorioMock
              .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(usuarioGranosOk);


            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);


            target = new AltaEmpresaGranosService(repositorioMock.Object, dataAgroServiceMock.Object);

            var result = target.ObtenerArchivosSubidos(mailUsuario, proveedorId, false);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Archivo>()), Times.Never);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ObtenerArchivoTest()
        {
            var mailUsuario = "existente@mail.com";

            var usuarioGranosOk = new Usuario
            {
                Mail = mailUsuario,
                TipoUsuario = new TipoUsuario { Id = 1, Nombre = "Granos" },
                Proveedores = new List<Proveedor>()
            };

            int proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
            };


            Archivo archivoInfoComercial = new Archivo
            {
                FileKey = FileKeys.InformeComercialFirmado,
                Id = 1,
                Ruta = "C:/test.txt"
            };

            Archivo archivoConstanciaCBU = new Archivo
            {
                FileKey = FileKeys.ConstanciaCBU,
                Id = 2,
                Ruta = "C:/test2.txt"
            };

            proveedorOk.Archivos.Add(archivoInfoComercial);
            proveedorOk.Archivos.Add(archivoConstanciaCBU);

            usuarioGranosOk.Proveedores.Add(proveedorOk);


            repositorioMock
              .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
              .Returns(usuarioGranosOk);


            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);



            target = new AltaEmpresaGranosService(repositorioMock.Object, dataAgroServiceMock.Object);

            var result = target.ObtenerArchivo(mailUsuario, 2, proveedorId);

            var expected = "C:/test2.txt";

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Archivo>()), Times.Never);
            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

            Assert.AreEqual(expected, result);
        }


        [Test]
        public void CargarSolicitudUsuarioTest()
        {
            var mailUsuario = "existente@mail.com";

            var usuarioOk = new Usuario
            {
                Mail = mailUsuario,
                Proveedores = new List<Proveedor>(),

            };

            var proveedorId = 1;

            var proveedorOk = new Proveedor
            {
                Id = proveedorId,
                Mail = mailUsuario,
                EstadoAprobacion = EstadoAprobacion.DocumentacionPendiente,
                CUIT = "233333333333",
                Archivos = new List<Archivo>(),
                RelacionConEmpleados = new List<ProveedorRelacionConEmpleados>(),
                RelacionConFuncionarios = new List<ProveedorRelacionConFuncionarios> { new ProveedorRelacionConFuncionarios { CargoFirma = "", CargoFuncionario = "", Id = 1, NombreFirma = "", NombreFuncionario = "", Proveedor_Id = 1, Vinculo = "" } },
                VinculoConEmpleadosDeMolinos = false,
                VinculoConFuncionariosPublicos = true
            };

            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.InformeComercialFirmado });
            proveedorOk.Archivos.Add(new Archivo { FileKey = FileKeys.ConstanciaCBU });

            usuarioOk.Proveedores.Add(proveedorOk);

            usuarioOk.Mail = mailUsuario;

            repositorioMock
                .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuarioOk);


            repositorioMock
              .Setup(x => x.Obtener<Proveedor>(It.IsAny<int>()))
              .Returns(proveedorOk);


            var result = target.CargarSolicitudUsuario(mailUsuario, proveedorId);

            var expected = new AltaEmpresaViewModel
            {
                VinculoConEmpleadosDeMolinos = proveedorOk.VinculoConEmpleadosDeMolinos,
                VinculoConFuncionariosPublicos = proveedorOk.VinculoConFuncionariosPublicos,
                Empleados = proveedorOk.RelacionConEmpleados.Select(a => new AltaEmpresaEmpleadosViewModel { CargoProveedora = a.CargoProveedora, NombreMolinos = a.NombreMolinos, NombreProveedora = a.NombreProveedora, Vinculo = a.Vinculo }).ToList(),
                Funcionarios = proveedorOk.RelacionConFuncionarios.Select(a => new AltaEmpresaFuncionariosViewModel { CargoFirma = a.CargoFirma, CargoFuncionario = a.CargoFuncionario, NombreFirma = a.NombreFirma, NombreFuncionario = a.NombreFuncionario, Vinculo = a.Vinculo }).ToList()
            };

            Assert.AreEqual(expected.VinculoConEmpleadosDeMolinos, result.VinculoConEmpleadosDeMolinos);
            Assert.AreEqual(expected.VinculoConFuncionariosPublicos, result.VinculoConFuncionariosPublicos);
            Assert.AreEqual(expected.Empleados.Count, result.Empleados.Count);
            Assert.AreEqual(expected.Funcionarios.Count, result.Funcionarios.Count);
        }
    }
}
