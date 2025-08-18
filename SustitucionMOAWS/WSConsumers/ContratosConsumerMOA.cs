using SustitucionMOAFotmatter;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Contrato;
using SustitucionMOAWS.ContratosWebServiceMOA;
using SustitucionMOAWS.CredentialService;
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
    public class ContratosConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_in = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES3900[] contratos_out = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES3900[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4070[] cosechas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4070[] { };
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

                    var request = new Z_MPMF_MOAOP_CONTRATOS()
                    {
                        PE_PEND_ENTREGA = "",
                        PE_PROVEEDOR = proveedor,
                        T_CONTRATOS_IN =  contratos_in,
                        T_CONTRATOS_OUT = contratos_out,
                        T_COSECHA_IN = cosechas,
                        T_FECHA_OPERACION_IN = fechasSAPArray,
                        T_MATERIAL_IN = materiales,
                        T_VENDEDOR_IN = vendedores
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CONTRATOS request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_CONTRATOS(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CONTRATOS response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response.PS_ERROR, response.T_CONTRATOS_IN , response.T_CONTRATOS_OUT, response.T_COSECHA_IN, response.T_FECHA_OPERACION_IN, response.T_MATERIAL_IN, response.T_VENDEDOR_IN);

                }
                else
                {
                    ContratosWebServiceMOA.ZMPES4060[] contratos_in = new ContratosWebServiceMOA.ZMPES4060[] { };
                    ContratosWebServiceMOA.ZMPES3900[] contratos_out = new ContratosWebServiceMOA.ZMPES3900[] { };
                    ContratosWebServiceMOA.ZMPES4070[] cosechas = new ContratosWebServiceMOA.ZMPES4070[] { };
                    ContratosWebServiceMOA.ZMPES4090[] materiales = new ContratosWebServiceMOA.ZMPES4090[] { };
                    ContratosWebServiceMOA.ZMPES4080[] vendedores = new ContratosWebServiceMOA.ZMPES4080[] { };
                    List<ContratosWebServiceMOA.ZMPES4100> fechasSAP = new List<ContratosWebServiceMOA.ZMPES4100>() { };
                    foreach (FechaWS fecha in fechas)
                    {
                        fechasSAP.Add(new ContratosWebServiceMOA.ZMPES4100()
                        {
                            FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                            FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                        });
                    }
                    ContratosWebServiceMOA.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    SI_MPMF_MOAOP_CONTRATOSClient service = new SI_MPMF_MOAOP_CONTRATOSClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string error = service.SI_MPMF_MOAOP_CONTRATOS("", proveedor, ref contratos_in, ref contratos_out, ref cosechas, ref fechasSAPArray, ref materiales, ref vendedores);
                    return Map(error, contratos_in, contratos_out, cosechas, fechasSAPArray, materiales, vendedores);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(string error, ContratosWebServiceMOA.ZMPES4060[] contratos_in, ContratosWebServiceMOA.ZMPES3900[] contratos_out, ContratosWebServiceMOA.ZMPES4070[] cosechas, ContratosWebServiceMOA.ZMPES4100[] fechasSAPArray, ContratosWebServiceMOA.ZMPES4090[] materiales, ContratosWebServiceMOA.ZMPES4080[] vendedores)
        {
            ContratosWSMOAResponse result = new ContratosWSMOAResponse();

            result.error.codigo = error;

            foreach (ContratosWebServiceMOA.ZMPES4060 contrato in contratos_in)
            {
                result.contratos.Add(contrato.CONTRATO);
            }

            foreach (ContratosWebServiceMOA.ZMPES3900 contratoInfo in contratos_out)
            {
                result.contratosInfo.Add(new ContratoCumplidoView()
                {
                    nroContrato = contratoInfo.NRO_CONTRATO,
                    contrvend = contratoInfo.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contratoInfo.FECHA_CONCERT),
                    fechaDate = SAPFormatter.GetDateTime(contratoInfo.FECHA_CONCERT),
                    idVendedor = contratoInfo.ID_VENDEDOR,
                    vendedor = contratoInfo.VENDEDOR,
                    cantKilos = contratoInfo.CANT_KILOS,
                    cantKilosString = SAPFormatter.FormatearCantidad(contratoInfo.CANT_KILOS, contratoInfo.UNIME_PACTADO),
                    precio = SAPFormatter.FormatearMonto(contratoInfo.PRECIO, contratoInfo.MONEDA),
                    lugarDescarga = contratoInfo.LUGAR_DESCARGA,
                    cosecha = contratoInfo.COSECHA,
                    material = contratoInfo.MATERIAL,
                    tipoContrato = TipoContrato.GetTipoContrato(contratoInfo.CLASE_DOC, contratoInfo.PAGO_DIFERIDO, contratoInfo.DOL_EXPRESS, contratoInfo.DOL_CORREDOR, contratoInfo.PAGO_DIF_ARP, contratoInfo.CANJE, contratoInfo.CESION, contratoInfo.COMPENSACION, ""),
                    estadoBoleto = contratoInfo.ESTADO_BOLETO,
                    aplicacionesString = SAPFormatter.FormatearCantidad(contratoInfo.APLICACIONES, contratoInfo.UNIME_ENTREGADO),
                    aplicaciones = contratoInfo.APLICACIONES,
                    liquidado = contratoInfo.LIQUIDADO,
                    liquidadoString = SAPFormatter.FormatearCantidad(contratoInfo.LIQUIDADO, contratoInfo.UNIME_LIQUIDADO),
                    estado = contratoInfo.ESTADO,
                    pagoDiferido = contratoInfo.PAGO_DIFERIDO,
                    dolarExpress = contratoInfo.DOL_EXPRESS,
                    dolarCorredor = contratoInfo.DOL_CORREDOR,
                    fechaLimite = contratoInfo.FECHA_LIMITE,
                    pagoDiferidoArp = contratoInfo.PAGO_DIF_ARP,
                    diasDiferim = contratoInfo.DIAS_DIFERIM,
                    canje = contratoInfo.CANJE,
                    cesion = contratoInfo.CESION,
                    compensacion = contratoInfo.COMPENSACION
                });
            }

            foreach (ContratosWebServiceMOA.ZMPES4070 cosecha in cosechas)
            {
                result.cosechas.Add(cosecha.COSECHA);
            }

            foreach (ContratosWebServiceMOA.ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (ContratosWebServiceMOA.ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (ContratosWebServiceMOA.ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }
            
            return result;
        }
        protected virtual object MapSinPI(string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_in, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES3900[] contratos_out, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4070[] cosechas, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] materiales, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] vendedores)
        {
            ContratosWSMOAResponse result = new ContratosWSMOAResponse();

            result.error.codigo = error;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060 contrato in contratos_in)
            {
                result.contratos.Add(contrato.CONTRATO);
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES3900 contratoInfo in contratos_out)
            {
                result.contratosInfo.Add(new ContratoCumplidoView()
                {
                    nroContrato = contratoInfo.NRO_CONTRATO,
                    contrvend = contratoInfo.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contratoInfo.FECHA_CONCERT),
                    fechaDate = SAPFormatter.GetDateTime(contratoInfo.FECHA_CONCERT),
                    idVendedor = contratoInfo.ID_VENDEDOR,
                    vendedor = contratoInfo.VENDEDOR,
                    cantKilos = contratoInfo.CANT_KILOS,
                    cantKilosString = SAPFormatter.FormatearCantidad(contratoInfo.CANT_KILOS, contratoInfo.UNIME_PACTADO),
                    precio = SAPFormatter.FormatearMonto(contratoInfo.PRECIO, contratoInfo.MONEDA),
                    lugarDescarga = contratoInfo.LUGAR_DESCARGA,
                    cosecha = contratoInfo.COSECHA,
                    material = contratoInfo.MATERIAL,
                    tipoContrato = TipoContrato.GetTipoContrato(contratoInfo.CLASE_DOC, contratoInfo.PAGO_DIFERIDO, contratoInfo.DOL_EXPRESS, contratoInfo.DOL_CORREDOR, contratoInfo.PAGO_DIF_ARP, contratoInfo.CANJE, contratoInfo.CESION, contratoInfo.COMPENSACION, ""),
                    estadoBoleto = contratoInfo.ESTADO_BOLETO,
                    aplicacionesString = SAPFormatter.FormatearCantidad(contratoInfo.APLICACIONES, contratoInfo.UNIME_ENTREGADO),
                    aplicaciones = contratoInfo.APLICACIONES,
                    liquidado = contratoInfo.LIQUIDADO,
                    liquidadoString = SAPFormatter.FormatearCantidad(contratoInfo.LIQUIDADO, contratoInfo.UNIME_LIQUIDADO),
                    estado = contratoInfo.ESTADO,
                    pagoDiferido = contratoInfo.PAGO_DIFERIDO,
                    dolarExpress = contratoInfo.DOL_EXPRESS,
                    dolarCorredor = contratoInfo.DOL_CORREDOR,
                    fechaLimite = contratoInfo.FECHA_LIMITE,
                    pagoDiferidoArp = contratoInfo.PAGO_DIF_ARP,
                    diasDiferim = contratoInfo.DIAS_DIFERIM,
                    canje = contratoInfo.CANJE,
                    cesion = contratoInfo.CESION,
                    compensacion = contratoInfo.COMPENSACION
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4070 cosecha in cosechas)
            {
                result.cosechas.Add(cosecha.COSECHA);
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

    public class ContratosExcelConsumerMOA : ContratosConsumerMOA
    {
        protected override object Map(string error, ContratosWebServiceMOA.ZMPES4060[] contratos_in, ContratosWebServiceMOA.ZMPES3900[] contratos_out, ContratosWebServiceMOA.ZMPES4070[] cosechas, ContratosWebServiceMOA.ZMPES4100[] fechasSAPArray, ContratosWebServiceMOA.ZMPES4090[] materiales, ContratosWebServiceMOA.ZMPES4080[] vendedores)
        {
            ContratosExcelWSMOAResponse result = new ContratosExcelWSMOAResponse();

            result.error.codigo = error;

            foreach (ContratosWebServiceMOA.ZMPES4060 contrato in contratos_in)
            {
                result.contratos.Add(contrato.CONTRATO);
            }

            foreach (ContratosWebServiceMOA.ZMPES3900 contratoInfo in contratos_out)
            {
                result.contratosInfo.Add(new ContratoCumplido()
                {
                    nroContrato = contratoInfo.NRO_CONTRATO,
                    contrvend = contratoInfo.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contratoInfo.FECHA_CONCERT),
                    idVendedor = contratoInfo.ID_VENDEDOR,
                    vendedor = contratoInfo.VENDEDOR,
                    unidadCantKilos = contratoInfo.UNIME_PACTADO,
                    cantKilos = contratoInfo.CANT_KILOS, 
                    moneda =  contratoInfo.MONEDA,
                    precio = contratoInfo.PRECIO,
                    lugarDescarga = contratoInfo.LUGAR_DESCARGA,
                    cosecha = contratoInfo.COSECHA,
                    material = contratoInfo.MATERIAL,
                    estadoBoleto = contratoInfo.ESTADO_BOLETO,
                    unidadAplicaciones = contratoInfo.UNIME_ENTREGADO,
                    aplicaciones = contratoInfo.APLICACIONES,
                    unidadLiquidado = contratoInfo.UNIME_LIQUIDADO,
                    liquidado = contratoInfo.LIQUIDADO,
                    estado = contratoInfo.ESTADO
                });
            }

            foreach (ContratosWebServiceMOA.ZMPES4070 cosecha in cosechas)
            {
                result.cosechas.Add(cosecha.COSECHA);
            }

            foreach (ContratosWebServiceMOA.ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (ContratosWebServiceMOA.ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (ContratosWebServiceMOA.ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }

            return result;
        }
        protected override object MapSinPI(string error, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060[] contratos_in, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES3900[] contratos_out, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4070[] cosechas, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4090[] materiales, WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4080[] vendedores)
        {
            ContratosExcelWSMOAResponse result = new ContratosExcelWSMOAResponse();

            result.error.codigo = error;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4060 contrato in contratos_in)
            {
                result.contratos.Add(contrato.CONTRATO);
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES3900 contratoInfo in contratos_out)
            {
                result.contratosInfo.Add(new ContratoCumplido()
                {
                    nroContrato = contratoInfo.NRO_CONTRATO,
                    contrvend = contratoInfo.CONTRVEND,
                    fecha = SAPFormatter.FormatearFecha(contratoInfo.FECHA_CONCERT),
                    idVendedor = contratoInfo.ID_VENDEDOR,
                    vendedor = contratoInfo.VENDEDOR,
                    unidadCantKilos = contratoInfo.UNIME_PACTADO,
                    cantKilos = contratoInfo.CANT_KILOS,
                    moneda = contratoInfo.MONEDA,
                    precio = contratoInfo.PRECIO,
                    lugarDescarga = contratoInfo.LUGAR_DESCARGA,
                    cosecha = contratoInfo.COSECHA,
                    material = contratoInfo.MATERIAL,
                    estadoBoleto = contratoInfo.ESTADO_BOLETO,
                    unidadAplicaciones = contratoInfo.UNIME_ENTREGADO,
                    aplicaciones = contratoInfo.APLICACIONES,
                    unidadLiquidado = contratoInfo.UNIME_LIQUIDADO,
                    liquidado = contratoInfo.LIQUIDADO,
                    estado = contratoInfo.ESTADO
                });
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4070 cosecha in cosechas)
            {
                result.cosechas.Add(cosecha.COSECHA);
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
