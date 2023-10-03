using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAUtils.DBMethods;
using SustitucionMOAAssets;
using SustitucionMOAUtils.Interfaces;

namespace SustitucionMOAUtils.Services
{
    public class RYDMantenimientoService : IRYDMantenimientoService
    {
        private readonly IDBService dBService;

        public RYDMantenimientoService(IDBService dBService)
        {
            this.dBService = dBService;
        }

        private List<DbElement> InicializartDropdown(List<DbElement> data)
        {
            List<DbElement> options = new List<DbElement>();
            options.Add(new DbElement() { value = "", label = "Todos" });
            if (data != null)
            {
                options = options.Concat(data).ToList(); ;
            }
            return options;
        }        

        #region Commodity

        public Commodity ObtenerFiltrosCommodities(string commodity)
        {
            int commodityInt = 0;
            try
            {
                commodityInt = Int32.Parse(commodity);
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Commodity"));
            }
            return dBService.SqlSPCBCommodity(commodityInt);
        }

        public InputsCargaPesadas ObtenerDataInputsCommoditie()
        {

            InputsCargaPesadas dataInputs = new InputsCargaPesadas();

            dataInputs.commodities = new List<DbElement>() { new DbElement() { label = "", value = "0" } };

            dataInputs.commodities.AddRange(ObtenerCommodities());

            return dataInputs;

        }

        public List<DbElement> ObtenerCommodities()
        {
            return dBService.SqlSPCBElement("Sp_cb_Commodities", new List<DbParameter>());
        }

        public string GuardarCommodity(string materialSAP, string almacenOrigen, string descripcion)
        {
            dBService.SqlSPInsertCommodity(descripcion, materialSAP, almacenOrigen);
            return String.Format(SuccessMsg.CommodityGuardadoOK, descripcion);
        }

        public string ActualizarCommodity(string materialSAP, string almacenOrigen, string descripcion, string commodityId)
        {
            dBService.SqlSPUpdateCommodity(Int32.Parse(commodityId), descripcion, materialSAP, almacenOrigen);
            return String.Format(SuccessMsg.CommodityActualizacionOK, descripcion);
        }

