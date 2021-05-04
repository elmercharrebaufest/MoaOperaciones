using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAModel.Models;

namespace SustitucionMOAWS.WSConsumers
{
    public class ScatoConsumer : IScatoConsumer
    {
        private readonly ServicioRepositorioClient service = new ServicioRepositorioClient();

        public List<CartaPorteFoto> ObtenerFotoCartaPorte(string cartaPorteId)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes("C:/Users/jyon/Downloads/580452949-150207.tif");
            //return ObtenerFotoCartasPorte(new List<string> { cartaPorteId });
            return new List<CartaPorteFoto> {
                 new  CartaPorteFoto("1000", fileBytes, fileBytes, "tiff")
            };
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

                if (cartaPorteFotos.Count == 0)
                {
                    throw new SustitucionMOAModel.CustomExceptions.InfoCustomException("No hay foto para la/s carta/s porte seleccionada");
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
                    //todo: modificar la extension cuando este listo
                    cartaPorteFotos.Add(new CartaPorteFoto(cartaPorteId, foto.Foto, foto.FotoChica, "tiff"));
                }
            }
            catch
            {
                throw;
            }
        }
    }

}