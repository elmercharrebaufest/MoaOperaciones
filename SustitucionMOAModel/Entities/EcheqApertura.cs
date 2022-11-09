using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class EcheqApertura
    {
        [Key]
        public int Id { get; set; }
        public int OrdenCheque { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int EcheqLiquidacionId { get; set; }
        public decimal ImporteCheque { get; set; }


        [ForeignKey("UsuarioCreacionId")]
        public virtual Usuario UsuarioCreacion { get; set; }

        [ForeignKey("UsuarioModificacionId")]
        public virtual Usuario UsuarioModificacion { get; set; }

        [ForeignKey("EcheqLiquidacionId")]
        public virtual EcheqLiquidacion EcheqLiquidacion { get; set; }
        public bool Estado { get; set; }
    }
}
