using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IHomeService
    {
        string getTitulo();
        HomeViewModel getHomeNGInfo(string proveedor, string fechaInicio, string fechaFin, string sociedad);
        HomeViewModel getHomeInfo(string proveedor, string fechaInicio, string fechaFin, string sociedad);
        List<BuscadorOption> getBusqueda(string palabraABuscar, string mailUsuario, string proveedor);
    }
}
