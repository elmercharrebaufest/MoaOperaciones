using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAModel.Dto
{
    public class EcheqReporteDto
    {
        public string RazonSocial { get; set; }
        public string Mail { get; set; }
        public string CodigoProveedor { get; set; }
        public string Contrato { get; set; }
        public string Liquidacion { get; set; }
        public bool LiquidacionMarcada { get; set; }
        public bool EcheqGenerados { get { return this.CantidadDeEcheqs > 0; } }
        public DateTime FechaCreacion { get; set; }
        public int CantidadDeEcheqs { get { return this.MontosEcheqs.Count(); } }
        public List<decimal> MontosEcheqs { get; set; } = new List<decimal>();
        public decimal ImporteTotal { get { return this.MontosEcheqs.Sum(); } }
        public string NumeroCOE { get; set; }
        public EcheqReporteDto() { }
    }

}
