using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Models.WSMapMOA.Pesificacion;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerCabecerasEntradaServicioWebServiceMOA;
using SustitucionMOAWS.ResponseHandler.EntradadeServicios;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MLBO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BAPIESSR = SustitucionMOAWS.ObtenerCabecerasEntradaServicioWebServiceMOA.BAPIESSR;
using BAPIRETURN = SustitucionMOAWS.ObtenerCabecerasEntradaServicioWebServiceMOA.BAPIRETURN;
using BAPIRETURN1 = SustitucionMOAWS.ObtenerCabecerasEntradaServicioWebServiceMOA.BAPIRETURN1;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerCabecerasEntradaServicioConsumerMOA : IObtenerCabecerasEntradaServicioConsumerMOA
    {
        // Obtiene cabeceras de entradas de servicios desde unra Fecha dada
        //private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];


        public ObtenerCabecerasEntradaServicioConsumerMOA()
        {

            //this.repositorio = repositorio;
        }

        public async Task<List<EntradaServicioCabeceraDto>> ObtenerEntradasServicioCabeceraAsync(string fechaDesde)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_BAPI_DIRECT_MLBOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new BAPI_ENTRYSHEET_GETLIST()
                    {
                         ENTRYSHEET_DATE = fechaDesde
                    };
                    Log.Info($"SAP sin PI BAPI_ENTRYSHEET_GETLIST request");
                    Log.Info(request.ToXml());
                    var response = agent.BAPI_ENTRYSHEET_GETLIST(request);
                    Log.Info($"SAP sin PI BAPI_ENTRYSHEET_GETLIST response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_BAPI_ENTRYSHEET_GETLISTClient service;
                    service = new SI_MMRFC_BAPI_ENTRYSHEET_GETLISTClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string _UrlServicio = SAPCredential.DevolverEndpoint(System.Configuration.ConfigurationManager.AppSettings["ServicioSAPEntradasServicioCabecera"]).ToString();
                    string _SOAPAction = System.Configuration.ConfigurationManager.AppSettings["SOAPAction"];

                    string Authorization = service.ClientCredentials.UserName.UserName + ":" + service.ClientCredentials.UserName.Password;
                    byte[] userNameBytes = System.Text.Encoding.UTF8.GetBytes(Authorization);
                    string authorizationBase64 = System.Convert.ToBase64String(userNameBytes);

                    string _Authorization = "Basic " + authorizationBase64;
                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _UrlServicio);
                    request.Headers.Add("SOAPAction", _SOAPAction);
                    request.Headers.Add("Authorization", _Authorization);

                    StringContent content = new StringContent(
                        $@"<?xml version=""1.0"" encoding=""utf-8""?>
                       <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                           <soap:Header>
                               <wsse:Security soap:mustUnderstand=""1"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd""/>
                           </soap:Header>
                           <soap:Body>
                               <BAPI_ENTRYSHEET_GETLIST xmlns=""urn:sap-com:document:sap:rfc:functions"">
                                   <ENTRYSHEET_DATE>{fechaDesde}</ENTRYSHEET_DATE>  
                               </BAPI_ENTRYSHEET_GETLIST>
                           </soap:Body>
                       </soap:Envelope>",
                        Encoding.UTF8,
                        "text/xml"
                    );

                    content.Headers.ContentType.CharSet = "utf-8"; // Establecer el conjunto de caracteres

                    request.Content = content;
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string xmlString = await response.Content.ReadAsStringAsync();

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlString);
                        var levelObject = xmlDoc.DocumentElement.ChildNodes[1].ChildNodes[0].ChildNodes[0];

                        XmlSerializer serializer = new XmlSerializer(typeof(EntradaServicioCabeceraConsumerDto), "");

                        List<EntradaServicioCabeceraConsumerDto> resultList = new List<EntradaServicioCabeceraConsumerDto>();

                        foreach (XmlNode itemNode in levelObject)
                        {
                            string wrappedXmlString = $"<item>{itemNode.InnerXml}</item>";
                            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(wrappedXmlString)))
                            {
                                EntradaServicioCabeceraConsumerDto data = (EntradaServicioCabeceraConsumerDto)serializer.Deserialize(ms);
                                resultList.Add(data); // Agrega el objeto deserializado a la lista
                            }
                        }
                        return Map(resultList);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        private List<EntradaServicioCabeceraDto> Map(List<EntradaServicioCabeceraConsumerDto> Cabeceras)
        {
            List<EntradaServicioCabeceraDto> result = new List<EntradaServicioCabeceraDto>();
            //IEnumerable<BAPIESSR> cabecerasFiltradas = ENTRYSHEET_HEADER.Where(a => a.ACCEPTANCE != "X");

            //List<ItemEntradaServicioDto> items = new List<ItemEntradaServicioDto>();
            foreach (var cabecera in Cabeceras)
            {
                var cabe = new EntradaServicioCabeceraDto();
                DateTime fechaCreacion;
                DateTime.TryParseExact(cabecera.CREATED_ON, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaCreacion);

                cabe.EntradaServicio = cabecera.SHEET_NO;
                cabe.FechaCreacionDateTime = fechaCreacion;
                cabe.FechaCreacion = cabecera.CREATED_ON;
                cabe.OrdenCompra = cabecera.PO_NUMBER;
                //cabe.Proveedor = cabecera.PERSON_EXT; //Nombre del proveedor en la oc
                cabe.Descripcion = cabecera.SHORT_TEXT;
                cabe.MontoTotal = cabecera.GROSS_VAL;
                cabe.Ingresante = cabecera.CREATED_BY;

                // Momentaneo mientras se encontra la forma de buscar el aprobador o fiscal de la ES de sap
                cabe.Aprobador = "-"; 
                cabe.Fiscal = "-";
                cabe.DesdeSap = true;
                cabe.Moneda = cabecera.CURRENCY;
                result.Add(cabe);
            }
            return result;
        }

        private List<EntradaServicioCabeceraDto> MapSinPI(BAPI_ENTRYSHEET_GETLISTResponse Cabeceras)
        {
            List<EntradaServicioCabeceraDto> result = new List<EntradaServicioCabeceraDto>();
            //IEnumerable<BAPIESSR> cabecerasFiltradas = ENTRYSHEET_HEADER.Where(a => a.ACCEPTANCE != "X");

            //List<ItemEntradaServicioDto> items = new List<ItemEntradaServicioDto>();
            foreach (var cabecera in Cabeceras.ENTRYSHEET_HEADER)
            {
                var cabe = new EntradaServicioCabeceraDto();
                DateTime fechaCreacion;
                DateTime.TryParseExact(cabecera.CREATED_ON, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaCreacion);

                cabe.EntradaServicio = cabecera.SHEET_NO;
                cabe.FechaCreacionDateTime = fechaCreacion;
                cabe.FechaCreacion = cabecera.CREATED_ON;
                cabe.OrdenCompra = cabecera.PO_NUMBER;
                //cabe.Proveedor = cabecera.PERSON_EXT; //Nombre del proveedor en la oc
                cabe.Descripcion = cabecera.SHORT_TEXT;
                cabe.MontoTotal = cabecera.GROSS_VAL.ToString();
                cabe.Ingresante = cabecera.CREATED_BY;

                // Momentaneo mientras se encontra la forma de buscar el aprobador o fiscal de la ES de sap
                cabe.Aprobador = "-";
                cabe.Fiscal = "-";
                cabe.DesdeSap = true;
                cabe.Moneda = cabecera.CURRENCY;
                result.Add(cabe);
            }
            return result;
        }

    }

    public interface IObtenerCabecerasEntradaServicioConsumerMOA
    {
    }
}






