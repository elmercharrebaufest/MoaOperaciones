using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenDeCompraConsumerMOA : IObtenerOrdenDeCompraConsumerMOA
    {
        BAPI_PO_GETDETAIL1PortTypeClient service;
        private const string COMP_CODE = "MOA";

        public ObtenerOrdenDeCompraConsumerMOA()
        {
            service = new BAPI_PO_GETDETAIL1PortTypeClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }


        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            try
            {
                string ACCOUNT_ASSIGNMENT = "";
                string DELIVERY_ADDRESS = "";
                string HEADER_TEXT = "";
                string INVOICEPLAN = ""; 
                string ITEM_TEXT = ""; 
                string PURCHASEORDER = ""; 
                string SERIALNUMBERS = "";
                string SERVICES = "";
                string VERSION = "";

                BAPIMEPOACCOUNT[] POACCOUNT = new BAPIMEPOACCOUNT[] { };
                BAPIMEPOADDRDELIVERY[] POADDRDELIVERY = new BAPIMEPOADDRDELIVERY[] { };
                BAPIMEPOCOND[] POCOND = new BAPIMEPOCOND[] { };
                BAPIMEPOITEM[] POITEM = new BAPIMEPOITEM[] { }; 
                BAPIMEPOTEXTHEADER[] POTEXTHEADER = new BAPIMEPOTEXTHEADER[] { };
                BAPIMEPOTEXT[] POTEXTITEM = new BAPIMEPOTEXT[] { };
                BAPIRET2[] RETURN = new BAPIRET2[] { };
                BAPIESLLC[] POSERVICES = new BAPIESLLC[] { };
                BAPIMEPOHEADER POHEADER = new BAPIMEPOHEADER { };
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
                BAPIMEPOSCHEDULE[] POSCHEDULE = new BAPIMEPOSCHEDULE[] { };
                BAPIMEPOSHIPPEXP[] POSHIPPINGEXP = new BAPIMEPOSHIPPEXP[] { };
                BAPIESKLC[] POSRVACCESSVALUES = new BAPIESKLC[] { };
                BAPIMEPOSERIALNO[] SERIALNUMBER = new BAPIMEPOSERIALNO[] { };

                var result = service.BAPI_PO_GETDETAIL1(ACCOUNT_ASSIGNMENT,
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

                return map(POHEADER, RETURN);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual OrdenDeCompraSAPDto map(BAPIMEPOHEADER POHEADER, BAPIRET2[] RETURN)
        {
            OrdenDeCompraSAPDto result = new OrdenDeCompraSAPDto();
            result.Cabecera = new OrdenDeCompraSAPCabecera { 
                OrdenDeCompra = POHEADER.PO_NUMBER,
                CodigoProveedor = POHEADER.VENDOR
            };
            return result;
        }
    }

   
}
