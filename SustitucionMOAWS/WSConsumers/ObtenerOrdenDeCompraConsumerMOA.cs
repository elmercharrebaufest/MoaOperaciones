using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenDeCompraConsumerMOA : IObtenerOrdenDeCompraConsumerMOA
    {
        BAPI_PO_GETDETAIL1PortTypeClient service;
        //private const string COMP_CODE = "MOA";
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
                ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY);

                return mapOrdenDeCompraSAPDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY);
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
                ObtenerOcSap(nroOC, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY);

                return mapAdjudicacionDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ObtenerOcSap(
            string nroOC, out BAPIMEPOITEM[] POITEM, out BAPIRET2[] RETURN, out BAPIMEPOHEADER POHEADER, out BAPIEIKP result,
           out BAPIMEPOTEXTHEADER[] POTEXTHEADER, out BAPIMEPOTEXT[] POTEXTITEM, out BAPIESLLC[] POSERVICES, out BAPIMEPOSCHEDULE[] POSCHEDULE,
           out BAPIMEPOADDRDELIVERY[] POADDRDELIVERY
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

            BAPIMEPOACCOUNT[] POACCOUNT = new BAPIMEPOACCOUNT[] { };
            POADDRDELIVERY = new BAPIMEPOADDRDELIVERY[] { };
            BAPIMEPOCOND[] POCOND = new BAPIMEPOCOND[] { };
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
            BAPIESKLC[] POSRVACCESSVALUES = new BAPIESKLC[] { };
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


        private OrdenDeCompraSAPDto mapOrdenDeCompraSAPDto(BAPIEIKP result, BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN, BAPIMEPOITEM[] POITEM, BAPIMEPOTEXTHEADER[] POTEXTHEADER, BAPIMEPOTEXT[] POTEXTITEM, BAPIESLLC[] POSERVICES, BAPIMEPOSCHEDULE[] POSCHEDULE, BAPIMEPOADDRDELIVERY[] POADDRDELIVERY)
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

            resultado.Cabecera = new OrdenDeCompraSAPCabecera
            {
                OrdenDeCompra = POHEADER.PO_NUMBER,
                CodigoProveedor = POHEADER.VENDOR,
                Moneda = POHEADER.CURRENCY,
                FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                FechaCreacionString = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString()
            };

            foreach (var pos in POITEM.ToList())
            {
                resultado.Cabecera.MontoTotal += pos.NET_PRICE * pos.QUANTITY;
                resultado.Posiciones.Add(new OrdenDeCompraSAPPosicion
                {
                    Indice = pos.PO_ITEM
                });
            }
            return resultado;
        }


        private AdjudicacionDto mapAdjudicacionDto(BAPIEIKP result, BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN, BAPIMEPOITEM[] POITEM, BAPIMEPOTEXTHEADER[] POTEXTHEADER,
        BAPIMEPOTEXT[] POTEXTITEM, BAPIESLLC[] POSERVICES, BAPIMEPOSCHEDULE[] POSCHEDULE, BAPIMEPOADDRDELIVERY[] POADDRDELIVERY)
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
            adjudicacion.PrecioFinal = POITEM.Sum(a => a.QUANTITY * a.NET_PRICE);
            adjudicacion.TextoDeCabecera = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F01").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDeEntrega = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F05").Select(a => a.TEXT_LINE));
            adjudicacion.CondicionesDePago = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F07").Select(a => a.TEXT_LINE));
            adjudicacion.Garantias = string.Join(" ", POTEXTHEADER.Where(a => a.TEXT_ID == "F08").Select(a => a.TEXT_LINE));
            adjudicacion.Moneda_Id = monedas.First(a => a.Codigo == POHEADER.CURRENCY).Id;
            adjudicacion.MonedaDescripcion = monedas.First(a => a.Codigo == POHEADER.CURRENCY).Descripcion;
            adjudicacion.FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);


            adjudicacion.AdjudicacionPosiciones = new List<AdjudicacionPosicionDto>();

            foreach (var posicion in POITEM)
            {

                

                AdjudicacionPosicionDto pos = new AdjudicacionPosicionDto();

                pos.SolpPosicion_Id = 0;
                pos.Id = 0; //P
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
                pos.UnidadDescripcion = unidades.FirstOrDefault(a => a.Codigo == posicion.PO_UNIT)?.Descripcion ?? "";
                pos.MonedaDescripcion = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.Descripcion;
                pos.MonedaCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                pos.PrecioTotal = posicion.QUANTITY * posicion.NET_PRICE;
                pos.CentroComprasCodigo = posicion.PLANT;

                try
                {
                    var fecha = POSCHEDULE.First(a => a.PO_ITEM == posicion.PO_ITEM).DELIVERY_DATE;
                    pos.FechaEntregaServicio = DateTime.ParseExact(fecha, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
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
                        sub.UnidadComprasDescripcion = unidades.FirstOrDefault(a => a.Codigo == subpos.BASE_UOM)?.Descripcion ?? "";
                        sub.MonedaCotizacionDescripcion = POHEADER.CURRENCY;
                        sub.MonedaCotizacionCodigo = monedas.FirstOrDefault(a => a.Codigo == POHEADER.CURRENCY)?.CodigoSap;
                        sub.PrecioTotalSubPosicion = subpos.NET_VALUE;
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
                        pos.SubposicionesCompras.Add(sub);
                    }
                }

                adjudicacion.AdjudicacionPosiciones.Add(pos);
            }

            return adjudicacion;
        }






        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public DetalleOrdenDeCompraDto ObtenerDetalleDeOrdenDeCompra(string numeroDeOrdenCompra)
        {
            try
            {
                BAPIMEPOITEM[] POITEM;
                BAPIRET2[] RETURN;
                BAPIMEPOHEADER POHEADER;//
                BAPIEIKP result;
                BAPIMEPOTEXTHEADER[] POTEXTHEADER;
                BAPIMEPOTEXT[] POTEXTITEM;
                BAPIESLLC[] POSERVICES;
                BAPIMEPOSCHEDULE[] POSCHEDULE;
                BAPIMEPOADDRDELIVERY[] POADDRDELIVERY;
                BAPIEKBE[] POHISTORY;
                ObtenerDetalleDeOrdenDeCompraSap(numeroDeOrdenCompra, out POITEM, out RETURN, out POHEADER, out result, out POTEXTHEADER, out POTEXTITEM, out POSERVICES, out POSCHEDULE, out POADDRDELIVERY, out POHISTORY);

                return map(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POHISTORY);

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private DetalleOrdenDeCompraDto map(BAPIEIKP result, BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN, BAPIMEPOITEM[] POITEM, BAPIMEPOTEXTHEADER[] POTEXTHEADER,
        BAPIMEPOTEXT[] POTEXTITEM, BAPIESLLC[] POSERVICES, BAPIMEPOSCHEDULE[] POSCHEDULE, BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, BAPIEKBE[] POHISTORY)
        {
            DetalleOrdenDeCompraDto detalleOrdenDeCompra = new DetalleOrdenDeCompraDto();
            var servicios = new List<ServicioSolp>();

           // Obtiene datos de la cabecera de una OC
            detalleOrdenDeCompra.NumeroOrdenDeCompra = POHEADER.PO_NUMBER;
            detalleOrdenDeCompra.Proveedor = POHEADER.VENDOR;
            detalleOrdenDeCompra.MontoTotal = Math.Round(POITEM.Sum(a => a.QUANTITY * a.NET_PRICE), 4);
            detalleOrdenDeCompra.FechaCreacion = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString();
            detalleOrdenDeCompra.UsuarioCreador = POHEADER.CREATED_BY;
            detalleOrdenDeCompra.Posiciones = new List<PosicionDto>();

            /// Por cada Posicion ...
            foreach (var posicion in POITEM)
            {
                PosicionDto pos = new PosicionDto();
                var ListaEntradasServicio = new List<string>(); // Para obtener las entradas de servicio de cada posicion

              
                pos.Id = int.Parse(posicion.PCKG_NO);
                pos.NumeroPosicion = long.Parse(posicion.PO_ITEM);
                pos.CodigoMaterial = posicion.MATERIAL?.TrimStart('0');                
                pos.Descripcion = posicion.SHORT_TEXT;               
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = Math.Round(posicion.NET_PRICE, 4);
                pos.PrecioTotal = Math.Round(posicion.QUANTITY * posicion.NET_PRICE, 4);
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.UM = posicion.PO_UNIT;
                pos.GrupoArticulos = posicion.MATL_GROUP;
                pos.Centro = posicion.PLANT;
                pos.Almacen = posicion.STGE_LOC;
                pos.NumeroSolp = posicion.PREQ_NO;
                pos.Contrato = posicion.AGREEMENT;
                pos.Solicitante = posicion.PREQ_NAME;
                //pos.MonedaId = posicion.CURRENCY;

                pos.Items = ObtenerItemsdelaPosicion(POSERVICES, POHISTORY, pos.Id) ;
                         
                detalleOrdenDeCompra.Posiciones.Add(pos);
            }

            return detalleOrdenDeCompra;
        }


        private List<ItemDto> ObtenerItemsdelaPosicion(BAPIESLLC[] pOSERVICES, BAPIEKBE[] pOHISTORY, int idPosicion)
        {
            List<ItemDto> itemsDeLaPosicion = new List<ItemDto>();
            string idPosicionString = idPosicion.ToString("D10");
            var itemsValidos = pOSERVICES.Where(x => x.PCKG_NO == idPosicionString).FirstOrDefault();

            if (itemsValidos == null)
                return itemsDeLaPosicion;


            foreach (var item in pOSERVICES.Where(x => x.PCKG_NO == itemsValidos.SUBPCKG_NO))
            {
                ItemDto itemDto = new ItemDto();

                itemDto.Id = item.PCKG_NO;
                itemDto.NumeroLinea = int.Parse(item.EXT_LINE);
                itemDto.Descripcion = item.SHORT_TEXT;
                itemDto.Cantidad = item.QUANTITY;
                itemDto.PosicionId = Convert.ToInt32(item.PCKG_NO);
                itemDto.PrecioBruto = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0,4);
                itemDto.ServicioNumero = long.Parse(item.SERVICE ==""?"0":item.SERVICE);
                itemDto.UM = item.BASE_UOM;
                itemDto.Importe = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0, 4);

                itemDto.EntradasServicio = ObtenerEntradasDeServicioDelItem(pOSERVICES, pOHISTORY, itemDto.Id);

                itemsDeLaPosicion.Add(itemDto);
            }

            return itemsDeLaPosicion;
        }

        private List<EntradaServicioDto> ObtenerEntradasDeServicioDelItem(BAPIESLLC[] pOSERVICES, BAPIEKBE[] pOHISTORY, string idDelItem)
        {
            List<EntradaServicioDto> EntradasServicioDelItem = new List<EntradaServicioDto>();
            var entradasDeServicioPotenciales = pOHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D").ToList();


            foreach (var entradaServicioPotencial in entradasDeServicioPotenciales)
            {
                EntradaServicioDto entradaServicioDto = new EntradaServicioDto();
                EntradaServicioDto entradaServicioSAP = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicio(entradaServicioPotencial.MAT_DOC);
                var elementosEntradaServicio = entradaServicioSAP.Items;

                foreach (ItemEntradaServicioDto elemento in elementosEntradaServicio)
                {

                    if (elemento.ItemNumero == idDelItem)
                    {
                        entradaServicioDto.Id = int.Parse(elemento.Id);
                        entradaServicioDto.TextoBreve = elemento.Descripcion;
                        entradaServicioDto.Cantidad = elemento.Cantidad;

                        EntradasServicioDelItem.Add(entradaServicioDto);
                    }
                }
            }

            return EntradasServicioDelItem;

        }

        private void ObtenerDetalleDeOrdenDeCompraSap(
            string nroOC, out BAPIMEPOITEM[] POITEM, out BAPIRET2[] RETURN, out BAPIMEPOHEADER POHEADER, out BAPIEIKP result,
           out BAPIMEPOTEXTHEADER[] POTEXTHEADER, out BAPIMEPOTEXT[] POTEXTITEM, out BAPIESLLC[] POSERVICES, out BAPIMEPOSCHEDULE[] POSCHEDULE,
           out BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, out BAPIEKBE[] POHISTORY
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

            BAPIMEPOACCOUNT[] POACCOUNT = new BAPIMEPOACCOUNT[] { };
            POADDRDELIVERY = new BAPIMEPOADDRDELIVERY[] { };
            BAPIMEPOCOND[] POCOND = new BAPIMEPOCOND[] { };
            POITEM = new BAPIMEPOITEM[] { };
            POTEXTHEADER = new BAPIMEPOTEXTHEADER[] { };
            POTEXTITEM = new BAPIMEPOTEXT[] { };
            RETURN = new BAPIRET2[] { };
            POSERVICES = new BAPIESLLC[] { };
            POHEADER = new BAPIMEPOHEADER { };
            POHISTORY = new BAPIEKBE[] { };
            BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = new BAPI_INVOICE_PLAN_HEADER[] { };
            BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = new BAPIMEDCM_ALLVERSIONS[] { };
            BAPIPAREX[] EXTENSIONOUT = new BAPIPAREX[] { };
            BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = new BAPI_INVOICE_PLAN_ITEM[] { };
            BAPIMEPOCOMPONENT[] POCOMPONENTS = new BAPIMEPOCOMPONENT[] { };
            BAPIMEPOCONDHEADER[] POCONDHEADER = new BAPIMEPOCONDHEADER[] { };
            BAPIEKES[] POCONFIRMATION = new BAPIEKES[] { };
            BAPIESUCC[] POCONTRACTLIMITS = new BAPIESUCC[] { };
            BAPIEIPO[] POEXPIMPITEM = new BAPIEIPO[] { };
            //BAPIEKBE[] POHISTORY = new BAPIEKBE[] { };
            BAPIEKBE_MA[] POHISTORY_MA = new BAPIEKBE_MA[] { };
            BAPIEKBES[] POHISTORY_TOTALS = new BAPIEKBES[] { };
            BAPIESUHC[] POLIMITS = new BAPIESUHC[] { };
            BAPIEKKOP[] POPARTNER = new BAPIEKKOP[] { };
            POSCHEDULE = new BAPIMEPOSCHEDULE[] { };
            BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new BAPIMEPOSHIPPEXP[] { };
            BAPIESKLC[] POSRVACCESSVALUES = new BAPIESKLC[] { };
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

    }
}
