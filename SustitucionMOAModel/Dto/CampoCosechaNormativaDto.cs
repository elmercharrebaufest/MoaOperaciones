using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CampoCosechaNormativaDto
    {
        public int Id { get; set; }
        public int CampoCosecha_Id { get; set; }
        public int TipoNormativa_Id { get; set; }
        public TipoNormativaDto TipoNormativa { get; set; }
        public double ToneladasAprobadas { get; set; }
        public string MotivoRechazo { get; set; }
        public bool Validado { get; set; }
        public int ValidadoPor { get; set; }
        public DateTime? ValidadoFecha { get; set; }
        public string DescripcionNormativa { get; set; }
    }
}
