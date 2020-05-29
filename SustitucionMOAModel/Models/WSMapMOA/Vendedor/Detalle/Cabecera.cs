using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle
{
    public class Cabecera
    {
        public string proveedor { get; set; }

        public string descripcion { get; set; }

        public string rg2300 { get; set; }

        public string cuit { get; set; }

        public string actividadAfip { get; set; }

        public string calleFiscal { get; set; }

        public string locaFiscal { get; set; }

        public string provFiscal { get; set; }

        public string cpFiscal { get; set; }

        public string calleEnvio { get; set; }

        public string locaEnvio { get; set; }

        public string provEnvio { get; set; }

        public string cpEnvio { get; set; }

        public string cateFiscalRet { get; set; }

        public string nroInscIibb { get; set; }

        public string cateFiscalIibb { get; set; }

        public string tribuDifPrecio { get; set; }

        public string fecha1276 { get; set; }

        public string cm05 { get; set; }

        public string fechaLegajo { get; set; }

        public List<string> categorias { get; set; }

        public Cabecera(){
            this.categorias = new List<string>() { };
        }
    }
}
