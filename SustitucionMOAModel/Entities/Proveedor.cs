using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Entities
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CUIT { get; set; }
        public string ComercialAsignado { get; set; }
        public bool Habilitado { get; set; }
        public TipoProveedor TipoProveedor { get; set; }
}
}
