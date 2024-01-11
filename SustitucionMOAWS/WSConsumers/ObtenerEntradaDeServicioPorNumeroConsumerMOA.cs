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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerEntradaDeServicioPorNumeroConsumerMOA : IObtenerEntradaDeServicioPorNumeroConsumerMOA
    {
        // Obtiene detalle de una entrada de servicio por numero de entrada de servicio
        SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient service;
        //private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;
        /// <summary>
        /// MMSN-491 - Modificar el formato de fecha. DD/MM/AAAA
        /// </summary>
        private string dateTimeFormat = "dd/MM/yyyy";

        public ObtenerEntradaDeServicioPorNumeroConsumerMOA()
        {
            service = new SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            //this.repositorio = repositorio;
        }

        public EntradaServicioDto ObtenerEntradaServicio(string nroES) // El tipo que devuelve esta puesto solo para que no marque error
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
                .Where(a => a.DELETE_IND != "X" && a.PLN_PCKG != "0000000000");

            List<ItemEntradaServicioDto> items = new List<ItemEntradaServicioDto>();
            foreach (var elementoEntrySheetService in itemsEntrySheetService)
            {
                var item = new ItemEntradaServicioDto();

                item.Id = cabecera.SHEET_NO;
                item.ItemNumero = elementoEntrySheetService.PLN_PCKG;

                item.Cantidad = elementoEntrySheetService.QUANTITY;
                item.Descripcion = elementoEntrySheetService.SHORT_TEXT;

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
            result.ImporteARPUSD = cabecera.CURRENCY;
            result.FechaContabilizacion = cabecera.POST_DATE;

            result.Items = items;

            return result;
        }

        
    }
}
