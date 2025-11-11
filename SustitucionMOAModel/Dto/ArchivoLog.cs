using System;

namespace SustitucionMOAModel.Dto
{
    public class ArchivoLog
    {
        public string Nombre { get; set; }
        public long PesoKB { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
