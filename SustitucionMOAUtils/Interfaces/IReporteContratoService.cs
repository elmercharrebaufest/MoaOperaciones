using SustitucionMOAModel.Models.ViewModel.ReporteContrato;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Models.WSMapMOA.ReporteContrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IReporteContratoService
    {
        ReporteContratoViewModel GetContratosReporte(string proveedor, string fechaInicio, string fechaFin, string cliente, string producto,string tipoContrato, bool mostrarPendientes, bool esFiltro, string mailUsuario);
        
    }
}
