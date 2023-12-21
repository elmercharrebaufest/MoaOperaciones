using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class NotificacionAdjunto
    {
        [Key]
        public int AdjuntoId { get; set; }
        public int NotificacionId { get; set; }
        public string AdjuntoTipo { get; set; }
        public string AdjuntoNombre { get; set; }
        public string AdjuntoContenido { get; set; }
    }
}
