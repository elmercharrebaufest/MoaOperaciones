using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ModificarOCWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using static SustitucionMOAWS.WSConsumers.ModificarOrdenDeCompraConsumerMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class ModificarOrdenDeCompraConsumerMOA : IModificarOrdenDeCompraConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly ObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraconsumerMOA;
        private readonly IRepositorio repositorio;

        public ModificarOrdenDeCompraConsumerMOA(IRepositorio repositorio)
        {
            obtenerOrdenDeCompraconsumerMOA = new ObtenerOrdenDeCompraConsumerMOA(repositorio);
            this.repositorio = repositorio;
        }

        public CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion)
        {
            var respuesta = new CrearPedidoConsumerMOAResponse();

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                ModificarPedidoSAPSinPI modificarPedidoSAP = ConvertirAdjudicacionSinPI(adjudicacion);
                WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2[] result = EditarPedidoRequestSinPI(modificarPedidoSAP);

                respuesta.NumeroPedido = adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.NroOrdenDeCompraAdicional;
                respuesta.Resultado = "";

                respuesta.Errores = new List<CrearPedidoConsumerMOAError>();

                foreach (var errorSAP in result.Where(e => e.TYPE == "E"))
                {
                    var error = new CrearPedidoConsumerMOAError
                    {
                        Codigo = errorSAP.FIELD,
                        Mensaje = errorSAP.MESSAGE,
                        Tipo = errorSAP.TYPE
                    };

                    respuesta.Errores.Add(error);
                }
            }
            else
            {
                ModificarPedidoSAP modificarPedidoSAP = ConvertirAdjudicacion(adjudicacion);
                ModificarOCWebServiceMOA.BAPIRET2[] result = EditarPedidoRequest(modificarPedidoSAP);

                respuesta.NumeroPedido = adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.NroOrdenDeCompraAdicional;
                respuesta.Resultado = "";

                respuesta.Errores = new List<CrearPedidoConsumerMOAError>();

                foreach (var errorSAP in result.Where(e => e.TYPE == "E"))
                {
                    var error = new CrearPedidoConsumerMOAError
                    {
                        Codigo = errorSAP.FIELD,
                        Mensaje = errorSAP.MESSAGE,
                        Tipo = errorSAP.TYPE
                    };

                    respuesta.Errores.Add(error);
                }
            }



            return respuesta;
        }

        public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2[] EditarPedidoRequestSinPI(ModificarPedidoSAPSinPI modificarPedidoSAP)
        {
            var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var serxml = new System.Xml.Serialization.XmlSerializer(modificarPedidoSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, modificarPedidoSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());
            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            var nombreArchivoLlamada = string.Concat(modificarPedidoSAP.PURCHASEORDER, " - ", fecha, " - modificar OC.xml");
            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = modificarPedidoSAP.ALLVERSIONS?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIPAREX[] EXTENSIONIN = modificarPedidoSAP.EXTENSIONIN?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIPAREX[] EXTENSIONOUT = modificarPedidoSAP.EXTENSIONOUT?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = modificarPedidoSAP.INVPLANHEADER?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_HEADERX[] INVPLANHEADERX = modificarPedidoSAP.INVPLANHEADERX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = modificarPedidoSAP.INVPLANITEM?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_ITEMX[] INVPLANITEMX = modificarPedidoSAP.INVPLANITEMX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT[] POACCOUNT = modificarPedidoSAP.POACCOUNT?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTPROFITSEGMENT[] POACCOUNTPROFITSEGMENT = modificarPedidoSAP.POACCOUNTPROFITSEGMENT?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX[] POACCOUNTX = modificarPedidoSAP.POACCOUNTX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY = modificarPedidoSAP.POADDRDELIVERY?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOMPONENT[] POCOMPONENTS = modificarPedidoSAP.POCOMPONENTS?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOMPONENTX[] POCOMPONENTSX = modificarPedidoSAP.POCOMPONENTSX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND[] POCOND = modificarPedidoSAP.POCOND?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADER[] POCONDHEADER = modificarPedidoSAP.POCONDHEADER?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADERX[] POCONDHEADERX = modificarPedidoSAP.POCONDHEADERX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX[] POCONDX = modificarPedidoSAP.POCONDX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKES[] POCONFIRMATION = modificarPedidoSAP.POCONFIRMATION?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESUCC[] POCONTRACTLIMITS = modificarPedidoSAP.POCONTRACTLIMITS?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIPO[] POEXPIMPITEM = modificarPedidoSAP.POEXPIMPITEM?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIPOX[] POEXPIMPITEMX = modificarPedidoSAP.POEXPIMPITEMX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBE[] POHISTORY = modificarPedidoSAP.POHISTORY?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBE_MA[] POHISTORY_MA = modificarPedidoSAP.POHISTORY_MA?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBES[] POHISTORY_TOTALS = modificarPedidoSAP.POHISTORY_TOTALS?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM[] POITEM = modificarPedidoSAP.POITEM?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX[] POITEMX = modificarPedidoSAP.POITEMX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESUHC[] POLIMITS = modificarPedidoSAP.POLIMITS?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKKOP[] POPARTNER = modificarPedidoSAP.POPARTNER?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE[] POSCHEDULE = modificarPedidoSAP.POSCHEDULE?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX[] POSCHEDULEX = modificarPedidoSAP.POSCHEDULEX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC[] POSERVICES = modificarPedidoSAP.POSERVICES?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLTX[] POSERVICESTEXT = modificarPedidoSAP.POSERVICESTEXT?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIITEMSHIP[] POSHIPPING = modificarPedidoSAP.POSHIPPING?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = modificarPedidoSAP.POSHIPPINGEXP?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIITEMSHIPX[] POSHIPPINGX = modificarPedidoSAP.POSHIPPINGX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC[] POSRVACCESSVALUES = modificarPedidoSAP.POSRVACCESSVALUES?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER[] POTEXTHEADER = modificarPedidoSAP.POTEXTHEADER?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT[] POTEXTITEM = modificarPedidoSAP.POTEXTITEM?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2[] RETURN = modificarPedidoSAP.RETURN?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSERIALNO[] SERIALNUMBER = modificarPedidoSAP.SERIALNUMBER?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSERIALNOX[] SERIALNUMBERX = modificarPedidoSAP.SERIALNUMBERX?.ToArray();
            WS_GAQ_sin_PI_DIRECT_COMPRAS._NFM_BAPIDOCITM[] NFMETALLITMS = modificarPedidoSAP.NFMETALLITMS?.ToArray();

            var request = new Z_MMRFC_MODIFICAR_OC()
            {
                IM_URL = modificarPedidoSAP.IM_URL ?? "",
                MEMORY_COMPLETE = modificarPedidoSAP.MEMORY_COMPLETE,
                MEMORY_UNCOMPLETE = modificarPedidoSAP.MEMORY_UNCOMPLETE,
                NO_AUTHORITY = modificarPedidoSAP.NO_AUTHORITY,
                NO_MESSAGE_REQ = modificarPedidoSAP.NO_MESSAGE_REQ,
                NO_MESSAGING = modificarPedidoSAP.NO_MESSAGING,
                NO_PRICE_FROM_PO = modificarPedidoSAP.NO_PRICE_FROM_PO,
                PARK_COMPLETE = modificarPedidoSAP.PARK_COMPLETE,
                PARK_UNCOMPLETE = modificarPedidoSAP.PARK_UNCOMPLETE,
                POADDRVENDOR = modificarPedidoSAP.POADDRVENDOR,
                POEXPIMPHEADER = modificarPedidoSAP.POEXPIMPHEADER,
                POEXPIMPHEADERX = modificarPedidoSAP.POEXPIMPHEADERX,
                POHEADER = modificarPedidoSAP.POHEADER,
                POHEADERX = modificarPedidoSAP.POHEADERX,
                PURCHASEORDER = modificarPedidoSAP.PURCHASEORDER,
                TESTRUN = modificarPedidoSAP.TESTRUN,
                VERSIONS = modificarPedidoSAP.VERSIONS,
                ALLVERSIONS = ALLVERSIONS,
                EXTENSIONIN = EXTENSIONIN,
                EXTENSIONOUT = EXTENSIONOUT,
                INVPLANHEADER = INVPLANHEADER,
                INVPLANHEADERX = INVPLANHEADERX,
                INVPLANITEM = INVPLANITEM,
                INVPLANITEMX = INVPLANITEMX,
                POACCOUNT = POACCOUNT,
                POACCOUNTPROFITSEGMENT = POACCOUNTPROFITSEGMENT,
                POACCOUNTX = POACCOUNTX,
                POADDRDELIVERY = POADDRDELIVERY,
                POCOMPONENTS = POCOMPONENTS,
                POCOMPONENTSX = POCOMPONENTSX,
                POCOND = POCOND,
                POCONDHEADER = POCONDHEADER,
                POCONDHEADERX = POCONDHEADERX,
                POCONDX = POCONDX,
                POCONFIRMATION = POCONFIRMATION,
                POCONTRACTLIMITS = POCONTRACTLIMITS,
                POEXPIMPITEM = POEXPIMPITEM,
                POEXPIMPITEMX = POEXPIMPITEMX,
                POHISTORY = POHISTORY,
                POHISTORY_MA = POHISTORY_MA,
                POHISTORY_TOTALS = POHISTORY_TOTALS,
                POITEM = POITEM,
                POITEMX = POITEMX,
                POLIMITS = POLIMITS,
                POPARTNER = POPARTNER,
                POSCHEDULE = POSCHEDULE,
                POSCHEDULEX = POSCHEDULEX,
                POSERVICES = POSERVICES,
                POSERVICESTEXT = POSERVICESTEXT,
                POSHIPPING = POSHIPPING,
                POSHIPPINGEXP = POSHIPPINGEXP,
                POSHIPPINGX = POSHIPPINGX,
                POSRVACCESSVALUES = POSRVACCESSVALUES,
                POTEXTHEADER = POTEXTHEADER,
                POTEXTITEM = POTEXTITEM,
                RETURN = RETURN,
                SERIALNUMBER = SERIALNUMBER,
                SERIALNUMBERX = SERIALNUMBERX,
                NFMETALLITMS = NFMETALLITMS,
            };
            Log.Info($"SAP sin PI Z_MMRFC_MODIFICAR_OC request");
            Log.Info(request.ToXml());

            var response = agent.Z_MMRFC_MODIFICAR_OC(request);
            Log.Info($"SAP sin PI Z_MMRFC_MODIFICAR_OC response");
            Log.Info(response.ToXml());

            serxml = new System.Xml.Serialization.XmlSerializer(response.RETURN.GetType());
            ms = new MemoryStream();
            serxml.Serialize(ms, response.RETURN);
            xml = Encoding.UTF8.GetString(ms.ToArray());
            using (StreamWriter writer = File.AppendText(rutaArchivoLlamada))
            {
                writer.WriteLine(xml);
            }

            return response.RETURN;

        }
        public ModificarOCWebServiceMOA.BAPIRET2[] EditarPedidoRequest(ModificarPedidoSAP modificarPedidoSAP)
        {
            SI_MMRFC_MODIFICAR_OCClient service;
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_MODIFICAR_OC&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_MODIFICAR_OCClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            var serxml = new System.Xml.Serialization.XmlSerializer(modificarPedidoSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, modificarPedidoSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(modificarPedidoSAP.PURCHASEORDER, " - ", fecha, " - modificar OC.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            ModificarOCWebServiceMOA.BAPIMEDCM_ALLVERSIONS[] ALLVERSIONS = modificarPedidoSAP.ALLVERSIONS?.ToArray();
            ModificarOCWebServiceMOA.BAPIPAREX[] EXTENSIONIN = modificarPedidoSAP.EXTENSIONIN?.ToArray();
            ModificarOCWebServiceMOA.BAPIPAREX[] EXTENSIONOUT = modificarPedidoSAP.EXTENSIONOUT?.ToArray();
            ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_HEADER[] INVPLANHEADER = modificarPedidoSAP.INVPLANHEADER?.ToArray();
            ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_HEADERX[] INVPLANHEADERX = modificarPedidoSAP.INVPLANHEADERX?.ToArray();
            ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_ITEM[] INVPLANITEM = modificarPedidoSAP.INVPLANITEM?.ToArray();
            ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_ITEMX[] INVPLANITEMX = modificarPedidoSAP.INVPLANITEMX?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOACCOUNT[] POACCOUNT = modificarPedidoSAP.POACCOUNT?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOACCOUNTPROFITSEGMENT[] POACCOUNTPROFITSEGMENT = modificarPedidoSAP.POACCOUNTPROFITSEGMENT?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOACCOUNTX[] POACCOUNTX = modificarPedidoSAP.POACCOUNTX?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOADDRDELIVERY[] POADDRDELIVERY = modificarPedidoSAP.POADDRDELIVERY?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOCOMPONENT[] POCOMPONENTS = modificarPedidoSAP.POCOMPONENTS?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOCOMPONENTX[] POCOMPONENTSX = modificarPedidoSAP.POCOMPONENTSX?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOCOND[] POCOND = modificarPedidoSAP.POCOND?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOCONDHEADER[] POCONDHEADER = modificarPedidoSAP.POCONDHEADER?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOCONDHEADERX[] POCONDHEADERX = modificarPedidoSAP.POCONDHEADERX?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOCONDX[] POCONDX = modificarPedidoSAP.POCONDX?.ToArray();
            ModificarOCWebServiceMOA.BAPIEKES[] POCONFIRMATION = modificarPedidoSAP.POCONFIRMATION?.ToArray();
            ModificarOCWebServiceMOA.BAPIESUCC[] POCONTRACTLIMITS = modificarPedidoSAP.POCONTRACTLIMITS?.ToArray();
            ModificarOCWebServiceMOA.BAPIEIPO[] POEXPIMPITEM = modificarPedidoSAP.POEXPIMPITEM?.ToArray();
            ModificarOCWebServiceMOA.BAPIEIPOX[] POEXPIMPITEMX = modificarPedidoSAP.POEXPIMPITEMX?.ToArray();
            ModificarOCWebServiceMOA.BAPIEKBE[] POHISTORY = modificarPedidoSAP.POHISTORY?.ToArray();
            ModificarOCWebServiceMOA.BAPIEKBE_MA[] POHISTORY_MA = modificarPedidoSAP.POHISTORY_MA?.ToArray();
            ModificarOCWebServiceMOA.BAPIEKBES[] POHISTORY_TOTALS = modificarPedidoSAP.POHISTORY_TOTALS?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOITEM[] POITEM = modificarPedidoSAP.POITEM?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOITEMX[] POITEMX = modificarPedidoSAP.POITEMX?.ToArray();
            ModificarOCWebServiceMOA.BAPIESUHC[] POLIMITS = modificarPedidoSAP.POLIMITS?.ToArray();
            ModificarOCWebServiceMOA.BAPIEKKOP[] POPARTNER = modificarPedidoSAP.POPARTNER?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOSCHEDULE[] POSCHEDULE = modificarPedidoSAP.POSCHEDULE?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOSCHEDULX[] POSCHEDULEX = modificarPedidoSAP.POSCHEDULEX?.ToArray();
            ModificarOCWebServiceMOA.BAPIESLLC[] POSERVICES = modificarPedidoSAP.POSERVICES?.ToArray();
            ModificarOCWebServiceMOA.BAPIESLLTX[] POSERVICESTEXT = modificarPedidoSAP.POSERVICESTEXT?.ToArray();
            ModificarOCWebServiceMOA.BAPIITEMSHIP[] POSHIPPING = modificarPedidoSAP.POSHIPPING?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = modificarPedidoSAP.POSHIPPINGEXP?.ToArray();
            ModificarOCWebServiceMOA.BAPIITEMSHIPX[] POSHIPPINGX = modificarPedidoSAP.POSHIPPINGX?.ToArray();
            ModificarOCWebServiceMOA.BAPIESKLC[] POSRVACCESSVALUES = modificarPedidoSAP.POSRVACCESSVALUES?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOTEXTHEADER[] POTEXTHEADER = modificarPedidoSAP.POTEXTHEADER?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOTEXT[] POTEXTITEM = modificarPedidoSAP.POTEXTITEM?.ToArray();
            ModificarOCWebServiceMOA.BAPIRET2[] RETURN = modificarPedidoSAP.RETURN?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOSERIALNO[] SERIALNUMBER = modificarPedidoSAP.SERIALNUMBER?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOSERIALNOX[] SERIALNUMBERX = modificarPedidoSAP.SERIALNUMBERX?.ToArray();

            ModificarOCWebServiceMOA._NFM_BAPIDOCITM[] NFMETALLITMS = modificarPedidoSAP.NFMETALLITMS?.ToArray();
            ModificarOCWebServiceMOA.BAPIMEPOHEADER result = service.SI_MMRFC_MODIFICAR_OC(
                modificarPedidoSAP.IM_URL ?? "",
                modificarPedidoSAP.MEMORY_COMPLETE,
                modificarPedidoSAP.MEMORY_UNCOMPLETE,
                modificarPedidoSAP.NO_AUTHORITY,
                modificarPedidoSAP.NO_MESSAGE_REQ,
                modificarPedidoSAP.NO_MESSAGING,
                modificarPedidoSAP.NO_PRICE_FROM_PO,
                modificarPedidoSAP.PARK_COMPLETE,
                modificarPedidoSAP.PARK_UNCOMPLETE,
                modificarPedidoSAP.POADDRVENDOR,
                modificarPedidoSAP.POEXPIMPHEADER,
                modificarPedidoSAP.POEXPIMPHEADERX,
                modificarPedidoSAP.POHEADER,
                modificarPedidoSAP.POHEADERX,
                modificarPedidoSAP.PURCHASEORDER,
                modificarPedidoSAP.TESTRUN,
                 modificarPedidoSAP.VERSIONS,
                 ref ALLVERSIONS,
                 ref EXTENSIONIN,
                 ref EXTENSIONOUT,
                 ref INVPLANHEADER,
                 ref INVPLANHEADERX,
                 ref INVPLANITEM,
                 ref INVPLANITEMX,
                 ref NFMETALLITMS,
                 ref POACCOUNT,
                 ref POACCOUNTPROFITSEGMENT,
                 ref POACCOUNTX,
                 ref POADDRDELIVERY,
                 ref POCOMPONENTS,
                 ref POCOMPONENTSX,
                 ref POCOND,
                 ref POCONDHEADER,
                 ref POCONDHEADERX,
                 ref POCONDX,
                 ref POCONFIRMATION,
                 ref POCONTRACTLIMITS,
                 ref POEXPIMPITEM,
                 ref POEXPIMPITEMX,
                 ref POHISTORY,
                 ref POHISTORY_MA,
                 ref POHISTORY_TOTALS,
                 ref POITEM,
                 ref POITEMX,
                 ref POLIMITS,
                 ref POPARTNER,
                 ref POSCHEDULE,
                 ref POSCHEDULEX,
                 ref POSERVICES,
                 ref POSERVICESTEXT,
                 ref POSHIPPING,
                 ref POSHIPPINGEXP,
                 ref POSHIPPINGX,
                 ref POSRVACCESSVALUES,
                 ref POTEXTHEADER,
                 ref POTEXTITEM,
                 ref RETURN,
                 ref SERIALNUMBER,
                 ref SERIALNUMBERX,
                 out ModificarOCWebServiceMOA.BAPIEIKP EXPPOEXPIMPHEADER
            );

            serxml = new System.Xml.Serialization.XmlSerializer(RETURN.GetType());
            ms = new MemoryStream();
            serxml.Serialize(ms, RETURN);
            xml = Encoding.UTF8.GetString(ms.ToArray());
            using (StreamWriter writer = File.AppendText(rutaArchivoLlamada))
            {
                writer.WriteLine(xml);
            }

            return RETURN;
        }

        private ModificarPedidoSAP ConvertirAdjudicacion(Adjudicacion adjudicacion)
        {
            //TODO: Crear OC ConvertirSOLP - fields hardcodeados o para revisar
            ///DOC_TYPE  ok por ahora. Clase de documento de compras / Estrategia de liberacion hardcore ZPE1 

            ///STREET y STREET_NO ok. no tenemos el campo separado mandamos todo en street            
            ///SERIAL_NO/serialNumber siempre 1 por que se imputa todo a lo mismo sino son imputaciones multiples, en ese caso analizar como se envia.

            var modificarPedidoSAP = new ModificarPedidoSAP
            {
                PURCHASEORDER = adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.NroOrdenDeCompraAdicional
            };
            string poItem = "";
            string numeroDeImputacion = "";
            var PCKG_NO = 1000;
            var numeroDePaquete = 1;

            var ocSAP = obtenerOrdenDeCompraconsumerMOA.ObtenerOrdenDeCompra(modificarPedidoSAP.PURCHASEORDER);
            int nroItemPO = ocSAP.Posiciones.Max(a => a.NumeroItemOC);

            bool esPosicionDeMateriales = adjudicacion.Posiciones.FirstOrDefault().Posicion.TipoPosicion.Codigo == "MATERIALES";

            var unidadesCodigoSap = adjudicacion.Posiciones.SelectMany(p => new[] { p.Posicion.Unidad?.CodigoSap }.Concat(p.Posicion.Subposiciones.Select(sp => sp.Unidad.CodigoSap))).Distinct();

            var unidadesMedidaSap = repositorio.Listar<UnidadMedidaSap, dynamic>(x => new { x.Comercial, x.UM },
                x => unidadesCodigoSap.Contains(x.Comercial))?.Select(x => System.Tuple.Create(x.Comercial, x.UM)).ToList();

            List<int> idsPosiciones = adjudicacion.Posiciones.Select(x => x.SolpPosicion_Id).ToList();
            var posicionesSolp = repositorio.Listar<SolpPosicion>(posi => idsPosiciones.Contains(posi.Id));
            foreach (var posicion in posicionesSolp)
            {
                nroItemPO += 1;
                var adjudicacionPosicion = adjudicacion.Posiciones.Single(a => a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == posicion.Id);

                numeroDePaquete++;
                numeroDeImputacion = "01";// SERIAL_NO por ahora siempre 01 por que no hay imputaciones multiples
                poItem = $"{nroItemPO:00000}";

                //Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
                var IM_POITEM = new ModificarOCWebServiceMOA.BAPIMEPOITEM
                {
                    PO_ITEM = poItem,
                    SHORT_TEXT = posicion.Tarea,
                    PLANT = posicion.Centro.CodigoSap.ToString(),
                    MATL_GROUP = posicion.GrupoArticulo?.CodigoSap?.ToString() ?? "",
                    MATERIAL = esPosicionDeMateriales ? posicion.MaterialSolp?.CodigoSap.ToString() : "",
                    STGE_LOC = posicion.Almacen != null ? posicion.Almacen.CodigoSap.ToString() : "",
                    ITEM_CAT = posicion.TipoPosicion.Codigo.ToLower() == "servicio" ? "9" : "0",//ITEM_CAT PSTYP   Tipo de posición del documento de compras
                    TRACKINGNO = posicion.NroNecesidad,
                    INFO_REC = "",
                    QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0,
                    QUANTITYSpecified = esPosicionDeMateriales,
                    PO_UNIT = esPosicionDeMateriales ? unidadesMedidaSap?.Find(u => u.Item1 == adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.CodigoSap).Item2 : "001",
                    NET_PRICE = esPosicionDeMateriales ? (decimal)adjudicacionPosicion.CotizacionPosicion.Precio : CalcularPrecioBrutoServicio(posicion, adjudicacionPosicion),
                    NET_PRICESpecified = true,
                    PRICE_UNIT = 1,
                    PRICE_UNITSpecified = true,
                    GR_PR_TIME = 0,
                    DELETE_IND = "",
                    TAX_CODE = "",
                    VAL_TYPE = "",
                    NO_MORE_GR = "",
                    FINAL_INV = "",
                    DISTRIB = "",
                    PART_INV = "",
                    GR_IND = "",
                    GR_NON_VAL = "",
                    IR_IND = "",
                    FREE_ITEM = "",
                    GR_BASEDIV = "",
                    ACKN_REQD = "",
                    ACKNOWL_NO = "",
                    AGREEMENT = "",
                    AGMT_ITEM = "",
                    RFQ_NO = "",
                    RFQ_ITEM = "",
                    PREQ_NO = posicion.Solp.NroSolp,
                    PREQ_ITEM = $"{posicion.Indice ?? 0:00000}",
                    PCKG_NO = esPosicionDeMateriales ? "" : $"{numeroDePaquete:0000000000}"
                };
                //IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
                //IM_POITEM.GR_PR_TIMESpecified = true; 

                switch (posicion.TipoImputacion?.Codigo.ToLower())
                {
                    case "centrodecosto":
                        IM_POITEM.ACCTASSCAT = "K";
                        break;
                    case "ordendeot":
                        IM_POITEM.ACCTASSCAT = "F";
                        break;
                    case "ordendeinversion":
                        IM_POITEM.ACCTASSCAT = "F";
                        break;
                    case "siniestrobeneficio":
                        IM_POITEM.ACCTASSCAT = "Y";
                        break;
                }
                modificarPedidoSAP.POITEM.Add(IM_POITEM);

                modificarPedidoSAP.POITEMX.Add(new ModificarOCWebServiceMOA.BAPIMEPOITEMX
                {
                    PO_ITEM = poItem,
                    DELETE_IND = "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = string.IsNullOrEmpty(posicion.NroNecesidad) ? "" : "X",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = (IM_POITEM.QUANTITY == 0) ? "" : "X",
                    PO_UNIT = "X",
                    NET_PRICE = "X",
                    PRICE_UNIT = "X",
                    GR_PR_TIME = "",
                    TAX_CODE = "",
                    VAL_TYPE = "",
                    NO_MORE_GR = "",
                    FINAL_INV = "",
                    ITEM_CAT = "X",
                    ACCTASSCAT = (IM_POITEM.ACCTASSCAT != null) ? "X" : "",
                    DISTRIB = "",
                    PART_INV = "",
                    GR_IND = "",
                    GR_NON_VAL = "",
                    IR_IND = "",
                    FREE_ITEM = "",
                    GR_BASEDIV = "",
                    ACKN_REQD = "",
                    ACKNOWL_NO = "",
                    AGREEMENT = "",
                    AGMT_ITEM = "",
                    RFQ_NO = "",
                    RFQ_ITEM = "",
                    PREQ_NO = "X",
                    PREQ_ITEM = "X",
                    PCKG_NO = "X"
                });

                modificarPedidoSAP.POCOND.Add(new ModificarOCWebServiceMOA.BAPIMEPOCOND
                {
                    ITM_NUMBER = poItem,  //el número de ítem al que corresponda la condición
                    COND_TYPE = "ZP01",// siempre va el mismo dato
                    COND_VALUE = IM_POITEM.NET_PRICE, //el importe de la condición
                    COND_VALUESpecified = true,
                    CURRENCY = adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo,//moneda de la adjudicacion
                    CHANGE_ID = "U",// siempra va el mismo valor

                });
                modificarPedidoSAP.POCONDX.Add(new ModificarOCWebServiceMOA.BAPIMEPOCONDX
                {
                    ITM_NUMBER = poItem,
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                });

                //Nombre: ZBAPIMEPOACCOUNT IM_POACCOUNT Denominación:	Imputación
                if (esPosicionDeMateriales)
                {
                    var imputacion = new ModificarOCWebServiceMOA.BAPIMEPOACCOUNT
                    {
                        PO_ITEM = poItem,
                        SERIAL_NO = numeroDeImputacion,
                        GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, posicion),
                        QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0,
                        BUS_AREA = "GENE",
                        CO_AREA = "MOA",
                        COSTCENTER = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "centrodecosto" }),
                        ORDERID = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "ordendeot", "ordendeinversion" }),
                        PROFIT_CTR = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "siniestrobeneficio" }),
                        SUB_NUMBER = "",
                        ASSET_NO = "",
                        COSTOBJECT = "",
                        DELETE_IND = ""
                    };
                    imputacion.QUANTITYSpecified = imputacion.QUANTITY > 0;
                    modificarPedidoSAP.POACCOUNT.Add(imputacion);

                    modificarPedidoSAP.POACCOUNTX.Add(new ModificarOCWebServiceMOA.BAPIMEPOACCOUNTX
                    {
                        PO_ITEM = poItem,
                        SERIAL_NO = numeroDeImputacion,
                        DELETE_IND = "",
                        QUANTITY = "X",
                        GL_ACCOUNT = "X",
                        BUS_AREA = "X",
                        ASSET_NO = "",
                        SUB_NUMBER = "",
                        CO_AREA = "X",
                        COSTOBJECT = "",
                        COSTCENTER = (posicion.TipoImputacion?.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                        ORDERID = (posicion.TipoImputacion?.Codigo.ToLower() == "ordendeot" || posicion.TipoImputacion?.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                        PROFIT_CTR = (posicion.TipoImputacion?.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                    });
                }

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                modificarPedidoSAP.POADDRDELIVERY.Add(new ModificarOCWebServiceMOA.BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = poItem,
                    POSTL_COD1 = posicion.CpEntrega,
                    CITY = posicion.Centro.Descripcion,
                    ADDR_NO = "",
                    NAME = posicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = posicion.CalleEntrega,
                    STREET_NO = "",//no tenemos el campo separado en calle y altura
                    REGION = adjudicacion.RegionSap.CodigoSap
                });

                //subposiciones
                if (!esPosicionDeMateriales)
                {
                    var LINE_NO = 1;
                    //cabecera de subposiciones
                    var cabeceraSubPos = new ModificarOCWebServiceMOA.BAPIESLLC
                    {
                        PCKG_NO = $"{numeroDePaquete:0000000000}",
                        LINE_NO = $"{LINE_NO++:0000000000}",
                        OUTL_IND = "X",
                        OUTL_LEVEL = 0,
                        SUBPCKG_NO = $"{PCKG_NO:0000000000}",
                    };
                    modificarPedidoSAP.POSERVICES.Add(cabeceraSubPos);
                    var numeroImputacion = 0;

                    foreach (var subposicion in posicion.Subposiciones)
                    {
                        var cotizacionSubPosicion = adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones.Single(a => a.SolpSubPosicion_Id == subposicion.Id);

                        var subposicionSap = new ModificarOCWebServiceMOA.BAPIESLLC
                        {
                            PCKG_NO = $"{PCKG_NO:0000000000}",
                            LINE_NO = $"{LINE_NO:0000000000}",
                            EXT_LINE = $"{LINE_NO * 10:0000000000}",
                            SERVICE = subposicion.ServicioSolp?.Codigo,
                            SHORT_TEXT = subposicion.Tarea,
                            QUANTITY = cotizacionSubPosicion.Cantidad.Value,
                            QUANTITYSpecified = true,
                            BASE_UOM = unidadesMedidaSap.Find(u => u.Item1 == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).Item2,
                            UOM_ISO = unidadesMedidaSap.Find(u => u.Item1 == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).Item2,
                            PRICE_UNIT = 1,
                            PRICE_UNITSpecified = true,
                            GR_PRICE = cotizacionSubPosicion.Precio.Value,
                            GR_PRICESpecified = true
                        };
                        modificarPedidoSAP.POSERVICES.Add(subposicionSap);


                        if (!modificarPedidoSAP.POACCOUNT.Any(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.ORDERID == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.PROFIT_CTR == getCodigoTablaSap(subposicion.TipoImputacionSap)
                            ))
                        {

                            numeroImputacion++;
                            var imputacion = new ModificarOCWebServiceMOA.BAPIMEPOACCOUNT
                            {
                                PO_ITEM = $"{poItem:00000}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                                GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, posicion),
                                QUANTITY = subposicion.Cantidad.Value,
                                QUANTITYSpecified = true,
                                BUS_AREA = "GENE",
                                CO_AREA = "MOA",
                                COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ?
                                    getCodigoTablaSap(subposicion.TipoImputacionSap) : "",
                                ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ?
                                    getCodigoTablaSap(subposicion.TipoImputacionSap) : "",
                                PROFIT_CTR = getCodigoTablaSap(subposicion.TipoImputacionSap),
                                SUB_NUMBER = "",
                                ASSET_NO = "",
                                COSTOBJECT = "",
                                DELETE_IND = ""
                            };
                            modificarPedidoSAP.POACCOUNT.Add(imputacion);

                            modificarPedidoSAP.POACCOUNTX.Add(new ModificarOCWebServiceMOA.BAPIMEPOACCOUNTX
                            {
                                PO_ITEM = $"{poItem:00000}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                                DELETE_IND = "",
                                QUANTITY = "X",
                                GL_ACCOUNT = "X",
                                BUS_AREA = "X",
                                ASSET_NO = "",
                                SUB_NUMBER = "",
                                CO_AREA = "X",
                                COSTOBJECT = "",
                                COSTCENTER = (posicion.TipoImputacion?.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                                ORDERID = (posicion.TipoImputacion?.Codigo.ToLower() == "ordendeot" || posicion.TipoImputacion?.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                                PROFIT_CTR = (posicion.TipoImputacion?.Codigo.ToLower() == "siniestrobeneficio") ? "X" : "X"
                            });

                            var imputacionSubPos = new ModificarOCWebServiceMOA.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                            };
                            modificarPedidoSAP.POSRVACCESSVALUES.Add(imputacionSubPos);
                        }
                        else
                        {
                            var imputacionUsada = modificarPedidoSAP.POACCOUNT.FirstOrDefault(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.ORDERID == getCodigoTablaSap(subposicion.TipoImputacionSap) &&
                                x.PROFIT_CTR == getCodigoTablaSap(subposicion.TipoImputacionSap)
                                );
                            imputacionUsada.QUANTITY += subposicion.Cantidad.Value;

                            var imputacionSubPos = new ModificarOCWebServiceMOA.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{imputacionUsada.SERIAL_NO:00}",
                            };
                            modificarPedidoSAP.POSRVACCESSVALUES.Add(imputacionSubPos);
                        }
                    }
                    PCKG_NO++;
                }


                modificarPedidoSAP.POSCHEDULE.Add(new ModificarOCWebServiceMOA.BAPIMEPOSCHEDULE
                {
                    DELIVERY_DATE = adjudicacionPosicion.PlazoDeEntrega.ToString("dd.MM.yyyy"),
                    PO_ITEM = poItem,
                    SCHED_LINE = "1"
                });

                modificarPedidoSAP.POSCHEDULEX.Add(new ModificarOCWebServiceMOA.BAPIMEPOSCHEDULX
                {
                    DELIVERY_DATE = "X",
                    PO_ITEM = poItem,
                    SCHED_LINE = "1"
                });
            }

            var listaVaciaTexto = new string[] { "" };

            var textosDiccionario = new Dictionary<string, string[]>() {
                {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            };

            foreach (var grupos in textosDiccionario)
            {
                bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
                if (!todosVacios)
                {
                    foreach (var texto in grupos.Value)
                    {
                        modificarPedidoSAP.POTEXTHEADER.Add(new ModificarOCWebServiceMOA.BAPIMEPOTEXTHEADER
                        {
                            TEXT_ID = grupos.Key,
                            PO_NUMBER = "",
                            PO_ITEM = "0",
                            TEXT_FORM = "*",
                            TEXT_LINE = texto
                        });
                    }
                }

            }

            if (adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.Urgencia == true)
            {
                modificarPedidoSAP.POTEXTITEM.Add(new ModificarOCWebServiceMOA.BAPIMEPOTEXT
                {
                    TEXT_ID = "F12",
                    PO_NUMBER = "",
                    PO_ITEM = poItem,
                    TEXT_FORM = "*",
                    TEXT_LINE = "Urgencia"
                });
            }

            modificarPedidoSAP.IM_URL = ConfigurationManager.AppSettings["SpaUrl"] + "/verLegajoOrdenDeCompra/" + adjudicacion.Id + "/" + adjudicacion.Token;

            return modificarPedidoSAP;
        }
        private ModificarPedidoSAPSinPI ConvertirAdjudicacionSinPI(Adjudicacion adjudicacion)
        {
            //TODO: Crear OC ConvertirSOLP - fields hardcodeados o para revisar
            ///DOC_TYPE  ok por ahora. Clase de documento de compras / Estrategia de liberacion hardcore ZPE1 

            ///STREET y STREET_NO ok. no tenemos el campo separado mandamos todo en street            
            ///SERIAL_NO/serialNumber siempre 1 por que se imputa todo a lo mismo sino son imputaciones multiples, en ese caso analizar como se envia.

            var modificarPedidoSAPSinPI = new ModificarPedidoSAPSinPI
            {
                PURCHASEORDER = adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.NroOrdenDeCompraAdicional
            };
            string poItem = "";
            string numeroDeImputacion = "";
            var PCKG_NO = 1000;
            var numeroDePaquete = 1;

            var ocSAP = obtenerOrdenDeCompraconsumerMOA.ObtenerOrdenDeCompra(modificarPedidoSAPSinPI.PURCHASEORDER);
            int nroItemPO = ocSAP.Posiciones.Max(a => a.NumeroItemOC);

            bool esPosicionDeMateriales = adjudicacion.Posiciones.FirstOrDefault().Posicion.TipoPosicion.Codigo == "MATERIALES";

            var unidadesCodigoSap = adjudicacion.Posiciones.SelectMany(p => new[] { p.Posicion.Unidad?.CodigoSap }.Concat(p.Posicion.Subposiciones.Select(sp => sp.Unidad.CodigoSap))).Distinct();

            var unidadesMedidaSap = repositorio.Listar<UnidadMedidaSap, dynamic>(x => new { x.Comercial, x.UM },
                x => unidadesCodigoSap.Contains(x.Comercial))?.Select(x => System.Tuple.Create(x.Comercial, x.UM)).ToList();

            List<int> idsPosiciones = adjudicacion.Posiciones.Select(x => x.SolpPosicion_Id).ToList();
            var posicionesSolp = repositorio.Listar<SolpPosicion>(posi => idsPosiciones.Contains(posi.Id));
            foreach (var posicion in posicionesSolp)
            {
                //    foreach (var posicion in solp.Posiciones.Where(a => posIds.Contains(a.Id)).OrderBy(x => x.Id))
                //{
                nroItemPO += 1;
                var adjudicacionPosicion = adjudicacion.Posiciones.Single(a => a.CotizacionPosicion.PeticionDeOfertaSolpPosicion.SolpPosicion_Id == posicion.Id);

                numeroDePaquete++;
                numeroDeImputacion = "01";// SERIAL_NO por ahora siempre 01 por que no hay imputaciones multiples
                poItem = $"{nroItemPO:00000}";

                //Nombre: ZBAPIMEPOITEM Denominación:	Posición de PEDIDOS
                var IM_POITEM = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM
                {
                    PO_ITEM = poItem,
                    SHORT_TEXT = posicion.Tarea,
                    PLANT = posicion.Centro.CodigoSap.ToString(),
                    MATL_GROUP = posicion.GrupoArticulo?.CodigoSap?.ToString() ?? "",
                    MATERIAL = esPosicionDeMateriales ? posicion.MaterialSolp?.CodigoSap.ToString() : "",
                    STGE_LOC = posicion.Almacen != null ? posicion.Almacen.CodigoSap.ToString() : "",
                    ITEM_CAT = posicion.TipoPosicion.Codigo.ToLower() == "servicio" ? "9" : "0",//ITEM_CAT PSTYP   Tipo de posición del documento de compras
                    TRACKINGNO = posicion.NroNecesidad,
                    INFO_REC = "",
                    QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0,
                    PO_UNIT = esPosicionDeMateriales ? unidadesMedidaSap?.Find(u => u.Item1 == adjudicacionPosicion.CotizacionPosicion.UnidadDeMedida.CodigoSap).Item2 : "001",
                    NET_PRICE = esPosicionDeMateriales ? (decimal)adjudicacionPosicion.CotizacionPosicion.Precio : CalcularPrecioBrutoServicio(posicion, adjudicacionPosicion),
                    PRICE_UNIT = 1,
                    GR_PR_TIME = 0,
                    DELETE_IND = "",
                    TAX_CODE = "",
                    VAL_TYPE = "",
                    NO_MORE_GR = "",
                    FINAL_INV = "",
                    DISTRIB = "",
                    PART_INV = "",
                    GR_IND = "",
                    GR_NON_VAL = "",
                    IR_IND = "",
                    FREE_ITEM = "",
                    GR_BASEDIV = "",
                    ACKN_REQD = "",
                    ACKNOWL_NO = "",
                    AGREEMENT = "",
                    AGMT_ITEM = "",
                    RFQ_NO = "",
                    RFQ_ITEM = "",
                    PREQ_NO = posicion.Solp.NroSolp,
                    PREQ_ITEM = $"{posicion.Indice ?? 0:00000}",
                    PCKG_NO = esPosicionDeMateriales ? "" : $"{numeroDePaquete:0000000000}"
                };
                //IM_POITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
                ///IM_POITEM.QUANTITYSpecified = esPosicionDeMateriales ? true : false;
                ///IM_POITEM.NET_PRICESpecified = true;
                ///IM_POITEM.PRICE_UNITSpecified = true;
                //IM_POITEM.GR_PR_TIMESpecified = true; 

                switch (posicion.TipoImputacion?.Codigo.ToLower())
                {
                    case "centrodecosto":
                        IM_POITEM.ACCTASSCAT = "K";
                        break;
                    case "ordendeot":
                        IM_POITEM.ACCTASSCAT = "F";
                        break;
                    case "ordendeinversion":
                        IM_POITEM.ACCTASSCAT = "F";
                        break;
                    case "siniestrobeneficio":
                        IM_POITEM.ACCTASSCAT = "Y";
                        break;
                }
                modificarPedidoSAPSinPI.POITEM.Add(IM_POITEM);

                modificarPedidoSAPSinPI.POITEMX.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX
                {
                    PO_ITEM = poItem,
                    DELETE_IND = "",
                    SHORT_TEXT = "X",
                    MATERIAL = "X",
                    PLANT = "X",
                    STGE_LOC = "X",
                    TRACKINGNO = string.IsNullOrEmpty(posicion.NroNecesidad) ? "" : "X",
                    MATL_GROUP = "X",
                    INFO_REC = "",
                    QUANTITY = (IM_POITEM.QUANTITY == 0) ? "" : "X",
                    PO_UNIT = "X",
                    NET_PRICE = "X",
                    PRICE_UNIT = "X",
                    GR_PR_TIME = "",
                    TAX_CODE = "",
                    VAL_TYPE = "",
                    NO_MORE_GR = "",
                    FINAL_INV = "",
                    ITEM_CAT = "X",
                    ACCTASSCAT = (IM_POITEM.ACCTASSCAT != null) ? "X" : "",
                    DISTRIB = "",
                    PART_INV = "",
                    GR_IND = "",
                    GR_NON_VAL = "",
                    IR_IND = "",
                    FREE_ITEM = "",
                    GR_BASEDIV = "",
                    ACKN_REQD = "",
                    ACKNOWL_NO = "",
                    AGREEMENT = "",
                    AGMT_ITEM = "",
                    RFQ_NO = "",
                    RFQ_ITEM = "",
                    PREQ_NO = "X",
                    PREQ_ITEM = "X",
                    PCKG_NO = "X"
                });

                modificarPedidoSAPSinPI.POCOND.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND
                {
                    ITM_NUMBER = poItem,  //el número de ítem al que corresponda la condición
                    COND_TYPE = "ZP01",// siempre va el mismo dato
                    COND_VALUE = Math.Round(IM_POITEM.NET_PRICE, 4), //el importe de la condición
                    ///COND_VALUESpecified = true,
                    CURRENCY = adjudicacionPosicion.CotizacionPosicion.Moneda.Codigo,//moneda de la adjudicacion
                    CHANGE_ID = "U",// siempra va el mismo valor

                });
                modificarPedidoSAPSinPI.POCONDX.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX
                {
                    ITM_NUMBER = poItem,
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                });

                //Nombre: ZBAPIMEPOACCOUNT IM_POACCOUNT Denominación:	Imputación
                if (esPosicionDeMateriales)
                {
                    var imputacion = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT
                    {
                        PO_ITEM = poItem,
                        SERIAL_NO = numeroDeImputacion,
                        GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, posicion),
                        QUANTITY = esPosicionDeMateriales ? adjudicacionPosicion.Cantidad : 0,
                        BUS_AREA = "GENE",
                        CO_AREA = "MOA",
                        COSTCENTER = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "centrodecosto" }),
                        ORDERID = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "ordendeot", "ordendeinversion" }),
                        PROFIT_CTR = ObtenerImputacion(esPosicionDeMateriales, posicion, new List<string> { "siniestrobeneficio" }),
                        SUB_NUMBER = "",
                        ASSET_NO = "",
                        COSTOBJECT = "",
                        DELETE_IND = ""
                    };
                    ///imputacion.QUANTITYSpecified = imputacion.QUANTITY > 0;
                    modificarPedidoSAPSinPI.POACCOUNT.Add(imputacion);

                    modificarPedidoSAPSinPI.POACCOUNTX.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX
                    {
                        PO_ITEM = poItem,
                        SERIAL_NO = numeroDeImputacion,
                        DELETE_IND = "",
                        QUANTITY = "X",
                        GL_ACCOUNT = "X",
                        BUS_AREA = "X",
                        ASSET_NO = "",
                        SUB_NUMBER = "",
                        CO_AREA = "X",
                        COSTOBJECT = "",
                        COSTCENTER = (posicion.TipoImputacion?.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                        ORDERID = (posicion.TipoImputacion?.Codigo.ToLower() == "ordendeot" || posicion.TipoImputacion?.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                        PROFIT_CTR = (posicion.TipoImputacion?.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                    });
                }

                //Nombre: ZBAPIMEPOADDREDELIVERY Denominación:	Direcciones de entrega
                modificarPedidoSAPSinPI.POADDRDELIVERY.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = poItem,
                    POSTL_COD1 = posicion.CpEntrega,
                    CITY = posicion.Centro.Descripcion,
                    ADDR_NO = "",
                    NAME = posicion.NombreEntrega,
                    TEL1_NUMBR = "",
                    STREET = posicion.CalleEntrega,
                    STREET_NO = "",//no tenemos el campo separado en calle y altura
                    REGION = adjudicacion.RegionSap.CodigoSap
                });

                //subposiciones
                if (!esPosicionDeMateriales)
                {
                    var LINE_NO = 1;
                    //cabecera de subposiciones
                    var cabeceraSubPos = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC
                    {
                        PCKG_NO = $"{numeroDePaquete:0000000000}",
                        LINE_NO = $"{LINE_NO++:0000000000}",
                        OUTL_IND = "X",
                        OUTL_LEVEL = 0,
                        SUBPCKG_NO = $"{PCKG_NO:0000000000}",
                    };
                    modificarPedidoSAPSinPI.POSERVICES.Add(cabeceraSubPos);
                    var numeroImputacion = 0;

                    foreach (var subposicion in posicion.Subposiciones)
                    {
                        var cotizacionSubPosicion = adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones.Single(a => a.SolpSubPosicion_Id == subposicion.Id);

                        var subposicionSap = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC
                        {
                            PCKG_NO = $"{PCKG_NO:0000000000}",
                            LINE_NO = $"{LINE_NO:0000000000}",
                            EXT_LINE = $"{LINE_NO * 10:0000000000}",
                            SERVICE = subposicion.ServicioSolp?.Codigo,
                            SHORT_TEXT = subposicion.Tarea,
                            QUANTITY = cotizacionSubPosicion.Cantidad.Value,
                            BASE_UOM = unidadesMedidaSap.Find(u => u.Item1 == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).Item2,
                            UOM_ISO = unidadesMedidaSap.Find(u => u.Item1 == cotizacionSubPosicion.UnidadDeMedida.CodigoSap).Item2,
                            PRICE_UNIT = 1,
                            GR_PRICE = Math.Round(cotizacionSubPosicion.Precio.Value, 4),
                            BEGINTIME = "00:00:00",
                            ENDTIME = "00:00:00"
                        };
                        /// estos valores no se envian cuando es SIN PI
                        ///subposicionSap.QUANTITYSpecified = true;
                        ///subposicionSap.PRICE_UNITSpecified = true;
                        ///subposicionSap.GR_PRICESpecified = true;
                        modificarPedidoSAPSinPI.POSERVICES.Add(subposicionSap);


                        if (!modificarPedidoSAPSinPI.POACCOUNT.Any(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == ((getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "") &&
                                x.ORDERID == ((getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "") &&
                                x.PROFIT_CTR == ((getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "")
                            ))
                        {

                            numeroImputacion++;
                            var imputacion = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT
                            {
                                PO_ITEM = $"{poItem:00000}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                                GL_ACCOUNT = ObtenerCuentaMayor(esPosicionDeMateriales, posicion),
                                QUANTITY = subposicion.Cantidad.Value,
                                BUS_AREA = "GENE",
                                CO_AREA = "MOA",
                                COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ?
                                    getCodigoTablaSap(subposicion.TipoImputacionSap) : "",
                                ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ?
                                    getCodigoTablaSap(subposicion.TipoImputacionSap) : "",
                                PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ?
                                    getCodigoTablaSap(subposicion.TipoImputacionSap) : "",
                                SUB_NUMBER = "",
                                ASSET_NO = "",
                                COSTOBJECT = "",
                                DELETE_IND = ""
                            };
                            /// este valor no se envia cuando es SIN PI
                            ///imputacion.QUANTITYSpecified = true;
                            modificarPedidoSAPSinPI.POACCOUNT.Add(imputacion);

                            modificarPedidoSAPSinPI.POACCOUNTX.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX
                            {
                                PO_ITEM = $"{poItem:00000}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                                DELETE_IND = "",
                                QUANTITY = "X",
                                GL_ACCOUNT = "X",
                                BUS_AREA = "X",
                                ASSET_NO = "",
                                SUB_NUMBER = "",
                                CO_AREA = "X",
                                COSTOBJECT = "",
                                COSTCENTER = (posicion.TipoImputacion?.Codigo.ToLower() == "centrodecosto") ? "X" : "",
                                ORDERID = (posicion.TipoImputacion?.Codigo.ToLower() == "ordendeot" || posicion.TipoImputacion?.Codigo.ToLower() == "ordendeinversion") ? "X" : "",
                                PROFIT_CTR = (posicion.TipoImputacion?.Codigo.ToLower() == "siniestrobeneficio") ? "X" : ""
                            });

                            var imputacionSubPos = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                ///PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{numeroDeImputacion:00}",
                            };
                            modificarPedidoSAPSinPI.POSRVACCESSVALUES.Add(imputacionSubPos);
                        }
                        else
                        {
                            var imputacionUsada = modificarPedidoSAPSinPI.POACCOUNT.FirstOrDefault(x =>
                                x.PO_ITEM == $"{poItem:00000}" &&
                                x.GL_ACCOUNT == getCodigoTablaSap(subposicion.CuentaMayorSap) &&
                                x.COSTCENTER == ((getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "") &&
                                x.ORDERID == ((getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "") &&
                                x.PROFIT_CTR == ((getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ?
                                getCodigoTablaSap(subposicion.TipoImputacionSap) : "")
                                );
                            imputacionUsada.QUANTITY += subposicion.Cantidad.Value;

                            var imputacionSubPos = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC()
                            {
                                PCKG_NO = $"{PCKG_NO:0000000000}",
                                LINE_NO = $"{LINE_NO++:0000000000}",
                                PERCENTAGE = 100,
                                ///PERCENTAGESpecified = true,
                                SERNO_LINE = $"{poItem:00}",
                                SERIAL_NO = $"{imputacionUsada.SERIAL_NO:00}",
                            };
                            modificarPedidoSAPSinPI.POSRVACCESSVALUES.Add(imputacionSubPos);
                        }
                    }
                    PCKG_NO++;
                }

                modificarPedidoSAPSinPI.POSCHEDULE.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE
                {
                    DELIVERY_DATE = adjudicacionPosicion.PlazoDeEntrega.ToString("dd.MM.yyyy"),
                    PO_ITEM = poItem,
                    SCHED_LINE = "1",
                    DELIV_TIME = "00:00:00",
                    MS_TIME = "00:00:00",
                    LOAD_TIME = "00:00:00",
                    TP_TIME = "00:00:00",
                    GI_TIME = "00:00:00",
                    GR_END_TIME = "00:00:00",
                    HANDOVERTIME = "00:00:00",
                });

                modificarPedidoSAPSinPI.POSCHEDULEX.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX
                {
                    DELIVERY_DATE = "X",
                    PO_ITEM = poItem,
                    SCHED_LINE = "1"
                });
            }

            var listaVaciaTexto = new string[] { "" };

            var textosDiccionario = new Dictionary<string, string[]>() {
                {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            };

            foreach (var grupos in textosDiccionario)
            {
                bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
                if (!todosVacios)
                {
                    foreach (var texto in grupos.Value)
                    {
                        modificarPedidoSAPSinPI.POTEXTHEADER.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER
                        {
                            TEXT_ID = grupos.Key,
                            PO_NUMBER = "",
                            PO_ITEM = "0",
                            TEXT_FORM = "*",
                            TEXT_LINE = texto
                        });
                    }
                }
            }

            if (adjudicacion.Posiciones.FirstOrDefault().Posicion.Solp.Urgencia == true)
            {
                modificarPedidoSAPSinPI.POTEXTITEM.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT
                {
                    TEXT_ID = "F12",
                    PO_NUMBER = "",
                    PO_ITEM = poItem,
                    TEXT_FORM = "*",
                    TEXT_LINE = "Urgencia"
                });
            }

            modificarPedidoSAPSinPI.IM_URL = ConfigurationManager.AppSettings["SpaUrl"] + "/verLegajoOrdenDeCompra/" + adjudicacion.Id + "/" + adjudicacion.Token;

            return modificarPedidoSAPSinPI;
        }


        private static string ObtenerImputacion(bool esPosicionDeMateriales, SolpPosicion posicion, List<string> tipos)
        {
            if (tipos.Contains(posicion.TipoImputacion?.Codigo.ToLower()))
            {
                if (esPosicionDeMateriales)
                {
                    return posicion.TipoImputacionSap?.Codigo ?? "";
                }
                else
                {
                    return posicion.Subposiciones.FirstOrDefault()?.TipoImputacionSap?.Codigo ?? "";
                }
            }
            else
            {
                return "";
            }
        }

        private string getCodigoTablaGeneral(TablaGeneral imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }
        private static string ObtenerCuentaMayor(bool esPosicionDeMateriales, SolpPosicion posicion)
        {
            return esPosicionDeMateriales ? (posicion.CuentaMayorSap?.Codigo ?? "") : posicion.Subposiciones.FirstOrDefault()?.CuentaMayorSap?.Codigo ?? "";
        }

        private decimal CalcularPrecioBrutoServicio(SolpPosicion posicion, AdjudicacionPosicion adjudicacionPosicion)
        {
            decimal total = 0;

            foreach (var item in adjudicacionPosicion.CotizacionPosicion.CotizacionSubPosiciones)
            {
                total += item.Cantidad.Value * item.Precio.Value;
            }

            return total;
        }
        private string getCodigoTablaSap(TablaSap imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }


        public class ModificarPedidoSAP
        {
            public string NRO_SOLP { get; set; }
            public string MEMORY_COMPLETE { get; set; }
            public string MEMORY_UNCOMPLETE { get; set; }
            public string NO_AUTHORITY { get; set; }
            public string NO_MESSAGE_REQ { get; set; }
            public string NO_MESSAGING { get; set; }
            public string NO_PRICE_FROM_PO { get; set; }
            public string PARK_COMPLETE { get; set; }
            public string PARK_UNCOMPLETE { get; set; }
            public ModificarOCWebServiceMOA.BAPIMEPOADDRVENDOR POADDRVENDOR { get; set; }
            public ModificarOCWebServiceMOA.BAPIEIKP POEXPIMPHEADER { get; set; }
            public ModificarOCWebServiceMOA.BAPIEIKPX POEXPIMPHEADERX { get; set; }
            public ModificarOCWebServiceMOA.BAPIMEPOHEADER POHEADER { get; set; }
            public ModificarOCWebServiceMOA.BAPIMEPOHEADERX POHEADERX { get; set; }
            public string PURCHASEORDER { get; set; }
            public string TESTRUN { get; set; }
            public ModificarOCWebServiceMOA.BAPIMEDCM VERSIONS { get; set; }
            public List<ModificarOCWebServiceMOA.BAPIMEDCM_ALLVERSIONS> ALLVERSIONS { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEDCM_ALLVERSIONS>();
            public List<ModificarOCWebServiceMOA.BAPIPAREX> EXTENSIONIN { get; set; } = new List<ModificarOCWebServiceMOA.BAPIPAREX>();
            public List<ModificarOCWebServiceMOA.BAPIPAREX> EXTENSIONOUT { get; set; } = new List<ModificarOCWebServiceMOA.BAPIPAREX>();
            public List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_HEADER> INVPLANHEADER { get; set; } = new List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_HEADER>();
            public List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_HEADERX> INVPLANHEADERX { get; set; } = new List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_HEADERX>();
            public List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_ITEM> INVPLANITEM { get; set; } = new List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_ITEM>();
            public List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_ITEMX> INVPLANITEMX { get; set; } = new List<ModificarOCWebServiceMOA.BAPI_INVOICE_PLAN_ITEMX>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOACCOUNT> POACCOUNT { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOACCOUNT>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOACCOUNTPROFITSEGMENT> POACCOUNTPROFITSEGMENT { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOACCOUNTPROFITSEGMENT>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOACCOUNTX> POACCOUNTX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOACCOUNTX>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOADDRDELIVERY> POADDRDELIVERY { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOADDRDELIVERY>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOCOMPONENT> POCOMPONENTS { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOCOMPONENT>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOCOMPONENTX> POCOMPONENTSX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOCOMPONENTX>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOCOND> POCOND { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOCOND>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOCONDHEADER> POCONDHEADER { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOCONDHEADER>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOCONDHEADERX> POCONDHEADERX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOCONDHEADERX>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOCONDX> POCONDX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOCONDX>();
            public List<ModificarOCWebServiceMOA.BAPIEKES> POCONFIRMATION { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEKES>();
            public List<ModificarOCWebServiceMOA.BAPIESUCC> POCONTRACTLIMITS { get; set; } = new List<ModificarOCWebServiceMOA.BAPIESUCC>();
            public List<ModificarOCWebServiceMOA.BAPIEIPO> POEXPIMPITEM { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEIPO>();
            public List<ModificarOCWebServiceMOA.BAPIEIPOX> POEXPIMPITEMX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEIPOX>();
            public List<ModificarOCWebServiceMOA.BAPIEKBE> POHISTORY { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEKBE>();
            public List<ModificarOCWebServiceMOA.BAPIEKBE_MA> POHISTORY_MA { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEKBE_MA>();
            public List<ModificarOCWebServiceMOA.BAPIEKBES> POHISTORY_TOTALS { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEKBES>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOITEM> POITEM { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOITEM>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOITEMX> POITEMX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOITEMX>();
            public List<ModificarOCWebServiceMOA.BAPIESUHC> POLIMITS { get; set; } = new List<ModificarOCWebServiceMOA.BAPIESUHC>();
            public List<ModificarOCWebServiceMOA.BAPIEKKOP> POPARTNER { get; set; } = new List<ModificarOCWebServiceMOA.BAPIEKKOP>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOSCHEDULE> POSCHEDULE { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOSCHEDULE>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOSCHEDULX> POSCHEDULEX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOSCHEDULX>();
            public List<ModificarOCWebServiceMOA.BAPIESLLC> POSERVICES { get; set; } = new List<ModificarOCWebServiceMOA.BAPIESLLC>();
            public List<ModificarOCWebServiceMOA.BAPIESLLTX> POSERVICESTEXT { get; set; } = new List<ModificarOCWebServiceMOA.BAPIESLLTX>();
            public List<ModificarOCWebServiceMOA.BAPIITEMSHIP> POSHIPPING { get; set; } = new List<ModificarOCWebServiceMOA.BAPIITEMSHIP>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOSHIPPEXP> POSHIPPINGEXP { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOSHIPPEXP>();
            public List<ModificarOCWebServiceMOA.BAPIITEMSHIPX> POSHIPPINGX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIITEMSHIPX>();
            public List<ModificarOCWebServiceMOA.BAPIESKLC> POSRVACCESSVALUES { get; set; } = new List<ModificarOCWebServiceMOA.BAPIESKLC>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOTEXTHEADER> POTEXTHEADER { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOTEXTHEADER>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOTEXT> POTEXTITEM { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOTEXT>();
            public List<ModificarOCWebServiceMOA.BAPIRET2> RETURN { get; set; } = new List<ModificarOCWebServiceMOA.BAPIRET2>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOSERIALNO> SERIALNUMBER { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOSERIALNO>();
            public List<ModificarOCWebServiceMOA.BAPIMEPOSERIALNOX> SERIALNUMBERX { get; set; } = new List<ModificarOCWebServiceMOA.BAPIMEPOSERIALNOX>();
            public ModificarOCWebServiceMOA.BAPIEIKP EXPPOEXPIMPHEADER { get; set; }
            public List<ModificarOCWebServiceMOA._NFM_BAPIDOCITM> NFMETALLITMS { get; set; } = new List<ModificarOCWebServiceMOA._NFM_BAPIDOCITM>();
            public string IM_URL { get; set; }
        }

        public class ModificarPedidoSAPSinPI
        {
            public string NRO_SOLP { get; set; }
            public string MEMORY_COMPLETE { get; set; }
            public string MEMORY_UNCOMPLETE { get; set; }
            public string NO_AUTHORITY { get; set; }
            public string NO_MESSAGE_REQ { get; set; }
            public string NO_MESSAGING { get; set; }
            public string NO_PRICE_FROM_PO { get; set; }
            public string PARK_COMPLETE { get; set; }
            public string PARK_UNCOMPLETE { get; set; }
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRVENDOR POADDRVENDOR { get; set; }
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIKP POEXPIMPHEADER { get; set; }
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIKPX POEXPIMPHEADERX { get; set; }
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER POHEADER { get; set; }
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADERX POHEADERX { get; set; }
            public string PURCHASEORDER { get; set; }
            public string TESTRUN { get; set; }
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEDCM VERSIONS { get; set; }
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEDCM_ALLVERSIONS> ALLVERSIONS { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEDCM_ALLVERSIONS>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIPAREX> EXTENSIONIN { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIPAREX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIPAREX> EXTENSIONOUT { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIPAREX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_HEADER> INVPLANHEADER { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_HEADER>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_HEADERX> INVPLANHEADERX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_HEADERX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_ITEM> INVPLANITEM { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_ITEM>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_ITEMX> INVPLANITEMX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_INVOICE_PLAN_ITEMX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT> POACCOUNT { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTPROFITSEGMENT> POACCOUNTPROFITSEGMENT { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTPROFITSEGMENT>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX> POACCOUNTX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY> POADDRDELIVERY { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOMPONENT> POCOMPONENTS { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOMPONENT>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOMPONENTX> POCOMPONENTSX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOMPONENTX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND> POCOND { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADER> POCONDHEADER { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADER>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADERX> POCONDHEADERX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDHEADERX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX> POCONDX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKES> POCONFIRMATION { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKES>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESUCC> POCONTRACTLIMITS { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESUCC>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIPO> POEXPIMPITEM { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIPO>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIPOX> POEXPIMPITEMX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIPOX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBE> POHISTORY { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBE>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBE_MA> POHISTORY_MA { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBE_MA>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBES> POHISTORY_TOTALS { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKBES>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM> POITEM { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX> POITEMX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESUHC> POLIMITS { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESUHC>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKKOP> POPARTNER { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEKKOP>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE> POSCHEDULE { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX> POSCHEDULEX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC> POSERVICES { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLTX> POSERVICESTEXT { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLTX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIITEMSHIP> POSHIPPING { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIITEMSHIP>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSHIPPEXP> POSHIPPINGEXP { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSHIPPEXP>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIITEMSHIPX> POSHIPPINGX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIITEMSHIPX>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC> POSRVACCESSVALUES { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESKLC>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER> POTEXTHEADER { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT> POTEXTITEM { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXT>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2> RETURN { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSERIALNO> SERIALNUMBER { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSERIALNO>();
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSERIALNOX> SERIALNUMBERX { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSERIALNOX>();
            public WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIEIKP EXPPOEXPIMPHEADER { get; set; }
            public List<WS_GAQ_sin_PI_DIRECT_COMPRAS._NFM_BAPIDOCITM> NFMETALLITMS { get; set; } = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS._NFM_BAPIDOCITM>();
            public string IM_URL { get; set; }
        }

    }

    public interface IModificarOrdenDeCompraConsumerMOA
    {
        CrearPedidoConsumerMOAResponse Request(Adjudicacion adjudicacion);
        WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRET2[] EditarPedidoRequestSinPI(ModificarPedidoSAPSinPI modificarPedidoSAP);
        ModificarOCWebServiceMOA.BAPIRET2[] EditarPedidoRequest(ModificarPedidoSAP modificarPedidoSAP);

    }

}
