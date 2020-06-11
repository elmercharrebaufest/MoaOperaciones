using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorteFoto
    {
        public string Foto { get; set; }

        public string FotoChica { get; set; }

        public CartaPorteFoto(byte[] foto, byte[] fotoChica)
        {
            Foto = Convert.ToBase64String(foto); 
            FotoChica = Convert.ToBase64String(fotoChica); ;
        }

    }
}
