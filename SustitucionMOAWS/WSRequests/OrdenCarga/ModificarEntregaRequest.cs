using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSRequests.OrdenCarga
{
    public class ModificarEntregaRequest
    {
        public string Chasis { get; set; }
        public string NumeroEntrega { get; set; }
        public string Acoplado { get; set; }
        public string Chofer { get; set; }
        public string TipoDoc { get; set; }
        public string Documento { get; set; }
        public string CUITTransporte { get; set; }
        public string CUITIntermediarioFlete { get; set; }

        public ModificarEntregaRequest() { }

        public ModificarEntregaRequest(OrdenDeCarga orden)
        {
            Chasis = orden.ChasisAcoplado;
            Acoplado = orden.PatenteAcoplado;
            Chofer = orden.NombreChofer;
            NumeroEntrega = orden.NumeroEntrega;
            TipoDoc = string.Empty;
            Documento = string.Empty;
            CUITTransporte = orden.CUITTransporte;
            CUITIntermediarioFlete = orden.CUITIntermediarioFlete;
        }
    }
}
