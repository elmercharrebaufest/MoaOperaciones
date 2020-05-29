using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle
{
    public class VendedorDetalleWSMOAResponse
    {
        public string error { get; set; }
        public List<Cabecera> cabeceras { get; set; }
        public List<Convenio> convenios { get; set; }
        public List<Cuenta> cuentas { get; set; }
        public List<Exencion> exenciones { get; set; }

        public VendedorDetalleWSMOAResponse()
        {
            this.cabeceras = new List<Cabecera>() { };
            this.convenios = new List<Convenio>() { };
            this.cuentas = new List<Cuenta>() { };
            this.exenciones = new List<Exencion>() { };
        }
    }
}
