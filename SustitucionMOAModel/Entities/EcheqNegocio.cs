using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Dto;

namespace SustitucionMOAModel.Entities
{
    public class EcheqNegocio
    {
        [Key]
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
        public int MaterialId { get; set; }
        public string Fecha { get; set; }
        public bool MarcaCheque { get; set; }
        public string Clasificacion { get; set;  }
        public int ProveedorId { get; set; }


        [ForeignKey("UsuarioCreacionId")]
        public virtual Usuario UsuarioCreacion { get; set; }

        [ForeignKey("UsuarioModificacionId")]
        public virtual Usuario UsuarioModificacion { get; set; }

        [ForeignKey("MaterialId")]
        public virtual MaterialFason Material { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }

        [InverseProperty("EcheqNegocio")]
        public virtual List<EcheqLiquidacion> Documentos { get; set; }

        public EcheqNegocio() { }

        public EcheqNegocio(EcheqNegocioDto echeqNegocio)
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
            this.UsuarioCreacion = echeqNegocio.UsuarioCreacion;
            this.UsuarioModificacion = echeqNegocio.UsuarioModificacion;
            this.Material = echeqNegocio.Material;
            this.Proveedor = echeqNegocio.Proveedor;
            this.Documentos = echeqNegocio.Documentos.Select(a => new EcheqLiquidacion(a)).ToList();
        }
    }

    
}
