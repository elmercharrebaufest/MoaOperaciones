using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAModel.Models.DBMap.RYD.CargaPesada;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IRYDService
    {
        InputsCargaPesadas ObtenerDataInputsCargaPesadas(int centro);
        List<DbElement> ObtenerFiltrosInforme(int centro);

        FiltroListadoPesadas ObtenerFiltrosListadoPesadas();
        PesadaBalanzaInforme ObtenerInforme(int centro, string balanza);
        ListadoEncabezadoPesadas ObtenerListadoPesadas(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin);
        string DescargarInforme(int centro, string balanza);
        string DescargarListadoPesada(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin);
        string RegistrarPesada(int codigo, string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado, int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto);
        string FinalizarCargaPesadas(int codigo, string balanza, string fecha);
        CargaPesadaBalanza VerificarBalanzaEnProceso(int codigo, string balanza);

    }
}
