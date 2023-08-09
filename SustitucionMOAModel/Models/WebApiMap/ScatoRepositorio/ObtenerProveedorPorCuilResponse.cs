using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio
{
    [Newtonsoft.Json.JsonObject]
    public class ObtenerProveedorPorCuilResponse : Respuesta<Proveedor>
    {
        public bool TieneError(ObtenerProveedorPorCuilError tipoError)
        {
            if (IsValid)
            {
                throw new Exception("No se puede obtener error, la consulta fue exitosa");
            }
            var errorStr = ConvertirTipoErrorATexto(tipoError);
            return Messages.Any(x => x.MessageCode == errorStr);
        }

        private string ConvertirTipoErrorATexto(ObtenerProveedorPorCuilError tipoError)
        {
            switch (tipoError)
            {
                case ObtenerProveedorPorCuilError.CuilRequerido: return "01";
                case ObtenerProveedorPorCuilError.CuilFormatoNoValido: return "02";
                case ObtenerProveedorPorCuilError.DigitoVerificadorNoValido: return "03";
                case ObtenerProveedorPorCuilError.ProveedorNoEncontrado: return "04";
                case ObtenerProveedorPorCuilError.TipoNoValido: return "05";
                default:
                    throw new Exception("Tipo ObtenerProveedorPorCuilError no mapeado");
            }
        }
    }

    public enum ObtenerProveedorPorCuilError
    {
        CuilRequerido,
        CuilFormatoNoValido,
        DigitoVerificadorNoValido,
        ProveedorNoEncontrado,
        TipoNoValido
    }
}
