
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Comunicacion
    {
        [Key]
        public int Id { get; set; }
        public int ComunicacionTipo { get; set; }
        public string ProveedorId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Leida { get; set; }
        public string Detalle { get; set; }
        public string CM05 { get; set; }
        public DateTime? FechaRecomunicacion { get; set; }
        public string Comprobante { get; set; }
    }
}
