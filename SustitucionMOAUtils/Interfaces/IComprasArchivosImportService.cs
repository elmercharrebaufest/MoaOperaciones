using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras.PrecargaSolp;
using System.Collections.Generic;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IComprasArchivosImportService
    {
        ProcesarPrecargaSolpResponse ProcesarArchivoPrecargaSolp(
            HttpPostedFileBase archivo,
            List<TablaGeneralDto> tiposImputaciones,
            List<TablaSapDto> monedas,
            List<TablaSapDto> gruposCompras,
            List<TablaSapDto> gruposArticulos,
            List<TablaSapDto> centros,
            List<TablaSapDto> almacenes,
            List<TablaSapDto> unidades,
            List<TablaSapDto> cuentasMayor,
            TablaGeneralDto tipoPosicion);
    }
}
