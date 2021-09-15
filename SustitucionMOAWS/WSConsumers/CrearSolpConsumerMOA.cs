using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearSolpConsumerMOA : ICrearSolpConsumerMOA
    {
        private readonly SI_MMRFC_CREAR_SOLPEDClient service;

        public CrearSolpConsumerMOA()
        {
            service = new SI_MMRFC_CREAR_SOLPEDClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object Request()
        {

            ZMPES5690[] IM_PRACCOUNT = new ZMPES5690[0];



            ZMPES5680[] IM_PRACCOUNTX = new ZMPES5680[0];
            ZMPES5750[] IM_PRADDRDELIVERY = new ZMPES5750[0];
            BAPIMEREQHEADTEXT[] IM_PRHEADERTEXT = new BAPIMEREQHEADTEXT[0];
            ZMPES5700[] IM_PRITEM = new ZMPES5700[0];
            BAPIMEREQITEMTEXT[] IM_PRITEMTEXT = new BAPIMEREQITEMTEXT[0];
            ZMPES5660[] IM_PRITEMX = new ZMPES5660[0];
            string IM_PR_TYPE = "";
            ZMPES5790[] IM_SERVICEACCOUNT = new ZMPES5790[0];
            BAPI_SRV_ACC_DATAX[] IM_SERVICEACCOUNTX = new BAPI_SRV_ACC_DATAX[0];
            ZMPES5780[] IM_SERVICELINES = new ZMPES5780[0];
            ZMPES5720[] IM_SERVICELINESX = new ZMPES5720[0];
            
            var result = service.SI_MMRFC_CREAR_SOLPED(IM_PRACCOUNT, IM_PRACCOUNTX, IM_PRADDRDELIVERY, IM_PRHEADERTEXT, IM_PRITEM, IM_PRITEMTEXT, IM_PRITEMX, IM_PR_TYPE, IM_SERVICEACCOUNT,
                IM_SERVICEACCOUNTX, IM_SERVICELINES, IM_SERVICELINESX
                , out string EX_PREQ_NO, out BAPIRETURN[] EX_RETURN);


            return result;
        }

        public object Map()
        {
            return null;
        }
    }

    public interface ICrearSolpConsumerMOA
    {
        object Request();

    }
}
