using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioEntradaServicio : IRepositorio
    {
        bool ExisteRemitoActivoParaProveedor(string remitoNro, string proveedorCodigo);
        List<Solp> ObtenerSolpsAutocertificablesDeOC(List<string> nroSolps);
        Adjudicacion ObtenerUltimaAdjudicacionOC(string nroOC);
    }
}
