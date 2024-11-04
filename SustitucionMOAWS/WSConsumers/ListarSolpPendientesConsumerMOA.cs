// Ignore Spelling: Sustitucion

using SustitucionMOAModel.Dto;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ListarSolpPendienteWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ListarSolpPendientesConsumerMOA : IListarSolpPendientesConsumerMOA
    {
        private readonly SI_MMRFC_BAPI_REQUISITION_GETITEMSClient service;

        private readonly ICache Cache;
        private const string CACHE_KEY = "SI_MMRFC_BAPI_REQUISITION_GETITEMSClientCache";
        private readonly DateTimeOffset CACHE_EXPIRATION = DateTimeOffset.Now.AddSeconds(15);
        private static readonly object _lockObject = new object();

        public ListarSolpPendientesConsumerMOA(ICache cache)
        {
            Cache = cache;

            const string url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_BAPI_REQUISITION_GETITEMS&amp;interfaceNamespace=urn%3AOPERACIONES";

            service = new SI_MMRFC_BAPI_REQUISITION_GETITEMSClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public List<PosicionPendienteDto> ListarSolpPendientes()
        {
            lock (_lockObject)
            {
                // en caso de parametrizar el método ListarSolpPendientes(), agregar parámetros a la CACHE_KEY.
                if (Cache.IntentarObtener(CACHE_KEY, out List<PosicionPendienteDto> solps))
                {
                    return solps;
                }

                const string ASSIGNED_ITEMS = "X";
                const string CLOSED_ITEMS = "";
                const string DELETED_ITEMS = "";
                const string DELIV_DATE = "";
                const string DOC_TYPE = "";
                const string MATERIAL = "";
                BAPIMGVMATNR MATERIAL_EVG = new BAPIMGVMATNR();
                const string MATERIAL_LONG = "";
                const string MAT_GRP = "";
                const string ONLY_NON_MATERIAL_ITEMS = "";
                const string OPEN_ITEMS = "X";  // Valor agregado
                const string PARTIALLY_ORDERED_ITEMS = "X";
                const string PLANT = "";
                const string PREQ_DATE = "";
                const string PREQ_NAME = "";
                const string PREQ_NO = "";
                const string PUR_GROUP = "";
                const string REL_DATE = "";
                const string SHORT_TEXT = "";
                const string TRACKINGNO = "";
                BAPIEBANC[] REQUISITION_ITEMS = new List<BAPIEBANC>().ToArray(); // Array vacío
                BAPIRETURN[] RETURN = new List<BAPIRETURN>().ToArray(); // Array vacío

                service.SI_MMRFC_BAPI_REQUISITION_GETITEMS(
                    ASSIGNED_ITEMS, CLOSED_ITEMS, DELETED_ITEMS, DELIV_DATE, DOC_TYPE, MATERIAL,
                    MATERIAL_EVG, MATERIAL_LONG, MAT_GRP, ONLY_NON_MATERIAL_ITEMS, OPEN_ITEMS,
                    PARTIALLY_ORDERED_ITEMS, PLANT, PREQ_DATE, PREQ_NAME, PREQ_NO, PUR_GROUP,
                    REL_DATE, SHORT_TEXT, TRACKINGNO, ref REQUISITION_ITEMS, ref RETURN);
                List<BAPIEBANC> solpsSAP = REQUISITION_ITEMS.ToList();


                solps = solpsSAP.ConvertAll(x => new PosicionPendienteDto
                {
                    NroSolp = x.PREQ_NO,
                    NumeroPosicion = int.Parse(x.PREQ_ITEM),
                    CodigoMaterial = x.MATERIAL,
                    Cantidad = x.QUANTITY,
                    Pedido = x.ORDERED
                });

                Cache.Agregar(CACHE_KEY, solps, CACHE_EXPIRATION);

                return solps;
            }
        }
    }

    public interface IListarSolpPendientesConsumerMOA
    {
        List<PosicionPendienteDto> ListarSolpPendientes();
    }
}
