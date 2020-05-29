using System.Collections.Generic;

namespace SustitucionMOAModel.Models.WSMapMOA.Pesificacion
{
    public class PesificacionSetContratosWSMOAResponse
    {
        public List<Item> Log { get; set; }

        public PesificacionSetContratosWSMOAResponse()
        {
            this.Log = new List<Item>() { };
        }
    }
}
