using System;
using System.Collections.Generic;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.ViewModel.Home;
using SustitucionMOAModel.Models.ViewModel.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.CuentaCorriente;
using SustitucionMOAModel.Models.WSMapMOA.Home;
using SustitucionMOAModel.Models.WSMapMOA.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class HomeService : IHomeService
    {
        protected readonly IRepositorio repositorio;
        private ContratoService _contratoService = new ContratoService();
        private readonly ILiquidacionService liquidacionService;
        readonly IListarPesificacionesConsumer pesificacionesConsumer;

        public HomeService(IRepositorio repositorio, IListarPesificacionesConsumer pesificacionesConsumer, ILiquidacionService liquidacionService)
        {
            this.repositorio = repositorio;
            this.pesificacionesConsumer = pesificacionesConsumer;
            this.liquidacionService = liquidacionService;
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
            List<BuscadorOption> listaResultados = new List<BuscadorOption> {};
            ContratoDetalleWSMOAResponse detalleContratoResultado = null;
            LiquidacionViewModel todasLiquidaciones = null;
            ListarPesificacionesWSMOAResponse historialPesificaciones = null;
            CartaPorteWSMOAResponse cartasDePorte = null;
            bool existePesificacionDelContrato = false;
            string fechaInicio = DateTime.Now.AddYears(-1).ToString("yyyy - MM - dd");
            string fechaFin = DateTime.Now.AddDays(+1).ToString("yyyy - MM - dd");
            palabraABuscar.Trim().Replace("/t", "");

            try
            {
                detalleContratoResultado = _contratoService.getDetalleContrato(proveedor, palabraABuscar);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }


            if (detalleContratoResultado != null)
            {
                listaResultados.Add(new BuscadorOption { Link = "/contrato/detalle", Tipo = "detalle de contrato", Value = palabraABuscar, Code = TipoBusqueda.DetalleContrato, CtaParams = 1});
            }

            //HISTORIAL PESIFICACIONES
            try
            {
                historialPesificaciones = pesificacionesConsumer.Request(proveedor);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (historialPesificaciones != null)
            {
                existePesificacionDelContrato = historialPesificaciones.Pesificaciones.Exists(p => p.Contrato == palabraABuscar);

                if (existePesificacionDelContrato)
                {
                    listaResultados.Add(new BuscadorOption { Link = "/pesificacion/listado", Tipo = "historial de pesificaciónes", Value = palabraABuscar, Code = TipoBusqueda.HistorialPesificaciones, CtaParams = 1 });
                }
            }

            //CARTAS DE PORTE
            try
            {
                List<FechaWS> fechas = CommonService.toDateList(fechaInicio, fechaFin);
                cartasDePorte = (CartaPorteWSMOAResponse)new AplicacionesConsumerMOA().request(proveedor, fechas);
            }
            catch(Exception e)
            {
                Log.Error(e);
            }

            if(cartasDePorte != null)
            {
                var existeccpp = cartasDePorte.cartasPorte.Exists(ccpp => ccpp.contrnum == palabraABuscar);

                if (existeccpp)
                {
                    listaResultados.Add(new BuscadorOption { Link = "/carta-porte/aplicacion", Tipo = "carta de porte", Value = palabraABuscar, Code = TipoBusqueda.CCPP, CtaParams = 1 });
                }
            }

            //LIQUIDACIONES
            try
            {
                todasLiquidaciones = liquidacionService.TodasLiquidaciones(proveedor, fechaInicio, fechaFin);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            if (todasLiquidaciones != null)
            {
                var liquidacion = todasLiquidaciones.data.liquidaciones.Find(lp => lp.contrato == palabraABuscar) ?? null;

                if (liquidacion != null)
                {
                    listaResultados.Add(new BuscadorOption { Link = "", Tipo = "liquidación", Value = liquidacion.documento + "," + liquidacion.ejercicio, Code = TipoBusqueda.Liquidacion, CtaParams = 1 });
                }
            }

            return listaResultados;
        }
    }
}
