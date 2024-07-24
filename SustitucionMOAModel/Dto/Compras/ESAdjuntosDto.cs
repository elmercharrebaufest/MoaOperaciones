using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras
{
    public class ESAdjuntosDto
    {
        public byte[] Adjuntos { get; set; }
        public string NombreArchivo { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
    }
}
