using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ListarPesificaciones;
using System;

namespace SustitucionMOAWS.WSConsumers
{
    //public interface IListarPesificacionesConsumer
    //{
    //    ListarPesificacionesWSMOAResponse Request(string proveedor);
    //}

    public class ListarPesificacionesConsumer : IListarPesificacionesConsumer
    {
        readonly SI_MPMF_MOAOP_LISTAR_PESIFClient service = new SI_MPMF_MOAOP_LISTAR_PESIFClient();
        private readonly IRepositorio repositorio;

        public ListarPesificacionesConsumer(IRepositorio repositorio)
        {
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;

        }

        public ListarPesificacionesWSMOAResponse Request(string proveedor)
        {
            var pesificaciones = service.SI_MPMF_MOAOP_LISTAR_PESIF(proveedor);
            return Map(pesificaciones);
        }

        private ListarPesificacionesWSMOAResponse Map(ZMPES6500[] pesificaciones)
        {
            ListarPesificacionesWSMOAResponse result = new ListarPesificacionesWSMOAResponse();

            foreach (ZMPES6500 pesificacion in pesificaciones)
            {
                var FechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_PESIFICACION, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                var Soja200FechaCotizacion = repositorio.Obtener<Configuracion>(a => a.Code == "Soja200FechaCotizacion").Value;
                if (FechaPesificacionDate.Date == DateTime.ParseExact(Soja200FechaCotizacion, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
                {
                    FechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
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
                    FechaPesificacion = SAPFormatter.FormatearFecha(FechaPesificacionDate),
                    FechaPesificacionDate = FechaPesificacionDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    TipoCambio = SAPFormatter.FormatearMonto(pesificacion.TIPO_CAMBIO, "ARP"),
                });
            }

            return result;
        }
    }
}
