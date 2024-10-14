using Moq;
using NUnit.Framework;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSRequests.AplicacionCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class AplicacionCartaPorteTest
    {
        private Mock<IRepositorio> repositorio;
        private Mock<IAplicacionCartaPorteConsumer> consumer;
        private Mock<IEmailAplicacionCPService> mIEmailAplicacionCPService;
        private IAplicacionCartaPorteService aplicacionCCPPService;


        private ZMPES7070 contratoListado;
        private ZMPES7070 cartaPorteListada;

        [SetUp]
        public void SetUp()
        {
            repositorio = new Mock<IRepositorio>();
            consumer = new Mock<IAplicacionCartaPorteConsumer>();
            mIEmailAplicacionCPService = new Mock<IEmailAplicacionCPService>();
            aplicacionCCPPService = new AplicacionCartaPorteService(repositorio.Object, consumer.Object, mIEmailAplicacionCPService.Object);
        }

        [Test]
        public void EliminarAplicacion_IdNoExiste_ShouldThrowInfoError()
        {
            repositorio.Setup(repositorio =>
                repositorio.Obtener<AplicacionCartaPorte>(It.IsAny<int>())
            ).Returns(null as AplicacionCartaPorte);

            Assert.That(
                () => aplicacionCCPPService.EliminarAplicacion(It.IsAny<int>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        [TestCase(EstadoAplicacionCartaPorte.Aplicado)]
        [TestCase(EstadoAplicacionCartaPorte.Eliminado)]
        [TestCase(EstadoAplicacionCartaPorte.Error)]
        public void EliminarAplicacion_IdExisteAplicacionEstadoDiferenteAPendiente_ShouldThrowInfoError(EstadoAplicacionCartaPorte estado)
        {
            repositorio.Setup(repositorio =>
                repositorio.Obtener<AplicacionCartaPorte>(It.IsAny<int>())
            ).Returns(new AplicacionCartaPorte { Estado = estado });

            Assert.That(
                () => aplicacionCCPPService.EliminarAplicacion(It.IsAny<int>()),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void EliminarAplicacion_IdExisteAplicacionEstadoAPendiente_ShouldChangeEstadoToEliminado()
        {
            var aplicacionAEliminar = new AplicacionCartaPorte { Estado = EstadoAplicacionCartaPorte.Pendiente };
            repositorio.Setup(repositorio =>
                repositorio.Obtener<AplicacionCartaPorte>(It.IsAny<int>())
            ).Returns(aplicacionAEliminar);
            aplicacionCCPPService.EliminarAplicacion(It.IsAny<int>());
            Assert.That(aplicacionAEliminar.Estado, Is.EqualTo(EstadoAplicacionCartaPorte.Eliminado));
        }
        [Test]
        public void GuardarAplicacion_TodoValido_CreaAplicacion()
        {
            var mailUsuario = "tester@baufest.com";
            var material = "0012";
            var codigoProveedor = "49012";
            var kgMaximo = 4500;

            SetContratoListado(codigoProveedor, material);
            SetCartaPorteListada(kgMaximo, material);


            var contratoSeleccionado = new ContratoParaAplicacionCartaPorte(contratoListado.CONTRATO, contratoListado.MATERIAL, contratoListado.PROVEEDOR);
            var cartaPorteSeleccionada = new CartaPorteParaAplicacionCartaPorte(cartaPorteListada.CCPP, cartaPorteListada.CANTIDAD, cartaPorteListada.MATERIAL);

            var aplicacionesRespuestaSAP = new ZMPES7070[] { contratoListado, cartaPorteListada };
            SetupGuardarAplicacion(mailUsuario, codigoProveedor, aplicacionesRespuestaSAP);
            consumer.Setup(c => c.ObtenerPendientesDeAplicar(It.IsAny<AppCartasPortePendienteRequest>())).Returns(new AppCartasPortePendienteResponse
            {
                CartasDePorte = new List<AplicacionPendienteCartaPorte> { new AplicacionPendienteCartaPorte { Cantidad = 1, Material = "0012", NumeroCartaPorte = "123142" } },
                Contratos = new List<AplicacionPendienteContrato> { new AplicacionPendienteContrato { NumeroContrato = "1" } }
            });

            var aplicacionAGuardar = new CrearAplicacionCartaPorte
            {
                ContratoSeleccionado = contratoSeleccionado,
                CartaPorteSeleccionada = cartaPorteSeleccionada,
                Kilogramos = kgMaximo
            };
            aplicacionCCPPService.GuardarAplicacion(aplicacionAGuardar, mailUsuario);

            repositorio.Verify(r => r.Agregar(It.IsAny<AplicacionCartaPorte>()), Times.Once);
            repositorio.Verify(r => r.GuardarCambios(), Times.Once);
        }

        [Test]
        public void GuardarAplicacion_MasKgQueDisponibles_ThrowErrorInfoValidacion()
        {
            var mailUsuario = "tester@baufest.com";
            var codigoProveedor = "49012";
            var material = "0012";
            var kgMaximo = 4500;

            SetContratoListado(codigoProveedor, material);
            SetCartaPorteListada(kgMaximo, material);


            var contratoSeleccionado = new ContratoParaAplicacionCartaPorte(contratoListado.CONTRATO, contratoListado.MATERIAL, contratoListado.PROVEEDOR);
            var cartaPorteSeleccionada = new CartaPorteParaAplicacionCartaPorte(cartaPorteListada.CCPP, cartaPorteListada.CANTIDAD, cartaPorteListada.MATERIAL);

            var aplicacionesRespuestaSAP = new ZMPES7070[] { contratoListado, cartaPorteListada };
            SetupGuardarAplicacion(mailUsuario, codigoProveedor, aplicacionesRespuestaSAP);


            var aplicacionAGuardar = new CrearAplicacionCartaPorte
            {
                ContratoSeleccionado = contratoSeleccionado,
                CartaPorteSeleccionada = cartaPorteSeleccionada,
                Kilogramos = 5000
            };

            Assert.That(
                () => aplicacionCCPPService.GuardarAplicacion(aplicacionAGuardar, mailUsuario),
                Throws.TypeOf<InfoCustomException>());
        }
        [Test]
        public void GuardarAplicacion_ContratoNoEstaEnRespuestaDeSAP_ThrowErrorInfoValidacion()
        {
            var mailUsuario = "tester@baufest.com";
            var codigoProveedor = "49012";
            var material = "0012";
            var kgMaximo = 4500;

            SetContratoListado(codigoProveedor, material);
            SetCartaPorteListada(kgMaximo, material);


            var contratoSeleccionado = new ContratoParaAplicacionCartaPorte(contratoListado.CONTRATO, contratoListado.MATERIAL, contratoListado.PROVEEDOR);
            var cartaPorteSeleccionada = new CartaPorteParaAplicacionCartaPorte(cartaPorteListada.CCPP, cartaPorteListada.CANTIDAD, cartaPorteListada.MATERIAL);

            var aplicacionesRespuestaSAP = new ZMPES7070[] { cartaPorteListada };
            SetupGuardarAplicacion(mailUsuario, codigoProveedor, aplicacionesRespuestaSAP);
            consumer.Setup(c => c.ObtenerPendientesDeAplicar(It.IsAny<AppCartasPortePendienteRequest>())).Returns(new AppCartasPortePendienteResponse
            {
                CartasDePorte = new List<AplicacionPendienteCartaPorte> { new AplicacionPendienteCartaPorte { } },
                Contratos = new List<AplicacionPendienteContrato> { new AplicacionPendienteContrato { } }
            });

            var aplicacionAGuardar = new CrearAplicacionCartaPorte
            {
                ContratoSeleccionado = contratoSeleccionado,
                CartaPorteSeleccionada = cartaPorteSeleccionada,
                Kilogramos = kgMaximo
            };

            Assert.That(
                () => aplicacionCCPPService.GuardarAplicacion(aplicacionAGuardar, mailUsuario),
                Throws.TypeOf<InfoCustomException>());
        }        
        [Test]
        public void GuardarAplicacion_MaterialesDiferentesEnCartaPorteContrato_ThrowErrorInfoValidacion()
        {
            var mailUsuario = "tester@baufest.com";
            var codigoProveedor = "49012";
            var material = "0012";
            var materialDistinto = "12300";
            var kgMaximo = 4500;

            SetContratoListado(codigoProveedor, materialDistinto);
            SetCartaPorteListada(kgMaximo, material);


            var contratoSeleccionado = new ContratoParaAplicacionCartaPorte(contratoListado.CONTRATO, contratoListado.MATERIAL, contratoListado.PROVEEDOR);
            var cartaPorteSeleccionada = new CartaPorteParaAplicacionCartaPorte(cartaPorteListada.CCPP, cartaPorteListada.CANTIDAD, cartaPorteListada.MATERIAL);

            var aplicacionesRespuestaSAP = new ZMPES7070[] { contratoListado, cartaPorteListada };
            SetupGuardarAplicacion(mailUsuario, codigoProveedor, aplicacionesRespuestaSAP);
            consumer.Setup(c => c.ObtenerPendientesDeAplicar(It.IsAny<AppCartasPortePendienteRequest>())).Returns(new AppCartasPortePendienteResponse
            {
                CartasDePorte = new List<AplicacionPendienteCartaPorte> { new AplicacionPendienteCartaPorte { } },
                Contratos = new List<AplicacionPendienteContrato> { new AplicacionPendienteContrato { } }
            });

            var aplicacionAGuardar = new CrearAplicacionCartaPorte
            {
                ContratoSeleccionado = contratoSeleccionado,
                CartaPorteSeleccionada = cartaPorteSeleccionada,
                Kilogramos = kgMaximo
            };

            Assert.That(
                () => aplicacionCCPPService.GuardarAplicacion(aplicacionAGuardar, mailUsuario),
                Throws.TypeOf<InfoCustomException>());
        }
        
        
        private void SetContratoListado(string codigoProveedor, string material)
        {
            contratoListado = new ZMPES7070 { CONTRATO = "1", PROVEEDOR = codigoProveedor, MATERIAL = material };
        }
        private void SetCartaPorteListada(decimal kgMaximo, string material)
        {
            cartaPorteListada = new ZMPES7070 { CCPP = "123142", CANTIDAD = kgMaximo, MATERIAL = material };
        }
      
        private void SetupGuardarAplicacion(string mailUsuario, string codigoProveedor, ZMPES7070[] aplicacionesRespuestaSAP, List<AplicacionCartaPorte> aplicacionesCargadas = null)
        {
            repositorio.Setup(r => r.Obtener<Usuario>(u => u.Mail == mailUsuario)).Returns(
                new Usuario
                {
                    Roles = new Rol[] {
                        new Rol
                        {
                            PermisosAsociados = new PermisoPorRol[]
                            {
                                new PermisoPorRol {Permiso =  "SELECCIONAR VENDEDOR"}
                            }
                        }
                    },
                }
            );
            consumer.Setup(c => c.ObtenerAplicacionesPendientes(It.IsAny<AppCartasPortePendienteRequest>())).Returns(aplicacionesRespuestaSAP);
            consumer.Setup(c => c.ObtenerPendientesDeAplicar(It.IsAny<AppCartasPortePendienteRequest>())).Returns(new AppCartasPortePendienteResponse
            {
                Contratos = new List<AplicacionPendienteContrato> { new AplicacionPendienteContrato { NumeroContrato = "1" } }
            });

            repositorio.Setup(r => r.Listar(It.IsAny<Expression<Func<AplicacionCartaPorte, bool>>>(), 0, null, DirOrden.Asc, null)).Returns(
                aplicacionesCargadas ?? new List<AplicacionCartaPorte>()
            );
            repositorio.Setup(r => r.Obtener(It.IsAny<Expression<Func<Proveedor, bool>>>())).Returns(
                new Proveedor
                {
                    CodigoProveedor = codigoProveedor
                }
            );

        }
    }
}
