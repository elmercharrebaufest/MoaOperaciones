using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Noticia
{
    public class NoticiasDetallesWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<Noticia> noticias { get; set; }
        public List<Notificacion> notificaciones { get; set; }
        public int cantidad { get; set; }

        public NoticiasDetallesWSMOAResponse() {
            this.error = new ErrorWS();
            this.noticias = new List<Noticia>() { };
            this.notificaciones = new List<Notificacion>() { };
            this.cantidad = 0;
        }
    }
}
