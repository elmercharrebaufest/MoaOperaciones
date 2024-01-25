using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioCampoSustentable : IRepositorio
    {
        ReporteCertificadorDto ObtenerReporteCertificador(int idCampoCosecha, int idProveedor);

        Usuario ObtenerUsuarioPorMail(string mail);

        DeclaracionCampoSustentable ObtenerDeclaracionDeProveedor(string cuitProveedor, int idCosecha);

        Archivo ObtenerArchivo(int idArchivo);
    }
}
