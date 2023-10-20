using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IRYDMantenimientoService
    {
        Commodity ObtenerFiltrosCommodities(string commodity);
        InputsCargaPesadas ObtenerDataInputsCommoditie();
        List<DbElement> ObtenerCommodities();

        string GuardarCommodity(string materialSAP, string almacenOrigen, string descripcion);
        string ActualizarCommodity(string materialSAP, string almacenOrigen, string descripcion, string commodityId);
        string BorrarCommodity(string commodityId);
        Exportador ObtenerFiltrosExportador(string exportador);

        InputsCargaPesadas ObtenerDataInputsExportador();
        List<DbElement> ObtenerExportador();
        string GuardarExportador(string almacenSAP, string descripcion);
        string ActualizarExportador(string almacenSAP, string descripcion, string exportadorId);
        string BorrarExportador(string exportadorId);
        InputsBalanzas ObtenerInputDropDown(int centro);
        InputsBalanzas ObtenerFiltrosNroPuesto(int centro, int itcID);
        List<Balanza> BuscarBalanzaAplicar(int centro, string codigo, string descripcion, string tipoId, string codigoCabezalId, string codigoSAP);
        List<Balanza> BuscarBalanza(int centro, string codigo, string descripcion, string tipoId, string codigoCabezalId, string codigoSAP);
        string GuardarBalanza(int centro, string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId);
        string ActualizarBalanza(int centro, string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId);
        string BorrarBalanza(int centro, string codigo);

    }
}
