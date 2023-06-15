namespace SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS
{
    public enum CrearOrdenResEnum
    {
        PedidoCreado,
        VerificarDatos, //'Verificar Contrato, Material, Cliente'
        VerificarCantidadPendiente, //'Verificar cantidad pendiente de Contratada'
        PedidoCreadoVerificarCredito, //'Pedido creado - Verificar Crédito de pedido' 
        ContratoSinKg, //Contrato sin kg pendientes para generar pedido
        NoEsperado, //Lo usamos cuando tenemos una respuesta inesperada por parte de SAP
        Vacia, //Lo usamos cuando tenemos una respuesta vacia
    }
    public static class OrdenCargaCrearOrdenClass
    {
        public static string GetCodigo(CrearOrdenResEnum result)
        {
            return ResponseConverter.GetCodigoCrearOrden(result);
        }
    }
}
