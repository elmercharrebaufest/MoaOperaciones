using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Adjudicacion
    {
        [Key]
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public int Solp_Id { get; set; }
        public string NumeroOrdenDeCompra { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public int Moneda_Id { get; set; }
        public decimal MontoTotal { get; set; }

        public string TextoDeCabecera { get; set; }
        public string CondicionesDeEntrega { get; set; }
        public string CondicionesDePago { get; set; }
        public string Garantias { get; set; }


        [ForeignKey("UsuarioCreador_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Solp_Id")]
        public virtual Solp Solp { get; set; }

        [ForeignKey("Cotizacion_Id")]
        public virtual Cotizacion Cotizacion { get; set; }

        [InverseProperty("Adjudicacion")]
        public virtual ICollection<AdjudicacionPosicion> Posiciones { get; set; } = new List<AdjudicacionPosicion>();

        [ForeignKey("Moneda_Id")]
        public virtual TablaSap Moneda { get; set; }
        public DateTime? FechaLiberacionSap { get; set; }
    }
}
