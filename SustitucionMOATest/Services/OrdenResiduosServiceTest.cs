using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAUtils.Services.Email;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class OrdenResiduosServiceTest
    {
        private Mock<IRepositorioOrdenResiduos> mIRepositorioOrdenResiduos;
        private Mock<IScatoRepositorioClient> mIScatoRepositorioClient;
        private Mock<IOrdenCargaConsumerMOA> mIOrdenCargaConsumerMOA;
        private Mock<IEmailResiduosService> mIEmailResiduosService;
        private Mock<ICNRTClient> mICNRTClient;
        private Mock<IFeriadoService> mIFeriadoService;
        private Mock<IScatoConsumer> mIScatoConsumer;
        private IOrdenResiduosService target;

        [SetUp]
        public void SetUp()
        {
            mIRepositorioOrdenResiduos = new Mock<IRepositorioOrdenResiduos>();
            mIScatoRepositorioClient = new Mock<IScatoRepositorioClient>();
            mIOrdenCargaConsumerMOA = new Mock<IOrdenCargaConsumerMOA>();
            mICNRTClient = new Mock<ICNRTClient>();
            mIFeriadoService = new Mock<IFeriadoService>();
            mIEmailResiduosService = new Mock<IEmailResiduosService>();
            mIScatoConsumer = new Mock<IScatoConsumer>();
            target = new OrdenResiduosService(
                mIOrdenCargaConsumerMOA.Object,
                mIScatoConsumer.Object,
                mIScatoRepositorioClient.Object,
                mIRepositorioOrdenResiduos.Object,
                mICNRTClient.Object,
                mIFeriadoService.Object,
                mIEmailResiduosService.Object);
        }

        [Test]
        public void ObtenerMateriales_Ok()
        {
            var materialesRepo = new MaterialDto[]
            {
                new MaterialDto { MaterialId = 3, Descripcion = "Residuo1" },
                new MaterialDto { MaterialId = 4, Descripcion = "Insumo1" }
            };

            mIRepositorioOrdenResiduos
                .Setup(x => x.ObtenerMateriales())
                .Returns(materialesRepo);

            var resp = target.ObtenerMateriales();

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            mIRepositorioOrdenResiduos
                .Verify(x => x.ObtenerMateriales(), Times.Once);
        }

        [Test]
        public void ObtenerClientes_Ok()
        {
            var clientesRepo = new List<ProveedorDto>
            {
                new ProveedorDto { Id = 1, RazonSocial = "Cliente1" },
                new ProveedorDto { Id = 2, RazonSocial = "Cliente2" }
            };

            mIRepositorioOrdenResiduos
                .Setup(x => x.ObtenerClientesResiduos())
                .Returns(clientesRepo);

            var resp = target.ObtenerClientes();

            Assert.IsNotNull(resp);
            Assert.AreEqual(2, resp.Count());
            mIRepositorioOrdenResiduos
                .Verify(x => x.ObtenerClientesResiduos(), Times.Once);
        }

        [Test]
        public void ObtenerListadoOrdenes_Ok()
        {
            var fechaInicio = new DateTime(2024, 3, 1);
            var fechaFin = new DateTime(2024, 3, 7);
            var mailUsuario = "carlos@baufest.com";

            var usuario = new Usuario
            {
                Mail = mailUsuario,
                Roles = new List<Rol> { new Rol { PermisosAsociados =  new List<PermisoPorRol> { new PermisoPorRol { Permiso = "VER ORDENES DE CARGA RESIDUOS ADMIN" } } } }
            };

            var filas = new List<OrdenResiduosFila>
            {
                new OrdenResiduosFila { Id = 1, RazonSocialCliente = "Cliente34" },
                new OrdenResiduosFila { Id = 3, RazonSocialCliente = "Cliente75" }
            };

            mIRepositorioOrdenResiduos
                .Setup(x => x.ObtenerUsuarioSegunMail(It.Is<string>(m => m == mailUsuario)))
                .Returns(usuario);

            mIRepositorioOrdenResiduos
                .Setup(x => x.ObtenerListadoOrdenes(
                    It.Is<DateTime>(fi => fi == fechaInicio),
                    It.Is<DateTime>(ff => ff == fechaFin),
                    true))
                .Returns(filas);

            var listadoRes = target.ObtenerListadoOrdenes(fechaInicio.ToString(), fechaFin.ToString(), mailUsuario);

            Assert.IsNotNull(listadoRes);
            Assert.That(listadoRes.ListaOrdenes.Count, Is.EqualTo(2));
            mIRepositorioOrdenResiduos
                .Verify(x => x.ObtenerListadoOrdenes(
                    It.Is<DateTime>(fi => fi == fechaInicio),
                    It.Is<DateTime>(ff => ff == fechaFin),
                    true), Times.Once);
        }

        [Test]
        public void ObtenerListadoOrdenes_NoHayNinguna()
        {
            var fechaInicio = new DateTime(2024, 3, 1);
            var fechaFin = new DateTime(2024, 3, 7);
            var mailUsuario = "carlos@baufest.com";

            var usuario = new Usuario
            {
                Mail = mailUsuario,
                Roles = new List<Rol> { new Rol { PermisosAsociados = new List<PermisoPorRol> { new PermisoPorRol { Permiso = "VER ORDENES DE CARGA RESIDUOS ADMIN" } } } }
            };
            mIRepositorioOrdenResiduos
               .Setup(x => x.ObtenerUsuarioSegunMail(It.Is<string>(m => m == mailUsuario)))
               .Returns(usuario);

            var filas = new List<OrdenResiduosFila>();

            mIRepositorioOrdenResiduos
                .Setup(x => x.ObtenerListadoOrdenes(
                    It.Is<DateTime>(fi => fi == fechaInicio),
                    It.Is<DateTime>(ff => ff == fechaFin),
                    true))
                .Returns(filas);

            Assert.Throws<InfoCustomException>(() => target.ObtenerListadoOrdenes(fechaInicio.ToString(), fechaFin.ToString(), mailUsuario));
            mIRepositorioOrdenResiduos
                .Verify(x => x.ObtenerListadoOrdenes(
                    It.Is<DateTime>(fi => fi == fechaInicio),
                    It.Is<DateTime>(ff => ff == fechaFin),
                    true), Times.Once);
        }
    }
}
