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

        public List<SolpDto> GetSolpDisponiblesPliegosMultiple()
        {
            IEnumerable<string> codigosSapEstadosSolpValidos = new HashSet<string> { "02", "05" };
            IEnumerable<string> tiposSolpValidos = new HashSet<string> { "CON_PLIEGO", "SIN_PLIEGO" };
            IEnumerable<string> tiposPosicionSolpValidos = new HashSet<string> { "SERVICIO", "MATERIALES" };

            IQueryable<Solp> consultaSolp = repositorio
                .ListarConsultable<Solp>(solpQuery =>
                    codigosSapEstadosSolpValidos.Contains(solpQuery.EstadoSolpSap.CodigoSap)
                    && tiposSolpValidos.Contains(solpQuery.TipoSolp.Codigo)
                    && solpQuery.Posiciones.Any(posicion => tiposPosicionSolpValidos.Contains(posicion.TipoPosicion.Codigo))
                    && !(solpQuery.TrabajoYaHecho == true || solpQuery.Adicional == true || solpQuery.Urgencia == true || solpQuery.CondEspProveedorAsignado == true)
                    && !solpQuery.Pliego.Multiple
                    && !solpQuery.Posiciones.Any(posicion => posicion.AdjudicacionPosiciones.Any())
                    )
                ;

            return consultaSolp
                .ToList()
                .ConvertAll(solp => (SolpDto)solp);
        }
    }
}
