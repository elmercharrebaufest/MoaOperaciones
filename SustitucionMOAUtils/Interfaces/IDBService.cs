using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.DBMap.Pesada;
using SustitucionMOAModel.Models.DBMap.Pesada.Detalle;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAModel.Models.DBMap.RYD.CargaPesada;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IDBService
    {
        PesadaWSResponse SqlSPReporte(int centro, string fechaInicio, string fechaFin);
        List<PesadaDetalle> SqlSPDeltallePesada(int centro, int nroOrden);
        List<DbElement> SqlSPCBElement(string command, List<DbParameter> paramenters);
        List<DbElement> SqlSPCBCabezales(int centro);
        Commodity SqlSPCBCommodity(int id);
        Exportador SqlSPCBExportador(int id);
        List<DbElement> SqlSPCBITCs(int centro);
        List<DbElement> SqlSPCBNroPuestos(int centro, int itcID);
        List<DbElement> SqlSPCBTipoAccesos();
        void SqlSPGrabarPesadaBalanzaPuerto(int centro, DateTime fechaInicio, string balanza, string commodity, string bodega, string destino, string exportador, string vapor, int pesoProgramado, int nroPesada, DateTime fechaPesada, double pesoTara, double pesoBruto, double pesoNeto, string error, string docSap);
        void SqlSPCerrarPesadaBalanzaPuerto(int centro, DateTime fechaInicio, string balanza);
        CargaPesadaBalanza SqlSPBalanzaEnProcesoData(int centro, string balanza);
        List<Balanza> SqlSPBalanzaSearchABM(int centro, string codigo, string descripcion, string tipo, string cabezal, string codigoSap);
        string SqlSPBalanzaGuardarABM(int centro, string codigo, string descripcion, string centroemisor, bool automatico, int tolerancia, decimal tolerxmil, string tipo, decimal pesoMaximo, string codigoSap, string codCabezal, string itc, int nroPuesto, string tipoAcceso, int maxCereo);
        string SqlSPBalanzaActualizarABM(int centro, string codigo, string descripcion, string centroemisor, bool automatico, int tolerancia, decimal tolerxmil, string tipo, decimal pesoMaximo, string codigoSap, string codCabezal, string itc, int nroPuesto, string tipoAcceso, int idUsuario, string Usuario, int maxCereo);

        string SqlSPBalanzaBorrarABM(int centro, string codigo);
        void SqlSPInsertCommodity(string descripcion, string materialSap, string almacenOrigen);
        void SqlSPUpdateCommodity(int id, string descripcion, string materialSap, string almacenOrigen);
        string SqlSPDeleteCommodity(int id);
        void SqlSPInsertExportador(string descripcion, string almacenSap);
        void SqlSPUpdateExportador(int id, string descripcion, string almacenSap);
        string SqlSPDeleteExportador(int id);
        PesadaBalanzaInforme SqlSPInformeBalanza(int centro, string balanza);
        ListadoEncabezadoPesadas SqlSPListadoPesadas(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin);
    }
}
