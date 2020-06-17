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
            return ObtenerFotoCartasPorte(new List<string> { cartaPorteId });
        }

        public List<CartaPorteFoto> ObtenerFotoCartasPorte(List<string> cartaPorteIds)
        {
            try
            {
                List<CartaPorteFoto> cartaPorteFotos = new List<CartaPorteFoto>();
                foreach (string cartaPorteId in cartaPorteIds)
                {
                    ObtenerFotosPorCartaPorteID(cartaPorteFotos, cartaPorteId);
                }

                return cartaPorteFotos;
            }
            catch
            {
                throw;
            }
        }

        private void ObtenerFotosPorCartaPorteID(List<CartaPorteFoto> cartaPorteFotos, string cartaPorteId)
        {
            try
            {
                FotosDto fotos = service.ObtenerFotosCartaPortePorNumero(cartaPorteId);

                foreach (FotoDto foto in fotos.Fotos)
                {
                    cartaPorteFotos.Add(new CartaPorteFoto(foto.Foto, foto.FotoChica));
                }
            }
            catch
            {
                throw;
            }
        }
    }
}