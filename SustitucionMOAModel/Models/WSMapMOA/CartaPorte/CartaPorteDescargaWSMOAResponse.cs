using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorteDescargaWSMOAResponse
    {
        
        public ErrorWS error { get; set; }
        public List<CartaPorteDescargaView> cartasPorte { get; set; }

        public CartaPorteDescargaWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cartasPorte = new List<CartaPorteDescargaView>() { };
        }
        
    }

    public class CartaPorteDescargaExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CartaPorteDescarga> cartasPorte { get; set; }

        public CartaPorteDescargaExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.cartasPorte = new List<CartaPorteDescarga>() { };
        }
    }
}
