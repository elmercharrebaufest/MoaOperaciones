using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CotizacionHora
    {
        [Key]
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public string Categoria { get; set; }
        public int CantidadPersonas { get; set; }
        public int HorasNormales { get; set; }
        public int HorasNocturnas { get; set; }
        public int HorasExtras { get; set; }
        public string Gremio { get; set; }

        public bool? ConfigurarHora { get; set; }

        

        [ForeignKey("Cotizacion_Id")]
        public virtual Cotizacion Cotizacion { get; set; }
      

    }
}
