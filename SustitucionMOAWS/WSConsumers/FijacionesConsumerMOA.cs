using SustitucionMOAFotmatter;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Contrato;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FijacionesWebServiceMOA;
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
    public class FijacionesConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        public object request(string proveedor, List<string> contratos, List<FechaWS> fechas, string tipo_op)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4200[] fijaciones_out = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4200[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] materiales = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] vendedores = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] { };
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060> contratos_inSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060>() { };
                    foreach (string contrato in contratos)
                    {
                        contratos_inSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060()
                        {
                            CONTRATO = contrato
                        });
                    }
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_inSAPArray = contratos_inSAP.ToArray();

                    var request = new Z_MPMF_MOAOP_FIJACIONES()
                    {
                        PE_PROVEEDOR = proveedor,
                        PE_TIPO_OP = tipo_op,
                        T_CONTRATOS_IN = contratos_inSAPArray,
                        T_FECHA_OPERACION_IN = fechasSAPArray,
                        T_FIJACIONES_OUT = fijaciones_out,
                        T_MATERIAL_IN = materiales,
                        T_VENDEDOR_IN = vendedores
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_FIJACIONES request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_FIJACIONES(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_FIJACIONES response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.MENSAJE_ERROR , response.T_CONTRATOS_IN, response.T_FECHA_OPERACION_IN , response.T_FIJACIONES_OUT , response.T_MATERIAL_IN , response.T_VENDEDOR_IN);


                }
                else
                {
                    FijacionesWebServiceMOA.ZMPES4200[] fijaciones_out = new FijacionesWebServiceMOA.ZMPES4200[] { };
                    FijacionesWebServiceMOA.ZMPES4090[] materiales = new FijacionesWebServiceMOA.ZMPES4090[] { };
                    FijacionesWebServiceMOA.ZMPES4080[] vendedores = new FijacionesWebServiceMOA.ZMPES4080[] { };
                    List<FijacionesWebServiceMOA.ZMPES4100> fechasSAP = new List<FijacionesWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new FijacionesWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    FijacionesWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    List<FijacionesWebServiceMOA.ZMPES4060> contratos_inSAP = new List<FijacionesWebServiceMOA.ZMPES4060>() { };
                    foreach (string contrato in contratos)
                    {
                        contratos_inSAP.Add(new FijacionesWebServiceMOA.ZMPES4060()
                        {
                            CONTRATO = contrato
                        });
                    }
                    FijacionesWebServiceMOA.ZMPES4060[] contratos_inSAPArray = contratos_inSAP.ToArray();
                    //ZmprfcGolFijaciones requestInfo = new ZmprfcGolFijaciones { PeProveedor = proveedor, TFechaOperacionIn = fechasSAP.ToArray(), PeTipoOp = "FIJ" };
                    SI_MPMF_MOAOP_FIJACIONESClient service = new SI_MPMF_MOAOP_FIJACIONESClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    FijacionesWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_FIJACIONES(proveedor, tipo_op, ref contratos_inSAPArray, ref fechasSAPArray, ref fijaciones_out, ref materiales, ref vendedores);
                    return Map(error, contratos_inSAPArray, fechasSAPArray, fijaciones_out, materiales, vendedores);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(FijacionesWebServiceMOA.ZMPES4910 error, FijacionesWebServiceMOA.ZMPES4060[] contratos_in, FijacionesWebServiceMOA.ZMPES4100[] fechasSAPArray, FijacionesWebServiceMOA.ZMPES4200[] fijaciones_out, FijacionesWebServiceMOA.ZMPES4090[] materiales, FijacionesWebServiceMOA.ZMPES4080[] vendedores)
        {
            ContratosNoCumplidosWSMOAResponse result = new ContratosNoCumplidosWSMOAResponse();

            if (error != null) {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (FijacionesWebServiceMOA.ZMPES4200 contrato in fijaciones_out)
            {
                result.contratosInfo.Add(new ContratoNoCumplidoView()
                {
                    ampliadoString = SAPFormatter.FormatearCantidad(contrato.AMPLIADO, contrato.UNIME_AMPLIADO),
                    ampliado = contrato.AMPLIADO,
                    anuladoString = SAPFormatter.FormatearCantidad(contrato.ANULADO, contrato.UNIME_ANULADO),
                    anulado = contrato.ANULADO,
                    contrvend = contrato.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contrato.FECHA_CONTRATO),
                    fechaDate = SAPFormatter.GetDateTime(contrato.FECHA_CONTRATO),
                    kilosFijadosString = SAPFormatter.FormatearCantidad(contrato.KILOS_FIJADOS, contrato.UNIME_FIJADO),
                    kilosFijados = contrato.KILOS_FIJADOS,
                    cantKilosString = SAPFormatter.FormatearCantidad(contrato.KILOS_PACTADOS, contrato.UNIME_PACTADO),
                    cantKilos = contrato.KILOS_PACTADOS,
                    liquidadoString = SAPFormatter.FormatearCantidad(contrato.LIQUIDADO, contrato.UNIME_LIQUIDADO),
                    liquidado = contrato.LIQUIDADO,
                    material = contrato.MATERIAL,
                    tipoContrato = TipoContrato.GetTipoContrato(contrato.CLASE_DOC, "", contrato.DOL_EXPRESS, contrato.DOL_CORREDOR, contrato.PAGO_DIF_ARP, "", "", "", contrato.DOLARIZADO),
                    nroContrato = contrato.NRO_CONTRATO,
                    importeString = SAPFormatter.FormatearMonto(contrato.IMPORTE, contrato.MONEDA),
                    importe = contrato.IMPORTE,
                    totalString = SAPFormatter.FormatearCantidad(contrato.KILOS_PACTADOS - contrato.ANULADO + contrato.AMPLIADO, contrato.UNIME_PACTADO),
                    total = contrato.KILOS_PACTADOS - contrato.ANULADO + contrato.AMPLIADO,
                    dolarizado = contrato.DOLARIZADO,
                    dolarExpress = contrato.DOL_EXPRESS,
                    dolarCorredor = contrato.DOL_CORREDOR,
                    fechaLimite = contrato.FECHA_LIMITE,
                    pagoDiferidoArp = contrato.PAGO_DIF_ARP,
                    diasDiferim = contrato.DIAS_DIFERIM

                }
                );
            }

            foreach (FijacionesWebServiceMOA.ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (FijacionesWebServiceMOA.ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (FijacionesWebServiceMOA.ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }

            return result;
        }
        protected virtual object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_in, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4200[] fijaciones_out, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] materiales, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] vendedores)
        {
            ContratosNoCumplidosWSMOAResponse result = new ContratosNoCumplidosWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4200 contrato in fijaciones_out)
            {
                result.contratosInfo.Add(new ContratoNoCumplidoView()
                {
                    ampliadoString = SAPFormatter.FormatearCantidad(contrato.AMPLIADO, contrato.UNIME_AMPLIADO),
                    ampliado = contrato.AMPLIADO,
                    anuladoString = SAPFormatter.FormatearCantidad(contrato.ANULADO, contrato.UNIME_ANULADO),
                    anulado = contrato.ANULADO,
                    contrvend = contrato.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contrato.FECHA_CONTRATO),
                    fechaDate = SAPFormatter.GetDateTime(contrato.FECHA_CONTRATO),
                    kilosFijadosString = SAPFormatter.FormatearCantidad(contrato.KILOS_FIJADOS, contrato.UNIME_FIJADO),
                    kilosFijados = contrato.KILOS_FIJADOS,
                    cantKilosString = SAPFormatter.FormatearCantidad(contrato.KILOS_PACTADOS, contrato.UNIME_PACTADO),
                    cantKilos = contrato.KILOS_PACTADOS,
                    liquidadoString = SAPFormatter.FormatearCantidad(contrato.LIQUIDADO, contrato.UNIME_LIQUIDADO),
                    liquidado = contrato.LIQUIDADO,
                    material = contrato.MATERIAL,
                    tipoContrato = TipoContrato.GetTipoContrato(contrato.CLASE_DOC, "", contrato.DOL_EXPRESS, contrato.DOL_CORREDOR, contrato.PAGO_DIF_ARP, "", "", "", contrato.DOLARIZADO),
                    nroContrato = contrato.NRO_CONTRATO,
                    importeString = SAPFormatter.FormatearMonto(contrato.IMPORTE, contrato.MONEDA),
                    importe = contrato.IMPORTE,
                    totalString = SAPFormatter.FormatearCantidad(contrato.KILOS_PACTADOS - contrato.ANULADO + contrato.AMPLIADO, contrato.UNIME_PACTADO),
                    total = contrato.KILOS_PACTADOS - contrato.ANULADO + contrato.AMPLIADO,
                    dolarizado = contrato.DOLARIZADO,
                    dolarExpress = contrato.DOL_EXPRESS,
                    dolarCorredor = contrato.DOL_CORREDOR,
                    fechaLimite = contrato.FECHA_LIMITE,
                    pagoDiferidoArp = contrato.PAGO_DIF_ARP,
                    diasDiferim = contrato.DIAS_DIFERIM

                }
                );
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }

            return result;
        }

    }

    public class FijacionesExcelConsumerMOA : FijacionesConsumerMOA
    {
        protected override object Map(FijacionesWebServiceMOA.ZMPES4910 error, FijacionesWebServiceMOA.ZMPES4060[] contratos_in, FijacionesWebServiceMOA.ZMPES4100[] fechasSAPArray, FijacionesWebServiceMOA.ZMPES4200[] fijaciones_out, FijacionesWebServiceMOA.ZMPES4090[] materiales, FijacionesWebServiceMOA.ZMPES4080[] vendedores)
        {
            ContratosNoCumplidosExcelWSMOAResponse result = new ContratosNoCumplidosExcelWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (FijacionesWebServiceMOA.ZMPES4200 contrato in fijaciones_out)
            {
                result.contratosInfo.Add(new ContratoNoCumplido()
                {
                    unidadAmpliado = contrato.UNIME_AMPLIADO,
                    ampliado = contrato.AMPLIADO, 
                    unidadAnulado = contrato.UNIME_ANULADO,
                    anulado = contrato.ANULADO,
                    contrvend = contrato.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contrato.FECHA_CONTRATO),
                    unidadKilosFijados = contrato.UNIME_FIJADO,
                    kilosFijados = contrato.KILOS_FIJADOS,
                    unidadCantKilos = contrato.UNIME_PACTADO,
                    cantKilos = contrato.KILOS_PACTADOS,
                    unidadLiquidado = contrato.UNIME_LIQUIDADO,
                    liquidado = contrato.LIQUIDADO,
                    material = contrato.MATERIAL,
                    nroContrato = contrato.NRO_CONTRATO,
                    moneda =  contrato.MONEDA,
                    importe = contrato.IMPORTE,
                    unidadTotal = contrato.UNIME_PACTADO,
                    total = contrato.KILOS_PACTADOS - contrato.ANULADO + contrato.AMPLIADO,
                }
                );
            }

            foreach (FijacionesWebServiceMOA.ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (FijacionesWebServiceMOA.ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (FijacionesWebServiceMOA.ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }

            return result;
        }
        protected override object MapSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4910 error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_in, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4200[] fijaciones_out, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] materiales, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] vendedores)
        {
            ContratosNoCumplidosExcelWSMOAResponse result = new ContratosNoCumplidosExcelWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4200 contrato in fijaciones_out)
            {
                result.contratosInfo.Add(new ContratoNoCumplido()
                {
                    unidadAmpliado = contrato.UNIME_AMPLIADO,
                    ampliado = contrato.AMPLIADO,
                    unidadAnulado = contrato.UNIME_ANULADO,
                    anulado = contrato.ANULADO,
                    contrvend = contrato.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contrato.FECHA_CONTRATO),
                    unidadKilosFijados = contrato.UNIME_FIJADO,
                    kilosFijados = contrato.KILOS_FIJADOS,
                    unidadCantKilos = contrato.UNIME_PACTADO,
                    cantKilos = contrato.KILOS_PACTADOS,
                    unidadLiquidado = contrato.UNIME_LIQUIDADO,
                    liquidado = contrato.LIQUIDADO,
                    material = contrato.MATERIAL,
                    nroContrato = contrato.NRO_CONTRATO,
                    moneda = contrato.MONEDA,
                    importe = contrato.IMPORTE,
                    unidadTotal = contrato.UNIME_PACTADO,
                    total = contrato.KILOS_PACTADOS - contrato.ANULADO + contrato.AMPLIADO,
                }
                );
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }

            return result;
        }

    }
}
