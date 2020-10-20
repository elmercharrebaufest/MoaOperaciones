using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICrearContratoService
    {
        string ObteneDatosContrato();
        string CrearContratoAPrecio(ContratoAPrecio contratoAPrecio);
        string CrearContratoAFijar(ContratoAFijar contratoAPrecio);
        string ObtenerDatosCompraNet(int proveedorId);
        string ValidarDirecto(string cuit);
        string BuscarProveedoresConCorredor(string filtro, string cuitCorredor);
    }
}
