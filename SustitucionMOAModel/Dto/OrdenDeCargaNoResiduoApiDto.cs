using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public abstract class OrdenDeCargaNoResiduoApiDto : OrdenDeCargaApiDtoBase
    {
        private readonly string CUIT_MOA = "30715118773";

        public int Cantidad { get; set; }
        public string CUITCorredor { get; set; }
        public bool Reventa { get; set; }
        public bool FleteMOA { get; set; }
        public string CUITDestinatario { get; set; }
        public string CUITDestino { get; set; }
        public string CUITIntermediarioFlete { get; set; }
        public string DestinoMercaderia { get; set; }
        public bool Escalable { get; set; }
        public string RazonSocialDestinatario { get; set; }
        public string RazonSocialDestino { get; set; }
        public string RazonSocialIntermediarioFlete { get; set; }
        public string RemitenteComercial
        {
            get
            {
                if (Reventa && CUITDestino != CUITCliente && TipoOrden == TipoOrdenes.FASON)
                {
                    return CUITCliente;
                }
                return null;
            }
        }
        public string PagadorFlete
        {
            get
            {
                if (FleteMOA && TipoOrden == TipoOrdenes.FASON)
                {
                    string cUIT_MOA = CUIT_MOA;

                    return cUIT_MOA;
                }
                return CUITCliente;
            }
        }
    }
}
