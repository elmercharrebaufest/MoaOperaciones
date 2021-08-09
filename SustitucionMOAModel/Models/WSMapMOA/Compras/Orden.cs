using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class Orden
    {
        public string ORDER { get; set; }
        public string Descripcion { get; set; }
        public string Clase { get; set; }
        public string Tipo { get; set; }
        public string CompCode { get; set; }
    }
}
