namespace SustitucionMOAWS.Enum.OrdenCargaConsumer
{
    public enum OrdenCargaCrearOrden
    {
        PedidoCreado,
        VerificarDatos, //'Verificar Contrato, Material, Cliente'
        VerificarCantidadPendiente, //'Verificar cantidad pendiente de Contratada'
        PedidoCreadoVerificarCredito, //'Pedido creado - Verificar Crédito de pedido' 
        ContratoSinKg, //Contrato sin kg pendientes para generar pedido
    }
    public static class OrdenCargaCrearOrdenClass
    {
        public static string GetCodigo(OrdenCargaCrearOrden result)
        {
            return ResponseConverter.GetCodigoCrearOrden(result);
        }
    }
}
