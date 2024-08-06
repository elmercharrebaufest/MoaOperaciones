using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class Area
    {
        [Key]
        public int ID_Area { get; set; }
        public string NombreArea { get; set; }
        public string Descripcion { get; set; }
        [InverseProperty("Areas")]
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
