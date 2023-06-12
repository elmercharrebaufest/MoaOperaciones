using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.ResponseHandler.OrdenCarga
{
    public class ModEntregaResponseHandler
    {
        private static readonly string RespuestaSAP_ActualizadoOK = "Se actualizaron los datos correctamente";
        private static readonly string RespuestaSAP_EntregaAnulada = "Entrega anulada en SAP";

        public bool ActualizadoOK { get; private set; } = false;
        public bool EntregaAnulada { get; private set; } = false;
        public bool EntregaTomadaEnSap { get; private set; } = false;

        public ModEntregaResponseHandler(string respuestaSap)
        {
            var respuestaMayusculas = respuestaSap != null ? respuestaSap.ToUpper() : "";
            if (respuestaMayusculas == RespuestaSAP_ActualizadoOK.ToUpper())
            {
                ActualizadoOK = true;
            }
            else
            {
                if (respuestaMayusculas == RespuestaSAP_EntregaAnulada.ToUpper())
                {
                    EntregaAnulada = true;
                }
                else
                {
                    EntregaTomadaEnSap = true;
                }
            }
        }
    }
}
