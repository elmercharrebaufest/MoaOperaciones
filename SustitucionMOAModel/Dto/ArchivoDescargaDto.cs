using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class ArchivoDescargaDto
    {
        public string Nombre { get; set; }
        public byte[] Datos { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ArchivoDescargaDto dto &&
                   Nombre == dto.Nombre;
        }

        public override int GetHashCode()
        {
            return 289764928 + EqualityComparer<string>.Default.GetHashCode(Nombre);
        }
    }
}
