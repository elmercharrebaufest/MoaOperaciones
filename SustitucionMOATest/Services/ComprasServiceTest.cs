// Ignore Spelling: Sustitucion Util

using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAUtils.Services.Email.Dto;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Web;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasServiceTest
    {
        private ComprasService target;
        private ComprasService target2;
        private ComprasSapService targetSap;
        private Mock<IObtenerCecoSolpConsumerMOA> cecoConsumerMock;
        private Mock<IObtenerCuentasSolpConsumerMOA> cuentasConsumerMock;
        private Mock<IObtenerOrdenSolpConsumerMOA> ordenesConsumerMock;
        private Mock<IObtenerServiciosSolpConsumerMOA> serviciosConsumerMock;
        private Mock<IRepositorioCompras> repositorioComprasMock;
        private Mock<IObtenerSolpConsumerMOA> obtenerSolpConsumerMOAMock;
        private Mock<ICrearSolpConsumerMOA> crearSolpConsumerMOAMock;
        private Mock<IModificarSolpConsumerMOA> modificarSolpConsumerMOAMock;
        private Mock<ICrearPedidoConsumerMOA> crearPedidoConsumerMOAMock;
        private Mock<IObtenerFuenteAprovisionamientoConsumerMOA> obtenerFuenteAprovisionamientoConsumerMOAMock;
        private Mock<IObtenerContratoSolpConsumerMOA> obtenerContratoSolpConsumerMOAMock;
        private Mock<IVendedorService> vendedorServiceMock;
        private Mock<IObtenerOrdenDeCompraConsumerMOA> obtenerOrdenDeCompraConsumerMOAMock;
        private Mock<IHttpContextService> httpContextServiceMock;
        private Mock<IUsuarioService> usuarioServiceMock;
        private Mock<IModificarOrdenDeCompraConsumerMOA> modificarOrdenDeCompraConsumerMOAMock;
        private Mock<IEmailService> emailServiceMock;
        private Mock<IReporteOrdenDeCompraConsumerMOA> reporteOrdenDeCompraConsumerMOAMock;
        private Mock<IObtenerPDFOrdenCompraConsumerMOA> obtenerPDFOrdenCompraConsumerMOAMock;
        private Mock<IListarSolpPendientesConsumerMOA> listarSolpPendientesConsumerMOAMock;
        private Mock<IObtenerAdjuntosSOLPEDConsumerMOA> obtenerAdjuntosSOLPEDConsumerMOAMock;
        private Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA> mIObtenerOrdenesDeCompraParaSOLPConsumerMOA;
        private Mock<IEmailComprasService> mIEmailComprasService;
        private Mock<IComprasArchivosService> mIComprasArchivosService;
        private Mock<IComprasArchivosImportService> mIComprasArchivosImportService;

        private Mock<ICentroDireccionService> centroDireccionServiceMock;
        private Mock<ITablaSapService> tablaSapServiceMock;
        private Mock<IUnidadMedidaService> unidadMedidaServiceMock;
        private Mock<ITipoCambioService> tipoCambioServiceMock;
        private Mock<IRegistroInfoService> registroInfoServiceMock;

        private Mock<IComprasSapService> mIComprasSapService;

        private GuardarCotizacion GuardarCotizacionToClone()
        {
            return new GuardarCotizacion
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
                ObservacionTecnica = "Observación técnica",
                ObservacionEconomica = "Observación económica",
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
                        UnidadDeMedidaId = 1,
                        CotizacionSubPosicionId = 1,
                        CotizacionPosicionId = 1,
                        SolpSubPosicionId = 1
                    }
                }
            };
        }
        private Cotizacion CotizacionToClone()
        {
            return new Cotizacion
            {
                Id = 1,
                CotizacionEstado_Id = 1,
                CotizacionesHoras = new List<CotizacionHora>()
            {
                new CotizacionHora
                {
                    Id = 1,
                    CantidadPersonas = 1,
                    Categoria = "Categoria",
                    ConfigurarHora = false,
                    Cotizacion_Id = 1,
                    Gremio = "Gremio",
                    HorasExtras = 1,
                    HorasNocturnas = 1,
                    HorasNormales = 1
                }
            },
                FechaCreacion = DateTime.Now,
                UsuarioCreador_Id = 1,
                CotizacionEstado = new CotizacionEstado
                {
                    Descripcion = "Cerrado",
                    Id = 1
                },
                Archivos = new List<Archivo> {

                new Archivo
                {
                    Id = 1,
                    FileKey = "FileKey",
                    Ruta = "Ruta"
                }
            },
                Revision = 1,
                UsuarioCreador = new Usuario
                {
                    Id = 1,
                    Mail = "bmelgarejo@prueba.com"
                },
                PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                {
                    PeticionDeOferta = new PeticionDeOferta
                    {
                        Usuario = new Usuario
                        {
                            Mail = "bmelgarejo@prueba.com"
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
                            FechaEntregaServicio = DateTime.Now,
                            TipoPosicion = new TablaGeneral { Codigo = "MATERIALES" },
                            TipoPosicion_Id = 1,
                            Subposiciones = new List<SolpSubposicion>
                            {

                            },
                            Solp = new Solp
                            {
                                 Id = 1,
                            ProveedorAsignado_Id = 1,
                            UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com" },
                            UsuarioCompras = new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
                            UsuarioCreacion_Id = 1,
                            UsuarioCompras_Id = 1,
                            FechaCreacion = new DateTime(),
                            TipoSolpSap = 1,
                            ClaseDocumento = new TablaSap{ CodigoSap = "ZP1"},
                            Pliego_Id = 1,
                              Pliego = new Pliego
                            {
                                RevisadoPor = "Tonio",
                                NombreObra = "NombreObra",
                                FiscalContrato = "Fiscal",
                                Email = "email@email.com",
                                Telefono = "5555",
                                FechaHoraEntrega = new DateTime(),
                                SupervisorSector = "SupervisorSelec",
                                SupervisorTrabajo = "SupervisorTrabajo",
                                VisitasMasivas = new List<PliegoVisita>(),
                                FechaHoraLimiteConsulta = new DateTime(),
                                ObservacionesGeneracion = "",
                                ObservacionesCotizacion = "",
                                Archivos = new List<Archivo> { new Archivo { FileKey = "fileKey" } },
                            },
                            Posiciones = new List<SolpPosicion>
                 {
                     new SolpPosicion
                     {
                         Id = 1,
                         Indice = 1,
                         TipoPosicion = new TablaGeneral { Codigo = "MATERIALES" },
                         Codigo = "3323",
                         GrupoCompras = new TablaSap { CodigoSap = "300" },
                         Solicitante = "Solicitante",
                         Tarea = "Tarea",
                         Centro = new TablaSap { CodigoSap = "1029" },
                         NroNecesidad = "NroNec",
                         GrupoArticulo = new TablaSap { CodigoSap = "100" },
                         Cantidad = 1,
                         Unidad = new TablaSap { CodigoSap = "200" },
                         PrecioBruto = 1500,
                         ProveedorFijo = "ProvFijo",
                         OrganizacionCompras = "OrgCompras",
                         NumeroContratoSuperior = "",
                         NumeroPosicionContratoSuperior = "NroPosicionContratoSup",
                         Moneda = new TablaSap { CodigoSap = "ARP" },
                         PlazoEntrega = 5,
                         CuentaMayorSap = new TablaSap { CodigoSap = "C" },
                         TipoImputacion = new TablaGeneral { Codigo = "TI" },
                         TipoImputacionSap = new TablaSap { CodigoSap = "TIS" },
                         TextoSuministro = "Texto",
                         Almacen = new TablaSap { CodigoSap = "Alm" },
                         MaterialSolp = new MaterialSolp { CodigoSap = "50000" },
                         NombreEntrega = "NombreEntrega",
                         CpEntrega = "CP",
                         CalleEntrega = "Calle",
                         NumeroEntrega = "NroEntrega",
                         FechaEntregaServicio = DateTime.Now,
                         Subposiciones = new List<SolpSubposicion> { new SolpSubposicion {
                             Id = 1, Tarea = "Tarea", Cantidad = 2, PrecioBruto = 500, Unidad = new TablaSap { CodigoSap = "UNI" } } }
                     }
                 },
                            }
                        }
                    }
                },
                        Usuarios = new List<PeticionDeOfertaUsuario>
                {
                    new PeticionDeOfertaUsuario
                    {
                        Id = 1,

                        PeticionDeOferta = new PeticionDeOferta
                    {

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
                        Moneda = new TablaSap
                        {
                            Id = 1,
                            Descripcion = "ARP",
                            Codigo = "ARP"
                        },
                        UnidadDeMedida = new TablaSap
                        {
                            Id = 1,
                            Descripcion = "ARP"
                        },
                        Precio = 1000,
                        PeticionDeOfertaSolpPosicion = new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 1, SolpPosicion = new SolpPosicion{ FechaEntregaServicio = DateTime.Now} },
                        PeticionDeOfertaSolpPosicion_Id = 1,
                        CotizacionSubPosiciones = new List<CotizacionSubPosicion>
                        {
                            new CotizacionSubPosicion
                            {
                                Id = 1,
                                Cantidad = 1,
                                Moneda_Id = 1,
                                Moneda = new TablaSap{ Codigo = "ARP", Id = 1},
                                Precio = 1,
                                UnidadDeMedida_Id = 1,
                                UnidadDeMedida = new TablaSap{ Codigo = "ARP", Id = 1},
                                CotizacionPosicion_Id = 0,
                                SolpSubPosicion_Id = 1,

                            },
                             new CotizacionSubPosicion
                            {
                                 Id = 1,
                                Cantidad = 1,
                                Moneda_Id = 1,
                                Moneda = new TablaSap{ Codigo = "ARP", Id = 1},
                                Precio = 449999,
                                UnidadDeMedida_Id = 1,
                                UnidadDeMedida = new TablaSap{ Codigo = "ARP", Id = 1},
                                CotizacionPosicion_Id = 1,
                                SolpSubPosicion_Id = 1,
                            }
                        }
                    }
                }
            };
        }
        private Solp SolpToClone()
        {
            return new Solp
            {
                Id = 1,
                NroSolp = "123",
                ProveedorAsignado_Id = 1,
                UsuarioCreacion = new Usuario
                {
                    Id = 1,
                    Mail = "bmelgarejo@prueba.com",
                    TipoUsuario = new TipoUsuario
                    {
                        Id = 1,
                        Nombre = "",
                        NombreCorto = ""
                    },
                    Roles = new List<Rol> {
                    new Rol
                    {
                        Nombre = "COMPRADOR",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Permiso = "COMPRADOR" }}
                    }
                }
                },
                UsuarioCompras = new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
                UsuarioCreacion_Id = 1,
                UsuarioCompras_Id = 1,
                FechaCreacion = new DateTime(),
                TipoSolpSap = 1,
                Pliego_Id = 1,
                Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    Moneda_Id = 1,
                                    Peticiones = new List<PeticionDeOfertaSolpPosicion> {
                                        new PeticionDeOfertaSolpPosicion {
                                            PeticionDeOferta = new PeticionDeOferta {
                                                Usuarios = new List<PeticionDeOfertaUsuario>
                                                {
                                                    new PeticionDeOfertaUsuario
                                                    {
                                                        Id = 1,
                                                        Usuario = new Usuario { Id = 1, Mail = "drodriguez@prueba", Proveedores = new List<Proveedor> {
                                                            new Proveedor { Id = 11, CUIT = "20043159381", CodigoProveedor = "0004315938", TipoProveedor = new TipoUsuario { Id = 1 } } } },
                                                        PeticionDeOferta = new PeticionDeOferta
                                                        {
                                                        },
                                                        ChatExterno = new List<ChatExternoCompras> {
                                                            new ChatExternoCompras {
                                                                Id = 1,
                                                                PeticionDeOferta_Id = 1,
                                                                Usuario_Id = 5776,
                                                                Leido = true,
                                                                Mensaje = "Hola",

                                                                PeticionDeOfertaUsuario_Id = 1,
                                                                Usuario = new Usuario {
                                                                    Mail = "test@mail.com",
                                                                    Roles = new List<Rol> { new Rol { Codigo = "SOLP" } },
                                                                    Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } }
                                                                }

                }

                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                    } },
                                    TipoPosicion = new TablaGeneral { Codigo = "MATERIALES" },
                                    Codigo = "3323",
                                    GrupoCompras = new TablaSap { CodigoSap = "300" },
                                    Solicitante = "Solicitante",
                                    Tarea = "Tarea",
                                    Centro = new TablaSap { CodigoSap = "1029" },
                                    NroNecesidad = "NroNec",
                                    GrupoArticulo = new TablaSap { CodigoSap = "100" },
                                    Cantidad = 1,
                                    Unidad = new TablaSap { CodigoSap = "200" },
                                    Unidad_Id = 1,
                                    PrecioBruto = 1500,
                                    ProveedorFijo = "ProvFijo",
                                    OrganizacionCompras = "OrgCompras",
                                    NumeroContratoSuperior = "",
                                    NumeroPosicionContratoSuperior = "NroPosicionContratoSup",
                                    Moneda = new TablaSap { CodigoSap = "ARP" },
                                    PlazoEntrega = 5,
                                    CuentaMayorSap = new TablaSap { CodigoSap = "C" },
                                    TipoImputacion = new TablaGeneral { Codigo = "TI" },
                                    TipoImputacionSap = new TablaSap { CodigoSap = "TIS" },
                                    TextoSuministro = "Texto",
                                    Almacen = new TablaSap { CodigoSap = "Alm" },
                                    MaterialSolp = new MaterialSolp { CodigoSap = "50000" },
                                    NombreEntrega = "NombreEntrega",
                                    CpEntrega = "CP",
                                    CalleEntrega = "Calle",
                                    NumeroEntrega = "NroEntrega",
                                    FechaEntregaServicio = DateTime.Now,
                                    Indice = 1,
                                    Subposiciones = new List<SolpSubposicion> {
                                        new SolpSubposicion {
                                            Id = 1,
                                            Tarea = "Tarea",
                                            Cantidad = 2,
                                            PrecioBruto = 500,
                                            Unidad_Id = 1,
                                            Numero = 1,
                                            Unidad = new TablaSap { CodigoSap = "UNI" }
                                        }
                                    }
                                }
                            },
                Pliego = new Pliego
                {
                    RevisadoPor = "Tonio",
                    NombreObra = "NombreObra",
                    FiscalContrato = "Fiscal",
                    Email = "email@email.com",
                    Telefono = "5555",
                    FechaHoraEntrega = new DateTime(),
                    SupervisorSector = "SupervisorSelec",
                    SupervisorTrabajo = "SupervisorTrabajo",
                    VisitasMasivas = new List<PliegoVisita>(),
                    FechaHoraLimiteConsulta = new DateTime(),
                    ObservacionesGeneracion = "",
                    ObservacionesCotizacion = "",
                    Archivos = new List<Archivo>
                {
                    new Archivo { Id = 1, FileKey = FileKeys.AdjuntoSolp, Ruta = "ruta1" },
                    new Archivo { Id = 2, FileKey = FileKeys.AdjuntoCotizacionesSolp, Ruta = "ruta2" },
                    new Archivo { Id = 3, FileKey = FileKeys.AdjuntoCotizacionesSolpCondEsp, Ruta = "ruta3" }
                },
                    ObservacionesCotizacionCondEsp = "ObservacionesTest"
                },
                EstadoSolpSap_Id = 1,
                EstadoDocumento_Id = 1,
                EstadoDocumento = new TablaEstado(),
                EstadoPasos = "",
                LiberadoresSapSolp = new List<LiberadorSapSolp> { new LiberadorSapSolp { Id = 1, Solp_Id = 1, LiberadorSap_Id = 1 } },
                ClaseDocumento_Id = 1,
                TipoSolp = new TablaGeneral { Codigo = "CON_PLIEGO" },
                EstadoSolpSap = new TablaSap { CodigoSap = "05" },

            };
        }
        private SolpDto SolpDtoToClone()
        {
            return new SolpDto
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
                EstadoPasos = "34",
                TipoSolpSap = 1,
                NombreDeObra = "obra",
                TrabajoYaHecho = false,
                THProveedorDirecto = false,
                THAjustePolinomica = false,
                THServicioPermanente = false,
                FiscalContrato = "fiscal",
                Telefono = "3332323",
                Email = "bmelgarejo@test.com",
                FechaHoraEntrega = DateTime.Now,
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
                EditarCondicionesEspeciales = false,
                FechaCreacion = DateTime.Now,
                FechaLiberacionSap = DateTime.Now,
                Posiciones = new List<SolpPosicionDto>
                {
                    new SolpPosicionDto
                    {
                        Id = 1,
                        Cantidad = 1000,
                        Codigo = "123",
                        Unidad = new TablaSapDto { Codigo = "UNI", CodigoSap = "UNI" },
                        Indice = 1,
                    }
                },
                LiberadoresSapSolp = new List<LiberadorSapSolpDto> { new LiberadorSapSolpDto { Id = 1, Solp_Id = 1, LiberadorSap_Id = 1 } }
            };
        }
        private PeticionDeOferta PeticionDeOfertaToClone()
        {
            return new PeticionDeOferta
            {
                Id = 1,
                Usuario = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com", UsuarioSap = "UsuarioSAP" },
                PlazoDeOferta = DateTime.Now.AddDays(5),
                Posiciones = new List<PeticionDeOfertaSolpPosicion>
                {
                    new PeticionDeOfertaSolpPosicion
                    {
                        PeticionDeOferta = new PeticionDeOferta
                        {
                            Id = 1,
                            Agrupada = false
                        },
                        PeticionDeOferta_Id = 1,
                        SolpPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion = new SolpPosicion
                        {
                            Id = 1,
                            Unidad_Id = 1,
                            Moneda_Id = 1,
                            Cantidad = 1000,
                            PrecioBruto = 1000,
                            Subposiciones = new List<SolpSubposicion>{ },
                            TipoPosicion_Id = 1,
                            TipoPosicion = new TablaGeneral { Codigo = "MATERIALES" },
                            Estado = true,
                            EsConcluido = true,
                            NombreEntrega = "Nombre",
                            CalleEntrega = "Calle",
                            CpEntrega = "9999",
                            FechaEntregaServicio = DateTime.Now,
                            ProvinciaId = 1,
                            Centro = new TablaSap { CodigoSap = "Centro" },
                            Unidad = new TablaSap { Descripcion = "Unidad" },
                            Solp = new Solp
                            {
                                Id = 1,
                                NroSolp = "123",
                                ProveedorAsignado_Id = 1,
                                UsuarioCreacion = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com" },
                                UsuarioCompras = new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
                                UsuarioCreacion_Id = 1,
                                UsuarioCompras_Id = 1,
                                FechaCreacion = new DateTime(),
                                TipoSolpSap = 1,
                                Pliego = new Pliego
                                  {
                                      RevisadoPor = "Tonio",
                                      NombreObra = "NombreObra",
                                      FiscalContrato = "Fiscal",
                                      Email = "email@email.com",
                                      Telefono = "5555",
                                      FechaHoraEntrega = new DateTime(),
                                      SupervisorSector = "SupervisorSelec",
                                      SupervisorTrabajo = "SupervisorTrabajo",
                                      VisitasMasivas = new List<PliegoVisita>(),
                                      FechaHoraLimiteConsulta = new DateTime(),
                                      ObservacionesGeneracion = "",
                                      ObservacionesCotizacion = "",
                                      Archivos = new List<Archivo> { new Archivo { FileKey = "fileKey" } },

                                },
                                Posiciones = new List<SolpPosicion>
                            {
                                new SolpPosicion
                                {
                                    Id = 1,
                                    TipoPosicion = new TablaGeneral { Codigo = "MATERIALES" },
                                    Codigo = "3323",
                                    GrupoCompras = new TablaSap { CodigoSap = "300" },
                                    Solicitante = "Solicitante",
                                    Tarea = "Tarea",
                                    Centro = new TablaSap { CodigoSap = "1029" },
                                    NroNecesidad = "NroNec",
                                    GrupoArticulo = new TablaSap { CodigoSap = "100" },
                                    Cantidad = 1,
                                    Unidad = new TablaSap { CodigoSap = "200" },
                                    PrecioBruto = 1500,
                                    ProveedorFijo = "ProvFijo",
                                    OrganizacionCompras = "OrgCompras",
                                    NumeroContratoSuperior = "",
                                    NumeroPosicionContratoSuperior = "NroPosicionContratoSup",
                                    Moneda = new TablaSap { CodigoSap = "ARP" },
                                    PlazoEntrega = 5,
                                    CuentaMayorSap = new TablaSap { CodigoSap = "C" },
                                    TipoImputacion = new TablaGeneral { Codigo = "TI" },
                                    TipoImputacionSap = new TablaSap { CodigoSap = "TIS" },
                                    TextoSuministro = "Texto",
                                    Almacen = new TablaSap { CodigoSap = "Alm" },
                                    MaterialSolp = new MaterialSolp { CodigoSap = "50000" },
                                    NombreEntrega = "NombreEntrega",
                                    CpEntrega = "CP",
                                    CalleEntrega = "Calle",
                                    NumeroEntrega = "NroEntrega",
                                    FechaEntregaServicio = DateTime.Now,
                                    Subposiciones = new List<SolpSubposicion> { new SolpSubposicion {
                                        Id = 1, Tarea = "Tarea", Cantidad = 2, PrecioBruto = 500, Unidad = new TablaSap { CodigoSap = "UNI" } } }
                                },
                            },
                            }
                        }
                    }
                },
                Usuarios = new List<PeticionDeOfertaUsuario>
            {
                new PeticionDeOfertaUsuario
                {
                    Id = 1,
                    Usuario = new Usuario { Id = 1, Mail = "drodriguez@prueba", Proveedores = new List<Proveedor> {
                        new Proveedor { Id = 11, CUIT = "20043159381", CodigoProveedor = "0004315938", TipoProveedor = new TipoUsuario { Id = 1 } } } },
                    PeticionDeOferta = new PeticionDeOferta
                    {
                    },
                    Cotizaciones = new List<Cotizacion>
                    {
                        new Cotizacion
                        {
                            Id = 1,
                            CotizacionPosiciones = new List<CotizacionPosicion>
                            {
                                new CotizacionPosicion
                                {
                                    Id = 1,
                                    Cotizacion_Id = 1,
                                    Cantidad = 1000,
                                    Moneda_Id = 1,
                                    Precio = 1000,
                                    PeticionDeOfertaSolpPosicion = new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 1, SolpPosicion = new SolpPosicion{ FechaEntregaServicio = DateTime.Now} },
                                    PeticionDeOfertaSolpPosicion_Id = 1,
                                    CotizacionSubPosiciones = new List<CotizacionSubPosicion>
                                    {
                                        new CotizacionSubPosicion
                                        {
                                            Id = 1
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            },
                RevisionTecnica = new PeticionDeOfertaRevisionTecnica
                {
                    Id = 1,
                    Usuario_Id = 1,
                    Usuario = new Usuario { Id = 1, Mail = "bmelgarejo@prueba.com", CUITRegistro = "1212121211" },
                    Fecha = DateTime.Now,
                    RecotizacionEconomica = false,
                    ModificacionSolp = false,
                    ObservacionRecotizacion = "Observacion",
                    Finalizada = true
                },
                Agrupada = false
            };
        }
        private PeticionDeOfertaRevisionTecnicaDto PeticionDeOfertaRevisionTecnicaToClone()
        {
            return new PeticionDeOfertaRevisionTecnicaDto
            {
                Id = 1,
                Usuario_Id = 1,
                Fecha = DateTime.Now,
                RecotizacionEconomica = false,
                ModificacionSolp = false,
                ObservacionRecotizacion = "Observacion",
                Finalizada = true
            };
        }

        [SetUp]
        public void SetUp()
        {
            repositorioComprasMock = new Mock<IRepositorioCompras>();
            cecoConsumerMock = new Mock<IObtenerCecoSolpConsumerMOA>();
            cuentasConsumerMock = new Mock<IObtenerCuentasSolpConsumerMOA>();
            ordenesConsumerMock = new Mock<IObtenerOrdenSolpConsumerMOA>();
            serviciosConsumerMock = new Mock<IObtenerServiciosSolpConsumerMOA>();
            obtenerSolpConsumerMOAMock = new Mock<IObtenerSolpConsumerMOA>();
            crearSolpConsumerMOAMock = new Mock<ICrearSolpConsumerMOA>();
            modificarSolpConsumerMOAMock = new Mock<IModificarSolpConsumerMOA>();
            crearPedidoConsumerMOAMock = new Mock<ICrearPedidoConsumerMOA>();
            obtenerFuenteAprovisionamientoConsumerMOAMock = new Mock<IObtenerFuenteAprovisionamientoConsumerMOA>();
            obtenerContratoSolpConsumerMOAMock = new Mock<IObtenerContratoSolpConsumerMOA>();
            vendedorServiceMock = new Mock<IVendedorService>();
            httpContextServiceMock = new Mock<IHttpContextService>();
            modificarOrdenDeCompraConsumerMOAMock = new Mock<IModificarOrdenDeCompraConsumerMOA>();
            obtenerOrdenDeCompraConsumerMOAMock = new Mock<IObtenerOrdenDeCompraConsumerMOA>();
            usuarioServiceMock = new Mock<IUsuarioService>();
            emailServiceMock = new Mock<IEmailService>();
            reporteOrdenDeCompraConsumerMOAMock = new Mock<IReporteOrdenDeCompraConsumerMOA>();
            listarSolpPendientesConsumerMOAMock = new Mock<IListarSolpPendientesConsumerMOA>();
            obtenerPDFOrdenCompraConsumerMOAMock = new Mock<IObtenerPDFOrdenCompraConsumerMOA>();
            obtenerAdjuntosSOLPEDConsumerMOAMock = new Mock<IObtenerAdjuntosSOLPEDConsumerMOA>();
            mIObtenerOrdenesDeCompraParaSOLPConsumerMOA = new Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA>();
            mIEmailComprasService = new Mock<IEmailComprasService>();
            mIComprasArchivosService = new Mock<IComprasArchivosService>();
            mIComprasArchivosImportService = new Mock<IComprasArchivosImportService>();
            centroDireccionServiceMock = new Mock<ICentroDireccionService>();
            tablaSapServiceMock = new Mock<ITablaSapService>();
            unidadMedidaServiceMock = new Mock<IUnidadMedidaService>();
            tipoCambioServiceMock = new Mock<ITipoCambioService>();
            registroInfoServiceMock = new Mock<IRegistroInfoService>();

            mIComprasSapService = new Mock<IComprasSapService>();

            httpContextServiceMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png");

            targetSap = new ComprasSapService(
                cecoConsumerMock.Object,
                crearPedidoConsumerMOAMock.Object,
                crearSolpConsumerMOAMock.Object,
                modificarOrdenDeCompraConsumerMOAMock.Object,
                modificarSolpConsumerMOAMock.Object,
                cuentasConsumerMock.Object,
                ordenesConsumerMock.Object,
                serviciosConsumerMock.Object,
                obtenerOrdenDeCompraConsumerMOAMock.Object,
                obtenerSolpConsumerMOAMock.Object,
                obtenerFuenteAprovisionamientoConsumerMOAMock.Object,
                obtenerContratoSolpConsumerMOAMock.Object,
                listarSolpPendientesConsumerMOAMock.Object,
                reporteOrdenDeCompraConsumerMOAMock.Object,
                obtenerPDFOrdenCompraConsumerMOAMock.Object,
                obtenerAdjuntosSOLPEDConsumerMOAMock.Object,
                mIObtenerOrdenesDeCompraParaSOLPConsumerMOA.Object,
                centroDireccionServiceMock.Object,
                tablaSapServiceMock.Object,
                unidadMedidaServiceMock.Object,
                usuarioServiceMock.Object,
                tipoCambioServiceMock.Object
                );

            target = new ComprasService(
                repositorioComprasMock.Object,
                vendedorServiceMock.Object,
                httpContextServiceMock.Object,
                usuarioServiceMock.Object,
                emailServiceMock.Object,
                mIEmailComprasService.Object,
                mIComprasArchivosService.Object,
                mIComprasArchivosImportService.Object,
                targetSap,
                tipoCambioServiceMock.Object,
                unidadMedidaServiceMock.Object,
                registroInfoServiceMock.Object,
                tablaSapServiceMock.Object
                );

            target2 = new ComprasService(
                repositorioComprasMock.Object,
                vendedorServiceMock.Object,
                httpContextServiceMock.Object,
                usuarioServiceMock.Object,
                emailServiceMock.Object,
                mIEmailComprasService.Object,
                mIComprasArchivosService.Object,
                mIComprasArchivosImportService.Object,
                mIComprasSapService.Object,
                tipoCambioServiceMock.Object,
                unidadMedidaServiceMock.Object,
                registroInfoServiceMock.Object,
                tablaSapServiceMock.Object
                );
        }

        private void SetUpOCPeticionCotizacion()
        {
            var registroInfo = new List<RegistroInfoDto>() { new RegistroInfoDto { ProveedorId = 1, PosicionId = 1, CantidadAdjudicacion = 5, Moneda = "ARP" } };
            var adjudicacionPosiciones = new List<AdjudicacionPosicionDto> { new AdjudicacionPosicionDto { Adjudicacion_Id = 1, PlazoDeEntrega = DateTime.Now } };
            //var POConRegistroInfo = peticionDeOfertaToClone();
            //POConRegistroInfo.RegistroInfo = true;

            emailServiceMock.Setup(y => y.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(),
                It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, byte[]>>()));
            httpContextServiceMock.Setup(y => y.GetDirectory(It.IsAny<string>())).Returns(TestContext.CurrentContext.TestDirectory + "\\Templates\\Example.html");


            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                            .Returns(new List<SolpPosicion>() { new SolpPosicion { Id = 1, FechaEntregaServicio = DateTime.Now, Solp = SolpToClone() } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion {
                Id = 1, SolpPosicion = new SolpPosicion {  FechaEntregaServicio = DateTime.Now, TipoPosicion = new TablaGeneral { Codigo = "SERVICIO" } }
            } });

            repositorioComprasMock.Setup(x => x.Listar<Usuario>(null, 0, null, DirOrden.Asc, null)).Returns(new List<Usuario> { new Usuario { Id = 1, Mail = "drodriguez@prueba.com" } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(PeticionDeOfertaToClone());
            repositorioComprasMock.Setup(x => x.Obtener<Cotizacion>(It.IsAny<int>())).Returns(CotizacionToClone());
            repositorioComprasMock.Setup(x => x.Agregar(It.IsAny<PeticionDeOferta>())).Returns(PeticionDeOfertaToClone());
            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(new PeticionDeOfertaUsuario()
            {
                PeticionDeOferta = PeticionDeOfertaToClone()
            });
            repositorioComprasMock.Setup(x => x.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario
            {
                Id = 1,
                Mail = "drodriguez@prueba.com",
                Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } } }
            });
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>())).Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });

            tipoCambioServiceMock.Setup(y => y.ObtenerTipoCambio(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });
            tipoCambioServiceMock.Setup(y => y.ObtenerTipoCambio(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });

            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>(), It.IsAny<bool>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });
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

            var pathbase = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Solp, bool>>>()))
               .Returns(solp);

            var expected = $"Solp-xxx-pliego-{DateTime.Now.ToString("yyyyMMdd")}.zip";
            var result = target.GenerarZipPliego(solpMock.Id, pathbase);

            Assert.AreEqual(expected, result);
        }*/

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
                .Setup(x => x.request(string.Empty))
                .Returns(servicioWSMOAResponseTest);

            List<ServicioSolp> listadoServiciosSolp = new List<ServicioSolp>
            {
                new ServicioSolp { Id = 1, CodigoSap = 1, Descripcion = "descripcion" },
                new ServicioSolp { Id = 2, CodigoSap = 2, Descripcion = "descripcion2" },
                new ServicioSolp { Id = 3, CodigoSap = 4, Descripcion = "descripcion3" },
                new ServicioSolp { Id = 3, CodigoSap = 4, Descripcion = "descripcion3", Codigo = "003", TipoServicio = "serv3",
                    AmbitoServicio = "ser3", Edicion = 3, UnidadMedidaBase = "base3", SSCItem = "sc3"},
            };

            this.repositorioComprasMock
                .Setup(x => x.Listar<ServicioSolp>(null, 0, null, DirOrden.Asc, null))
                .Returns(listadoServiciosSolp);

            this.repositorioComprasMock
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

            this.serviciosConsumerMock.Verify(x => x.request(string.Empty), Times.Once);
            this.repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ObtenerRutaArchivo()
        {
            var rutaArchivo = "ruta/archivo";
            var archivoId = 1;

            repositorioComprasMock
              .Setup(y => y.Obtener(It.IsAny<Expression<Func<Archivo, bool>>>()))
              .Returns(new Archivo
              {
                  Id = archivoId,
                  FileKey = "12323",
                  Ruta = rutaArchivo
              });

            var result = target.ObtenerRutaArchivo(archivoId);

            repositorioComprasMock.Verify(x => x.Obtener(It.IsAny<Expression<Func<Archivo, bool>>>()), Times.Once);

            Assert.AreEqual(rutaArchivo, result);
        }

        [Test]
        public void GrabarAdjudicacionMaterialOk()
        {
            var solpLocal = SolpToClone();
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
                        PlazoDeEntrega = DateTime.Now,
                    }
                },
                Cotizacion_Id = 1,
                FechaCreacion = DateTime.Now,
                Moneda_Id = 1,
                UsuarioCreador_Id = 1,
                Solp_Id = 1,
                NroSolp = "929292",
                CondicionesDeEntrega = "Condiciones"
            };

            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = solpLocal
            };

            repositorioComprasMock
                .Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpPosicion>() { posicion });

            repositorioComprasMock
                .Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "bmelgarejo@prueba.com.ar" });

            repositorioComprasMock
                .Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
                .Returns(CotizacionToClone());

            repositorioComprasMock
                .Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            repositorioComprasMock
                .Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
                .Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });

            tipoCambioServiceMock
                .Setup(y => y.ObtenerTipoCambio(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse
                {
                    MonedaDestino = "ARP",
                    MonedaOrigen = "USD",
                    TipoCambio = 450
                });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<RegionSap>() { new RegionSap { CodigoSap = "ARP", Id = 1 } });
            obtenerOrdenDeCompraConsumerMOAMock.Setup(x => x.ObtenerOrdenDeCompra((It.IsAny<string>()))).Returns(new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    RazonSocialProveedor = "PARISI",
                    CUITProveedor = "2737237394",
                    CodigoProveedor = "003723739",
                    Usuario_Id = 1,
                    UsuarioComprasSAP = "A"
                },
            });
            usuarioServiceMock.Setup(x => x.ObtenerVendedorSap(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            repositorioComprasMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new Usuario
                {
                    Id = 1,
                    CUITRegistro = "232323",
                    TipoUsuario = new TipoUsuario { Id = 3 },
                    Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } },
                    Habilitado = true,
                    Mail = "bmelgarejo@prueba.com.ar"
                });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado, false, It.IsAny<string>())).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            usuarioServiceMock.Setup(x => x.ObtenerProveedorSap(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1 } });
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>(), It.IsAny<bool>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { CotizacionToClone() });
            modificarSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new ModificarSolpConsumerMOAResponse());

            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.AtLeastOnce());
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarAdjudicacionServicioOk()
        {
            var solpLocal = SolpToClone();
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
                        PlazoDeEntrega = DateTime.Now,
                    }
                },
                Cotizacion_Id = 1,
                FechaCreacion = DateTime.Now,
                Moneda_Id = 1,
                UsuarioCreador_Id = 1,
                Solp_Id = 1,
                CondicionesDeEntrega = "Condiciones"
            };
            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = solpLocal
            };

            repositorioComprasMock
                .Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpPosicion>() { posicion });

            repositorioComprasMock
                .Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "bmelgarejo@prueba.com.ar" });

            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
               .Returns(CotizacionToClone());
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
                .Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            tipoCambioServiceMock.Setup(y => y.ObtenerTipoCambio(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse
                {
                    MonedaDestino = "ARP",
                    MonedaOrigen = "USD",
                    TipoCambio = 450
                });
            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>(), It.IsAny<bool>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { CotizacionToClone() });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<RegionSap>() { new RegionSap { CodigoSap = "ARP", Id = 1 } });
            obtenerOrdenDeCompraConsumerMOAMock.Setup(x => x.ObtenerOrdenDeCompra((It.IsAny<string>()))).Returns(new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    RazonSocialProveedor = "PARISI",
                    CUITProveedor = "2737237394",
                    CodigoProveedor = "003723739",
                    Usuario_Id = 1,
                    UsuarioComprasSAP = "A"
                },
            });
            usuarioServiceMock.Setup(x => x.ObtenerVendedorSap(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            repositorioComprasMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new Usuario
                {
                    Id = 1,
                    CUITRegistro = "232323",
                    TipoUsuario = new TipoUsuario { Id = 3 },
                    Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } },
                    Habilitado = true,
                    Mail = "bmelgarejo@prueba.com.ar"
                });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado, false, It.IsAny<string>())).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            usuarioServiceMock.Setup(x => x.ObtenerProveedorSap(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1 } });
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });
            modificarSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new ModificarSolpConsumerMOAResponse());

            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.AtLeastOnce());
            Assert.That(result.Errores.Count == 0);
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
            var peticionDeOfertaLocal = new GuardarPeticionDeOfertaDto
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
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<TablaSap>() { new TablaSap { Id = 1, Codigo = "", CodigoSap = "", Descripcion = "" } });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });
            repositorioComprasMock.Setup(repo => repo.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now, Solp = new Solp { NroSolp = "1" } } });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
           .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });


            var result = target.GrabarPeticionDeOferta(peticionDeOfertaLocal, null, false, null);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarCotizacionOk()
        {
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Id = 1 });

            repositorioComprasMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(new PeticionDeOfertaUsuario
               {
                   Id = 1,
                   PeticionDeOferta = PeticionDeOfertaToClone()
               });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion {
                Id = 1, SolpPosicion = new SolpPosicion { TipoPosicion = new TablaGeneral { Codigo = "SERVICIO" } }
            } });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(new Cotizacion { Id = 1, CotizacionEstado_Id = 1 });

            var result = target.GrabarCotizacion(GuardarCotizacionToClone(), null, false, false);

            repositorioComprasMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioComprasMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<Cotizacion>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarCotizacionEditarOk()
        {
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Id = 1 });

            repositorioComprasMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(new PeticionDeOfertaUsuario
               {
                   Id = 1,
                   PeticionDeOferta = PeticionDeOfertaToClone()
               });
            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = SolpToClone()
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });
            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(CotizacionToClone());

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.GrabarCotizacion(GuardarCotizacionToClone(), null, false, false);

            repositorioComprasMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioComprasMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void CrearCotizacionAutomaticaOk()
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

            var peticionDeOfertaUsuario = new PeticionDeOfertaUsuario
            {
                Id = 1,
                PeticionDeOferta = PeticionDeOfertaToClone()
            };

            var subposicion = new SolpSubposicion
            {
                Id = 1,
                Tarea = "Tarea",
                Cantidad = 2,
                PrecioBruto = 500,
                Unidad = new TablaSap { CodigoSap = "UNI" }
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { subposicion });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion { Id = 1, SolpPosicion = new SolpPosicion { TipoPosicion = new TablaGeneral { Codigo = "SERVICIO" } } } });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(CotizacionToClone());
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1, Mail = "bmelgarejo", CUITRegistro = "2373739293", TipoUsuario = new TipoUsuario { Id = 1 }, Proveedores = new List<Proveedor> { new Proveedor { CUIT = "2373739293", TipoProveedor = new TipoUsuario { Id = 1 } } } });

            repositorioComprasMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(peticionDeOfertaUsuario);

            repositorioComprasMock.Setup(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(PeticionDeOfertaToClone());

            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(CotizacionToClone());

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            repositorioComprasMock.Setup(repo => repo.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now, Solp = new Solp { NroSolp = "1" } } });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
           .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });


            var result = target.CrearCotizacionConTrabajoYaHechoOPresupuestado(SolpToClone());

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(6));
        }

        [Test]
        public void EditarSolpOk()
        {
            var solpDtoLocal = SolpDtoToClone();
            solpDtoLocal.Id = 1;

            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));

            var adjuntosMock = new Mock<HttpFileCollectionBase>();
            adjuntosMock.Setup(x => x.GetMultiple(It.IsAny<string>())).Returns(new List<HttpPostedFileBase> { file1.Object });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOferta>() { PeticionDeOfertaToClone() });

            repositorioComprasMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(SolpToClone());

            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });

            target.GuardarSolp(solpDtoLocal, adjuntosMock.Object);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(6));
        }

        [Test]
        public void FinalizarSolpOk()
        {
            var solpDtoLocal = SolpDtoToClone();
            solpDtoLocal.Finalizar = true;
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));

            var adjuntosMock = new Mock<HttpFileCollectionBase>();
            adjuntosMock.Setup(x => x.GetMultiple(It.IsAny<string>())).Returns(new List<HttpPostedFileBase> { file1.Object });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
               .Returns(new List<PeticionDeOferta>() { PeticionDeOfertaToClone() });

            repositorioComprasMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(SolpToClone());
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>())).Returns(new TablaGeneral { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>())).Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            target.GuardarSolp(solpDtoLocal, adjuntosMock.Object);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<Solp>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.AtLeastOnce());
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
            var guardarCotizacionLocal = new GuardarCotizacion
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

            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
              .Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
          .Returns(new TablaSap { Id = 1, Codigo = "23234" });
            tipoCambioServiceMock.Setup(y => y.ObtenerTipoCambio(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(new ObtenerTipoCambioConsumerMOAResponse
                {
                    MonedaDestino = "ARP",
                    MonedaOrigen = "USD",
                    TipoCambio = 450
                });

            target.ObtenerPrecioTotalPosicionProveedor(guardarCotizacionLocal);
            repositorioComprasMock.Verify(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()), Times.Once);
        }

        [Test]
        public void CrearOrdenDeCompraConRegistroInfoOk()
        {
            var solpDtoLocal = SolpDtoToClone();
            solpDtoLocal.Finalizar = true;
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));

            repositorioComprasMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(SolpToClone());

            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>())).Returns(new TablaGeneral { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>())).Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });

            vendedorServiceMock.Setup(y => y.GetDatosFiscales(It.IsAny<string>(), It.IsAny<string>())).Returns(new VendedorDetalleWSMOAResponse
            {
                cabeceras = new List<Cabecera> { new Cabecera { cuit = "21373773772", descripcion = "descripcion", calleFiscal = "", cpFiscal = "", provFiscal = "", locaFiscal = "" } }
            });

            var registroInfo = new List<RegistroInfoDto>() { new RegistroInfoDto {
                ProveedorId = 1, PosicionId = 1, CantidadAdjudicacion = 5, Moneda = "ARP"
            } };
            SetUpOCPeticionCotizacion();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { CotizacionToClone() });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CentroDireccion, bool>>>()))
              .Returns(new CentroDireccion { CodigoSap = "29292", RegionSap = new RegionSap { Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<RegionSap>() { new RegionSap { CodigoSap = "ARP", Id = 1 } });
            obtenerOrdenDeCompraConsumerMOAMock.Setup(x => x.ObtenerOrdenDeCompra((It.IsAny<string>()))).Returns(new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    RazonSocialProveedor = "PARISI",
                    CUITProveedor = "2737237394",
                    CodigoProveedor = "003723739",
                    Usuario_Id = 1,
                    UsuarioComprasSAP = "A"
                },
            });
            usuarioServiceMock.Setup(x => x.ObtenerVendedorSap(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            repositorioComprasMock
                .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new Usuario
                {
                    Id = 1,
                    CUITRegistro = "232323",
                    TipoUsuario = new TipoUsuario { Id = 3 },
                    Proveedores = new List<Proveedor>() { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } },
                    Habilitado = true,
                    Mail = "bmelgarejo@prueba.com.ar"
                });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado, false, It.IsAny<string>())).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            usuarioServiceMock.Setup(x => x.ObtenerProveedorSap(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Desc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1, NumeroOrdenDeCompra = "1", Usuario = new Usuario { Mail = "mail@mail.com" } } });
            repositorioComprasMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<RegionSap>() { new RegionSap { CodigoSap = "ARP", Id = 1 } });
            repositorioComprasMock.Setup(repo => repo.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion {Id=1, Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now,
                Solp = new Solp {UsuarioCreacion_Id=1, NroSolp = "1", UsuarioCreacion = new Usuario{Id=1 },
                    Posiciones = new  List<SolpPosicion>{
                    new SolpPosicion {Id=1, Centro= new TablaSap{CodigoSap="1" }, Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now,
                Solp = new Solp { UsuarioCreacion_Id=1, NroSolp = "1", UsuarioCreacion = new Usuario{Id=1 } } }
                    } }} });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
            .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });


            var result = target.CrearOrdenDeCompraConRegistroInfo(registroInfo, 1);

            var expected = new List<RespuestaCrearOrdenDeCompra> { new RespuestaCrearOrdenDeCompra {
                Errores = new List<string>(), IdEntidad = 0, Mensaje = "OK", NumeroPedido = "383383932"
            } };

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<Cotizacion>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.AtLeastOnce());
            Assert.IsNotNull(result);
            Assert.That(result.First().Errores.Count == 0);
            Assert.AreEqual(expected.Count, result.Count);
        }

        [Test]
        public void GrabarRevisionTecnicaOk()
        {
            var peticiones = new List<PeticionDeOfertaUsuarioDto>
            {
                new PeticionDeOfertaUsuarioDto
                 {
                   Id = 1
                 }
            };

            var finalizar = false;

            var revision = new PeticionDeOfertaRevisionTecnicaDto
            {
                Id = 1
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaUsuario>() { new PeticionDeOfertaUsuario { Id = 1, RealizoVisita = true, PeticionDeOferta = PeticionDeOfertaToClone() } });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOfertaRevisionTecnica>())).Returns(new PeticionDeOfertaRevisionTecnica { Id = 1 });


            target.GrabarRevisionTecnica(peticiones, 1, finalizar, revision);
            repositorioComprasMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void GrabarRevisionTecnicaFinalizarTrueOk()
        {
            var peticiones = new List<PeticionDeOfertaUsuarioDto>
            {
                new PeticionDeOfertaUsuarioDto
                 {
                   Id = 1,
                   PlazoDeOferta = DateTime.Now.AddDays(-5)
                 }
            };

            var finalizar = true;

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaUsuario>() { new PeticionDeOfertaUsuario { Id = 1, RealizoVisita = true, PeticionDeOferta = PeticionDeOfertaToClone(), } });
            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOfertaRevisionTecnicaDto>())).Returns(PeticionDeOfertaRevisionTecnicaToClone());


            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<Expression<Func<PeticionDeOferta, PeticionDeOfertaDto>>>()))
                .Returns(new PeticionDeOfertaDto { Id = 1, Solp_Id = 1, RegistroInfo = false, UsuarioCreador_Id = 1 });

            target.GrabarRevisionTecnica(peticiones, 1, finalizar, PeticionDeOfertaRevisionTecnicaToClone());
            repositorioComprasMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void TraerCotizacionOk()
        {
            repositorioComprasMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<TraerCotizacionConsulta>())).Returns(new PeticionDeOfertaDto
            {
                CotizacionId = 1,
                Cotizacion = new CotizacionDto { ArchivosCotizacion = null },
                TipoPosicionCodigo = "MATERIALES",
                PeticionDeOfertaPosicion = new List<PeticionDeOfertaSolpPosicionDto> { new PeticionDeOfertaSolpPosicionDto { Posiciones = new SolpPosicionDto { Codigo = "000000000050224373", Cantidad = 232, NroSolp = "1", Indice = 1 } } }
            });
            var cotizacion = CotizacionToClone();
            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacion);
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" },
            new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" }};
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            tablaSapServiceMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(tablaSap);
            unidadMedidaServiceMock.Setup(y => y.ObtenerUnidadesDesdeServicioSap(It.IsAny<List<string>>())).Returns(new List<UnidadesDeMedida>
            { new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "UNI", Denominador = 1, Numerador = 1 }});
            repositorioComprasMock.Setup(repo => repo.Listar(
                It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now, Solp = new Solp { NroSolp = "1" } } });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
            .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });

            target.TraerCotizacion(It.IsAny<int>());
            repositorioComprasMock.Verify(y => y.ObtenerConsultaEscalar(It.IsAny<TraerCotizacionConsulta>()), Times.Once);
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
                    Usuario_Id = 1,
                    //UsuarioComprasSAP = "A"
                },
            });

            usuarioServiceMock.Setup(x => x.ObtenerProveedorSap(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            usuarioServiceMock.Setup(x => x.ObtenerVendedorSap(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario
            {
                Mail = "test",
                CUITRegistro = "232323",
                TipoUsuario = new TipoUsuario { Id = 3 },
                Id = 1,
                Proveedores = new List<Proveedor> { new Proveedor { Id = 1, CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } }
            });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), null)).Returns(new List<Adjudicacion> { new Adjudicacion { Usuario = new Usuario { Mail = "test2" } } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), null)).Returns(new List<UsuarioCompras> { new UsuarioCompras { Id = 1, Mail = "test2" } });

            target.ObtenerOrdenDeCompra("454565645");
            usuarioServiceMock.Verify(y => y.ObtenerYCrearProveedorCompras(It.IsAny<string>()), Times.Once());
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

            usuarioServiceMock.Setup(x => x.ObtenerProveedorSap(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            usuarioServiceMock.Setup(x => x.ObtenerVendedorSap(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion> { new Adjudicacion { Usuario = new Usuario { Mail = "" } } });

            target.ObtenerOrdenDeCompra(It.IsAny<string>());
            usuarioServiceMock.Verify(y => y.ObtenerYCrearProveedorCompras(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void ActualizarFechaLiberacionConTrabajoHechoOk()
        {
            var solpLocal = SolpToClone();
            solpLocal.TrabajoYaHecho = true;
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1 });
            SetUpOCPeticionCotizacion();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
              .Returns(new List<PeticionDeOferta>());
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
           .Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });
            repositorioComprasMock.Setup(repo => repo.Listar(
                It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now, Solp = new Solp { NroSolp = "1" } } });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
           .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });

            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(8));
        }

        [Test]
        public void ActualizarFechaLiberacionConProveedorAsignadoOk()
        {
            var solpLocal = SolpToClone();
            solpLocal.CondEspProveedorAsignado = true;
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1 });
            SetUpOCPeticionCotizacion();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
              .Returns(new List<PeticionDeOferta>());

            repositorioComprasMock.Setup(repo => repo.Listar(
    It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
.Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now, Solp = new Solp { NroSolp = "1" } } });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
         .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });


            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(4));
        }

        [Test]
        public void ActualizarFechaLiberacionConAdicionalOk()
        {
            var solpLocal = SolpToClone();
            solpLocal.Adicional = true;

            vendedorServiceMock.Setup(y => y.GetDatosFiscales(It.IsAny<string>(), It.IsAny<string>())).Returns(new VendedorDetalleWSMOAResponse
            {
                cabeceras = new List<Cabecera> { new Cabecera { cuit = "21373773772", descripcion = "descripcion", calleFiscal = "", cpFiscal = "", provFiscal = "", locaFiscal = "" } }
            });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1, CodigoSap = "Codigo" });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(new Localidad { ProvinciaId = 1 });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CentroDireccion, bool>>>())).Returns(new CentroDireccion { CodigoSap = "Codigo" });
            SetUpOCPeticionCotizacion();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOferta>());

            repositorioComprasMock.Setup(repo => repo.Listar(
                It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now, Solp = new Solp { NroSolp = "1" } } });
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
           .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });

            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(4));
        }

        [Test]
        public void ListarProveedorPOOk()
        {
            var listaPO = new ListaPaginada<PeticionDeOfertaDto>(new List<PeticionDeOfertaDto> { new PeticionDeOfertaDto { Id = 1, ItemsTotales = 7 } }, 1, 10, 5);

            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario { CUITRegistro = "30709142301", Roles = new List<Rol> { new Rol { Codigo = "COMPRADOR" } } });
            repositorioComprasMock.Setup(y => y.ListarConsultaPaginada(It.IsAny<ListarSolpPOConsulta>())).Returns(listaPO);
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOferta> { PeticionDeOfertaToClone() });

            var result = target.ListarPOProveedor(new Paginacion(), "nroSolp", "nroPo", "nombrePedido", "username", null, null, null, null);
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(listaPO.GetType(), result.GetType());
        }

        [Test]
        public void ActualizarFechaLiberacionOCOk()
        {
            var nroOc = "1212";
            var fechaLiberacion = DateTime.Now;

            var adjudicaciones = new List<Adjudicacion> {
                new Adjudicacion {
                    Id = 1,
                    NumeroOrdenDeCompra = nroOc,
                    FechaLiberacionSap = null,
                    Usuario = new Usuario { Mail = "comprador@mail.com" , Proveedores = new List<Proveedor>()},
                    Cotizacion = new Cotizacion
                    {
                        PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                        {
                            Usuario = new Usuario { Mail = "bmelgarejo@prueba.com" },
                            PeticionDeOferta = new PeticionDeOferta { UsuariosAdicionales = new List<PeticionDeOfertaUsuarioAdicional>() }
                        }
                    },
                }
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(adjudicaciones);

            emailServiceMock.Setup(y => y.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(),
               It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, byte[]>>()));

            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Configuracion, bool>>>())).Returns(new Configuracion { Code = "EnvioMailLiberacionOC", Value = "1" });

            target.ActualizarFechaLiberacionOC(nroOc, fechaLiberacion);

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void CerrarCotizacionOk()
        {
            var peticionCierre = new PeticionDeOfertaCierre
            {
                PeticionDeOferta_Id = 1,
                Usuario_Id = 1,
                Fecha = DateTime.Now,
                Observacion = "observacion"
            };


            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOfertaCierre>())).Returns(peticionCierre);

            var result = target.CerrarCotizacion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOfertaCierre>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void DescargarAdjuntosCotizacionOk()
        {
            var cotizacionlocal = CotizacionToClone();
            cotizacionlocal.Archivos = new List<Archivo>()
            {
                new Archivo
                {
                  Id = 1,
                  FileKey = "12323",
                  Ruta = "ruta/archivo"
                }
            };
            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionlocal);
            var result = target.DescargarAdjuntosCotizacion(It.IsAny<int>(), TestContext.CurrentContext.TestDirectory, true);
            repositorioComprasMock.Verify(y => y.Obtener<Cotizacion>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GrabarProveedoresEnPeticionDeOfertaOk()
        {
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
              DirOrden.Asc, null)).Returns(new List<Usuario>()
              {
                  new Usuario
                  {
                       Id = 1,
                       Mail = "bmelgarejo@prueba.com",
                       Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } } }
                  }
              });

            var listaIds = new List<int>() { 1, 3, 4 };


            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(PeticionDeOfertaToClone());
            emailServiceMock.Setup(y => y.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(),
               It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, byte[]>>()));
            httpContextServiceMock.Setup(y => y.GetDirectory(It.IsAny<string>())).Returns(TestContext.CurrentContext.TestDirectory + "\\Templates\\Example.html");
            repositorioComprasMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Configuracion, bool>>>()))
            .Returns(new Configuracion
            {
                Value = "Configuraciones"
            });
            repositorioComprasMock
           .Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()))
           .Returns(new Localidad
           {
               Nombre = "Localidad"
           });
            repositorioComprasMock
           .Setup(y => y.Obtener(It.IsAny<Expression<Func<CentroDireccion, bool>>>()))
           .Returns(new CentroDireccion
           {
               CodigoSap = "CentroDireccion"
           });

            vendedorServiceMock.Setup(y => y.GetDatosFiscales(It.IsAny<string>(), It.IsAny<string>())).Returns(new VendedorDetalleWSMOAResponse
            {
                cabeceras = new List<Cabecera> { new Cabecera { cuit = "21373773772", descripcion = "descripcion", calleFiscal = "", cpFiscal = "", provFiscal = "", locaFiscal = "" } }
            });
            var result = target.GrabarProveedoresEnPeticionDeOferta(listaIds, It.IsAny<int>());

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
            repositorioComprasMock.Verify(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ObtenerTablaSapOk()
        {
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" },
            new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" }};
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            tablaSapServiceMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(tablaSap);
            var result = targetSap.ObtenerTablaSap("EstadoSolpSap");

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test]
        public void ObtenerTablaGeneralOk()
        {
            var tablaGralDto = new List<TablaGeneralDto> { new TablaGeneralDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            var tablaGral = new List<TablaGeneral> { new TablaGeneral { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaGeneral, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaGral);
            var result = target.ObtenerTablaGeneral("tabla");

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaGralDto.GetType());
        }

        [Test]
        public void ObtenerTablaEstadoOk()
        {
            var tablaEstadoDto = new List<TablaEstadoDto> { new TablaEstadoDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            var tablaEstado = new List<TablaEstado> { new TablaEstado { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaEstado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaEstado);
            var result = target.ObtenerTablaEstado("tabla");

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaEstadoDto.GetType());
        }

        [Test]
        public void ObtenerCentrosDireccionOk()
        {
            var centroDireccionDto = new List<CentroDireccionDto> { new CentroDireccionDto { CodigoSap = "1029", Direccion = "Benielli 398", Numero = "408411", Cp = "2200", Pais = "AR" } };
            var centroDireccion = new List<CentroDireccion> { new CentroDireccion { CodigoSap = "1029", Direccion = "Benielli 398", Numero = "408411", Cp = "2200", Pais = "AR" } };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CentroDireccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(centroDireccion);
            var result = target.ObtenerCentrosDireccion();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), centroDireccionDto.GetType());
        }

        [Test]
        public void ObtenerDatosPorCodigosSapOk()
        {
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            var result = target.ObtenerDatosPorCodigosSap(tablaSapDto);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test]
        public void ListarProvinciaOk()
        {
            var listaPciaDto = new List<ProvinciaDto> { new ProvinciaDto { ProvinciaId = 1, Nombre = "Jujuy", Orden = 1 } };
            var listaPcia = new List<Provincia> { new Provincia { ProvinciaId = 1, Nombre = "Jujuy", Orden = 1 } };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
            .Returns(listaPcia);
            var result = target.ListarProvincia();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), listaPciaDto.GetType());
        }

        [Test]
        public void BorrarSolpOk()
        {
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(SolpToClone());
            var result = target.BorrarSolp(1);
            Assert.AreEqual(result, "Se borró correctamente.");
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void TraerSolpIdOk()
        {
            var solpLocalDto = SolpDtoToClone();
            var solpLocal = SolpToClone();
            solpLocal.EstadoSolpSap = new TablaSap { CodigoSap = "05" };
            solpLocal.UsuarioCompras = new UsuarioCompras();
            solpLocal.UsuarioCreacion = new Usuario
            {
                Id = 1,
                Mail = "bmelgarejo@prueba.com",
                TipoUsuario = new TipoUsuario
                {
                    Id = 1,
                    Nombre = "",
                    NombreCorto = ""
                },
                Roles = new List<Rol> {
                    new Rol
                    {
                        Nombre = "COMPRADOR",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Permiso = "COMPRADOR" }}
                    }
                },

            };
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<IEnumerable<Expression<Func<Solp, object>>>>(), It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Proveedores = new List<Proveedor>() });
            var result = target.TraerSolpId(1);

            Assert.AreEqual(result.GetType(), solpLocalDto.GetType());
        }

        [Test]
        public void ActualizarEstadoSolpBulkOk()
        {
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Solp> { new Solp { NroSolp = "00553322" } });
            obtenerSolpConsumerMOAMock.Setup(y => y.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>())).Returns(new ObtenerSolpSAPResponse
            {
                Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { EstadoSolpSap = "05" } }
            });
            repositorioComprasMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(SolpToClone());

            target.ActualizarEstadoSolpBulk();

            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ActualizarOfertasAlEditarSolpMaterialLiberadaOk()
        {
            var solpDtoLocal = SolpDtoToClone();
            solpDtoLocal.Id = 1;
            solpDtoLocal.Finalizar = true;
            solpDtoLocal.TrabajoYaHecho = true;
            var solpLocal = SolpToClone();
            solpLocal.EstadoSolpSap = new TablaSap { CodigoSap = "05" };
            solpLocal.NroSolp = "0212303203";
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));

            var adjuntosMock = new Mock<HttpFileCollectionBase>();
            adjuntosMock.Setup(x => x.GetMultiple(It.IsAny<string>())).Returns(new List<HttpPostedFileBase> { file1.Object });

            repositorioComprasMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaGeneral, bool>>>())).Returns(new TablaGeneral { Codigo = "23234" });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaEstado, bool>>>())).Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOferta>() { PeticionDeOfertaToClone() });
            modificarSolpConsumerMOAMock.Setup(y => y.Request(It.IsAny<SolpSAPDto>())).Returns(new ModificarSolpConsumerMOAResponse
            {
                Errores = new List<ModificarSolpConsumerMOAError>()
            });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>())).Returns(new PeticionDeOferta { Id = 1 });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>())).Returns(new PeticionDeOfertaUsuario { Id = 1 });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cotizacion, bool>>>())).Returns(new Cotizacion { Id = 1 });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<CotizacionPosicion> { new CotizacionPosicion { Id = 1, PeticionDeOfertaSolpPosicion = new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 1 } } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaSolpPosicion> { new PeticionDeOfertaSolpPosicion { Id = 1 } });

            target.GuardarSolp(solpDtoLocal, adjuntosMock.Object);
            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<CotizacionPosicion>()), Times.Never);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.AtLeastOnce());
        }

        [Test]
        public void ActualizarFechaLiberacionConUsuarioComprasYTipoServicioEnviaMail()
        {
            var solpLocal = SolpToClone();
            solpLocal.UsuarioCompras = new UsuarioCompras(); // Simula que hay un usuario de compras asignado
            solpLocal.SeEnvioMailLiberacion = false; // Asegura que el correo no se ha enviado previamente
            solpLocal.Posiciones = new List<SolpPosicion>
                                    {
                                        new SolpPosicion
                                        {
                                            Id = 1,
                                            TipoPosicion = new TablaGeneral
                                            {
                                                Codigo = "SERVICIO"
                                            },
                                            Peticiones = new List<PeticionDeOfertaSolpPosicion>()
                                        }
                                    };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOferta>() { PeticionDeOfertaToClone() });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal); // Usar solpLocal en lugar de solp
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1 });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
             .Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });
            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void ObtenerLegajoParaExternosOk()
        {
            LegajoExternoDto legajo = new LegajoExternoDto { ListaLegajos = new List<LegajoDto>() };
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Adjudicacion, bool>>>())).Returns(new Adjudicacion
            {
                Cotizacion = new Cotizacion
                {
                    PeticionDeOfertaUsuario = new PeticionDeOfertaUsuario
                    {
                        Id = 1,
                        Usuario = new Usuario
                        {
                            Id = 1,
                            TipoUsuario = new TipoUsuario
                            {
                                Id = 3,
                                Nombre = "Usuario",
                                NombreCorto = "Usuario"
                            },
                            Roles = new List<Rol>
                            {
                               new Rol
                               {
                                   Nombre = "COMPRADOR",
                                   PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Permiso = "COMPRADOR" }},
                               }
                            },
                        }
                    },
                    PeticionDeOfertaUsuario_Id = 1
                },

            });
            //para ObtenerLegajo():
            var po = PeticionDeOfertaToClone();
            po.Posiciones.ToList().ForEach(a => a.SolpPosicion.Solp.EstadoSolpSap = new TablaSap { CodigoSap = "5" });
            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(po);
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaCierre, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaCierre> { new PeticionDeOfertaCierre { Id = 1, Fecha = DateTime.Now, Observacion = "", Usuario_Id = 1, Usuario = new Usuario { Id = 1, Mail = "", CUITRegistro = "005522" } } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Circular, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Circular> { new Circular { Id = 1, Archivos = new Collection<Archivo> { new Archivo { Id = 1, Ruta = "Ruta" } }, FechaCreacion = DateTime.Now, Usuario = new Usuario { Id = 1, TipoUsuario = new TipoUsuario { Id = 1, Nombre = "", NombreCorto = "" } } } });

            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOfertaVisualizacionPrecio, bool>>>()))
                .Returns(new PeticionDeOfertaVisualizacionPrecio { Archivo = new Archivo { Id = 1, Ruta = "Ruta" }, FechaCreacion = DateTime.Now, PeticionDeOferta_Id = 1, UsuarioCreador_Id = 1, Observaciones = "Observacion", Usuario = new Usuario { Id = 1, TipoUsuario = new TipoUsuario { Id = 1, Nombre = "", NombreCorto = "" } } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaVisualizacionPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaVisualizacionPrecio> { new PeticionDeOfertaVisualizacionPrecio { Id = 1 } });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
               .Returns(new List<Adjudicacion> { });

            var result = target.ObtenerLegajoParaExternos(1, "token", "moaoperaciones@baufest.com"); //pasa por ObtenerLegajo() también
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.ListaLegajos.GetType(), legajo.ListaLegajos.GetType());
            Assert.AreEqual(result.GetType(), legajo.GetType());
        }

        [Test]
        public void ObtenerReporteOrdenDeCompra_FechaHastaNull_ResultadoCorrecto()
        {
            // Arrange
            string nroOC = "12345";
            string fechaDesde = "2023-01-01";
            string fechaHasta = null;
            string codigoProveedor = "PROV123";
            string usuarioActual = "";

            reporteOrdenDeCompraConsumerMOAMock.Setup(x => x.Request(nroOC, fechaDesde, codigoProveedor))
                .Returns(new List<OrdenDeCompraSAPDto>
                {
                new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        FechaCreacion = DateTime.Parse("2023-02-01")
                    }
                }
                });
            usuarioServiceMock.Setup(s => s.GetUsuario(It.IsAny<string>()))
                     .Returns(new UsuarioDto { Id = 1, Permisos = new List<string>() });

            var result = targetSap.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor, usuarioActual);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(DateTime.Parse("2023-02-01"), result[0].Cabecera.FechaCreacion);
        }
        [Test]
        public void GrabarPeticionDeOfertaVisualizacionPrecioOk()
        {
            var peticion = new PeticionDeOfertaVisualizacionPrecioDto
            {
                Observacion = "Observacion",
                UsuarioCreador_Id = 1,
                PeticionDeOferta_Id = 1,
                FechaCreacion = DateTime.Now,
            };
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));

            var adjuntosMock = new Mock<HttpFileCollectionBase>();
            adjuntosMock.Setup(x => x.GetMultiple(It.IsAny<string>())).Returns(new List<HttpPostedFileBase> { file1.Object });

            var result = target.GrabarPeticionDeOfertaVisualizacionPrecio(peticion, adjuntosMock.Object);
            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOfertaVisualizacionPrecio>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void AutocompleteServicioSolpOk()
        {
            // Arrange
            string valor = "Servicio 1 Descripción";

            var serviciosSimulados = new List<ServicioSolpDto>
            {
                new ServicioSolpDto { Descripcion = "Servicio 1 Descripción" },
            };

            repositorioComprasMock.Setup(x => x.Listar(
                It.IsAny<Expression<Func<ServicioSolp, ServicioSolpDto>>>(),
                It.IsAny<Expression<Func<ServicioSolp, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DirOrden>()))
                .Returns(serviciosSimulados);


            List<ServicioSolpDto> result = target.AutocompleteServicioSolp(valor);

            Assert.That(result, Is.Not.Null);

            foreach (var servicioDto in result)
            {
                Assert.That(serviciosSimulados.Any(s => s.Descripcion.Contains(servicioDto.Descripcion)), Is.True);
            }
        }

        [Test]
        public void AutocompleteCodigoServicioSolpOk()
        {
            // Arrange
            string valor = "123"; // Establece un valor de búsqueda

            // Crea una lista de ServicioSolp simulada que contiene elementos coincidentes y no coincidentes con el valor de búsqueda
            var serviciosSimulados = new List<ServicioSolpDto>
            {
                new ServicioSolpDto { Codigo = 123 },
                new ServicioSolpDto { Codigo = 12345 },
            };

            // Configura el mock del repositorio para devolver la lista simulada
            repositorioComprasMock.Setup(x => x.Listar(
                            It.IsAny<Expression<Func<ServicioSolp, ServicioSolpDto>>>(),
                            It.IsAny<Expression<Func<ServicioSolp, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>()))
                            .Returns(serviciosSimulados);
            // Act
            List<ServicioSolpDto> result = target.AutocompleteCodigoServicioSolp(valor);

            // Assert
            Assert.That(result, Is.Not.Null);

            // Verificar que la lista resultante contenga elementos códigos que contengan el valor de búsqueda
            foreach (var servicioDto in result)
            {
                Assert.That(serviciosSimulados.Any(s => s.Codigo.ToString().Contains(servicioDto.Codigo.ToString())), Is.True);
            }
        }

        [Test]
        public void ObtenerChatTest()
        {
            int solpId = 1;
            int usuarioActualId = 2;
            var solpLocal = SolpToClone();
            repositorioComprasMock.Setup(x => x.Obtener<Solp>(It.IsAny<int>())).Returns(solpLocal);
            repositorioComprasMock.Setup(x => x.Obtener<Usuario>(It.IsAny<int>())).Returns(
                new Usuario
                {
                    Mail = "test@mail.com",
                    Roles = new List<Rol> { new Rol { Codigo = "SOLP" } },
                    Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } } }

                });


            var result = target.ObtenerChat(solpId, usuarioActualId);

            Assert.IsNotNull(result);
            Assert.AreEqual(solpLocal.Id, result.ChatCompras.Solp_Id);
            Assert.AreEqual(solpLocal.FechaCreacion.ToString("dd-MM-yyyy HH-mm-ss"), result.ChatCompras.FechaCreacion);
            Assert.AreEqual(solpLocal.FechaCreacion, result.ChatCompras.FechaCreacionDate);
            Assert.AreEqual(usuarioActualId, result.ChatCompras.UsuarioActualId);

            repositorioComprasMock.Verify(x => x.Obtener<Solp>(solpId), Times.Once);
            repositorioComprasMock.Verify(x => x.Obtener<Usuario>(usuarioActualId), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void ObtenerChatProveedorTest()
        {
            int peticionDeOfertaUsuarioId = 1;
            int usuarioActualId = 2;

            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId)).Returns(new PeticionDeOfertaUsuario
            {
                PeticionDeOferta = new PeticionDeOferta
                {
                    Usuario = new Usuario
                    {
                        Mail = "bmelgarejo@prueba.com",
                        CUITRegistro = "20202020202",
                    },
                    Id = 1,
                    FechaCreacion = DateTime.Now,


                },
                Usuario = new Usuario
                {
                    Mail = "test@mail.com",
                    Roles = new List<Rol> { new Rol { Codigo = "SOLP" } },
                    Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } } },
                    CUITRegistro = "20202020202"
                },
                Id = 1
            });

            var result = target.ObtenerChatProveedor(peticionDeOfertaUsuarioId, usuarioActualId);

            Assert.IsNotNull(result);

            repositorioComprasMock.Verify(x => x.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void GrabarMensajeChatInternoTest()
        {
            var mensajeDto = new ChatInternoComprasDto
            {
                Mensaje = "Test Message",
                Solp_Id = 1,
                Usuario_Id = 2
            };

            var chatInternoCompras = new ChatInternoCompras
            {
                Id = 0,
                FechaEnvio = DateTime.Now,
                Leido = false,
                Mensaje = mensajeDto.Mensaje,
                Solp_Id = mensajeDto.Solp_Id,
                Usuario_Id = mensajeDto.Usuario_Id
            };

            repositorioComprasMock.Setup(x => x.Agregar(It.IsAny<ChatInternoCompras>())).Callback((ChatInternoCompras entity) =>
            {
                Assert.AreEqual(chatInternoCompras.Mensaje, entity.Mensaje);
                Assert.AreEqual(chatInternoCompras.Solp_Id, entity.Solp_Id);
                Assert.AreEqual(chatInternoCompras.Usuario_Id, entity.Usuario_Id);
            });

            var result = target.GrabarMensajeChatInterno(mensajeDto);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<ChatInternoCompras>()), Times.Once, "Agregar method should be called once.");
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once, "GuardarCambios method should be called once.");

            Assert.IsNotNull(result);
            Assert.AreEqual(chatInternoCompras.Id, result.IdEntidad);
        }

        [Test]
        public void GrabarMensajeChatExternoTest()
        {
            var mensajeDto = new ChatExternoComprasDto
            {
                Mensaje = "Test Message",
                PeticionDeOferta_Id = 1,
                Usuario_Id = 2
            };

            var chatExternoCompras = new ChatExternoCompras
            {
                Id = 0,
                FechaEnvio = DateTime.Now,
                Leido = false,
                Mensaje = mensajeDto.Mensaje,
                PeticionDeOferta_Id = mensajeDto.PeticionDeOferta_Id,
                Usuario_Id = mensajeDto.Usuario_Id,
                PeticionDeOfertaUsuario_Id = mensajeDto.PeticionDeOfertaUsuario_Id
            };

            repositorioComprasMock.Setup(x => x.Agregar(It.IsAny<ChatExternoCompras>())).Callback((ChatExternoCompras entity) =>
            {
                Assert.AreEqual(chatExternoCompras.Mensaje, entity.Mensaje);
                Assert.AreEqual(chatExternoCompras.PeticionDeOferta_Id, entity.PeticionDeOferta_Id);
                Assert.AreEqual(chatExternoCompras.Usuario_Id, entity.Usuario_Id);
            });

            var result = target.GrabarMensajeChatExterno(mensajeDto);

            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<ChatExternoCompras>()), Times.Once, "Agregar method should be called once.");
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once, "GuardarCambios method should be called once.");

            Assert.IsNotNull(result);
            Assert.AreEqual(chatExternoCompras.Id, result.IdEntidad);
        }

        [Test]
        public void ExportarChatInternoAtextoTest()
        {
            var solp_id = 1;
            var peticionDeOfertaUsuario_id = 1;



            var rutaArchivo = Path.Combine(Path.GetTempPath(), "ArchivosComprasTest", Guid.NewGuid().ToString());
            Directory.CreateDirectory(rutaArchivo);

            repositorioComprasMock.Setup(x => x.Obtener<Solp>(solp_id)).Returns(SolpToClone());
            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuario_id)).Returns(new PeticionDeOfertaUsuario
            {
                PeticionDeOferta = new PeticionDeOferta
                {
                    Usuario = new Usuario
                    {
                        Mail = "bmelgarejo@prueba.com",
                        CUITRegistro = "20202020202"
                    }
                }
            });


            var result = target.ExportarChatInternoAtexto(solp_id, rutaArchivo, peticionDeOfertaUsuario_id);

            repositorioComprasMock.Verify(x => x.Obtener<Solp>(solp_id), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(File.Exists(result));

        }

        [Test]
        public void ListarOfertasCompradorOk()
        {
            var usuario = new UsuarioDto { Permisos = new List<string> { "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS" } };
            var solpNro = "102002020";
            var solpPosicionId = 1025;
            var codigoProveedor = "002384238";

            var peticionDeOferta = new PeticionDeOfertaDto
            {
                CotizacionId = 1,
                Cotizacion = new CotizacionDto { ArchivosCotizacion = null, Adjudicaciones = new List<AdjudicacionDto>() },
                TipoPosicionCodigo = "MATERIALES",
                Usuarios = new List<PeticionDeOfertaUsuarioDto>
                {
                    new PeticionDeOfertaUsuarioDto
                    {
                        CodigoProveedor = codigoProveedor,
                        Cotizacion = new CotizacionDto
                        {
                            Adjudicaciones = new List<AdjudicacionDto>(),
                            CotizacionPosiciones = new List<CotizacionPosicionDto>
                            {
                                new CotizacionPosicionDto
                                {
                                    Id = 1,
                                    PeticionDeOfertaSolpPosicion_Id = 1,
                                    Cantidad = 2,
                                    Precio = 500,
                                    UnidadMedida = new TablaSapDto { Descripcion = "UNI" },
                                    TotalPesos = 1000
                                }
                            }
                        }
                    }
                },
                PeticionDeOfertaPosicion = new List<PeticionDeOfertaSolpPosicionDto>
                {
                    new PeticionDeOfertaSolpPosicionDto
                    {
                        Id = 1,
                        Posicion = new SolpPosicionDto
                        {
                            Indice=1,
                            NroSolp="1",
                            Unidad = new TablaSapDto { Descripcion = "PAR" },
                            CodigoMaterialSap = new MaterialSolpDto { Codigo = "000000000050224373" }
                        },
                        Posiciones = new SolpPosicionDto { Codigo = "000000000050224373", Indice = 1 },
                        SolpId = 345,
                        SolpPosicion_Id = solpPosicionId
                    }
                },
                NroSolp = solpNro,
                NrosSolp = new List<string> { solpNro },
                SolpDto = new SolpDto
                {
                    ObservacionesCotizacionLista = new List<NroSolpObservacionCondEspDto>
                    {
                        new NroSolpObservacionCondEspDto
                        {
                            NroSolp = solpNro,
                            ObservacionesCotizacionCondEsp = "Observacion"
                        }
                    }
                }
            };

            repositorioComprasMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<ComparadorOfertasConsulta>())).Returns(peticionDeOferta);

            var cotizacionLocal = CotizacionToClone();
            repositorioComprasMock
                .Setup(y => y.Listar(
                    It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>(),
                    null))
                .Returns(cotizacionLocal.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.ToList());

            repositorioComprasMock
                .Setup(y => y.Listar(
                    It.IsAny<Expression<Func<PeticionDeOfertaVisualizacionPrecio, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    DirOrden.Asc,
                    null))
                .Returns(new List<PeticionDeOfertaVisualizacionPrecio> { new PeticionDeOfertaVisualizacionPrecio { Id = 1 } });

            repositorioComprasMock
                .Setup(y => y.Obtener<TablaSap>(It.IsAny<int>()))
                .Returns(new TablaSap { Id = 1 });

            repositorioComprasMock
                .Setup(y => y.Listar(
                    It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<DirOrden>(),
                    null))
                .Returns(new List<Adjudicacion> { new Adjudicacion { Usuario = new Usuario { Mail = "test@gmail.com" } } });

            obtenerSolpConsumerMOAMock
                .Setup(y => y.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
                .Returns(new ObtenerSolpSAPResponse
                {
                    Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { EstadoSolpSap = "05", NumeroPosicion = "1" } }
                });

            unidadMedidaServiceMock
                .Setup(y => y.ObtenerUnidadesDesdeServicioSap(It.IsAny<List<string>>()))
                .Returns(new List<UnidadesDeMedida>
                {
                    new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "UNI", Denominador = 1, Numerador = 1 },
                    new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "PAR", Denominador = 2, Numerador = 1 }
                });

            obtenerSolpConsumerMOAMock
                .Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
                .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });

            var ordenesCompraSapResponse = new List<OrdenDeCompraSAPDto>(); ;
            mIObtenerOrdenesDeCompraParaSOLPConsumerMOA
                .Setup(cons => cons.Request(solpNro, solpPosicionId.ToString()))
                .Returns(ordenesCompraSapResponse);

            target.ListarOfertasComprador(It.IsAny<int>(), usuario);
            repositorioComprasMock.Verify(y => y.ObtenerConsultaEscalar(It.IsAny<ComparadorOfertasConsulta>()), Times.Once);
        }

        [Test]
        public void ValidarSolpTratadaTest()
        {
            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
           .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });

            target.ValidarSolpTratada("02929292");
            obtenerSolpConsumerMOAMock.Verify(y => y.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()), Times.Once);
        }

        [Test]
        public void ListarSolpCompradorOk()
        {
            var listaSolp = new ListaPaginada<SolpDto>(new List<SolpDto> { new SolpDto { Id = 1, ItemsTotales = 7 } }, 1, 10, 5);
            repositorioComprasMock.Setup(y => y.ListarConsultaPaginada(It.IsAny<ListarSolpConsulta>())).Returns(listaSolp);

            var result = target.ListarSolpComprador(1, new Paginacion(), "nroSolp", "", null, null, false, false, false, true, EstadoListarTratamientoSolp.Todas, false);
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(listaSolp.GetType(), result.GetType());
        }

        [Test]
        public void DevolverMonedaProveedorOk()
        {
            usuarioServiceMock.Setup(x => x.ObtenerProveedorSap(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI", CURRENCY = "ARP" });
            usuarioServiceMock.Setup(x => x.ObtenerVendedorSap(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            var result = target.DevolverMonedaProveedor(It.IsAny<string>());
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ListarLiberadorSapOk()
        {
            var liberadorSapDto = new List<LiberadorSapDto> { new LiberadorSapDto { NombreCompleto = "Nombre", Cargo = "Cargo", Habilitado = true, LiberadorSapTipo_Id = 1 } };
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<LiberadorSap, LiberadorSapDto>>>(), It.IsAny<Expression<Func<LiberadorSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<LiberadorSapDto>() { new LiberadorSapDto { NombreCompleto = "Nombre", Cargo = "Cargo", Habilitado = true, LiberadorSapTipo_Id = 1 } });

            var result = target.ListarLiberadorSap();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), liberadorSapDto.GetType());
        }

        [Test]
        public void ListarRegionesSap_DeberiaDevolverListaDeRegiones()
        {
            var regionesEsperadas = new List<RegionSap>
            {
                new RegionSap { CodigoPais = "AR", CodigoSap = "Region1" },
                new RegionSap { CodigoPais = "AR", CodigoSap = "Region2" }
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(regionesEsperadas);

            var resultado = target.ListarRegionesSap();

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado.GetType(), regionesEsperadas.GetType());
        }

        [Test]
        public void EnviarMailSolpCreadasReporteOk()
        {
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(), It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<SolpDto>() { SolpDtoToClone() });
            DateTime startDate = new DateTime(2023, 9, 1);
            DateTime endDate = new DateTime(2024, 9, 1);
            target.ObtenerDatosReporteSolp();
            repositorioComprasMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(),
                It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc), Times.AtLeastOnce);
        }

        [Test]
        public void EnviarReporteTrabajoYaHecho_Ok()
        {
            var ordenesDeCompraUltimaSemana = new List<OrdenCompraDto>
            {
                new OrdenCompraDto { Id = 450000001 },
                new OrdenCompraDto { Id = 450000002 }
            };

            var ordenCompra1 = new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = "450000001",
                    CreadoPor = "USRSAP1",
                    FechaCreacion = new DateTime(2025, 8, 26, 0, 0, 0, DateTimeKind.Local)
                },
                Posiciones = new List<OrdenDeCompraSAPPosicion> { new OrdenDeCompraSAPPosicion { NroSolp = "102002021" } }
            };

            var ordenCompra2 = new OrdenDeCompraSAPDto
            {
                Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = "450000002",
                    CreadoPor = "USRSAP2",
                    FechaCreacion = new DateTime(2025, 8, 27, 0, 0, 0, DateTimeKind.Local)
                },
                Posiciones = new List<OrdenDeCompraSAPPosicion> { new OrdenDeCompraSAPPosicion { NroSolp = "102002022" } }
            };

            var solpsTrabajosHechos = new List<TrabajoYaHechoReporte>
            {
                new TrabajoYaHechoReporte
                {
                    SolpNro = "102002021",
                    SolpCreador = "usuario1@moa.com",
                    SolpFecha = "15/08/2025",
                    SolpAprobador = "usuario1@moa.com",
                    SolpProveedor = "usuario1@moa.com",
                    SolpProveedorNombre = "RS prov1",
                    Posiciones = new List<DetallePosicion>
                    {
                        new DetallePosicion
                        {
                            NroPosicion = "1",
                            TextoPosicion = "texto 1"
                        },
                        new DetallePosicion
                        {
                            NroPosicion = "2",
                            TextoPosicion = "texto 2"
                        }
                    }
                },
                new TrabajoYaHechoReporte
                {
                    SolpNro = "102002022",
                    SolpCreador = "usuario2@moa.com",
                    SolpFecha = "25/08/2025",
                    SolpAprobador = "usuario1@moa.com",
                    SolpProveedor = "usuario1@moa.com",
                    SolpProveedorNombre = "RS prov2",
                    Posiciones = new List<DetallePosicion>
                    {
                        new DetallePosicion
                        {
                            NroPosicion = "1",
                            TextoPosicion = "texto 1"
                        },
                        new DetallePosicion
                        {
                            NroPosicion = "2",
                            TextoPosicion = "texto 2"
                        }
                    }
                }
            };

            var fechasLiberacionPorOc = new Dictionary<string, DateTime>
            {
                { "450000001", new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Local) },
                { "450000002", new DateTime(2025, 9, 2, 0, 0, 0, DateTimeKind.Local) }
            };

            var fechaDesde = DateTime.Today.AddDays(-7);

            mIComprasSapService
                .Setup(s => s.ObtenerOrdenesDeCompra(It.Is<DateTime>(x => x == fechaDesde)))
                .Returns(ordenesDeCompraUltimaSemana);

            mIComprasSapService
                .Setup(s => s.ObtenerOrdenDeCompra("450000001"))
                .Returns(ordenCompra1);

            mIComprasSapService
                .Setup(s => s.ObtenerOrdenDeCompra("450000002"))
                .Returns(ordenCompra2);

            repositorioComprasMock
                .Setup(s => s.ObtenerSolpsReporteTrabajoYaHecho(It.Is<ICollection<string>>(list => list.Contains("102002021") && list.Contains("102002022"))))
                .Returns(solpsTrabajosHechos);

            repositorioComprasMock
                .Setup(s => s.ObtenerFechasLiberacionOcs(It.Is<ICollection<string>>(list => list.Contains("450000001") && list.Contains("450000002"))))
                .Returns(fechasLiberacionPorOc);

            mIEmailComprasService
                .Setup(s => s.EnviarMailReporteTrabajoYaHecho(It.IsAny<byte[]>(), It.IsAny<string>()));

            target2.EnviarReporteTrabajoYaHecho();

            mIComprasSapService.Verify(s => s.ObtenerOrdenesDeCompra(It.Is<DateTime>(x => x == fechaDesde)), Times.Once);
            mIComprasSapService.Verify(s => s.ObtenerOrdenDeCompra("450000001"), Times.Once);
            mIComprasSapService.Verify(s => s.ObtenerOrdenDeCompra("450000002"), Times.Once);
            repositorioComprasMock.Verify(s => s.ObtenerSolpsReporteTrabajoYaHecho(It.Is<ICollection<string>>(list => list.Contains("102002021") && list.Contains("102002022"))), Times.Once);
            repositorioComprasMock.Verify(s => s.ObtenerFechasLiberacionOcs(It.Is<ICollection<string>>(list => list.Contains("450000001") && list.Contains("450000002"))), Times.Once);
            mIEmailComprasService.Verify(s => s.EnviarMailReporteTrabajoYaHecho(It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ListarClaseDocumentoOk()
        {
            List<int> clasesDoc = new List<int>();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Solp>() { SolpToClone() });
            var result = target.ListarClaseDocumento(1);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), clasesDoc.GetType());
        }

        [Test]
        public void ActualizarProveedorVisibleEnSolicitanteOk()
        {
            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(new PeticionDeOfertaUsuario { Id = 1, VisibleSolicitante = true });
            var result = target.ActualizarProveedorVisibleEnSolicitante(It.IsAny<int>(), It.IsAny<bool>());
            repositorioComprasMock.Setup(x => x.GuardarCambios());
            Assert.That(result, Is.Not.Null);
            repositorioComprasMock.Verify(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ListarHistorialDeFechasOk()
        {
            int peticionDeOfertaId = 1;
            var cotizacionHistorialList = new List<CotizacionHistorial>
                {
                    new CotizacionHistorial
                    {
                        Id = 1,
                        Cotizacion_Id = 100,
                        Log = "Log de prueba",
                        FechaFinalizacion = DateTime.Now.AddDays(-2),
                        Usuario_Id = 1,
                        Cotizacion = CotizacionToClone(),
                        Usuario = new Usuario
                        {
                            Id = 1,
                            Roles = new List<Rol>()
                            {
                                new Rol
                                {
                                    Nombre = "Solicitante",
                                }
                            }
                        }
                    },
                };

            repositorioComprasMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(PeticionDeOfertaToClone());
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionHistorial, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(cotizacionHistorialList);
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaCierre, DateTime>>>(),
               It.IsAny<Expression<Func<PeticionDeOfertaCierre, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<DateTime>() { DateTime.Now });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(),
                It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
               .Returns(new List<SolpDto>() { SolpDtoToClone() });

            var resultado = target.ListarHistorialDeFechas(peticionDeOfertaId);
            Assert.NotNull(resultado);
            repositorioComprasMock.Verify(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>()), Times.Once);

        }

        [Test]
        public void ListarPeticionesDeOfertaOk()
        {
            repositorioComprasMock.Setup(x => x.Obtener<Solp>(It.IsAny<int>())).Returns(SolpToClone());
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, PeticionDeOfertaDto>>>(),
                It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
                .Returns(new List<PeticionDeOfertaDto> { new PeticionDeOfertaDto {  CotizacionId = 1,
                Cotizacion = new CotizacionDto { ArchivosCotizacion = null },
                TipoPosicionCodigo = "MATERIALES",
                Usuarios = new List<PeticionDeOfertaUsuarioDto> { new PeticionDeOfertaUsuarioDto { Cotizacion = new CotizacionDto { CotizacionPosiciones = new List<CotizacionPosicionDto> { new CotizacionPosicionDto {
                Id = 1, PeticionDeOfertaSolpPosicion_Id = 1, Cantidad = 2, Precio = 500, UnidadMedida = new TablaSapDto { Descripcion = "UNI" }, TotalPesos = 1000 } } } } },
                PeticionDeOfertaPosicion = new List<PeticionDeOfertaSolpPosicionDto> {
                    new PeticionDeOfertaSolpPosicionDto { Id = 1, Posicion = new SolpPosicionDto { Unidad = new TablaSapDto { Descripcion = "PAR" }, CodigoMaterialSap = new MaterialSolpDto { Codigo = "000000000050224373" } }, Posiciones = new SolpPosicionDto { Codigo = "000000000050224373" } }
                },
                NrosSolp = new List<string> { "102002020"}} });

            var resultado = target.ListarPeticionesDeOferta(It.IsAny<int>());
            Assert.NotNull(resultado);
            repositorioComprasMock.Verify(x => x.Obtener<Solp>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void MarcarChatProveedorComoLeidoTest()
        {
            var chatProveedor = new ChatProveedoresDto
            {
                Mensajes = new List<ChatExternoComprasDto> {
                         new ChatExternoComprasDto {
                            Id = 1,
                            PeticionDeOferta_Id = 1,
                            Usuario_Id = 5776,
                            FechaEnvioDate = DateTime.Now,
                            Leido = true,
                            Mensaje = "Hola",
                            PeticionDeOfertaUsuario_Id = 1,
                            RolUsuario = "SOLP",
                            FechaEnvio = "12-04-2024",
                            Mail = "rorlando@baufest.com",
                            FechaDiaEnvio = "12-04-2024"
                        }
                    },
                PeticionDeOferta_Id = 1,
                FechaCreacion = "12-04-2024",
                FechaCreacionDate = DateTime.Now,
                UsuarioActualId = 5776,
                RazonSocialProveedor = "Aca",
                CuitProveedor = "20043159381",
                PeticionDeOfertaUsuario_Id = 1

            };

            var chatExterno = new ChatExternoCompras
            {
                Id = 1,
                PeticionDeOferta_Id = 1,
                Usuario_Id = 5776,
                Leido = true,
                Mensaje = "Hola",
                PeticionDeOfertaUsuario_Id = 1,
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ChatExternoCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<ChatExternoCompras>() { chatExterno });
            target.MarcarChatProveedorComoLeido(chatProveedor);

            this.repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Once);

        }


        [Test]
        public void ListarSolpCondicionEspecialTestOk()
        {
            var filtro = new FiltroDto
            {
                Columna = "NroSolp"
            };
            var solpsDto = new List<SolpDto>(new List<SolpDto> { SolpDtoToClone() });
            repositorioComprasMock.Setup(y => y.ListarConsulta(It.IsAny<ListarSolpCondicionEspecialConsulta>())).Returns(solpsDto);
            target.ListarSolpCondicionEspecial(filtro);
            repositorioComprasMock.Verify(y => y.ListarConsulta(It.IsAny<ListarSolpCondicionEspecialConsulta>()), Times.Once);
        }


        [Test]
        public void AgruparPeticionesDeOfertaOk()
        {

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
              .Returns(new List<PeticionDeOferta>() { PeticionDeOfertaToClone() });
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion>() { CotizacionToClone() });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionHistorial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<CotizacionHistorial>() { new CotizacionHistorial { Cotizacion_Id = 1 } });

            target.AgruparPeticionesDeOferta(It.IsAny<int>(), It.IsAny<string>());

            repositorioComprasMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioComprasMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
        }

        [Test]
        public void ValidarPrecioCotizadoOk()
        {
            var adjudicacionDto = new AdjudicacionDto
            {
                AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>
                {
                    new AdjudicacionPosicionDto
                    {
                        Adjudicacion_Id = 1,
                        Cantidad = 1,
                        CotizacionPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion_Id = 1,
                        PrecioTotal = 1,
                        MonedaId = 1,
                        MonedaCodigo = "ARP",
                        Moneda = new TablaSapDto
                        {
                            CodigoSap = "ARP"
                        },
                        PlazoDeEntrega = DateTime.Now,
                    }
                },
                Cotizacion_Id = 1,
                FechaCreacion = DateTime.Now,
                Moneda_Id = 1,
                UsuarioCreador_Id = 1,
                Solp_Id = 1,
                CondicionesDeEntrega = "Condiciones"
            };
            var solpLocal = SolpToClone();
            var cotizacionLocal = CotizacionToClone();
            var cotizacionPosicion = cotizacionLocal.CotizacionPosiciones.ToList();
            cotizacionPosicion.ForEach(x => x.Cotizacion = cotizacionLocal);


            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(solpLocal.Posiciones.ToList());
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(cotizacionPosicion);

            tipoCambioServiceMock.Setup(y => y.ObtenerTipoCambio(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });
            cotizacionLocal.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo = "SERVICIO";

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.ValidarPrecioCotizado(adjudicacionDto);
            Assert.That(result.Errores.Count == 0);

        }

        [Test]
        public void ValidarPrecioCotizadoImporteError()
        {
            var adjudicacionDto = new AdjudicacionDto
            {
                AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>
                {
                    new AdjudicacionPosicionDto
                    {
                        Adjudicacion_Id = 1,
                        Cantidad = 4,
                        CotizacionPosicion_Id = 1,
                        Id = 1,
                        SolpPosicion_Id = 1,
                        PrecioTotal = 10,
                        MonedaId = 5,
                        MonedaCodigo = "USD",
                        Moneda = new TablaSapDto
                        {
                            CodigoSap = "USD"
                        },
                        PlazoDeEntrega = DateTime.Now,
                    },
                },
                Cotizacion_Id = 1,
                FechaCreacion = DateTime.Now,
                Moneda_Id = 1,
                UsuarioCreador_Id = 1,
                Solp_Id = 1,
                CondicionesDeEntrega = "Condiciones"
            };
            var solpLocal = SolpToClone();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(solpLocal.Posiciones.ToList());

            var cotizacionLocal = CotizacionToClone();
            var cotizacionPosicion = cotizacionLocal.CotizacionPosiciones.ToList();
            cotizacionPosicion.ForEach(x => x.Cotizacion = cotizacionLocal);
            cotizacionPosicion.FirstOrDefault().CotizacionSubPosiciones.ToList().ForEach(x => x.Precio = 1000);

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(cotizacionPosicion);

            tipoCambioServiceMock.Setup(y => y.ObtenerTipoCambio(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });
            cotizacionLocal.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo = "SERVICIO";
            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
                .Returns(cotizacionLocal);
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.ValidarPrecioCotizado(adjudicacionDto);
            Assert.That(result.Errores.Count > 0);

        }

        [Test]
        [Ignore("Falta terminar de corregir.")]
        public void DesagruparPOOk()
        {
            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = SolpToClone()
            };
            var peticionDeOfertaLocal = PeticionDeOfertaToClone();
            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                           .Returns(new List<SolpPosicion>() { posicion });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(peticionDeOfertaLocal.Posiciones.ToList());



            var peticionDeOfertaUsuario = new PeticionDeOfertaUsuario
            {
                Id = 1,
                PeticionDeOferta = peticionDeOfertaLocal
            };

            var subposicion = new SolpSubposicion
            {
                Id = 1,
                Tarea = "Tarea",
                Cantidad = 2,
                PrecioBruto = 500,
                Unidad = new TablaSap { CodigoSap = "UNI" }
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { subposicion });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion { Id = 1, SolpPosicion = new SolpPosicion { TipoPosicion = new TablaGeneral { Codigo = "SERVICIO" } } } });

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });

            repositorioComprasMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(CotizacionToClone());
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1, Mail = "bmelgarejo", CUITRegistro = "2373739293", TipoUsuario = new TipoUsuario { Id = 1 }, Proveedores = new List<Proveedor> { new Proveedor { CUIT = "2373739293", TipoProveedor = new TipoUsuario { Id = 1 } } } });

            repositorioComprasMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(peticionDeOfertaUsuario);

            repositorioComprasMock.Setup(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(PeticionDeOfertaToClone());

            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(CotizacionToClone());

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioComprasMock.Setup(x => x.Listar(
                It.IsAny<Expression<Func<Usuario, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DirOrden>(),
                It.IsAny<IEnumerable<Expression<Func<Usuario, object>>>>())
                ).Returns(new List<Usuario> { new Usuario { Id = 1, Mail = "drodriguez@prueba.com" } });

            repositorioComprasMock.Setup(repo => repo.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), It.IsAny<IEnumerable<Expression<Func<SolpPosicion, object>>>>()))
            .Returns(new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now,
                Solp = new Solp { Pliego= new Pliego(), UsuarioCreacion= new Usuario{ Id = 1}, NroSolp = "1",ProveedorAsignado_Id=1
                , Posiciones = new List<SolpPosicion> { new SolpPosicion { Indice = 1, Cantidad = 22, FechaEntregaServicio = DateTime.Now,
                Solp = new Solp { Pliego= new Pliego(), UsuarioCreacion= new Usuario{ Id = 1}, NroSolp = "1",ProveedorAsignado_Id=1 } } }
                } } });

            obtenerSolpConsumerMOAMock.Setup(service => service.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>()))
             .Returns(new ObtenerSolpSAPResponse { Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { NumeroPosicion = "1", Cantidad = 22, Ordered = 0, NumeroSolicitud = "1" } } });

            var resultado = target.DesagruparPO(It.IsAny<string>(), It.IsAny<string>());
            this.repositorioComprasMock.Verify(x => x.GuardarCambios(), Times.Exactly(7));
        }

        [Test]
        public void GenerarSolpPdf_DebeRetornarArrayDeBytes()
        {
            var solpLocal = SolpToClone();

            var testImagePath = TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png";
            var testImage = iTextSharp.text.Image.GetInstance(testImagePath);
            httpContextServiceMock.Setup(y => y.ObtenerLogoImagen()).Returns(testImage);


            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<IEnumerable<Expression<Func<Solp, object>>>>(), It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Proveedores = new List<Proveedor>() });

            var usuariosComprasMockData = new List<UsuarioCompras>
            {
                new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
            };

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(usuariosComprasMockData);

            httpContextServiceMock.Setup(y => y.GetDirectory(It.IsAny<string>())).Returns(TestContext.CurrentContext.TestDirectory + "\\Templates\\NewPliegoSolpSinCondicionesTemplate.html");

            // Act
            var resultado = target.GenerarSolpPdf(1);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsInstanceOf<byte[]>(resultado);
            Assert.Greater(resultado.Length, 0); // Verifica que el array de bytes no esté vacío

        }

        [Test]
        public void ObtenerAdjuntosSolpAgrupar_RetornaAdjuntosCorrectamente()
        {

            var solpLocal = SolpToClone();
            solpLocal.TipoSolp.Codigo = "CON_PLIEGO";
            //solpLocal.Id = 1;
            solpLocal.EstadoSolpSap = new TablaSap { CodigoSap = "05" };
            solpLocal.UsuarioCompras = new UsuarioCompras();
            solpLocal.UsuarioCreacion = new Usuario
            {
                Id = 1,
                Mail = "bmelgarejo@prueba.com",
                TipoUsuario = new TipoUsuario
                {
                    Id = 1,
                    Nombre = "",
                    NombreCorto = ""
                },
                Roles = new List<Rol> {
                    new Rol
                    {
                        Nombre = "COMPRADOR",
                        PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Permiso = "COMPRADOR" }}
                    }
                },

            };
            solpLocal.UsuarioCompras = new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" };

            var usuariosComprasMockData = new List<UsuarioCompras>
            {
                new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
            };

            var testImagePath = TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png";
            var testImage = iTextSharp.text.Image.GetInstance(testImagePath);

            httpContextServiceMock.Setup(y => y.ObtenerLogoImagen()).Returns(testImage);
            httpContextServiceMock.Setup(y => y.GetDirectory(It.IsAny<string>())).Returns(TestContext.CurrentContext.TestDirectory + "\\Templates\\NewPliegoSolpSinCondicionesTemplate.html");

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(usuariosComprasMockData);
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<IEnumerable<Expression<Func<Solp, object>>>>(), It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioComprasMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Proveedores = new List<Proveedor>() });
            repositorioComprasMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>()))
             .Returns(solpLocal);

            repositorioComprasMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(CotizacionToClone());

            repositorioComprasMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioComprasMock.Setup(x => x.Listar<Usuario>(null, 0, null, DirOrden.Asc, null)).Returns(new List<Usuario> { new Usuario { Id = 1, Mail = "drodriguez@prueba.com" } });

            // Act
            var resultado = target.ObtenerAdjuntosSolpAgrupar("123");

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual("Solp-123-pliego-" + DateTime.Now.ToString("yyyyMMdd") + ".pdf", resultado.Pliego);
            Assert.IsNotNull(resultado.ArchivosEspecificacionesTecnicas);
            Assert.AreEqual(1, resultado.ArchivosEspecificacionesTecnicas.Count);
            Assert.AreEqual(1, resultado.ArchivosEspecificacionesTecnicas.First().Id);
            Assert.AreEqual("ruta1", resultado.ArchivosEspecificacionesTecnicas.First().Nombre); // Asumiendo que ObtenerNombre retorna la ruta
                                                                                                 // Continúa con más aserciones según sea necesario
        }

        [Test]
        public void GuardarEnvioCircularProveedor_DeberiaGuardarEnvioCircularYRetornarResultado()
        {
            // Arrange
            int id = 1;
            EnviarCircularEnum envioCircularA = EnviarCircularEnum.EnviarATodos;
            DateTime fechaLimite = DateTime.Today.AddDays(7);
            var solp = new Solp { Id = id };
            var resultadoEsperado = new Resultado
            {
                IdEntidad = id,
                Mensaje = "Se grabó con éxito"
            };

            repositorioComprasMock.Setup(r => r.Obtener<Solp>(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solp);
            repositorioComprasMock.Setup(r => r.GuardarCambios());

            // Act
            var resultado = target.GuardarEnvioCircularProveedor(id, envioCircularA, fechaLimite);

            // Assert
            DateTime fechalimiteEsperada = new DateTime(fechaLimite.Year, fechaLimite.Month, fechaLimite.Day, 23, 59, 59, DateTimeKind.Local);

            Assert.AreEqual(resultadoEsperado.IdEntidad, resultado.IdEntidad);
            Assert.AreEqual(resultadoEsperado.Mensaje, resultado.Mensaje);
            Assert.AreEqual(envioCircularA, solp.EnvioCircularA);
            Assert.AreEqual(fechalimiteEsperada, solp.FechaLimiteReenvioDocumentacionPorCambioCondiciones);
            repositorioComprasMock.Verify(r => r.Obtener<Solp>(It.IsAny<Expression<Func<Solp, bool>>>()), Times.Once);
            repositorioComprasMock.Verify(r => r.GuardarCambios(), Times.Once);
        }

        [Test]
        public void DesvincularSolpDePOMultiple_ValidacionPODebeQuedarConAlMenosUnaPosicion()
        {
            var solpPosicionId = 19876;
            var idsPeticionesDesvincular = new List<int> { 1001, 1024, 1032 };

            var peticionesOfertaBD = new List<PeticionDeOferta>
            {
                new PeticionDeOferta { Id = 1001, Posiciones = new Collection<PeticionDeOfertaSolpPosicion>() },
                new PeticionDeOferta { Id = 1024, Posiciones = new Collection<PeticionDeOfertaSolpPosicion> { new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 19876 } } }
            };

            repositorioComprasMock
                .Setup(r => r.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), 0, null, DirOrden.Asc, It.IsAny<IEnumerable<Expression<Func<PeticionDeOferta, object>>>>()))
                .Returns(peticionesOfertaBD);

            Assert.Throws<ValidationCustomException>(
                () => target.DesvincularSolpDePOMultipleMaterial(solpPosicionId, idsPeticionesDesvincular),
                "La PO 1024 no puede quedar sin posiciones vinculadas");
        }

        [Test]
        public void DesvincularSolpDePOMultiple_Ok()
        {
            var solpPosicionId = 19876;
            var idsPeticionesDesvincular = new List<int> { 1032 };

            var mUsuario1 = new Mock<Usuario>();
            mUsuario1.SetupProperty(u => u.Mail, "carlitos@mail.com");
            mUsuario1.SetupProperty(u => u.CUITRegistro, "20284850123");
            mUsuario1.Setup(u => u.ObtenerRazonSocial()).Returns("Carlitos");
            mUsuario1.Setup(u => u.ObtenerCodigoProveedor()).Returns("000487265");

            var mPoUsuario1 = new Mock<PeticionDeOfertaUsuario>().SetupProperty(pou => pou.Usuario, mUsuario1.Object);

            var peticionesOfertaBD = new List<PeticionDeOferta>
            {
                new PeticionDeOferta
                {
                    Id = 1032,
                    Posiciones = new Collection<PeticionDeOfertaSolpPosicion>
                    {
                        new PeticionDeOfertaSolpPosicion
                        {
                            SolpPosicion_Id = 19876,
                            SolpPosicion = new SolpPosicion
                            {
                                Solp = new Solp { Pliego = new Pliego() },
                                TipoPosicion_Id = 2,
                                TipoPosicion = new TablaGeneral { Codigo = "SERVICIOS" }
                            }
                        },
                        new PeticionDeOfertaSolpPosicion
                        {
                            SolpPosicion_Id = 19543,
                            SolpPosicion = new SolpPosicion
                            {
                                Solp = new Solp { Pliego = new Pliego() },
                                TipoPosicion_Id = 2,
                                TipoPosicion = new TablaGeneral { Codigo = "SERVICIOS" }
                            }
                        }
                    },
                    Usuario = new Usuario { Mail = "carlitos@gmail.com" },
                    Usuarios = new List<PeticionDeOfertaUsuario> { mPoUsuario1.Object },
                    UsuariosAdicionales = new List<PeticionDeOfertaUsuarioAdicional>(),
                    Archivos = new List<PeticionDeOfertaArchivo>()
                }
            };

            repositorioComprasMock
                .Setup(r => r.Listar<PeticionDeOferta>(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), 0, null, DirOrden.Asc, It.IsAny<IEnumerable<Expression<Func<PeticionDeOferta, object>>>>()))
                .Returns(peticionesOfertaBD);

            var solpPosicionBD = new SolpPosicion
            {
                Peticiones = new List<PeticionDeOfertaSolpPosicion>
                {
                    new PeticionDeOfertaSolpPosicion { PeticionDeOferta_Id = 1076, Id = 1 },
                    new PeticionDeOfertaSolpPosicion { PeticionDeOferta_Id = 1032, Id = 2 }
                }
            };

            repositorioComprasMock
                .Setup(r => r.Obtener<SolpPosicion>(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<Expression<Func<SolpPosicion, object>>[]>()))
                .Returns(solpPosicionBD);

            repositorioComprasMock
                .Setup(r => r.RemoverTodos(
                    It.Is<IEnumerable<PeticionDeOfertaSolpPosicion>>(
                        x => x.Count() == 1 && x.First().PeticionDeOferta_Id == 1032 && x.First().Id == 2)));

            repositorioComprasMock.Setup(r => r.GuardarCambios());

            repositorioComprasMock
                .Setup(r => r.Obtener<Configuracion>(It.IsAny<Expression<Func<Configuracion, bool>>>()))
                .Returns(new Configuracion { Value = "[ { \"Filename\": \"F-2285-4 PLIEGO GENERALIDADES\", \"MimeType\": \"application/pdf\" } ]" });

            mIEmailComprasService.Setup(e => e.EnviarMailPeticionDeOferta(It.Is<MailPeticionDeOfertaRequest>(r => r.EsProveedor)));
            mIEmailComprasService.Setup(e => e.EnviarMailPeticionDeOferta(It.Is<MailPeticionDeOfertaRequest>(r => !r.EsProveedor)));

            // Act
            target.DesvincularSolpDePOMultipleMaterial(solpPosicionId, idsPeticionesDesvincular);

            // Assert
            repositorioComprasMock.Verify(r =>
                r.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), 0, null, DirOrden.Asc, It.IsAny<IEnumerable<Expression<Func<PeticionDeOferta, object>>>>()),
                Times.Once);

            repositorioComprasMock.Verify(r =>
                r.Obtener(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<Expression<Func<SolpPosicion, object>>[]>()),
                Times.Once);

            repositorioComprasMock.Verify(r =>
                r.RemoverTodos(It.IsAny<IEnumerable<PeticionDeOfertaSolpPosicion>>()),
                Times.Once);

            repositorioComprasMock.Verify(r => r.GuardarCambios(), Times.Once);

            repositorioComprasMock.Verify(r =>
                r.Obtener(It.IsAny<Expression<Func<Configuracion, bool>>>()),
                Times.Once);

            mIEmailComprasService.Verify(e => e.EnviarMailPeticionDeOferta(It.Is<MailPeticionDeOfertaRequest>(r => r.EsProveedor)), Times.Once);
            mIEmailComprasService.Verify(e => e.EnviarMailPeticionDeOferta(It.Is<MailPeticionDeOfertaRequest>(r => !r.EsProveedor)), Times.Once);
        }
    }
}
