using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PagoDetalleWebServiceMOA;
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
    public class PagoDetalleConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        public object request(string proveedor, string pago)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] cabeceras = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5320[] salidas = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5320[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5320[] salidasExcelDetalle = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES5320[] { };
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4510[] vendedores = new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4510[] { };

                    var request = new Z_MPMF_MOAOP_DETALLE_PAGO()
                    {
                        PE_PAGO = pago,
                        PE_PROVEEDOR = proveedor,
                        T_SALIDA = salidas,
                        T_CABECERA = cabeceras,
                        T_VENDEDOR = vendedores,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLE_PAGO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_DETALLE_PAGO(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_DETALLE_PAGO response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response,pago,proveedor);
                }
                else
                {
                    SI_MPMF_MOAOP_DETALLE_PAGOClient service = new SI_MPMF_MOAOP_DETALLE_PAGOClient();
                    PagoDetalleWebServiceMOA.ZMPES4360[] cabeceras = new PagoDetalleWebServiceMOA.ZMPES4360[] { };
                    PagoDetalleWebServiceMOA.ZMPES5320[] salidas = new PagoDetalleWebServiceMOA.ZMPES5320[] { };
                    PagoDetalleWebServiceMOA.ZMPES5320[] salidasExcelDetalle = new PagoDetalleWebServiceMOA.ZMPES5320[] { };
                    PagoDetalleWebServiceMOA.ZMPES4510[] vendedores = new PagoDetalleWebServiceMOA.ZMPES4510[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    service.SI_MPMF_MOAOP_DETALLE_PAGO(pago, proveedor, ref cabeceras, ref salidas, ref vendedores);
                    return Map(cabeceras, salidas, vendedores, pago, proveedor);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        protected virtual object Map(PagoDetalleWebServiceMOA.ZMPES4360[] cabeceras, PagoDetalleWebServiceMOA.ZMPES5320[] salidas, PagoDetalleWebServiceMOA.ZMPES4510[] vendedores, string pago, string proveedor)
        {
            string moneda = "$";

            List<string> contratos = new List<string>() { };

            PagoDetalleWSMOAResponse result = new PagoDetalleWSMOAResponse();

            result.pago = pago;

            foreach (PagoDetalleWebServiceMOA.ZMPES4360 cabecera in cabeceras)
            {
                result.cabeceras.Add(new CabeceraView()
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA),
                    brutoString = SAPFormatter.FormatearMonto(cabecera.BRUTO, cabecera.MONEDA),
                    idPago = cabecera.ID_PAGO,
                    ivaString = SAPFormatter.FormatearMonto(cabecera.IVA, cabecera.MONEDA),
                    netoString = SAPFormatter.FormatearMonto((cabecera.BRUTO + cabecera.IVA - cabecera.RETENCIONES), cabecera.MONEDA),
                    retencionesString = SAPFormatter.FormatearMonto(cabecera.RETENCIONES, cabecera.MONEDA)
                });

                moneda = cabecera.MONEDA;
            }

            foreach (PagoDetalleWebServiceMOA.ZMPES4510 vendedor in vendedores)
            {
                result.vendedores.Add(new Vendedor()
                {
                    contrato = vendedor.CONTRATO,
                    idVendedor = vendedor.ID_VENDEDOR,
                    vendedor = vendedor.VENDEDOR
                });

                contratos.Add(vendedor.CONTRATO);
            }

            result.salidas = salidas
                .Where(x => contratos.Contains(x.CONTRATO))
                .GroupBy(x => new { x.CONTRATO, x.TIPO } )
                .Select(x => new Salida
                {
                    contrato = x.Key.CONTRATO,
                    tipo = x.Key.TIPO,
                    registros = x.Select(e => new SalidaElement()
                    {
                        bruto = e.BRUTO,
                        brutoString = SAPFormatter.FormatearMonto(e.BRUTO, e.MONEDA),
                        caract = e.CARACT,
                        cbuIva = e.CBU_IVA,
                        impGanancias = e.IMP_GANANCIAS,
                        impGananciasString = SAPFormatter.FormatearMonto(e.IMP_GANANCIAS, e.MONEDA),
                        impIibb = e.IMP_IIBB,
                        impIibbString = SAPFormatter.FormatearMonto(e.IMP_IIBB, e.MONEDA),
                        iva = e.IVA,
                        ivaString = SAPFormatter.FormatearMonto(e.IVA, e.MONEDA),
                        nroLegal = e.NRO_LEGAL,
                        retIva = e.RET_IVA,
                        retIvaString = SAPFormatter.FormatearMonto(e.RET_IVA, e.MONEDA),
                        remanenteIva = e.IVA + e.RET_IVA,
                        remanenteIvaString = SAPFormatter.FormatearMonto(e.IVA + e.RET_IVA, e.MONEDA),
                        ivaCorredor = e.IVA_CORR,
                        ivaCorredorString = SAPFormatter.FormatearMonto(e.IVA_CORR, e.MONEDA),
                        ivaVendedor = e.IVA_VEND,
                        ivaVendedorString = SAPFormatter.FormatearMonto(e.IVA_VEND, e.MONEDA),
                        mercCorredor = e.MERC_CORR,
                        mercCorredorString = SAPFormatter.FormatearMonto(e.MERC_CORR, e.MONEDA),
                        mercVendedor = e.MERC_VEND,
                        mercVendedorString = SAPFormatter.FormatearMonto(e.MERC_VEND, e.MONEDA)
                    }).ToList(),
                    total = x.Sum(s => s.BRUTO + s.IMP_GANANCIAS + s.IMP_IIBB + s.IVA + s.RET_IVA),
                    totalString = SAPFormatter.FormatearMonto(x.Sum(s => s.BRUTO + s.IMP_GANANCIAS + s.IMP_IIBB + s.IVA + s.RET_IVA), x.First().MONEDA),
                    subtotalIvaCorredor = x.Sum(s => s.IVA_CORR),
                    subtotalIvaVendedor = x.Sum(s => s.IVA_VEND),
                    subtotalMercCorredor = x.Sum(s => s.MERC_CORR),
                    subtotalMercVendedor = x.Sum(s => s.MERC_VEND),
                    subtotalIvaCorredorString = SAPFormatter.FormatearMonto(x.Sum(s => s.IVA_CORR), x.First().MONEDA),
                    subtotalIvaVendedorString = SAPFormatter.FormatearMonto(x.Sum(s => s.IVA_VEND), x.First().MONEDA),
                    subtotalMercCorredorString = SAPFormatter.FormatearMonto(x.Sum(s => s.MERC_CORR), x.First().MONEDA),
                    subtotalMercVendedorString = SAPFormatter.FormatearMonto(x.Sum(s => s.MERC_VEND), x.First().MONEDA)
                }).ToList();


            result.subtotalIvaCorredor = result.salidas.Sum(s => s.subtotalIvaCorredor);
            result.subtotalIvaVendedor = result.salidas.Sum(s => s.subtotalIvaVendedor);
            result.subtotalIvaCorredorString = SAPFormatter.FormatearMonto(result.subtotalIvaCorredor, moneda);
            result.subtotalIvaVendedorString = SAPFormatter.FormatearMonto(result.subtotalIvaVendedor, moneda);
            result.subtotalMercCorredor = result.salidas.Sum(s => s.subtotalMercCorredor);
            result.subtotalMercVendedor = result.salidas.Sum(s => s.subtotalMercVendedor);
            result.subtotalMercCorredorString = SAPFormatter.FormatearMonto(result.subtotalMercCorredor, moneda);
            result.subtotalMercVendedorString = SAPFormatter.FormatearMonto(result.subtotalMercVendedor, moneda);

            return result;
        }

        protected virtual object MapSinPI(Z_MPMF_MOAOP_DETALLE_PAGOResponse response, string pago, string proveedor)
        {
            string moneda = "$";

            List<string> contratos = new List<string>() { };

            PagoDetalleWSMOAResponse result = new PagoDetalleWSMOAResponse();

            result.pago = pago;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360 cabecera in response.T_CABECERA)
            {
                result.cabeceras.Add(new CabeceraView()
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA),
                    brutoString = SAPFormatter.FormatearMonto(cabecera.BRUTO, cabecera.MONEDA),
                    idPago = cabecera.ID_PAGO,
                    ivaString = SAPFormatter.FormatearMonto(cabecera.IVA, cabecera.MONEDA),
                    netoString = SAPFormatter.FormatearMonto((cabecera.BRUTO + cabecera.IVA - cabecera.RETENCIONES), cabecera.MONEDA),
                    retencionesString = SAPFormatter.FormatearMonto(cabecera.RETENCIONES, cabecera.MONEDA)
                });

                moneda = cabecera.MONEDA;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4510 vendedor in response.T_VENDEDOR)
            {
                result.vendedores.Add(new Vendedor()
                {
                    contrato = vendedor.CONTRATO,
                    idVendedor = vendedor.ID_VENDEDOR,
                    vendedor = vendedor.VENDEDOR
                });

                contratos.Add(vendedor.CONTRATO);
            }

            result.salidas = response.T_SALIDA
                .Where(x => contratos.Contains(x.CONTRATO))
                .GroupBy(x => new { x.CONTRATO, x.TIPO })
                .Select(x => new Salida
                {
                    contrato = x.Key.CONTRATO,
                    tipo = x.Key.TIPO,
                    registros = x.Select(e => new SalidaElement()
                    {
                        bruto = e.BRUTO,
                        brutoString = SAPFormatter.FormatearMonto(e.BRUTO, e.MONEDA),
                        caract = e.CARACT,
                        cbuIva = e.CBU_IVA,
                        impGanancias = e.IMP_GANANCIAS,
                        impGananciasString = SAPFormatter.FormatearMonto(e.IMP_GANANCIAS, e.MONEDA),
                        impIibb = e.IMP_IIBB,
                        impIibbString = SAPFormatter.FormatearMonto(e.IMP_IIBB, e.MONEDA),
                        iva = e.IVA,
                        ivaString = SAPFormatter.FormatearMonto(e.IVA, e.MONEDA),
                        nroLegal = e.NRO_LEGAL,
                        retIva = e.RET_IVA,
                        retIvaString = SAPFormatter.FormatearMonto(e.RET_IVA, e.MONEDA),
                        remanenteIva = e.IVA + e.RET_IVA,
                        remanenteIvaString = SAPFormatter.FormatearMonto(e.IVA + e.RET_IVA, e.MONEDA),
                        ivaCorredor = e.IVA_CORR,
                        ivaCorredorString = SAPFormatter.FormatearMonto(e.IVA_CORR, e.MONEDA),
                        ivaVendedor = e.IVA_VEND,
                        ivaVendedorString = SAPFormatter.FormatearMonto(e.IVA_VEND, e.MONEDA),
                        mercCorredor = e.MERC_CORR,
                        mercCorredorString = SAPFormatter.FormatearMonto(e.MERC_CORR, e.MONEDA),
                        mercVendedor = e.MERC_VEND,
                        mercVendedorString = SAPFormatter.FormatearMonto(e.MERC_VEND, e.MONEDA)
                    }).ToList(),
                    total = x.Sum(s => s.BRUTO + s.IMP_GANANCIAS + s.IMP_IIBB + s.IVA + s.RET_IVA),
                    totalString = SAPFormatter.FormatearMonto(x.Sum(s => s.BRUTO + s.IMP_GANANCIAS + s.IMP_IIBB + s.IVA + s.RET_IVA), x.First().MONEDA),
                    subtotalIvaCorredor = x.Sum(s => s.IVA_CORR),
                    subtotalIvaVendedor = x.Sum(s => s.IVA_VEND),
                    subtotalMercCorredor = x.Sum(s => s.MERC_CORR),
                    subtotalMercVendedor = x.Sum(s => s.MERC_VEND),
                    subtotalIvaCorredorString = SAPFormatter.FormatearMonto(x.Sum(s => s.IVA_CORR), x.First().MONEDA),
                    subtotalIvaVendedorString = SAPFormatter.FormatearMonto(x.Sum(s => s.IVA_VEND), x.First().MONEDA),
                    subtotalMercCorredorString = SAPFormatter.FormatearMonto(x.Sum(s => s.MERC_CORR), x.First().MONEDA),
                    subtotalMercVendedorString = SAPFormatter.FormatearMonto(x.Sum(s => s.MERC_VEND), x.First().MONEDA)
                }).ToList();


            result.subtotalIvaCorredor = result.salidas.Sum(s => s.subtotalIvaCorredor);
            result.subtotalIvaVendedor = result.salidas.Sum(s => s.subtotalIvaVendedor);
            result.subtotalIvaCorredorString = SAPFormatter.FormatearMonto(result.subtotalIvaCorredor, moneda);
            result.subtotalIvaVendedorString = SAPFormatter.FormatearMonto(result.subtotalIvaVendedor, moneda);
            result.subtotalMercCorredor = result.salidas.Sum(s => s.subtotalMercCorredor);
            result.subtotalMercVendedor = result.salidas.Sum(s => s.subtotalMercVendedor);
            result.subtotalMercCorredorString = SAPFormatter.FormatearMonto(result.subtotalMercCorredor, moneda);
            result.subtotalMercVendedorString = SAPFormatter.FormatearMonto(result.subtotalMercVendedor, moneda);

            return result;
        }


    }

    public class PagoDetalleExcelConsumerMOA : PagoDetalleConsumerMOA
    {
        protected override object Map(PagoDetalleWebServiceMOA.ZMPES4360[] cabeceras, PagoDetalleWebServiceMOA.ZMPES5320[] salidas, PagoDetalleWebServiceMOA.ZMPES4510[] vendedores, string pago, string proveedor)
        {
            string moneda = "$";

            List<string> contratos = new List<string>() { };

            PagoDetalleExcelWSMOAResponse result = new PagoDetalleExcelWSMOAResponse();

            result.pago = pago;

            foreach (PagoDetalleWebServiceMOA.ZMPES4360 cabecera in cabeceras)
            {
                result.cabeceras.Add(new Cabecera()
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA),
                    moneda = cabecera.MONEDA,
                    bruto = cabecera.BRUTO,
                    idPago = cabecera.ID_PAGO,
                    iva = cabecera.IVA,
                    neto = cabecera.NETO,
                    retenciones = cabecera.RETENCIONES
                });

                moneda = cabecera.MONEDA;
            }

            foreach (PagoDetalleWebServiceMOA.ZMPES4510 vendedor in vendedores)
            {
                result.vendedores.Add(new Vendedor()
                {
                    contrato = vendedor.CONTRATO,
                    idVendedor = vendedor.ID_VENDEDOR,
                    vendedor = vendedor.VENDEDOR
                });

                
                contratos.Add(vendedor.CONTRATO);
                
            }

            result.salidasExcelDetalle = salidas
                .Where(x => contratos.Contains(x.CONTRATO))
                .Select(x => new SalidaExcelDetalle
                {
                    contrato = x.CONTRATO,
                    tipo = x.TIPO,
                    moneda = x.MONEDA,
                    total = x.BRUTO + x.IMP_IIBB + x.IMP_GANANCIAS + x.RET_IVA + x.IVA,
                    totalIva = x.IVA + x.RET_IVA,
                    bruto = x.BRUTO, 
                    caract = x.CARACT,
                    cbuIva = "'" + x.CBU_IVA,
                    impGanancias =x.IMP_GANANCIAS,
                    impIibb = x.IMP_IIBB,
                    iva = x.IVA,
                    nroLegal = "'" + x.NRO_LEGAL,
                    retIva = x.RET_IVA,
                    remanenteIva = x.IVA + x.RET_IVA
                }).ToList();


            foreach (PagoDetalleWebServiceMOA.ZMPES4510 vendedor in vendedores)
            {
                result.vendedores.Add(new Vendedor()
                {
                    contrato = vendedor.CONTRATO,
                    idVendedor = vendedor.ID_VENDEDOR,
                    vendedor = vendedor.VENDEDOR
                });
            }



            return result;
        }

        protected override object MapSinPI(Z_MPMF_MOAOP_DETALLE_PAGOResponse response, string pago, string proveedor)
        {
            string moneda = "$";

            List<string> contratos = new List<string>() { };

            PagoDetalleExcelWSMOAResponse result = new PagoDetalleExcelWSMOAResponse();

            result.pago = pago;

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4360 cabecera in response.T_CABECERA)
            {
                result.cabeceras.Add(new Cabecera()
                {
                    fecha = SAPFormatter.FormatearFecha(cabecera.FECHA),
                    moneda = cabecera.MONEDA,
                    bruto = cabecera.BRUTO,
                    idPago = cabecera.ID_PAGO,
                    iva = cabecera.IVA,
                    neto = cabecera.NETO,
                    retenciones = cabecera.RETENCIONES
                });

                moneda = cabecera.MONEDA;
            }

            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4510 vendedor in response.T_VENDEDOR)
            {
                result.vendedores.Add(new Vendedor()
                {
                    contrato = vendedor.CONTRATO,
                    idVendedor = vendedor.ID_VENDEDOR,
                    vendedor = vendedor.VENDEDOR
                });


                contratos.Add(vendedor.CONTRATO);

            }

            result.salidasExcelDetalle = response.T_SALIDA
                .Where(x => contratos.Contains(x.CONTRATO))
                .Select(x => new SalidaExcelDetalle
                {
                    contrato = x.CONTRATO,
                    tipo = x.TIPO,
                    moneda = x.MONEDA,
                    total = x.BRUTO + x.IMP_IIBB + x.IMP_GANANCIAS + x.RET_IVA + x.IVA,
                    totalIva = x.IVA + x.RET_IVA,
                    bruto = x.BRUTO,
                    caract = x.CARACT,
                    cbuIva = "'" + x.CBU_IVA,
                    impGanancias = x.IMP_GANANCIAS,
                    impIibb = x.IMP_IIBB,
                    iva = x.IVA,
                    nroLegal = "'" + x.NRO_LEGAL,
                    retIva = x.RET_IVA,
                    remanenteIva = x.IVA + x.RET_IVA
                }).ToList();


            foreach (WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4510 vendedor in response.T_VENDEDOR)
            {
                result.vendedores.Add(new Vendedor()
                {
                    contrato = vendedor.CONTRATO,
                    idVendedor = vendedor.ID_VENDEDOR,
                    vendedor = vendedor.VENDEDOR
                });
            }



            return result;
        }



    }
}
