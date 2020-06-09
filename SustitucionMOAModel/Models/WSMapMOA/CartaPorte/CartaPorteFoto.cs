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
        public byte[] Foto { get; set; }

        public byte[] FotoChica { get; set; }

        public CartaPorteFoto(byte[] foto, byte[] fotoChica)
        {
            Foto = Compress(foto);
            FotoChica = fotoChica;
        }


        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }
}
