using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Echeq
{
    public class EcheqVisualizacionPendientePago
    {
        public string Contrato { get; set; }
        public List<EcheqDocumento> Documentos { get; set; }
        public string Pedido { get; set; }
        public decimal Kilos { get; set; }
        public bool kILOSFieldSpecified { get; set; }
        public decimal KilosPagados { get; set; }
        public bool kILOS_PAGADOSFieldSpecified { get; set; }
        public decimal Precio { get; set; }
        public bool pRECIOFieldSpecified { get; set; }
        public string Moneda { get; set; }
        public string Material { get; set; }
        public string DescripcionMaterial { get; set; }
        public string Fecha { get; set; }
        public string zLSCHField { get; set; }
  
    }

    
}
