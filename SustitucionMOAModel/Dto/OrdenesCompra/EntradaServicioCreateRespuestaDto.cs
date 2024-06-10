using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioCreateRespuestaDto
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Number { get; set; }
        public string Message { get; set; }
        public string NroESSap { get; set; }

        public override string ToString()
        {
            return $"Type: {Type}, Id: {Id}, Number: {Number}, Message: {Message}";
        }
    }
}
