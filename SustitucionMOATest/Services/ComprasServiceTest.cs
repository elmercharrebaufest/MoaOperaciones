using Moq;
using NUnit.Framework;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SustitucionMOATest.Services
{
    [TestFixture()]
    public class ComprasServiceTest
    {

        private ComprasService target;
        private Mock<IRepositorio> repositorioMock;

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

            var solpMock = new Solp() 
            { 
                Id = 1, 
                Pliego_Id = 1, 
                Pliego = pliegoMock, 
                UsuarioCreacion_Id = 1, 
                UsuarioCreacion = usuarioMock,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                UsuarioModificacion_Id = 1,
                UsuarioModificacion = usuarioMock,
                ClaseDocumento_Id = 1,
                ClaseDocumento = claseDocumentoMock,
                EstadoDocumento_Id = 1,
                EstadoSolpSap = claseDocumentoMock,
                EstadoSolpSap_Id = 1,
                EstadoDocumento = tablaEstado,
                Posiciones = posicionesMock
            };

            var pathbase = $"{ConfigurationManager.AppSettings["RutaArchivosCompras"]}/{DateTime.Now.Ticks}";

            repositorioMock
               .Setup(x => x.Obtener(It.IsAny<Expression<Func<Solp, bool>>>()))
               .Returns(solpMock);

            var expected = $"Solp-xxx-pliego-{DateTime.Now.ToString("yyyyMMdd")}.zip";
            var result = target.GenerarZipPliego(solpMock.Id, pathbase);

            Assert.AreEqual(expected, result);
        }*/
    }
}
