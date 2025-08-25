using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.Interfaces;
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
    }
}
