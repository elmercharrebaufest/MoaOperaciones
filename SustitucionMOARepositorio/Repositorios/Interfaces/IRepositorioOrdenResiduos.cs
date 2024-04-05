using SustitucionMOAModel.Dto.OrdenResiduos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioOrdenResiduos : IRepositorio
    {
        List<OrdenResiduosFila> ObtenerListadoOrdenes();
    }
}
