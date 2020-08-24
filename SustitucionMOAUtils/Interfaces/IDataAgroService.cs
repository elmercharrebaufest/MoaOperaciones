using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IDataAgroService
    {
        DataAgroAuthWSMOAResponse goToDataAgro(string proveedor, string nombre);
        bool ValidarCUITProveedorGranos(UsuarioGranos usuario, Proveedor proveedor);
        string ObtenerCBUProveedor(string CUITproveedor);
    }
}
