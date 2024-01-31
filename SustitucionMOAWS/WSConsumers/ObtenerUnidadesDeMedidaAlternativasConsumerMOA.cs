using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerUnidadesDeMedidaAlternativasWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerUnidadesDeMedidaAlternativasConsumerMOA : IObtenerUnidadesDeMedidaAlternativasConsumerMOA
    {
        private readonly IRepositorio repositorio;
        readonly SI_MMRFC_UM_ALTERNATIVASClient service;

        public ObtenerUnidadesDeMedidaAlternativasConsumerMOA(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_UM_ALTERNATIVAS&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_UM_ALTERNATIVASClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public List<UnidadesDeMedida> Request(List<string> codigosMaterial)
        {
            try
            {
                WSELMATNR[] IM_MATERIAL = new WSELMATNR[] { };
                ZMMTT_UM_ALT[] EX_UM_ALT = new ZMMTT_UM_ALT[] { };

                var result = new List<UnidadesDeMedida>();

                IM_MATERIAL = codigosMaterial.Select(x => new WSELMATNR { SIGN = "I", OPTION = "EQ", LOW = x }).ToArray();

                service.SI_MMRFC_UM_ALTERNATIVAS(IM_MATERIAL, ref EX_UM_ALT);

                var unidadMedidaSap = repositorio.Listar<UnidadMedidaSap>();

                foreach (var umAlt in EX_UM_ALT)
                {
                    var codigoUnidad = unidadMedidaSap.Where(a => a.UM == umAlt.UM).Single().Comercial;

                    result.Add(new UnidadesDeMedida
                    {
                        CodigoMaterial = umAlt.MATERIAL,
                        UnidadDeMedida = codigoUnidad,
                        Denominador = umAlt.DENOMINADOR,
                        Numerador = umAlt.NUMERADOR,
                    });

                }

                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}