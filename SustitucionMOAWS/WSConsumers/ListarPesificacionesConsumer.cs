using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
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

        public ListarPesificacionesConsumer()
        {
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
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
                result.Pesificaciones.Add(new PesificacionSapDto
                {
                    FechaCarga = SAPFormatter.FormatearFecha(pesificacion.FECHA_CARGA),
                    FechaCargaDate = (DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-MM-dd", null)).ToString("yyyy-MM-ddTHH:mm:ss"),
                    Contrato = pesificacion.CONTRATO,
                    Fijacion = pesificacion.FIJACION,
                    Kilos = pesificacion.KILOS,
                    KilosString = SAPFormatter.FormatearCantidad(pesificacion.KILOS, "KG"),
                    Precio = pesificacion.PRECIO,
                    PrecioString = SAPFormatter.FormatearMonto(pesificacion.PRECIO, "ARP"),
                    FechaPesificacion = SAPFormatter.FormatearFecha(pesificacion.FECHA_PESIFICACION),
                    FechaPesificacionDate = DateTime.ParseExact(pesificacion.FECHA_PESIFICACION, "yyyy-MM-dd", null).ToString("yyyy-MM-ddTHH:mm:ss"),
                    TipoCambio = SAPFormatter.FormatearMonto(pesificacion.TIPO_CAMBIO, "ARP"),
                });
            }

            return result;
        }
    }
}
