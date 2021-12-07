using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ServicioSuscriptorAccesos;

namespace SustitucionMOAWS.WSConsumers
{
    public class ServicioSuscriptorAccesosConsumer : IServicioSuscriptorAccesosConsumer
    {
        private readonly ServicioSuscriptorClient servicioSuscriptorClient = new ServicioSuscriptorClient();

        public string ObtenerQRTransitoTemporal(string email)
        {
            return servicioSuscriptorClient.ObtenerQRTransitoTemporal(email);
        }
    }
}
