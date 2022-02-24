using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.DataAgroServices;

namespace SustitucionMOAWS.WSConsumers
{
    public class DataAgroConsumer
    {
        private DataAgroServicesClient service = new DataAgroServicesClient();

        public DataAgroConsumer()
        {
            service.ClientCredentials.UserName.UserName = string.Concat(DataAgroWSCredential.getDominio(), @"\", DataAgroWSCredential.getUserName());
            service.ClientCredentials.UserName.Password = DataAgroWSCredential.getPassword();
            //service.ClientCredentials.UserName. = DataAgroWSCredential.getDominio();
        }


        public ResultadoValidarProveedorComercial ValidarCUIT(string CUIT, bool? corredor = false)
        {
            try
            {
                return service.ValidarProveedorComercial(CUIT, corredor); 
            }
            //Significa que no estamos conectados
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool ProveedorApocrifo(string CUIT)
        {
            try
            {
                return service.ProveedorApocrifo(CUIT);
            }
            //Significa que no estamos conectados
            catch (Exception ex)
            {
                return false;
            }
        }

        public decimal TraerTipoDeCambio()
        {
            return service.TraerTipoDeCambio(null);
        }

        public SustitucionMOAWS.DataAgroServices.Resultado AltaCampoSustentable(SustitucionMOAModel.Entities.CampoProveedor campo, string kmz)
        {
             
            return service.AltaCampoSustentable(new CampoDetalleTerceroDto {
                Campania= campo.CampoCosecha.Cosecha.Nombre,
                KMZfileBase64 = kmz,
                //... el resto
            });
        }

    }

}
