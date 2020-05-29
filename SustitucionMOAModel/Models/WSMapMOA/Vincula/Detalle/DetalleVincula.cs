using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle
{
    public class DetalleVincula
    {
        public string cartaPorte { get; set; }
        public string fecha { get; set; }
        public decimal kgRecibidos { get; set; }
        public decimal kgLiquidados { get; set; }
        public string unidad { get; set; }
    }

    public class DetalleVinculaView : DetalleVincula
    {
        public string kgRecibidosString { get; set; }
        public string kgLiquidadosString { get; set; }
    }
}
