using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAModel.Models.WSMapMOA.Contrato;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerContratoSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerContratoSolpConsumerMOA : IObtenerContratoSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_CONTRATOClient service;

        public ObtenerContratoSolpConsumerMOA()
        {
            service = new SI_MMRFC_OBTENER_CONTRATOClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object request() 
        {
            try
            {
                string IM_COMP_CODE = "";
                string IM_CONTRACT = "";
                string IM_DETAIL = "";
                string IM_ITEM_NO = "";
                ZMPES5800[] IM_MATERIAL = new ZMPES5800[] { };
                ZMPES5880[] IM_NOM_VENDOR = new ZMPES5880[] { };
                string IM_PLANT = "";
                ZMPES5810[] IM_TEXT_POS = new ZMPES5810[] { };
                ZMPES5870[] IM_VENDOR = new ZMPES5870[] { };
                ZMPES5890[] EX_HEADER = new ZMPES5890[] { };
                ZMPES5900[] EX_ITEM = new ZMPES5900[] { };
                BAPIRETURN[] EX_RETURN = new BAPIRETURN[] { };
                ZMPES5910[] EX_SUB_ITEM = new ZMPES5910[] { };


                string contratoObtenido = service.SI_MMRFC_OBTENER_CONTRATO(IM_COMP_CODE, IM_CONTRACT, IM_DETAIL, IM_ITEM_NO, IM_MATERIAL, IM_NOM_VENDOR,  IM_PLANT,  IM_TEXT_POS,  IM_VENDOR, out EX_HEADER, out EX_ITEM, out EX_RETURN, out EX_SUB_ITEM);

                return map(contratoObtenido, EX_HEADER, EX_ITEM, EX_RETURN, EX_SUB_ITEM);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual object map(string contratoObtenido, ZMPES5890[] EX_HEADER, ZMPES5900[] EX_ITEM, BAPIRETURN[] EX_RETURN, ZMPES5910[] EX_SUB_ITEM)
        {
            ContratoSolpWSMOAResponse result = new ContratoSolpWSMOAResponse();
            result.ContratosSolp = new List<ContratoSolp> { };

            foreach (var contrato in contratoObtenido)
            {
                ContratoSolp contratoSolp = new ContratoSolp()
                {
                    //Nombre: ZMMTT_HEADER Denominación:	Resultado de Cabera de Contratos Marco
                    //Nombre Dominio / Tipo  Denominación
                    //NUMBER  EBELN Número del documento de compras
                    //COMP_CODE BUKRS   Sociedad
                    //DELETE_IND_HDR  ELOEK Indicador de borrado en el documento de compras
                    //VENDOR  ELIFN Número de cuenta del proveedor
                    //NAM_VENDOR LFA1-NAME1  Nombre del Proveedor
                    //PURCH_ORG   EKORG Organización de compras
                    //PUR_GROUP BKGRP   Grupo de compras
                    //CURRENCY    WAERS Clave de moneda
                    //VPER_START KDATB   In.período validez
                    //VPER_END KDATE   Fin período validez

                    //Resultado de Cabera de Contratos Marco
                    

                };
                result.ContratosSolp.Add(contratoSolp);
            }

            return result;
        }

    }  
}


