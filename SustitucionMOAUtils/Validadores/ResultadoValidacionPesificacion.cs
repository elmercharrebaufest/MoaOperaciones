using SustitucionMOAModel.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Validadores
{
    public class ResultadoValidacionPesificacion
    {
        public List<ValidationCustomException> Errores { get; set; } = new List<ValidationCustomException>();

        public bool IsValid
        {
            get
            {
                return !Errores.Any();
            }
        }

        public override string ToString()
        {
            StringBuilder mensaje = new StringBuilder();

            Errores.ForEach(error =>
            {
                mensaje.AppendLine(error.Message);
            });

            return mensaje.ToString();
        }
    }
}
