using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class PosicionPendienteDto
    {
        public string NroSolp { get; set; }
        public int NumeroPosicion { get; set; }
        public string CodigoMaterial { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Pedido { get; set; }
        public decimal Pendiente { get { return Cantidad - Pedido; } }

    }
}