using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class CuentaCorrienteService : ICuentaCorrienteService
    {
        public CuentaCorrienteService()
        {

        }
        public CuentaCorrienteViewModel GetCuentasCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);

                CuentaCorrienteViewModel dataView = new CuentaCorrienteViewModel
                {
                    filtroConcepto = new DropdownContent(),
                    data = (CuentaCorrienteWSMOAResponse)new CuentaCorrientesConsumerMOA().request("", proveedor, sociedad, fechas, contrato, pago, retencion)
                };

                dataView.data.msj = ValidarRespuesta(dataView.data);

                try
                {
                    dataView.filtroConcepto =
                            new DropdownContent(
                                dataView.data.cuentasCorrientes
                                    .GroupBy(i => i.contrato)
                                    .Select(x => new DropdownOption { value = x.Key, label = x.Key + " (" + x.Count() + ")" })
                                    .ToList()
                            );
                }
                catch { }

                return dataView;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public CuentaCorrienteAgrupadaViewModel GetCuentasCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                CuentaCorrienteAgrupadaViewModel dataView = new CuentaCorrienteAgrupadaViewModel
                {
                    filtroConcepto = new DropdownContent(),
                    data = (CuentaCorrienteAgrupadaWSMOAResponse)new CuentaCorrientesAgrupadaConsumerMOA().request("X", proveedor, sociedad, fechas, contrato, pago, retencion)
                };

                dataView.data.msj = ValidarRespuesta(dataView.data);

                return dataView;
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public string DownloadCuentaCorrientes(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);
                
                CuentaCorrienteExcelWSMOAResponse data = (CuentaCorrienteExcelWSMOAResponse)new CuentaCorrientesExcelConsumerMOA().request("", proveedor, sociedad, fechas, contrato, pago, retencion);

                ValidarRespuesta(data);

                return ExcelExport.ToExcel(data.cuentasCorrientes, new string[] { "Fecha Documento", "Fecha Vencimiento", "Nro. Documento", "Descripcion", "Contrato", "Tipo Cambio", "Debe", "Haber", "Moneda", "Importe [ARS]", "Saldo", "Agrupador", "Doc. Pago", "Nro. Comprobante", "Periodo Fiscal" }, "Reporte Movimientos");
            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }

            //return "";
        }

        public string DownloadCuentaCorrientesAgrupadas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);

                CuentaCorrienteAgrupadaExcelWSMOAResponse data = (CuentaCorrienteAgrupadaExcelWSMOAResponse)new CuentaCorrientesAgrupadaExcelConsumerMOA().request("X", proveedor, sociedad, fechas, contrato, pago, retencion);
                ValidarRespuesta(data);
                List<CuentaCorrienteAgrupada> list = new List<CuentaCorrienteAgrupada>();

                list.AddRange(data.cuentasCorrientesAgrupadas);
                
                return ExcelExport.ToExcelCuentaCorrienteAgrupada(list, new string[] { "Fecha Documento", "Fecha Vencimiento", "Nro. Documento", "Descripcion", "Contrato", "Tipo Cambio", "Debe", "Haber", "Saldo", "Moneda", "Importe [ARS]", "Agrupador", "Doc. Pago", "Nro. Comprobante", "Periodo Fiscal" }, "Reporte Cuenta Corriente");

            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }

            //return "";
        }

        public string DownloadCuentasCorrientesPartidasAbiertas(string proveedor, string sociedad, string fechaInicio, string fechaFin, string contrato, string pago, string retencion)
        {
            try
            {
                FechaWS fechas = CommonUtil.toDate(fechaInicio, fechaFin);

                CuentaCorrienteAgrupadaExcelWSMOAResponse data = (CuentaCorrienteAgrupadaExcelWSMOAResponse)new CuentaCorrientesAgrupadaExcelConsumerMOA().request("X", proveedor, sociedad, fechas, contrato, pago, retencion);
                ValidarRespuesta(data);
                List<CuentaCorrienteAgrupada> list = new List<CuentaCorrienteAgrupada>();

                if (data.cuentasCorrientesSinAgrupar != null)
                    list.Add(data.cuentasCorrientesSinAgrupar);


                return ExcelExport.ToExcelCuentaCorrientePartidasAbiertas(list, new string[] { "Fecha Documento", "Fecha Vencimiento", "Nro. Documento", "Descripcion", "Contrato", "Tipo Cambio", "Debe", "Haber", "Saldo", "Moneda", "Importe [ARS]", "Agrupador", "Doc. Pago", "Nro. Comprobante", "Periodo Fiscal" }, "Reporte Cuenta Corriente");

            }
            catch (InfoCustomException)
            {
                throw;
            }
            catch (ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }

            //return "";
        }

        private string ValidarRespuesta(CuentaCorrienteWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" &&
                data.error.codigo != "16" && data.error.codigo != "06")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }

            if (data.error != null && data.error.codigo == "00" && data.error.descripcion != "")
            {
                return data.error.descripcion;
            }

            if (data.cuentasCorrientes == null || data.cuentasCorrientes.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente"));
            }

            return null;
        }

        private string ValidarRespuesta(CuentaCorrienteAgrupadaWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && 
                data.error.codigo != "16" && data.error.codigo != "06")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }

            if (data.error != null && data.error.codigo == "00" && data.error.descripcion != "")
            {
                return data.error.descripcion;
            }

            if (data.cuentasCorrientesSinAgrupar == null && (data.cuentasCorrientesAgrupadas == null || data.cuentasCorrientesAgrupadas.Count == 0))
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente"));
            }

            return null;
        }

        private void ValidarRespuesta(CuentaCorrienteExcelWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && 
                data.error.codigo != "16" && data.error.codigo != "06")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }

            if (data.cuentasCorrientes == null || data.cuentasCorrientes.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "CuentaCorrientes"));
            }
        }

        private void ValidarRespuesta(CuentaCorrienteAgrupadaExcelWSMOAResponse data)
        {
            if (data == null)
            {
                throw new ValidationCustomException(ErrorMsg.Error);
            }

            if (data.error != null && data.error.codigo != null && data.error.codigo != "" && data.error.codigo != "00" && data.error.codigo != "11" && 
                data.error.codigo != "16" && data.error.codigo != "06")
            {
                throw new ValidationCustomException(data.error.descripcion);
            }

            if (data.cuentasCorrientesSinAgrupar == null && (data.cuentasCorrientesAgrupadas == null || data.cuentasCorrientesAgrupadas.Count == 0))
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente"));
            }
        }
    }
}
