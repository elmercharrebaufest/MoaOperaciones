using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenDeCompraConsumerMOA : IObtenerOrdenDeCompraConsumerMOA
    {
        BAPI_PO_GETDETAIL1PortTypeClient service;
        private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;

        public ObtenerOrdenDeCompraConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_PO_GETDETAIL1&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
            service = new BAPI_PO_GETDETAIL1PortTypeClient(SAPCredential.CrearSapLongBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            try
            {
                BAPIMEPOITEM[] POITEM;
                BAPIRET2[] RETURN;
                BAPIMEPOHEADER POHEADER;
                BAPIEIKP result;
                BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                BAPIMEPOTEXT[] POTEXTITEM;
                BAPIESLLC[] POSERVICES;
                BAPIMEPOSCHEDULE[] POSCHEDULE;
                BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                BAPIMEPOCOND[] POCOND;
                BAPIMEPOACCOUNT[] POACCOUNT;
                BAPIESKLC[] POSRVACCESSVALUES;
                ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY, out POCOND, out POACCOUNT, out POSRVACCESSVALUES);

                return mapOrdenDeCompraSAPDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POCOND, POSRVACCESSVALUES);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public AdjudicacionDto ObtenerOrdenDeCompraAdjudicacion(string nroOC)
        {
            try
            {
                BAPIMEPOITEM[] POITEM;
                BAPIRET2[] RETURN;
                BAPIMEPOHEADER POHEADER;
                BAPIEIKP result;
                BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                BAPIMEPOTEXT[] POTEXTITEM;
                BAPIESLLC[] POSERVICES;
                BAPIMEPOSCHEDULE[] POSCHEDULE;
                BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                BAPIMEPOCOND[] POCOND;
                BAPIMEPOACCOUNT[] POACCOUNT;
                BAPIESKLC[] POSRVACCESSVALUES;
                ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY, out POCOND, out POACCOUNT, out POSRVACCESSVALUES);

                return mapAdjudicacionDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POSRVACCESSVALUES);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ResultBAPI_PO_GETDETAIL1 ObtenerOrdenDeCompraRFC(string nroOC)
        {
            try
            {
                BAPIMEPOITEM[] POITEM;
                BAPIRET2[] RETURN;
                BAPIMEPOHEADER POHEADER;
                BAPIEIKP result;
                BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                BAPIMEPOTEXT[] POTEXTITEM;
                BAPIESLLC[] POSERVICES;
                BAPIMEPOSCHEDULE[] POSCHEDULE;
                BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                BAPIMEPOCOND[] POCOND;
                BAPIMEPOACCOUNT[] POACCOUNT;
                BAPIESKLC[] POSRVACCESSVALUES;
                ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY, out POCOND, out POACCOUNT, out POSRVACCESSVALUES);

                return new ResultBAPI_PO_GETDETAIL1(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POCOND, POACCOUNT, POSRVACCESSVALUES);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ObtenerOcSap(
            string nroOC, out BAPIMEPOITEM[] POITEM, out BAPIRET2[] RETURN, out BAPIMEPOHEADER POHEADER, out BAPIEIKP result,
           out BAPIMEPOTEXTHEADER[] POTEXTHEADER, out BAPIMEPOTEXT[] POTEXTITEM, out BAPIESLLC[] POSERVICES, out BAPIMEPOSCHEDULE[] POSCHEDULE,
           out BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, out BAPIMEPOCOND[] POCOND, out BAPIMEPOACCOUNT[] POACCOUNT, out BAPIESKLC[] POSRVACCESSVALUES
            )
        {
            string ACCOUNT_ASSIGNMENT = "X";
            string DELIVERY_ADDRESS = "X";
            string HEADER_TEXT = "X";
            string INVOICEPLAN = "X";
            string ITEM_TEXT = "X";
            string PURCHASEORDER = nroOC;
            string SERIALNUMBERS = "X";
            string SERVICES = "X";
            string VERSION = "X";

            POACCOUNT = new BAPIMEPOACCOUNT[] { };
            POADDRDELIVERY = new BAPIMEPOADDRDELIVERY[] { };
            POCOND = new BAPIMEPOCOND[] { };
            POITEM = new BAPIMEPOITEM[] { };
            POTEXTHEADER = new BAPIMEPOTEXTHEADER[] { };
            POTEXTITEM = new BAPIMEPOTEXT[] { };
            RETURN = new BAPIRET2[] { };
            POSERVICES = new BAPIESLLC[] { };
            POHEADER = new BAPIMEPOHEADER { };
            BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = new BAPI_INVOICE_PLAN_HEADER[] { };
            BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = new BAPIMEDCM_ALLVERSIONS[] { };
            BAPIPAREX[] EXTENSIONOUT = new BAPIPAREX[] { };
            BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = new BAPI_INVOICE_PLAN_ITEM[] { };
            BAPIMEPOCOMPONENT[] POCOMPONENTS = new BAPIMEPOCOMPONENT[] { };
            BAPIMEPOCONDHEADER[] POCONDHEADER = new BAPIMEPOCONDHEADER[] { };
            BAPIEKES[] POCONFIRMATION = new BAPIEKES[] { };
            BAPIESUCC[] POCONTRACTLIMITS = new BAPIESUCC[] { };
            BAPIEIPO[] POEXPIMPITEM = new BAPIEIPO[] { };
            BAPIEKBE[] POHISTORY = new BAPIEKBE[] { };
            BAPIEKBE_MA[] POHISTORY_MA = new BAPIEKBE_MA[] { };
            BAPIEKBES[] POHISTORY_TOTALS = new BAPIEKBES[] { };
            BAPIESUHC[] POLIMITS = new BAPIESUHC[] { };
            BAPIEKKOP[] POPARTNER = new BAPIEKKOP[] { };
            POSCHEDULE = new BAPIMEPOSCHEDULE[] { };
            BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new BAPIMEPOSHIPPEXP[] { };
            POSRVACCESSVALUES = new BAPIESKLC[] { };
            BAPIMEPOSERIALNO[] SERIALNUMBER = new BAPIMEPOSERIALNO[] { };

            result = service.BAPI_PO_GETDETAIL1(ACCOUNT_ASSIGNMENT,
                DELIVERY_ADDRESS,
                HEADER_TEXT,
                INVOICEPLAN,
                ITEM_TEXT,
                PURCHASEORDER,
                SERIALNUMBERS,
                SERVICES,
                VERSION,
                ref ALLVERSIONS,
                ref EXTENSIONOUT,
                ref INVPLANHEADER,
                ref INVPLANITEM,
                ref POACCOUNT,
                ref POADDRDELIVERY,
                ref POCOMPONENTS,
                ref POCOND,
                ref POCONDHEADER,
                ref POCONFIRMATION,
                ref POCONTRACTLIMITS,
                ref POEXPIMPITEM,
                ref POHISTORY,
                ref POHISTORY_MA,
                ref POHISTORY_TOTALS,
                ref POITEM,
                ref POLIMITS,
                ref POPARTNER,
                ref POSCHEDULE,
                ref POSERVICES,
                ref POSHIPPINGEXP,
                ref POSRVACCESSVALUES,
                ref POTEXTHEADER,
                ref POTEXTITEM,
                ref RETURN,
                ref SERIALNUMBER,
                out POHEADER);
        }


        private OrdenDeCompraSAPDto mapOrdenDeCompraSAPDto(BAPIEIKP result, BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN, BAPIMEPOITEM[] POITEM, BAPIMEPOTEXTHEADER[] POTEXTHEADER,
        BAPIMEPOTEXT[] POTEXTITEM, BAPIESLLC[] POSERVICES, BAPIMEPOSCHEDULE[] POSCHEDULE, BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, BAPIMEPOCOND[] POCOND, BAPIESKLC[] POSRVACCESSVALUES)
        {
            OrdenDeCompraSAPDto resultado = new OrdenDeCompraSAPDto();

            if (RETURN != null)
            {
                if (RETURN.Length > 0)
                {
                    resultado.Error = new ErrorOC
                    {
                        Mensaje = RETURN[0].MESSAGE,
                        Tipo = RETURN[0].TYPE
                    };
                }
            }

            if (resultado.Error == null)
            {
                resultado.Cabecera = new OrdenDeCompraSAPCabecera
                {
                    OrdenDeCompra = POHEADER.PO_NUMBER,
                    CodigoProveedor = POHEADER.VENDOR,
                    UsuarioComprasSAP = POHEADER.CREATED_BY,
                    Moneda = POHEADER.CURRENCY,
                    FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    FechaCreacionString = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString(),
                    OrganizacionDeComprasCodigo = POHEADER.PURCH_ORG
                };

                foreach (var pos in POITEM.ToList())
                {
                    resultado.Cabecera.MontoTotal += pos.NET_PRICE * pos.QUANTITY;
                    var dato = POADDRDELIVERY.Where(x => x.PO_ITEM == pos.PO_ITEM).SingleOrDefault();
                    resultado.Posiciones.Add(new OrdenDeCompraSAPPosicion
                    {
                        Indice = pos.PO_ITEM,
                        RegistroInfo = pos.INFO_REC,
                        NroSolp = pos.PREQ_NO,
                        DireccionDeEntrega = new OrdenDeCompraSAPPosicionDireccionDeEntrega
                        {
                            RegionSap = dato.REGION
                        }
                    });
                }
            }


            return resultado;
        }


        private AdjudicacionDto mapAdjudicacionDto(BAPIEIKP result, BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN, BAPIMEPOITEM[] POITEM, BAPIMEPOTEXTHEADER[] POTEXTHEADER,
        BAPIMEPOTEXT[] POTEXTITEM, BAPIESLLC[] POSERVICES, BAPIMEPOSCHEDULE[] POSCHEDULE, BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, BAPIESKLC[] POSRVACCESSVALUES)
        {
            AdjudicacionDto adjudicacion = new AdjudicacionDto();
            var codigoMateriales = POITEM.Select(a => a.MATERIAL).ToList();
            var materiales = repositorio.Listar<MaterialSolp>(x => codigoMateriales.Contains(x.CodigoSap));

            if (RETURN == null)
            {
                return null;
            }

            var monedas = repositorio.Listar<TablaSap>(a => a.Tabla == "Moneda");
            var unidades = repositorio.Listar<TablaSap>(a => a.Tabla == "Unidad");
            var unidadesSAP = repositorio.Listar<UnidadMedidaSap>();

            var servicios = new List<ServicioSolp>();
            if (POITEM.First().ITEM_CAT == "9")
            {
                var codigoServicios = POSERVICES.Select(a => a.SERVICE).ToList();
                servicios = repositorio.Listar<ServicioSolp>(x => codigoServicios.Contains(x.Codigo));
            }

            adjudicacion.Id = 0;
            adjudicacion.TipoPosicionCodigo = POITEM.First().ITEM_CAT == "9" ? "SERVICIOS" : "MATERIALES";
            adjudicacion.NumeroOrdenDeCompra = POHEADER.PO_NUMBER;
            adjudicacion.Proveedor = POHEADER.VENDOR;
            adjudicacion.Centro = POADDRDELIVERY.FirstOrDefault()?.NAME;
            adjudicacion.CalleEntrega = POADDRDELIVERY.FirstOrDefault()?.STREET;
            adjudicacion.CodigoPostal = POADDRDELIVERY.FirstOrDefault()?.POSTL_COD1;
            adjudicacion.PrecioFinal = POITEM.Where(a => a.DELETE_IND != "L").Sum(a => a.QUANTITY * a.NET_PRICE);
            adjudicacion.TextoDeCabecera = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F01").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDeEntrega = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F05").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDePago = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F07").Select(a => a.TEXT_LINE));
            adjudicacion.Garantias = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F08").Select(a => a.TEXT_LINE));
            adjudicacion.Moneda_Id = monedas.First(a => a.Codigo == POHEADER.CURRENCY).Id;
            adjudicacion.MonedaDescripcion = monedas.First(a => a.Codigo == POHEADER.CURRENCY).Descripcion;
            adjudicacion.FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);


            adjudicacion.AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>();

            foreach (var posicion in POITEM.Where(a=>a.DELETE_IND != "L"))
            {
                AdjudicacionPosicionDto pos = new AdjudicacionPosicionDto();

                pos.SolpPosicion_Id = 0;
                pos.Id = 0;
                pos.MaterialComprasCodigo = posicion.MATERIAL?.TrimStart('0');
                pos.MaterialComprasDescripcion = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.Descripcion : "";
                pos.MaterialTextoAmpliado = !string.IsNullOrEmpty(posicion.MATERIAL) ? materiales.Where(x => x.CodigoSap == posicion.MATERIAL).FirstOrDefault()?.TextoAmpliado : "";
                pos.Indice = int.Parse(posicion.PO_ITEM);
                pos.Tarea = posicion.SHORT_TEXT;
                pos.TextoSuministro = string.Join(" ", POTEXTITEM.Where(a => a.PO_ITEM == posicion.PO_ITEM && a.TEXT_ID == "F02").Select(a => a.TEXT_LINE));
                pos.Modelo = "";// posicion.Posicion.Modelo;
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = posicion.NET_PRICE;
                pos.MonedaId = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.Id;
                var unidadSap = unidadesSAP.FirstOrDefault(a => a.UM == posicion.PO_UNIT);
                var unidad = unidades.FirstOrDefault(a => a.Codigo == unidadSap?.Comercial);
                pos.UnidadDescripcion = unidad?.Descripcion ?? "";
                pos.MonedaDescripcion = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.Descripcion;
                pos.MonedaCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                pos.PrecioTotal = posicion.QUANTITY * posicion.NET_PRICE; 
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.Eliminado = posicion.DELETE_IND == "L";

                try
                {
                    var fecha = POSCHEDULE.First(a => a.PO_ITEM == posicion.PO_ITEM).DELIVERY_DATE;
                    pos.FechaEntregaServicio = DateTime.ParseExact(fecha, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    pos.FechaEntregaServicioFormateado = fecha;
                }
                catch
                {
                    pos.FechaEntregaServicio = null;
                }

                pos.PlazoDeOferta = Decimal.ToInt32(posicion.PLAN_DEL);
                if (POITEM.First().ITEM_CAT == "9")
                {
                    pos.SubposicionesCompras = new List<SolpSubposicionDto>();
                    var SUBPCKG_NO = POSERVICES.First(a => a.PCKG_NO == posicion.PCKG_NO).SUBPCKG_NO;
                    foreach (var subpos in POSERVICES.Where(a => a.PCKG_NO == SUBPCKG_NO))
                    {
                        SolpSubposicionDto sub = new SolpSubposicionDto();
                        sub.Numero = int.Parse(subpos.LINE_NO);
                        sub.Tarea = subpos.SHORT_TEXT;
                        sub.CodigoSolp = servicios.FirstOrDefault(a => a.Codigo == subpos.SERVICE)?.CodigoSap ?? 0;
                        sub.Cantidad = subpos.QUANTITY;
                        sub.PrecioBruto = subpos.NET_VALUE / subpos.QUANTITY;
                        var unidadSapsp = unidadesSAP.FirstOrDefault(a => a.UM == subpos.BASE_UOM);
                        var unidadsp = unidades.FirstOrDefault(a => a.Codigo == unidadSapsp?.Comercial);
                        sub.UnidadComprasDescripcion = unidadsp?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = POHEADER.CURRENCY;
                        sub.MonedaCotizacionCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                        sub.PrecioTotalSubPosicion = subpos.NET_VALUE;
                        sub.Eliminado = subpos.DELETE_IND == "L";
                        pos.SubposicionesCompras.Add(sub);
                    }
                }
                else
                {
                    foreach (var subpos in POSERVICES.Where(a => a.PCKG_NO == posicion.PCKG_NO).Skip(1))
                    {
                        SolpSubposicionDto sub = new SolpSubposicionDto();
                        sub.Numero = int.Parse(subpos.LINE_NO);
                        sub.Tarea = subpos.SHORT_TEXT;
                        sub.CodigoSolp = servicios.FirstOrDefault(a => a.Codigo == subpos.SERVICE)?.CodigoSap ?? 0;
                        sub.Cantidad = subpos.QUANTITY;
                        sub.PrecioBruto = subpos.NET_VALUE / subpos.PRICE_UNIT;
                        sub.UnidadComprasDescripcion = unidades.FirstOrDefault(a => a.Codigo == subpos.BASE_UOM)?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = POHEADER.CURRENCY;
                        sub.PrecioTotalSubPosicion = sub.Cantidad ?? 0 * sub.PrecioBruto ?? 0;
                        sub.Eliminado = subpos.DELETE_IND == "L";
                        pos.SubposicionesCompras.Add(sub);
                    }
                }

                adjudicacion.AdjudicacionPosiciones.Add(pos);
            }

            return adjudicacion;
        }
    
    }

    public class ResultBAPI_PO_GETDETAIL1
    {
        public BAPIEIKP Result { get; }
        public BAPIMEPOHEADER POHEADER { get; }
        public BAPIRET2[] RETURN { get; }
        public BAPIMEPOITEM[] POITEM { get; }
        public BAPIMEPOTEXTHEADER[] POTEXTHEADER { get; }
        public BAPIMEPOTEXT[] POTEXTITEM { get; }
        public BAPIESLLC[] POSERVICES { get; }
        public BAPIMEPOSCHEDULE[] POSCHEDULE { get; }
        public BAPIMEPOADDRDELIVERY[] POADDRDELIVERY { get; }
        public BAPIMEPOCOND[] POCOND { get; }
        public BAPIMEPOACCOUNT[] POACCOUNT { get; set; }
        public BAPIESKLC[] POSRVACCESSVALUES { get; set; }

        public ResultBAPI_PO_GETDETAIL1(BAPIEIKP result, BAPIMEPOHEADER pOHEADER, BAPIRET2[] rETURN, BAPIMEPOITEM[] pOITEM, BAPIMEPOTEXTHEADER[] pOTEXTHEADER, BAPIMEPOTEXT[] pOTEXTITEM, BAPIESLLC[] pOSERVICES, BAPIMEPOSCHEDULE[] pOSCHEDULE, BAPIMEPOADDRDELIVERY[] pOADDRDELIVERY, BAPIMEPOCOND[] pOCOND, BAPIMEPOACCOUNT[] pOACCOUNT, BAPIESKLC[] pOSRVACCESSVALUES)
        {
            Result = result;
            POHEADER = pOHEADER;
            RETURN = rETURN;
            POITEM = pOITEM;
            POTEXTHEADER = pOTEXTHEADER;
            POTEXTITEM = pOTEXTITEM;
            POSERVICES = pOSERVICES;
            POSCHEDULE = pOSCHEDULE;
            POADDRDELIVERY = pOADDRDELIVERY;
            POCOND = pOCOND;
            POACCOUNT = pOACCOUNT;
            POSRVACCESSVALUES = pOSRVACCESSVALUES;
        }

        public override bool Equals(object obj)
        {
            return obj is ResultBAPI_PO_GETDETAIL1 other &&
                   EqualityComparer<BAPIEIKP>.Default.Equals(Result, other.Result) &&
                   EqualityComparer<BAPIMEPOHEADER>.Default.Equals(POHEADER, other.POHEADER) &&
                   EqualityComparer<BAPIRET2[]>.Default.Equals(RETURN, other.RETURN) &&
                   EqualityComparer<BAPIMEPOITEM[]>.Default.Equals(POITEM, other.POITEM) &&
                   EqualityComparer<BAPIMEPOTEXTHEADER[]>.Default.Equals(POTEXTHEADER, other.POTEXTHEADER) &&
                   EqualityComparer<BAPIMEPOTEXT[]>.Default.Equals(POTEXTITEM, other.POTEXTITEM) &&
                   EqualityComparer<BAPIESLLC[]>.Default.Equals(POSERVICES, other.POSERVICES) &&
                   EqualityComparer<BAPIMEPOSCHEDULE[]>.Default.Equals(POSCHEDULE, other.POSCHEDULE) &&
                   EqualityComparer<BAPIMEPOADDRDELIVERY[]>.Default.Equals(POADDRDELIVERY, other.POADDRDELIVERY);
        }

        public override int GetHashCode()
        {
            int hashCode = 996742197;
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIEIKP>.Default.GetHashCode(Result);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIMEPOHEADER>.Default.GetHashCode(POHEADER);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIRET2[]>.Default.GetHashCode(RETURN);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIMEPOITEM[]>.Default.GetHashCode(POITEM);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIMEPOTEXTHEADER[]>.Default.GetHashCode(POTEXTHEADER);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIMEPOTEXT[]>.Default.GetHashCode(POTEXTITEM);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIESLLC[]>.Default.GetHashCode(POSERVICES);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIMEPOSCHEDULE[]>.Default.GetHashCode(POSCHEDULE);
            hashCode = hashCode * -1521134295 + EqualityComparer<BAPIMEPOADDRDELIVERY[]>.Default.GetHashCode(POADDRDELIVERY);
            return hashCode;
        }
    }
}