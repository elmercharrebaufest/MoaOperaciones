using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOATest.Services
{
    public class NotificacionServiceTest
    {
        private NotificacionService target;
        private Mock<IRepositorio> repositorioMock;
        Notificacion notificacion1;
        List<Rol> listadoRoles = new List<Rol>();
        List<TipoUsuario> listadoTipoUsuarios = new List<TipoUsuario>();

        [SetUp]
        public void SetUp()
        {
            repositorioMock = new Mock<IRepositorio>();
            target = new NotificacionService(repositorioMock.Object);

            var rolAdmin = new Rol
            {
                Id = 1,
                Codigo = "ADM",
                Nombre = "Admin"
            };

            listadoRoles.Add(rolAdmin);

            var tipoGranos = new TipoUsuario
            {
                Id = 1,
                NombreCorto = "GRAN",
                Nombre = "GRANOS"
            };

            listadoTipoUsuarios.Add(tipoGranos);

            repositorioMock
              .Setup(y => y.Obtener<Rol>(It.IsAny<int>()))
              .Returns(rolAdmin);

            repositorioMock
             .Setup(y => y.Obtener<TipoUsuario>(It.IsAny<int>()))
             .Returns(tipoGranos);

            notificacion1 = new Notificacion
            {
                Nombre = "Test 1",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                Borrada = false,
                Mensaje = "Hola",
                Habilitada = true,
                LinkAdjunto = "https://google.com/",
                FiltroRoles = listadoRoles,
                FiltroTipoUsuario = listadoTipoUsuarios
            };

        }

        [Test]
        public void AgregarNueva()
        {
            var result = target.GrabarNotificacion(notificacion1);

            repositorioMock
             .Setup(y => y.Existe(It.IsAny<Expression<Func<Notificacion, bool>>>()))
             .Returns(false);


            var expected = SuccessMsg.NotificacionAgregada;
            Assert.AreEqual(expected, result);

            repositorioMock.Verify(x => x.Obtener<Rol>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<TipoUsuario>(It.IsAny<int>()), Times.Once);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Notificacion>()), Times.Once);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);

        }

        [Test]
        public void EditarExistente()
        {
            int idNotificacion = 1;
            notificacion1.Id = idNotificacion;

            repositorioMock
              .Setup(y => y.Existe(It.IsAny<Expression<Func<Notificacion, bool>>>()))
              .Returns(false);

            repositorioMock
                .Setup(y => y.Obtener<Notificacion>(idNotificacion))
                .Returns(notificacion1);

            var notificacionEditada = new Notificacion
            {
                Id = 1,
                Nombre = "Test 1",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                Borrada = false,
                Mensaje = "Hola",
                Habilitada = true,
                LinkAdjunto = "https://google.com/",
                FiltroRoles = new List<Rol> { new Rol { Id = 1 } },
                FiltroTipoUsuario = new List<TipoUsuario> { new TipoUsuario { Id = 1 } }
            };

            var result = target.GrabarNotificacion(notificacionEditada);

            var expected = SuccessMsg.NotificacionActualizada;
            Assert.AreEqual(expected, result);

            repositorioMock.Verify(x => x.Obtener<Rol>(It.IsAny<int>()), Times.Once);
            repositorioMock.Verify(x => x.Obtener<TipoUsuario>(It.IsAny<int>()), Times.Once);

            repositorioMock.Verify(x => x.Agregar(It.IsAny<Notificacion>()), Times.Never);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void AgregarRepetida()
        {
            Notificacion nueva = new Notificacion
            {
                Nombre = "Test 1",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                Borrada = false,
                Mensaje = "Hola",
                Habilitada = true,
                LinkAdjunto = "https://google.com/",
                FiltroRoles = listadoRoles,
                FiltroTipoUsuario = listadoTipoUsuarios
            };

            repositorioMock
              .Setup(y => y.Existe(It.IsAny<Expression<Func<Notificacion, bool>>>()))
              .Returns(true);


            var ex = Assert.Throws<ValidationCustomException>(() => target.GrabarNotificacion(notificacion1));

            var expected = "Ya existe una notificación con el mismo nombre";
            Assert.AreEqual(expected, ex.Message);

            repositorioMock.Verify(x => x.GuardarCambios(), Times.Never);
        }


        [Test]
        public void Habilitar()
        {
            int idNotificacion = 1;

            repositorioMock
                .Setup(y => y.Obtener<Notificacion>(idNotificacion))
                .Returns(notificacion1);

            var result = target.Habilitar(idNotificacion);

            var expected = SuccessMsg.NotificacionActualizada;
            Assert.AreEqual(expected, result);

            var resultNotificacion = repositorioMock.Object.Obtener<Notificacion>(idNotificacion);
            Assert.AreEqual(true, resultNotificacion.Habilitada);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void Deshabilitar()
        {
            int idNotificacion = 1;

            repositorioMock
                .Setup(y => y.Obtener<Notificacion>(idNotificacion))
                .Returns(notificacion1);

            var result = target.Deshabilitar(idNotificacion);

            var expected = SuccessMsg.NotificacionActualizada;
            Assert.AreEqual(expected, result);

            var resultNotificacion = repositorioMock.Object.Obtener<Notificacion>(idNotificacion);
            Assert.AreEqual(false, resultNotificacion.Habilitada);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }

        [Test]
        public void Eliminar()
        {
            int idNotificacion = 1;

            repositorioMock
                .Setup(y => y.Obtener<Notificacion>(idNotificacion))
                .Returns(notificacion1);

            var result = target.Eliminar(idNotificacion);

            var expected = SuccessMsg.NotificacionBorrada;
            Assert.AreEqual(expected, result);

            var resultNotificacion = repositorioMock.Object.Obtener<Notificacion>(idNotificacion);
            Assert.AreEqual(true, resultNotificacion.Borrada);
            repositorioMock.Verify(x => x.GuardarCambios(), Times.Once);
        }
    }
}
