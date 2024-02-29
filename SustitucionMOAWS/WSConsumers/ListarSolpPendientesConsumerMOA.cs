using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.ListarSolpPendienteWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class ListarSolpPendientesConsumerMOA : IListarSolpPendientesConsumerMOA
    {
       
        private readonly SI_MMRFC_BAPI_REQUISITION_GETITEMSClient service;

        public ListarSolpPendientesConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_BAPI_REQUISITION_GETITEMS&amp;interfaceNamespace=urn%3AOPERACIONES";
          
            service = new SI_MMRFC_BAPI_REQUISITION_GETITEMSClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
          
        }

        public List<string> ListarSolpPendientes()
        {
            var solps = new List<string>();
            string ASSIGNED_ITEMS = "";
            string CLOSED_ITEMS = "";
            string DELETED_ITEMS = "";
            string DELIV_DATE = "";
            string DOC_TYPE = "";
            string MATERIAL = "";
            BAPIMGVMATNR MATERIAL_EVG = new BAPIMGVMATNR();
            string MATERIAL_LONG = "";
            string MAT_GRP = "";
            string ONLY_NON_MATERIAL_ITEMS = "";
            string OPEN_ITEMS = "X";  // Valor agregado
            string PARTIALLY_ORDERED_ITEMS = "X";
            string PLANT = "";
            string PREQ_DATE = "";
            string PREQ_NAME = "";
            string PREQ_NO = "";
            string PUR_GROUP = "";
            string REL_DATE = "";
            string SHORT_TEXT = "";
            string TRACKINGNO = "";
            BAPIEBANC[] REQUISITION_ITEMS = new List<BAPIEBANC>().ToArray(); // Array vacío
            BAPIRETURN[] RETURN = new List<BAPIRETURN>().ToArray(); // Array vacío

            service.SI_MMRFC_BAPI_REQUISITION_GETITEMS(
                ASSIGNED_ITEMS, CLOSED_ITEMS, DELETED_ITEMS, DELIV_DATE, DOC_TYPE, MATERIAL,
                MATERIAL_EVG, MATERIAL_LONG, MAT_GRP, ONLY_NON_MATERIAL_ITEMS, OPEN_ITEMS,
                PARTIALLY_ORDERED_ITEMS, PLANT, PREQ_DATE, PREQ_NAME, PREQ_NO, PUR_GROUP,
                REL_DATE, SHORT_TEXT, TRACKINGNO, ref REQUISITION_ITEMS, ref RETURN);
            var solpsSAP = REQUISITION_ITEMS.ToList();

            if (solpsSAP.Count > 0) {
                solps.AddRange(solpsSAP.Select(x => x.PREQ_NO));
            }

            return solps;
        }
    }


    public interface IListarSolpPendientesConsumerMOA
    {
        List<string> ListarSolpPendientes();

    }

}
