using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PaisesSap
    {
        [Key]
        public string Codigo { get; set; }
        public string DenominacionPais { get; set; }
    }
}
