using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class OrganizacionDeCompra
    {
        [Key]
        public string Id { get; set; }

        public string Descripcion { get; set; }
    }
}
