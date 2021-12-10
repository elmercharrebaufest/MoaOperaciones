using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class SolpPosicion
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int Solp_Id { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int? TipoPosicion_Id { get; set; }
        public int? TipoImputacion_Id { get; set; }
        public string TextoGenerico { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public int? PlazoEntrega { get; set; }
        public bool? EsConcluido { get; set; }
        public bool? EsFijacion { get; set; }
        public int? Centro_Id { get; set; }
        public int? Almacen_Id { get; set; }
        public string NombreEntrega { get; set; }
        public string CalleEntrega { get; set; }
        public string NumeroEntrega { get; set; }
        public string CpEntrega { get; set; }
        public string PaisEntrega { get; set; }
        public int? GrupoCompras_Id { get; set; }
        public string Solicitante { get; set; }
        public string NroNecesidad { get; set; }
        public string TextoSuministro { get; set; }
        public string Motivo { get; set; }
        public string Modelo { get; set; }
        public int? GrupoArticulo_Id { get; set; }
        public string CodigosProveedores { get; set; }
        public int? Moneda_Id { get; set; }
        public bool Estado { get; set; }
        public int? Indice { get; set; }

        [ForeignKey("Solp_Id")]
        public virtual Solp Solp { get; set; }
        [ForeignKey("TipoPosicion_Id")]
        public virtual TablaGeneral TipoPosicion { get; set; }
        [ForeignKey("TipoImputacion_Id")]
        public virtual TablaGeneral TipoImputacion { get; set; }
        [ForeignKey("Centro_Id")]
        public virtual TablaSap Centro { get; set; }
        [ForeignKey("Almacen_Id")]
        public virtual TablaSap Almacen { get; set; }
        [ForeignKey("GrupoCompras_Id")]
        public virtual TablaSap GrupoCompras { get; set; }
        [ForeignKey("GrupoArticulo_Id")]
        public virtual TablaSap GrupoArticulo { get; set; }
        [ForeignKey("Moneda_Id")]
        public virtual TablaSap Moneda { get; set; }

        public virtual ICollection<SolpSubposicion> Subposiciones { get; set; }
        public virtual ICollection<SolpProveedor> Proveedores { get; set; }

    }
}
