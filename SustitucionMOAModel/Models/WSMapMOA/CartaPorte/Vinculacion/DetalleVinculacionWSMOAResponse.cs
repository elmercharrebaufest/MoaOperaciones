using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Vinculacion
{
    public class DetalleVinculacionWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CartaPorteVinculacion> cartasPorte { get; set; }

        public DetalleVinculacionWSMOAResponse()
        {
            this.error = new ErrorWS() { };
            this.cartasPorte = new List<CartaPorteVinculacion>() { };
        }
    }
}
