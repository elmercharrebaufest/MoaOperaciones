using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioCampoSustentable : IRepositorio
    {
        CampoReporteDTO ObtenerReporteCertificador(int idCampoCosecha, int idProveedor);

        Usuario ObtenerUsuarioPorMail(string mail);

        DeclaracionCampoSustentable ObtenerDeclaracionDeProveedor(string cuitProveedor, int idCosecha);

        Archivo ObtenerArchivo(int idArchivo);

        List<SugerenciaCampoDto> ObtenerSugerenciaCamposNuevaCosecha(int proveedorId, int nuevaCosechaId, string cuitTitularCP);
    }
}
