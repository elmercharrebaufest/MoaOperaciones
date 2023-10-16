using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.ViewModel.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Comunicacion = SustitucionMOAModel.Entities.Comunicacion;

namespace SustitucionMOAUtils.Services

{

    public class ComunicacionService : IComunicacionService
    {
        protected readonly IRepositorio repositorio;
        private readonly ILiquidacionService _liquidacionService;

        public ComunicacionService(IRepositorio repositorio, ILiquidacionService liquidacionService)
        {
            this.repositorio = repositorio;
            _liquidacionService = liquidacionService;
        }


        /// <summary>
        /// Paso 1. Verifica si hay algun cambio que deba ser notificado para cada una de las notificaciones
        /// PAso 2 Muestra todas las comunicaciones (notificaciones) para un determinado Id de vendedor/proveedor
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="proveedor"></param>
        /// <param name="fechaInicio" aaaa/mm/dd> </param>
        /// <param name="fechaFin" aaaa/mm/dd ></param>
        /// <returns></returns>
        public List<ComunicacionDto> ObtenerComunicacionesPorProveedor(string vendedor, string proveedor, string fechaInicio, string fechaFin)
        {
            ProcesarCM05(vendedor, proveedor);
            ProcesarCuentasHabilitadas(vendedor, proveedor);
            ProcesarLiquidacionesObservadas(vendedor, fechaInicio, fechaFin);

            var listado = repositorio
                .Listar<Comunicacion>()
                .Where(x => x.ProveedorId == vendedor)
                .Select(x => new ComunicacionDto
                {
                    Id = x.Id,
                    ComunicacionTipo = x.ComunicacionTipo,
                    ProveedorId = x.ProveedorId,
                    FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    Leida = x.Leida
                })
                .OrderBy(x => x.Leida)
                .ThenByDescending(
                (x =>
                {
                    DateTime dt;
                    DateTime.TryParse(x.FechaCreacion, out dt);
                    return dt;
                })
                )
                .ToList();

            return listado;
        }


        /// <summary>
        /// Persiste estado de una comunicacion (Notificacion) como "Leida"
        /// </summary>
        /// <param name="notificacionId"></param>
        /// <returns></returns>
        //public string GrabarComunicacionComoLeida(int notificacionId)
        //{
        //    var comunicacion = repositorio.Obtener<Comunicacion>(x => x.Id == notificacionId);

        //    if (comunicacion != null)
        //    {
        //        comunicacion.Leida = true;
        //        repositorio.GuardarCambios();
        //        return "Comunicacion marcada como LEIDA";
        //    }
        //    else
        //    {
        //        return "Comunicacion no encontrada";
        //    }
        //}


        public string GrabarComunicacionComoLeida(ComunicacionListaIdDto notificacionIds)
        {
            foreach (var notificacionId in notificacionIds.ItemId)
            {
                var comunicacion = repositorio.Obtener<Comunicacion>(x => x.Id == notificacionId);

                if (comunicacion != null)
                {
                    comunicacion.Leida = true;
                    repositorio.GuardarCambios();
                }
                else
                {
                    return "No se encuentra ID";
                }
            }

            return "Comunicaciones marcadas como LEIDAS";
        }


        /// <summary>
        /// Persiste estado de una comunicacion (Notificacion) como "No leida"
        /// </summary>
        /// <param name="notificacionId"></param>
        /// <returns></returns>
        //public string GrabarComunicacionComoNoLeida(int notificacionId)
        //{
        //    var comunicacion = repositorio.Obtener<Comunicacion>(x => x.Id == notificacionId);

        //    if (comunicacion != null)
        //    {
        //        comunicacion.Leida = false;
        //        repositorio.GuardarCambios();
        //        return "Comunicacion marcada como NO LEIDA";
        //    }
        //    else
        //    {
        //        return "Comunicacion no encontrada";
        //    }
        //}


