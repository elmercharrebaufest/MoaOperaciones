using Newtonsoft.Json;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Util;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ObtenerContratosDisponiblesResponse
    {
        //[JsonProperty("contratosDisponibles")]
        public List<ContratoOrdenFas> Contratos { get; set; }

        //[JsonProperty("info")]
        public string Info { get; set; }

        //[JsonProperty("error")]
        public string Error { get; set; }

        //[JsonProperty("logout")]
        public bool Logout { get; set; }
    }

    public class ContratoOrdenFas
    {
        public string NumeroContrato { get; set; }

        public MaterialDto Producto { get; set; }
        public decimal? KgDisponibles { get; set; }
        public string Label
        {
            get
            {
                var kg = KgDisponibles == null ? "" : $" {KgDisponibles} kg Disp.";
                return $"{NumeroContrato} - {DescripcionProducto}{kg}";
            }
        }
        public string DescripcionProducto
        {
            get
            {
                return !string.IsNullOrEmpty(Producto.Abreviacion) ? Producto.Abreviacion : Producto.NombreProducto;
            }
        }

        public TipoContratoFAS TipoContrato { get; set; }
        public ContratoOrdenFas(Result contratoSAP, List<Material> productosBD)
        {
            var producto = productosBD
                            .Where(p => p.CodigoSap == contratoSAP.Producto.Trim().TrimStart('0'))
                            .Select(p => new MaterialDto
                            {
                                MaterialId = p.Id,
                                Descripcion = p.Nombre,
                                CodigoSap = p.CodigoSap,
                                Abreviacion = p.Abreviacion,
                            })
                            .Single();
            NumeroContrato = contratoSAP.Contrato;
            KgDisponibles = ObtenerKgDisponibles(contratoSAP);
            Producto = producto;
            TipoContrato = contratoSAP.TipoContrato;
        }

        public ContratoOrdenFas(Entities.OrdenDeCarga orden)
        {
            NumeroContrato = orden.ContratoIngresado;
            var producto = orden.Producto != null ? orden.Producto : null;

            Producto = new Models.DataAgro.MaterialDto
            {
                MaterialId = orden.Producto_Id,
                Descripcion = producto?.Nombre,
                Abreviacion = producto?.Abreviacion
            };
        }
        public ContratoOrdenFas(Entities.OrdenDeCarga orden, Result contratoSAP)
        {
            NumeroContrato = orden.ContratoIngresado;
            var producto = orden.Producto != null ? orden.Producto : null;

            Producto = new Models.DataAgro.MaterialDto
            {
                MaterialId = orden.Producto_Id,
                Descripcion = producto?.Nombre,
                Abreviacion = producto?.Abreviacion
            };
            KgDisponibles = ObtenerKgDisponibles(contratoSAP);
        }
        public decimal ObtenerKgDisponibles(Result contratoSAP)
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

        private decimal KgDisponiblesContratoNormal(Result contratoSAP)
        {
            var kgEntregadosYPendientesEntrega = contratoSAP.Detalles.Select(det =>
                det.KilosEntrega == 0 ? ObtenerKgEstandar(contratoSAP) : det.KilosEntrega).Sum();

            return contratoSAP.KilosTotales - kgEntregadosYPendientesEntrega;
        }
        private decimal KgDisponiblesContratoAnticipado(Result contratoSAP)
        {
            var kilosDisponibles = contratoSAP.KilosTotales;
            contratoSAP.Detalles.GroupBy(det => det.Pedido).ToList().ForEach(grupoPedidos =>
            {
                var pedidoPrincipal = grupoPedidos.FirstOrDefault(det =>
                        det.CantidadFactura > 0 && !string.IsNullOrEmpty(det.FacturaLegal)
                    );
                if (pedidoPrincipal is null)
                    return;
                if (PedidoEstaCargado(pedidoPrincipal))
                {
                    kilosDisponibles -= pedidoPrincipal.CantidadFactura;
                    return;
                }
                grupoPedidos.ToList().ForEach(det => kilosDisponibles -= det.KilosEntrega);
            });

            return kilosDisponibles;
        }
        private decimal ObtenerKgEstandar(Result contratoSAP)
        {
            return contratoSAP.Producto.TrimStart('0') == Constante.CODIGO_PELLET_GIRASOL ?
                Constante.KG_STANDARD_PELLET_GIRASOL : Constante.KG_STANDARD;
        }
        private bool PedidoEstaCargado(Detail pedido)
        {
            return !string.IsNullOrEmpty(pedido.Chasis) &&
                !string.IsNullOrEmpty(pedido.Acoplado) &&
                !string.IsNullOrEmpty(pedido.Chofer) &&
                !string.IsNullOrEmpty(pedido.Destinatario) &&
                !string.IsNullOrEmpty(pedido.NombreDestinatario);
        }
    }
}