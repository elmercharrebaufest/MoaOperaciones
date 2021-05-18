using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class LogPesificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public int? Contrato { get; set; }

        public int? Fijacion { get; set; }

        public decimal? CantidadKilos { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public bool EsCargaMasiva { get; set; }

        public string RutaFisicaArchivo { get; set; }

        public string CodigoProveedor { get; set; }

        public int? IdArchivo { get; set; }

        [ForeignKey("IdArchivo")]
        public virtual Archivo Archivo { get; set; }

        public bool EnvioExitoso { get; set; }
    }
}