        public string GrabarComunicacionComoNoLeida(ComunicacionListaIdDto notificacionIds)
        {
            foreach (var notificacionId in notificacionIds.ItemId)
            {
                var comunicacion = repositorio.Obtener<Comunicacion>(x => x.Id == notificacionId);

                if (comunicacion != null)
                {
                    comunicacion.Leida = false;
                    repositorio.GuardarCambios();
                }
                else
                {
                    return "No se encuentra ID";
                }
            }

            return "Comunicaciones marcadas como NO LEIDAS";
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Metodo principal para persistencia de la comunicacion (Notificacion) referente al CM05
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public String ProcesarCM05(string vendedor, string proveedor)
        {

            try
            {
                VendedorDetalleWSMOAResponse response = new VendedorDetalleConsumerMOA().request(vendedor, proveedor);
                string anioCM05 = response.cabeceras[0].cm05;

                bool cm05vencido = Cm05Vencido(anioCM05);
                bool cm05comunicado = Cm05Comunicado(vendedor, anioCM05);

                ComunicarCM05(cm05vencido, cm05comunicado, vendedor, anioCM05);
                ReComunicarCM05(cm05vencido, cm05comunicado, vendedor, anioCM05);

                return "Fin de revision: CM05.";
            }
            catch (Exception e)
            {
                Logger.Log.Error(
                    System.Web.HttpContext.Current.Request.UserHostAddress, "", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);

                string mensaje = $"Falló revisión CM05";
                return mensaje;

            }
        }

        /// <summary>
        /// Verifica si CM05 05 está vencido
        /// </summary>
        /// <param name="_anioCM05"></param>
        /// <returns> bool </returns>
        public bool Cm05Vencido(string _anioCM05)
        {
            var anioCM05 = Convert.ToInt32(_anioCM05);
            DateTime fechaActual = DateTime.Now;
            var anioActual = fechaActual.Year;
            int diaLimite = 1;
            int mesLimite = 7;
            DateTime fechaLimite = new DateTime(fechaActual.Year, mesLimite, diaLimite);
            int anioVigente = (fechaActual < fechaLimite) ? anioActual - 1 : anioActual;

            return anioCM05 <= anioVigente - 2;
        }


        /// <summary>
        /// Verifica si la la comunicacion de CM05 fué persistida anteriormente.
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="anioCM05"></param>
        /// <returns> bool </returns>
        private bool Cm05Comunicado(string vendedor, string anioCM05)
        {
            bool comunicacionPersistida = repositorio.Listar<Comunicacion>()
                .Any(x => x.ComunicacionTipo == 3
                       && x.ProveedorId == vendedor
                       && x.CM05 == anioCM05);

            return comunicacionPersistida;
        }


        /// <summary>
        /// Verifica para CM05, qúe estén presente las condiciones para crear una notificacion.
        /// Que CM05 esté vencido y la comunicacion no fué persistida antes. Entonces persiste la comunicacion.
        /// </summary>
        /// <param name="cm05vencido"></param>
        /// <param name="cm05comunicado"></param>
        /// <param name="vendedor"></param>
        /// <param name="anioCM05"></param>
        public void ComunicarCM05(bool cm05vencido, bool cm05comunicado, string vendedor, string anioCM05)
        {
            if (cm05vencido && !cm05comunicado)
            {
                Comunicacion oComunicacion = new Comunicacion
                {
                    ComunicacionTipo = 3,
                    ProveedorId = vendedor,
                    FechaCreacion = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Second),
                    Leida = false,
                    CM05 = anioCM05
                };
                repositorio.Agregar(oComunicacion);
                repositorio.GuardarCambios();
            }
        }


