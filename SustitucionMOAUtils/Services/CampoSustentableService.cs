using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Permiso;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Resultado Agregar(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            campoProveedor.CampoCosecha.CampoSustentable_Id = ObtenerIdCampoSustentable(campoProveedor);

            campoProveedor.CampoCosecha.ToneladasAprobadas = 0;

            repositorio.Agregar(campoProveedor);

            GuardarArchivoKMZ(campoProveedor, archivoKmz);

            repositorio.GuardarCambios();

            return new Resultado { IdEntidad = campoProveedor.CampoCosecha.CampoSustentable_Id, Mensaje = SuccessMsg.NotificacionAgregada };
        }


        private void GuardarArchivoKMZ(CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz)
        {
            string fileName = string.Concat(campoProveedor.CampoCosecha.CampoSustentable_Id, Path.GetExtension(archivoKmz.FileName));

            string rutaCarpeta = ConfigurationManager.AppSettings["RutaArchivosCampoSustentable"];

            string rutaArchivo = string.Concat(rutaCarpeta, "/", fileName);

            Directory.CreateDirectory(rutaCarpeta);

            campoProveedor.Archivo = (new Archivo { FileKey = FileKeys.CampoSustentableKMZ, Ruta = rutaArchivo });

            archivoKmz.SaveAs(rutaArchivo);
        }

        private int ObtenerIdCampoSustentable(CampoProveedor campoProveedor)
        {
            var campoSustentable = repositorio.Obtener<CampoSustentable>(c =>
                                                                         c.Nombre == campoProveedor.CampoCosecha.Campo.Nombre &&
                                                                         c.Localidad_Id == campoProveedor.CampoCosecha.Campo.Localidad_Id);

            var idCampo = campoSustentable?.Id ?? 0;

            if (idCampo == 0 )
            {
                campoProveedor.CampoCosecha.ToneladasAprobadas = 0;
            }

            return campoSustentable?.Id ?? 0;
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
                               .Listar<CampoProveedor>()
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
                listado = repositorio
                         .Listar<CampoProveedor>(p => usuario.Proveedores.Contains(p.Proveedor))
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

    }
}
