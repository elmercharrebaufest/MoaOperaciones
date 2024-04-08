using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class OrdenResiduosService : IOrdenResiduosService
    {
        protected readonly IRepositorioOrdenResiduos repositorio;

        public OrdenResiduosService(IRepositorioOrdenResiduos repositorio)
        {
            this.repositorio = repositorio;
        }

        public MaterialDto[] ObtenerMateriales()
        {
            return repositorio.ObtenerMateriales();
        }

        public ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicioStr, string fechaFinStr)
        {
            var fechaInicio = DataFormatter.StringToDateTime(fechaInicioStr, "");
            var fechaFin = DataFormatter.StringToDateTime(fechaFinStr, "");

            var listado = repositorio.ObtenerListadoOrdenes(fechaInicio, fechaFin);

            if (listado == null || listado.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga de residuos e insumos"));
            }

            return new ListarOrdenesResiduosResponse
            {
                ListaOrdenes = listado
            };
        }
    }
}
