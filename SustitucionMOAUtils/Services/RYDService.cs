using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAModel.Models.DBMap.RYD.CargaPesada;
using SustitucionMOAUtils.DBMethods;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAUtils.Export;
using SustitucionMOAModel.Models.WSMapMOA.Balanza;

namespace SustitucionMOAUtils.Services
{
    public class RYDService
    {
        DBService _dbService = new DBService();

        public InputsCargaPesadas getDataInputsCargaPesadas(int centro)
        {
            try
            {
                InputsCargaPesadas dataInputs = new InputsCargaPesadas();

                dataInputs.balanzas = getBalanzas(centro, true);
                dataInputs.bodegas = getBodegas();
                dataInputs.commodities = getCommodities();
                dataInputs.destinos = getDestinos();
                dataInputs.exportadores = getExportadores();
                dataInputs.vapores = getVapores();

                return dataInputs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DbElement> getFiltrosInforme(int centro)
        {
            try
            {
                return initDropdownVacio(getBalanzas(centro,false));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public FiltroListadoPesadas getFiltrosListadoPesadas()
        {
            try
            {
                FiltroListadoPesadas dataInputs = new FiltroListadoPesadas();
                dataInputs.commodities = initDropdownTodo(getCommodities());
                dataInputs.exportadores = initDropdownTodo(getExportadores());
                return dataInputs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PesadaBalanzaInforme getInforme(int centro, string balanza)
        {
            try
            {
                PesadaBalanzaInforme data = _dbService.SqlSPInformeBalanza(centro, balanza);
                if (data.pesadas.Count == 0) {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pesadas"));
                }
                return data;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ListadoEncabezadoPesadas getListadoPesadas(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin)
        {
            try
            {
                ListadoEncabezadoPesadas data = _dbService.SqlSPListadoPesadas(empresa, commodity, exportador, fechaInicio, fechaFin);
                if (data.pesadas.Count == 0)
                {
                    throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "Pesadas"));
                }
                return data;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string downloadInforme(int centro, string balanza)
        {
            try
            {
                PesadaBalanzaInforme data = getInforme(centro, balanza);

                return ExcelExport.ToExcel(data.pesadas, new string[] { "Nro Pesada", "Fecha", "Hora", "Peso Tara", "Peso Bruto", "Peso Neto"}, "Reporte RYD Informe");
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string downloadListadoPesada(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin)
        {
            try
            {
                ListadoEncabezadoPesadas data = getListadoPesadas(empresa, commodity, exportador, fechaInicio, fechaFin);

                return ExcelExport.ToExcel(data.pesadas, new string[] { "Nro Viaje", "Vapor", "Commodity", "Exportador", "Fecha", "Total TN Cargadas", "00-06", "'06-12", "'12-18", "18-24" }, "Reporte RYD Informe");
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string registrarPesada(int codigo, string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado, int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto)
        {
            string docSap = "";
            DateTime fechaInicio, fechaPesadaDT;
            int commodityInt, exportadorInt;
            validarDatos(balanza, fecha, bodega, commodity, destino, exportador, vapor, pesoProgramado, pesoAcumulado, numeroPesada, fechaPesada, pesoTara, pesoBruto);
            fechaInicio = DataFormatter.StringToDateTime(fecha, "Fecha Encabezado");
            fechaInicio = fechaInicio.AddSeconds(-fechaInicio.Second);
            fechaPesadaDT = DataFormatter.StringToDateTime(fechaPesada, "Fecha Pesada");
            fechaPesadaDT = fechaPesadaDT.AddSeconds(-fechaPesadaDT.Second);
            try
            {
                commodityInt = Convert.ToInt32(commodity);

            }catch (Exception e) {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Commodity"), e);
            }
            Commodity comm = _dbService.SqlSPCBCommodity(commodityInt);
            if (comm.AlmacenOrigen == null && comm.label == null && comm.MaterialSap == null && comm.value == null) {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Commodity"));
            }

            try
            {
                exportadorInt = Convert.ToInt32(exportador);

            }
            catch (Exception e)
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Exportador"), e);
            }
            Exportador expo = _dbService.SqlSPCBExportador(exportadorInt);
            if (expo.AlmacenSap == null& expo.label == null && expo.value == null)
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, "Exportador"));
            }
            
            MovimientoBalanzaMOAResponse response = new MovimientoBalanzaConsumerMOA().request(fechaInicio.ToString("yyyy-MM-dd"), fechaPesadaDT.ToString("yyyy-MM-dd"), "1029", comm.AlmacenOrigen != null ? comm.AlmacenOrigen : "", expo.AlmacenSap != null ? expo.AlmacenSap : "", comm.MaterialSap != null ? comm.MaterialSap : "", Convert.ToDecimal(pesoBruto - pesoTara));
            if (response.imMessage != "" && response.imMessage != null) {
                throw new ValidationCustomException(response.imMessage);
            }
            docSap = response.imMaterialdocument;
            _dbService.SqlSPGrabarPesadaBalanzaPuerto(codigo, fechaInicio, balanza, comm.label, bodega, destino, expo.label, vapor, pesoProgramado, numeroPesada, fechaPesadaDT, pesoTara, pesoBruto, pesoBruto - pesoTara, null, docSap);
            
            return "";
        }

        public string finalizarCargaPesadas(int codigo, string balanza, string fecha)
        {
            DateTime fechaInicio;
            validarDatosEncabezadoFinalizarCarga(balanza, fecha);
            try
            {
                fechaInicio = DataFormatter.StringToDateTime(fecha, "Fecha Encabezado");
            }
            catch (Exception e)
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "Fecha Encabezado"), e);
            }
            
            _dbService.SqlSPCerrarPesadaBalanzaPuerto(codigo, fechaInicio, balanza);
            return SuccessMsg.DatosPesadasGuardadosOK;
        }

        public CargaPesadaBalanza verificarBalanzaEnProceso(int codigo, string balanza) {
            CargaPesadaBalanza cargaEnProceso = _dbService.SqlSPBalanzaEnProcesoData(codigo, balanza);
            return cargaEnProceso;
        }

        private List<DbElement> getBalanzas(int centro, bool soloManuales)
        {
            List<DbParameter> parametros = new List<DbParameter>();
            parametros.Add(new DbParameter("centro", centro));
            parametros.Add(new DbParameter("SoloManuales", soloManuales));
            return _dbService.SqlSPCBElement("Sp_cb_Balanzas", parametros);
        }

        private List<DbElement> getBodegas()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Bodegas", new List<DbParameter>());
        }

        private List<DbElement> getCommodities()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Commodities", new List<DbParameter>());
        }

        private List<DbElement> getDestinos()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Destinos", new List<DbParameter>());
        }

        private List<DbElement> getExportadores()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Exportadores", new List<DbParameter>());
        }

        private List<DbElement> getVapores()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Vapores", new List<DbParameter>());
        }

