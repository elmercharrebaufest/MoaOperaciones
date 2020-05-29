using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Caracteristica
    {
        public string tipo { get; set; }
        public string descarga { get; set; }
        public string fechaConcerta { get; set; }
        public decimal cantidad { get; set; }
        public string unidad { get; set; }
        public string standardCali { get; set; }
        public string calificacion { get; set; }
        public string procedencia { get; set; }
        public string cosecha { get; set; }
        public decimal toleMin { get; set; }
        public decimal toleMax { get; set; }
        public string entregaMin { get; set; }
        public string entregaMax { get; set; }
        public string estadoBol { get; set; }
        public decimal pagoParcial { get; set; }
        public string pizarraRef { get; set; }
        public string condPagoFija { get; set; }
        public string fechaTopeFija { get; set; }
        public decimal fijaDiariaMin { get; set; }
        public decimal fijaDiariaMax { get; set; }
        public string corredor { get; set; }
        public string nomCorredor { get; set; }
        public string vendedor { get; set; }
        public string nomVendedor { get; set; }
        public decimal importeAPrecio { get; set; }
        public string monedaAPrecio { get; set; }
        public decimal porcAPrecio { get; set; }
        public decimal importeSPrecio { get; set; }
        public string monedaSPrecio { get; set; }
        public decimal porcSPrecio { get; set; }
        public decimal descuentoAcarreo { get; set; }
        public string cdCdg { get; set; }
        public string canje { get; set; }
        public string retenerIva { get; set; }
        public string warrant { get; set; }
        public string pagoDirVend { get; set; }
        public string cesion { get; set; }
        public string confirma { get; set; }
    }

    public class CaracteristicaView : Caracteristica
    {
        public string cantidadString { get; set; }
    }
}
