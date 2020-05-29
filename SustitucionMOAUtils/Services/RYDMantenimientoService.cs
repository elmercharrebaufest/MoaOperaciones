using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAUtils.DBMethods;
using SustitucionMOAAssets;

namespace SustitucionMOAUtils.Services
{
    public class RYDMantenimientoService
    {
        DBService _dbService = new DBService();

        private List<DbElement> initDropdown(List<DbElement> data)
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

        public Commodity getFiltrosCommodities(string commodity)
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
            return _dbService.SqlSPCBCommodity(commodityInt);
        }

        public InputsCargaPesadas getDataInputsCommoditie()
        {

            InputsCargaPesadas dataInputs = new InputsCargaPesadas();

            dataInputs.commodities = new List<DbElement>() { new DbElement() { label = "", value = "0" } };

            dataInputs.commodities.AddRange(getCommodities());

            return dataInputs;

        }

        public List<DbElement> getCommodities()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Commodities", new List<DbParameter>());
        }

        public string guardarCommodity(string materialSAP, string almacenOrigen, string descripcion)
        {
            _dbService.SqlSPInsertCommodity(descripcion, materialSAP, almacenOrigen);
            return String.Format(SuccessMsg.CommodityGuardadoOK, descripcion);
        }

        public string actualizarCommodity(string materialSAP, string almacenOrigen, string descripcion, string commodityId)
        {
            _dbService.SqlSPUpdateCommodity(Int32.Parse(commodityId), descripcion, materialSAP, almacenOrigen);
            return String.Format(SuccessMsg.CommodityActualizacionOK, descripcion);
        }

        public string borrarCommodity(string commodityId)
        {
            string response = _dbService.SqlSPDeleteCommodity(Int32.Parse(commodityId));
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

        public Exportador getFiltrosExportador(string exportador)
        {
            int exploradorInt = 0;
            try {
                exploradorInt = Int32.Parse(exportador);
            } catch {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Exportador" ));
            }
            return _dbService.SqlSPCBExportador(exploradorInt);
        }

        public InputsCargaPesadas getDataInputsExportador()
        {

            InputsCargaPesadas dataInputs = new InputsCargaPesadas();

            dataInputs.exportadores = new List<DbElement>() { new DbElement() { label = "", value = "0" } };

            dataInputs.exportadores.AddRange(getExportador());

            return dataInputs;

        }

        public List<DbElement> getExportador()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Exportadores", new List<DbParameter>());
        }

        public string guardarExportador(string almacenSAP, string descripcion)
        {

            _dbService.SqlSPInsertExportador(descripcion, almacenSAP);
            return String.Format(SuccessMsg.ExportadorGuardadoOK, descripcion);
        }

        public string actualizarExportador(string almacenSAP, string descripcion, string exportadorId)
        {
            _dbService.SqlSPUpdateExportador(Int32.Parse(exportadorId), descripcion, almacenSAP);
            return String.Format(SuccessMsg.ExportadorActualizacionOK, descripcion);
        }

        public string borrarExportador(string exportadorId)
        {
            string response = _dbService.SqlSPDeleteExportador(Int32.Parse(exportadorId));
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

        public InputsBalanzas getInputDropDown(int centro)
        {

            InputsBalanzas dataInputs = new InputsBalanzas();

            dataInputs.tipo = getTipos();
            dataInputs.codigoCabezal = getCodigoCabezal(centro);
            dataInputs.itc = getItc(centro);
            dataInputs.tipoAcceso = getTipoAccesos();

            return dataInputs;

        }

        public InputsBalanzas getFiltrosNroPuesto(int centro, int itcID)
        {

            InputsBalanzas dataInputs = new InputsBalanzas();

            dataInputs.nroPuesto = getNroPuestos(centro, itcID);

            return dataInputs;

        }

        private List<DbElement> getTipos()
        {
            return _dbService.SqlSPCBElement("Sp_cb_TipoBalanza", new List<DbParameter>());
        }

        private List<DbElement> getCodigoCabezal(int centro)
        {
            List<DbParameter> parametros = new List<DbParameter>();
            parametros.Add(new DbParameter("CENTRO", centro));
            return _dbService.SqlSPCBCabezales(centro);
        }

        private List<DbElement> getItc(int centro)
        {
            List<DbParameter> parametros = new List<DbParameter>();
            parametros.Add(new DbParameter("CENTRO", centro));
            return _dbService.SqlSPCBITCs(centro);
        }

        private List<DbElement> getNroPuestos(int centro, int itcID)
        {
            return _dbService.SqlSPCBNroPuestos(centro, itcID);
        }

        private List<DbElement> getTipoAccesos()
        {
            return _dbService.SqlSPCBTipoAccesos();
        }

        public List<Balanza> buscarBalanzaAplicar(int centro, string codigo, string descripcion, string tipoId, string codigoCabezalId, string codigoSAP)
        {
            return _dbService.SqlSPBalanzaSearchABM(centro, codigo, descripcion, tipoId, codigoCabezalId, codigoSAP);
        }

        public List<Balanza> buscarBalanza(int centro, string codigo, string descripcion, string tipoId, string codigoCabezalId, string codigoSAP)
        {
            List<Balanza> balanzas = _dbService.SqlSPBalanzaSearchABM(centro, codigo, descripcion, tipoId, codigoCabezalId, codigoSAP);
            if (balanzas == null || balanzas.Count == 0)
            {
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Balanzas"));
            }
            return balanzas;
        }

        public string guardarBalanza(int centro, string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            bool automaticoBool = false;
            int toleriaInt = 0, nroPuestoIdInt = 0;
            decimal tolerXDecimal = 0, pesoMaximoDecimal = 0;
            validarDatosBalanza(codigoCabezalId, descripcion, centroEmisor, automatico, toleria, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId, ref automaticoBool, ref toleriaInt, ref nroPuestoIdInt, ref tolerXDecimal, ref pesoMaximoDecimal);
            string response = _dbService.SqlSPBalanzaGuardarABM(centro, codigo, descripcion, centroEmisor, automaticoBool, toleriaInt, tolerXDecimal, tipoId, pesoMaximoDecimal, codigoSAP, codigoCabezalId, itcId, nroPuestoIdInt, tipoAccesoId, 0 );
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.BalanzaGuardadoOK, codigo);
            }
            else
            {
                throw new ValidationCustomException(response);
            }

        }

        public string actualizarBalanza(int centro, string codigo, string descripcion, string automatico, string toleria, string centroEmisor, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId)
        {
            bool automaticoBool = false;
            int toleriaInt = 0, nroPuestoIdInt = 0;
            decimal tolerXDecimal = 0, pesoMaximoDecimal = 0;
            validarDatosBalanza(codigo, descripcion, centroEmisor, automatico, toleria, tolerX, tipoId, pesoMaximo, codigoSAP, codigoCabezalId, itcId, nroPuestoId, tipoAccesoId, ref automaticoBool, ref toleriaInt, ref nroPuestoIdInt, ref tolerXDecimal, ref pesoMaximoDecimal);
            string response = _dbService.SqlSPBalanzaActualizarABM(centro, codigo, descripcion, centroEmisor, automaticoBool, toleriaInt, tolerXDecimal, tipoId, pesoMaximoDecimal, codigoSAP, codigoCabezalId, itcId, nroPuestoIdInt, tipoAccesoId, 0, "", 0);
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.BalanzaActualizacionOK, codigo);
            }
            else {
                throw new ValidationCustomException(response);
            }
        }

        public string borrarBalanza(int centro, string codigo)
        {
            if (codigo == null || codigo == "") {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, "Código"));
            }
            string response = _dbService.SqlSPBalanzaBorrarABM(centro, codigo);
            if (response == null || response == "")
            {
                return String.Format(SuccessMsg.BalanzaBorradoOK, codigo);
            }
            else
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorEliminacionElementoRYD, "Balanza (" + codigo + ")"));
            }
        }

        private void validarDatosBalanza(string codigo, string descripcion, string centroEmisor, string automatico, string toleria, string tolerX, string tipoId, string pesoMaximo, string codigoSAP, string codigoCabezalId, string itcId, string nroPuestoId, string tipoAccesoId, ref bool automaticoBool, ref int toleriaInt, ref int nroPuestoIdInt, ref decimal tolerXDecimal, ref decimal pesoMaximoDecimal) {
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
