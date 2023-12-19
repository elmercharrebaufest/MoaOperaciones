using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class RegionSap
    {
        [Key]
        public int Id { get; set; }
        public string CodigoPais { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }

        [ForeignKey("CodigoPais")]
        public virtual PaisesSap PaisesSap { get; set; }
    }
}
