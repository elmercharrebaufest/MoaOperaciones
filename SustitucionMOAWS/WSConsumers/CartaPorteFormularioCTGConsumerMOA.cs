using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAWS.CambioPassWebServiceMOA;
using SustitucionMOAWS.CartaPorteFormularioCTGWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CartaPorteFormularioCTGConsumerMOA
    {
        SI_MPMF_MOAOP_BUSCA_CTGClient service = new SI_MPMF_MOAOP_BUSCA_CTGClient();

        public CartaPorteCTGWSMOAResponse request(decimal ctg)
        {
            try
            {
                ZMPES5390 destino = new ZMPES5390();
                ZMPES5380 granos = new ZMPES5380();
                ZMPES5370 intervinientes = new ZMPES5370();
                ZMPES5410[] mensajes = new ZMPES5410[] { };
                ZMPES5400 transporte = new ZMPES5400();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES5360 response = service.SI_MPMF_MOAOP_BUSCA_CTG(ctg, out destino, out granos, out intervinientes, out mensajes, out transporte);
                CartaPorteCTGWSMOAResponse result = map(response, destino, granos, intervinientes, mensajes, transporte);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private CartaPorteCTGWSMOAResponse map(ZMPES5360 response, ZMPES5390 destino, ZMPES5380 granos, ZMPES5370 intervinientes, ZMPES5410[] mensajes, ZMPES5400 transporte)
        {
            CartaPorteCTGWSMOAResponse result = new CartaPorteCTGWSMOAResponse();
            if (response != null)
            {
                result.cee = response.CEE;
                result.cp = response.CP;
                result.ctg = response.CTG;
                try { result.fechaCarga = DateTime.Parse(response.FECHA_CARGA).ToString("yyyy-MM-dd"); } catch { }
                try { result.fechaVto = DateTime.Parse(response.FECHA_VTO).ToString("yyyy-MM-dd"); } catch { }
                result.renspa = response.RENSPA;
            }

            if (destino != null)
            {
                result.destino.value = destino.DIRECCION + ", " + destino.LOCAL_DEST + ", " + destino.PROV_DEST;
                result.destino.direccion = destino.DIRECCION;
                result.destino.localDest = destino.LOCAL_DEST;
                result.destino.provDest = destino.PROV_DEST;
            }

            if (granos != null)
            {
                result.granos.bruto = SAPFormatter.FormatearMonto(granos.BRUTO);
                result.granos.condicional = granos.CONDICIONAL != "" ? true : false;
                result.granos.conforme = granos.CONFORME != "" ? true : false;
                result.granos.contrato = granos.CONTRATO;
                result.granos.cosecha = granos.COSECHA;
                result.granos.decCalidad = granos.DEC_CALIDAD != "" ? true : false;
                result.granos.establecim = granos.ESTABLECIM;
                result.granos.grano = granos.GRANO;
                result.granos.kilosEst = SAPFormatter.FormatearMonto(granos.KILOS_EST);
                result.granos.localOrig = granos.LOCAL_ORIG;
                result.granos.neto = SAPFormatter.FormatearMonto(granos.NETO);
                result.granos.observa = granos.OBSERVA;
                result.granos.pesadaDest = granos.PESADA_DEST != "" ? true : false;
                result.granos.procedencia = granos.PROCEDENCIA;
                result.granos.provOrig = granos.PROV_ORIG;
                result.granos.tara = SAPFormatter.FormatearMonto(granos.TARA);
                result.granos.tipo = granos.TIPO;
            }

            if (intervinientes != null)
            {
                result.intervinientes.chofer = intervinientes.CHOFER;
                result.intervinientes.choferCuit = intervinientes.CHOFER_CUIT;
                result.intervinientes.corrComp = intervinientes.CORR_COMP;
                result.intervinientes.corrCompCuit = intervinientes.CORR_COMP_CUIT;
                result.intervinientes.corrVend = intervinientes.CORR_VEND;
                result.intervinientes.corrVendCuit = intervinientes.CORR_VEND_CUIT;
                result.intervinientes.destinatario = intervinientes.DESTINATARIO;
                result.intervinientes.destinatarioCuit = intervinientes.DESTINATARIO_CUIT;
                result.intervinientes.destino = intervinientes.DESTINO;
                result.intervinientes.destinoCuit = intervinientes.DESTINO_CUIT;
                result.intervinientes.interFlete = intervinientes.INTER_FLETE;
                result.intervinientes.interFleteCuit = intervinientes.INTER_FLETE_CUIT;
                result.intervinientes.interm = intervinientes.INTERM;
                result.intervinientes.intermCuit = intervinientes.INTERM_CUIT;
                result.intervinientes.mercadot = intervinientes.MERCADOT;
                result.intervinientes.mercodatCuit = intervinientes.MERCADOT_CUIT;
                result.intervinientes.remCom = intervinientes.REM_COM;
                result.intervinientes.remComCuit = intervinientes.REM_COM_CUIT;
                result.intervinientes.represent = intervinientes.REPRESENT;
                result.intervinientes.representCuit = intervinientes.REPRESENT_CUIT;
                result.intervinientes.titular = intervinientes.TITULAR;
                result.intervinientes.titularCuit = intervinientes.TITULAR_CUIT;
                result.intervinientes.transpor = intervinientes.TRANSPOR;
                result.intervinientes.transporCuit = intervinientes.TRANSPOR_CUIT;
            }

            if (transporte != null)
            {
                result.transporte.acoplado = transporte.ACOPLADO;
                result.transporte.camion = transporte.CAMION;
                result.transporte.fleteAPag = SAPFormatter.FormatearMonto(transporte.FLETE_A_PAG);
                result.transporte.fletePag = SAPFormatter.FormatearMonto(transporte.FLETE_PAG);
                result.transporte.kmRecorrer = SAPFormatter.FormatearMonto(transporte.KM_RECORRER);
                result.transporte.pagaFlete = transporte.PAGA_FLETE;
                result.transporte.tarifa = SAPFormatter.FormatearMonto(transporte.TARIFA);
                result.transporte.tarifaRef = SAPFormatter.FormatearMonto(transporte.TARIFA_REF);
            }

            foreach (ZMPES5410 mensaje in mensajes) {
                result.mensaje.Add(new Mensaje()
                {
                    codError = mensaje.COD_ERROR,
                    msgError = mensaje.MSG_ERROR 
                });
            }

            return result;

        }
    }
}
