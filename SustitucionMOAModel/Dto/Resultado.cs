using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class Resultado
    {
        public string Mensaje { get; set; }
        public int IdEntidad { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Resultado resultado &&
                   Mensaje == resultado.Mensaje;
        }

        public override int GetHashCode()
        {
            return 653725650 + EqualityComparer<string>.Default.GetHashCode(Mensaje);
        }
    }
}
