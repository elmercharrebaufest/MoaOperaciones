using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ExternalApiOrdenesResiduosService : IExternalApiOrdenesResiduosService
    {
        private IRepositorio repositorio { get; set; }

        public ExternalApiOrdenesResiduosService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<OrdenesDeCargaApiDto> ObtenerOrdenes(string patenteChasis = null)
        {
            var listado = repositorio.Listar<OrdenResiduos>(
                    or => string.IsNullOrEmpty(patenteChasis) || or.PatenteChasis == patenteChasis
                ).Select(
                    or => OrdenesDeCargaApiDto.From(or)
                );

            return listado.ToList();
        }
    }
}
