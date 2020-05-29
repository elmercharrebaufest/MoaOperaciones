using System;
using System.Collections.Generic;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class HomeService
    {
        public string getTitulo()
        {
            return "Home";
        }

        public HomeViewModel getHomeInfo(string proveedor, string fechaInicio, string fechaFin, string sociedad) {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                FechaWS fecha = CommonService.toDate(fechaInicio, fechaFin);
                HomeViewModel data = new HomeViewModel();
                HomeWSMOAResponse homeWsRes = new HomeConsumerMOA().request(proveedor, fechas);
                if (homeWsRes != null) {
                    data.resumen = homeWsRes.resumen;
                }
                CuentaCorrienteWSMOAResponse CtaCteWsRes = (CuentaCorrienteWSMOAResponse) new CuentaCorrientesConsumerMOA().request("", proveedor, sociedad, fecha, "", "", "");
                if (CtaCteWsRes != null)
                {
                    if (CtaCteWsRes.cuentasCorrientes.Count > 0)
                    {
                        data.cuentasCorrientes = CtaCteWsRes.cuentasCorrientes;
                    }
                    else {
                        data.msjCtaCte = String.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente");
                    }
                }
                return data;
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

        public HomeViewModel getHomeNGInfo(string proveedor, string fechaInicio, string fechaFin, string sociedad)
        {
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                FechaWS fecha = CommonService.toDate(fechaInicio, fechaFin);
                HomeViewModel data = new HomeViewModel();
                HomeWSMOAResponse homeWsRes = new HomeNGConsumerMOA().request(proveedor, fechas);
                if (homeWsRes != null)
                {
                    data.resumen = homeWsRes.resumen;
                }
                CuentaCorrienteWSMOAResponse CtaCteWsRes = (CuentaCorrienteWSMOAResponse)new CuentaCorrientesConsumerMOA().request("", proveedor, sociedad, fecha, "", "", "");
                if (CtaCteWsRes != null)
                {
                    if (CtaCteWsRes.cuentasCorrientes.Count > 0)
                    {
                        data.cuentasCorrientes = CtaCteWsRes.cuentasCorrientes;
                    }
                    else
                    {
                        data.msjCtaCte = String.Format(InfoMsg.SinRegistros, "datos de Cuenta Corriente");
                    }
                }

                return data;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