        private List<DbElement> initDropdownTodo(List<DbElement> data)
        {
            List<DbElement> options = new List<DbElement>();
            options.Add(new DbElement() { value = "", label = "Todos" });
            return initDropdown(options, data);
        }

        private List<DbElement> initDropdownVacio(List<DbElement> data)
        {
            List<DbElement> options = new List<DbElement>();
            return initDropdown(options, data);
        }

        private List<DbElement> initDropdown(List<DbElement> options, List<DbElement> data)
        {
            if (data != null)
            {
                options = options.Concat(data).ToList(); ;
            }
            return options;
        }


        private void validarDatosEncabezadoFinalizarCarga(string balanza, string fecha) {
            InputValidator.notEmptyOrNull(balanza, "Balanza");
        }

        private void validarDatos(string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado, int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto) {
            validarDatosEncabezado(balanza, fecha, bodega, commodity, destino, exportador, vapor, pesoProgramado, pesoAcumulado);
            validarDatosPesada(numeroPesada, fechaPesada, pesoTara, pesoBruto);
        }

        private void validarDatosEncabezado(string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado)
        {
            InputValidator.notEmptyOrNull(balanza, "Balanza");
            InputValidator.notEmptyOrNull(bodega, "Bodega");
            InputValidator.notEmptyOrNull(commodity, "Commodity");
            InputValidator.notEmptyOrNull(destino, "Destino");
            InputValidator.notEmptyOrNull(exportador, "Exportador");
            InputValidator.notEmptyOrNull(vapor, "Vapor");
        }

        protected void validarDatosPesada(int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto)
        {

        }
    }
}
