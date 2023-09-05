using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        private Mock<IObtenerOrdenDeCompraConsumerMOA> obtenerOrdenDeCompraConsumerMOAMock;
        private Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA> obtenerOrdenesDeCompraParaSOLPConsumerMOAMock;
        private Mock<IHttpContextService> httpContextServiceMock;
        private Mock<IObtenerRegistroInfoConsumerMOA> obtenerRegistroInfoConsumerMOAMock;
        private Mock<IUsuarioService> usuarioServiceMock;
        private Mock<IObtenerProveedorConsumerMOA> obtenerProveedorConsumerMOA;
        private Mock<IModificarOrdenDeCompraConsumerMOA> modificarOrdenDeCompraConsumerMOAMock;
        private string filePath = "";
        private Mock<IVendedoresConsumerMOA> vendedoresConsumerMOAMock;
        private Mock<IAgregarRegistroInfoConsumerMOA> agregarRegistroInfoConsumerMOAMock;

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
            httpContextServiceMock = new Mock<IHttpContextService>();
            obtenerRegistroInfoConsumerMOAMock = new Mock<IObtenerRegistroInfoConsumerMOA>();
            modificarOrdenDeCompraConsumerMOAMock = new Mock<IModificarOrdenDeCompraConsumerMOA>();
            agregarRegistroInfoConsumerMOAMock = new Mock<IAgregarRegistroInfoConsumerMOA>();
            

            // httpContextServiceMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png");

            filePath = Path.GetFullPath(TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png");
            obtenerOrdenDeCompraConsumerMOAMock = new Mock<IObtenerOrdenDeCompraConsumerMOA>();
            obtenerOrdenesDeCompraParaSOLPConsumerMOAMock = new Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA>();
            usuarioServiceMock = new Mock<IUsuarioService>();
            obtenerProveedorConsumerMOA = new Mock<IObtenerProveedorConsumerMOA>();
            vendedoresConsumerMOAMock = new Mock<IVendedoresConsumerMOA>();

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
                obtenerTipoCambioConsumerMOAMock.Object,
                httpContextServiceMock.Object,
                obtenerRegistroInfoConsumerMOAMock.Object,
                obtenerOrdenDeCompraConsumerMOAMock.Object,
                obtenerOrdenesDeCompraParaSOLPConsumerMOAMock.Object,
                usuarioServiceMock.Object,
                obtenerProveedorConsumerMOA.Object,
                modificarOrdenDeCompraConsumerMOAMock.Object,
                vendedoresConsumerMOAMock.Object,
                agregarRegistroInfoConsumerMOAMock.Object
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
                new ServicioSolp { Id = 3, CodigoSap = 4, Descripcion = "descripcion3", Codigo = "003", TipoServicio = "serv3",
                    AmbitoServicio = "ser3", Edicion = 3, UnidadMedidaBase = "base3", SSCItem = "sc3"},
            };

            this.repositorioMock
                .Setup(x => x.Listar<ServicioSolp>(null, 0, null, DirOrden.Asc, null))
                .Returns(listadoServiciosSolp);

            this.repositorioMock
                .Setup(x => x.Agregar(It.IsAny<ServicioSolp>()))
                .Callback<ServicioSolp>(servicioSolp => listadoServiciosSolp.Add(servicioSolp));

            target.ActualizarServiciosSolp();

            Assert.AreEqual(5, listadoServiciosSolp.Count);

            Assert.AreEqual("003", listadoServiciosSolp[3].Codigo);
            Assert.AreEqual(4, listadoServiciosSolp[3].CodigoSap);
            Assert.AreEqual("descripcion3", listadoServiciosSolp[3].Descripcion);

            Assert.AreEqual("serv3", listadoServiciosSolp[3].TipoServicio);
            Assert.AreEqual("ser3", listadoServiciosSolp[3].AmbitoServicio);
            Assert.AreEqual(3, listadoServiciosSolp[3].Edicion);
            Assert.AreEqual("base3", listadoServiciosSolp[3].UnidadMedidaBase);
            Assert.AreEqual("sc3", listadoServiciosSolp[3].SSCItem);

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

        [Test]
        public void GrabarAdjudicacionMaterialOk()
        {
            var adjudicacionDto = new AdjudicacionDto
            {
                AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>
                {
                    new AdjudicacionPosicionDto
                    {
                        Adjudicacion_Id = 1,
                        Cantidad = 1000,
                        CotizacionPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion_Id = 1,
                        PrecioTotal = 1000,
                        MonedaId = 1,
                    }
                },
                Cotizacion_Id = 1,
                FechaCreacion = DateTime.Now,
                Moneda_Id = 1,
                UsuarioCreador_Id = 1,
                Solp_Id = 1,
                CondicionesDeEntrega = "Condiciones"
            };
            var cotizacion = new Cotizacion
            {
                PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                {
                    PeticionDeOferta = new PeticionDeOferta
                    {
                        Solp = new Solp
                        {
                            Id = 1,
                            Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral
                                    {
                                        Codigo = "MATERIALES"
                                    },
                                }
                            }
                        }
                    }
                },
                CotizacionPosiciones = new List<CotizacionPosicion>()
                {
                    new CotizacionPosicion
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Moneda_Id = 1,
                        Precio = 1000,
                    }
                }
            };
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
               .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "bmelgarejo@prueba.com.ar" });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
               .Returns(cotizacion);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
                .Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse
                {
                    MonedaDestino = "ARP",
                    MonedaOrigen = "USD",
                    TipoCambio = 450
                });
            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });
            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarAdjudicacionServicioOk()
        {
            var adjudicacionDto = new AdjudicacionDto
            {
                AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>
                {
                    new AdjudicacionPosicionDto
                    {
                        Adjudicacion_Id = 1,
                        Cantidad = 1000,
                        CotizacionPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion_Id = 1,
                        PrecioTotal = 1000,
                        MonedaId = 1,
                    }
                },
                Cotizacion_Id = 1,
                FechaCreacion = DateTime.Now,
                Moneda_Id = 1,
                UsuarioCreador_Id = 1,
                Solp_Id = 1,
                CondicionesDeEntrega = "Condiciones"
            };
            var cotizacion = new Cotizacion
            {
                PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                {
                    PeticionDeOferta = new PeticionDeOferta
                    {
                        Solp = new Solp
                        {
                            Id = 1,
                            Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral
                                    {
                                        Codigo = "SERVICIOS"
                                    },
                                }
                            }
                        }
                    }
                },
                CotizacionPosiciones = new List<CotizacionPosicion>()
                {
                    new CotizacionPosicion
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Moneda_Id = 1,
                        Precio = 1000,
                        CotizacionSubPosiciones = new List<CotizacionSubPosicion>
                        {
                            new CotizacionSubPosicion
                            {
                                Precio = 1000,
                                Cantidad = 1000,
                                Moneda_Id = 1
                            }
                        }
                    }
                }
            };
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
               .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "bmelgarejo@prueba.com.ar" });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
               .Returns(cotizacion);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
                .Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse
                {
                    MonedaDestino = "ARP",
                    MonedaOrigen = "USD",
                    TipoCambio = 450
                });
            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });
            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void ListarAdjudicaciones()
        {
            var adjudicacionId = 1;
            var nroSolp = "0212303121";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<Expression<Func<Solp, string>>>()))
            .Returns(nroSolp);
            obtenerOrdenesDeCompraParaSOLPConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<OrdenDeCompraSAPDto> {
                new OrdenDeCompraSAPDto { Cabecera = new OrdenDeCompraSAPCabecera { Tipo = "", OrdenDeCompra = "", FechaCreacion = new DateTime(), RazonSocialProveedor = "Proveedor", Moneda = "ARP", MontoTotal = 1500 } }
            });
            var result = target.ListarAdjudicaciones(adjudicacionId);
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<Expression<Func<Solp, string>>>()), Times.Once);
        }

        [Test]
        public void ObtenerAdjudicacion()
        {
            var adjudicacionId = 1;
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<Expression<Func<Adjudicacion, AdjudicacionDto>>>()))
            .Returns(new AdjudicacionDto { });
            var result = target.ObtenerAdjudicacion(adjudicacionId);
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<Expression<Func<Adjudicacion, AdjudicacionDto>>>()), Times.Once);
        }

        [Test]
        public void GrabarPeticionDeOfertaOk()
        {

            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = new Solp
                {
                    TrabajoYaHecho = true,
                    Pliego = new Pliego
                    {
                        FechaHoraEntrega = DateTime.Now
                    }
                }
            };
            var peticionDeOferta = new GuardarPeticionDeOfertaDto
            {
                Id = 1,
                Adjuntos = null,
                Observacion = "",
                PosIds = new List<int> { 1 },
                SolpId = 1,
                UsuarioIds = new List<int> { 1 },
                UsuarioActual = new UsuarioDto
                {
                    Id = 1
                }
            };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });


            var result = target.GrabarPeticionDeOferta(peticionDeOferta, null, false, null);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarCotizacionOk()
        {

            var cotizacion = new GuardarCotizacion
            {
                Cantidad = 1000,
                CotizacionPosiciones = new List<GuardarCotizacionPosicionDto>
                {
                    new GuardarCotizacionPosicionDto
                    {
                        Cantidad = 1,
                        FechaDeEntrega = DateTime.Now,
                        MonedaId = 1,
                        NoDisponible = true,
                        PeticionDeOfertaSolpPosicionId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        TotalPesos = 1000,
                        UnidadDeMedidaId = 1,
                        FechaDeVigencia= DateTime.Now,
                    },
                    new GuardarCotizacionPosicionDto
                    {
                        Cantidad = 1,
                        FechaDeEntrega = DateTime.Now,
                        MonedaId = 1,
                        NoDisponible = false,
                        PeticionDeOfertaSolpPosicionId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        TotalPesos = 1000,
                        UnidadDeMedidaId = 1
                    },
                },
                EsFinalizado = false,
                MonedaId = 1,
                ObservacionEconomica = "",
                ObservacionTecnica = "",
                FechaDeEntrega = DateTime.Now,
                FechaDeVigencia = DateTime.Now,
                CotizacionSubposiciones = new List<CotizacionSubposicionesDto>
                {
                    new CotizacionSubposicionesDto
                    {
                        Cantidad = 1,
                        MonedaId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        UnidadDeMedidaId = 1
                    }
                }
            };

            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1 });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(new PeticionDeOfertaUsuario { Id = 1 });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });


            repositorioMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(new Cotizacion { Id = 1, CotizacionEstado_Id = 1 });


            var result = target.GrabarCotizacion(cotizacion, null, false, 1, false);

            repositorioMock.Verify(y => y.Obtener<Usuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Cotizacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarCotizacionEditarOk()
        {

            var cotizacion = new GuardarCotizacion
            {
                Cantidad = 1000,
                CotizacionId = 1,
                CotizacionPosiciones = new List<GuardarCotizacionPosicionDto>
                {
                    new GuardarCotizacionPosicionDto
                    {
                        Cantidad = 1,
                        FechaDeEntrega = DateTime.Now,
                        MonedaId = 1,
                        NoDisponible = true,
                        PeticionDeOfertaSolpPosicionId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        TotalPesos = 1000,
                        UnidadDeMedidaId = 1
                    },
                    new GuardarCotizacionPosicionDto
                    {
                        Cantidad = 1,
                        FechaDeEntrega = DateTime.Now,
                        MonedaId = 1,
                        NoDisponible = false,
                        PeticionDeOfertaSolpPosicionId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        TotalPesos = 1000,
                        UnidadDeMedidaId = 1
                    },
                },
                EsFinalizado = false,
                MonedaId = 1,
                ObservacionEconomica = "",
                ObservacionTecnica = "",
                FechaDeEntrega = DateTime.Now,
                CotizacionSubposiciones = new List<CotizacionSubposicionesDto>
                {
                    new CotizacionSubposicionesDto
                    {
                        Cantidad = 1,
                        MonedaId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        UnidadDeMedidaId = 1
                    }
                }
            };
            var cotizacionEntidad = new Cotizacion
            {
                PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                {
                    PeticionDeOferta = new PeticionDeOferta
                    {
                        Solp = new Solp
                        {
                            Id = 1,
                            Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral
                                    {
                                        Codigo = "MATERIALES"
                                    },
                                }
                            }
                        }
                    }
                },
                CotizacionPosiciones = new List<CotizacionPosicion>()
                {
                    new CotizacionPosicion
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Moneda_Id = 1,
                        Precio = 1000,
                        PeticionDeOfertaSolpPosicion_Id = 1,
                        CotizacionSubPosiciones = new List<CotizacionSubPosicion>
                        {
                            new CotizacionSubPosicion
                            {
                                Cantidad = 1000,
                                Moneda_Id = 1,
                                Moneda = new TablaSap{ Codigo = "ARP", Id = 1},
                                Precio = 10000,
                                UnidadDeMedida_Id = 1,
                                UnidadDeMedida = new TablaSap{ Codigo = "ARP", Id = 1},
                                CotizacionPosicion_Id = 0,
                                SolpSubPosicion_Id = 1,

                            },
                             new CotizacionSubPosicion
                            {
                                Cantidad = 1000,
                                Moneda_Id = 1,
                                Moneda = new TablaSap{ Codigo = "ARP", Id = 1},
                                Precio = 10000,
                                UnidadDeMedida_Id = 1,
                                UnidadDeMedida = new TablaSap{ Codigo = "ARP", Id = 1},
                                CotizacionPosicion_Id = 1,
                                SolpSubPosicion_Id = 1,

                            }
                        }
                    }
                }
            };
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1 });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(new PeticionDeOfertaUsuario { Id = 1 });

            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
             .Returns(cotizacionEntidad);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.GrabarCotizacion(cotizacion, null, false, 1, false);

            repositorioMock.Verify(y => y.Obtener<Usuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void CrearCotizacionAutomaticaOk()
        {

            var cotizacion = new GuardarCotizacion
            {
                Cantidad = 1000,
                CotizacionId = 1,
                CotizacionPosiciones = new List<GuardarCotizacionPosicionDto>
                {
                    new GuardarCotizacionPosicionDto
                    {
                        Cantidad = 1,
                        FechaDeEntrega = DateTime.Now,
                        MonedaId = 1,
                        NoDisponible = true,
                        PeticionDeOfertaSolpPosicionId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        TotalPesos = 1000,
                        UnidadDeMedidaId = 1
                    },
                    new GuardarCotizacionPosicionDto
                    {
                        Cantidad = 1,
                        FechaDeEntrega = DateTime.Now,
                        MonedaId = 1,
                        NoDisponible = false,
                        PeticionDeOfertaSolpPosicionId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        TotalPesos = 1000,
                        UnidadDeMedidaId = 1
                    },
                },
                EsFinalizado = false,
                MonedaId = 1,
                ObservacionEconomica = "",
                ObservacionTecnica = "",
                FechaDeEntrega = DateTime.Now,
                CotizacionSubposiciones = new List<CotizacionSubposicionesDto>
                {
                    new CotizacionSubposicionesDto
                    {
                        Cantidad = 1,
                        MonedaId = 1,
                        Precio = 1000,
                        PrecioTotal = 1000,
                        UnidadDeMedidaId = 1
                    }
                }
            };
            var cotizacionEntidad = new Cotizacion
            {
                Id = 1,
                PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                {
                    PeticionDeOferta = new PeticionDeOferta
                    {
                        Usuario = new Usuario
                        {
                            Mail = "bmelgarejo@prueba.com"
                        },
                        Solp = new Solp
                        {
                            UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com", },
                            NroSolp = "3344534",
                        },
                        Posiciones = new List<PeticionDeOfertaSolpPosicion>
                {
                    new PeticionDeOfertaSolpPosicion
                    {
                        PeticionDeOferta_Id = 1,
                        SolpPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion = new SolpPosicion
                        {
                            Unidad_Id = 1,
                            Moneda_Id = 1,
                            Cantidad = 1000,
                            PrecioBruto = 1000,
                            Subposiciones = new List<SolpSubposicion>
                            {

                            }
                        }
                    }
                },
                        Usuarios = new List<PeticionDeOfertaUsuario>
                {
                    new PeticionDeOfertaUsuario
                    {
                        Id = 1,
                    }
                }
                    }
                },
                CotizacionPosiciones = new List<CotizacionPosicion>()
                {
                    new CotizacionPosicion
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Moneda_Id = 1,
                        Precio = 1000,
                        PeticionDeOfertaSolpPosicion_Id = 1,
                        CotizacionSubPosiciones = new List<CotizacionSubPosicion>
                        {
                            new CotizacionSubPosicion
                            {
                                Cantidad = 1000,
                                Moneda_Id = 1,
                                Moneda = new TablaSap{ Codigo = "ARP", Id = 1},
                                Precio = 10000,
                                UnidadDeMedida_Id = 1,
                                UnidadDeMedida = new TablaSap{ Codigo = "ARP", Id = 1},
                                CotizacionPosicion_Id = 0,
                                SolpSubPosicion_Id = 1,

                            },
                             new CotizacionSubPosicion
                            {
                                Cantidad = 1000,
                                Moneda_Id = 1,
                                Moneda = new TablaSap{ Codigo = "ARP", Id = 1},
                                Precio = 10000,
                                UnidadDeMedida_Id = 1,
                                UnidadDeMedida = new TablaSap{ Codigo = "ARP", Id = 1},
                                CotizacionPosicion_Id = 1,
                                SolpSubPosicion_Id = 1,

                            }
                        }
                    }
                }
            };

            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = new Solp
                {
                    TrabajoYaHecho = true,
                    Pliego = new Pliego
                    {
                        FechaHoraEntrega = DateTime.Now
                    }
                }
            };
            var peticionDeOferta = new GuardarPeticionDeOfertaDto
            {
                Id = 1,
                Adjuntos = null,
                Observacion = "",
                PosIds = new List<int> { 1 },
                SolpId = 1,
                UsuarioIds = new List<int> { 1 },
                UsuarioActual = new UsuarioDto
                {
                    Id = 1
                },

            };

            var peticionDeOfertaUsuario = new PeticionDeOfertaUsuario
            {
                Id = 1,
                PeticionDeOferta = new PeticionDeOferta
                {
                    Id = 1,
                    Usuario = new Usuario
                    {
                        Mail = "bmelgarejo@prueba.com",
                    },
                    Solp = new Solp
                    {
                        UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com", CUITRegistro = "338383", },
                        NroSolp = "3344534",
                    },
                    Posiciones = new List<PeticionDeOfertaSolpPosicion>
                {
                    new PeticionDeOfertaSolpPosicion
                    {
                        PeticionDeOferta_Id = 1,
                        SolpPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion = new SolpPosicion
                        {
                            Unidad_Id = 1,
                            Moneda_Id = 1,
                            Cantidad = 1000,
                            PrecioBruto = 1000,
                            Subposiciones = new List<SolpSubposicion>
                            {

                            }
                        }
                    }
                },
                    Usuarios = new List<PeticionDeOfertaUsuario>
                {
                    new PeticionDeOfertaUsuario
                    {
                        Id = 1
                    }
                }
                }
            };

            var solp = new Solp
            {
                Id = 1,
                ProveedorAsignado_Id = 1,
                UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com" },
                UsuarioCreacion_Id = 1,
                Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral
                                    {
                                        Codigo = "MATERIALES"
                                    },
                                }
                            }
            };

            var peticionEntidad = new PeticionDeOferta
            {
                Usuario = new Usuario
                {
                    Mail = "bmelgarejo@prueba.com"
                },
                Solp = new Solp
                {
                    UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com", },
                    NroSolp = "3344534",
                },
                Posiciones = new List<PeticionDeOfertaSolpPosicion>
                {
                    new PeticionDeOfertaSolpPosicion
                    {
                        PeticionDeOferta_Id = 1,
                        SolpPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion = new SolpPosicion
                        {
                            Unidad_Id = 1,
                            Moneda_Id = 1,
                            Cantidad = 1000,
                            PrecioBruto = 1000,
                            Subposiciones = new List<SolpSubposicion>
                            {

                            }
                        }
                    }
                },
                Usuarios = new List<PeticionDeOfertaUsuario>
                {
                    new PeticionDeOfertaUsuario
                    {
                        Id = 1
                    }
                }
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(cotizacionEntidad);
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1, Mail = "bmelgarejo", CUITRegistro = "2373739293", TipoUsuario = new TipoUsuario { Id = 1 }, Proveedores = new List<Proveedor> { new Proveedor { CUIT = "2373739293", TipoProveedor = new TipoUsuario { Id = 1 } } } });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(peticionDeOfertaUsuario);

            repositorioMock.Setup(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>()))
            .Returns(peticionEntidad);

            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
             .Returns(cotizacionEntidad);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            target.CrearCotizacionConTrabajoYaHecho(solp, false);

            repositorioMock.Verify(y => y.Obtener<Usuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(4));
        }

        [Test]
        public void EditarSolpOk()
        {
            var solp = new Solp
            {
                Id = 1,
                ProveedorAsignado_Id = 1,
                UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com" },
                UsuarioCreacion_Id = 1,
                Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral
                                    {
                                        Codigo = "MATERIALES"
                                    },
                                    Codigo = "3323"
                                }
                            },
                Pliego = new Pliego
                {
                    RevisadoPor = "Tonio"
                },

            };
            var solpDto = new SolpDto
            {
                Id = 1,
                TipoSolp = new TablaGeneralDto
                {
                    Codigo = "23234"
                },
                ClaseDocumento = new TablaSapDto
                {
                    Codigo = "23234"
                },
                UsuarioActual = new UsuarioDto
                {
                    Id = 1
                },
                RevisadoPor = "Tonio",
                PasoCompletado = 1,
                UsuarioCompras = new UsuarioComprasDto
                {
                    Id = 1
                },
                EstadoPasos = "34",
                TipoSolpSap = 1,
                NombreDeObra = "obra",
                TrabajoYaHecho = false,
                FiscalContrato = "fiscal",
                Telefono = "3332323",
                Email = "bmelgarejo@test.com",
                FechaHoraEntrega = DateTime.Now,
                TieneVisitaObra = true,
                TieneVisitaObraMasiva = false,
                TieneObradores = false,
                TieneMedioElevacion = false,
                TieneAndamio = false,
                TieneGrillaPersonal = false,
                TieneFabricacionTallerExterno = false,
                TieneTecnicoSeguridad = false,
                TieneDescripcionTecnica = false,
                TieneDocumentacionTecnica = false,
                FechaHoraLimiteConsulta = DateTime.Now,
                ObservacionesGeneracion = "",
                DiasEjecucion = 1,
                ObservacionesCotizacion = "",
                TieneCondicionesGenerales = false,
                Posiciones = new List<SolpPosicionDto>
                {
                    new SolpPosicionDto
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Codigo = "123",
                    }
                }
            };


            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            file1.Setup(d => d.InputStream).Returns(fileStream);
            file1.Setup(d => d.ContentLength).Returns(Convert.ToInt32(fileStream.Length));

            var adjuntosMock = new Mock<HttpFileCollectionBase>();
            adjuntosMock.Setup(x => x.GetMultiple(It.IsAny<string>())).Returns(new List<HttpPostedFileBase> { file1.Object });

            repositorioMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>()))
               .Returns(solp);

            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });

            target.GuardarSolp(solpDto, adjuntosMock.Object);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(4));

        }

        [Test]
        public void FinalizarSolpOk()
        {
            var solp = new Solp
            {
                Id = 1,
                ProveedorAsignado_Id = 1,
                UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com" },
                UsuarioCreacion_Id = 1,
                Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral
                                    {
                                        Codigo = "MATERIALES"
                                    },
                                    Codigo = "3323"
                                }
                            },
                Pliego = new Pliego
                {
                    RevisadoPor = "Tonio"
                },

            };
            var solpDto = new SolpDto
            {
                TipoSolp = new TablaGeneralDto
                {
                    Codigo = "23234"
                },
                ClaseDocumento = new TablaSapDto
                {
                    Codigo = "23234"
                },
                UsuarioActual = new UsuarioDto
                {
                    Id = 1
                },
                RevisadoPor = "Tonio",
                PasoCompletado = 1,
                UsuarioCompras = new UsuarioComprasDto
                {
                    Id = 1
                },
                Finalizar = true,
                EstadoPasos = "34",
                TipoSolpSap = 1,
                NombreDeObra = "obra",
                TrabajoYaHecho = false,
                FiscalContrato = "fiscal",
                Telefono = "3332323",
                Email = "bmelgarejo@test.com",
                FechaHoraEntrega = DateTime.Now,
                TieneVisitaObra = true,
                TieneVisitaObraMasiva = false,
                TieneObradores = false,
                TieneMedioElevacion = false,
                TieneAndamio = false,
                TieneGrillaPersonal = false,
                TieneFabricacionTallerExterno = false,
                TieneTecnicoSeguridad = false,
                TieneDescripcionTecnica = false,
                TieneDocumentacionTecnica = false,
                FechaHoraLimiteConsulta = DateTime.Now,
                ObservacionesGeneracion = "",
                DiasEjecucion = 1,
                ObservacionesCotizacion = "",
                TieneCondicionesGenerales = false,
                Posiciones = new List<SolpPosicionDto>
                {
                    new SolpPosicionDto
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Codigo = "123",
                    }
                }
            };


            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            file1.Setup(d => d.InputStream).Returns(fileStream);
            file1.Setup(d => d.ContentLength).Returns(Convert.ToInt32(fileStream.Length));

            var adjuntosMock = new Mock<HttpFileCollectionBase>();
            adjuntosMock.Setup(x => x.GetMultiple(It.IsAny<string>())).Returns(new List<HttpPostedFileBase> { file1.Object });

            repositorioMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>()))
               .Returns(solp);

            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(new SolpSAPDto { })).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK" });


            target.GuardarSolp(solpDto, adjuntosMock.Object);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Solp>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));

        }
        [Test]
        public void ObtenerPrecioTotalPosicionProveedorOk()
        {
            // Instancia de GuardarCotizacionPosicionDto
            var cotizacionPosicion = new GuardarCotizacionPosicionDto
            {
                PeticionDeOfertaSolpPosicionId = 1,
                Precio = 10.99m,
                MonedaId = 1,
                UnidadDeMedidaId = 1,
                Cantidad = 5,
                FechaDeEntrega = DateTime.Now.AddDays(7),
                PrecioTotal = 54.95m,
                TotalPesos = 120.50m,
                NoDisponible = false
            };

            // Instancia de ArchivoDto
            var archivo = new ArchivoDto
            {
                Id = 1,
                FileKey = "abc123",
                Nombre = "archivo.pdf",
                Ruta = "/archivos/"
            };

            // Instancia de CotizacionHorasDto
            var cotizacionHoras = new CotizacionHorasDto
            {
                Id = 1,
                Cotizacion_Id = 1,
                Categoria = "Programación",
                CantidadPersonas = 3,
                HorasNormales = 40,
                HorasNocturnas = 10,
                HorasExtras = 5,
                Gremio = "Informática"
            };

            // Instancia de CotizacionSubposicionesDto
            var cotizacionSubposicion = new CotizacionSubposicionesDto
            {
                CotizacionSubPosicionId = 1,
                Precio = 20.50m,
                MonedaId = 1,
                CotizacionPosicionId = 1,
                UnidadDeMedidaId = 1,
                Cantidad = 10,
                SolpSubPosicionId = 1,
                PrecioTotal = 205.00m
            };

            // Instancia de GuardarCotizacion
            var guardarCotizacion = new GuardarCotizacion
            {
                PeticionOfertaUsuarioId = 1,
                CotizacionPosiciones = new List<GuardarCotizacionPosicionDto> { cotizacionPosicion },
                ObservacionTecnica = "Observación técnica",
                ObservacionEconomica = "Observación económica",
                ArchivosNuevos = new List<ArchivoDto> { archivo },
                ArchivosGuardados = new List<ArchivoDto> { archivo },
                CotizacionId = 1,
                EsFinalizado = true,
                RespetaMateriales = true,
                RespetaServicios = true,
                CotizacionesHoras = new List<CotizacionHorasDto> { cotizacionHoras },
                MonedaId = 1,
                UnidadDeMedidaId = 1,
                Cantidad = 5,
                FechaDeEntrega = DateTime.Now.AddDays(14),
                CotizacionSubposiciones = new List<CotizacionSubposicionesDto> { cotizacionSubposicion }
            };

            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
              .Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
          .Returns(new TablaSap { Id = 1, Codigo = "23234" });
            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse
                {
                    MonedaDestino = "ARP",
                    MonedaOrigen = "USD",
                    TipoCambio = 450
                });

            target.ObtenerPrecioTotalPosicionProveedor(guardarCotizacion);
            repositorioMock.Verify(y => y.Obtener<TablaSap>(It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void CrearOrdenDeCompraConRegistroInfoOk()
        {
            var registroInfo = new List<RegistroInfoDto>() { new RegistroInfoDto { ProveedorId = 1, PosicionId = 1, CantidadAdjudicacion = 5, Moneda = "ARP" } };
            var adjudicacionPosiciones = new List<AdjudicacionPosicionDto> { new AdjudicacionPosicionDto { Adjudicacion_Id = 1 } };
            var peticion = new PeticionDeOferta { Id = 1 };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpPosicion>() { new SolpPosicion { Id = 1, Solp = new Solp {
                    TrabajoYaHecho = true,
                    Posiciones = new List<SolpPosicion> { new SolpPosicion { Id = 1} },
                    UsuarioCreacion = new Usuario { Id = 1 },
                    UsuarioCreacion_Id = 1
                } } });
            repositorioMock.Setup(x => x.Listar<Usuario>(null, 0, null, DirOrden.Asc, null)).Returns(new List<Usuario> { new Usuario { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(new PeticionDeOferta
            {
                Id = 1,
                Usuarios = new List<PeticionDeOfertaUsuario> { new PeticionDeOfertaUsuario { Id = 1 } },
                Posiciones = new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 1 } }
            });
            repositorioMock.Setup(x => x.Obtener<Cotizacion>(It.IsAny<int>())).Returns(new Cotizacion
            {
                PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                {
                    PeticionDeOferta = new PeticionDeOferta
                    {
                        Solp_Id = 1,
                        Solp = new Solp
                        {
                            Id = 1,
                            UsuarioCreacion = new Usuario { Id = 1, Mail = "drodriguez@prueba.com" },
                            Posiciones = new List<SolpPosicion> { new SolpPosicion { Id = 1, TipoPosicion = new TablaGeneral { Codigo = "MATERIALES" } } }
                        }
                    },
                    Usuario = new Usuario { Id = 1, Mail = "drodriguez@prueba.com" }
                },
                CotizacionPosiciones = new List<CotizacionPosicion> { new CotizacionPosicion {
                    Id = 1,
                    PeticionDeOfertaSolpPosicion = new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 1 },
                    Moneda_Id = 1, Cantidad = 5, Precio = 5 }
                }
            });
            repositorioMock.Setup(x => x.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });
            repositorioMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(new PeticionDeOfertaUsuario());
            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario
            {
                Id = 1,
                Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } } }
            });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>())).Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });
            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });

            var result = target.CrearOrdenDeCompraConRegistroInfo(registroInfo, 1);
            var expected = new List<RespuestaCrearOrdenDeCompra> { new RespuestaCrearOrdenDeCompra {
                Errores = new List<string>(), IdEntidad = 0, Mensaje = "OK", NumeroPedido = "383383932"
            } };

            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Cotizacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(5));
            Assert.IsNotNull(result);
            Assert.That(result.First().Errores.Count == 0);
            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test]
        public void GrabarRevisionTecnicaOk()
        {
            var peticiones = new List<PeticionDeOfertaUsarioDto>
            {
                new PeticionDeOfertaUsarioDto
                 {
                   Id = 1
                 }
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaUsuario>() { new PeticionDeOfertaUsuario { Id = 1, RealizoVisita = true } });
            target.GrabarRevisionTecnica(peticiones, 1);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }
        [Test]
        public void TraerCotizacionOk()
        {
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<TraerCotizacionConsulta>())).Returns(new PeticionDeOfertaDto { CotizacionId = 1, Cotizacion = new CotizacionDto { ArchivosCotizacion = null } });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
           .Returns(new Cotizacion
           {
               Id = 1,
               Archivos = new List<Archivo> { new Archivo { Id = 1, FileKey = "", Ruta = "ruta" } },
               CotizacionesHoras = new List<CotizacionHora> { new CotizacionHora { Categoria = "Categoria", CantidadPersonas = 1, ConfigurarHora = false, Gremio = "Otros", HorasExtras = 1, HorasNormales = 1, Id = 1 },
            new CotizacionHora { Categoria = "Categoria", CantidadPersonas = 1, ConfigurarHora = false, Gremio = "UOCRA", HorasExtras = 1, HorasNormales = 1, Id = 1 }}
           });
            target.TraerCotizacion(It.IsAny<int>());
            repositorioMock.Verify(y => y.ObtenerConsultaEscalar(It.IsAny<TraerCotizacionConsulta>()), Times.Once);
        }

        [Test]
        public void ObtenerOrdenDeCompraConUsuarioOk()
        {
            obtenerOrdenDeCompraConsumerMOAMock.Setup(x => x.ObtenerOrdenDeCompra((It.IsAny<string>()))).Returns(new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    RazonSocialProveedor = "PARISI",
                    CUITProveedor = "2737237394",
                    CodigoProveedor = "003723739",
                    Usuario_Id = 1
                },
            });

            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario
            {
                CUITRegistro = "232323",
                TipoUsuario = new TipoUsuario { Id = 3 },
                Id = 1
            });
            target.ObtenerOrdenDeCompra(It.IsAny<string>());
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

        }

        [Test]
        public void ObtenerOrdenDeCompraSinUsuarioOk()
        {
            obtenerOrdenDeCompraConsumerMOAMock.Setup(x => x.ObtenerOrdenDeCompra((It.IsAny<string>()))).Returns(new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    RazonSocialProveedor = "PARISI",
                    CUITProveedor = "2737237394",
                    CodigoProveedor = "003723739",
                    Usuario_Id = 1
                },
            });

            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            target.ObtenerOrdenDeCompra(It.IsAny<string>());
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

        }


    }
}