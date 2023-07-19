using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class KgDisponiblesFasService : IKgDisponiblesFasService
    {

        public decimal ObtenerKgDisponiblesContrato(Result contratoSAP)
        {
            decimal kilosDisponibles = 0;
            switch (contratoSAP.TipoContrato)
            {
                case TipoContratoFAS.Anticipado:
                    kilosDisponibles = KgDisponiblesContratoAnticipado(contratoSAP);
                    break;
                default:
                    kilosDisponibles = KgDisponiblesContratoNormal(contratoSAP);
                    break;
            }
            return Math.Round(kilosDisponibles, 2);
        }

        public decimal KgDisponiblesContratoNormal(Result contratoSAP)
        {
            var kgEntregadosYPendientesEntrega = contratoSAP.Detalles.Select(det =>
                det.KilosEntrega == 0 ? ObtenerKgEstandar(contratoSAP) : det.KilosEntrega).Sum();

            return contratoSAP.KilosTotales - kgEntregadosYPendientesEntrega;
        }
        public decimal KgDisponiblesContratoAnticipado(Result contratoSAP)
        {
            var kilosConsumidosPorTodosLosPedidos = contratoSAP.Detalles
                .GroupBy(det => det.Pedido)
                .Sum(grupoPedidos => ObtenerKgEntregadosPorGrupoPedidos(grupoPedidos.ToList())
            );

            return contratoSAP.KilosTotales - kilosConsumidosPorTodosLosPedidos;
        }
        public decimal ObtenerKgEntregadosPorGrupoPedidos(List<Detail> grupoPedidos)
        {
            var pedidoPrincipal = AuxObtenerDetallePedidoPrincipal(grupoPedidos);
            if (pedidoPrincipal is null)
                return 0;

            var kilosConsumidosPorPedido = ObtenerKgEntregadosPorPedido(grupoPedidos);
            return kilosConsumidosPorPedido;
        }
        public decimal ObtenerKgEntregadosPorPedido(List<Detail> grupoPedidos)
        {
            return grupoPedidos.Sum(det => det.KilosEntrega);
        }
        public decimal ObtenerKgEstandar(Result contratoSAP)
        {
            return contratoSAP.Producto.TrimStart('0') == Constante.CODIGO_PELLET_GIRASOL ?
                Constante.KG_STANDARD_PELLET_GIRASOL : Constante.KG_STANDARD;
        }
        public bool PedidoEstaCargado(Detail pedido)
        {
            return !string.IsNullOrEmpty(pedido.Chasis) &&
                !string.IsNullOrEmpty(pedido.Acoplado) &&
                !string.IsNullOrEmpty(pedido.Chofer) &&
                !string.IsNullOrEmpty(pedido.Destinatario) &&
                !string.IsNullOrEmpty(pedido.NombreDestinatario);
        }
        public decimal ObtenerKgDisponiblesPedido(List<Detail> grupoPedidos, Detail pedidoPrincipal)
        {
            var kilosEntregadosPorPedido = ObtenerKgEntregadosPorPedido(grupoPedidos);
            return Math.Round(pedidoPrincipal.CantidadFactura - kilosEntregadosPorPedido);
        }
        public Detail AuxObtenerDetallePedidoPrincipal(List<Detail> grupoPedidos)
        {
            return grupoPedidos.FirstOrDefault(det =>
                    det.CantidadFactura > 0 && !string.IsNullOrEmpty(det.FacturaLegal));
        }
    }
}
