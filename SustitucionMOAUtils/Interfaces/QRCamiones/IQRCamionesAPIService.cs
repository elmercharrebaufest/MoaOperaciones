
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces.QRCamiones
{
    public interface IQRCamionesAPIService
    {
        List<QRCamionesConfiguracion> ObtenerConfiguracionesPorTipoWorkflow(string tipoWorkflow);
        QRCamionesConfiguracion ObtenerConfiguracionPorNombre(string nombreEtapa);
        ResultadoGenerico GuardarConfiguracion(QRCamionesConfiguracion model);
        ResultadoGenerico EliminarConfiguracion(int id);
    }
}