        public string BorrarCommodity(string commodityId)
        {
            string response = dBService.SqlSPDeleteCommodity(Int32.Parse(commodityId));
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.CommodityBorradoOK, commodityId);
            }
            else
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorEliminacionElementoRYD, "Commodity (" + commodityId + ")"));
            }
        }
        #endregion

        #region Exportador

        public Exportador ObtenerFiltrosExportador(string exportador)
        {
            int exploradorInt = 0;
            try {
                exploradorInt = Int32.Parse(exportador);
            } catch {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Exportador" ));
            }
            return dBService.SqlSPCBExportador(exploradorInt);
        }

        public InputsCargaPesadas ObtenerDataInputsExportador()
        {

            InputsCargaPesadas dataInputs = new InputsCargaPesadas();

            dataInputs.exportadores = new List<DbElement>() { new DbElement() { label = "", value = "0" } };

            dataInputs.exportadores.AddRange(ObtenerExportador());

            return dataInputs;

        }

        public List<DbElement> ObtenerExportador()
        {
            return dBService.SqlSPCBElement("Sp_cb_Exportadores", new List<DbParameter>());
        }

        public string GuardarExportador(string almacenSAP, string descripcion)
        {

            dBService.SqlSPInsertExportador(descripcion, almacenSAP);
            return String.Format(SuccessMsg.ExportadorGuardadoOK, descripcion);
        }

        public string ActualizarExportador(string almacenSAP, string descripcion, string exportadorId)
        {
            dBService.SqlSPUpdateExportador(Int32.Parse(exportadorId), descripcion, almacenSAP);
            return String.Format(SuccessMsg.ExportadorActualizacionOK, descripcion);
        }

        public string BorrarExportador(string exportadorId)
        {
            string response = dBService.SqlSPDeleteExportador(Int32.Parse(exportadorId));
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.ExportadorBorradoOK, exportadorId);
            }
            else
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorEliminacionElementoRYD, "Exportador ("+ exportadorId +")"));
            }
        }

        #endregion


        #region BALANZA

        public InputsBalanzas ObtenerInputDropDown(int centro)
        {

            InputsBalanzas dataInputs = new InputsBalanzas();

            dataInputs.tipo = ObtenerTipos();
            dataInputs.codigoCabezal = ObtenerCodigoCabezal(centro);
            dataInputs.itc = ObtenerItc(centro);
            dataInputs.tipoAcceso = ObtenerTipoAccesos();

            return dataInputs;

        }

        public InputsBalanzas ObtenerFiltrosNroPuesto(int centro, int itcID)
        {

            InputsBalanzas dataInputs = new InputsBalanzas();

            dataInputs.nroPuesto = ObtenerNroPuestos(centro, itcID);

            return dataInputs;

        }

        private List<DbElement> ObtenerTipos()
        {
            return dBService.SqlSPCBElement("Sp_cb_TipoBalanza", new List<DbParameter>());
        }

        private List<DbElement> ObtenerCodigoCabezal(int centro)
        {
            List<DbParameter> parametros = new List<DbParameter>();
            parametros.Add(new DbParameter("CENTRO", centro));
            return dBService.SqlSPCBCabezales(centro);
        }

        private List<DbElement> ObtenerItc(int centro)
        {
            List<DbParameter> parametros = new List<DbParameter>();
            parametros.Add(new DbParameter("CENTRO", centro));
            return dBService.SqlSPCBITCs(centro);
        }

        private List<DbElement> ObtenerNroPuestos(int centro, int itcID)
        {
            return dBService.SqlSPCBNroPuestos(centro, itcID);
        }

        private List<DbElement> ObtenerTipoAccesos()
        {
            return dBService.SqlSPCBTipoAccesos();
        }

        public List<Balanza> BuscarBalanzaAplicar(int centro, string codigo, string descripcion, string tipoId, string codigoCabezalId, string codigoSAP)
        {
            return dBService.SqlSPBalanzaSearchABM(centro, codigo, descripcion, tipoId, codigoCabezalId, codigoSAP);
        }

        public List<Balanza> BuscarBalanza(int centro, string codigo, string descripcion, string tipoId, string codigoCabezalId, string codigoSAP)
        {
            List<Balanza> balanzas = dBService.SqlSPBalanzaSearchABM(centro, codigo, descripcion, tipoId, codigoCabezalId, codigoSAP);
            if (balanzas == null || balanzas.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Balanzas"));
            }
            return balanzas;
        }

        public string GuardarBalanza(int centro, string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            bool automaticoBool = false;
            int toleriaInt = 0, nroPuestoIdInt = 0;
            decimal tolerXDecimal = 0, pesoMaximoDecimal = 0;
            ValidarDatosBalanza(codigoCabezalId, descripcion, centroEmisor, automatico, toleria, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId, ref automaticoBool, ref toleriaInt, ref nroPuestoIdInt, ref tolerXDecimal, ref pesoMaximoDecimal);
            string response = dBService.SqlSPBalanzaGuardarABM(centro, codigo, descripcion, centroEmisor, automaticoBool, toleriaInt, tolerXDecimal, tipoId, pesoMaximoDecimal, codigoSAP, codigoCabezalId, itcId, nroPuestoIdInt, tipoAccesoId, 0 );
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.BalanzaGuardadoOK, codigo);
            }
            else
            {
                throw new ValidationCustomException(response);
            }

        }

        public string ActualizarBalanza(int centro, string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            bool automaticoBool = false;
            int toleriaInt = 0, nroPuestoIdInt = 0;
            decimal tolerXDecimal = 0, pesoMaximoDecimal = 0;
            ValidarDatosBalanza(codigo, descripcion, centroEmisor, automatico, toleria, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId, ref automaticoBool, ref toleriaInt, ref nroPuestoIdInt, ref tolerXDecimal, ref pesoMaximoDecimal);
            string response = dBService.SqlSPBalanzaActualizarABM(centro, codigo, descripcion, centroEmisor, automaticoBool, toleriaInt, tolerXDecimal, tipoId, pesoMaximoDecimal, codigoSAP, codigoCabezalId, itcId, nroPuestoIdInt, tipoAccesoId, 0, "", 0);
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.BalanzaActualizacionOK, codigo);
            }
            else {
                throw new ValidationCustomException(response);
            }
        }

        public string BorrarBalanza(int centro, string codigo)
        {
            if (codigo == null || codigo == "") {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Código"));
            }
            string response = dBService.SqlSPBalanzaBorrarABM(centro, codigo);
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.BalanzaBorradoOK, codigo);
            }
            else
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorEliminacionElementoRYD, "Balanza (" + codigo + ")"));
            }
        }

        private void ValidarDatosBalanza(string codigo, string descripcion, string centroEmisor, string automatico, string toleria, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId, ref bool automaticoBool, ref int toleriaInt, ref int nroPuestoIdInt, ref decimal tolerXDecimal, ref decimal pesoMaximoDecimal) {
            if (codigo == null || codigo == "") {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Código"));
            }

            if (descripcion == null || descripcion == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Descripción"));
            }

            if (automatico == null || automatico == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Automático"));
            }

            try
            {
                automaticoBool = Boolean.Parse(automatico);
            }
            catch {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Automático"));
            }

            if (toleria == null || toleria == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Tolería"));
            }

            try
            {
                toleriaInt = Convert.ToInt32(toleria);
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Tolería"));
            }

            if (centroEmisor == null || centroEmisor == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Centro Emisor"));
            }

            if (tolerX == null || tolerX == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Tol. x 1000"));
            }

            try
            {
                tolerXDecimal = Convert.ToDecimal(tolerX);
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Tol. x 1000"));
            }

            if (tipoId == null || tipoId == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Tipo"));
            }

            if(pesoMaximo == null || pesoMaximo == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Peso máximo"));
            }

            try
            {
                pesoMaximoDecimal = Convert.ToDecimal(pesoMaximo);
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Peso máximo"));
            }

            if (codigoSAP == null || codigoSAP == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Código SAP"));
            }

            if (codigoCabezalId == null || codigoCabezalId == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Código Cabezal"));
            }

            if (itcId == null || itcId == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "ITC"));
            }

            if (nroPuestoId == null || nroPuestoId == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "N° Puesto"));
            }

            try
            {
                nroPuestoIdInt = Convert.ToInt32(nroPuestoId);
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "N° Puesto"));
            }

            if (tipoAccesoId == null || tipoAccesoId == "")
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Tipo de Entreda"));
            }
        }

        #endregion

    }
}
