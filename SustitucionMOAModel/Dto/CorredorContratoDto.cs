using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CorredorContratoDto
    {
        public string Key { get; set; }

        public int ContratoId { get; set; }

        public int CorredorId { get; set; }

        public string Corredor { get; set; }

        public string Contrato { get; set; }

        public CorredorContratoDto(int ContratoId, int CorredorId, string Corredor, string Contrato)
        {
            this.Contrato = Contrato;
            this.ContratoId = ContratoId;
            this.CorredorId = CorredorId;
            this.Corredor = Corredor;

            Key = string.Concat(ContratoId, "-", CorredorId);
        }
    }
}
