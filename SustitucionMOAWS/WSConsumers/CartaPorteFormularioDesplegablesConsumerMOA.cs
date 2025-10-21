using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAWS.CartaPorteFormularioDesplegablesWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class CartaPorteFormularioDesplegablesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public CartaPorteFormularioDropdownsWSMOAResponse request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_CP_DESPLEGABLES()
                    {
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CP_DESPLEGABLES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_CP_DESPLEGABLES(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CP_DESPLEGABLES response");
                    Log.Info(response.ToXml());
                    CartaPorteFormularioDropdownsWSMOAResponse result = MapSinPI(response);
                    return result;
                }
                else
                {
                    SI_MPMF_MOAOP_CP_DESPLEGABLESClient service = new SI_MPMF_MOAOP_CP_DESPLEGABLESClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    CartaPorteFormularioDesplegablesWebServiceMOA.Z_MPMF_MOAOP_CP_DESPLEGABLESResponse response = service.SI_MPMF_MOAOP_CP_DESPLEGABLES("");
                    CartaPorteFormularioDropdownsWSMOAResponse result = Map(response);
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private CartaPorteFormularioDropdownsWSMOAResponse Map(CartaPorteFormularioDesplegablesWebServiceMOA.Z_MPMF_MOAOP_CP_DESPLEGABLESResponse response)
        {
            CartaPorteFormularioDropdownsWSMOAResponse result = new CartaPorteFormularioDropdownsWSMOAResponse();

            foreach (CartaPorteFormularioDesplegablesWebServiceMOA.ZMPDE10780 cosecha in response.T_COSECHAS)
            {
                result.cosechas.Add(new FormularioDropdownElement()
                {
                    label = cosecha.COSECHA,
                    value = cosecha.COSECHA
                });
            }

            foreach (CartaPorteFormularioDesplegablesWebServiceMOA.ZMPDE10770 localidad in response.T_LOCALIDADES)
            {
                result.localidades.Add(new FormularioDropdownElement()
                {
                    label = localidad.LOCALIDADES,
                    value = localidad.LOCALIDADES
                });
            }

            foreach (CartaPorteFormularioDesplegablesWebServiceMOA.ZMPDE10760 provincia in response.T_PROVINCIAS)
            {
                result.provincias.Add(new FormularioDropdownElement()
                {
                    label = provincia.PROVINCIA,
                    value = provincia.PROVINCIA
                });
            }

            foreach (CartaPorteFormularioDesplegablesWebServiceMOA.ZMPDE10750 destinatario in response.T_DESTINATARIO)
            {
                result.destinatarios.Add(new FormularioDropdownElement()
                {
                    value = destinatario.CUIT,
                    label = destinatario.RAZON_SOC
                });
            }

            foreach (CartaPorteFormularioDesplegablesWebServiceMOA.ZMPDE10740 destino in response.T_DESTINO)
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
        private CartaPorteFormularioDropdownsWSMOAResponse MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.Z_MPMF_MOAOP_CP_DESPLEGABLESResponse response)
        {
            CartaPorteFormularioDropdownsWSMOAResponse result = new CartaPorteFormularioDropdownsWSMOAResponse();

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPDE10780 cosecha in response.T_COSECHAS)
            {
                result.cosechas.Add(new FormularioDropdownElement()
                {
                    label = cosecha.COSECHA,
                    value = cosecha.COSECHA
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPDE10770 localidad in response.T_LOCALIDADES)
            {
                result.localidades.Add(new FormularioDropdownElement()
                {
                    label = localidad.LOCALIDADES,
                    value = localidad.LOCALIDADES
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPDE10760 provincia in response.T_PROVINCIAS)
            {
                result.provincias.Add(new FormularioDropdownElement()
                {
                    label = provincia.PROVINCIA,
                    value = provincia.PROVINCIA
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPDE10750 destinatario in response.T_DESTINATARIO)
            {
                result.destinatarios.Add(new FormularioDropdownElement()
                {
                    value = destinatario.CUIT,
                    label = destinatario.RAZON_SOC
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPDE10740 destino in response.T_DESTINO)
            {
                result.destinos.Add(new FormularioDestinoElement()
                {
                    label = destino.DIRECCION + ", " + destino.LOCALIDAD + ", " + destino.PROVINCIA,
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
