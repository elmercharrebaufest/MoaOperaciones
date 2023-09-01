using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IFacturaAnticipadaService
    {
        List<FacturaOrdenCarga> ObtenerFacturasDeContrato(OrdenDeCarga orden);
        List<FacturaOrdenCarga> ObtenerFacturasDeContrato(string numeroContrato);
        void SeleccionarFactura(OrdenDeCarga orden, string facturaSeleccionada);
        bool OrdenConMultiplesFacturas(OrdenDeCarga orden);
        bool OrdenConMultiplesFacturas(Result contrato);

    }
}
