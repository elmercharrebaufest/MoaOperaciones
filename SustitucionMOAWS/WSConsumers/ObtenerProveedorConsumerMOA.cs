using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerProveedorWebServiceMOA;
using System;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerProveedorConsumerMOA : IObtenerProveedorConsumerMOA
    {
        SI_MMRFC_OBTENER_PROVClient service;
        private const string COMP_CODE = "MOA";

        public ObtenerProveedorConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_PROV&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_OBTENER_PROVClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }


        public ObtenerProveedorWSMOAResponse ObtenerProveedor(string codigoProveedor)
        {
            try
            {
                Log.Info($"ObtenerProveedor SI_MMRFC_OBTENER_PROVC: {codigoProveedor}");

                ZMPES5980[] IM_COMP_CODE = new ZMPES5980[] { new ZMPES5980 { SIGN = "I", OPTION = "EQ", LOW = "MOA" } };
                ZMPES5880[] IM_NAME = new ZMPES5880[] { };
                ZMPES6000[] IM_PURCH_ORG = new ZMPES6000[] { };
                ZMPES5970[] IM_SORTL = new ZMPES5970[] { };
                string IM_TYPE = "PROV";
                ROIJ_LIFNR_RSTR[] IM_VENDOR = new ROIJ_LIFNR_RSTR[] { new ROIJ_LIFNR_RSTR { OPTION = "EQ", SIGN = "I", LOW = codigoProveedor } };

                var result = service.SI_MMRFC_OBTENER_PROV(
                    IM_COMP_CODE,
                    IM_NAME,
                    IM_PURCH_ORG,
                    IM_SORTL,
                     IM_TYPE,
                     IM_VENDOR,
                    out BAPIRET2[] EX_RETURN,
                    out ZMPES5990[] EX_VENDOR
                    );

                return map(result, EX_RETURN, EX_VENDOR);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private ObtenerProveedorWSMOAResponse map(string result, BAPIRET2[] RETURN, ZMPES5990[] VENDOR)
        {
            ObtenerProveedorWSMOAResponse resultado = null;
            if (result != "200" || VENDOR.Length == 0)
                return null;


            resultado = new ObtenerProveedorWSMOAResponse
            {
                LAND1 = VENDOR.First().LAND1,
                VENDOR = VENDOR.First().VENDOR,
                COMPCODE = VENDOR.First().COMP_CODE,
                COUNTRY = VENDOR.First().COUNTRY,
                CURRENCY = VENDOR.First().CURRENCY,
                KTOKK = VENDOR.First().KTOKK,
                MAIL = VENDOR.First().MAIL,
                NAME = VENDOR.First().NAME,
                PMNTTRMS = VENDOR.First().PMNTTRMS,
                PURCHORG = VENDOR.First().PURCH_ORG,
                SORT1 = VENDOR.First().SORT1,
                SORT2 = VENDOR.First().SORT2,
                STREET = VENDOR.First().STREET,
                TELEFONO = VENDOR.First().TELEFONO,
                VERIFFEM = VENDOR.First().VERIF_F_EM
            };

            return resultado;
        }
    }


}
