using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFacturaAnticipadaService
    {
        List<string> ObtenerFacturasDeContrato(OrdenDeCarga orden);
        List<string> ObtenerFacturasDeContrato(string numeroContrato);
        List<string> ObtenerFacturasDeContrato(Result contrato, bool logger = true);
        void SeleccionarFactura(int ordenId, string facturaSeleccionada);
        bool OrdenConMultiplesFacturas(OrdenDeCarga orden);
        bool OrdenConMultiplesFacturas(Result contrato);

    }
}
