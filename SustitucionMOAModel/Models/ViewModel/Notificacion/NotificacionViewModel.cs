using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAModel.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Models.ViewModel.Notificacion
{
    public class NotificacionViewModel
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Habilitada { get; set; }

        public bool Borrada { get; set; }

        public string LinkAdjunto { get; set; }

        public string Mensaje { get; set; }

        public int Prioridad { get; set; }

        [InverseProperty("NotificacionesAsociadas")]
        public virtual ICollection<Rol> FiltroRoles { get; set; }

        //public List<NotificacionAdjuntoViewModel> NotificacionAdjuntos { get; set; }
    }
}