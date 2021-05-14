using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.LogPesificacion;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Validadores;
using SustitucionMOAUtils.Services;
using SustitucionMOAWS.DataAgroServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    public class LogPesificacionServiceTest
    {
        private ILogPesificacionService service;
        private Mock<IValidadorPesificacion> validador;
        private Mock<IRepositorio> repositorioMock;


        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            validador = new Mock<IValidadorPesificacion>();
            service = new LogPesificacionService(repositorioMock.Object, validador.Object);
        }

        [Test]
        public void ObtenerPesificacionesManualesTest_OK()
        {
            var pesificaciones = new List<LogPesificacion> {
                new LogPesificacion {
                Id = 1,
                IdUsuario= 1,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = false

                },
                new LogPesificacion {
                Id = 2,
                IdUsuario= 2,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = false
                }
            };

            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });



            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
               .Returns(pesificaciones);

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            var lista = service.ObtenerPesificacionesManuales(1);

            Assert.IsNotNull(lista);
            Assert.IsInstanceOf<List<LogPesificacionDto>>(lista);
            Assert.IsTrue(lista.Any());
        }

        [Test]
        public void ObtenerPesificacionesAutomaticasTest_OK()
        {
            var pesificaciones = new List<LogPesificacion> {
                new LogPesificacion {
                Id = 1,
                IdUsuario= 1,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = true

                },
                new LogPesificacion {
                Id = 2,
                IdUsuario= 2,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = true
                }
            };

            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });



            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
               .Returns(pesificaciones);

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            var lista = service.ObtenerPesificacionesAutomaticas(1);

            Assert.IsNotNull(lista);
            Assert.IsInstanceOf<List<LogPesificacionDto>>(lista);
            Assert.IsTrue(lista.Any());
        }

        [Test]
        public void ObtenerPesificacionesAutomaticasTest_NoAdmin_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "NoADM" } }
               });

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            Assert.Throws(Is.TypeOf<InfoCustomException>()
               .And.Message.EqualTo("El usuario no es administrador."),
             () => service.ObtenerPesificacionesAutomaticas(1));
        }

        [Test]
        public void ObtenerPesificacionesManualesTest_NoAdmin_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "NoADM" } }
               });

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            Assert.Throws(Is.TypeOf<InfoCustomException>()
               .And.Message.EqualTo("El usuario no es administrador."),
             () => service.ObtenerPesificacionesManuales(1));
        }

        [Test]
        public void ObtenerPesificacionesAutomaticasTest_Empty_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });

            repositorioMock
            .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                             It.IsAny<int>(),
                             It.IsAny<string>(),
                             It.IsAny<DirOrden>(),
                             It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
            .Returns(new List<LogPesificacion>());


            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            Assert.Throws(Is.TypeOf<InfoCustomException>()
               .And.Message.EqualTo(String.Format(InfoMsg.SinRegistros, "Pesificaciones")),
             () => service.ObtenerPesificacionesAutomaticas(1));
        }

        [Test]
        public void ObtenerPesificacionesManualesTest_Empty_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });

            repositorioMock
          .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                           It.IsAny<int>(),
                           It.IsAny<string>(),
                           It.IsAny<DirOrden>(),
                           It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
          .Returns(new List<LogPesificacion>());


            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            Assert.Throws(Is.TypeOf<InfoCustomException>()
               .And.Message.EqualTo(String.Format(InfoMsg.SinRegistros, "Pesificaciones")),
             () => service.ObtenerPesificacionesManuales(1));
        }

        [Test]
        public void ObtenerPorFiltrosPesificacionesManuales_OK()
        {
            var pesificaciones = new List<LogPesificacion> {
                new LogPesificacion {
                Id = 1,
                IdUsuario= 1,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = false

                },
                new LogPesificacion {
                Id = 2,
                IdUsuario= 2,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = false
                }
            };

            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });



            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
               .Returns(pesificaciones);

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            var lista = service.ObtenerPorFiltrosPesificacionesManuales(new FiltroDeBusquedaDto());

            Assert.IsNotNull(lista);
            Assert.IsInstanceOf<List<LogPesificacionDto>>(lista);
            Assert.IsTrue(lista.Any());
        }

        [Test]
        public void ObtenerPorFiltrosPesificacionesAutomaticasTest_OK()
        {
            var pesificaciones = new List<LogPesificacion> {
                new LogPesificacion {
                Id = 1,
                IdUsuario= 1,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = true

                },
                new LogPesificacion {
                Id = 2,
                IdUsuario= 2,
                Fecha = DateTime.Now,
                CantidadKilos = 100,
                Fijacion = 100,
                Contrato = 100,
                EsCargaMasiva = true
                }
            };

            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });



            repositorioMock
               .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                                It.IsAny<int>(),
                                It.IsAny<string>(),
                                It.IsAny<DirOrden>(),
                                It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
               .Returns(pesificaciones);

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            var lista = service.ObtenerPorFiltrosPesificacionesAutomaticas(new FiltroDeBusquedaMasicoDto());

            Assert.IsNotNull(lista);
            Assert.IsInstanceOf<List<LogPesificacionDto>>(lista);
            Assert.IsTrue(lista.Any());
        }

        [Test]
        public void ObtenerPorFiltrosPesificacionesManualeTest_NoAdmin_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "NoADM" } }
               });

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            Assert.Throws(Is.TypeOf<InfoCustomException>()
               .And.Message.EqualTo("El usuario no es administrador."),
             () => service.ObtenerPorFiltrosPesificacionesManuales(new FiltroDeBusquedaDto()));
        }

        [Test]
        public void ObtenerPorFiltrosPesificacionesAutomaticasTest_NoAdmin_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "NoADM" } }
               });

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            Assert.Throws(Is.TypeOf<InfoCustomException>()
               .And.Message.EqualTo("El usuario no es administrador."),
             () => service.ObtenerPorFiltrosPesificacionesAutomaticas(new FiltroDeBusquedaMasicoDto()));
        }

        [Test]
        public void ObtenerPorFiltrosPesificacionesManualeTest_Empty_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });

            repositorioMock
           .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>(),
                            It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
           .Returns(new List<LogPesificacion>());

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            var listado = service.ObtenerPorFiltrosPesificacionesManuales(new FiltroDeBusquedaDto());

            Assert.IsNotNull(listado);
            Assert.IsInstanceOf<List<LogPesificacionDto>>(listado);
            Assert.IsFalse(listado.Any());
        }

        [Test]
        public void ObtenerPorFiltrosPesificacionesAutomaticasTest_Empty_InfoCustomException()
        {
            repositorioMock = new Mock<IRepositorio>();

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
               .Returns(new Usuario
               {
                   Id = 1,
                   Roles = new List<Rol> { new Rol { Codigo = "ADM" } }
               });

            repositorioMock
           .Setup(x => x.Listar(It.IsAny<Expression<Func<LogPesificacion, bool>>>(),
                            It.IsAny<int>(),
                            It.IsAny<string>(),
                            It.IsAny<DirOrden>(),
                            It.IsAny<IEnumerable<Expression<Func<LogPesificacion, object>>>>()))
           .Returns(new List<LogPesificacion>());

            service = new LogPesificacionService(repositorioMock.Object, validador.Object);

            var listado = service.ObtenerPorFiltrosPesificacionesAutomaticas(new FiltroDeBusquedaMasicoDto());

            Assert.IsNotNull(listado);
            Assert.IsInstanceOf<List<LogPesificacionDto>>(listado);
            Assert.IsFalse(listado.Any());
        }


    }
}
