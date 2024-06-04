using System.Data.Entity;
using System.Linq;

namespace SustitucionMOARepositorio
{
    public interface IConsultaEscalar<TEntidad>
    {
        TEntidad Ejecutar(DbContext contexto);
    }
}
