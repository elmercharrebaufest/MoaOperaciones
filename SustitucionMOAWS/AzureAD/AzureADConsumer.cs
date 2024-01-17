using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.AzureAD
{
    public class AzureADConsumer : IAzureADConsumer
    {
        private readonly IUsersGraphAPIClient ClienteApi;

        public AzureADConsumer(IUsersGraphAPIClient clienteApi)
        {
            ClienteApi = clienteApi;
        }

        public void BorrarUsuarioSegunMail(string mail)
        {
            var clienteABorrar = ClienteApi.ObtenerUsuarioPorDisplayName(mail);
            
            if (clienteABorrar == null || clienteABorrar.Value == null || clienteABorrar.Value.Count() == 0)
            {
                throw new Exception("No se encontró un cliente con el mail " + mail);
            }
            
            ClienteApi.BorrarUsuario(clienteABorrar.Value.First().Id);
        }
    }
}
