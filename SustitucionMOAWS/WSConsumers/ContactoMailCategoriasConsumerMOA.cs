using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAWS.ContactoMailCategoriasWebServiceMOA;
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
    public class ContactoMailCategoriasConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public List<CategoriaContacto> request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_CATEG_CONTACTO()
                    {
                        
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CATEG_CONTACTO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_CATEG_CONTACTO(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CATEG_CONTACTO response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.CATEGORIAS);
                }
                else
                {
                    SI_MPMF_MOAOP_CATEG_CONTACTOClient service = new SI_MPMF_MOAOP_CATEG_CONTACTOClient();
                    ContactoMailCategoriasWebServiceMOA.ZMPES5330[] categorias = new ContactoMailCategoriasWebServiceMOA.ZMPES5330[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    ContactoMailCategoriasWebServiceMOA.Z_MPMF_MOAOP_CATEG_CONTACTOResponse result = service.SI_MPMF_MOAOP_CATEG_CONTACTO(categorias);
                    return Map(result.CATEGORIAS);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private List<CategoriaContacto> Map(ContactoMailCategoriasWebServiceMOA.ZMPES5330[] categorias)
        {

            List<CategoriaContacto> result = new List<CategoriaContacto>() { };

            foreach (ContactoMailCategoriasWebServiceMOA.ZMPES5330 categoria in categorias)
            {
                result.Add(new CategoriaContacto()
                {
                    value = categoria.ID,
                    label = categoria.DESCRIPCION,
                    camposAdicionales = categoria.FORM_ESPECIAL != null ? categoria.FORM_ESPECIAL.ToUpper() : ""
                });
            }

            return result;
        }
        private List<CategoriaContacto> MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5330[] categorias) {

            List<CategoriaContacto> result = new List<CategoriaContacto>() { };

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5330 categoria in categorias) {
                result.Add(new CategoriaContacto()
                {
                    value = categoria.ID,
                    label = categoria.DESCRIPCION,
                    camposAdicionales = categoria.FORM_ESPECIAL != null ? categoria.FORM_ESPECIAL.ToUpper() : ""
                });
            }

            return result;
        }


    }
}
