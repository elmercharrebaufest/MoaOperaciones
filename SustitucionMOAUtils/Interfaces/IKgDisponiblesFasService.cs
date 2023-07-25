using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System.Collections.Generic;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IKgDisponiblesFasService
    {
        //decimal KgDisponiblesContratoAnticipado(Result contratoSAP);
        //decimal KgDisponiblesContratoNormal(Result contratoSAP);
        decimal ObtenerKgDisponiblesContrato(Result contratoSAP, List<OrdenDeCarga> contratosPedidosPendientesDeCrear);
        //decimal ObtenerKgEntregadosPorGrupoPedidos(List<Detail> grupoPedidos);
        //decimal ObtenerKgEntregadosPorPedido(List<Detail> grupoPedidos);
        decimal ObtenerKgEstandar(Result contratoSAP);
        bool PedidoEstaCargado(Detail pedido);
        decimal ObtenerKgDisponiblesPedido(List<Detail> grupoPedidos, List<OrdenDeCarga> ordenesPendientesDeCrear, Detail pedidoPrincipal, decimal kilosEntregaEstandar);
        /// <summary>
        /// Método auxiliar, correspondería en otrro lado.
        /// </summary>
        /// <param name="grupoPedidos"></param>
        /// <returns></returns>
        Detail AuxObtenerDetallePedidoPrincipal(List<Detail> grupoPedidos);
    }
}
