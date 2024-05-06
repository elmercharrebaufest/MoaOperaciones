using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ObtenerContratosDisponiblesResponse
    {
        public List<ContratoOrdenFas> Contratos { get; set; }

        public string Info { get; set; }

        public string Error { get; set; }

        public bool Logout { get; set; }
    }

    public class ContratoOrdenFas
    {
        public string NumeroContrato { get; set; }

        public MaterialDto Producto { get; set; }
        public decimal? KgDisponibles { get; set; }
        public TipoContratoFAS TipoContrato { get; set; }
        public CondicionRetiro CondicionRetiro { get; set; }

        public string DescripcionProducto
        {
            get
            {
                return !string.IsNullOrEmpty(Producto.Abreviacion) ? Producto.Abreviacion : Producto.NombreProducto;
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
                                CodigoSap = p.CodigoSap,
                                Abreviacion = p.Abreviacion,
                            })
                            .Single();
            NumeroContrato = contratoSAP.Contrato;
            Producto = producto;
            TipoContrato = contratoSAP.TipoContrato;
            CondicionRetiro = contratoSAP.PrecioFlete == 0 ? CondicionRetiro.RetiroEnPlanta : CondicionRetiro.PuestoEnDestino;
        }

        public ContratoOrdenFas(Entities.OrdenDeCarga orden)
        {
            NumeroContrato = orden.ContratoIngresado;
            var producto = orden.Producto;

            Producto = new Models.DataAgro.MaterialDto
            {
                MaterialId = orden.Producto_Id,
                Descripcion = producto?.Nombre,
                Abreviacion = producto?.Abreviacion
            };
        }
    }
}