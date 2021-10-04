using System;
using System.Collections.Generic;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class HomeService : IHomeService
    {
        protected readonly IRepositorio repositorio;
        private ContratoService _contratoService = new ContratoService();

        public HomeService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

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

        public List<BuscadorOption> getBusqueda(string palabraABuscar, string mailUsuario, string proveedor)
        {
            try
            {
                List<BuscadorOption> listaResultados = new List<BuscadorOption> {};
                var detalleContratoResultado = _contratoService.getDetalleContrato(proveedor, palabraABuscar);

                if (detalleContratoResultado != null)
                {
                    listaResultados.Add(new BuscadorOption { Id = 1, Link = "/contrato/detalle", Tipo = "Detalle de contrato", Value = palabraABuscar });
                }         

                return listaResultados;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
