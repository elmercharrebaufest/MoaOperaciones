using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class SolpSubposicion
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int SolpPosicion_Id { get; set; }
        public int Numero { get; set; }
        public int? CodigoServicioSap_Id { get; set; }
        public string Tarea { get; set; }
        public string CuentaMayor { get; set; }
        public decimal? Cantidad { get; set; }
        public int? Unidad_Id { get; set; }
        public decimal? PrecioBruto { get; set; }
        public string CentroCosto { get; set; }
        public string OrdenOT { get; set; }
        public string OrdenInversion { get; set; }
        public string Siniestro { get; set; }
        public int? TipoImputacion_Id { get; set; }

        [ForeignKey("SolpPosicion_Id")]
        public virtual SolpPosicion SolpPosicion { get; set; }
        [ForeignKey("CodigoServicioSap_Id")]
        public virtual TablaSap CodigoServicioSap { get; set; }
        [ForeignKey("Unidad_Id")]
        public virtual TablaSap Unidad { get; set; }
        [ForeignKey("TipoImputacion_Id")]
        public virtual TablaSap TipoImputacionSap { get; set; }
    }
}
