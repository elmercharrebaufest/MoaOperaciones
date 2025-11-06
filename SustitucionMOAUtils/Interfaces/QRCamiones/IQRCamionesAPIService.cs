public interface IQRCamionesAPIService
{
    List<QRCamionesConfiguracion> ObtenerConfiguracionesPorTipoWorkflow(string tipoWorkflow);
    QRCamionesConfiguracion ObtenerConfiguracionPorNombre(string nombreEtapa);
    ResultadoGenerico GuardarConfiguracion(QRCamionesConfiguracion model);
    ResultadoGenerico EliminarConfiguracion(int id);
}