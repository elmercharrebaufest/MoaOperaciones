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


        public ResultadoValidarProveedorComercial ValidarCUIT(string CUIT)
        {
            return service.ValidarProveedorComercial(CUIT); ;
        }
    }

}
