namespace SustitucionMOAUtils.Interfaces
{
    public interface IMaterialService
    {
        void ActualizarMaterialesSolp();
        void ActualizarMaterialSolpDadoCentroYCod(int centroId, string codMaterial);
    }
}
