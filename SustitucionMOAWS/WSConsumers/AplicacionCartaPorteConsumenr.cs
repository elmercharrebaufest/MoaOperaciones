using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAWS.CartaPorteDetalleWebServiceMOA;
using SustitucionMOAWS.ContratoDetalleWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class AplicacionCartaPorteConsumer : IAplicacionCartaPorteConsumer
    {
        readonly SI_MPMF_MOAOP_DETALLE_CCPPClient serviceDetalleCCPP = new SI_MPMF_MOAOP_DETALLE_CCPPClient();
        readonly SI_MPMF_MOAOP_DETALLES_CONTRATOClient serviceDetalleContrato = new SI_MPMF_MOAOP_DETALLES_CONTRATOClient();
        public List<ContratoParaAplicacionCartaPorte> ObtenerContratosProveedor(string cartaPorte, string proveedor)
        {
            serviceDetalleCCPP.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            serviceDetalleCCPP.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            var aplicaciones = new ZMPES4300[] { };
            var calidad = new ZMPES4310[] { };
            var pt1000 = new ZMPES6190[] { };
            var response = serviceDetalleCCPP.SI_MPMF_MOAOP_DETALLE_CCPP(cartaPorte, proveedor, ref aplicaciones, ref calidad, ref pt1000);

            return new List<ContratoParaAplicacionCartaPorte> { };
        }

        public List<CartaPorteParaAplicacionCartaPorte> ObtenerCartasPorteProveedor(string contrato, string proveedor)
        {
            serviceDetalleContrato.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            serviceDetalleContrato.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            var ampliaciones = new ZMPES4390[] { };
            var aplicaciones = new ZMPES4400[] { };
            var calidades = new ZMPES4410[] { };
            var caracteres = new ZMPES6200[] { };
            var cond_pago = new ZMPES4710[] { };
            var t_fija = new ZMPES4380[] { };
            var hijos = new ZMPES4420[] { };
            var liquidaciones = new ZMPES6210[] { };
            var pagos = new ZMPES4360[] { };
            var resumenes = new ZMPES4520[] { };
            string response;
            serviceDetalleContrato.SI_MPMF_MOAOP_DETALLES_CONTRATO(
            contrato, proveedor, ref ampliaciones, ref aplicaciones, ref calidades, ref caracteres, ref cond_pago,
            ref t_fija, ref hijos, ref liquidaciones, ref pagos, ref resumenes, PS_RETURN: out response
            );

            return new List<CartaPorteParaAplicacionCartaPorte> { };
        }
    }
}
