using System.Collections.Generic;

namespace SustitucionMOAModel.Models.WSMapMOA.Pesificacion
{
    public class PesificacionGetContratosWSMOAResponse
    {
        public List<Contrato> Contratos { get; set; }

        public PesificacionGetContratosWSMOAResponse()
        {
            this.Contratos = new List<Contrato>() { };
        }
    }
}
