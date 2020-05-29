using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DBMap.RYD
{
    public class InputsBalanzas
    {
        public List<DbElement> tipo { get; set; }
        public List<DbElement> codigoCabezal { get; set; }
        public List<DbElement> itc { get; set; }
        public List<DbElement> nroPuesto { get; set; }
        public List<DbElement> exportadores { get; set; }
        public List<DbElement> tipoAcceso { get; set; }

        public InputsBalanzas()
        {
            this.tipo = new List<DbElement>() { };
            this.codigoCabezal = new List<DbElement>() { };
            this.itc = new List<DbElement>() { };
            this.nroPuesto = new List<DbElement>() { };
            this.exportadores = new List<DbElement>() { };
            this.tipoAcceso = new List<DbElement>() { };
        }
    }
}
