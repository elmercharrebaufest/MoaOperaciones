using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor
{
    public class VendedoresWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<Vendedor> vendedores { get; set; }

        public VendedoresWSMOAResponse() {
            this.error = new ErrorWS();
            this.vendedores = new List<Vendedor>() { };
        }
    }
}
