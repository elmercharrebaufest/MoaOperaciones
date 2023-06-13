using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Services
{
    public class FacturaAnticipadaService : IFacturaAnticipadaService
    {
        public List<string> ObtenerFacturasDeContrato(string numeroContrato)
        {
            return new List<string> { "00124232", };
        }
    }
}