//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Original
//    XElement xml = XElement.Parse(xmlString);
//    var ENTRYSHEET_HEADER = xml.Element("ENTRYSHEET_HEADER").ToXml();
//    Stream xmlStream = await response.Content.ReadAsStreamAsync();
//    XmlSerializer serializer = new XmlSerializer(typeof(EntradasdeServicioResponse));
//    EntradasdeServicioResponse sapResponse = (EntradasdeServicioResponse)serializer.Deserialize(xmlStream);
//    // Hacer algo con sapResponse, como trabajar con sus propiedades
//}
//else
//{
//    //Console.WriteLine("La solicitud al servicio SAP falló.");
//}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Todo el try es el original que utiliza servicio referenciado.
/// Pero no funciona correctamente, siempre devuelve null
/// Si se asume que nunca va a funcionar referenciado, se puede eliminar todo el bloque try de más abajo
//try
//{
//    string DOC_DATE = "";
//    string DOC_TYPE = "";
//    string ENTRYSHEET_DATE = fechaDesde;
//    string MAT_GRP = "";
//    string PLANT = "";
//    string PO_NUMBER = "";
//    string PURCH_ORG = "";
//    string PUR_GROUP = "";
//    string REL_CODE = "";
//    string REL_GROUP = "";
//    string VENDOR = "";
//    BAPIESSR[] ENTRYSHEET_HEADER = new BAPIESSR[] { };
//    BAPIRETURN1[] RETURN = new BAPIRETURN1[] { };
//    BAPIEINE[] INFORECORD_PURCHORG = new BAPIEINE[] { };
//    BAPISEGM[] INFORECORD_SEGMENT = new BAPISEGM[] { };
//    BAPIRETURN[] RETURN1 = new BAPIRETURN[] { };




