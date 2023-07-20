using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System.Collections.Generic;
using System.Linq;

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
        //public string Label
        //{
        //    get
        //    {
        //        var kg = KgDisponiblesTn == null ? "" : $" {KgDisponiblesTn} kg Disp.";
        //        return $"{NumeroContrato} - {DescripcionProducto}{kg}";
        //    }
        //}
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
            var producto = orden.Producto;

            Producto = new Models.DataAgro.MaterialDto
            {
                MaterialId = orden.Producto_Id,
                Descripcion = producto?.Nombre,
                Abreviacion = producto?.Abreviacion
            };
            KgDisponiblesTn = ObtenerKgDisponiblesTn(contratoSAP);
        }
        public decimal ObtenerKgDisponiblesTn(Result contratoSAP)
        {
            var kgEntregadosYPendientesEntrega = contratoSAP.Detalles.Select(det =>
                det.KilosEntrega== 0 ? ObtenerKgEstandar(contratoSAP) : det.KilosEntrega).Sum();
            
            return Math.Round(contratoSAP.KilosTotales - kgEntregadosYPendientesEntrega, 2);
        }
        private decimal ObtenerKgEstandar(Result contratoSAP) {
            return contratoSAP.Producto.TrimStart('0') == "99709" ? 20000 : 30000;
        }
    }
}