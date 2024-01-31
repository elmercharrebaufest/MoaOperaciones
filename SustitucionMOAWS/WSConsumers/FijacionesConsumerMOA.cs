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
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.FijacionesWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class FijacionesConsumerMOA
    {
        SI_MPMF_MOAOP_FIJACIONESClient service = new SI_MPMF_MOAOP_FIJACIONESClient();

        public object request(string proveedor, List<string> contratos, List<FechaWS> fechas, string tipo_op)
        {
            try
            {
                ZMPES4200[] fijaciones_out = new ZMPES4200[] { };
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
                List<ZMPES4060> contratos_inSAP = new List<ZMPES4060>() { };
                foreach (string contrato in contratos)
                {
                    contratos_inSAP.Add(new ZMPES4060()
                    {
                        CONTRATO = contrato
                    });
                }
                ZMPES4060[] contratos_inSAPArray = contratos_inSAP.ToArray();
                //ZmprfcGolFijaciones requestInfo = new ZmprfcGolFijaciones { PeProveedor = proveedor, TFechaOperacionIn = fechasSAP.ToArray(), PeTipoOp = "FIJ" };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_FIJACIONES(proveedor, tipo_op, ref contratos_inSAPArray, ref fechasSAPArray, ref fijaciones_out, ref materiales, ref vendedores);
                return map(error, contratos_inSAPArray, fechasSAPArray, fijaciones_out, materiales, vendedores);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object map(ZMPES4910 error, ZMPES4060[] contratos_in, ZMPES4100[] fechasSAPArray, ZMPES4200[] fijaciones_out, ZMPES4090[] materiales, ZMPES4080[] vendedores)
        {
            ContratosNoCumplidosWSMOAResponse result = new ContratosNoCumplidosWSMOAResponse();

            if (error != null) {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4200 contrato in fijaciones_out)
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

    public class FijacionesExcelConsumerMOA : FijacionesConsumerMOA
    {
        protected override object map(ZMPES4910 error, ZMPES4060[] contratos_in, ZMPES4100[] fechasSAPArray, ZMPES4200[] fijaciones_out, ZMPES4090[] materiales, ZMPES4080[] vendedores)
        {
            ContratosNoCumplidosExcelWSMOAResponse result = new ContratosNoCumplidosExcelWSMOAResponse();

            if (error != null)
            {
                result.error.descripcion = error.DESCRIPCION;
                result.error.codigo = error.CODIGO;
                result.error.tipo = error.TIPO;
            }

            foreach (ZMPES4200 contrato in fijaciones_out)
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
