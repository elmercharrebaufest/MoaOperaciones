using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class FacturaOrdenCarga
    {
        public string NumeroFactura { get; set; }
        public string NumeroPedido { get; set; }
        public decimal? KgDisponibles { get; set; }
        public string Label
        {
            get
            {
                var kgLabel = KgDisponibles != null ? $" - {KgDisponibles} kg Disp." : "";
                return $"{NumeroFactura}{kgLabel}";
            }
        }

        public FacturaOrdenCarga(Detail detail, decimal? kgDisponibles=null)
        {
            NumeroFactura = detail.FacturaLegal;
            NumeroPedido = detail.Pedido;
            KgDisponibles = kgDisponibles;
        }
    }
}
