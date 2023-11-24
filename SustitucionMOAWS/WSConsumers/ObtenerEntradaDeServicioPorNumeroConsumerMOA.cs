using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
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
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerEntradaDeServicioPorNumeroConsumerMOA : IObtenerEntradaDeServicioPorNumeroConsumerMOA
    {
        SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient service;
        //private const string COMP_CODE = "MOA";
        private readonly IRepositorio repositorio;

        public ObtenerEntradaDeServicioPorNumeroConsumerMOA()
        {
            service = new SI_MMRFC_BAPI_ENTRYSHEET_GETDETAILClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            //this.repositorio = repositorio;
        }

        public OrdenCompraEntradaServicioDto ObtenerEntradaServicio(string nroES) // El tipo que devuelve esta puesto solo para que no marque error
        {
            try
            {
                string ENTRYSHEET = nroES; //Valor de prueba, existe en SAP
                string LONG_TEXTS = "";
                BAPIESKN[] ENTRYSHEET_ACCOUNT_ASSIGMENT = new BAPIESKN[] { };
                BAPIESSRTX[] ENTRYSHEET_HEADER_TEXT = new BAPIESSRTX[] { };
                BAPIESLL[] ENTRYSHEET_SERVICES = new BAPIESLL[] { };
                BAPIESLLTX[] ENTRYSHEET_SERVICES_TEXTS = new BAPIESLLTX[] { };
                BAPIESKL[] ENTRYSHEET_SRV_ACCASS_VALUES = new BAPIESKL[] { };
                BAPIRETURN1[] RETURN = new BAPIRETURN1[] { };

                BAPIESSR cabecera = service.SI_MMRFC_BAPI_ENTRYSHEET_GETDETAIL(ENTRYSHEET, LONG_TEXTS, ref ENTRYSHEET_ACCOUNT_ASSIGMENT, ref ENTRYSHEET_HEADER_TEXT, ref ENTRYSHEET_SERVICES, ref ENTRYSHEET_SERVICES_TEXTS, ref ENTRYSHEET_SRV_ACCASS_VALUES, ref RETURN);

                return Map(cabecera);


                //return result;
                //return mapOrdenDeCompraSAPDto(result, POHEADER, RETURN, POITEM, POTEXTHEADER, POTEXTITEM, POSERVICES, POSCHEDULE, POADDRDELIVERY);

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private OrdenCompraEntradaServicioDto Map(BAPIESSR cabecera)
        {
            var aux = new BAPIESSR[] { };
            return null;
        }

    }
}
