using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class MaterialSolp
    {
        [Key]
        public int Id { get; set; }
        public int? Centro_Id { get; set; }
        public string Codigo { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public int? GrupoArticulo_Id { get; set; }
        public string TipoMaterial { get; set; }
        public int? UnidadMedidaBase_Id { get; set; }
        public int? UnidadMedidaCompras_Id { get; set; }
        public int? UnidadMedidaSalida_Id { get; set; }
        public string TipoValoracion { get; set; }
        public decimal? PrecioMaterial { get; set; }
        public int? GrupoCompras_Id { get; set; }
        public int? CuentaMayor_Id { get; set; }
        public bool Estado { get; set; }

        [ForeignKey("Centro_Id")]
        public virtual TablaSap CentroLogistico { get; set; }
        [ForeignKey("GrupoArticulo_Id")]
        public virtual TablaSap GrupoArticulo { get; set; }
        [ForeignKey("UnidadMedidaBase_Id")]
        public virtual TablaSap UnidadMedidaBase { get; set; }
        [ForeignKey("UnidadMedidaCompras_Id")]
        public virtual TablaSap UnidadMedidaCompras { get; set; }
        [ForeignKey("UnidadMedidaSalida_Id")]
        public virtual TablaSap UnidadMedidaSalida { get; set; }
        [ForeignKey("GrupoCompras_Id")]
        public virtual TablaSap GrupoCompras { get; set; }
        [ForeignKey("CuentaMayor_Id")]
        public virtual TablaSap CuentaMayor { get; set; }
        public string TextoAmpliado { get; set; }
    }
}
