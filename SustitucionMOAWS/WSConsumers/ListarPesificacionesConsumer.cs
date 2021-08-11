using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ListarPesificaciones;
using System;

namespace SustitucionMOAWS.WSConsumers
{
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
                    FechaCarga = DateTime.ParseExact(pesificacion.FECHA_CARGA, "yyyy-mm-dd", null),
                    Contrato = pesificacion.CONTRATO,
                    Fijacion = pesificacion.FIJACION,
                    Kilos = pesificacion.KILOS,
                    Precio = pesificacion.PRECIO,
                    FechaPesificacion = DateTime.ParseExact(pesificacion.FECHA_PESIFICACION, "yyyy-mm-dd", null),
                    TipoCambio = pesificacion.TIPO_CAMBIO
                });
            }

            return result;
        }
    }
}
