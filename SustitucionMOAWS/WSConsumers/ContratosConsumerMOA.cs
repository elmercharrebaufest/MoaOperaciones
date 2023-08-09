using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Contrato;
using SustitucionMOAWS.ContratosWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class ContratosConsumerMOA
    {
        SI_MPMF_MOAOP_CONTRATOSClient service = new SI_MPMF_MOAOP_CONTRATOSClient();

        public object request(string proveedor, List<FechaWS> fechas)
        {
            try
            {
                ZMPES4060[] contratos_in = new ZMPES4060[] { };
                ZMPES3900[] contratos_out = new ZMPES3900[] { };
                ZMPES4070[] cosechas = new ZMPES4070[] { };
                ZMPES4090[] materiales = new ZMPES4090[] { };
                ZMPES4080[] vendedores = new ZMPES4080[] { };
                List<ZMPES4100> fechasSAP = new List<ZMPES4100>() { };
                foreach (FechaWS fecha in fechas)
                {
                    fechasSAP.Add(new ZMPES4100()
                    {
                        FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                        FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                    });
                }
                ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                string error = service.SI_MPMF_MOAOP_CONTRATOS("", proveedor, ref contratos_in, ref contratos_out, ref cosechas, ref fechasSAPArray, ref materiales, ref vendedores);
                return map(error, contratos_in, contratos_out, cosechas, fechasSAPArray, materiales, vendedores);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(string error, ZMPES4060[] contratos_in, ZMPES3900[] contratos_out, ZMPES4070[] cosechas, ZMPES4100[] fechasSAPArray, ZMPES4090[] materiales, ZMPES4080[] vendedores)
        {
            ContratosWSMOAResponse result = new ContratosWSMOAResponse();

            result.error.codigo = error;

            foreach (ZMPES4060 contrato in contratos_in)
            {
                result.contratos.Add(contrato.CONTRATO);
            }

            foreach (ZMPES3900 contratoInfo in contratos_out)
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

            foreach (ZMPES4070 cosecha in cosechas)
            {
                result.cosechas.Add(cosecha.COSECHA);
            }

            foreach (ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }
            
            return result;
        }
    }

    public class ContratosExcelConsumerMOA : ContratosConsumerMOA
    {
        protected override object map(string error, ZMPES4060[] contratos_in, ZMPES3900[] contratos_out, ZMPES4070[] cosechas, ZMPES4100[] fechasSAPArray, ZMPES4090[] materiales, ZMPES4080[] vendedores)
        {
            ContratosExcelWSMOAResponse result = new ContratosExcelWSMOAResponse();

            result.error.codigo = error;

            foreach (ZMPES4060 contrato in contratos_in)
            {
                result.contratos.Add(contrato.CONTRATO);
            }

            foreach (ZMPES3900 contratoInfo in contratos_out)
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

            foreach (ZMPES4070 cosecha in cosechas)
            {
                result.cosechas.Add(cosecha.COSECHA);
            }

            foreach (ZMPES4100 fechaOperacion in fechasSAPArray)
            {
                result.fechas.Add(new FechaStringWS()
                {
                    fechaInicio = fechaOperacion.FECHA_OP,
                    fechaFin = fechaOperacion.FECHA_OP_HASTA
                }
                );
            }

            foreach (ZMPES4090 material in materiales)
            {
                result.materiales.Add(material.MATERIAL);
            }

            foreach (ZMPES4080 vendedor in vendedores)
            {
                result.vendedores.Add(vendedor.VENDEDOR);
            }

            return result;
        }
    }
}
