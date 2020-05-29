using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAWS.ContactoMailCategoriasWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class ContactoMailCategoriasConsumerMOA
    {
        SI_MPMF_MOAOP_CATEG_CONTACTOClient service = new SI_MPMF_MOAOP_CATEG_CONTACTOClient();

        public List<CategoriaContacto> request()
        {
            try
            {
                ZMPES5330[] categorias = new ZMPES5330 []{ };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Z_MPMF_MOAOP_CATEG_CONTACTOResponse result = service.SI_MPMF_MOAOP_CATEG_CONTACTO(categorias);
               return map(result.CATEGORIAS);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private List<CategoriaContacto> map(ZMPES5330[] categorias) {

            List<CategoriaContacto> result = new List<CategoriaContacto>() { };

            foreach (ZMPES5330 categoria in categorias) {
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
