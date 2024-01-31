using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class Resultado
    {
        public string Mensaje { get; set; }
        public int IdEntidad { get; set; }

        public string error { get; set; }

        public string info { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Resultado resultado &&
                   Mensaje == resultado.Mensaje &&
                   IdEntidad == resultado.IdEntidad;
        }

        public override int GetHashCode()
        {
            int hashCode = 1790892119;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mensaje);
            hashCode = hashCode * -1521134295 + IdEntidad.GetHashCode();
            return hashCode;
        }
    }
}
