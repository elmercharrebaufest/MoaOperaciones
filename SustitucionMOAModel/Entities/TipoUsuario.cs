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

        public static TipoUsuario GetTipoAmbos()
        {
            return new TipoUsuario()
            {
                Id = 1,
                Nombre = "Ambos",
                NombreCorto = "A"
            };
        }

        public static TipoUsuario GetTipoGranos()
        {
            return new TipoUsuario()
            {
                Id = 2,
                Nombre = "Granos",
                NombreCorto = "G"
            };
        }
        public static TipoUsuario GetTipoNoGranos()
        {
            return new TipoUsuario()
            {
                Id = 3,
                Nombre = "No Granos",
                NombreCorto = "NG"
            };
        }


        public static TipoUsuario GetTipoCorredor()
        {
            return new TipoUsuario()
            {
                Id = 4,
                Nombre = "Corredor",
                NombreCorto = "CORR"
            };
        }

        public static TipoUsuario GetTipoCliente()
        {
            return new TipoUsuario()
            {
                Id = 5,
                Nombre = "Cliente",
                NombreCorto = "CLI"
            };
        }
    }
}
