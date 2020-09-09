using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAUtils.Export;
using SustitucionMOAValidator;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class CuentaCorrienteService
    {
        public CuentaCorrienteViewModel getCuentasCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonService.toDate(fechaInicio, fechaFin);
                CuentaCorrienteViewModel dataView = new CuentaCorrienteViewModel();
                dataView.filtroConcepto = new DropdownContent();
                dataView.data = (CuentaCorrienteWSMOAResponse)new CuentaCorrientesConsumerMOA().request("", proveedor, sociedad, fechas, contrato, pago, retencion);
                dataView.data.msj = validarRespuesta(dataView.data);
                try
                {
                    dataView.filtroConcepto = new DropdownContent(dataView.data.cuentasCorrientes.GroupBy(i => i.contrato).Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" }).ToList());
                }
                catch { }
                return dataView;
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

        public CuentaCorrienteAgrupadaViewModel getCuentasCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonService.toDate(fechaInicio, fechaFin);
                CuentaCorrienteAgrupadaViewModel dataView = new CuentaCorrienteAgrupadaViewModel();
                dataView.filtroConcepto = new DropdownContent();
                dataView.data = (CuentaCorrienteAgrupadaWSMOAResponse)new CuentaCorrientesAgrupadaConsumerMOA().request("X", proveedor, sociedad, fechas, contrato, pago, retencion);
                dataView.data.msj = validarRespuesta(dataView.data);
                return dataView;
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

        public string downloadCuentaCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonService.toDate(fechaInicio, fechaFin);
                CuentaCorrienteExcelWSMOAResponse data = (CuentaCorrienteExcelWSMOAResponse)new CuentaCorrientesExcelConsumerMOA().request("", proveedor, sociedad, fechas, contrato, pago, retencion);
                validarRespuesta(data);
                return ExcelExport.ToExcel(data.cuentasCorrientes, new string[] { "Fecha Documento", "Fecha Vencimiento", "Nro. Documento", "Descripcion",  "Contrato", "Tipo Cambio", "Debe",  "Haber", "Moneda", "Importe [ARS]", "Agrupador", "Doc. Pago", "Nro. Comprobante", "Periodo Fiscal" }, "Reporte Movimientos");

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

            //return "";
        }

        public string downloadCuentaCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonService.toDate(fechaInicio, fechaFin);
                CuentaCorrienteAgrupadaExcelWSMOAResponse data = (CuentaCorrienteAgrupadaExcelWSMOAResponse)new CuentaCorrientesAgrupadaExcelConsumerMOA().request("X", proveedor, sociedad, fechas, contrato, pago, retencion);
                validarRespuesta(data);
                List<CuentaCorrienteAgrupada> list = new List<CuentaCorrienteAgrupada>();

                if (data.cuentasCorrientesSinAgrupar != null)
                    list.Add(data.cuentasCorrientesSinAgrupar);

                list.AddRange(data.cuentasCorrientesAgrupadas);
                return ExcelExport.ToExcelCuentaCorrienteAgrupada(list, new string[] { "Fecha Documento", "Fecha Vencimiento", "Nro. Documento", "Descripcion", "Contrato", "Tipo Cambio", "Debe", "Haber", "Saldo", "Moneda", "Importe [ARS]", "Agrupador", "Doc. Pago", "Nro. Comprobante", "Periodo Fiscal" }, "Reporte Cuenta Corriente");

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

            //return "";
        }

        private string validarRespuesta(CuentaCorrienteWSMOAResponse data) {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && data.error.codigo != "16" && data.error.codigo != "06")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.error != null && data.error.codigo == "00" && data.error.descripcion != "")
                return data.error.descripcion;
            if (data.cuentasCorrientes == null || data.cuentasCorrientes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente"));
            return null;
        }

        private string validarRespuesta(CuentaCorrienteAgrupadaWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && data.error.codigo != "16" && data.error.codigo != "06")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.error != null && data.error.codigo == "00" && data.error.descripcion != "")
                return data.error.descripcion;
            if (data.cuentasCorrientesSinAgrupar == null && (data.cuentasCorrientesAgrupadas == null || data.cuentasCorrientesAgrupadas.Count == 0))
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente"));
            return null;
        }

        private void validarRespuesta(CuentaCorrienteExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && data.error.codigo != "16" && data.error.codigo != "06")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.cuentasCorrientes == null || data.cuentasCorrientes.Count == 0)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "CuentaCorrientes"));
        }

        private void validarRespuesta(CuentaCorrienteAgrupadaExcelWSMOAResponse data)
        {
            if (data == null)
                throw new ValidationCustomException(ErrorMsg.Error);
            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && data.error.codigo != "16" && data.error.codigo != "06")
                throw new ValidationCustomException(data.error.descripcion);
            if (data.cuentasCorrientesSinAgrupar == null && (data.cuentasCorrientesAgrupadas == null || data.cuentasCorrientesAgrupadas.Count == 0))
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente"));
        }
    }
}
