using System;
using System.Collections.Generic;
using System.Linq;
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
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using System.Collections;

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

        public HomeViewModel getHomeInfo(string proveedor, string fechaInicio, string fechaFin, string sociedad)
        {
            try
            {
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                FechaWS fecha = CommonUtil.toDate(fechaInicio, fechaFin);
                HomeViewModel data = new HomeViewModel();
                HomeWSMOAResponse homeWsRes = new HomeConsumerMOA().request(proveedor, fechas);
                if (homeWsRes != null)
                {
                    data.resumen = homeWsRes.resumen;
                }
                var CtaCteWsRes = new CuentaCorrientesConsumerMOA().Request("", proveedor, sociedad, fecha, "", "", "");
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
                List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
                FechaWS fecha = CommonUtil.toDate(fechaInicio, fechaFin);
                HomeViewModel data = new HomeViewModel();
                HomeWSMOAResponse homeWsRes = new HomeNGConsumerMOA().request(proveedor, fechas);
                if (homeWsRes != null)
                {
                    data.resumen = homeWsRes.resumen;
                }
                var CtaCteWsRes = new CuentaCorrientesConsumerMOA().Request("", proveedor, sociedad, fecha, "", "", "");
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
            List<BuscadorOption> listaResultados = new List<BuscadorOption> { };
            ContratoDetalleWSMOAResponse detalleContratoResultado = null;
            LiquidacionViewModel todasLiquidaciones = null;
            LiquidacionWSMOAResponse liquidacionWSMOAResponse = null;
            ListarPesificacionesWSMOAResponse historialPesificaciones = null;
            CartaPorteWSMOAResponse cartasPorteAplicacion = null;
            CartaPorteDescargaWSMOAResponse cartasPorteDescargas = null;
            bool existePesificacionDelContrato = false;
            string fechaInicio = DateTime.Now.AddDays(-150).ToString("yyyy - MM - dd");
            bool ccppAplicacion = false;
            bool ccppDescargas = false;
            string fechaFin = DateTime.Now.AddDays(+1).ToString("yyyy - MM - dd");
            List<FechaWS> fechas = CommonUtil.toDateList(fechaInicio, fechaFin);
            palabraABuscar.Trim().Replace("\t", "");
            if (palabraABuscar == null || palabraABuscar == "undefined" || palabraABuscar.Length < 5)
            {
                return listaResultados;
            }
            var formatoContrato = "0000000000";
            // Ahora (respeta largos mayores a 10)
            var palabraABuscarContrato = palabraABuscar.Length > 10 ? palabraABuscar : (formatoContrato + palabraABuscar).Substring((formatoContrato + palabraABuscar).Length - 10);
            var formatoCCPP1 = (formatoContrato + palabraABuscar).Substring((formatoContrato + palabraABuscar).Length - 12);
            var formatoCCPP2 = string.Format("000{0}", palabraABuscar.Substring(3));

            List<string> palabrasABuscar = new List<string>() { palabraABuscarContrato, formatoCCPP1, formatoCCPP2 };

            palabrasABuscar.ForEach(palabra =>
            {
                try
                {
                    detalleContratoResultado = _contratoService.ObtenerDetalleContrato(proveedor, palabra);
                }
                catch (Exception e)
                {
                    //Log.Error(e);
                }


                if (detalleContratoResultado != null)
                {
                    listaResultados.Add(new BuscadorOption { Link = "/contrato/detalle", Tipo = "detalle de contrato", Value = palabra, Code = TipoBusqueda.DetalleContrato, CtaParams = 1 });
                    var opcionProformaFinal = new BuscadorOption { Link = "", Tipo = "", Value = "", Code = TipoBusqueda.ProformaFinalAgrupador, CtaParams = 1 };

                    detalleContratoResultado.liquidaciones.ForEach(liq =>
                    {
                        if (!string.IsNullOrEmpty(liq.pedido) && liq.tipo.Contains("Fijac."))
                        {
                            opcionProformaFinal.SubOpciones.Add(new SubOption { Nombre = liq.tipo, Value = liq.pedido });
                        }
                    });

                    if (opcionProformaFinal.SubOpciones.Count >= 1) listaResultados.Add(opcionProformaFinal);

                    //HISTORIAL PESIFICACIONES
                    try
                    {
                        if (historialPesificaciones == null) historialPesificaciones = pesificacionesConsumer.Request(proveedor);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    if (historialPesificaciones != null)
                    {
                        existePesificacionDelContrato = historialPesificaciones.Pesificaciones.Exists(p => p.Contrato == palabra);

                        if (existePesificacionDelContrato)
                        {
                            listaResultados.Add(new BuscadorOption { Link = "/pesificacion/listado", Tipo = "historial de pesificaciónes", Value = palabra, Code = TipoBusqueda.HistorialPesificaciones, CtaParams = 1 });
                        }
                    }

                    //CARTAS DE PORTE
                    try
                    {
                        if (cartasPorteAplicacion == null)
                        {
                            cartasPorteAplicacion = new AplicacionesConsumerMOA().request(proveedor, new List<FechaWS>(), new List<string> { palabra }, "");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    if (cartasPorteAplicacion != null && cartasPorteAplicacion?.cartasPorte.Count > 0)
                    {
                        var existeccpp = cartasPorteAplicacion.cartasPorte.Exists(c => c.contrnum == palabra);

                        if (existeccpp)
                        {
                            listaResultados.Add(new BuscadorOption { Link = "/carta-porte/aplicacion", Tipo = "aplicaciones", Value = palabra, Code = TipoBusqueda.CCPP, CtaParams = 1 });
                        }
                    }

                    //LIQUIDACIONES
                    try
                    {
                        if (todasLiquidaciones == null) todasLiquidaciones = liquidacionService.TodasLiquidaciones(proveedor, fechaInicio, fechaFin, palabra);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    if (todasLiquidaciones != null)
                    {
                        var liquidacion = todasLiquidaciones.data.liquidaciones.Find(lp => lp.contrato == palabra) ?? null;
                        var liquidaciones = todasLiquidaciones.data.liquidaciones.Where(lp => lp.contrato == palabra).ToList();
                        if (liquidaciones != null && liquidaciones.Any())
                        {
                            var value = string.Join("|", liquidaciones.Select(a => a.documento).ToList());
                            listaResultados.Add(new BuscadorOption { Link = "", Tipo = "liquidación emitida", Value = value + "," + liquidacion.ejercicio, Code = TipoBusqueda.Liquidacion, CtaParams = 1 });
                        }
                    }

                    detalleContratoResultado = null;
                }
                else
                {
                    //liquidaciones
                    try
                    {
                        liquidacionWSMOAResponse = (LiquidacionWSMOAResponse)new LiquidacionesConsumerMOA().request(proveedor, new List<FechaWS> { new FechaWS { fechaInicio = new DateTime(2018, 01, 01), fechaFin = DateTime.Today.AddDays(+1) } }, "", palabra);

                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }

                    if (liquidacionWSMOAResponse != null && liquidacionWSMOAResponse?.liquidaciones.Count > 0)
                    {
                        LiquidacionViewModel dataView = new LiquidacionViewModel();
                        dataView.data = liquidacionWSMOAResponse;
                        todasLiquidaciones = dataView;

                        var liquidacion = todasLiquidaciones.data.liquidaciones.Find(lp => lp.comprobante == palabra);
                        var liquidaciones = todasLiquidaciones.data.liquidaciones.Where(lp => lp.comprobante == palabra).ToList();

                        if (liquidaciones.Count > 0 || liquidaciones != null)
                        {
                            listaResultados.Add(new BuscadorOption { Link = "", Tipo = "liquidación emitida", Value = liquidacion.documento + "," + liquidacion.ejercicio, Code = TipoBusqueda.Liquidacion, CtaParams = 1 });

                        }
                    }


                    //CARTAS DE PORTE
                    try
                    {
                        //if (cartasPorteAplicacion == null)
                        //{
                        cartasPorteAplicacion = new AplicacionesConsumerMOA().request(proveedor, new List<FechaWS> { }, new List<string>(), palabra);
                        //}

                        //if (cartasPorteDescargas == null)
                        //{
                        cartasPorteDescargas = (CartaPorteDescargaWSMOAResponse)new RecepcionesConsumerMOA().request(proveedor, new List<FechaWS> { new FechaWS { fechaFin = DateTime.Now.Date.AddDays(1), fechaInicio = new DateTime(2010, 1, 1) } }, new List<string> { palabra });
                        //}
                    }
                    catch (Exception e)
                    {
                        //Log.Error(e);
                    }


                    if (((cartasPorteAplicacion != null && cartasPorteAplicacion?.cartasPorte.Count > 0)
                    || (cartasPorteDescargas != null && cartasPorteDescargas?.cartasPorte.Count > 0))
                    && (!ccppAplicacion && !ccppDescargas))
                    {
                        ccppAplicacion = cartasPorteAplicacion.cartasPorte.Count > 0;
                        ccppDescargas = cartasPorteDescargas.cartasPorte.Count > 0;
                        if (ccppAplicacion || ccppDescargas)
                        {
                            var ccpp = cartasPorteAplicacion.cartasPorte.Count > 0 ? cartasPorteAplicacion.cartasPorte.First().cartaPorte : cartasPorteDescargas.cartasPorte.First().cartaPorte;
                            listaResultados.Add(new BuscadorOption { Link = "/carta-porte/detalle", Tipo = "detalle carta de porte", Value = ccpp, Code = TipoBusqueda.CCPP, CtaParams = 1 });
                            listaResultados.Add(new BuscadorOption { Link = "", Tipo = "carta de porte", Value = ccpp, Code = TipoBusqueda.CCPP, CtaParams = 1 });
                        }
                    }
                }

            });

            return listaResultados.GroupBy(x => x.Tipo).Select(y => y.First()).ToList();
        }
    }
}