        /// <summary>
        /// Si CM05 está vencido, fue comunicacdo, la notificacion fue leída y transcurrieron 15 días de la comunicacion, persiste una nueva notificacion.
        /// </summary>
        /// <param name="cm05vencido"></param>
        /// <param name="cm05comunicado"></param>
        /// <param name="vendedor"></param>
        /// <param name="anioCM05"></param>
        public void ReComunicarCM05(bool cm05vencido, bool cm05comunicado, string vendedor, string anioCM05)
        {
            if (cm05vencido && cm05comunicado)
            {
                Comunicacion oComunicacion = repositorio.Listar<Comunicacion>()
                    .Where(x => x.ComunicacionTipo == 3
                                && x.ProveedorId == vendedor
                                && x.CM05 == anioCM05)
                    .OrderByDescending(x => x.FechaCreacion)
                    .FirstOrDefault();

                if (oComunicacion != null)
                {
                    bool siLeida = oComunicacion.Leida;
                    TimeSpan diferenciaDias = DateTime.Now - oComunicacion.FechaCreacion;
                    bool debeRenotificar = diferenciaDias.TotalDays > 15;


                    if (siLeida && debeRenotificar)
                    {
                        Comunicacion oReComunicacion = new Comunicacion
                        {
                            ComunicacionTipo = 3,
                            ProveedorId = vendedor,
                            FechaCreacion = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Second),
                            Leida = false,
                            CM05 = anioCM05,
                            FechaRecomunicacion = DateTime.Now
                        };
                        repositorio.Agregar(oReComunicacion);
                        repositorio.GuardarCambios();
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Metodo principal para persistencia de la comunicacion (Notificacion) referente a Cuentas Habilitadas
        /// </summary>
        /// <param name="vendedor"></param>
        /// <param name="proveedor"></param>
        /// <returns></returns>
        public String ProcesarCuentasHabilitadas(string vendedor, string proveedor)
        {
            try
            {
                VendedorDetalleWSMOAResponse response = new VendedorDetalleConsumerMOA().request(vendedor, proveedor);
                List<Cuenta> cuentasHabilitadas = response.cuentas;

                bool cuentasHabilitadasDebeNotificarse = CuentasHabilitadasConFaltantes(cuentasHabilitadas);
                bool cuentasHabilitadasComunicado = CuentasHabilitadasComunicado(vendedor);

                ComunicarCuentasHabilitadas(cuentasHabilitadasDebeNotificarse, cuentasHabilitadasComunicado, vendedor);
                ReComunicarCuentasHabilitadas(cuentasHabilitadasDebeNotificarse, cuentasHabilitadasComunicado, vendedor);

                return "Fin de revision: Cuentas Habilitadas.";

            }
            catch (Exception e)
            {
                Logger.Log.Error(
                    System.Web.HttpContext.Current.Request.UserHostAddress, "", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);

                string mensaje = $"Fin de busqueda. Cantidad de liquidaciones observadas: 0";
                return mensaje;
            }
        }


        /// <summary>
        /// Verifica para cuentas habilitadas, si se cumple alguna condicion que deba generar una notificacion.
        /// Estas son: Debe tener al menos una cuenta que sea tipo AFIP, y que el número de cuenta no sea nulo.
        /// </summary>
        /// <param name="_cuentasHabilitadas"></param>
        /// <returns> bool </returns>
        public bool CuentasHabilitadasConFaltantes(List<Cuenta> _cuentasHabilitadas)
        {
            bool cuentaHabilitadaConError = false;

            if (_cuentasHabilitadas != null)
            {
                bool cuentaHabilitadaOk = _cuentasHabilitadas.Any(cuenta => cuenta.tipoCta == "AFIP" && cuenta.cuenta != "");
                cuentaHabilitadaConError = !cuentaHabilitadaOk;
            }
            else
            {
                cuentaHabilitadaConError = true;
            }

            return cuentaHabilitadaConError;
        }


        /// <summary>
        /// Verifica si la la comunicacion de cuentas habilitadas fué persistida anteriormente.
        /// </summary>
        /// <param name="vendedor"></param>
        /// <returns>bool</returns>
        private bool CuentasHabilitadasComunicado(string vendedor)
        {
            bool comunicacionPersistida = repositorio.Listar<Comunicacion>()
                .Any(x => x.ComunicacionTipo == 4
                       && x.ProveedorId == vendedor);

            return comunicacionPersistida;
        }


        /// <summary>
        /// Verifica para Cuentas Habilitadas, qúe estén presente las condiciones para crear una notificacion.
        /// Debe existir al menos una cuenta tipo AFIP, y para esa cuenta, el numero de cuenta no debe estar vacío.
        /// </summary>
        /// <param name="cuentasHabilitadasDebeNotificarse"></param>
        /// <param name="cuentasHabilitadasComunicado"></param>
        /// <param name="vendedor"></param>
        public void ComunicarCuentasHabilitadas(bool cuentasHabilitadasDebeNotificarse, bool cuentasHabilitadasComunicado, string vendedor)
        {
            if (cuentasHabilitadasDebeNotificarse && !cuentasHabilitadasComunicado)
            {
                Comunicacion oComunicacion = new Comunicacion
                {
                    ComunicacionTipo = 4,
                    ProveedorId = vendedor,
                    FechaCreacion = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Second),
                    Leida = false
                };
                repositorio.Agregar(oComunicacion);
                repositorio.GuardarCambios();
            }
        }


        /// <summary>
        /// Verifica si cuentas habilitadas, fue comunicado previamente, la notificacion fue leída y transcurrieron 15 días de la comunicacion
        /// Entonces persiste una nueva notificacion.
        /// </summary>
        /// <param name="cuentasHabilitadasDebeNotificarse"></param>
        /// <param name="cuentasHabilitadasComunicado"></param>
        /// <param name="vendedor"></param>
        public void ReComunicarCuentasHabilitadas(bool cuentasHabilitadasDebeNotificarse, bool cuentasHabilitadasComunicado, string vendedor)
        {
            if (cuentasHabilitadasDebeNotificarse && cuentasHabilitadasComunicado)
            {
                Comunicacion oComunicacion = repositorio.Listar<Comunicacion>()
                    .Where(x => x.ComunicacionTipo == 4
                                && x.ProveedorId == vendedor)
                    .OrderByDescending(x => x.FechaCreacion)
                    .FirstOrDefault();

                if (oComunicacion != null)
                {
                    bool siLeida = oComunicacion.Leida;
                    TimeSpan diferenciaDias = DateTime.Now - oComunicacion.FechaCreacion;
                    bool debeRenotificar = diferenciaDias.TotalDays > 15;


                    if (siLeida && debeRenotificar)
                    {
                        Comunicacion oReComunicacion = new Comunicacion
                        {
                            ComunicacionTipo = 4,
                            ProveedorId = vendedor,
                            FechaCreacion = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Second),
                            Leida = false,
                            FechaRecomunicacion = DateTime.Now
                        };
                        repositorio.Agregar(oReComunicacion);
                        repositorio.GuardarCambios();
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Metodo principal para persistencia de la comunicacion (Notificacion) referente a Liquidaciones observadas.
        /// Obtiene datos de SAP
        /// </summary>
        /// <returns></returns>
        public String ProcesarLiquidacionesObservadas(string vendedor, string fechaInicio, string fechaFin)
        {
            // Obtener datos de SAP
            //fechaInicio = "2023-09-13"; // Para poder probar desde postman
            //fechaFin = "2023-09-14";    // Para poder probar desde postman
            //vendedor = "0068514169";   //Este proveedor tiene 8 liquidaciones observadas en la base de ejemplo.

            try
            {
                LiquidacionViewModel _liquidacionesObservadas = _liquidacionService.getLiquidaciones(vendedor, "OBSERVADA", fechaInicio, fechaFin);
                List<LiquidacionView> liquidacionesObservadas = _liquidacionesObservadas.data.liquidaciones.ToList();
                ComunicarLiquidacionesObservadas(liquidacionesObservadas, vendedor);
                int cantidad = liquidacionesObservadas.Count(); 
                string mensaje = $"Fin de busqueda. Cantidad liquidaciones observadas: {cantidad}";
                return mensaje;
            }
            catch (Exception e)
            {
                Logger.Log.Error(
                    System.Web.HttpContext.Current.Request.UserHostAddress, "", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);

                string mensaje = $"Fin de busqueda. Cantidad de liquidaciones observadas: 0";
                return mensaje;
            }

        }


        /// <summary>
        /// Persiste las liquidaciones observadas que trae de SAP^.
        /// </summary>
        /// <param name="liquidacionesObservadas"></param>
        /// <param name="vendedor"></param>
        public void ComunicarLiquidacionesObservadas(List<LiquidacionView> liquidacionesObservadas, string vendedor)
        {
            // Agregar aquí la lógica para verificar cuentasHabilitadasDebeNotificarse y cuentasHabilitadasComunicado si es necesario.

           

        

            if (liquidacionesObservadas != null)
            {
                foreach (var liquidacion in liquidacionesObservadas)
                {
                    // Verificar si el registro ya existe en la base de datos
                    if (!ExisteComprobanteEnBaseDeDatos(6, vendedor, liquidacion.comprobante))
                    {
                        Comunicacion oComunicacion = new Comunicacion
                        {
                            ComunicacionTipo = 6,
                            ProveedorId = vendedor,
                            FechaCreacion = DateTime.Now.AddSeconds(-DateTime.Now.Second).AddMilliseconds(-DateTime.Now.Second),
                            Leida = false,
                            Comprobante = liquidacion.comprobante
                        };

                        repositorio.Agregar(oComunicacion);
                        repositorio.GuardarCambios();
                    }
                }
            }
        }

        /// <summary>
        /// Verifica que la liquidacion observada no esté cargada previamente en la tabla de campana de notificaciones.
        /// El fin principal es verificar si fué previamente grabada, para no persistir mas de una vez cada liquidacion observada.
        /// </summary>
        /// <param name="comunicacionTipo"></param>
        /// <param name="proveedorId"></param>
        /// <param name="comprobante"></param>
        /// <returns></returns>
        private bool ExisteComprobanteEnBaseDeDatos(int comunicacionTipo, string proveedorId, string comprobante)
        {
            bool comunicacionPersistida = repositorio.Listar<Comunicacion>()
                .Any(x => x.ComunicacionTipo == 6
                    && x.ProveedorId == proveedorId
                    && x.Comprobante == comprobante);

            return comunicacionPersistida; 
        }

    }
}

