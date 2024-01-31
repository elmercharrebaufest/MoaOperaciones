using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSRequests.OrdenCarga
{
    public class CrearEntregaRequest
    {
        public string Documento { get; set; }
        public decimal Kilos { get; set; }
        public string NombreConductor { get; set; }
        public string PatenteAcoplado { get; set; }
        public string PatenteChasis { get; set; }
        public string Pedido { get; set; }
        public string TipoDocumento { get; set; }
        public string Transportista { get; set; }
        public string CuitDestinatario { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string CuitDestino { get; set; }
        public string RazonSocialDestino { get; set; }
        public bool Reventa { get; set; }
        public string TransportistaReal { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public string DestinoMercaderia { get; set; }
    }
}
