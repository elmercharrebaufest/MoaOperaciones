using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAModel.Dto
{
    public class EcheqNegocioDto
    {
        public int Id { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public decimal? Kilos { get; set; }
        public decimal KilosPagados { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
        public string MaterialCodigo { get; set; }
        public string Fecha { get; set; }
        public bool MarcaCheque { get; set; }
        public string Clasificacion { get; set; }
        public int ProveedorId { get; set; }
        public string DescripcionMaterial { get; set; }
        public Usuario UsuarioCreacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }
        public MaterialFason Material { get; set; }
        public Proveedor Proveedor { get; set; }
        public List<EcheqLiquidacionDto> Documentos { get; set; }
        public string TipoContrato { get; set; }

        public EcheqNegocioDto() { }

        public EcheqNegocioDto(EcheqNegocio echeqNegocio) 
        {
            this.Id = echeqNegocio.Id;
            this.UsuarioCreacionId = echeqNegocio.UsuarioCreacionId;
            this.UsuarioModificacionId = echeqNegocio.UsuarioModificacionId;
            this.FechaCreacion = echeqNegocio.FechaCreacion;
            this.FechaModificacion = echeqNegocio.FechaModificacion;
            this.Contrato = echeqNegocio.Contrato;
            this.Pedido = echeqNegocio.Pedido;
            this.Kilos = echeqNegocio.Kilos;
            this.KilosPagados = echeqNegocio.KilosPagados;
            this.Precio = echeqNegocio.Precio;
            this.Moneda = echeqNegocio.Moneda;
            this.Fecha = echeqNegocio.Fecha;
            this.MarcaCheque = echeqNegocio.MarcaCheque;
            this.Clasificacion = echeqNegocio.Clasificacion;
            this.ProveedorId = echeqNegocio.ProveedorId;
            this.DescripcionMaterial = echeqNegocio.Material.Nombre;
            this.UsuarioCreacion = echeqNegocio.UsuarioCreacion;
            this.UsuarioModificacion = echeqNegocio.UsuarioModificacion;
            this.Material = echeqNegocio.Material;
            this.Proveedor = echeqNegocio.Proveedor;
            this.Documentos = echeqNegocio.Documentos.Select(a => new EcheqLiquidacionDto(a)).ToList();
        }
    }

}
