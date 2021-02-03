using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrdenDeCargaDto
    {
        public int Id { get; set; }

        public string CUITCliente { get; set; }

        public string DescripcionEstado { get; set; }

        public string ColorSemaforo { get; set; }

    }
}
