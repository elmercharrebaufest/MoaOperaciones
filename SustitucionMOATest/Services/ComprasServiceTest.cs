using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Org.BouncyCastle.Asn1.X509;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.ConsultasEF;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Security;

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
        private Mock<IVendedoresConsumerMOA> vendedoresConsumerMOAMock;
        private Mock<IAgregarRegistroInfoConsumerMOA> agregarRegistroInfoConsumerMOAMock;
        private Mock<IEmailService> emailServiceMock;
        private Mock<IReporteOrdenDeCompraConsumerMOA> reporteOrdenDeCompraConsumerMOAMock;
        private Mock<IObtenerUnidadesDeMedidaAlternativasConsumerMOA> obtenerUnidadesDeMedidaAlternativasConsumerMOAMock;
        private Mock<IObtenerPDFOrdenCompraConsumerMOA> obtenerPDFOrdenCompraConsumerMOAMock;
        private Mock<IListarSolpPendientesConsumerMOA> listarSolpPendientesConsumerMOAMock;
        private Mock<IObtenerAdjuntosSOLPEDConsumerMOA> obtenerAdjuntosSOLPEDConsumerMOAMock;
        private Mock<IEmailComprasService> mIEmailComprasService;
        private Mock<IComprasArchivosService> mIComprasArchivosService;
        private Mock<IComprasSapService> comprasSapService;

        private GuardarCotizacion guardarCotizacionToClone()
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
        private Cotizacion cotizacionToClone()
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
        private Solp solpToClone()
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
                                    Subposiciones = new List<SolpSubposicion> { new SolpSubposicion {
                                        Id = 1, Tarea = "Tarea", Cantidad = 2, PrecioBruto = 500, Unidad_Id = 1, Unidad = new TablaSap { CodigoSap = "UNI" } } }
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
        private SolpDto solpDtoToClone()
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
                        Unidad = new TablaSapDto { Codigo = "UNI", CodigoSap = "UNI" }
                    }
                },
                LiberadoresSapSolp = new List<LiberadorSapSolpDto> { new LiberadorSapSolpDto { Id = 1, Solp_Id = 1, LiberadorSap_Id = 1 } }
            };
        }
        private PeticionDeOferta peticionDeOfertaToClone()
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
        private PeticionDeOfertaRevisionTecnicaDto peticionDeOfertaRevisionTecnicaToClone()
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
            obtenerOrdenDeCompraConsumerMOAMock = new Mock<IObtenerOrdenDeCompraConsumerMOA>();
            obtenerOrdenesDeCompraParaSOLPConsumerMOAMock = new Mock<IObtenerOrdenesDeCompraParaSOLPConsumerMOA>();
            usuarioServiceMock = new Mock<IUsuarioService>();
            obtenerProveedorConsumerMOA = new Mock<IObtenerProveedorConsumerMOA>();
            vendedoresConsumerMOAMock = new Mock<IVendedoresConsumerMOA>();
            emailServiceMock = new Mock<IEmailService>();
            reporteOrdenDeCompraConsumerMOAMock = new Mock<IReporteOrdenDeCompraConsumerMOA>();
            obtenerUnidadesDeMedidaAlternativasConsumerMOAMock = new Mock<IObtenerUnidadesDeMedidaAlternativasConsumerMOA>();
            listarSolpPendientesConsumerMOAMock = new Mock<IListarSolpPendientesConsumerMOA>();
            obtenerPDFOrdenCompraConsumerMOAMock = new Mock<IObtenerPDFOrdenCompraConsumerMOA>();
            obtenerAdjuntosSOLPEDConsumerMOAMock = new Mock<IObtenerAdjuntosSOLPEDConsumerMOA>();
            mIEmailComprasService = new Mock<IEmailComprasService>();
            mIComprasArchivosService = new Mock<IComprasArchivosService>();
            comprasSapService = new Mock<IComprasSapService>();

            httpContextServiceMock.Setup(x => x.ObtenerPathLogoMail()).Returns(TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png");

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
                agregarRegistroInfoConsumerMOAMock.Object,
                emailServiceMock.Object,
                reporteOrdenDeCompraConsumerMOAMock.Object,
                obtenerUnidadesDeMedidaAlternativasConsumerMOAMock.Object,
                listarSolpPendientesConsumerMOAMock.Object,
                obtenerPDFOrdenCompraConsumerMOAMock.Object,
                obtenerAdjuntosSOLPEDConsumerMOAMock.Object,
                mIEmailComprasService.Object,
                mIComprasArchivosService.Object,
                comprasSapService.Object
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


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                            .Returns(new List<SolpPosicion>() { new SolpPosicion { Id = 1, FechaEntregaServicio = DateTime.Now, Solp = solpToClone() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion {
                Id = 1, SolpPosicion = new SolpPosicion {  FechaEntregaServicio = DateTime.Now, TipoPosicion = new TablaGeneral { Codigo = "SERVICIOS" } }
            } });

            repositorioMock.Setup(x => x.Listar<Usuario>(null, 0, null, DirOrden.Asc, null)).Returns(new List<Usuario> { new Usuario { Id = 1, Mail = "drodriguez@prueba.com" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(peticionDeOfertaToClone());
            repositorioMock.Setup(x => x.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionToClone());
            repositorioMock.Setup(x => x.Agregar(It.IsAny<PeticionDeOferta>())).Returns(peticionDeOfertaToClone());
            repositorioMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(new PeticionDeOfertaUsuario()
            {
                PeticionDeOferta = peticionDeOfertaToClone()
            });
            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario
            {
                Id = 1,
                Mail = "drodriguez@prueba.com",
                OrganizacionDeCompra = "2029",
                Proveedores = new List<Proveedor> { new Proveedor { Id = 1, RazonSocial = "Proveedor", CUIT = "000050", TipoProveedor = new TipoUsuario { Id = 1 } } }
            });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>())).Returns(new TablaSap { CodigoSap = "ARP", Id = 1 });
            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
                Cuentas = new List<SustitucionMOAModel.Models.WSMapMOA.Compras.Cuenta>()
                {
                    new SustitucionMOAModel.Models.WSMapMOA.Compras.Cuenta() { Descripcion = "MOA", Codigo = "MOA", Comp = "MOA"}
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
            List<TablaSapDto> ListaSap = new List<TablaSapDto>
            {
                new TablaSapDto {Id=1, Descripcion = "Prueba 1", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=2, Descripcion = "Prueba 2", CodigoSap="Otro", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=3, Descripcion = "Prueba 3", CodigoSap="MOA", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=4, Descripcion = "Prueba 1", CodigoSap="MOA Operaciones", Tabla = TablasSap.CodigoServicioSap},
                new TablaSapDto {Id=5, Descripcion = "Prueba 1", CodigoSap="MOA", Tabla = TablasSap.CecoSolpSap},
            };

            repositorioMock.Setup(x => x.Listar(
                It.IsAny<Expression<Func<TablaSap, TablaSapDto>>>(),
                It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DirOrden>()))
                .Returns(ListaSap);

            //repositorioMock
            //    .Setup(x => x.Listar(It.IsAny<Expression<Func<TablaSap, TablaSap>>>(),
            //                    It.IsAny<int>(),
            //                    It.IsAny<string>(),
            //                    It.IsAny<DirOrden>(),
            //                    It.IsAny<IEnumerable<Expression<Func<TablaSap, object>>>>()))
            //    .Returns(ListaSap);


            var expected = new List<TablaSapDto>
            {
                new TablaSapDto { Id = 1,  Descripcion = "Prueba 2", CodigoSap="Otro", Tabla = TablasSap.CodigoServicioSap }
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
            var solpLocal = solpToClone();
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
               .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "bmelgarejo@prueba.com.ar", OrganizacionDeCompra = "2029" });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
               .Returns(cotizacionToClone());
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


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
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
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });


            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(new Usuario { Id = 1, CUITRegistro = "232323", TipoUsuario = new TipoUsuario { Id = 3 }, Proveedores = new List<Proveedor>() { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } }, Habilitado = true, Mail = "bmelgarejo@prueba.com.ar", OrganizacionDeCompra = "2029" });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado)).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>(), It.IsAny<bool>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { cotizacionToClone() });
            modificarSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new ModificarSolpConsumerMOAResponse());

            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarAdjudicacionServicioOk()
        {
            var solpLocal = solpToClone();
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
               .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "bmelgarejo@prueba.com.ar", OrganizacionDeCompra = "2029" });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
               .Returns(cotizacionToClone());
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
            crearPedidoConsumerMOAMock.Setup(y => y.Request(It.IsAny<Adjudicacion>(), It.IsAny<bool>())).Returns(new CrearPedidoConsumerMOAResponse
            {
                NumeroPedido = "383383932",
                Errores = new List<CrearPedidoConsumerMOAError> { },
                Resultado = "OK"
            });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { cotizacionToClone() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
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
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });


            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(new Usuario { Id = 1, CUITRegistro = "232323", TipoUsuario = new TipoUsuario { Id = 3 }, Proveedores = new List<Proveedor>() { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } }, Habilitado = true, Mail = "bmelgarejo@prueba.com.ar", OrganizacionDeCompra = "2029" });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado)).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });
            modificarSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new ModificarSolpConsumerMOAResponse());

            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
            Assert.That(result.Errores.Count == 0);
        }

        [Test]
        public void GrabarAdjudicacionServicioSinOrganizacionDeCompra()
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
                Solp = solpToClone()
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
               .Returns(new Usuario { Id = 1, CUITRegistro = "32332232", Habilitado = true, Mail = "drodriguez@prueba.com.ar" });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
               .Returns(cotizacionToClone());
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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { cotizacionToClone() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
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
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });


            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(new Usuario { Id = 1, CUITRegistro = "232323", TipoUsuario = new TipoUsuario { Id = 3 }, Proveedores = new List<Proveedor>() { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } }, Habilitado = true, Mail = "bmelgarejo@prueba.com.ar", OrganizacionDeCompra = "2029" });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado)).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            var result = target.GrabarAdjudicacion(adjudicacionDto, 1);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Adjudicacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
            Assert.That(result.Errores.Count == 1);
        }

        [Test]
        public void ListarAdjudicaciones()
        {
            var adjudicacionId = 1;
            var nroSolp = "0212303121";
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<Expression<Func<Solp, string>>>()))
            .Returns(nroSolp);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "1", Id = 1 } });
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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<TablaSap>() { new TablaSap { Id = 1, Codigo = "", CodigoSap = "", Descripcion = "" } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });

            var result = target.GrabarPeticionDeOferta(peticionDeOfertaLocal, null, false, null);

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
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Id = 1 });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(new PeticionDeOfertaUsuario
               {
                   Id = 1,
                   PeticionDeOferta = peticionDeOfertaToClone()
               });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion {
                Id = 1, SolpPosicion = new SolpPosicion { TipoPosicion = new TablaGeneral { Codigo = "SERVICIOS" } }
            } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(new Cotizacion { Id = 1, CotizacionEstado_Id = 1 });

            var result = target.GrabarCotizacion(guardarCotizacionToClone(), null, false, 1, false);

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
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Id = 1 });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()))
               .Returns(new PeticionDeOfertaUsuario
               {
                   Id = 1,
                   PeticionDeOferta = peticionDeOfertaToClone()
               });
            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = solpToClone()
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionToClone());

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.GrabarCotizacion(guardarCotizacionToClone(), null, false, 1, false);

            repositorioMock.Verify(y => y.Obtener<Usuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
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
                PeticionDeOferta = peticionDeOfertaToClone()
            };

            var subposicion = new SolpSubposicion
            {
                Id = 1,
                Tarea = "Tarea",
                Cantidad = 2,
                PrecioBruto = 500,
                Unidad = new TablaSap { CodigoSap = "UNI" }
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { subposicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion { Id = 1, SolpPosicion = new SolpPosicion { TipoPosicion = new TablaGeneral { Codigo = "SERVICIOS" } } } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(cotizacionToClone());
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1, Mail = "bmelgarejo", CUITRegistro = "2373739293", TipoUsuario = new TipoUsuario { Id = 1 }, Proveedores = new List<Proveedor> { new Proveedor { CUIT = "2373739293", TipoProveedor = new TipoUsuario { Id = 1 } } } });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(peticionDeOfertaUsuario);

            repositorioMock.Setup(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(peticionDeOfertaToClone());

            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionToClone());

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.CrearCotizacionConTrabajoYaHecho(solpToClone());

            repositorioMock.Verify(y => y.Obtener<Usuario>(It.IsAny<int>()), Times.Exactly(2));
            repositorioMock.Verify(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(6));
        }

        [Test]
        public void EditarSolpOk()
        {
            var solpDtoLocal = solpDtoToClone();
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOferta>() { peticionDeOfertaToClone() });

            repositorioMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(solpToClone());

            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });

            target.GuardarSolp(solpDtoLocal, adjuntosMock.Object);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(4));
        }

        [Test]
        public void FinalizarSolpOk()
        {
            var solpDtoLocal = solpDtoToClone();
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
               .Returns(new List<PeticionDeOferta>() { peticionDeOfertaToClone() });

            repositorioMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(solpToClone());
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>())).Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>())).Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            target.GuardarSolp(solpDtoLocal, adjuntosMock.Object);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Solp>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(5));
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

            target.ObtenerPrecioTotalPosicionProveedor(guardarCotizacionLocal);
            repositorioMock.Verify(y => y.Obtener<TablaSap>(It.IsAny<int>()), Times.Exactly(2));
        }

        [Test]
        public void CrearOrdenDeCompraConRegistroInfoOk()
        {
            var solpDtoLocal = solpDtoToClone();
            solpDtoLocal.Finalizar = true;
            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Setup(d => d.FileName).Returns("LogoBaufest.png");
            byte[] dummyData = new byte[1024];
            new Random().NextBytes(dummyData);
            MemoryStream memoryStream = new MemoryStream(dummyData);
            file1.Setup(d => d.InputStream).Returns(memoryStream);
            file1.Setup(d => d.ContentLength).Returns(new Random().Next(1024, 1024));

            repositorioMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(solpToClone());

            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>())).Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>())).Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });

            vendedorServiceMock.Setup(y => y.GetDatosFiscales(It.IsAny<string>(), It.IsAny<string>())).Returns(new VendedorDetalleWSMOAResponse
            {
                cabeceras = new List<Cabecera> { new Cabecera { cuit = "21373773772", descripcion = "descripcion", calleFiscal = "", cpFiscal = "", provFiscal = "", locaFiscal = "" } }
            });

            var registroInfo = new List<RegistroInfoDto>() { new RegistroInfoDto {
                ProveedorId = 1, PosicionId = 1, CantidadAdjudicacion = 5, Moneda = "ARP"
            } };
            SetUpOCPeticionCotizacion();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<AdjudicacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<AdjudicacionPosicion> { new AdjudicacionPosicion { Id = 1, SolpPosicion_Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion> { cotizacionToClone() });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CentroDireccion, bool>>>()))
              .Returns(new CentroDireccion { CodigoSap = "29292", RegionSap = new RegionSap { Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
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
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });


            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
            .Returns(new Usuario { Id = 1, CUITRegistro = "232323", TipoUsuario = new TipoUsuario { Id = 3 }, Proveedores = new List<Proveedor>() { new Proveedor { Id = 1, RazonSocial = "ARROYITO", CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } }, Habilitado = true, Mail = "bmelgarejo@prueba.com.ar", OrganizacionDeCompra = "2029" });

            usuarioServiceMock.Setup(x => x.GrabarProveedor(It.IsAny<ProveedorDto>(), EstadoAprobacion.Aprobado)).Returns(new ResultadoGenerico { ProveedorDto = new ProveedorDto { Id = 1 } });
            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<UsuarioCompras>() { new UsuarioCompras { Mail = "bmelgarejo@test.com", Id = 1 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Desc, null)).Returns(new List<Adjudicacion>() { new Adjudicacion { Id = 1, NumeroOrdenDeCompra = "1", Usuario = new Usuario { Mail = "mail@mail.com" } } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<Expression<Func<TablaSap, bool>>>()))
               .Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaGeneral>(It.IsAny<Expression<Func<TablaGeneral, bool>>>()))
              .Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener<TablaEstado>(It.IsAny<Expression<Func<TablaEstado, bool>>>()))
             .Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UnidadMedidaSap, Tuple<string, string>>>>(), It.IsAny<Expression<Func<UnidadMedidaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc)).Returns(new List<Tuple<string, string>> { Tuple.Create("Comercial", "UM") });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(),
               It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<RegionSap>() { new RegionSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.CrearOrdenDeCompraConRegistroInfo(registroInfo, 1);

            var expected = new List<RespuestaCrearOrdenDeCompra> { new RespuestaCrearOrdenDeCompra {
                Errores = new List<string>(), IdEntidad = 0, Mensaje = "OK", NumeroPedido = "383383932"
            } };

            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<Cotizacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(8));
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

            var finalizar = false;

            var revision = new PeticionDeOfertaRevisionTecnicaDto
            {
                Id = 1
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaUsuario>() { new PeticionDeOfertaUsuario { Id = 1, RealizoVisita = true, PeticionDeOferta = peticionDeOfertaToClone() } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOfertaRevisionTecnica>())).Returns(new PeticionDeOfertaRevisionTecnica { Id = 1 });


            target.GrabarRevisionTecnica(peticiones, 1, finalizar, revision);
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void GrabarRevisionTecnicaFinalizarTrueOk()
        {
            var peticiones = new List<PeticionDeOfertaUsarioDto>
            {
                new PeticionDeOfertaUsarioDto
                 {
                   Id = 1,
                   PlazoDeOferta = DateTime.Now.AddDays(-5)
                 }
            };

            var finalizar = true;

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaUsuario>() { new PeticionDeOfertaUsuario { Id = 1, RealizoVisita = true, PeticionDeOferta = peticionDeOfertaToClone(), } });
            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOfertaRevisionTecnicaDto>())).Returns(peticionDeOfertaRevisionTecnicaToClone());


            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<Expression<Func<PeticionDeOferta, PeticionDeOfertaDto>>>()))
                .Returns(new PeticionDeOfertaDto { Id = 1, Solp_Id = 1, RegistroInfo = false, UsuarioCreador_Id = 1, PlazoDeOferta = DateTime.Now.AddDays(-5) });

            target.GrabarRevisionTecnica(peticiones, 1, finalizar, peticionDeOfertaRevisionTecnicaToClone());
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void TraerCotizacionOk()
        {
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<TraerCotizacionConsulta>())).Returns(new PeticionDeOfertaDto
            {
                CotizacionId = 1,
                Cotizacion = new CotizacionDto { ArchivosCotizacion = null },
                TipoPosicionCodigo = "MATERIALES",
                PeticionDeOfertaPosicion = new List<PeticionDeOfertaSolpPosicionDto> { new PeticionDeOfertaSolpPosicionDto { Posiciones = new SolpPosicionDto { Codigo = "000000000050224373" } } }
            });
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionToClone());
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" },
            new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" }};
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            obtenerUnidadesDeMedidaAlternativasConsumerMOAMock.Setup(y => y.Request(It.IsAny<List<string>>())).Returns(new List<UnidadesDeMedida>
            { new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "UNI", Denominador = 1, Numerador = 1 }});
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
                    Usuario_Id = 1,
                    //UsuarioComprasSAP = "A"
                },
            });

            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI" });
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario
            {
                Mail = "test",
                CUITRegistro = "232323",
                TipoUsuario = new TipoUsuario { Id = 3 },
                Id = 1,
                Proveedores = new List<Proveedor> { new Proveedor { Id = 1, CUIT = "232323", TipoProveedor = new TipoUsuario { Id = 3 } } }
            });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), null)).Returns(new List<Adjudicacion> { new Adjudicacion { Usuario = new Usuario { Mail = "test2" } } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), null)).Returns(new List<UsuarioCompras> { new UsuarioCompras { Id = 1, Mail = "test2" } });

            target.ObtenerOrdenDeCompra("454565645");
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Exactly(1));

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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Adjudicacion> { new Adjudicacion { Usuario = new Usuario { Mail = "" } } });

            target.ObtenerOrdenDeCompra(It.IsAny<string>());
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);

        }

        [Test]
        public void ActualizarFechaLiberacionConTrabajoHechoOk()
        {
            var solpLocal = solpToClone();
            solpLocal.TrabajoYaHecho = true;
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1 });
            SetUpOCPeticionCotizacion();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
              .Returns(new List<PeticionDeOferta>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
           .Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });

            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(7));
        }

        [Test]
        public void ActualizarFechaLiberacionConProveedorAsignadoOk()
        {
            var solpLocal = solpToClone();
            solpLocal.CondEspProveedorAsignado = true;
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1 });
            SetUpOCPeticionCotizacion();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
              .Returns(new List<PeticionDeOferta>());
            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
        }

        [Test]
        public void ActualizarFechaLiberacionConAdicionalOk()
        {
            var solpLocal = solpToClone();
            solpLocal.Adicional = true;

            vendedorServiceMock.Setup(y => y.GetDatosFiscales(It.IsAny<string>(), It.IsAny<string>())).Returns(new VendedorDetalleWSMOAResponse
            {
                cabeceras = new List<Cabecera> { new Cabecera { cuit = "21373773772", descripcion = "descripcion", calleFiscal = "", cpFiscal = "", provFiscal = "", locaFiscal = "" } }
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1, CodigoSap = "Codigo" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>())).Returns(new Localidad { ProvinciaId = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<CentroDireccion, bool>>>())).Returns(new CentroDireccion { CodigoSap = "Codigo" });
            SetUpOCPeticionCotizacion();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOferta>());
            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
        }

        [Test]
        public void AutocompleteMaterialRFCOk()
        {
            RegistroInfoDto registroInfo = new RegistroInfoDto { Cantidad = 5, Moneda = "USDM", Centro = "1029", GrupoDeCompras = "" };
            obtenerRegistroInfoConsumerMOAMock.Setup(y => y.ObtenerRegistroInfoConsumer(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new List<RegistroInfoDto> { registroInfo });
            var result = target.ObtenerUltimoRegistroMaterial("codigoMaterial", "codigoCentro", "codigoGrupoDeCompras");
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(registroInfo.GetType(), result.GetType());
        }

        [Test]
        public void ListarProveedorPOOk()
        {
            var listaPO = new ListaPaginada<PeticionDeOfertaDto>(new List<PeticionDeOfertaDto> { new PeticionDeOfertaDto { Id = 1, ItemsTotales = 7 } }, 1, 10, 5);

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario { CUITRegistro = "30709142301", Roles = new List<Rol> { new Rol { Codigo = "COMPRADOR" } } });
            repositorioMock.Setup(y => y.ListarConsultaPaginada(It.IsAny<ListarSolpPOConsulta>())).Returns(listaPO);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOferta> { peticionDeOfertaToClone() });

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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(adjudicaciones);

            emailServiceMock.Setup(y => y.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(),
               It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, byte[]>>()));

            target.ActualizarFechaLiberacionOC(nroOc, fechaLiberacion);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ObtenerUltimaSolpOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                DirOrden.Asc, null)).Returns(new List<Solp>() { solpToClone() });

            var result = target.ObtenerUltimaSolp(It.IsAny<int>());
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                DirOrden.Asc, null), Times.Once);
            Assert.That(result, Is.Not.Null);
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


            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOfertaCierre>())).Returns(peticionCierre);

            var result = target.CerrarCotizacion(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOfertaCierre>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void DescargarAdjuntosCotizacionOk()
        {
            var cotizacionlocal = cotizacionToClone();
            cotizacionlocal.Archivos = new List<Archivo>()
            {
                new Archivo
                {
                  Id = 1,
                  FileKey = "12323",
                  Ruta = "ruta/archivo"
                }
            };
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionlocal);
            var result = target.DescargarAdjuntosCotizacion(It.IsAny<int>(), TestContext.CurrentContext.TestDirectory, true);
            repositorioMock.Verify(y => y.Obtener<Cotizacion>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GrabarProveedoresEnPeticionDeOfertaOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
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


            repositorioMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(peticionDeOfertaToClone());
            emailServiceMock.Setup(y => y.EnviarMail(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<AlternateView>(),
               It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, byte[]>>()));
            httpContextServiceMock.Setup(y => y.GetDirectory(It.IsAny<string>())).Returns(TestContext.CurrentContext.TestDirectory + "\\Templates\\Example.html");
            repositorioMock
            .Setup(y => y.Obtener(It.IsAny<Expression<Func<Configuracion, bool>>>()))
            .Returns(new Configuracion
            {
                Value = "Configuraciones"
            });
            repositorioMock
           .Setup(y => y.Obtener(It.IsAny<Expression<Func<Localidad, bool>>>()))
           .Returns(new Localidad
           {
               Nombre = "Localidad"
           });
            repositorioMock
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

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
            repositorioMock.Verify(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ObtenerTablaSapOk()
        {
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" },
            new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" }};
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            var result = target.ObtenerTablaSap("EstadoSolpSap");

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test]
        public void ListarTablaSapOk()
        {
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "0011", Tabla = "OrdenSolpSap", CodigoSap = "11", Descripcion = "Limpiar rotor" },
            new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" }};
            var tablaSap = new List<TablaSap> { new TablaSap { Id = 11, Codigo = "0011", Tabla = "OrdenSolpSap", CodigoSap = "11", Descripcion = "Limpiar rotor" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpPosicion> { new SolpPosicion { Id = 1, ValorTipoImputacion_Id = 11 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<SolpSubposicion> { new SolpSubposicion { Id = 1, TipoImputacion_Id = 11 } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            var result = target.ListarTablaSap(new List<string> { "OrdenSolpSap" });

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test]
        public void ObtenerTablaGeneralOk()
        {
            var tablaGralDto = new List<TablaGeneralDto> { new TablaGeneralDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            var tablaGral = new List<TablaGeneral> { new TablaGeneral { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaGeneral, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaGral);
            var result = target.ObtenerTablaGeneral("tabla");

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaGralDto.GetType());
        }

        [Test]
        public void ObtenerTablaEstadoOk()
        {
            var tablaEstadoDto = new List<TablaEstadoDto> { new TablaEstadoDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            var tablaEstado = new List<TablaEstado> { new TablaEstado { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaEstado, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaEstado);
            var result = target.ObtenerTablaEstado("tabla");

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaEstadoDto.GetType());
        }

        [Test]
        public void ObtenerCentrosDireccionOk()
        {
            var centroDireccionDto = new List<CentroDireccionDto> { new CentroDireccionDto { CodigoSap = "1029", Direccion = "Benielli 398", Numero = "408411", Cp = "2200", Pais = "AR" } };
            var centroDireccion = new List<CentroDireccion> { new CentroDireccion { CodigoSap = "1029", Direccion = "Benielli 398", Numero = "408411", Cp = "2200", Pais = "AR" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CentroDireccion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(centroDireccion);
            var result = target.ObtenerCentrosDireccion();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), centroDireccionDto.GetType());
        }

        [Test]
        public void ObtenerDatosPorCodigosSapOk()
        {
            var tablaSapDto = new List<TablaSapDto> { new TablaSapDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            var result = target.ObtenerDatosPorCodigosSap(tablaSapDto);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test]
        public void ListarSolpOk()
        {
            var usuarioCompras = new List<TablaGeneralDto> { new TablaGeneralDto { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", Descripcion = "Liberación concluida" } };
            var listaPaginada = new ListaPaginada<SolpDto>(new List<SolpDto> { new SolpDto { Id = 1, ItemsTotales = 2 } }, 1, 10, 5);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
            .Returns(new List<UsuarioCompras>());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, PeticionDeOfertaDto>>>(), It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<PeticionDeOfertaDto>() { new PeticionDeOfertaDto { Id = 1, Solp_Id = 1, RegistroInfo = false, UsuarioCreador_Id = 1, FechaCreacion = new DateTime(), Observaciones = "",
              Usuarios = new List<PeticionDeOfertaUsarioDto>() } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, AdjudicacionDto>>>(), It.IsAny<Expression<Func<Adjudicacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<AdjudicacionDto>() { new AdjudicacionDto { Id = 1, Solp_Id = 1, UsuarioCreador_Id = 1, FechaCreacion = new DateTime(), NumeroOrdenDeCompra = "Nro",
              Proveedor = "Proveedor" } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(), It.IsAny<Paginacion>(), It.IsAny<Expression<Func<Solp, bool>>>()))
              .Returns(listaPaginada);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>())).Returns(new Usuario { Roles = new List<Rol> { new Rol { Codigo = "COMPRADOR" } } });

            var result = target.ListarSolp(new UsuarioDto { Id = 1, Permisos = new List<string> { "VER TODAS SOLPS" } }, new Paginacion(), "", "", new DateTime(), new DateTime(), true, false, true, false, false);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result, listaPaginada);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }

        [Test]
        public void ListarProvinciaOk()
        {
            var listaPciaDto = new List<ProvinciaDto> { new ProvinciaDto { ProvinciaId = 1, Nombre = "Jujuy", Orden = 1 } };
            var listaPcia = new List<Provincia> { new Provincia { ProvinciaId = 1, Nombre = "Jujuy", Orden = 1 } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Provincia, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
            .Returns(listaPcia);
            var result = target.ListarProvincia();

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), listaPciaDto.GetType());
        }

        [Test]
        public void BorrarSolpOk()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpToClone());
            var result = target.BorrarSolp(1);
            Assert.AreEqual(result, "Se borró correctamente.");
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void TraerSolpIdOk()
        {
            var solpLocalDto = solpDtoToClone();
            var solpLocal = solpToClone();
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
            repositorioMock.Setup(y => y.Obtener(It.IsAny<IEnumerable<Expression<Func<Solp, object>>>>(), It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Proveedores = new List<Proveedor>() });
            var result = target.TraerSolpId(1);

            Assert.AreEqual(result.GetType(), solpLocalDto.GetType());
        }

        [Test]
        public void ActualizarMaterialesSolpOk()
        {
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "Centro", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            obtenerMaterialesSolpConsumerMOAMock.Setup(y => y.request(It.IsAny<List<string>>(), It.IsAny<string>())).Returns(new MaterialWSMOAResponse
            {
                Materiales = new List<SustitucionMOAModel.Models.WSMapMOA.Compras.Material> { new SustitucionMOAModel.Models.WSMapMOA.Compras.Material {
                NroMaterial = "", NombreDeMaterial = "", TipoMaterial = "", TipoValoracion = "", GrupoCompras = "", PrecioDelMaterial = 500, CuentaDeMayor = "", TextoAmpliado = ""} }
            });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<MaterialSolp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<MaterialSolp> { new MaterialSolp { CodigoSap = "", Centro_Id = 1 } });
            target.ActualizarMaterialesSolp();

            repositorioMock.Verify(x => x.Agregar(It.IsAny<MaterialSolp>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void ActualizarEstadoSolpBulkOk()
        {
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Solp> { new Solp { NroSolp = "00553322" } });
            obtenerSolpConsumerMOAMock.Setup(y => y.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>())).Returns(new ObtenerSolpSAPResponse
            {
                Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { EstadoSolpSap = "05" } }
            });
            repositorioMock.Setup(x => x.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpToClone());

            target.ActualizarEstadoSolpBulk();

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ActualizarOfertasAlEditarSolpMaterialLiberadaOk()
        {
            var solpDtoLocal = solpDtoToClone();
            solpDtoLocal.Id = 1;
            solpDtoLocal.Finalizar = true;
            solpDtoLocal.TrabajoYaHecho = true;
            var solpLocal = solpToClone();
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

            repositorioMock.Setup(y => y.Obtener<Solp>(It.IsAny<int>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaGeneral, bool>>>())).Returns(new TablaGeneral { Codigo = "23234" });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaEstado, bool>>>())).Returns(new TablaEstado { Id = 1, Codigo = "23234" });
            crearSolpConsumerMOAMock.Setup(x => x.Request(It.IsAny<SolpSAPDto>())).Returns(new CrearSolpConsumerMOAResponse { NumeroSolp = "383737373", Resultado = "OK", Errores = new List<CrearSolpConsumerMOAError>() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOferta>() { peticionDeOfertaToClone() });
            modificarSolpConsumerMOAMock.Setup(y => y.Request(It.IsAny<SolpSAPDto>())).Returns(new ModificarSolpConsumerMOAResponse
            {
                Errores = new List<ModificarSolpConsumerMOAError>()
            });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>())).Returns(new PeticionDeOferta { Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOfertaUsuario, bool>>>())).Returns(new PeticionDeOfertaUsuario { Id = 1 });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Cotizacion, bool>>>())).Returns(new Cotizacion { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<CotizacionPosicion> { new CotizacionPosicion { Id = 1, PeticionDeOfertaSolpPosicion = new PeticionDeOfertaSolpPosicion { SolpPosicion_Id = 1 } } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaSolpPosicion> { new PeticionDeOfertaSolpPosicion { Id = 1 } });

            target.GuardarSolp(solpDtoLocal, adjuntosMock.Object);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<CotizacionPosicion>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(9));
        }

        [Test]
        public void ActualizarFechaLiberacionConUsuarioComprasYTipoServicioEnviaMail()
        {
            var solpLocal = solpToClone();
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
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOferta>() { peticionDeOfertaToClone() });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal); // Usar solpLocal en lugar de solp
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<TablaSap, bool>>>())).Returns(new TablaSap { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
             .Returns(new List<SolpSubposicion>() { new SolpSubposicion { Id = 1 } });
            target.ActualizarFechaLiberacion(solpLocal.NroSolp, DateTime.Now);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(1));
        }

        [Test]
        public void ObtenerLegajoOk()
        {
            LegajoExternoDto legajo = new LegajoExternoDto { ListaLegajos = new List<LegajoDto>() };
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Adjudicacion, bool>>>())).Returns(new Adjudicacion
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
            repositorioMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(peticionDeOfertaToClone());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaCierre, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaCierre> { new PeticionDeOfertaCierre { Id = 1, Fecha = DateTime.Now, Observacion = "", Usuario_Id = 1, Usuario = new Usuario { Id = 1, Mail = "", CUITRegistro = "005522" } } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Circular, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Circular> { new Circular { Id = 1, Archivos = new Collection<Archivo> { new Archivo { Id = 1, Ruta = "Ruta" } }, FechaCreacion = DateTime.Now, Usuario = new Usuario { Id = 1, TipoUsuario = new TipoUsuario { Id = 1, Nombre = "", NombreCorto = "" } } } });

            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<PeticionDeOfertaVisualizacionPrecio, bool>>>()))
                .Returns(new PeticionDeOfertaVisualizacionPrecio { Archivo = new Archivo { Id = 1, Ruta = "Ruta" }, FechaCreacion = DateTime.Now, PeticionDeOferta_Id = 1, UsuarioCreador_Id = 1, Observaciones = "Observacion", Usuario = new Usuario { Id = 1, TipoUsuario = new TipoUsuario { Id = 1, Nombre = "", NombreCorto = "" } } });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaVisualizacionPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaVisualizacionPrecio> { new PeticionDeOfertaVisualizacionPrecio { Id = 1 } });

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

            var result = target.ObtenerReporteOrdenDeCompra(nroOC, fechaDesde, fechaHasta, codigoProveedor);

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
            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOfertaVisualizacionPrecio>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
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

            repositorioMock.Setup(x => x.Listar(
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
            repositorioMock.Setup(x => x.Listar(
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
            var solpLocal = solpToClone();
            repositorioMock.Setup(x => x.Obtener<Solp>(It.IsAny<int>())).Returns(solpLocal);
            repositorioMock.Setup(x => x.Obtener<Usuario>(It.IsAny<int>())).Returns(
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

            repositorioMock.Verify(x => x.Obtener<Solp>(solpId), Times.Once);
            repositorioMock.Verify(x => x.Obtener<Usuario>(usuarioActualId), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
        }

        [Test]
        public void ObtenerChatProveedorTest()
        {
            int peticionDeOfertaUsuarioId = 1;
            int usuarioActualId = 2;

            repositorioMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId)).Returns(new PeticionDeOfertaUsuario
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

            repositorioMock.Verify(x => x.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuarioId), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(2));
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

            repositorioMock.Setup(x => x.Agregar(It.IsAny<ChatInternoCompras>())).Callback((ChatInternoCompras entity) =>
            {
                Assert.AreEqual(chatInternoCompras.Mensaje, entity.Mensaje);
                Assert.AreEqual(chatInternoCompras.Solp_Id, entity.Solp_Id);
                Assert.AreEqual(chatInternoCompras.Usuario_Id, entity.Usuario_Id);
            });

            var result = target.GrabarMensajeChatInterno(mensajeDto);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<ChatInternoCompras>()), Times.Once, "Agregar method should be called once.");
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once, "GuardarCambios method should be called once.");

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

            repositorioMock.Setup(x => x.Agregar(It.IsAny<ChatExternoCompras>())).Callback((ChatExternoCompras entity) =>
            {
                Assert.AreEqual(chatExternoCompras.Mensaje, entity.Mensaje);
                Assert.AreEqual(chatExternoCompras.PeticionDeOferta_Id, entity.PeticionDeOferta_Id);
                Assert.AreEqual(chatExternoCompras.Usuario_Id, entity.Usuario_Id);
            });

            var result = target.GrabarMensajeChatExterno(mensajeDto);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<ChatExternoCompras>()), Times.Once, "Agregar method should be called once.");
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once, "GuardarCambios method should be called once.");

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

            repositorioMock.Setup(x => x.Obtener<Solp>(solp_id)).Returns(solpToClone());
            repositorioMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(peticionDeOfertaUsuario_id)).Returns(new PeticionDeOfertaUsuario
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

            repositorioMock.Verify(x => x.Obtener<Solp>(solp_id), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(File.Exists(result));

        }

        [Test]
        public void ListarOfertasCompradorOk()
        {
            var usuario = new UsuarioDto { Permisos = new List<string> { "ADJUDICAR DENTRO DEL PLAZO DE OFERTAS" } };
            repositorioMock.Setup(y => y.ObtenerConsultaEscalar(It.IsAny<ComparadorOfertasConsulta>())).Returns(new PeticionDeOfertaDto
            {
                CotizacionId = 1,
                Cotizacion = new CotizacionDto { ArchivosCotizacion = null },
                TipoPosicionCodigo = "MATERIALES",
                Usuarios = new List<PeticionDeOfertaUsarioDto> { new PeticionDeOfertaUsarioDto { Cotizacion = new CotizacionDto { CotizacionPosiciones = new List<CotizacionPosicionDto> { new CotizacionPosicionDto {
                Id = 1, PeticionDeOfertaSolpPosicion_Id = 1, Cantidad = 2, Precio = 500, UnidadMedida = new TablaSapDto { Descripcion = "UNI" }, TotalPesos = 1000 } } } } },
                PeticionDeOfertaPosicion = new List<PeticionDeOfertaSolpPosicionDto> {
                    new PeticionDeOfertaSolpPosicionDto { Id = 1, Posicion = new SolpPosicionDto { Unidad = new TablaSapDto { Descripcion = "PAR" }, CodigoMaterialSap = new MaterialSolpDto { Codigo = "000000000050224373" } }, Posiciones = new SolpPosicionDto { Codigo = "000000000050224373" } }
                },
                NrosSolp = new List<string> { "102002020" },
                SolpDto = new SolpDto { ObservacionesCotizacionLista = new List<String> { "Observacion" } }
            });
            var cotizacionLocal = cotizacionToClone();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), null)).Returns(cotizacionLocal.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.ToList());

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaVisualizacionPrecio, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<PeticionDeOfertaVisualizacionPrecio> { new PeticionDeOfertaVisualizacionPrecio { Id = 1 } });
            repositorioMock.Setup(y => y.Obtener<TablaSap>(It.IsAny<int>())).Returns(new TablaSap { Id = 1 });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Adjudicacion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DirOrden>(), null)).Returns(new List<Adjudicacion> { new Adjudicacion { Usuario = new Usuario { Mail = "test@gmail.com" } } });
            obtenerSolpConsumerMOAMock.Setup(y => y.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>())).Returns(new ObtenerSolpSAPResponse
            {
                Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { EstadoSolpSap = "05", NumeroPosicion = "1" } }
            });
            obtenerUnidadesDeMedidaAlternativasConsumerMOAMock.Setup(y => y.Request(It.IsAny<List<string>>())).Returns(new List<UnidadesDeMedida>
            { new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "UNI", Denominador = 1, Numerador = 1 },
            new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "PAR", Denominador = 2, Numerador = 1 }});

            target.ListarOfertasComprador(It.IsAny<int>(), usuario);
            repositorioMock.Verify(y => y.ObtenerConsultaEscalar(It.IsAny<ComparadorOfertasConsulta>()), Times.Once);
        }

        [Test]
        public void ValidarSolpTratadaTest()
        {
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpToClone());
            obtenerSolpConsumerMOAMock.Setup(y => y.RequestSolpWithNroAndDates(It.IsAny<ObtenerSolpRequest>())).Returns(new ObtenerSolpSAPResponse
            {
                Posiciones = new List<PosicionSolpSAP> { new PosicionSolpSAP { Cantidad = 1000, NumeroPosicion = "00011" } }
            });
            target.ValidarSolpTratada("02929292");
            repositorioMock.Verify(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>()), Times.Once);
        }

        [Test]
        public void ListarSolpCompradorOk()
        {
            var listaSolp = new ListaPaginada<SolpDto>(new List<SolpDto> { new SolpDto { Id = 1, ItemsTotales = 7 } }, 1, 10, 5);
            repositorioMock.Setup(y => y.ListarConsultaPaginada(It.IsAny<ListarSolpConsulta>())).Returns(listaSolp);

            var result = target.ListarSolpComprador(1, new Paginacion(), "nroSolp", "", null, null, false, false, false, true, false, false);
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(listaSolp.GetType(), result.GetType());
        }

        [Test]
        public void DevolverMonedaProveedorOk()
        {
            obtenerProveedorConsumerMOA.Setup(x => x.ObtenerProveedor(It.IsAny<string>())).Returns(new ObtenerProveedorWSMOAResponse { MAIL = "bmelgarejo@test.com", NAME = "PARISI", CURRENCY = "ARP" });
            vendedoresConsumerMOAMock.Setup(x => x.Request(It.IsAny<string>(), (It.IsAny<List<SustitucionMOAModel.Models.FechaWS>>()))).Returns(new SustitucionMOAModel.Models.WSMapMOA.Vendedor.VendedoresWSMOAResponse
            { vendedores = new List<SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor> { new SustitucionMOAModel.Models.WSMapMOA.Vendedor.Vendedor { cuit = "232323" } } });

            var result = target.DevolverMonedaProveedor(It.IsAny<string>());
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ListarLiberadorSapOk()
        {
            var liberadorSapDto = new List<LiberadorSapDto> { new LiberadorSapDto { NombreCompleto = "Nombre", Cargo = "Cargo", Habilitado = true, LiberadorSapTipo_Id = 1 } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<LiberadorSap, LiberadorSapDto>>>(), It.IsAny<Expression<Func<LiberadorSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<RegionSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(regionesEsperadas);

            var resultado = target.ListarRegionesSap();

            Assert.That(resultado, Is.Not.Null);
            Assert.AreEqual(resultado.GetType(), regionesEsperadas.GetType());
        }

        [Test]
        public void ListarUnidadesDeMedidaOk()
        {
            string material = "materialCodigo";
            List<TablaSapDto> tablaSapDto = new List<TablaSapDto>();
            var tablaSap = new List<TablaSap> { new TablaSap { Codigo = "FINALIZADA", Tabla = "EstadoSolpSap", CodigoSap = "05", Descripcion = "Liberación concluida" } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(tablaSap);
            obtenerUnidadesDeMedidaAlternativasConsumerMOAMock.Setup(y => y.Request(It.IsAny<List<string>>())).Returns(new List<UnidadesDeMedida>
            { new UnidadesDeMedida { CodigoMaterial = "000000000050224373", UnidadDeMedida = "UNI", Denominador = 1, Numerador = 1 }});

            var liberadorSapDto = new List<LiberadorSapDto> { new LiberadorSapDto { NombreCompleto = "Nombre", Cargo = "Cargo", Habilitado = true, LiberadorSapTipo_Id = 1 } };
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<LiberadorSap, LiberadorSapDto>>>(), It.IsAny<Expression<Func<LiberadorSap, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<LiberadorSapDto>() { new LiberadorSapDto { NombreCompleto = "Nombre", Cargo = "Cargo", Habilitado = true, LiberadorSapTipo_Id = 1 } });

            var result = target.ListarUnidadesDeMedida(material);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), tablaSapDto.GetType());
        }

        [Test]
        public void EnviarMailSolpCreadasReporteOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(), It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<SolpDto>() { solpDtoToClone() });
            DateTime startDate = new DateTime(2023, 9, 1);
            DateTime endDate = DateTime.Now.Date;
            int monthsApart = (endDate.Year - startDate.Year) * 12 + (endDate.Month - startDate.Month + 1);
            target.ObtenerDatosReporteSolp();
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(),
                It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc), Times.Exactly(monthsApart));
        }

        [Test]
        public void ListarClaseDocumentoOk()
        {
            List<int> clasesDoc = new List<int>();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Solp>() { solpToClone() });
            var result = target.ListarClaseDocumento(1);

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.GetType(), clasesDoc.GetType());
        }

        [Test]
        public void ActualizarProveedorVisibleEnSolicitanteOk()
        {
            repositorioMock.Setup(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(new PeticionDeOfertaUsuario { Id = 1, VisibleSolicitante = true });
            var result = target.ActualizarProveedorVisibleEnSolicitante(It.IsAny<int>(), It.IsAny<bool>());
            repositorioMock.Setup(x => x.GuardarCambios());
            Assert.That(result, Is.Not.Null);
            repositorioMock.Verify(x => x.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void ListarUsuarioSolicitanteOk()
        {

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, UsuarioDto>>>(),
                It.IsAny<Expression<Func<Usuario, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
               .Returns(new List<UsuarioDto>() { new UsuarioDto { Mail = "bmelgarejo@prueba.com", UsuarioSap = "BRISAM" } });

            var result = target.ListarUsuarioSolicitante();
            Assert.That(result, Is.Not.Null);
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
                        Cotizacion = cotizacionToClone(),
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

            repositorioMock.Setup(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(peticionDeOfertaToClone());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionHistorial, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(cotizacionHistorialList);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaCierre, DateTime>>>(),
               It.IsAny<Expression<Func<PeticionDeOfertaCierre, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
              .Returns(new List<DateTime>() { DateTime.Now });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, SolpDto>>>(),
                It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
               .Returns(new List<SolpDto>() { solpDtoToClone() });

            var resultado = target.ListarHistorialDeFechas(peticionDeOfertaId);
            Assert.NotNull(resultado);
            repositorioMock.Verify(x => x.Obtener<PeticionDeOferta>(It.IsAny<int>()), Times.Once);

        }

        [Test]
        public void ListarPeticionesDeOfertaOk()
        {
            repositorioMock.Setup(x => x.Obtener<Solp>(It.IsAny<int>())).Returns(solpToClone());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, PeticionDeOfertaDto>>>(),
                It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc))
                .Returns(new List<PeticionDeOfertaDto> { new PeticionDeOfertaDto {  CotizacionId = 1,
                Cotizacion = new CotizacionDto { ArchivosCotizacion = null },
                TipoPosicionCodigo = "MATERIALES",
                Usuarios = new List<PeticionDeOfertaUsarioDto> { new PeticionDeOfertaUsarioDto { Cotizacion = new CotizacionDto { CotizacionPosiciones = new List<CotizacionPosicionDto> { new CotizacionPosicionDto {
                Id = 1, PeticionDeOfertaSolpPosicion_Id = 1, Cantidad = 2, Precio = 500, UnidadMedida = new TablaSapDto { Descripcion = "UNI" }, TotalPesos = 1000 } } } } },
                PeticionDeOfertaPosicion = new List<PeticionDeOfertaSolpPosicionDto> {
                    new PeticionDeOfertaSolpPosicionDto { Id = 1, Posicion = new SolpPosicionDto { Unidad = new TablaSapDto { Descripcion = "PAR" }, CodigoMaterialSap = new MaterialSolpDto { Codigo = "000000000050224373" } }, Posiciones = new SolpPosicionDto { Codigo = "000000000050224373" } }
                },
                NrosSolp = new List<string> { "102002020"}} });

            var resultado = target.ListarPeticionesDeOferta(It.IsAny<int>());
            Assert.NotNull(resultado);
            repositorioMock.Verify(x => x.Obtener<Solp>(It.IsAny<int>()), Times.Once);
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<ChatExternoCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<ChatExternoCompras>() { chatExterno });
            target.MarcarChatProveedorComoLeido(chatProveedor);

            this.repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }


        [Test]
        public void ListarSolpCondicionEspecialTestOk()
        {
            var filtro = new FiltroDto
            {
                Columna = "NroSolp"
            };
            var solpsDto = new List<SolpDto>(new List<SolpDto> { solpDtoToClone() });
            repositorioMock.Setup(y => y.ListarConsulta(It.IsAny<ListarSolpCondicionEspecialConsulta>())).Returns(solpsDto);
            target.ListarSolpCondicionEspecial(filtro);
            repositorioMock.Verify(y => y.ListarConsulta(It.IsAny<ListarSolpCondicionEspecialConsulta>()), Times.Once);
        }


        [Test]
        public void AgruparPeticionesDeOfertaOk()
        {

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
              .Returns(new List<PeticionDeOferta>() { peticionDeOfertaToClone() });
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Cotizacion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<Cotizacion>() { cotizacionToClone() });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionHistorial, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                .Returns(new List<CotizacionHistorial>() { new CotizacionHistorial { Cotizacion_Id = 1 } });

            target.AgruparPeticionesDeOferta(It.IsAny<int>(), It.IsAny<string>());

            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOferta, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null), Times.Once);
            repositorioMock.Verify(x => x.Agregar(It.IsAny<PeticionDeOferta>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(3));
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
            var solpLocal = solpToClone();
            var cotizacionLocal = cotizacionToClone();
            var cotizacionPosicion = cotizacionLocal.CotizacionPosiciones.ToList();
            cotizacionPosicion.ForEach(x => x.Cotizacion = cotizacionLocal);


            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(solpLocal.Posiciones.ToList());
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(cotizacionPosicion);

            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });
            cotizacionLocal.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo = "SERVICIOS";

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
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
            var solpLocal = solpToClone();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(solpLocal.Posiciones.ToList());

            var cotizacionLocal = cotizacionToClone();
            var cotizacionPosicion = cotizacionLocal.CotizacionPosiciones.ToList();
            cotizacionPosicion.ForEach(x => x.Cotizacion = cotizacionLocal);
            cotizacionPosicion.FirstOrDefault().CotizacionSubPosiciones.ToList().ForEach(x => x.Precio = 1000);

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<CotizacionPosicion, bool>>>(),
           It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(cotizacionPosicion);

            obtenerTipoCambioConsumerMOAMock.Setup(y => y.Request(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new ObtenerTipoCambioConsumerMOAResponse { MonedaDestino = "ARP", MonedaOrigen = "USD", TipoCambio = 450 });
            cotizacionLocal.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.FirstOrDefault().SolpPosicion.TipoPosicion.Codigo = "SERVICIOS";
            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>()))
                .Returns(cotizacionLocal);
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });

            var result = target.ValidarPrecioCotizado(adjudicacionDto);
            Assert.That(result.Errores.Count > 0);

        }

        [Test]
        public void ListarUsuarioCompras_DebeRetornarListaDeUsuarioComprasDto()
        {
            // Arrange
            var usuariosComprasMockData = new List<UsuarioCompras>
            {
                new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(usuariosComprasMockData);

            // Act
            var resultado = target.ListarUsuarioCompras();

            // Assert
            Assert.NotNull(resultado);
            Assert.AreEqual(usuariosComprasMockData.Count, resultado.Count);
        }

        [Test]
        public void DesagruparPOOk()
        {
            var posicion = new SolpPosicion
            {
                Id = 1,
                FechaEntregaServicio = DateTime.Now,
                Solp = solpToClone()
            };
            var peticionDeOfertaLocal = peticionDeOfertaToClone();
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null))
                           .Returns(new List<SolpPosicion>() { posicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpPosicion, bool>>>(),
              It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpPosicion>() { posicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<SolpSubposicion, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<SolpSubposicion>() { subposicion });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<PeticionDeOfertaSolpPosicion, bool>>>(),
            It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<PeticionDeOfertaSolpPosicion>() { new PeticionDeOfertaSolpPosicion { Id = 1, SolpPosicion = new SolpPosicion { TipoPosicion = new TablaGeneral { Codigo = "SERVICIOS" } } } });

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Usuario, bool>>>(),
             It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<Usuario>() { new Usuario { Id = 1 } });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<PeticionDeOferta>())).Returns(new PeticionDeOferta { Id = 1 });

            repositorioMock.Setup(y => y.Agregar(It.IsAny<Cotizacion>())).Returns(cotizacionToClone());
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>()))
                 .Returns(new Usuario { Id = 1, Mail = "bmelgarejo", CUITRegistro = "2373739293", TipoUsuario = new TipoUsuario { Id = 1 }, Proveedores = new List<Proveedor> { new Proveedor { CUIT = "2373739293", TipoProveedor = new TipoUsuario { Id = 1 } } } });

            repositorioMock.Setup(y => y.Obtener<PeticionDeOfertaUsuario>(It.IsAny<int>())).Returns(peticionDeOfertaUsuario);

            repositorioMock.Setup(y => y.Obtener<PeticionDeOferta>(It.IsAny<int>())).Returns(peticionDeOfertaToClone());

            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionToClone());

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(x => x.Listar<Usuario>(null, 0, null, DirOrden.Asc, null)).Returns(new List<Usuario> { new Usuario { Id = 1, Mail = "drodriguez@prueba.com" } });

            var resultado = target.DesagruparPO(It.IsAny<string>(), It.IsAny<string>());
            this.repositorioMock.Verify(x => x.GuardarCambios(), Times.Exactly(7));
        }

        [Test]
        public void GenerarSolpPdf_DebeRetornarArrayDeBytes()
        {
            // Arrange
            int idSolp = 1;
            var solpLocal = solpToClone();

            var testImagePath = TestContext.CurrentContext.TestDirectory + "\\Util\\LogoBaufest.png";
            var testImage = iTextSharp.text.Image.GetInstance(testImagePath);
            httpContextServiceMock.Setup(y => y.ObtenerLogoImagen()).Returns(testImage);


            repositorioMock.Setup(y => y.Obtener(It.IsAny<IEnumerable<Expression<Func<Solp, object>>>>(), It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Proveedores = new List<Proveedor>() });

            var usuariosComprasMockData = new List<UsuarioCompras>
            {
                new UsuarioCompras { Id = 1, Mail = "bmelgarejo@prueba.com" },
            };

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(usuariosComprasMockData);

            var templateHtml = "<html><body>{{NOMBRE_OBRA}}</body></html>";
            var templateCss = "body { font-family: Arial; }";

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

            var solpLocal = solpToClone();
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

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<UsuarioCompras, bool>>>(), It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(usuariosComprasMockData);
            repositorioMock.Setup(y => y.Obtener(It.IsAny<IEnumerable<Expression<Func<Solp, object>>>>(), It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solpLocal);
            repositorioMock.Setup(y => y.Obtener<Usuario>(It.IsAny<int>())).Returns(new Usuario { Proveedores = new List<Proveedor>() });
            repositorioMock.Setup(y => y.Obtener(It.IsAny<Expression<Func<Solp, bool>>>()))
             .Returns(solpLocal);

            repositorioMock.Setup(y => y.Obtener<Cotizacion>(It.IsAny<int>())).Returns(cotizacionToClone());

            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<TablaSap, bool>>>(),
                It.IsAny<int>(), It.IsAny<string>(), DirOrden.Asc, null)).Returns(new List<TablaSap>() { new TablaSap { CodigoSap = "ARP", Id = 1 } });
            repositorioMock.Setup(x => x.Listar<Usuario>(null, 0, null, DirOrden.Asc, null)).Returns(new List<Usuario> { new Usuario { Id = 1, Mail = "drodriguez@prueba.com" } });

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
                Mensaje = "Se grabo con exito"
            };

            repositorioMock.Setup(r => r.Obtener<Solp>(It.IsAny<Expression<Func<Solp, bool>>>())).Returns(solp);
            repositorioMock.Setup(r => r.GuardarCambios());

            // Act
            var resultado = target.GuardarEnvioCircularProveedor(id, envioCircularA, fechaLimite);

            // Assert
            DateTime fechalimiteEsperada = new DateTime(fechaLimite.Year, fechaLimite.Month, fechaLimite.Day, 23, 59, 59, DateTimeKind.Local);

            Assert.AreEqual(resultadoEsperado.IdEntidad, resultado.IdEntidad);
            Assert.AreEqual(resultadoEsperado.Mensaje, resultado.Mensaje);
            Assert.AreEqual(envioCircularA, solp.EnvioCircularA);
            Assert.AreEqual(fechalimiteEsperada, solp.FechaLimiteReenvioDocumentacionPorCambioCondiciones);
            repositorioMock.Verify(r => r.Obtener<Solp>(It.IsAny<Expression<Func<Solp, bool>>>()), Times.Once);
            repositorioMock.Verify(r => r.GuardarCambios(), Times.Once);
        }
    }
}
