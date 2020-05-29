using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario
{
    public class CCPPFormularioElementPosition
    {
        public string value { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public CCPPFormularioElementPosition(string value, int x, int y) {
            this.value = value;
            this.x = x;
            this.y = y;
        }
    }
}
