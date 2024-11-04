using System;

namespace SustitucionMOAModel.Dto.OrdenResiduos
{
    public class IngresosEgresosResiduos
    {
        public double PesadaBruto { get; set; }
        public double PesadaTara { get; set; }
        public double PesadaNeto { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Balanza { get; set; }
        public string UniMedCant { get; set; }
        public string NroCertificacion { get; set; }
        public long OrdenCargaInterna { get; set; }
        public long IdOperaciones { get; set; }
    }
}