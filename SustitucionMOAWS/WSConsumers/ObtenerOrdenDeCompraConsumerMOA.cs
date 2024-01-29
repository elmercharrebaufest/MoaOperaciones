using SustitucionMOAFotmatter;
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
using System.Runtime.Remoting.Messaging;
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
        /// <summary>
        /// MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
        /// </summary>
        private string dateTimeFormat = "dd/MM/yyyy";

        /// <summary>
        /// //MMSN-491 - Ponerle separador de miles a la columna “Monto Total”. - Separador de miles ( , ) coma - Separador decimal ( . ) punto
        /// </summary>
        private string currencyFormat = "#,##0.00";

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

        /// <summary>
        /// Obtiene Detalle de una Orden de Compra
        /// Posición, item o línea, Entradas de Servicio si las tuviera, Historial de Entradas de Servicio.
        /// </summary>
        /// <param name="numeroDeOrdenCompra"></param>
        /// <returns></returns>
        public DetalleOrdenDeCompraDto ObtenerDetalleDeOrdenDeCompra(string numeroDeOrdenCompra, List<TablaSap> centro, List<TablaSap> almacen)
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

                return map(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY, POHISTORY, centro, almacen);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Mapea Detalle de una Orden de Compra
        /// </summary>
        /// <param name="result"></param>
        /// <param name="POHEADER"></param>
        /// <param name="RETURN"></param>
        /// <param name="POITEM"></param>
        /// <param name="POTEXTHEADER"></param>
        /// <param name="POTEXTITEM"></param>
        /// <param name="POSERVICES"></param>
        /// <param name="POSCHEDULE"></param>
        /// <param name="POADDRDELIVERY"></param>
        /// <param name="POHISTORY"></param>
        /// <returns></returns>
        private DetalleOrdenDeCompraDto map(BAPIEIKP result, BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN, BAPIMEPOITEM[] POITEM, BAPIMEPOTEXTHEADER[] POTEXTHEADER,
         BAPIMEPOTEXT[] POTEXTITEM, BAPIESLLC[] POSERVICES, BAPIMEPOSCHEDULE[] POSCHEDULE, BAPIMEPOADDRDELIVERY[] POADDRDELIVERY, BAPIEKBE[] POHISTORY, List<TablaSap> centros, List<TablaSap> almacenes)
        {
            DetalleOrdenDeCompraDto detalleOrdenDeCompra = new DetalleOrdenDeCompraDto();
            var servicios = new List<ServicioSolp>();

           // Obtiene datos de la cabecera de una OC
            detalleOrdenDeCompra.NumeroOrdenDeCompra = POHEADER.PO_NUMBER;
            detalleOrdenDeCompra.Proveedor = POHEADER.VENDOR;
            //detalleOrdenDeCompra.NombreProveedor = POHEADER.
            detalleOrdenDeCompra.MontoTotal = Math.Round(POITEM.Sum(a => a.QUANTITY * a.NET_PRICE), 4);
            //MMSN-491 - Ponerle separador de miles a la columna “Monto Total”. - Separador de miles ( , ) coma - Separador decimal ( . ) punto           
            detalleOrdenDeCompra.MontoTotalString = detalleOrdenDeCompra.MontoTotal.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);

            ////MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
            DateTime toFormat = DateTime.ParseExact(POHEADER.CREAT_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            detalleOrdenDeCompra.FechaCreacion = toFormat.ToString(dateTimeFormat);

            detalleOrdenDeCompra.UsuarioCreador = POHEADER.CREATED_BY;
            detalleOrdenDeCompra.Posiciones = new List<PosicionDto>();


            //var centros = repositorio.Listar<TablaSap>(a => a.Tabla == "Centro");
            //var almacenes = repositorio.Listar<TablaSap>(a => a.Tabla == "Almacen");




            /// Por cada Posicion ...
            foreach (var posicion in POITEM)
            {
                PosicionDto pos = new PosicionDto();
                //var ListaEntradasServicio = new List<string>(); // Para obtener las entradas de servicio de cada posicion
              
                pos.Id = int.Parse(posicion.PCKG_NO);
                pos.NumeroPosicion = long.Parse(posicion.PO_ITEM);
                pos.CodigoMaterial = posicion.MATERIAL?.TrimStart('0');                
                pos.Descripcion = posicion.SHORT_TEXT;               
                pos.Cantidad = posicion.QUANTITY;
                pos.PrecioUnidad = Math.Round(posicion.NET_PRICE, 4);

                if(pos.PrecioUnidad != null)
                {
                    decimal notNullValue = (decimal)pos.PrecioUnidad;
                    pos.PrecioUnidadString = notNullValue.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                pos.PrecioTotal = Math.Round(posicion.QUANTITY * posicion.NET_PRICE, 4);
                pos.CentroComprasCodigo = posicion.PLANT;
                pos.UM = posicion.PO_UNIT;
                pos.GrupoArticulos = posicion.MATL_GROUP;
                TablaSap centro = centros.FirstOrDefault(a => a.Codigo == posicion.PLANT);
                TablaSap Almacen = almacenes.FirstOrDefault(a => a.Codigo == posicion.STGE_LOC);

                if (centro == null)
                {
                    pos.Centro = posicion.PLANT + "- ";
                };

                if (centro != null)
                {
                    pos.Centro = posicion.PLANT + "-" + centro.Descripcion;
                };

                if (Almacen == null)
                {
                    pos.Almacen = posicion.STGE_LOC + "- ";
                };

                if (Almacen != null)
                {
                    pos.Almacen = posicion.STGE_LOC + "-" + Almacen.Descripcion;
                };


                //pos.Centro = posicion.PLANT;
                //pos.Almacen = posicion.STGE_LOC;

                pos.NumeroSolp = posicion.PREQ_NO;
                pos.Contrato = posicion.AGREEMENT;
                pos.Solicitante = posicion.PREQ_NAME;
                //pos.MonedaId = posicion.CURRENCY;
                pos.MonedaDescripcion = POHEADER.CURRENCY_ISO;

                pos.NroOrdenCompra = POHEADER.PO_NUMBER;

                pos.Items = ObtenerItemsdelaPosicion(POSERVICES, POHISTORY, POHEADER, pos);

                detalleOrdenDeCompra.Posiciones.Add(pos);
            }

            return detalleOrdenDeCompra;
        }

        /// <summary>
        /// Load Items of a position
        /// </summary>
        /// <param name="pOSERVICES"></param>
        /// <param name="pOHISTORY"></param>
        /// <param name="POCOND">MMSN-460 - Added as mentioned in v.1.8 - HU02-Compras-MVP1 - Detalle de OCs - Posiciones - Solicitante</param>
        /// <param name="idPosicion"></param>
        /// <returns></returns>
        private List<ItemDto> ObtenerItemsdelaPosicion(BAPIESLLC[] pOSERVICES, BAPIEKBE[] pOHISTORY, BAPIMEPOHEADER POHEADER, PosicionDto Posicion)
        {
            List<ItemDto> itemsDeLaPosicion = new List<ItemDto>();
            string idPosicionString = Posicion.Id.ToString("D10");
            var itemsValidos = pOSERVICES.Where(x => x.PCKG_NO == idPosicionString).FirstOrDefault();

            if (itemsValidos == null)
                return itemsDeLaPosicion;

            var items = pOSERVICES.Where(x => x.PCKG_NO == itemsValidos.SUBPCKG_NO);
            foreach (var item in items)
            {
                ItemDto itemDto = new ItemDto();


                itemDto.Id = item.PCKG_NO;
                itemDto.LINE_NO = item.LINE_NO;
                itemDto.NumeroLinea = int.Parse(item.EXT_LINE);
                itemDto.Descripcion = item.SHORT_TEXT;
                itemDto.Cantidad = item.QUANTITY;
                itemDto.PosicionId = Convert.ToInt32(item.PCKG_NO);
                itemDto.PrecioBruto = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0,4);               
                itemDto.ServicioNumero = item.SERVICE != "" ? long.Parse(item.SERVICE) : 0;
                itemDto.UM = item.BASE_UOM;
                itemDto.Importe = Math.Round((item.QUANTITY != 0) ? item.NET_VALUE / item.QUANTITY : 0, 4);
                
                if(itemDto.Importe != null)
                {
                    decimal notNullValue = (decimal)itemDto.Importe;
                    itemDto.ImporteString = notNullValue.ToString(currencyFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                //MMSN-460 - Moneda
                itemDto.Moneda = POHEADER.CURRENCY;
                //MMSN-460 - Nro de servicio
                itemDto.ServicioNumero = item.SERVICE != "" ? int.Parse(item.SERVICE) : 0;
                //MMSN-460 - Porcentaje (inicialización - necesaria para FE)
                itemDto.Porcentaje = "0"; // si no tiene entradas de servicios asociadas el porcentaje es 0
                itemDto.CantidadReal = 0; // si no tiene entras de servicios asociadas la cantidad real es = 0

                itemDto.EntradasServicio = ObtenerEntradasDeServicioDelItem(pOSERVICES, pOHISTORY, itemDto.Id, itemDto.LINE_NO);
                if (itemDto.EntradasServicio.Count > 0)
                {
                    itemDto = CalcularCampos(itemDto);
                }

                itemDto.NroOrdenCompra = POHEADER.PO_NUMBER;
                itemDto.NroPosicion = Posicion.NumeroPosicion.ToString();


                itemsDeLaPosicion.Add(itemDto);
            }

            return itemsDeLaPosicion;
        }


        /// <summary>
        /// Obtiene las Entradas de Servicio de un Item
        /// </summary>
        /// <param name="pOSERVICES"></param>
        /// <param name="pOHISTORY"></param>
        /// <param name="idDelItem"></param>
        /// <returns></returns>
        private List<EntradaServicioDto> ObtenerEntradasDeServicioDelItem(BAPIESLLC[] pOSERVICES, BAPIEKBE[] pOHISTORY, string idDelItem, string idDeLinea)
        {
            List<EntradaServicioDto> EntradasServicioDelItem = new List<EntradaServicioDto>();
            List<BAPIEKBE> entradasDeServicioPotenciales = pOHISTORY.Where(x => x.PROCESS_ID == "9" && x.HIST_TYPE == "D").ToList();
            List<string> listaDeEntradasDeServicioFacturadas = pOHISTORY
                .Where(x => (x.HIST_TYPE == "Q" || x.HIST_TYPE == "R") && x.PROCESS_ID == "2")
                .Select(x => x.REF_DOC)
                .ToList();


            foreach (var entradaServicioPotencial in entradasDeServicioPotenciales)
            {
                EntradaServicioDto entradaServicioDto = new EntradaServicioDto();
                string _nroES = entradaServicioPotencial.MAT_DOC;
                EntradaServicioDto entradaServicioSAP = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicio(_nroES);
                List<ItemEntradaServicioDto> _itemsDeEntradaServicio = entradaServicioSAP.Items;
                bool entradaServicioFacturada = EntradaServicioTieneFactura(listaDeEntradasDeServicioFacturadas, _nroES);

                foreach (ItemEntradaServicioDto itemES in _itemsDeEntradaServicio)
                {

                    if (itemES.ItemNumero == idDelItem && itemES.PLN_LINE == idDeLinea)
                    {
                        DateTime _fechaContabilizacion = SAPFormatter.GetDateTime(entradaServicioSAP.FechaContabilizacion);
                        bool entradaServicioDentroDePeriodoSAP = DentroPeriodoSAP(_fechaContabilizacion, DateTime.Now);


                        entradaServicioDto.Id = int.Parse(itemES.Id);
                        entradaServicioDto.itemNumero = itemES.ItemNumero;
                        entradaServicioDto.TextoBreve = itemES.Descripcion;
                        entradaServicioDto.Cantidad = itemES.Cantidad;
                        entradaServicioDto.ESS_PCKG_NO = itemES.PCKG_NO;
                        entradaServicioDto.ESS_LINE_NO = itemES.LINE_NO;
                        entradaServicioDto.ESS_EXT_LINE = itemES.EXT_LINE;

                        //MMSN-460 - Informacion de Cabecera p/ FE
                        entradaServicioDto.Fecha = entradaServicioSAP.Fecha;
                        entradaServicioDto.FechaDocumentoString = entradaServicioSAP.FechaDocumentoString;
                        entradaServicioDto.FechaContabilizacion = entradaServicioSAP.FechaContabilizacion;
                        entradaServicioDto.Referencia = entradaServicioSAP.Referencia;
                        entradaServicioDto.ImporteARPUSD = entradaServicioSAP.ImporteARPUSD;
                        entradaServicioDto.SePuedeBorrar = !entradaServicioFacturada && entradaServicioDentroDePeriodoSAP;

                        EntradasServicioDelItem.Add(entradaServicioDto);
                    }
                }
            }

            return EntradasServicioDelItem;
        }

        /// <summary>
        /// MMSN-460: Calculo de Cantidad Real y Porcentaje para ItemDTO
        /// </summary>
        /// <param name="itemDto"></param>
        /// <returns></returns>
        private ItemDto CalcularCampos(ItemDto itemDto)
        {
            bool calcularPorcentaje = false;

            //MMSN-460 - Cantidad Real
            //Inicializar en 0 si hay elementos en ES
            if (itemDto.EntradasServicio.Count > 0)
            {
                itemDto.CantidadReal = 0;
                calcularPorcentaje = true;
            }

            try
            {
                //recorrer la lista de Entradas de Servicio, y contabilizar la cantidad
                foreach (var es in itemDto.EntradasServicio)
                {
                    if (es.Cantidad != null)
                    {
                        itemDto.CantidadReal = itemDto.CantidadReal + es.Cantidad;
                    }

                }

                //MMSN-460 - Porcentaje (% del item = cantidadReal x 100 / cantidad)
                if (itemDto.Cantidad != null && itemDto.Cantidad != 0)
                {
                    if (calcularPorcentaje == true && (itemDto.CantidadReal != null && itemDto.CantidadReal != 0))
                    {
                        double res = Convert.ToDouble((itemDto.CantidadReal * 100) / itemDto.Cantidad);
                        itemDto.Porcentaje = res.ToString("0.##");

                        if (itemDto.Porcentaje.EndsWith(".00"))
                        {
                            var redondeo = Math.Round(res);
                            itemDto.Porcentaje = res.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //Evito detener ejecución
            }

            return itemDto;
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

        /// <summary>
        /// MMSN-480: Devuelve verdadero si la entrada de servicio tiene factura
        /// </summary>
        /// <param name="nrosEntradasServicioFacturadas"></param>
        /// <param name="nroES"></param>
        /// <returns></returns>
        public bool EntradaServicioTieneFactura(List<string> nrosEntradasServicioFacturadas, string nroES)
        {
            bool entradaServicioFacturada = nrosEntradasServicioFacturadas.Any(x => x == nroES);

            return entradaServicioFacturada;
        }

        /// <summary>
        /// MMSN-480: Devuelve verdadero si la fecha actual está dentro del período SAP (Dias del mes actual y mes anterior completo)
        /// </summary>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaActual"></param>
        /// <returns></returns>
        public bool DentroPeriodoSAP(DateTime fechaInicial, DateTime fechaActual)
        {
            int diferenciaEnMeses = ((fechaActual.Year - fechaInicial.Year) * 12) + fechaActual.Month - fechaInicial.Month;

            return diferenciaEnMeses < 2;
        }
    }
}
