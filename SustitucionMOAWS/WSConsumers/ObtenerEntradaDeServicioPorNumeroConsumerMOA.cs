using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerEntradaDeServicioPorNumeroWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MLBO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    /// <summary>
    /// Obtiene detalle de una entrada de servicio con el numero de entrada de servicio
    /// </summary>
    public class ObtenerEntradaDeServicioPorNumeroConsumerMOA : IObtenerEntradaDeServicioPorNumeroConsumerMOA
    {
        //private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;
        /// <summary>
        /// MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
        /// </summary>
        private string dateTimeFormat = "dd/MM/yyyy";
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ObtenerEntradaDeServicioPorNumeroConsumerMOA()
        {

        }


        /// <summary>
        /// Obtiene detalle de una entrada de servicio con el numero de entrada de servicio
        /// </summary>
        /// <param name="nroES"></param>
        /// <returns></returns>
        public EntradaServicioDto ObtenerEntradaServicio(string nroES)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_BAPI_DIRECT_MLBOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    string ENTRYSHEET = nroES;
                    string LONG_TEXTS = "";
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKN[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT     = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESSRTX[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLL[] ENTRYSHEET_SERVICES          = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLL[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS  = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLLTX[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKL[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIRETURN1[] RETURN                    = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIRETURN1[] { };

                    var request = new BAPI_ENTRYSHEET_GETDETAIL()
                    {
                        ENTRYSHEET = ENTRYSHEET,
                        LONG_TEXTS = LONG_TEXTS,
                        ENTRYSHEET_ACCOUNT_ASSIGNMENT = ENTRYSHEET_ACCOUNT_ASSIGMENT,
                        ENTRYSHEET_HEADER_TEXT = ENTRYSHEET_HEADER_TEXT,
                        ENTRYSHEET_SERVICES = ENTRYSHEET_SERVICES,
                        ENTRYSHEET_SERVICES_TEXTS = ENTRYSHEET_SERVICES_TEXTS,
                        ENTRYSHEET_SRV_ACCASS_VALUES = ENTRYSHEET_SRV_ACCASS_VALUES,
                        RETURN = RETURN
                    };
                    Log.Info($"SAP sin PI BAPI_ENTRYSHEET_GETDETAIL");
                    Log.Info(request.ToXml());
                    var response = agent.BAPI_ENTRYSHEET_GETDETAIL(request);
                    Log.Info(response.ToXml());
                    Log.Info($"SAP sin PI BAPI_ENTRYSHEET_GETDETAIL");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_PO_GETDETAIL1&amp;interfaceNamespace=urn:sap-com:document:sap:rfc:functions";
                    service = new SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string ENTRYSHEET = nroES;
                    string LONG_TEXTS = "";
                    //BAPIESSR[] ENTRYSHEET_HEADER = new BAPIESSR[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKN[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSRTX[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL[] ENTRYSHEET_SERVICES = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL[] { };
                    //ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLLTX[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKL[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIRETURN1[] RETURN = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIRETURN1[] { };

                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSR cabecera = service.SI_MMRFC_BAPI_ENTRYSHEET_GETDETAIL(ENTRYSHEET, LONG_TEXTS, ref ENTRYSHEET_ACCOUNT_ASSIGMENT, ref ENTRYSHEET_HEADER_TEXT, ref ENTRYSHEET_SERVICES, ref ENTRYSHEET_SERVICES_TEXTS, ref ENTRYSHEET_SRV_ACCASS_VALUES, ref RETURN);

                    return Map(cabecera, ENTRYSHEET_SERVICES);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private EntradaServicioDto Map(ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSR cabecera, ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL[] entrySheetService)
        {
            EntradaServicioDto result = new EntradaServicioDto();

            IEnumerable<ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL> itemsEntrySheetService = entrySheetService
                .Where(a => a.DELETE_IND != "X" && a.OUTL_IND != "X");

            List<ItemEntradaServicioDto> items = new List<ItemEntradaServicioDto>();

            /// Recorre el detalle de la entrada de servicio
            foreach (var elementoEntrySheetService in itemsEntrySheetService)
            {
                string formattedValue = elementoEntrySheetService.NET_VALUE.ToString("N2", CultureInfo.InvariantCulture);
                string currency = cabecera.CURRENCY;

                var item = new ItemEntradaServicioDto
                {
                    Id = cabecera.SHEET_NO,
                    //Descripcion = cabecera.SHORT_TEXT,
                    Descripcion =
                        string.IsNullOrEmpty(cabecera.SHORT_TEXT) || cabecera.SHORT_TEXT == "Este campo es ignorado por el servicio SAP, pero debe enviarsele algo"
                            // Por algún motivo se decidió enviar a sap ese texto cuando falta la descripción (en realidad, antes se enviaba siempre...).
                            // Por lo que ahora estamos atrapados consultando por ese texto para evitar mostrarlo... ¬¬
                            ? ""
                            : cabecera.SHORT_TEXT.Trim(),

                    ItemNumero = elementoEntrySheetService.PLN_PCKG,
                    Cantidad = elementoEntrySheetService.QUANTITY,
                    PLN_PCKG = elementoEntrySheetService.PLN_PCKG,
                    PLN_LINE = elementoEntrySheetService.PLN_LINE,
                    PCKG_NO = elementoEntrySheetService.PCKG_NO,
                    LINE_NO = elementoEntrySheetService.LINE_NO
                };

                if (string.Compare(currency, "ARP", true) == 0)
                {
                    item.ImporteARPUSD = $"$ {formattedValue}";
                }
                else
                {
                    item.ImporteARPUSD = $"{formattedValue} {currency.ToUpper()}";
                }

                items.Add(item);
            }

            //MMSN-460 + MMSN-491
            if (!string.IsNullOrEmpty(cabecera.CREATED_ON))
            {
                DateTime toFormat = DateTime.ParseExact(cabecera.CREATED_ON, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                result.Fecha = toFormat.ToString(dateTimeFormat);
            }

            if (!string.IsNullOrEmpty(cabecera.DOC_DATE))
            {
                DateTime toFormat = DateTime.ParseExact(cabecera.DOC_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                result.FechaDocumentoString = toFormat.ToString(dateTimeFormat);
            }

            result.Referencia = cabecera.REF_DOC_NO;
            result.FechaContabilizacion = cabecera.POST_DATE;
            result.TextoBreve = cabecera.SHORT_TEXT;

            result.Items = items;

            return result;
        }
        private EntradaServicioDto MapSinPI(BAPI_ENTRYSHEET_GETDETAILResponse response)
        {
            EntradaServicioDto result = new EntradaServicioDto();
            var cabecera = response.ENTRYSHEET_HEADER;
            IEnumerable<WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLL> itemsEntrySheetService = response.ENTRYSHEET_SERVICES
                .Where(a => a.DELETE_IND != "X" && a.OUTL_IND != "X");

            List<ItemEntradaServicioDto> items = new List<ItemEntradaServicioDto>();

            /// Recorre el detalle de la entrada de servicio
            foreach (var elementoEntrySheetService in itemsEntrySheetService)
            {
                string formattedValue = elementoEntrySheetService.NET_VALUE.ToString("N2", CultureInfo.InvariantCulture);
                string currency = cabecera.CURRENCY;

                var item = new ItemEntradaServicioDto
                {
                    Id = cabecera.SHEET_NO,
                    //Descripcion = cabecera.SHORT_TEXT,
                    Descripcion =
                        string.IsNullOrEmpty(cabecera.SHORT_TEXT) || cabecera.SHORT_TEXT == "Este campo es ignorado por el servicio SAP, pero debe enviarsele algo"
                            // Por algún motivo se decidió enviar a sap ese texto cuando falta la descripción (en realidad, antes se enviaba siempre...).
                            // Por lo que ahora estamos atrapados consultando por ese texto para evitar mostrarlo... ¬¬
                            ? ""
                            : cabecera.SHORT_TEXT.Trim(),

                    ItemNumero = elementoEntrySheetService.PLN_PCKG,
                    Cantidad = elementoEntrySheetService.QUANTITY,
                    PLN_PCKG = elementoEntrySheetService.PLN_PCKG,
                    PLN_LINE = elementoEntrySheetService.PLN_LINE,
                    PCKG_NO = elementoEntrySheetService.PCKG_NO,
                    LINE_NO = elementoEntrySheetService.LINE_NO
                };

                if (string.Compare(currency, "ARP", true) == 0)
                {
                    item.ImporteARPUSD = $"$ {formattedValue}";
                }
                else
                {
                    item.ImporteARPUSD = $"{formattedValue} {currency.ToUpper()}";
                }

                items.Add(item);
            }

            //MMSN-460 + MMSN-491
            if (!string.IsNullOrEmpty(cabecera.CREATED_ON))
            {
                DateTime toFormat = DateTime.ParseExact(cabecera.CREATED_ON, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                result.Fecha = toFormat.ToString(dateTimeFormat);
            }

            if (!string.IsNullOrEmpty(cabecera.DOC_DATE))
            {
                DateTime toFormat = DateTime.ParseExact(cabecera.DOC_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                result.FechaDocumentoString = toFormat.ToString(dateTimeFormat);
            }

            result.Referencia = cabecera.REF_DOC_NO;
            result.FechaContabilizacion = cabecera.POST_DATE;
            result.TextoBreve = cabecera.SHORT_TEXT;

            result.Items = items;

            return result;
        }


        public List<EntradaServicioDetalleDto> ObtenerEntradaServicioDetalle(string nroES) // El tipo que devuelve esta puesto solo para que no marque error
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_BAPI_DIRECT_MLBOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    string ENTRYSHEET = nroES; //Valor de prueba, existe en SAP
                    string LONG_TEXTS = "";
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKN[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT     = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESSRTX[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLL[] ENTRYSHEET_SERVICES          = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLL[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS  = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLLTX[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESKL[] { };
                    WS_GAQ_sin_PI_DIRECT_MLBO.BAPIRETURN1[] RETURN                    = new WS_GAQ_sin_PI_DIRECT_MLBO.BAPIRETURN1[] { };

                    var request = new BAPI_ENTRYSHEET_GETDETAIL()
                    {
                        ENTRYSHEET = ENTRYSHEET,
                        LONG_TEXTS = LONG_TEXTS,
                        ENTRYSHEET_ACCOUNT_ASSIGNMENT = ENTRYSHEET_ACCOUNT_ASSIGMENT,
                        ENTRYSHEET_HEADER_TEXT = ENTRYSHEET_HEADER_TEXT,
                        ENTRYSHEET_SERVICES = ENTRYSHEET_SERVICES,
                        ENTRYSHEET_SERVICES_TEXTS = ENTRYSHEET_SERVICES_TEXTS,
                        ENTRYSHEET_SRV_ACCASS_VALUES = ENTRYSHEET_SRV_ACCASS_VALUES,
                        RETURN = RETURN
                    };
                    Log.Info($"SAP sin PI BAPI_ENTRYSHEET_GETDETAIL request");
                    Log.Info(request.ToXml());
                    var response = agent.BAPI_ENTRYSHEET_GETDETAIL(request);
                    Log.Info($"SAP sin PI BAPI_ENTRYSHEET_GETDETAIL response");
                    Log.Info(response.ToXml());
                    return MapDetalleSinPI(response);
                }
                else
                {
                    SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_PO_GETDETAIL1&amp;interfaceNamespace=urn:sap-com:document:sap:rfc:functions";
                    service = new SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string ENTRYSHEET = nroES; //Valor de prueba, existe en SAP
                    string LONG_TEXTS = "";
                    //BAPIESSR[] ENTRYSHEET_HEADER = new BAPIESSR[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKN[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSRTX[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL[] ENTRYSHEET_SERVICES = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL[] { };
                    //ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLLTX[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESKL[] { };
                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIRETURN1[] RETURN = new ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIRETURN1[] { };

                    ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSR detalleES = service.SI_MMRFC_BAPI_ENTRYSHEET_GETDETAIL(ENTRYSHEET, LONG_TEXTS, ref ENTRYSHEET_ACCOUNT_ASSIGMENT, ref ENTRYSHEET_HEADER_TEXT, ref ENTRYSHEET_SERVICES, ref ENTRYSHEET_SERVICES_TEXTS, ref ENTRYSHEET_SRV_ACCASS_VALUES, ref RETURN);

                    return MapDetalle(detalleES, ENTRYSHEET_SERVICES);
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<EntradaServicioDetalleDto> MapDetalle(ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESSR cabecera, ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL[] entrySheetService)
        {
            List<EntradaServicioDetalleDto> result = new List<EntradaServicioDetalleDto>();

            IEnumerable<ObtenerEntradaDeServicioPorNumeroWebServiceMOA.BAPIESLL> itemsEntrySheetService = entrySheetService
                .Where(a => a.DELETE_IND != "X" && a.OUTL_IND != "X");

            List<EntradaServicioDetalleDto> items = new List<EntradaServicioDetalleDto>();
            foreach (var elemento in itemsEntrySheetService)
            {
                var item = new EntradaServicioDetalleDto();
                item.OrdenCompra = cabecera.PO_NUMBER;
                item.NumeroLinea = elemento.PLN_LINE;
                item.PLN_PCKG = elemento.PLN_PCKG;
                item.CodigoServicio = elemento.SERVICE;
                item.Descripcion = elemento.SHORT_TEXT;
                item.Cantidad = elemento.QUANTITY.ToString();
                item.UM = elemento.BASE_UOM;
                item.Monto = elemento.NET_VALUE;
                item.Ext_line = elemento.EXT_LINE;
                item.PrecioUnitario = elemento.GR_PRICE; // elemento.NET_VALUE * elemento.QUANTITY;

                items.Add(item);
            }
            return items;
        }
        private List<EntradaServicioDetalleDto> MapDetalleSinPI(BAPI_ENTRYSHEET_GETDETAILResponse response)
        {
            List<EntradaServicioDetalleDto> result = new List<EntradaServicioDetalleDto>();
            var cabecera = response.ENTRYSHEET_HEADER;
            IEnumerable<WS_GAQ_sin_PI_DIRECT_MLBO.BAPIESLL> itemsEntrySheetService = response.ENTRYSHEET_SERVICES
                .Where(a => a.DELETE_IND != "X" && a.OUTL_IND != "X");

            List<EntradaServicioDetalleDto> items = new List<EntradaServicioDetalleDto>();
            foreach (var elemento in itemsEntrySheetService)
            {
                var item = new EntradaServicioDetalleDto();
                item.OrdenCompra = cabecera.PO_NUMBER;
                item.NumeroLinea = elemento.PLN_LINE;
                item.PLN_PCKG = elemento.PLN_PCKG;
                item.CodigoServicio = elemento.SERVICE;
                item.Descripcion = elemento.SHORT_TEXT;
                item.Cantidad = elemento.QUANTITY.ToString();
                item.UM = elemento.BASE_UOM;
                item.Monto = elemento.NET_VALUE;
                item.Ext_line = elemento.EXT_LINE;
                item.PrecioUnitario = elemento.GR_PRICE; // elemento.NET_VALUE * elemento.QUANTITY;

                items.Add(item);
            }
            return items;
        }

    }
}
