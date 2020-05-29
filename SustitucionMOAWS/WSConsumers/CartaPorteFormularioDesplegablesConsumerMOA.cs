using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAWS.CartaPorteFormularioDesplegablesWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CartaPorteFormularioDesplegablesConsumerMOA
    {
        SI_MPMF_MOAOP_CP_DESPLEGABLESClient service = new SI_MPMF_MOAOP_CP_DESPLEGABLESClient();

        public CartaPorteFormularioDropdownsWSMOAResponse request()
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Z_MPMF_MOAOP_CP_DESPLEGABLESResponse response = service.SI_MPMF_MOAOP_CP_DESPLEGABLES("");
                CartaPorteFormularioDropdownsWSMOAResponse result = map(response);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private CartaPorteFormularioDropdownsWSMOAResponse map(Z_MPMF_MOAOP_CP_DESPLEGABLESResponse response)
        {
            CartaPorteFormularioDropdownsWSMOAResponse result = new CartaPorteFormularioDropdownsWSMOAResponse();

            foreach (ZMPDE10780 cosecha in response.T_COSECHAS)
            {
                result.cosechas.Add(new FormularioDropdownElement()
                {
                    label = cosecha.COSECHA,
                    value = cosecha.COSECHA
                });
            }

            foreach (ZMPDE10770 localidad in response.T_LOCALIDADES)
            {
                result.localidades.Add(new FormularioDropdownElement()
                {
                    label = localidad.LOCALIDADES,
                    value = localidad.LOCALIDADES
                });
            }

            foreach (ZMPDE10760 provincia in response.T_PROVINCIAS)
            {
                result.provincias.Add(new FormularioDropdownElement()
                {
                    label = provincia.PROVINCIA,
                    value = provincia.PROVINCIA
                });
            }

            foreach (ZMPDE10750 destinatario in response.T_DESTINATARIO)
            {
                result.destinatarios.Add(new FormularioDropdownElement()
                {
                    value = destinatario.CUIT,
                    label = destinatario.RAZON_SOC
                });
            }

            foreach (ZMPDE10740 destino in response.T_DESTINO)
            {
                result.destinos.Add(new FormularioDestinoElement()
                {
                    label = destino.DIRECCION +", "+ destino.LOCALIDAD + ", " + destino.PROVINCIA,
                    value = destino.DIRECCION + ", " + destino.LOCALIDAD + ", " + destino.PROVINCIA,
                    direccion = destino.DIRECCION,
                    localidad = destino.LOCALIDAD,
                    provincia = destino.PROVINCIA
                });
            }

            return result;
        }
    }

}
