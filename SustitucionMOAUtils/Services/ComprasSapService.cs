// Ignore Spelling: Solp Sustitucion Utils Solpe Posicion numeros

using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ComprasSapService : IComprasSapService
    {
        private readonly IObtenerCecoSolpConsumerMOA CentroDeCostoSolpConsumerMOA;
        private readonly IObtenerCuentasSolpConsumerMOA cuentasSolpConsumerMOA;
        private readonly IObtenerOrdenSolpConsumerMOA ordenesSolpConsumerMOA;
        private readonly IObtenerServiciosSolpConsumerMOA serviciosSolpConsumerMOA;
        private readonly IObtenerSolpConsumerMOA obtenerSolpConsumerMOA;

        public ComprasSapService(IObtenerCecoSolpConsumerMOA obtenerCentroDeCostoSolpConsumerMOA,
                                 IObtenerCuentasSolpConsumerMOA obtenerCuentasSolpConsumerMOA,
                                 IObtenerOrdenSolpConsumerMOA obtenerOrdenSolpConsumerMOA,
                                 IObtenerServiciosSolpConsumerMOA obtenerServiciosSolpConsumerMOA,
                                 IObtenerSolpConsumerMOA obtenerSolpConsumerMOA)
        {
            this.CentroDeCostoSolpConsumerMOA = obtenerCentroDeCostoSolpConsumerMOA;
            this.cuentasSolpConsumerMOA = obtenerCuentasSolpConsumerMOA;
            this.ordenesSolpConsumerMOA = obtenerOrdenSolpConsumerMOA;
            this.serviciosSolpConsumerMOA = obtenerServiciosSolpConsumerMOA;
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
        }

        public List<TablaSapDto> ObtenerCentrosDeCostoSap()
        {
            CecoWSMOAResponse resultSap = (CecoWSMOAResponse)CentroDeCostoSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Cecos.ConvertAll(c => new TablaSapDto()
            {
                Tabla = TablasSap.CecoSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.CostCenter, out codigoNum) ? codigoNum.ToString() : c.CostCenter,
                Codigo = c.CostCenter
            });
        }

        public List<TablaSapDto> ObtenerCuentasSap()
        {
            CuentaWSMOAResponse resultSap = (CuentaWSMOAResponse)cuentasSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Cuentas.ConvertAll(c => new TablaSapDto()
            {
                Tabla = TablasSap.CuentasSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            });
        }

        public List<TablaSapDto> ObtenerOrdenesSap(string idOrder = "")
        {
            OrdenWSMOAResponse resultSap = (OrdenWSMOAResponse)ordenesSolpConsumerMOA.request(idOrder);
            var codigoNum = 0;

            return resultSap.Ordenes.ConvertAll(c => new TablaSapDto()
            {
                Tabla = TablasSap.OrdenSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            });
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(string numeroSolp)
        {
            return ObtenerPosiciones(numeroSolp).Where(PosicionPendienteSap);
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<string> numerosSolp)
        {
            ConcurrentQueue<PosicionSolpSAP> result = new ConcurrentQueue<PosicionSolpSAP>();
            numerosSolp
                .Distinct()
                .AsParallel()
                .ForAll(numeroSolp =>
                    ObtenerPosicionesPendientesAdjudicar(numeroSolp)
                    .AsParallel()
                    .ForAll(posicionPendiente =>
                        result.Enqueue(posicionPendiente)
                    )
                );
            return result;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<PosicionSolpSAP> posicionesSap)
            => posicionesSap.Where(PosicionPendienteSap);

        public bool PosicionPendienteSap(PosicionSolpSAP position)
        {
            if (position is null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            return position.Ordered < position.Cantidad;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosiciones(string numeroSolp)
        {
            ObtenerSolpRequest request = new ObtenerSolpRequest()
            {
                NumeroSolp = numeroSolp,
                FechaDesde = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                FechaHasta = DateTime.Today.AddDays(1),
            };

            try
            {
                ObtenerSolpSAPResponse response = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(request);

                return response.Posiciones;
            }
            catch (Exception ex)
            {
                Logger.Log.ExternalAPIError(ex);
                throw;
            }
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosiciones(IEnumerable<string> numerosSolp)
        {
            ConcurrentQueue<PosicionSolpSAP> result = new ConcurrentQueue<PosicionSolpSAP>();
            numerosSolp
                .Distinct()
                .AsParallel()
                .ForAll(numeroSolp =>
                    ObtenerPosiciones(numeroSolp)
                    .AsParallel()
                    .ForAll(posicionPendiente =>
                        result.Enqueue(posicionPendiente)
                    )
                );
            return result;
        }

        public List<TablaSapDto> ObtenerServiciosSap()
        {
            List<Servicio> servicios = ObtenerServiciosSapRaw();
            var codigoNum = 0;

            return servicios.ConvertAll(s => new TablaSapDto()
            {
                Tabla = TablasSap.CodigoServicioSap,
                Descripcion = s.Descripcion,
                CodigoSap = int.TryParse(s.Codigo, out codigoNum) ? codigoNum.ToString() : s.Codigo,
                Codigo = s.Codigo
            });
        }

        public List<Servicio> ObtenerServiciosSapRaw()
        {
            ServicioWSMOAResponse resultSap = (ServicioWSMOAResponse)serviciosSolpConsumerMOA.request();
            return resultSap.Servicios;
        }
    }
}
