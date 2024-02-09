using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioCampoSustentable : IRepositorio
    {
        CampoReporteDTO ObtenerReporteCertificador(int idCampoCosecha, int idProveedor);

        Usuario ObtenerUsuarioPorMail(string mail);

        DeclaracionCampoSustentable ObtenerDeclaracionDeProveedor(string cuitProveedor, int idCosecha);

        Archivo ObtenerArchivo(int idArchivo);
    }
}
