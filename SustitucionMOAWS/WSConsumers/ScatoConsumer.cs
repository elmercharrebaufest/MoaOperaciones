using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;

namespace SustitucionMOAWS.WSConsumers
{
    public class ScatoConsumer
    {
        private ServicioRepositorioClient service = new ServicioRepositorioClient();

        public List<CartaPorteFoto> ObtenerFotoCartaPorte(string cartaPorteId)
        {
            FotosDto fotos = service.ObtenerFotosCartaPortePorNumero(cartaPorteId);

            List<CartaPorteFoto> cartaPorteFotos = new List<CartaPorteFoto>();

            foreach (FotoDto foto in fotos.Fotos)
            {
                cartaPorteFotos.Add(new CartaPorteFoto(foto.Foto, null));
            }

            return cartaPorteFotos;
        }
    }
}