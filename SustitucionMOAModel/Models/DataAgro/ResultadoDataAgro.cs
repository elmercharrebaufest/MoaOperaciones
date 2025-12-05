using System.Collections.Generic;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class ResultadoDataAgro
    {
        public ResultadoDataAgro()
        {
            Errores = new List<ErrorMessage>();
        }

        public List<ErrorMessage> Errores { get; set; }

        public bool HayError
        {
            get { return Errores.Count != 0; }
        }

    }
}
