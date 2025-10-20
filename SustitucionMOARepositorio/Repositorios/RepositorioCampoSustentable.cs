using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioCampoSustentable : RepositorioEF, IRepositorioCampoSustentable
    {
        public RepositorioCampoSustentable(DbContext context) : base(context) { }

        public CampoReporteDTO ObtenerReporteCertificador(int idCampoCosecha, int idProveedor)
        {
            var dto = (
                from cp in Set<CampoProveedor>()
                where cp.CampoCosecha_Id == idCampoCosecha && cp.Proveedor_Id == idProveedor
                select new CampoReporteDTO
                {
                    IdScato = cp.CampoCosecha.Campo.IdScato,
                    CUIT = cp.CUIT,
                    Departamento = cp.CampoCosecha.Campo.Localidad.Partido.Descripcion,
                    HectareasSoja = cp.HectareasSoja,
                    Id = cp.CampoCosecha.Campo.Id,
                    Latitud = cp.Latitud,
                    Localidad = cp.CampoCosecha.Campo.Localidad.Nombre,
                    Longitud = cp.Longitud,
                    Nombre = cp.CampoCosecha.Campo.Nombre,
                    NombreCosecha = cp.CampoCosecha.Cosecha.Nombre,
                    Provincia = cp.CampoCosecha.Campo.Localidad.Provincia.Nombre,
                    RazonSocial = cp.RazonSocial
                }).Single();

            return dto;
        }

        public Usuario ObtenerUsuarioPorMail(string mail)
        {
            return Obtener<Usuario>(u => u.Mail == mail);
        }

        public DeclaracionCampoSustentable ObtenerDeclaracionDeProveedor(string cuitProveedor, int idCosecha)
        {
            var declaracion = Obtener<DeclaracionCampoSustentable>(d =>
                d.Cosecha_Id == idCosecha &&
                d.CUIT == cuitProveedor);

            return declaracion;
        }

        public Archivo ObtenerArchivo(int idArchivo)
        {
            return Obtener<Archivo>(a => a.Id == idArchivo);
        }

        public List<SugerenciaCampoDto> ObtenerSugerenciaCamposNuevaCosecha(int proveedorId, int nuevaCosechaId, string cuitTitularCP)
        {
            var cosechaAnteriorId = (
                from cosechaNueva in Set<Cosecha>()
                join cosechaAnterior in Set<Cosecha>() on true equals true
                where
                    cosechaNueva.Id == nuevaCosechaId &&
                    cosechaAnterior.Inicio < cosechaNueva.Inicio
                 select new
                 {
                     cosechaAnterior.Id,
                     cosechaAnterior.Inicio
                 })
                .OrderByDescending(x => x.Inicio)
                .Select(x => x.Id)
                .FirstOrDefault();

            var camposSugerencia = (
                from campoProveedor in Set<CampoProveedor>()
                join campoProvPresentado in Set<CampoProveedor>() on
                    new { campoProveedor.CampoCosecha.Campo.Renspa, CosechaId = nuevaCosechaId, Borrado = false } equals
                    new { campoProvPresentado.CampoCosecha.Campo.Renspa, CosechaId = campoProvPresentado.CampoCosecha.Cosecha_Id, campoProvPresentado.Borrado } into campoPresentadoGroup
                from campoPresentado in campoPresentadoGroup.DefaultIfEmpty()
                where
                    campoProveedor.Proveedor_Id == proveedorId &&
                    campoProveedor.CUIT == cuitTitularCP &&
                    campoProveedor.CampoCosecha.Cosecha_Id == cosechaAnteriorId &&
                    campoProveedor.CampoCosecha.CampoCosechaNormativas.Any(n => n.ToneladasAprobadas > 0) &&
                    !campoProveedor.Borrado
                select new SugerenciaCampoDto
                {
                    NombreCosecha = campoProveedor.CampoCosecha.Cosecha.Nombre,
                    HectareasSoja = campoProveedor.HectareasSoja,
                    HectareasTotales = campoProveedor.HectareasTotales,
                    NombreCampo = campoProveedor.CampoCosecha.Campo.Nombre,
                    Renspa = campoProveedor.CampoCosecha.Campo.Renspa,
                    Localidad_Id = campoProveedor.CampoCosecha.Campo.Localidad_Id,
                    Latitud = campoProveedor.Latitud,
                    Longitud = campoProveedor.Longitud,
                    CampoCosechaId = campoProveedor.CampoCosecha_Id,
                    ProveedorNombre = campoProveedor.RazonSocial,
                    LocalidadNombre = campoProveedor.CampoCosecha.Campo.Localidad.Nombre,
                    CampoSustentableId = campoProveedor.CampoCosecha.CampoSustentable_Id,
                    CosechaId = campoProveedor.CampoCosecha.Cosecha_Id,
                    CUIT = campoProveedor.CUIT,
                    Archivo_Id = campoProveedor.Archivo_Id,
                    Proveedor_Id = campoProveedor.Proveedor_Id,
                    EvidenciaEPA_Id = campoProveedor.EvidenciaEPA_Id,
                    CodigoProveedor = campoProveedor.Proveedor.CodigoProveedor,
                    CampoYaPresentado = (campoPresentado != null && campoPresentado.CUIT == cuitTitularCP && !campoPresentado.Borrado),
                    EPA = campoProveedor.CampoCosecha.CampoCosechaNormativas.Any(n => n.TipoNormativa.Descripcion == "EPA" && n.ToneladasAprobadas > 0),
                    EUDR = campoProveedor.CampoCosecha.CampoCosechaNormativas.Any(n => n.TipoNormativa.Descripcion == "EUDR" && n.ToneladasAprobadas > 0),
                    BSVS2 = campoProveedor.CampoCosecha.CampoCosechaNormativas.Any(n => n.TipoNormativa.Descripcion == "2BSVS" && n.ToneladasAprobadas > 0),
                    Normativas = campoProveedor.CampoCosecha.CampoCosechaNormativas.Where(y => y.ToneladasAprobadas > 0).Select(x => new CampoCosechaNormativaDto
                    {
                        Id = x.Id,
                        ToneladasAprobadas = x.ToneladasAprobadas,
                        DescripcionNormativa = x.TipoNormativa.Descripcion,
                    }).ToList()
                })
                .ToList();

            foreach(var c in camposSugerencia.Where(cs => cs.EvidenciaEPA_Id != null))
            {
                c.NombreArchivoEPA = Path.GetFileName(Obtener<Archivo>(a => a.Id == c.EvidenciaEPA_Id).Ruta);
            }

            return camposSugerencia;
        }
    }
}
