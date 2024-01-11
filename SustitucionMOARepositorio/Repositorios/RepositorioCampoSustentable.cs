using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioCampoSustentable : RepositorioEF, IRepositorioCampoSustentable
    {
        public RepositorioCampoSustentable(DbContext context) : base(context) { }

        public ReporteCertificadorDto ObtenerReporteCertificador(int idCampoCosecha, int idProveedor)
        {
            var dto = (
                from cp in Set<CampoProveedor>()
                where cp.CampoCosecha_Id == idCampoCosecha && cp.Proveedor_Id == idProveedor
                select new ReporteCertificadorDto
                {
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
    }
}
