using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Fijacion.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FijacionDetalleWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class FijacionDetalleConsumerMOA
    {
        SI_MPMF_MOAOP_DET_FIJACIONESClient service = new SI_MPMF_MOAOP_DET_FIJACIONESClient();

        public FijacionDetalleWSMOAResponse request(string proveedor, string contrato, string fijacion)
        {
            try
            {
                ZMPES4680[] fijaciones = new ZMPES4680[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_DET_FIJACIONES(contrato, fijacion,  proveedor, ref fijaciones);
                FijacionDetalleWSMOAResponse result = map(error, fijaciones);
                result.contrato = contrato;
                result.fijacion = fijacion;
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private FijacionDetalleWSMOAResponse map(ZMPES4910 error, ZMPES4680[] fijaciones)
        {
            FijacionDetalleWSMOAResponse result = new FijacionDetalleWSMOAResponse();

            if (error != null) {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            decimal total = 0;
            string unidad = "KG";

            foreach (ZMPES4680 fijacion in fijaciones)
            {
                result.detalleFijacion.Add(new FijacionDetalle()
                {
                    cartaPorte = fijacion.CARTA_PORTE,
                    fechaDescarga = SAPFormatter.FormatearFecha(fijacion.FECHA_DESCARGA),
                    netoDescontado = SAPFormatter.FormatearCantidad(fijacion.NETO_DESCONTADO, fijacion.NETO_UNIME)
                });

                total += fijacion.NETO_DESCONTADO;
                unidad = fijacion.NETO_UNIME;
            }

            result.total = SAPFormatter.FormatearCantidad(total, unidad);

            return result;
        }
    }
}
