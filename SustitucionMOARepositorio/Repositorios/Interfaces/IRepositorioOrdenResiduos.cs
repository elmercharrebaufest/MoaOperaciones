using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioOrdenResiduos : IRepositorio
    {
        MaterialDto[] ObtenerMateriales();

        List<OrdenResiduosFila> ObtenerListadoOrdenes(DateTime fechaInicio, DateTime fechaFin);
    }
}
