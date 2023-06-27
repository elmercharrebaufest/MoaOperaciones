namespace SustitucionMOAWS.ResponseHandler.OrdenCarga
{
    public class ModOrdenCargaResponseHandler
    {
        private static readonly string RespuestaSAP_ActualizadoOK = "Se actualizaron los datos correctamente";
        private static readonly string RespuestaSAP_PedidoAnulado = "Pedido ya anulado";

        public bool ActualizadoOK { get; private set; } = false;
        public bool PedidoAnulado { get; private set; } = false;
        public bool PedidoTomadoEnSap { get; private set; } = false;

        public ModOrdenCargaResponseHandler(string respuestaSap)
        {
            var respuestaMayusculas = respuestaSap != null ? respuestaSap.ToUpper() : "";
            if (respuestaMayusculas == RespuestaSAP_ActualizadoOK.ToUpper())
            {
                ActualizadoOK = true;
            }
            else if (respuestaMayusculas == RespuestaSAP_PedidoAnulado.ToUpper())
            {
                PedidoAnulado = true;
            }
            else
            {
                PedidoTomadoEnSap = true;
            }
        }
    }
}
