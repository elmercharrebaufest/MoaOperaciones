using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasSolicitanteServiceTest
    {
        private ComprasSolicitanteService target;
        private Mock<IRepositorio> repositorioMock;

        private Mock<IComprasService> comprasServiceMock;
        private Mock<IComprasSapService> comprasServiceSapMock;
        private Mock<IRegistroInfoService> registroInfoServiceMock;
        private Mock<IUnidadMedidaService> unidadMedidaServiceMock;

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            comprasServiceMock = new Mock<IComprasService>();
            comprasServiceSapMock = new Mock<IComprasSapService>();
            registroInfoServiceMock = new Mock<IRegistroInfoService>();
            unidadMedidaServiceMock = new Mock<IUnidadMedidaService>();
            target = new ComprasSolicitanteService(repositorioMock.Object,
                                                   comprasServiceMock.Object,
                                                   comprasServiceSapMock.Object,
                                                   registroInfoServiceMock.Object,
                                                   unidadMedidaServiceMock.Object);
        }

        #region private methods
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
        #endregion

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
        public void ObtenerUltimaSolpOk()
        {
            repositorioMock.Setup(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                DirOrden.Asc, null)).Returns(new List<Solp>() { SolpToClone() });

            var result = target.ObtenerUltimaSolp(It.IsAny<int>());
            repositorioMock.Verify(y => y.Listar(It.IsAny<Expression<Func<Solp, bool>>>(), It.IsAny<int>(), It.IsAny<string>(),
                DirOrden.Asc, null), Times.Once);
            Assert.That(result, Is.Not.Null);
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
    }
}
