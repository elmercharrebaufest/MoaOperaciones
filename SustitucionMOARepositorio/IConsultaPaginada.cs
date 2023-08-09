using System.Data.Entity;
using SustitucionMOAModel.Consultas;

namespace Molinos.Scato.Repositorio
{
    public interface IConsultaPaginada<TEntidad>
    {
        ListaPaginada<TEntidad> Ejecutar(DbContext contexto);
    }
}
