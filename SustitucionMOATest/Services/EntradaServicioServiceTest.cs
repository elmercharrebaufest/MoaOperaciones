using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAUtils.Services.Email.Dto;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class EntradaServicioServiceTest
    {
        private Mock<IRepositorioEntradaServicio> mIRepositorioEntradaServicio;
        private Mock<IComprasService> mComprasService;
        private Mock<IEmailCertificationService> mEmailCertificationService;
        private Mock<IObtenerOrdenDeCompraConsumerMOA> mObtenerOrdenDeCompraConsumerMOA;
        private Mock<IReporteESService> mReporteESService;
        private Mock<OrderService> mOrderService;

        private EntradaServicioService target;

        [SetUp]
        public void SetUp()
        {
            mIRepositorioEntradaServicio = new Mock<IRepositorioEntradaServicio>();
            mComprasService = new Mock<IComprasService>();
            mEmailCertificationService = new Mock<IEmailCertificationService>();
            mObtenerOrdenDeCompraConsumerMOA = new Mock<IObtenerOrdenDeCompraConsumerMOA>();
            mReporteESService = new Mock<IReporteESService>();
            mOrderService = new Mock<OrderService>(mIRepositorioEntradaServicio.Object);

            target = new EntradaServicioService(
                mIRepositorioEntradaServicio.Object,
                mOrderService.Object,
                mComprasService.Object,
                mEmailCertificationService.Object,
                mObtenerOrdenDeCompraConsumerMOA.Object,
                mReporteESService.Object
            );
        }

        [Test]
        public void CrearEntradaServicio_ValidarRemitoYaFueUsado()
        {
            var nroRemito = "9898R12341234";
            var proveedorCodigo = "0057984261";

            var parametros = new CreateEntradaServicioDto
            {
                Posiciones = new List<EntradaServicioCreateParamsDto>
                {
                    new EntradaServicioCreateParamsDto
                    {
                        EntrySheetHeader = new EntrySheetHeaderSection
                        {
                            DocumentoReferenciaNumero = nroRemito,
                            SolPedNumber = "12345",
                            MontoTotalACertificar = "1000",
                            Proveedor = proveedorCodigo
                        },
                        EntrySheetServices = new EntrySheetServiceSection
                        {
                            Items = new List<EntrySheetServiceItemSection>
                            {
                                new EntrySheetServiceItemSection
                                {
                                    Service = "Servicio Test",
                                    Quantity = "10",
                                    UM = "UNI",
                                    ItemGrossPrice = "100"
                                }
                            }
                        }
                    }
                }
            };

            mIRepositorioEntradaServicio
                .Setup(x => x.ExisteRemitoActivoParaProveedor(
                    It.Is<string>(r => r == nroRemito),
                    It.Is<string>(p => p == proveedorCodigo)))
                .Returns(true);

            var ex = Assert.Throws<ValidationCustomException>(() => target.CrearEntradaServicio(parametros, "mail@bauf.com"));
            Assert.AreEqual($"El remito {nroRemito} ya fue utilizado para el proveedor {proveedorCodigo}", ex.Message);
        }

        [Test]
        public void CrearEntradaServicio_AutomaticaPorSuplenteAprobadorIgualAIngresante()
        {
            var mailUsuarioIngresante = "mailUsrIngr@bauf.com";
            var idUsuarioIngresante = 456;
            var mailUsuarioFiscal = "mailUsrFiscal@bauf.com";
            var nroRemito = "9898R12341234";
            var proveedorCodigo = "0057984261";
            var nroSolp = "1012345";

            var usuarioIngresante = new Usuario
            {
                Externo = false,
                Id = idUsuarioIngresante
            };
            var reasignacionUsuarioIngresante = new UsuarioReasignacion
            {
                Usuario_Id = idUsuarioIngresante
            };
            var usuarioFiscal = new Usuario
            {
                Mail = mailUsuarioFiscal
            };

            var crearEDRequest = new CreateEntradaServicioDto
            {
                Posiciones = new List<EntradaServicioCreateParamsDto>
                {
                    new EntradaServicioCreateParamsDto
                    {
                        EntrySheetHeader = new EntrySheetHeaderSection
                        {
                            DocumentoReferenciaNumero = nroRemito,
                            Proveedor = proveedorCodigo,
                            SolPedNumber = nroSolp,
                            FechaDocumento = new DateTime(2025, 8, 17, 0, 0, 0, DateTimeKind.Local).ToString("yyyy-MM-dd"),
                            FechaContabilizacion = new DateTime(2025, 8, 17, 0, 0, 0, DateTimeKind.Local).ToString("yyyy-MM-dd"),
                            OrdenCompraNumero = "4500012345",
                            OrdenCompraPosicionNumero = "000010",
                            MontoTotalACertificar = "5000"
                        },
                        EntrySheetServices = new EntrySheetServiceSection
                        {
                            Items = new List<EntrySheetServiceItemSection>
                            {
                                new EntrySheetServiceItemSection
                                {
                                    Descripcion = "Servicio test",
                                    ItemQuantity = "100",
                                    ItemGrossPrice = "500000",
                                    ExternalLineNumber = "100",
                                    Service = "SERVICIO001",
                                    ShortText = "Servicio test",
                                    UM = "UNI",
                                    Quantity = "10",
                                    Percentage = "20",
                                    PlannedPackage = "ABCX",
                                    PlannedLine = "0001",
                                    CertificationAmount = "10"
                                }
                            }
                        }
                    }
                },
                report = new List<ReporteDto>(),
                IdAdjuntos = new List<string>(),
            };

            var solpEsDto = new SolpESDto
            {
                Email = mailUsuarioFiscal
            };

            mIRepositorioEntradaServicio
                .Setup(x => x.ExisteRemitoActivoParaProveedor(
                    It.Is<string>(r => r == nroRemito),
                    It.Is<string>(p => p == proveedorCodigo)))
                .Returns(false);

            mComprasService
                .Setup(x => x.TraerSolpPorNumero(
                    It.Is<string>(s => s == nroSolp)))
                .Returns(solpEsDto);

            mIRepositorioEntradaServicio
                .Setup(x => x.GetUsuarioPorMail(
                    It.Is<string>(m => m == mailUsuarioIngresante)))
                .Returns(usuarioIngresante);

            mIRepositorioEntradaServicio
                .Setup(x => x.GetUsuarioPorMail(
                    It.Is<string>(m => m == mailUsuarioFiscal)))
                .Returns(usuarioFiscal);

            mIRepositorioEntradaServicio
                .Setup(x => x.GetReasignacion(
                    It.Is<int>(id => id == usuarioIngresante.Id)))
                .Returns(reasignacionUsuarioIngresante);

            mIRepositorioEntradaServicio
                .Setup(x => x.ObtenerMailSuplenteSegunFecha(
                    It.Is<string>(m => m == mailUsuarioFiscal),
                    It.IsAny<DateTime>()))
                .Returns(mailUsuarioIngresante);

            mIRepositorioEntradaServicio
                .Setup(x => x.ObtenerMailSuplenteSegunFecha(
                    It.Is<string>(m => m == mailUsuarioIngresante),
                    It.IsAny<DateTime>()))
                .Returns("");

            mIRepositorioEntradaServicio
                .Setup(x => x.ObtenerSiguienteValorSecuencia())
                .Returns(987654);

            mIRepositorioEntradaServicio
                .Setup(x => x.Agregar(It.IsAny<Aprobaciones>()));

            mEmailCertificationService
                .Setup(x => x.EnviarMailAprobacion(It.IsAny<MailAprobacionESRequest>()));

            var sapCrearESResult = new EntradaServicioCreateRespuestaDto
            {
                Type = "I",
                Id = "SE",
                Message = "1132"
            };

            var mCrearEntradaDeServicioConsumerMOA = new Mock<CrearEntradaDeServicioConsumerMOA>();
            mCrearEntradaDeServicioConsumerMOA
                .Setup(x => x.CrearEntradaServicio(
                    It.IsAny<EntradaServicioCreateParamsDto>()))
                .Returns(sapCrearESResult);

            target.crearEntradaDeServicioConsumer = mCrearEntradaDeServicioConsumerMOA.Object;
            var respuestasCreacion = target.CrearEntradaServicio(crearEDRequest, mailUsuarioIngresante);

            mIRepositorioEntradaServicio
                .Verify(x => x.Agregar(It.Is<Aprobaciones>(a =>
                    a.Cantidad == 100 &&
                    a.Monto == 500000 &&
                    a.Nro_linea == "100" &&
                    a.Nro_servicio == "SERVICIO001")), Times.Once);

            Assert.AreEqual(1, respuestasCreacion.Count);
            Assert.AreEqual("I", respuestasCreacion[0].Type);
            Assert.AreEqual("SE", respuestasCreacion[0].Id);
        }
    }
}
