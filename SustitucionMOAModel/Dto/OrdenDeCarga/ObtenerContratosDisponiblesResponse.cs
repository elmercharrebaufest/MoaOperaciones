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
        public decimal KgDisponiblesTn { get; set; }
        public string Label
        {
            get
            {
                return $"{NumeroContrato} - {NombreProducto} - {KgDisponiblesTn}";
            }
        }
        public string NombreProducto
        {
            get
            {
                return Producto.NombreProducto;
            }
        }

        public ContratoOrdenFas(Result contratoSAP, List<Material> productosBD)
        {
            var producto = productosBD
                            .Where(p => p.CodigoSap == contratoSAP.Producto.Trim().TrimStart('0'))
                            .Select(p => new MaterialDto
                            {
                                MaterialId = p.Id,
                                Descripcion = p.Nombre,
                                CodigoSap = p.CodigoSap
                            })
                            .Single();
            NumeroContrato = contratoSAP.Contrato;
            KgDisponiblesTn = ObtenerKgDisponiblesTn(contratoSAP);
            Producto = producto;
        }

        public ContratoOrdenFas(Entities.OrdenDeCarga orden)
        {
            NumeroContrato = orden.ContratoIngresado;
            Producto = new Models.DataAgro.MaterialDto { MaterialId = orden.Producto_Id };
        }

        public decimal ObtenerKgDisponiblesTn(Result contratoSAP)
        {
            return contratoSAP.KilosPendienteEntrega / 1000;
        }
    }
}