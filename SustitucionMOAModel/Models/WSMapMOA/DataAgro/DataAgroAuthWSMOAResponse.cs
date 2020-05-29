using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.DataAgro
{
    public class DataAgroAuthWSMOAResponse
    {
        public Int64 cuit { get; set; }
        public string error { get; set; }
        public string nombreUsuario { get; set; }
        public string url { get; set; }
        public DateTime vencimiento { get; set; }

        public DataAgroAuthWSMOAResponse()
        {
            this.cuit = 0;
            this.error = "";
            this.nombreUsuario = "";
            this.url = "";
            this.vencimiento = new DateTime();
        }

    }
}


