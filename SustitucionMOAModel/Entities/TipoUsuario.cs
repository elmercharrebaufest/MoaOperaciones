using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Entities
{
    public class TipoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }

        public static TipoUsuario GetTipoGranos()
        {
            return new TipoUsuario()
            {
                Id = 1,
                Nombre = "Ambos",
                NombreCorto = "A"
            };
        }
    }
}
