using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.ViewModel.Notificacion
{
    public class NotificacionAdjuntoViewModel
    {
        public int AdjuntoId { get; set; }
        public int NotificacionId { get; set; }
        public string AdjuntoTipo { get; set; }
        public string AdjuntoNombre { get; set; }
        public string AdjuntoContenido { get; set; }
    }
}
