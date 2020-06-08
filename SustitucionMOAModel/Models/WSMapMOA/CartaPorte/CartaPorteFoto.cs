using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorteFoto
    {
        public byte[] Foto { get; set; }

        public byte[] FotoChica { get; set; }

        public CartaPorteFoto(byte[] foto, byte[] fotoChica)
        {
            Foto = foto;
            FotoChica = fotoChica;
        }

    }
}
