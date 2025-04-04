using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAWS.CrearEntradaServicioWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//using BAPIESKLC = SustitucionMOAWS.CrearEntradaServicioWebServiceMOA.BAPIESKLC;
//using BAPIESLLC = SustitucionMOAWS.CrearEntradaServicioWebServiceMOA.BAPIESLLC;
//using BAPIESLLTX = SustitucionMOAWS.CrearEntradaServicioWebServiceMOA.BAPIESLLTX;
//using BAPIESSRTX = SustitucionMOAWS.CrearEntradaServicioWebServiceMOA.BAPIESSRTX;
//using BAPIRET2 = SustitucionMOAWS.CrearEntradaServicioWebServiceMOA.BAPIRET2;


namespace SustitucionMOAWS.WSConsumers
{
    public class CrearEntradaDeServicioConsumerMOA : ICrearEntradaDeServicioConsumerMOA
    {
        private readonly SI_MMRFC_BAPI_ENTRYSHEET_CREATEClient service;

        public CrearEntradaDeServicioConsumerMOA()
        {
            service = new SI_MMRFC_BAPI_ENTRYSHEET_CREATEClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public EntradaServicioCreateRespuestaDto CrearEntradaServicio(EntradaServicioCreateParamsDto parametros)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = CrearRequestMessage();
                    requestMessage.Content = CrearHttpContent(parametros);

                    var responseMessage = client.SendAsync(requestMessage).ConfigureAwait(false).GetAwaiter().GetResult();

                    var createResponseContent = responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    Logger.Log.Debug("Respuesta CrearEntradaServicio: " + createResponseContent);

                    var respuestaCreacionESDto = ParseReturnInfo(createResponseContent);

                    return respuestaCreacionESDto;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e, "Error con " + parametros.ToJson());
                throw;
            }
        }

        public async Task<EntradaServicioCreateRespuestaDto> CrearEntradaServicioAsync(EntradaServicioCreateParamsDto parametros)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var requestMessage = CrearRequestMessage();

                    var content = CrearHttpContent(parametros);

                    string contentAsString = await content.ReadAsStringAsync();
                    Logger.Log.Debug("CrearEntradaDeServicioConsumerMOA content: " + contentAsString);

                    requestMessage.Content = content;

                    await Task.Delay(500);

                    var responseMessage = await client.SendAsync(requestMessage).ConfigureAwait(false);

                    var createResponseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                    var respuestaCreacionESDto = ParseReturnInfo(createResponseContent);

                    Logger.Log.Info("CrearEntradaDeServicioConsumerMOA.CrearEntradaServicioAsync: " + respuestaCreacionESDto.ToJson());

                    return respuestaCreacionESDto;
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                throw;
            }
        }

        private HttpRequestMessage CrearRequestMessage()
        {
            var urlServicio = SAPCredential.DevolverEndpoint(System.Configuration.ConfigurationManager.AppSettings["ServicioSAPEntradasServicioCrear"]).ToString();
            var SOAPAction = System.Configuration.ConfigurationManager.AppSettings["SOAPAction"];

            var credenciales = service.ClientCredentials.UserName.UserName + ":" + service.ClientCredentials.UserName.Password;
            var credencialesBytes = Encoding.UTF8.GetBytes(credenciales);
            var credencialesBase64 = Convert.ToBase64String(credencialesBytes);
            var authorization = "Basic " + credencialesBase64;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, urlServicio);
            requestMessage.Headers.Add("SOAPAction", SOAPAction);
            requestMessage.Headers.Add("Authorization", authorization);

            return requestMessage;
        }

        private StringContent CrearHttpContent(EntradaServicioCreateParamsDto parametros)
        {
            var entrySheetHeaderXml = GenerateEntrySheetHeaderXml(parametros.EntrySheetHeader);
            var entrySheetServicesXml = GenerateEntrySheetServicesXml(parametros.EntrySheetServices.Items);

            var stringContent = new StringContent($@"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:rfc:functions"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <urn:BAPI_ENTRYSHEET_CREATE>
                            {entrySheetHeaderXml}
                        <ENTRYSHEETSERVICES>
                            {entrySheetServicesXml}
                        </ENTRYSHEETSERVICES>
                        <ENTRYSHEETSERVICESTEXTS></ENTRYSHEETSERVICESTEXTS>
                        <ENTRYSHEETSRVACCASSVALUES></ENTRYSHEETSRVACCASSVALUES>
                        <RETURN></RETURN>
                        </urn:BAPI_ENTRYSHEET_CREATE>
                    </soapenv:Body>
                </soapenv:Envelope>", Encoding.UTF8, "text/xml"
            );

            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/xml")
            {
                CharSet = "utf-8"
            };

            return stringContent;
        }

        private string GenerateEntrySheetHeaderXml(EntrySheetHeaderSection header)
        {
            return $@"
                <ENTRYSHEETHEADER>
                    <PCKG_NO>0000000001</PCKG_NO>
                    <SHORT_TEXT>{header.Descripcion}</SHORT_TEXT>
                    <PO_NUMBER>{header.OrdenCompraNumero}</PO_NUMBER>
                    <PO_ITEM>{header.OrdenCompraPosicionNumero}</PO_ITEM>
                    <REF_DOC_NO>{header.DocumentoReferenciaNumero}</REF_DOC_NO>
                    <DOC_DATE>{header.FechaDocumento}</DOC_DATE>
                    <POST_DATE>{header.FechaContabilizacion}</POST_DATE>
                    <ACCEPTANCE>X</ACCEPTANCE>
                </ENTRYSHEETHEADER>";
        }

        private string GenerateEntrySheetServicesXml(List<EntrySheetServiceItemSection> entrySheetServices)
        {
            var xmlBuilder = new StringBuilder();

            var entrySheetServicesCabeceraFija = new EntrySheetServiceItemSection
            {
                PackageNumber = "0000000001",
                LineNumber = "0000000001",
                OutlineIndicator = "X",
                SubPackageNumber = "0000000002",
                Quantity = "1"
            };

            var contadorDeInstancia = 1;

            foreach (var entrySheetService in entrySheetServices)
            {
                // Aquí va la lógica para generar dinámicamente el XML para cada EntrySheetServiceItemSection
                // Solo si es la primera vez se carga la parte fija que es como la "cabecera" del detalle
                if (contadorDeInstancia == 1)
                {
                    xmlBuilder.AppendLine(GenerateEntrySheetServiceXml(entrySheetServicesCabeceraFija));
                    contadorDeInstancia++;
                }
                entrySheetService.PackageNumber = "0000000002";
                entrySheetService.LineNumber = contadorDeInstancia.ToString("D10");
                entrySheetService.ShortText =
                    string.IsNullOrWhiteSpace(entrySheetService.ShortText)
                    ? "Este campo es ignorado por el servicio SAP, pero debe enviarsele algo"
                    : entrySheetService.ShortText;

                contadorDeInstancia++;

                xmlBuilder.AppendLine(GenerateEntrySheetServiceXml(entrySheetService));
            }

            return xmlBuilder.ToString();
        }


        private string GenerateEntrySheetServiceXml(EntrySheetServiceItemSection item)
        {
            return $@"
                <item>
                    <PCKG_NO>{item.PackageNumber}</PCKG_NO>
                    <LINE_NO>{item.LineNumber}</LINE_NO>
                    <OUTL_IND>{item.OutlineIndicator}</OUTL_IND>
                    <SUBPCKG_NO>{item.SubPackageNumber}</SUBPCKG_NO>
                    <EXT_LINE>{item.ExternalLineNumber}</EXT_LINE>
                    <SERVICE>{((item.Service ?? "0").Trim() == "0" ? "" : item.Service)}</SERVICE>
                    <QUANTITY>{item.Quantity.Replace(",", ".")}</QUANTITY>
                    <GR_PRICE>{item.GrossPrice.ToString().Replace(",", ".")}</GR_PRICE>
                    <SHORT_TEXT>{item.ShortText}</SHORT_TEXT>
                    <PLN_PCKG>{item.PlannedPackage}</PLN_PCKG>
                    <PLN_LINE>{item.PlannedLine}</PLN_LINE>
                </item>";
        }

        static EntradaServicioCreateRespuestaDto ParseReturnInfo(string soapResponse)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(soapResponse);

            XmlNodeList returnNodes = xmlDoc.SelectNodes("//RETURN/item"); // Obtener nodos de la sección RETURN

            EntradaServicioCreateRespuestaDto returnInfo = new EntradaServicioCreateRespuestaDto();
            // jji: Como la respuesta puede contener mas de un nodo, se recorren todos.
            // Es necesario mejorar / corregir porque solo está mostrando el ultimo mensaje.
            // Con lo cual no se sabe si hay mensajes de error en los anteriores.
            foreach (XmlNode returnNode in returnNodes)
            {
                returnInfo.Type = returnNode.SelectSingleNode("TYPE")?.InnerText;
                returnInfo.Id = returnNode.SelectSingleNode("ID")?.InnerText;
                returnInfo.Number = returnNode.SelectSingleNode("NUMBER")?.InnerText;
                returnInfo.Message = returnNode.SelectSingleNode("MESSAGE")?.InnerText;
            }

            return returnInfo;
        }


        /// <summary>
        /// El servicio referenciado no funciona. HttpClient() es el que funciona.
        /// </summary>
        /// <param name="parametros"></param>
        /// <returns></returns>
        //public string CrearEntradaServicio(EntradaServicioCreateParamsDto parametros)
        //{
        //    try
        //    {
        //        BAPIESSRC ENTRYSHEETHEADER = new BAPIESSRC();
        //        string NO_COMMIT = "";
        //        string TESTRUN = "";
        //        BAPIESKNC[] ENTRYSHEETACCOUNTASSIGNMENT = new BAPIESKNC[] { };
        //        BAPIESSRTX[] ENTRYSHEETHEADERTEXT = new BAPIESSRTX[] { };
        //        BAPIESLLC[] ENTRYSHEETSERVICES = new BAPIESLLC[1]; // Mantenemos la declaración como un array
        //        BAPIESLLTX[] ENTRYSHEETSERVICESTEXTS = new BAPIESLLTX[] { };
        //        BAPIESKLC[] ENTRYSHEETSRVACCASSVALUES = new BAPIESKLC[] { };
        //        BAPIRET2[] RETURN = new BAPIRET2[] { };

        //        // Mapeo de ENTRYSHEETHEADER
        //        ENTRYSHEETHEADER.PO_NUMBER = parametros.EntrySheetHeader.OrdenCompraNumero;
        //        ENTRYSHEETHEADER.PO_ITEM = parametros.EntrySheetHeader.OrdenCompraPosicionNumero;
        //        ENTRYSHEETHEADER.DOC_DATE = parametros.EntrySheetHeader.FechaDocumento;
        //        ENTRYSHEETHEADER.POST_DATE = parametros.EntrySheetHeader.FechaContabilizacion;
        //        //ENTRYSHEETHEADER.SHORT_TEXT = parametros.EntrySheetHeader.Descripcion;
        //        //ENTRYSHEETHEADER.PCKG_NO = parametros.EntrySheetHeader.PaqueteNumero;
        //        //ENTRYSHEETHEADER.REF_DOC_NO = parametros.EntrySheetHeader.DocumentoReferenciaNumero;

        //        // Mapeo de ENTRYSHEETSERVICES sin LinQ
        //        if (parametros.EntrySheetServices?.Items != null && parametros.EntrySheetServices.Items.Any())
        //        {
        //            ENTRYSHEETSERVICES = new BAPIESLLC[parametros.EntrySheetServices.Items.Count];

        //            int i = 0;
        //            foreach (var item in parametros.EntrySheetServices.Items)
        //            {
        //                ENTRYSHEETSERVICES[i] = new BAPIESLLC
        //                {
        //                    PCKG_NO = item.PackageNumber,
        //                    LINE_NO = item.LineNumber,
        //                    OUTL_IND = item.OutlineIndicator,
        //                    SUBPCKG_NO = item.SubPackageNumber,
        //                    EXT_LINE = item.ExternalLineNumber,
        //                    SERVICE = item.Service,
        //                    QUANTITY = Decimal.TryParse(item.Quantity, out decimal quantityValue) ? quantityValue : 0,
        //                    GR_PRICE = item.GrossPrice,
        //                    SHORT_TEXT = item.ShortText,
        //                    PLN_PCKG = item.PlannedPackage,
        //                    PLN_LINE = item.PlannedLine
        //                };
        //                i++;
        //            }
        //        }

        //        service.SI_MMRFC_BAPI_ENTRYSHEET_CREATE(ENTRYSHEETHEADER, NO_COMMIT, TESTRUN, ref ENTRYSHEETACCOUNTASSIGNMENT, ref ENTRYSHEETHEADERTEXT, ref ENTRYSHEETSERVICES, ref ENTRYSHEETSERVICESTEXTS, ref ENTRYSHEETSRVACCASSVALUES, ref RETURN);

        //        return Map(RETURN);
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}


        //private string Map(BAPIRET2[] RETURN)
        //{
        //    string result = string.Join(Environment.NewLine, RETURN.Select(r => r.MESSAGE));

        //    return result;
        //}
    }


    public interface ICrearEntradaDeServicioConsumerMOA
    {
    }
}

