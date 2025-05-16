using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ListarPesificaciones;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    //public interface IListarPesificacionesConsumer
    //{
    //    ListarPesificacionesWSMOAResponse Request(string proveedor);
    //}

    public class ListarPesificacionesConsumer : IListarPesificacionesConsumer
    {

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly IRepositorio repositorio;

        public ListarPesificacionesConsumer(IRepositorio repositorio)
        {
            this.repositorio = repositorio;

        }

        public ListarPesificacionesWSMOAResponse Request(string proveedor)
        {
            Log.Info($"Ingresando al Metodo ListarPesificacionesWSMOAResponse");
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                var request = new Z_MPMF_MOAOP_LISTAR_PESIF()
                {
                    IM_LIFNR = proveedor
                };
                Log.Info($"Sin PI Z_MPMF_MOAOP_LISTAR_PESIF: {new { request.IM_LIFNR }}");

                var response = agent.Z_MPMF_MOAOP_LISTAR_PESIF(request);
                Log.Info($"Sin PI Z_MPMF_MOAOP_LISTAR_PESIF: {new { request.IM_LIFNR }}");

                return MapSinPI(response);
            }
            else
            {
                SI_MPMF_MOAOP_LISTAR_PESIFClient service = new SI_MPMF_MOAOP_LISTAR_PESIFClient();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                var pesificaciones = service.SI_MPMF_MOAOP_LISTAR_PESIF(proveedor);
                return Map(pesificaciones);
            }

        }
        private ListarPesificacionesWSMOAResponse MapSinPI(Z_MPMF_MOAOP_LISTAR_PESIFResponse response)
        {
            var result = new ListarPesificacionesWSMOAResponse();
            var soja200FechaCotizacionStr = repositorio.Obtener<Configuracion>(a => a.Code == "Soja200FechaCotizacion").Value;
            var soja200FechaCotizacion = DateTime.ParseExact(soja200FechaCotizacionStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6500 pesificacion in response.EX_SALIDA)
            {
                var fechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_PESIFICACION, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                if (fechaPesificacionDate.Date == soja200FechaCotizacion)
                {
                    fechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                }
                result.Pesificaciones.Add(new PesificacionSapDto
                {
                    FechaCarga = SAPFormatter.FormatearFecha(pesificacion.FECHA_CARGA),
                    FechaCargaDate = (DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)).ToString("yyyy-MM-ddTHH:mm:ss"),
                    Contrato = pesificacion.CONTRATO,
                    Fijacion = pesificacion.FIJACION,
                    Kilos = pesificacion.KILOS,
                    KilosString = SAPFormatter.FormatearCantidad(pesificacion.KILOS, "KG"),
                    Precio = pesificacion.PRECIO,
                    PrecioString = SAPFormatter.FormatearMonto(pesificacion.PRECIO, "USD"),
                    FechaPesificacion = SAPFormatter.FormatearFecha(fechaPesificacionDate),
                    FechaPesificacionDate = fechaPesificacionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TipoCambio = SAPFormatter.FormatearMonto(pesificacion.TIPO_CAMBIO, "ARP"),
                });
            }

            return result;
        }
        private ListarPesificacionesWSMOAResponse Map(ListarPesificaciones.ZMPES6500[] pesificaciones)
        {
            var result = new ListarPesificacionesWSMOAResponse();
            var soja200FechaCotizacionStr = repositorio.Obtener<Configuracion>(a => a.Code == "Soja200FechaCotizacion").Value;
            var soja200FechaCotizacion = DateTime.ParseExact(soja200FechaCotizacionStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            foreach (ListarPesificaciones.ZMPES6500 pesificacion in pesificaciones)
            {
                var fechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_PESIFICACION, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                if (fechaPesificacionDate.Date == soja200FechaCotizacion)
                {
                    fechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                }
                result.Pesificaciones.Add(new PesificacionSapDto
                {
                    FechaCarga = SAPFormatter.FormatearFecha(pesificacion.FECHA_CARGA),
                    FechaCargaDate = (DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture)).ToString("yyyy-MM-ddTHH:mm:ss"),
                    Contrato = pesificacion.CONTRATO,
                    Fijacion = pesificacion.FIJACION,
                    Kilos = pesificacion.KILOS,
                    KilosString = SAPFormatter.FormatearCantidad(pesificacion.KILOS, "KG"),
                    Precio = pesificacion.PRECIO,
                    PrecioString = SAPFormatter.FormatearMonto(pesificacion.PRECIO, "USD"),
                    FechaPesificacion = SAPFormatter.FormatearFecha(fechaPesificacionDate),
                    FechaPesificacionDate = fechaPesificacionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TipoCambio = SAPFormatter.FormatearMonto(pesificacion.TIPO_CAMBIO, "ARP"),
                });
            }

            return result;
        }
    }
}
