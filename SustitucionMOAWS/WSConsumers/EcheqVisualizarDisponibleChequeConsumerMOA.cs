using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.EcheqVisualizarDisponibleChequeWebServiceMOA;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.Logger;
using System.Configuration;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;

namespace SustitucionMOAWS.WSConsumers
{
    class EcheqVisualizarDisponibleChequeConsumerMOA : IEcheqVisualizarDisponibleChequeConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public EcheqVisualizarDisponibleChequeConsumerMOA()
        {

        }

        public List<EcheqLiquidacionDto> Request(string proveedor, List<FechaWS> listaFechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] { };

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] {
                        new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                    }

                    string fechahasta = "";

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                    }

                    string IM_CONTRATO = "";
                    string IM_DOCUMENTO = "";


                    var req = new Z_MPRFC_VISU_DISPONIBLE_CHEQUE()
                    {
                        IM_CONTRATO = IM_DOCUMENTO,
                        IM_FECHA = fechas,
                        IM_COE = IM_CONTRATO,
                        IM_PROVEEDOR = proveedor
                    };
                    Log.Info($"Sin PI Z_MPRFC_VISU_DISPONIBLE_CHEQUE Request: {new { IM_CONTRATO, IM_DOCUMENTO, fechas, proveedor }}");
                    var response = agent.Z_MPRFC_VISU_DISPONIBLE_CHEQUE(req);
                    Log.Info($"Sin PI Z_MPRFC_VISU_DISPONIBLE_CHEQUE  Response: {response}");
                    return MapSinPI(response);

                }
                else
                {
                    SI_MPRFC_VISU_DISPONIBLE_CHEQUEClient service = new SI_MPRFC_VISU_DISPONIBLE_CHEQUEClient();
                    service = new SI_MPRFC_VISU_DISPONIBLE_CHEQUEClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    EcheqVisualizarDisponibleChequeWebServiceMOA.ZMPES4100[] fechas = new EcheqVisualizarDisponibleChequeWebServiceMOA.ZMPES4100[] { };

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechas = new EcheqVisualizarDisponibleChequeWebServiceMOA.ZMPES4100[] {
                        new EcheqVisualizarDisponibleChequeWebServiceMOA.ZMPES4100 {
                            FECHA_OP = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin)
                        }
                    };
                    }

                    string fechahasta = "";

                    if (listaFechas.FirstOrDefault() != null)
                    {
                        fechahasta = SAPFormatter.PrepararFecha(listaFechas.FirstOrDefault().fechaFin);
                    }

                    string IM_CONTRATO = "";
                    string IM_DOCUMENTO = "";


                    Log.Info($"SI_MPRFC_VISU_DISPONIBLE_CHEQUE Request: {new { IM_CONTRATO, IM_DOCUMENTO, fechas, proveedor }}");
                    var response = service.SI_MPRFC_VISU_DISPONIBLE_CHEQUE(IM_CONTRATO, IM_DOCUMENTO, fechas, proveedor);
                    Log.Info($"SI_MPRFC_VISU_DISPONIBLE_CHEQUE Response: {response}");

                    return Map(response);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<EcheqLiquidacionDto> MapSinPI(Z_MPRFC_VISU_DISPONIBLE_CHEQUEResponse response)
        {
            List<EcheqLiquidacionDto> listaChequesdisponibles = new List<EcheqLiquidacionDto>() { };


            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6890 cheque in response.EX_SALIDA)
            {
                listaChequesdisponibles.Add(new EcheqLiquidacionDto()
                {
                    //Contrato = cheque.CONTRATO,
                    //Pedido = cheque.PEDIDO,
                    //Sociedad = cheque.SOCIEDAD,
                    Documento = cheque.DOCUMENTO,
                    Ejercicio = cheque.EJERCICIO,
                    Fecha = SAPFormatter.FormatearFecha(cheque.FECHA),
                    NumeroCOE = cheque.XBLNR,
                    Solapa = cheque.SOLAPA,
                    ImporteMonedaDocumento = cheque.WRBTR,
                    //wRBTRFieldSpecified = cheque.WRBTRSpecified,
                    ImporteEnPesos = cheque.DMBTR,
                    //dMBTRFieldSpecified = cheque.DMBTRSpecified,
                    Moneda = cheque.MONEDA,
                    //Clasificacion = cheque.CLASIFICACION
                });
            }
            return listaChequesdisponibles;
        }

        private List<EcheqLiquidacionDto> Map(EcheqVisualizarDisponibleChequeWebServiceMOA.ZMPES6890[] EX_SALIDA)
        {      
            List<EcheqLiquidacionDto> listaChequesdisponibles = new List<EcheqLiquidacionDto>() { };
         

            foreach (EcheqVisualizarDisponibleChequeWebServiceMOA.ZMPES6890 cheque in EX_SALIDA)
            {
                listaChequesdisponibles.Add(new EcheqLiquidacionDto()
                {
                   //Contrato = cheque.CONTRATO,
                   //Pedido = cheque.PEDIDO,
                   //Sociedad = cheque.SOCIEDAD,
                   Documento = cheque.DOCUMENTO,
                   Ejercicio = cheque.EJERCICIO,
                   Fecha = SAPFormatter.FormatearFecha(cheque.FECHA),
                   NumeroCOE = cheque.XBLNR,
                   Solapa = cheque.SOLAPA,
                   ImporteMonedaDocumento = cheque.WRBTR,
                   //wRBTRFieldSpecified = cheque.WRBTRSpecified,
                   ImporteEnPesos = cheque.DMBTR,
                   //dMBTRFieldSpecified = cheque.DMBTRSpecified,
                   Moneda = cheque.MONEDA,
                   //Clasificacion = cheque.CLASIFICACION
                });
            }
                return listaChequesdisponibles;            
        }

    }
        internal interface IEcheqVisualizarDisponibleChequeConsumerMOA
        {
        List<EcheqLiquidacionDto> Request(string proveedor, List<FechaWS> listaFechas);
        }
}
