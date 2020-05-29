using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.VinculaDetalleWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class VinculaDetalleConsumerMOA
    {
        SI_MPMF_MOAOP_DET_VINCULAClient service = new SI_MPMF_MOAOP_DET_VINCULAClient();

        public object request(string proveedor, string contrato, string secuencia)
        {
            try
            {
                ZMPES6090[] salidas = new ZMPES6090[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_DET_VINCULA(contrato, proveedor, secuencia, out salidas);
                return map(error, salidas);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES6090[] salidas)
        {
            VinculaDetalleWSMOAResponse result = new VinculaDetalleWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
            }

            foreach (ZMPES6090 cartaPorte in salidas)
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

        protected override object map(ZMPES4910 error, ZMPES6090[] salidas)
        {
            VinculaDetalleExcelWSMOAResponse result = new VinculaDetalleExcelWSMOAResponse();

            if (error != null)
            {
                result.error.codigo = error.CODIGO;
                result.error.descripcion = error.DESCRIPCION;
            }

            foreach (ZMPES6090 cartaPorte in salidas)
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
