using SustitucionMOAModel.Dto.PliegoMultiple;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class PliegoMultipleService : IPliegoMultipleService
    {
        private readonly IRepositorio repositorio;

        public PliegoMultipleService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1155:Use StringComparison when comparing strings", Justification = "Not supported by EF")]
        public List<PliegoDto> GetPliegosMultiples(string nombrePliego)
        {
            IQueryable<Pliego> pliegos = repositorio.ListarConsultable<Pliego>(pliego => pliego.Multiple);
            if (string.IsNullOrWhiteSpace(nombrePliego))
            {
                return pliegos
                    .ToList()
                    .ConvertAll(pliego => (PliegoDto)pliego);
            }
            else
            {
                return pliegos
                    .Where(p => p.NombreObra.ToLower().Contains(nombrePliego.ToLower()))
                    .ToList()
                    .ConvertAll(pliego => (PliegoDto)pliego);
            }
        }
    }
}
