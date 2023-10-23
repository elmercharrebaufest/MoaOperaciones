using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.DBMap.RYD;
using SustitucionMOAModel.Models.DBMap.RYD.CargaPesada;
using SustitucionMOAModel.Models.WSMapMOA.Balanza;
using SustitucionMOAUtils.DBMethods;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class RYDService : IRYDService
    {
        DBService _dbService = new DBService();

        public RYDService()
        {

        }

        public InputsCargaPesadas ObtenerDataInputsCargaPesadas(int centro)
        {
            try
            {
                InputsCargaPesadas dataInputs = new InputsCargaPesadas();

                dataInputs.balanzas = ObtenerBalanzas(centro, true);
                dataInputs.bodegas = ObtenerBodegas();
                dataInputs.commodities = ObtenerCommodities();
                dataInputs.destinos = ObtenerDestinos();
                dataInputs.exportadores = ObtenerExportadores();
                dataInputs.vapores = ObtenerVapores();

                return dataInputs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DbElement> ObtenerFiltrosInforme(int centro)
        {
            try
            {
                return InicializarDropdownVacio(ObtenerBalanzas(centro,false));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public FiltroListadoPesadas ObtenerFiltrosListadoPesadas()
        {
            try
            {
                FiltroListadoPesadas dataInputs = new FiltroListadoPesadas();
                dataInputs.commodities = InicializarDropdownTodo(ObtenerCommodities());
                dataInputs.exportadores = InicializarDropdownTodo(ObtenerExportadores());
                return dataInputs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public PesadaBalanzaInforme ObtenerInforme(int centro, string balanza)
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

        public ListadoEncabezadoPesadas ObtenerListadoPesadas(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin)
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

        public string DescargarInforme(int centro, string balanza)
        {
            try
            {
                PesadaBalanzaInforme data = ObtenerInforme(centro, balanza);

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

        public string DescargarListadoPesada(int empresa, string commodity, string exportador, string fechaInicio, string fechaFin)
        {
            try
            {
                ListadoEncabezadoPesadas data = ObtenerListadoPesadas(empresa, commodity, exportador, fechaInicio, fechaFin);

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

        public string RegistrarPesada(int codigo, string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado, int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto)
        {
            string docSap = "";
            DateTime fechaInicio, fechaPesadaDT;
            int commodityInt, exportadorInt;
            ValidarDatos(balanza, fecha, bodega, commodity, destino, exportador, vapor, pesoProgramado, pesoAcumulado, numeroPesada, fechaPesada, pesoTara, pesoBruto);
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

        public string FinalizarCargaPesadas(int codigo, string balanza, string fecha)
        {
            DateTime fechaInicio;
            ValidarDatosEncabezadoFinalizarCarga(balanza, fecha);
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

        public CargaPesadaBalanza VerificarBalanzaEnProceso(int codigo, string balanza) {
            CargaPesadaBalanza cargaEnProceso = _dbService.SqlSPBalanzaEnProcesoData(codigo, balanza);
            return cargaEnProceso;
        }

        private List<DbElement> ObtenerBalanzas(int centro, bool soloManuales)
        {
            List<DbParameter> parametros = new List<DbParameter>();
            parametros.Add(new DbParameter("centro", centro));
            parametros.Add(new DbParameter("SoloManuales", soloManuales));
            return _dbService.SqlSPCBElement("Sp_cb_Balanzas", parametros);
        }

        private List<DbElement> ObtenerBodegas()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Bodegas", new List<DbParameter>());
        }

        private List<DbElement> ObtenerCommodities()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Commodities", new List<DbParameter>());
        }

        private List<DbElement> ObtenerDestinos()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Destinos", new List<DbParameter>());
        }

        private List<DbElement> ObtenerExportadores()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Exportadores", new List<DbParameter>());
        }

        private List<DbElement> ObtenerVapores()
        {
            return _dbService.SqlSPCBElement("Sp_cb_Vapores", new List<DbParameter>());
        }

        private List<DbElement> InicializarDropdownTodo(List<DbElement> data)
        {
            List<DbElement> options = new List<DbElement>();
            options.Add(new DbElement() { value = "", label = "Todos" });
            return InicializarDropdown(options, data);
        }

        private List<DbElement> InicializarDropdownVacio(List<DbElement> data)
        {
            List<DbElement> options = new List<DbElement>();
            return InicializarDropdown(options, data);
        }

        private List<DbElement> InicializarDropdown(List<DbElement> options, List<DbElement> data)
        {
            if (data != null)
            {
                options = options.Concat(data).ToList(); ;
            }
            return options;
        }


        private void ValidarDatosEncabezadoFinalizarCarga(string balanza, string fecha) {
            InputValidator.notEmptyOrNull(balanza, "Balanza");
        }

        private void ValidarDatos(string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado, int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto) {
            ValidarDatosEncabezado(balanza, fecha, bodega, commodity, destino, exportador, vapor, pesoProgramado, pesoAcumulado);
            ValidarDatosPesada(numeroPesada, fechaPesada, pesoTara, pesoBruto);
        }

        private void ValidarDatosEncabezado(string balanza, string fecha, string bodega, string commodity, string destino, string exportador, string vapor, int pesoProgramado, int pesoAcumulado)
        {
            InputValidator.notEmptyOrNull(balanza, "Balanza");
            InputValidator.notEmptyOrNull(bodega, "Bodega");
            InputValidator.notEmptyOrNull(commodity, "Commodity");
            InputValidator.notEmptyOrNull(destino, "Destino");
            InputValidator.notEmptyOrNull(exportador, "Exportador");
            InputValidator.notEmptyOrNull(vapor, "Vapor");
        }

        protected void ValidarDatosPesada(int numeroPesada, string fechaPesada, double pesoTara, double pesoBruto)
        {

        }
    }
}
