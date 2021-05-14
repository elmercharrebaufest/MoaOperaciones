using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.Validadores;


namespace SustitucionMOAUtils.Interfaces.Validadores
{
    public interface IValidadorPesificacion
    {
        ResultadoValidacionPesificacion IsValid(LogPesificacion entidad);
    }
}
