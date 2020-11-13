using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Habilitada { get; set; }

        public bool Borrada { get; set; }

        public string LinkAdjunto { get; set; }

        public string Mensaje { get; set; }

        public ICollection<Rol> FiltroRoles { get; set; }

        public ICollection<TipoUsuario> FiltroTipoUsuario { get; set; }
    }
}
