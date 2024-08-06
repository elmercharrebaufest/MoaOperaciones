using Microsoft.SqlServer.Server;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerEntradaDeServicioPorNumeroWebServiceMOA;
using SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA;
using SustitucionMOAWS.ObtenerOrdenesDeCompraWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace SustitucionMOAWS.WSConsumers
{
    /// <summary>
    /// Obtiene detalle de una entrada de servicio con el numero de entrada de servicio
    /// </summary>
    public class ObtenerEntradaDeServicioPorNumeroConsumerMOA : IObtenerEntradaDeServicioPorNumeroConsumerMOA
    {
        SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient service;
        //private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;
        /// <summary>
        /// MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
        /// </summary>
        private string dateTimeFormat = "dd/MM/yyyy";

        public ObtenerEntradaDeServicioPorNumeroConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=BAPI_PO_GETDETAIL1&amp;interfaceNamespace=urn:sap-com:document:sap:rfc:functions";
            //var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_BAPI_ENTRYSHEET_GETDETAIL&amp;interfaceNamespace=urn:OPERACIONES";
            service = new SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            //this.repositorio = repositorio;
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
                string ENTRYSHEET = nroES;
                string LONG_TEXTS = "";
                //BAPIESSR[] ENTRYSHEET_HEADER = new BAPIESSR[] { };
                BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new BAPIESKN[] { };
                BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT = new BAPIESSRTX[] { };
                BAPIESLL[] ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                //ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS = new BAPIESLLTX[] { };
                BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new BAPIESKL[] { };
                BAPIRETURN1[] RETURN = new BAPIRETURN1[] { };

                BAPIESSR cabecera = service.SI_MMRFC_BAPI_ENTRYSHEET_GETDETAIL(ENTRYSHEET, LONG_TEXTS, ref ENTRYSHEET_ACCOUNT_ASSIGMENT, ref ENTRYSHEET_HEADER_TEXT, ref ENTRYSHEET_SERVICES, ref ENTRYSHEET_SERVICES_TEXTS, ref ENTRYSHEET_SRV_ACCASS_VALUES, ref RETURN);

                return Map(cabecera, ENTRYSHEET_SERVICES);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private EntradaServicioDto Map(BAPIESSR cabecera, BAPIESLL[] entrySheetService)
        {
            EntradaServicioDto result = new EntradaServicioDto();

            IEnumerable<BAPIESLL> itemsEntrySheetService = entrySheetService
                .Where(a => a.DELETE_IND != "X" && a.OUTL_IND != "X");

            List<ItemEntradaServicioDto> items = new List<ItemEntradaServicioDto>();

            /// Recorre el detalle de la entrada de servicio
            foreach (var elementoEntrySheetService in itemsEntrySheetService)
            {

                string formattedValue = elementoEntrySheetService.NET_VALUE.ToString("N2");
                string currency = cabecera.CURRENCY;

                var item = new ItemEntradaServicioDto();


                item.Id = cabecera.SHEET_NO;
                item.Descripcion = cabecera.SHORT_TEXT;
                
                item.ItemNumero = elementoEntrySheetService.PLN_PCKG;
                item.Cantidad = elementoEntrySheetService.QUANTITY;
                item.PLN_PCKG = elementoEntrySheetService.PLN_PCKG;
                item.PLN_LINE = elementoEntrySheetService.PLN_LINE;
                item.PCKG_NO = elementoEntrySheetService.PCKG_NO;
                item.LINE_NO = elementoEntrySheetService.LINE_NO;
                item.ImporteARPUSD = currency == "ARP"
                ? $"$ {formattedValue}"
                : $"{formattedValue} {currency}";

                items.Add(item);
            }

            //MMSN-460 + MMSN-491
            if (!String.IsNullOrEmpty(cabecera.CREATED_ON))
            {
                DateTime toFormat = DateTime.ParseExact(cabecera.CREATED_ON, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                result.Fecha = toFormat.ToString(dateTimeFormat);
            }

            if (!String.IsNullOrEmpty(cabecera.DOC_DATE))
            {
                DateTime toFormat = DateTime.ParseExact(cabecera.DOC_DATE, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
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
                string ENTRYSHEET = nroES; //Valor de prueba, existe en SAP
                string LONG_TEXTS = "";
                //BAPIESSR[] ENTRYSHEET_HEADER = new BAPIESSR[] { };
                BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new BAPIESKN[] { };
                BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT = new BAPIESSRTX[] { };
                BAPIESLL[] ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                //ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS = new BAPIESLLTX[] { };
                BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new BAPIESKL[] { };
                BAPIRETURN1[] RETURN = new BAPIRETURN1[] { };

                BAPIESSR detalleES = service.SI_MMRFC_BAPI_ENTRYSHEET_GETDETAIL(ENTRYSHEET, LONG_TEXTS, ref ENTRYSHEET_ACCOUNT_ASSIGMENT, ref ENTRYSHEET_HEADER_TEXT, ref ENTRYSHEET_SERVICES, ref ENTRYSHEET_SERVICES_TEXTS, ref ENTRYSHEET_SRV_ACCASS_VALUES, ref RETURN);

                return MapDetalle(detalleES, ENTRYSHEET_SERVICES);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<EntradaServicioDetalleDto> MapDetalle(BAPIESSR cabecera, BAPIESLL[] entrySheetService)
        {
            List<EntradaServicioDetalleDto> result = new List<EntradaServicioDetalleDto>();

            IEnumerable<BAPIESLL> itemsEntrySheetService = entrySheetService
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

                items.Add(item);
            }
            return items;
        }

    }
}
