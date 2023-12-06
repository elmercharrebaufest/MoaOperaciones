using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class AplicacionCartaPorteCargaMasivaFila
    {
        [Key]
        public int Id { get; set; }

        public int AplicacionCartaPorteCargaMasivaId { get; set; }

        [ForeignKey(nameof(AplicacionCartaPorteCargaMasivaId))]
        public virtual AplicacionCartaPorteCargaMasiva AplicacionCartaPorteCargaMasiva { get; set; }

        public short FilaNumero { get; set; }

        public string Contrato { get; set; }

        public string CartaPorte { get; set; }

        public string Kilos { get; set; }

        public string Error { get; set; }
    }
}
