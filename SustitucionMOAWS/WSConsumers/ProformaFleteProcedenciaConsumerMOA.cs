using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Proforma;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ProformaFleteProcedenciaServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class ProformaFleteProcedenciaConsumerMOA
    {

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public object request(string proveedor, string contrato)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_DET_FLETE_PROC()
                    {
                        PE_CONTRATO = contrato,
                        PE_PROVEEDOR = proveedor,
                    };

                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_FLETE_PROC request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPMF_MOAOP_DET_FLETE_PROC(request);

                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DET_FLETE_PROC response");
                    SapLogHelper.LogResponse(response.ToXml(), "");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_DET_FLETE_PROCClient service = new SI_MPMF_MOAOP_DET_FLETE_PROCClient();
                    ProformaFleteProcedenciaServiceMOA.ZMPES6250[] salida = new ProformaFleteProcedenciaServiceMOA.ZMPES6250[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string error = service.SI_MPMF_MOAOP_DET_FLETE_PROC(contrato, proveedor, out salida);
                    return map(error, salida);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(string error, ProformaFleteProcedenciaServiceMOA.ZMPES6250[] salida)
        {
            ProcedenciaFleteWSMOAResponse result = new ProcedenciaFleteWSMOAResponse();

            result.error = error;


            foreach (ProformaFleteProcedenciaServiceMOA.ZMPES6250 procendenciaFlete in salida)
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

        protected virtual object MapSinPI(Z_MPMF_MOAOP_DET_FLETE_PROCResponse response)
        {
            ProcedenciaFleteWSMOAResponse result = new ProcedenciaFleteWSMOAResponse();

            result.error = response.PS_RETURN;


            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6250 procendenciaFlete in response.PT_SALIDA)
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

        protected override object MapSinPI(Z_MPMF_MOAOP_DET_FLETE_PROCResponse response)
        {
            ProcedenciaFleteExcelWSMOAResponse result = new ProcedenciaFleteExcelWSMOAResponse();

            result.error = response.PS_RETURN;


            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6250 procendenciaFlete in response.PT_SALIDA)
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

        protected override object map(string error, ProformaFleteProcedenciaServiceMOA.ZMPES6250[] salida)
        {
            ProcedenciaFleteExcelWSMOAResponse result = new ProcedenciaFleteExcelWSMOAResponse();

            result.error = error;


            foreach (ProformaFleteProcedenciaServiceMOA.ZMPES6250 procendenciaFlete in salida)
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
