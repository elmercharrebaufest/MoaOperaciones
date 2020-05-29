using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorteWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CartaPorteView> cartasPorte { get; set; }

        public CartaPorteWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cartasPorte = new List<CartaPorteView>() { };
        }
    }

    public class CartaPorteExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CartaPorte> cartasPorte { get; set; }

        public CartaPorteExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cartasPorte = new List<CartaPorte>() { };
        }
    }
}
