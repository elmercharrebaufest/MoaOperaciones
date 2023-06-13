using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFacturaAnticipadaService
    {
        List<string> ObtenerFacturasDeContrato(string numeroContrato);
    }
}
