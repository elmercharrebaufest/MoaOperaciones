using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAModel.Dto
{
    public class NotificacionSinAdjuntosDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string FechaInicio { get; set; }

        public string FechaFin { get; set; }

        public int HoraInicio { get; set; }
        
        public bool Habilitada { get; set; }

        public bool Borrada { get; set; }

        public string Mensaje { get; set;  }

        public string LinkAdjunto { get; set; }

        public int Prioridad { get; set; }

        public int Leida { get; set; }

        public string FechaCreacion { get; set; }

        public List<int> FiltroRoles { get; set; }

        //public List<NotificacionAdjunto> ArchivosAdjuntos { get; set; }
    }
}
