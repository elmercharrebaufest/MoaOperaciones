using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.VinculaDetalleWebServiceMOA;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class VinculaDetalleConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public object request(string proveedor, string contrato, string secuencia)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_DET_VINCULA()
                    {
                        PE_CONTRATO = contrato,
                        PE_PROVEEDOR = proveedor,
                        PE_SECUENCIA = secuencia
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_VINCULA request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DET_VINCULA(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_VINCULA response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.MENSAJE_ERROR, response.T_SALIDA);
                }
                else
                {
                    VinculaDetalleWebServiceMOA.ZMPES6090[] salidas = new VinculaDetalleWebServiceMOA.ZMPES6090[] { };
                    SI_MPMF_MOAOP_DET_VINCULAClient service = new SI_MPMF_MOAOP_DET_VINCULAClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    VinculaDetalleWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_DET_VINCULA(contrato, proveedor, secuencia, out salidas);
                    return Map(error, salidas);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(VinculaDetalleWebServiceMOA.ZMPES4910 error, VinculaDetalleWebServiceMOA.ZMPES6090[] salidas)
        {
            VinculaDetalleWSMOAResponse result = new VinculaDetalleWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
            }

            foreach (VinculaDetalleWebServiceMOA.ZMPES6090 cartaPorte in salidas)
            {
                result.data.Add(new DetalleVinculaView()
                {
                    cartaPorte = cartaPorte.CARTA_PORTE,
                    fecha = SAPFormatter.FormatearFecha(cartaPorte.FECHA),
                    kgLiquidadosString = SAPFormatter.FormatearCantidad(cartaPorte.KG_LIQUIDADOS, "KG"),
                    kgRecibidosString = SAPFormatter.FormatearCantidad(cartaPorte.KG_RECIBIDOS, "KG")
                });
            }

            return result;
        }
        protected virtual object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6090[] salidas)
        {
            VinculaDetalleWSMOAResponse result = new VinculaDetalleWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6090 cartaPorte in salidas)
            {
                result.data.Add(new DetalleVinculaView()
                {
                    cartaPorte = cartaPorte.CARTA_PORTE,
                    fecha = SAPFormatter.FormatearFecha(cartaPorte.FECHA),
                    kgLiquidadosString = SAPFormatter.FormatearCantidad(cartaPorte.KG_LIQUIDADOS, "KG"),
                    kgRecibidosString = SAPFormatter.FormatearCantidad(cartaPorte.KG_RECIBIDOS, "KG")
                });
            }

            return result;
        }
    }

    public class VinculaDetalleExcelConsumerMOA : VinculaDetalleConsumerMOA
    {

        protected override object Map(VinculaDetalleWebServiceMOA.ZMPES4910 error, VinculaDetalleWebServiceMOA.ZMPES6090[] salidas)
        {
            VinculaDetalleExcelWSMOAResponse result = new VinculaDetalleExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
            }

            foreach (VinculaDetalleWebServiceMOA.ZMPES6090 cartaPorte in salidas)
            {
                result.data.Add(new DetalleVincula()
                {
                    cartaPorte = cartaPorte.CARTA_PORTE,
                    fecha = SAPFormatter.FormatearFecha(cartaPorte.FECHA),
                    unidad = "KG",
                    kgLiquidados = cartaPorte.KG_LIQUIDADOS,
                    kgRecibidos = cartaPorte.KG_RECIBIDOS
                });
            }

            return result;
        }
        protected override object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6090[] salidas)
        {
            VinculaDetalleExcelWSMOAResponse result = new VinculaDetalleExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6090 cartaPorte in salidas)
            {
                result.data.Add(new DetalleVincula()
                {
                    cartaPorte = cartaPorte.CARTA_PORTE,
                    fecha = SAPFormatter.FormatearFecha(cartaPorte.FECHA),
                    unidad = "KG",
                    kgLiquidados = cartaPorte.KG_LIQUIDADOS,
                    kgRecibidos = cartaPorte.KG_RECIBIDOS
                });
            }

            return result;
        }
    }
}
