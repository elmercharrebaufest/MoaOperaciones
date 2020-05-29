using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario
{
    public class CartaPorteCTGWSMOAResponse
    {
        public string cp { get; set; }

        public string ctg { get; set; }

        public string fechaCarga { get; set; }

        public string cee { get; set; }

        public string renspa { get; set; }

        public string fechaVto { get; set; }

        public Interviniente intervinientes { get; set; }

        public Destino destino { get; set; }

        public Grano granos { get; set; }

        public Transporte transporte { get; set; }

        public List<Mensaje> mensaje { get; set; }

        public CartaPorteCTGWSMOAResponse() {
            this.mensaje = new List<Mensaje>() { };
            this.intervinientes = new Interviniente();
            this.destino = new Destino();
            this.granos = new Grano();
            this.transporte = new Transporte();
        }
    }
}
