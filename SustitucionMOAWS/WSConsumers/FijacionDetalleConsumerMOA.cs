using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Fijacion.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FijacionDetalleWebServiceMOA;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class FijacionDetalleConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public FijacionDetalleWSMOAResponse request(string proveedor, string contrato, string fijacion)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4680[] fijaciones = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4680[] { };

                    var request = new Z_MPMF_MOAOP_DET_FIJACIONES()
                    {
                        PE_PROVEEDOR = proveedor,
                        PE_CONTRATO = contrato,
                        PE_FIJACION = fijacion,
                        T_DETALLE_FIJACION = fijaciones
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_FIJACIONES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DET_FIJACIONES(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_FIJACIONES response");
                    Log.Info(response.ToXml());
                    FijacionDetalleWSMOAResponse result = MapSinPI(response);
                    result.contrato = contrato;
                    result.fijacion = fijacion;
                    return result;
                }
                else
                {

                    SI_MPMF_MOAOP_DET_FIJACIONESClient service = new SI_MPMF_MOAOP_DET_FIJACIONESClient();
                    FijacionDetalleWebServiceMOA.ZMPES4680[] fijaciones = new FijacionDetalleWebServiceMOA.ZMPES4680[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    FijacionDetalleWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_DET_FIJACIONES(contrato, fijacion, proveedor, ref fijaciones);
                    FijacionDetalleWSMOAResponse result = Map(error, fijaciones);
                    result.contrato = contrato;
                    result.fijacion = fijacion;
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private FijacionDetalleWSMOAResponse MapSinPI(Z_MPMF_MOAOP_DET_FIJACIONESResponse response)
        {
            FijacionDetalleWSMOAResponse result = new FijacionDetalleWSMOAResponse();

            if (response.MENSAJE_ERROR != null)
            {
                result.error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.error.tipo = response.MENSAJE_ERROR.TIPO;
            }

            decimal total = 0;
            string unidad = "KG";

            foreach (var fijacion in response.T_DETALLE_FIJACION)
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
        private FijacionDetalleWSMOAResponse Map(FijacionDetalleWebServiceMOA.ZMPES4910 error, FijacionDetalleWebServiceMOA.ZMPES4680[] fijaciones)
        {
            FijacionDetalleWSMOAResponse result = new FijacionDetalleWSMOAResponse();

            if (error != null) {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
                result.error.tipo = error.TIPO;
            }

            decimal total = 0;
            string unidad = "KG";

            foreach (FijacionDetalleWebServiceMOA.ZMPES4680 fijacion in fijaciones)
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
