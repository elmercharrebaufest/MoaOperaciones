using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioEntradaServicio : IRepositorio
    {
        List<Solp> ObtenerSolpsAutocertificablesDeOC(string nroOC);
    }
}
