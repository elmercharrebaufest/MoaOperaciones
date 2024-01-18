using SustitucionMOAModel.Dto.CampoSustentable;
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
    }
}