//    BAPIEINA[] resultado = service.SI_MMRFC_BAPI_ENTRYSHEET_GETLIST(DOC_DATE, DOC_TYPE, ENTRYSHEET_DATE, MAT_GRP, PLANT, PO_NUMBER, PURCH_ORG, PUR_GROUP, REL_CODE, REL_GROUP, VENDOR, ENTRYSHEET_HEADER, RETURN, out INFORECORD_PURCHORG, out INFORECORD_SEGMENT, out RETURN1);

//    List<EntradaServicioCabeceraMocDto> CabecerasMoc = new List<EntradaServicioCabeceraMocDto>
//                {
//                    new EntradaServicioCabeceraMocDto{SHEET_NO = "1001457063",EXT_NUMBER = "",CREATED_BY = "GWEBSRV_GOL",CREATED_ON = "2023-11-14",CH_ON = "2023-11-14",CHANGED_BY = "GWEBSRV_GOL",PERSON_INT = "",PERSON_EXT = "",LOCATION = "",REF_DATE = "2023-11-14",BEGDATE = "0000-00-00",ENDDATE = "0000-00-00",GROSS_VAL = "3699.7900",UNPL_VAL = "0.0000",UNPLC_VAL = "0.0000",CURRENCY = "ARP",CURR_ISOCD = "ARP",PCKG_NO = "0003193069",SHORT_TEXT = "andamio",PO_NUMBER = "4123001549",PO_ITEM = "00001",DELETE_IND = "",ACCEPTANCE = "X",FIN_ENTRY = "",REL_GROUP = "",REL_STRAT = "",REL_IND = "",REL_STATUS = "",SUBJ_TO_R = "",BLOCK_IND = "",SCORE_TIME = "0",SCORE_QUAL = "0",DOC_DATE = "2023-11-14",POST_DATE = "2023-11-14",REF_DOC_NO = "",ACCASSCAT = "F",NET_VALUE = "3699.7900",PREQ_NO = "",PREQ_ITEM = "00000",MAINTPLAN = "",MAINTITEM = "",CALL_NO = "0",FRCO_DOC = "",FRCO_ITEM = "000000",COMM_NO = "",USER_FIELD = "",REF_DOC_NO_LONG = "",EXT_NUMBER_LONG = ""},
//                    new EntradaServicioCabeceraMocDto{SHEET_NO = "1001457066",EXT_NUMBER = "",CREATED_BY = "RFC_MOAOP",CREATED_ON = "2023-11-16",CH_ON = "2023-11-16",CHANGED_BY = "SAPMP",PERSON_INT = "",PERSON_EXT = "",LOCATION = "",REF_DATE = "2023-11-16",BEGDATE = "0000-00-00",ENDDATE = "0000-00-00",GROSS_VAL = "0.0000",UNPL_VAL = "0.0000",UNPLC_VAL = "0.0000",CURRENCY = "ARP",CURR_ISOCD = "ARP",PCKG_NO = "0000000000",SHORT_TEXT = "Desc ES",PO_NUMBER = "4123001549",PO_ITEM = "00001",DELETE_IND = "",ACCEPTANCE = "",FIN_ENTRY = "",REL_GROUP = "",REL_STRAT = "",REL_IND = "",REL_STATUS = "",SUBJ_TO_R = "",BLOCK_IND = "",SCORE_TIME = "0",SCORE_QUAL = "0",DOC_DATE = "2023-11-02",POST_DATE = "2023-11-13",REF_DOC_NO = "",ACCASSCAT = "F",NET_VALUE = "0.0000",PREQ_NO = "",PREQ_ITEM = "00000",MAINTPLAN = "",MAINTITEM = "",CALL_NO = "0",FRCO_DOC = "",FRCO_ITEM = "000000",COMM_NO = "",USER_FIELD = "",REF_DOC_NO_LONG = "",EXT_NUMBER_LONG = ""},

//                };

//    return Map(CabecerasMoc, ENTRYSHEET_HEADER);
//}
//catch (Exception e)
//{
//    throw e;
//}
