using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ProformaFleteProcedenciaServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class ProformaFleteProcedenciaConsumerMOA
    {
        SI_MPMF_MOAOP_DET_FLETE_PROCClient service = new SI_MPMF_MOAOP_DET_FLETE_PROCClient();

        public object request(string proveedor, string contrato)
        {
            try
            {
                ZMPES6250[] salida = new ZMPES6250[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string error = service.SI_MPMF_MOAOP_DET_FLETE_PROC(contrato, proveedor, out salida);
                return map(error, salida);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(string error, ZMPES6250[] salida)
        {
            ProcedenciaFleteWSMOAResponse result = new ProcedenciaFleteWSMOAResponse();

            result.error = error;


            foreach (ZMPES6250 procendenciaFlete in salida)
            {
                result.procedenciasFlete.Add(new ProcedenciaFleteView()
                {
                    contrato = procendenciaFlete.CONTRATO,
                    cartaPorte = procendenciaFlete.CARTA_PORTE,
                    dtramo = procendenciaFlete.DTRAMO,
                    fechaIngreso = SAPFormatter.FormatearFecha(procendenciaFlete.FECHA_INGRESO),
                    fechaIngresoDate = SAPFormatter.GetDateTime(procendenciaFlete.FECHA_INGRESO),
                    fijacion = procendenciaFlete.FIJACION,
                    importeArp = procendenciaFlete.IMPORTE_ARP,
                    importeArpString = SAPFormatter.FormatearMonto(procendenciaFlete.IMPORTE_ARP, "$"),
                    importeUsdm = procendenciaFlete.IMPORTE_USDM,
                    importeUsdmString = SAPFormatter.FormatearMonto(procendenciaFlete.IMPORTE_USDM, "U$S"),
                    ivaArp = procendenciaFlete.IVA_ARP,
                    ivaArpString = SAPFormatter.FormatearMonto(procendenciaFlete.IVA_ARP, "$"),
                    ivaUsdm = procendenciaFlete.IVA_USDM,
                    ivaUsdmString = SAPFormatter.FormatearMonto(procendenciaFlete.IVA_USDM, "U$S"),
                    kilos = procendenciaFlete.KILOS,
                    kilosString = SAPFormatter.FormatearCantidad(procendenciaFlete.KILOS, "Kg"),
                    tarifaUsdm = procendenciaFlete.TARIFA_USDM,
                    tarifaUsdmString = SAPFormatter.FormatearMonto(procendenciaFlete.TARIFA_USDM, "U$S")
                });
            }
            return result;
        }
    }

    public class ProformaFleteProcedenciaExcelConsumerMOA : ProformaFleteProcedenciaConsumerMOA
    {
        protected override object map(string error, ZMPES6250[] salida)
        {
            ProcedenciaFleteExcelWSMOAResponse result = new ProcedenciaFleteExcelWSMOAResponse();

            result.error = error;


            foreach (ZMPES6250 procendenciaFlete in salida)
            {
                result.procedenciasFlete.Add(new ProcedenciaFlete()
                {
                    contrato = procendenciaFlete.CONTRATO,
                    cartaPorte = procendenciaFlete.CARTA_PORTE,
                    dtramo = procendenciaFlete.DTRAMO,
                    fechaIngreso = SAPFormatter.FormatearFecha(procendenciaFlete.FECHA_INGRESO),
                    fijacion = procendenciaFlete.FIJACION,
                    importeArpString = SAPFormatter.FormatearMonto(procendenciaFlete.IMPORTE_ARP, "$"),
                    importeUsdmString = SAPFormatter.FormatearMonto(procendenciaFlete.IMPORTE_USDM, "U$S"),
                    ivaArpString = SAPFormatter.FormatearMonto(procendenciaFlete.IVA_ARP, "$"),
                    ivaUsdmString = SAPFormatter.FormatearMonto(procendenciaFlete.IVA_USDM, "U$S"),
                    kilosString = SAPFormatter.FormatearCantidad(procendenciaFlete.KILOS, "Kg"),
                    tarifaUsdmString = SAPFormatter.FormatearMonto(procendenciaFlete.TARIFA_USDM, "U$S")
                });
            }
            return result;
        }
    }
}
