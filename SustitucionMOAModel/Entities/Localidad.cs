using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Localidad
    {
        [Key]
        public int LocalidadId { get; set; }

        public int CodLocalidad { get; set; }

        public string Nombre { get; set; }

        public int ProvinciaId { get; set; }

        public int PartidoId { get; set; }

        [ForeignKey("ProvinciaId")]
        public Provincia Provincia { get; set; }

        [ForeignKey("PartidoId")]
        public Partido Partido { get; set; }
    }
}
