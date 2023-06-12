using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasServiceTest
    {

        private ComprasService target;
        private Mock<IRepositorio> repositorioMock;
        private Mock<IObtenerCecoSolpConsumerMOA> cecoConsumerMock;
        private Mock<IObtenerCuentasSolpConsumerMOA> cuentasConsumerMock;
        private Mock<IObtenerOrdenSolpConsumerMOA> ordenesConsumerMock;
        private Mock<IObtenerServiciosSolpConsumerMOA> serviciosConsumerMock;
        private Mock<IObtenerSolpConsumerMOA> obtenerSolpConsumerMOAMock;
        private Mock<ICrearSolpConsumerMOA> crearSolpConsumerMOAMock;
        private Mock<IModificarSolpConsumerMOA> modificarSolpConsumerMOAMock;

        private Mock<IObtenerMaterialesSolpConsumerMOA> obtenerMaterialesSolpConsumerMOAMock;
        private Mock<ICrearPedidoConsumerMOA> crearPedidoConsumerMOAMock;
        private Mock<IObtenerFuenteAprovisionamientoConsumerMOA> obtenerFuenteAprovisionamientoConsumerMOAMock;
        private Mock<IObtenerContratoSolpConsumerMOA> obtenerContratoSolpConsumerMOAMock;
        private Mock<IVendedorService> vendedorServiceMock;
        private Mock<IObtenerTipoCambioConsumerMOA> obtenerTipoCambioConsumerMOAMock;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            cecoConsumerMock = new Mock<IObtenerCecoSolpConsumerMOA>();
            cuentasConsumerMock = new Mock<IObtenerCuentasSolpConsumerMOA>();
            ordenesConsumerMock = new Mock<IObtenerOrdenSolpConsumerMOA>();
            serviciosConsumerMock = new Mock<IObtenerServiciosSolpConsumerMOA>();
            obtenerSolpConsumerMOAMock = new Mock<IObtenerSolpConsumerMOA>();
            crearSolpConsumerMOAMock = new Mock<ICrearSolpConsumerMOA>();
            modificarSolpConsumerMOAMock = new Mock<IModificarSolpConsumerMOA>();

            obtenerMaterialesSolpConsumerMOAMock = new Mock<IObtenerMaterialesSolpConsumerMOA>();
            crearPedidoConsumerMOAMock = new Mock<ICrearPedidoConsumerMOA>();
            obtenerFuenteAprovisionamientoConsumerMOAMock = new Mock<IObtenerFuenteAprovisionamientoConsumerMOA>();
            obtenerContratoSolpConsumerMOAMock = new Mock<IObtenerContratoSolpConsumerMOA>();
            vendedorServiceMock = new Mock<IVendedorService>();
            obtenerTipoCambioConsumerMOAMock = new Mock<IObtenerTipoCambioConsumerMOA>();

            
            target = new ComprasService(
                repositorioMock.Object,
                cecoConsumerMock.Object,
                cuentasConsumerMock.Object,
                ordenesConsumerMock.Object,
                serviciosConsumerMock.Object,
                obtenerSolpConsumerMOAMock.Object,
                crearSolpConsumerMOAMock.Object,
                modificarSolpConsumerMOAMock.Object,
                obtenerMaterialesSolpConsumerMOAMock.Object,
                crearPedidoConsumerMOAMock.Object,
                obtenerFuenteAprovisionamientoConsumerMOAMock.Object,
                obtenerContratoSolpConsumerMOAMock.Object,
                vendedorServiceMock.Object,
                obtenerTipoCambioConsumerMOAMock.Object
                );
        }


        /*
        [Test()]
        public void GenerarZipPliegoConAdjuntoTest()
        {
            var pliegoMock = new Pliego()
            {
                Id = 1,
                Archivos = new List<Archivo> { new Archivo() { Id = 1, FileKey = FileKeys.AdjuntoSolp, Ruta = "" } }
            };

            var usuarioMock = new Usuario()
            {
                Id = 1,
                Mail = "test@test.com",
                CUITRegistro = "20202020202",
                Habilitado = true
            };

            var claseDocumentoMock = new TablaSap() { Id = 1, Codigo = "TEST", CodigoSap = "TEST", Descripcion = "TEST", Tabla = "TEST"};

            var tablaEstado = new TablaEstado() { Id = 1, Codigo = "TEST", Descripcion = "TEST", Tabla = "TEST", Color = "rojo", Orden = 1 };

            var posicionesMock = new List<SolpPosicion>() { new SolpPosicion() { Id = 1 } };

            var solpMock = new Solp() 
            { 
                Id = 1, 
                Pliego_Id = 1, 
                Pliego = pliegoMock, 
                UsuarioCreacion_Id = 1, 
                UsuarioCreacion = usuarioMock,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                UsuarioModificacion_Id = 1,
                UsuarioModificacion = usuarioMock,
                ClaseDocumento_Id = 1,
                ClaseDocumento = claseDocumentoMock,
                EstadoDocumento_Id = 1,
                EstadoSolpSap = claseDocumentoMock,
                EstadoSolpSap_Id = 1,
                EstadoDocumento = tablaEstado,
                Posiciones = posicionesMock
            };

            var pathbase = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Solp, bool>>>()))
               .Returns(solpMock);

            var expected = $"Solp-xxx-pliego-{DateTime.Now.ToString("yyyyMMdd")}.zip";
            var result = target.GenerarZipPliego(solpMock.Id, pathbase);

            Assert.AreEqual(expected, result);
        }*/

        [Test()]
        public void ObtenerCecoSapTest()
        {
            var rfcResultMock = new CecoWSMOAResponse()
            {
                Cecos = new List<Ceco>()
                {
                    new Ceco() { CostCenter = "MOA", CO_A = "MOA", Descripcion = "MOA"}
                }
            };

            cecoConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CecoSolpSap}
            };

            var result = target.ObtenerCecoSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void ObtenerCuentasSapTest()
        {
            var rfcResultMock = new CuentaWSMOAResponse()
            {
                Cuentas = new List<Cuenta>()
                {
                    new Cuenta() { Descripcion = "MOA", Codigo = "MOA", Comp = "MOA"}
                }
            };

            cuentasConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CuentasSolpSap}
            };

            var result = target.ObtenerCuentasSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void ObtenerOrdenesSapTest()
        {
            var rfcResultMock = new OrdenWSMOAResponse()
            {
                Ordenes = new List<Orden>()
                {
                    new Orden() { Descripcion = "MOA", Codigo = "MOA", CompCode = "MOA", Clase = "MOA", Tipo = "MOA"}
                }
            };

            ordenesConsumerMock.Setup(x => x.request("")).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.OrdenSolpSap}
            };

            var result = target.ObtenerOrdenesSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void ObtenerServiciosSapTest()
        {
            var rfcResultMock = new ServicioWSMOAResponse()
            {
                Servicios = new List<Servicio>()
                {
                    new Servicio() { Descripcion = "MOA", Codigo = "MOA", Serv = "MOA"}
                }
            };

            serviciosConsumerMock.Setup(x => x.request()).Returns(rfcResultMock);

            List<TablaSapDto> expected = new List<TablaSapDto>
            {
                new TablaSapDto {Id=0, Descripcion = "MOA", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap}
            };

            var result = target.ObtenerServiciosSap();

            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test()]
        public void AutocompleteServiciosSapTest()
        {
            List<TablaSap> ListaSap = new List<TablaSap>
            {
                new TablaSap {Id=1, Descripcion = "Prueba 1", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap},
                new TablaSap {Id=2, Descripcion = "Prueba 2", CodigoSap="Otro", Tabla = TablasSap.CodigoServicioSap},
                new TablaSap {Id=3, Descripcion = "Prueba 3", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap},
                new TablaSap {Id=4, Descripcion = "Prueba 1", CodigoSap="MOA Operaciones", Tabla = TablasSap.CodigoServicioSap},
                new TablaSap {Id=5, Descripcion = "Prueba 1", CodigoSap="MOA", Tabla = TablasSap.CecoSolpSap},
            };

            repositorioMock
                .Setup(x => x.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<TablaSap, object>>>>()))
                .Returns(ListaSap);

            var expected = new List<TablaSapDto>
            {
                new TablaSapDto { Id = 2,  Descripcion = "Prueba 2", CodigoSap="Otro", Tabla = TablasSap.CodigoServicioSap }
            };

            var result = target.AutocompleteTablaSap(TablasSap.CodigoServicioSap, "ot");

            Assert.AreEqual(expected[0].Id, result[0].Id);
        }

        [Test]
        public void ActualizarServiciosSolpOk()
        {
            ServicioWSMOAResponse servicioWSMOAResponseTest = new ServicioWSMOAResponse
            {
                error = "",
                Servicios = new List<Servicio>
                {
                    new Servicio{ Codigo = "001", Descripcion = "descripcion1", NroGrupo = "", Serv = "serv1", Ser = "ser1", Edit = "1", Bas = "base1", SSCItem = "sc1" },
                    new Servicio{ Codigo = "002", Descripcion = "descripcion2", NroGrupo = "2", Serv = "serv2", Ser = "ser2", Edit = "2", Bas = "base2", SSCItem = "sc2" },
                    new Servicio{ Codigo = "003", Descripcion = "descripcion3", NroGrupo = "3", Serv = "serv3", Ser = "ser3", Edit = "3", Bas = "base3", SSCItem = "sc3" },
                    new Servicio{ Codigo = "00A", Descripcion = "descripcion3", NroGrupo = "3", Serv = "serv3", Ser = "ser3", Edit = "3", Bas = "base3", SSCItem = "sc3" },
                },
            };

            this.serviciosConsumerMock
                .Setup(x => x.request())
                .Returns(servicioWSMOAResponseTest);

            List<ServicioSolp> listadoServiciosSolp = new List<ServicioSolp>
            {
                new ServicioSolp { Id = 1, CodigoSap = 1, Descripcion = "descripcion" },
                new ServicioSolp { Id = 2, CodigoSap = 2, Descripcion = "descripcion2" },
                new ServicioSolp { Id = 3, CodigoSap = 4, Descripcion = "descripcion3" },
            };

            this.repositorioMock
                .Setup(x => x.Listar<ServicioSolp>(null, 0, null, DirOrden.Asc, null))
                .Returns(listadoServiciosSolp);

            this.repositorioMock
                .Setup(x => x.Agregar(It.IsAny<ServicioSolp>()))
                .Callback<ServicioSolp>(servicioSolp => listadoServiciosSolp.Add(servicioSolp));

            target.ActualizarServiciosSolp();

            Assert.AreEqual(5, listadoServiciosSolp.Count);

            Assert.AreEqual("001", listadoServiciosSolp[3].Codigo);
            Assert.AreEqual(1, listadoServiciosSolp[3].CodigoSap);
            Assert.AreEqual("descripcion1", listadoServiciosSolp[3].Descripcion);
            Assert.IsNull(listadoServiciosSolp[3].GrupoArticulos);
            Assert.AreEqual("serv1", listadoServiciosSolp[3].TipoServicio);
            Assert.AreEqual("ser1", listadoServiciosSolp[3].AmbitoServicio);
            Assert.AreEqual(1, listadoServiciosSolp[3].Edicion);
            Assert.AreEqual("base1", listadoServiciosSolp[3].UnidadMedidaBase);
            Assert.AreEqual("sc1", listadoServiciosSolp[3].SSCItem);

            Assert.AreEqual("003", listadoServiciosSolp[4].Codigo);
            Assert.AreEqual(3, listadoServiciosSolp[4].CodigoSap);
            Assert.AreEqual("descripcion3", listadoServiciosSolp[4].Descripcion);
            Assert.AreEqual(3, listadoServiciosSolp[4].GrupoArticulos);
            Assert.AreEqual("serv3", listadoServiciosSolp[4].TipoServicio);
            Assert.AreEqual("ser3", listadoServiciosSolp[4].AmbitoServicio);
            Assert.AreEqual(3, listadoServiciosSolp[4].Edicion);
            Assert.AreEqual("base3", listadoServiciosSolp[4].UnidadMedidaBase);
            Assert.AreEqual("sc3", listadoServiciosSolp[4].SSCItem);

            Assert.AreEqual(3, listadoServiciosSolp[4].CodigoSap);

            this.serviciosConsumerMock.Verify(x => x.request(), Times.Once);
            this.repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }


        [Test]
        public void ObtenerRutaArchivo()
        {
            var rutaArchivo = "ruta/archivo";
            var archivoId = 1;

            repositorioMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Archivo, bool>>>()))
              .Returns(new Archivo
              {
                  Id = archivoId,
                  FileKey = "12323",
                  Ruta = rutaArchivo
              });

            var result = target.ObtenerRutaArchivo(archivoId);

            repositorioMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Archivo, bool>>>()), Times.Once);

            Assert.AreEqual(rutaArchivo, result);
        }
    }
}