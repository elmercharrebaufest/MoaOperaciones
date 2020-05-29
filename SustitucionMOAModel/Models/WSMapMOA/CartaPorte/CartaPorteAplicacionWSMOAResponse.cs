using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorteAplicacionWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CartaPorteAplicacionView> cartasPorte { get; set; }

        public CartaPorteAplicacionWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cartasPorte = new List<CartaPorteAplicacionView>() { };
        }
    }

    public class CartaPorteAplicacionExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CartaPorteAplicacion> cartasPorte { get; set; }

        public CartaPorteAplicacionExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cartasPorte = new List<CartaPorteAplicacion>() { };
        }
    }
}
