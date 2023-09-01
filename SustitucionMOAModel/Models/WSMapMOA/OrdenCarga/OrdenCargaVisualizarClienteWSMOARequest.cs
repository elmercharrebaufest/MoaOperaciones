using SustitucionMOAModel.Enums;
using System.Collections.Generic;

namespace SustitucionMOAModel.Models.WSMapMOA.OrdenCarga
{
    public class OrdenCargaVisualizarClienteWSMOARequest
    {
        public string Cliente { get; set; }
        public string Contrato { get; set; }
        public string Corredor { get; set; }
        public List<FechaWS> Fechas { get; set; }
        public string Material { get; set; }
        public bool Pendiente { get; set; }
        public TipoContratoFAS TipoContrato { get; set; }

        public OrdenCargaVisualizarClienteWSMOARequest()
        {
            Cliente = string.Empty;
            Contrato = string.Empty;
            Corredor = string.Empty;
            Material = string.Empty;
        }
    }
}
