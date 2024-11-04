using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios.Interfaces
{
    public interface IRepositorioOrdenDeCargaFason : IRepositorio
    {
        List<string> ObtenerCuilsChofer(int clienteId, string patenteAcoplado);
    }
}
