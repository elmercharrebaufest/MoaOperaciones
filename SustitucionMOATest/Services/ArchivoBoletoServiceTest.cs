using Moq;
using NUnit.Framework;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.ArchivoBoleto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOATest.Services
{
    [TestFixture]
    public class ArchivoBoletoServiceTest
    {
        private Mock<IRepositorio> repositorio { get; set; }
        private Mock<IAzureService> azureService { get; set; }
        private Mock<HttpPostedFileBase> archivo { get; set; }
        private IArchivoBoletoService service { get; set; }

        private string cuit1 = "1234567";
        private string cuit2 = "7654321";
        private Proveedor proveedor1 { get; set; }
        private Proveedor proveedor2 { get; set; }
        private Usuario usuario { get; set; }
        private EstadoArchivoBoleto estadoPendiente = new EstadoArchivoBoleto { Nombre = "Pendiente", Color = "yellow" };

        [SetUp]
        public void Setup()
        {
            repositorio = new Mock<IRepositorio>();
            azureService = new Mock<IAzureService>();
            archivo = new Mock<HttpPostedFileBase>();
            proveedor1 = new Proveedor { CUIT = cuit1, Id = 1 };
            proveedor2 = new Proveedor { CUIT = cuit2, Id = 2 };
            usuario = new Usuario { Id=1, Mail="mail@usuario.com" };
            service = new ArchivoBoletoService(repositorio.Object, azureService.Object);
        }

        [Test]
        public void ListarArchivosBoleto_FiltrarPorCUITYFechas() 
        {
            SetRespuestaProveedor(proveedor1);
            SetRespuestaProveedor(proveedor2);

            var nombre1 = "Nombre1"; 
            var nombre2 = "Nombre2";
            var nombre3 = "Nombre3";

            var lista1 = CrearListaArchivos(new string[]
            {
                nombre1,nombre2
            });

            var lista2 = CrearListaArchivos(new string[] { nombre3 });
            var req1 = ObtenerReqLista(proveedor1);

            SetRespuestaLista(lista1, req1);

            var result1 = service.ListarArchivosBoleto(req1);

            var req2 = ObtenerReqLista(proveedor2);
            SetRespuestaLista(lista2, req2);
            var result2 = service.ListarArchivosBoleto(req2);

            Assert.That(result1.Count == lista1.Count());
            Assert.That(result1[0].NombreArchivo == nombre1);
            Assert.That(result1[1].NombreArchivo==nombre2);
            Assert.That(result2.Count == lista2.Count());
            Assert.That(result2[0].NombreArchivo == nombre3);
        }

        [Test]
        public void ListarArchivosBoleto_ListaVacia_InfoCustomExcepcion()
        {
            SetRespuestaProveedor(proveedor1);
            var request = ObtenerReqLista(proveedor1);
            SetRespuestaLista(new ArchivoBoletoDto[] {}, request);

            Assert.That(() => service.ListarArchivosBoleto(request), Throws.InstanceOf<InfoCustomException>());
        }
        [Test]
        public async Task CrearArchivoBoleto_DatosCorrectos_CreaArchivo()
        {
            SetRespuestaProveedor(proveedor1);
            SetRespuestaUsuario(usuario);
            SetEstadoPendienteArchivo();
            var nombreArchivo = "nombre.pdf";
            SetNombreArchivo(nombreArchivo);

            var data = new CrearReqArchivoBoletoDto
            {
                Archivo = archivo.Object,
                ProveedorId = proveedor1.Id,
                EmailUsuario = usuario.Mail
            };

            var result = await service.CrearArchivoBoleto(data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.NombreArchivo == nombreArchivo);
            repositorio.Verify(r => r.Agregar(It.IsAny<ArchivoBoleto>()),Times.Once);
            azureService.Verify(aService=>aService.SubirArchivoABlobStorageAsync(
                archivo.Object,
                $"{result.Id}_{result.NombreArchivo}",
                "boletos"
                ), Times.Once);
        }
        [Test]
        public async Task CrearArchivoBoleto_ProveedorsinEncontrar_ThrowValidationException()
        {
            repositorio.Setup(r => r.Obtener(It.IsAny<Expression<Func<Proveedor,bool>>>())).Returns(null as Proveedor);
            SetRespuestaUsuario(usuario);
            SetEstadoPendienteArchivo();
            var nombreArchivo = "nombre.pdf";
            SetNombreArchivo(nombreArchivo);

            var data = new CrearReqArchivoBoletoDto
            {
                Archivo = archivo.Object,
                ProveedorId = proveedor1.Id,
                EmailUsuario = usuario.Mail
            };

            Assert.That(()=>service.CrearArchivoBoleto(data), Throws.InstanceOf<ValidationCustomException>());
        }
        [Test]
        public async Task CrearArchivoBoleto_UsuarioSinEncontrar_ThrowValidationException()
        {
            SetRespuestaProveedor(proveedor1);
            SetRespuestaUsuario(null);
            SetEstadoPendienteArchivo();
            var nombreArchivo = "nombre.pdf";
            SetNombreArchivo(nombreArchivo);

            var data = new CrearReqArchivoBoletoDto
            {
                Archivo = archivo.Object,
                ProveedorId = proveedor1.Id,
                EmailUsuario = usuario.Mail
            };

            Assert.That(() => service.CrearArchivoBoleto(data), Throws.InstanceOf<ValidationCustomException>());
        }

        private void SetRespuestaLista(IEnumerable<ArchivoBoletoDto> boletos, ListarReqArchivoBoletoDto request)
        {
            repositorio.Setup(r=>r.ListarProyeccion<ArchivoBoleto,ArchivoBoletoDto>(
                It.IsAny<Expression<Func<ArchivoBoleto, ArchivoBoletoDto>>>(),
                ab => ab.CUIT == request.CUIT &&
                    request.FechaInicio <= ab.FechaCarga && ab.FechaCarga < request.FechaFinLimite
                )).Returns(boletos.ToList());
        }
        private void SetRespuestaProveedor(Proveedor proveedor)
        {
            repositorio.Setup(r => r.Obtener<Proveedor>(proveedor.Id)).Returns(proveedor);
        }
        private ListarReqArchivoBoletoDto ObtenerReqLista(Proveedor proveedor)
        {
            var now = DateTime.Now;

            return new ListarReqArchivoBoletoDto
            {
                CUIT = proveedor.CUIT,
                FechaInicio = now,
                FechaFin = now.AddDays(2),
                ProveedorId = proveedor.Id
            };
        }
        private IEnumerable<ArchivoBoletoDto> CrearListaArchivos(IEnumerable<string> nombres)
        {
            return nombres.Select(nombre => new ArchivoBoletoDto { NombreArchivo = nombre, });
        }
        private void SetRespuestaUsuario(Usuario usuario)
        {
            repositorio
                .Setup(rep => rep.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(usuario);
        }
        private void SetEstadoPendienteArchivo()
        {
            repositorio
                .Setup(rep => rep.Obtener(It.IsAny<Expression<Func<EstadoArchivoBoleto, bool>>>()))
                .Returns(estadoPendiente);
        }
        private void SetNombreArchivo(string nombre)
        {
            archivo.Setup(a=>a.FileName).Returns(nombre);
        }
    }
}
