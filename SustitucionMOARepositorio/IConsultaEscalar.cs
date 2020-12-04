using System.Data.Entity;

namespace SustitucionMOARepositorio
{
    public interface IConsultaEscalar<TEntidad>
    {
        TEntidad Ejecutar(DbContext contexto);
    }
}
