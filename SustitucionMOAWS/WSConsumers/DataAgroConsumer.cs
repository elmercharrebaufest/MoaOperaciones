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
            service.ClientCredentials.Windows.ClientCredential.UserName = DataAgroWSCredential.getUserName();
            service.ClientCredentials.Windows.ClientCredential.Password = DataAgroWSCredential.getPassword();
            service.ClientCredentials.Windows.ClientCredential.Domain = DataAgroWSCredential.getDominio();
        }


        public ResultadoValidarProveedorComercial ValidarCUIT(string CUIT) {
            ResultadoValidarProveedorComercial resultado = new ResultadoValidarProveedorComercial();
            try
            {
                resultado = service.ValidarProveedorComercial(CUIT);
            }
            catch (Exception ex)
            {

            }

            return resultado;
        }
    }
}
