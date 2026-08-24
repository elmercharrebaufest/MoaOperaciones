using System;

namespace SustitucionMOAModel.Models.WSMapMOA.Pesificacion
{
    public class Contrato
    {
        public string NroContrato { get; set; }
        public string Fijacion { get; set; }
        public string Vendedor { get; set; }
        public string NombreVendedor { get; set; }
        public decimal CantidadPendiente { get; set; }
        public decimal Precio { get; set; }
        public decimal MontoPendiente { get; set; }
        public string Unidad { get; set; }
        public string Moneda { get; set; }
    }

    public class ContratoContenido
    {
        public string Contrato { get; set; }
        public string Fijacion { get; set; }
        public decimal Cantidad { get; set; }
        public string Correo { get; set; }
    }
}
