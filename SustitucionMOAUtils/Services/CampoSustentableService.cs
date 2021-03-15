using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class CampoSustentableService : ICampoSustentableService
    {
        private readonly IRepositorio repositorio;
        public CampoSustentableService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            ValidarCampo(usuario, campoProveedor);

            campoProveedor.CampoCosecha.CampoSustentable_Id = ObtenerIdCampoSustentable(campoProveedor);

            campoProveedor.FechaCreacion = DateTime.Now;
            campoProveedor.Borrado = false;
            campoProveedor.Archivo = (new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = "" });
            repositorio.Agregar(campoProveedor);

            repositorio.GuardarCambios();

            GuardarArchivoKMZ(campoProveedor, archivoKmz);

            repositorio.GuardarCambios();

            return new Resultado { IdEntidad = campoProveedor.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableAgregado };
        }

        private void ValidarUsuario(Usuario usuario, int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            if (!usuario.ObtenerPermisos().Contains("VER TODOS CAMPOS SUSTENTABLE"))
            {
                if (!usuario.Proveedores.Any(p => p.CUIT == proveedor.CUIT))
                {
                    throw new ValidationCustomException("Su usuario no tiene habilitado el proveedor con el que intenta operar.");
                }
            }
        }

        public Resultado Editar(string mailUsuario, CampoProveedor campoProveedorObj, HttpPostedFileBase archivoKmz)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedorObj.Proveedor_Id);

            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == campoProveedorObj.Proveedor_Id && cp.CampoCosecha_Id == campoProveedorObj.CampoCosecha_Id);

            if (campoProveedor == null)
            {
                throw new ValidationCustomException("No se encontró el campo sustentable para editar.");
            }

            campoProveedor.FechaModificacion = DateTime.Now;

            ValidarCampo(usuario, campoProveedor);

            campoProveedor.HectareasSoja = campoProveedorObj.HectareasSoja;
            campoProveedor.HectareasTotales = campoProveedorObj.HectareasTotales;
            campoProveedor.Longitud = campoProveedorObj.Longitud;
            campoProveedor.Latitud = campoProveedorObj.Latitud;

            repositorio.GuardarCambios();

            GuardarArchivoKMZ(campoProveedor, archivoKmz);

            repositorio.GuardarCambios();

            return new Resultado { IdEntidad = campoProveedorObj.CampoCosecha_Id, Mensaje = SuccessMsg.CampoSustentableActualizado };
        }

        public string Borrar(string mailUsuario, int campoCosechaId, int proveedorId)
        {
            var campoProveedor = repositorio.Obtener<CampoProveedor>(cp => cp.Proveedor_Id == proveedorId && cp.CampoCosecha_Id == campoCosechaId);

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, campoProveedor.Proveedor_Id);

            if (campoProveedor.CampoCosecha.ToneladasAprobadas > 0)
            {
                throw new ValidationCustomException("No se puede eliminar el campo debido a que ya tiene toneladas aprobadas");
            }

            campoProveedor.Borrado = true;

            return SuccessMsg.CampoSustentableBorrado;
        }

        private void ValidarCampo(Usuario usuario, CampoProveedor campoProveedor)
        {

        }

        private void GuardarArchivoKMZ(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {

            string fileName = string.Concat(campoProveedor.CampoCosecha.CampoSustentable_Id, "-", campoProveedor.Proveedor.CUIT, Path.GetExtension(archivoKmz.FileName));

            string rutaCarpeta = ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"];

            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            if (File.Exists(rutaArchivo))
            {
                File.Delete(rutaArchivo);
            }

            campoProveedor.Archivo.Ruta = rutaArchivo;

            archivoKmz.SaveAs(rutaArchivo);

        }

        private int ObtenerIdCampoSustentable(CampoProveedor campoProveedor)
        {
            var campoSustentable = repositorio.Obtener<CampoSustentable>(c =>
                                                                         c.Nombre == campoProveedor.CampoCosecha.Campo.Nombre &&
                                                                         c.Localidad_Id == campoProveedor.CampoCosecha.Campo.Localidad_Id);

            var idCampo = campoSustentable?.Id ?? 0;

            if (idCampo == 0)
            {
                campoProveedor.CampoCosecha.ToneladasAprobadas = 0;
            }
            else
            {
                var campoCosecha = repositorio.Obtener<CampoCosecha>(cc => cc.CampoSustentable_Id == idCampo && cc.Cosecha_Id == campoProveedor.CampoCosecha_Id);

                campoProveedor.CampoCosecha.ToneladasAprobadas = campoCosecha.ToneladasAprobadas;
            }

            return campoSustentable?.Id ?? 0;
        }

        public bool VerificarDeclaracion(int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            var cosechaActual = repositorio.Obtener<Cosecha>(c => DateTime.Now > c.Inicio && DateTime.Now < c.Fin);

            if (proveedor.FechaFirmaDeclaracionCampoSustentable == null)
            {
                return false;
            }

            if (proveedor.FechaFirmaDeclaracionCampoSustentable > cosechaActual.Inicio)
            {
                return true;
            }

            return false;
        }

        public string FirmarDeclaracion(string mailUsuario, int proveedorId, double hectareasTotales)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);

            proveedor.FechaFirmaDeclaracionCampoSustentable = DateTime.Now;

            proveedor.OpcionDeclaracionCampoSustentable = hectareasTotales > 0 ? OpcionesDeclaracionCampoSustentable.Parcial : OpcionesDeclaracionCampoSustentable.Totalidad;

            proveedor.HectareasDeclaracionCampoSustentable = hectareasTotales;

            return SuccessMsg.DeclaracionCampoSustentableFirmada;
        }

        public List<Cosecha> ObtenerCosechas()
        {
            return repositorio.Listar<Cosecha>();
        }

        public List<CampoProveedorListadoDto> Listar(string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            List<CampoProveedorListadoDto> listado;

            if (usuario.ObtenerPermisos().Contains("VER TODOS CAMPOS SUSTENTABLE"))
            {
                listado = repositorio
                               .Listar<CampoProveedor>(p => !p.Borrado)
                               .Select(cp => new CampoProveedorListadoDto
                               {
                                   NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                                   HectareasSoja = cp.HectareasSoja,
                                   HectareasTotales = cp.HectareasTotales,
                                   NombreCampo = cp.CampoCosecha.Campo.Nombre,
                                   ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas
                               }).ToList();
            }
            else
            {
                var proveedoresIds = usuario.Proveedores.Select(pr => pr.Id);
                listado = repositorio
                         .Listar<CampoProveedor>(p => proveedoresIds.Contains(p.Proveedor_Id) && !p.Borrado)
                         .Select(cp => new CampoProveedorListadoDto
                         {
                             NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                             HectareasSoja = cp.HectareasSoja,
                             HectareasTotales = cp.HectareasTotales,
                             NombreCampo = cp.CampoCosecha.Campo.Nombre,
                             ToneladasAprobadas = cp.CampoCosecha.ToneladasAprobadas
                         }).ToList();
            }
            return listado;
        }


        public CampoProveedorDto ObtenerCampo(string mailUsuario, int proveedorId, int campoCosechaId)
        {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            ValidarUsuario(usuario, proveedorId);

            var campo = repositorio.Obtener<CampoProveedor>(p => p.Proveedor_Id == proveedorId && p.CampoCosecha_Id == campoCosechaId);

            var campoDto = new CampoProveedorDto
            {
                NombreCosecha = campo.CampoCosecha.Cosecha.Nombre,
                HectareasSoja = campo.HectareasSoja,
                HectareasTotales = campo.HectareasTotales,
                NombreCampo = campo.CampoCosecha.Campo.Nombre,
                ToneladasAprobadas = campo.CampoCosecha.ToneladasAprobadas
            };

            return campoDto;
        }
    }
}
