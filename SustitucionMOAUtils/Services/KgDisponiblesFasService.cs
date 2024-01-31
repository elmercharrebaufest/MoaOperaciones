using SustitucionMOAModel.Entities;
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
        public KgDisponiblesFasService()
        {

        }
        public decimal ObtenerKgDisponiblesContrato(Result contratoSAP, List<OrdenDeCarga> ordenesPorEntregar)
        {
            decimal kilosDisponibles = 0;
            switch (contratoSAP.TipoContrato)
            {
                case TipoContratoFAS.Anticipado:
                    kilosDisponibles = KgDisponiblesContratoAnticipado(contratoSAP,
                            ordenesPorEntregar);
                    break;
                default:
                    kilosDisponibles = KgDisponiblesContratoNormal(contratoSAP,
                        ordenesPorEntregar.Count(orden =>
                            orden.TipoContrato == TipoContratoFAS.Normal &&
                            (!string.IsNullOrEmpty(orden.ContratoSAP) && orden.ContratoSAP == contratoSAP.Contrato) ||
                            (string.IsNullOrEmpty(orden.ContratoSAP) && orden.ContratoIngresado == contratoSAP.Contrato)));
                    break;
            }
            return Math.Round(kilosDisponibles, 2);
        }

        private decimal KgDisponiblesContratoNormal(Result contratoSAP, int cantidadPedidosPendientesDeCrear = 0)
        {
            var kilosEntregaEstandar = ObtenerKgEstandar(contratoSAP);

            var kgEntregadosYPendientesEntrega = contratoSAP.Detalles.Select(det =>
                det.KilosEntrega == 0 ? kilosEntregaEstandar : det.KilosEntrega).Sum();

            var kilosPedidosPendientesCreacion = cantidadPedidosPendientesDeCrear * kilosEntregaEstandar;

            return contratoSAP.KilosTotales - kgEntregadosYPendientesEntrega - kilosPedidosPendientesCreacion;
        }
        private decimal KgDisponiblesContratoAnticipado(Result contratoSAP, List<OrdenDeCarga> ordenesAnticipadasPendientesDeCrear)
        {
            var kilosEntregaEstandar = ObtenerKgEstandar(contratoSAP);

            var kilosConsumidosPorTodosLosPedidos = contratoSAP.Detalles
                .GroupBy(det => det.Pedido)
                .Sum(grupoPedidos => ObtenerKgConsumidosPorGrupoPedidos(
                    grupoPedidos.ToList(), ordenesAnticipadasPendientesDeCrear, kilosEntregaEstandar)
            );

            return contratoSAP.KilosTotales - kilosConsumidosPorTodosLosPedidos;
        }
        private decimal ObtenerKgConsumidosPorGrupoPedidos(
            List<Detail> grupoPedidos,
            List<OrdenDeCarga> ordenesPendientesDeCrear,
            decimal kilosEntregaEstandar
        )
        {
            var pedidoPrincipal = AuxObtenerDetallePedidoPrincipal(grupoPedidos);
            if (pedidoPrincipal is null)
                return 0;

            return ObtenerKgConsumidosPorPedido(grupoPedidos, ordenesPendientesDeCrear, kilosEntregaEstandar, pedidoPrincipal);
        }
        private decimal ObtenerKgConsumidosPorPedido(
            List<Detail> grupoPedidos,
            List<OrdenDeCarga> ordenesPendientesDeCrear,
            decimal kilosEntregaEstandar,
            Detail pedidoPrincipal)
        {
            var kilosEntregados = ObtenerKgEntregadosPorPedido(grupoPedidos);

            var kilosPorConsumir = ObtenerKgPorEntregarParaPedido(pedidoPrincipal, ordenesPendientesDeCrear, kilosEntregaEstandar);
            return kilosEntregados + kilosPorConsumir;
        }
        private decimal ObtenerKgEntregadosPorPedido(List<Detail> grupoPedidos)
        {
            return grupoPedidos.Sum(det => det.KilosEntrega);
        }
        private decimal ObtenerKgPorEntregarParaPedido(Detail pedidoPrincipal, List<OrdenDeCarga> ordenesPendientesDeCrear, decimal kilosEntregaEstandar)
        {
            var numeroFactura = pedidoPrincipal.FacturaLegal;
            var ordenesSinEnviar = ordenesPendientesDeCrear.Count(orden =>
                orden.TipoContrato == TipoContratoFAS.Anticipado && string.IsNullOrEmpty(orden.NumeroEntrega) && (
                (orden.SinSeleccionarFactura && orden.NumeroFactura == numeroFactura) ||
                (!orden.SinSeleccionarFactura && orden.NumeroFacturaSeleccionada == numeroFactura)));
            return ordenesSinEnviar * kilosEntregaEstandar;
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
        public decimal ObtenerKgDisponiblesPedido(
            Result contratoSAP,
            List<OrdenDeCarga> ordenesPendientesDeCrear,
            string numeroPedido)
        {
            var grupoPedidos = contratoSAP.Detalles.Where(det => det.Pedido == numeroPedido).ToList();
            var pedidoPrincipal = AuxObtenerDetallePedidoPrincipal(grupoPedidos);
            var kilosEntregaEstandar = ObtenerKgEstandar(contratoSAP);
            return ObtenerKgDisponiblesPedido(grupoPedidos, ordenesPendientesDeCrear, pedidoPrincipal, kilosEntregaEstandar);
        }
        public decimal ObtenerKgDisponiblesPedido(
            List<Detail> grupoPedidos,
            List<OrdenDeCarga> ordenesPendientesDeCrear,
            Detail pedidoPrincipal,
            decimal kilosEntregaEstandar)
        {
            var kilosConsumidos = ObtenerKgConsumidosPorPedido(grupoPedidos, ordenesPendientesDeCrear, kilosEntregaEstandar, pedidoPrincipal);
            return Math.Round(pedidoPrincipal.CantidadFactura - kilosConsumidos, 2);
        }
        public Detail AuxObtenerDetallePedidoPrincipal(List<Detail> grupoPedidos)
        {
            return grupoPedidos.FirstOrDefault(det =>
                    det.CantidadFactura > 0 && !string.IsNullOrEmpty(det.FacturaLegal));
        }
    }
}
