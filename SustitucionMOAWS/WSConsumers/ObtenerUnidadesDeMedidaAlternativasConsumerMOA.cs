using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerUnidadesDeMedidaAlternativasWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerUnidadesDeMedidaAlternativasConsumerMOA : IObtenerUnidadesDeMedidaAlternativasConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ObtenerUnidadesDeMedidaAlternativasConsumerMOA(IRepositorio repositorio)
        {
            this.repositorio = repositorio;

        }
        public List<UnidadesDeMedida> Request(string codigoMaterial)
        {
            return Request(new List<string> { codigoMaterial });
        }
        public List<UnidadesDeMedida> Request(List<string> codigosMaterial)
        {
            var result = new List<UnidadesDeMedida>();

            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new Z_MMRFC_UM_ALTERNATIVAS()
                    {
                        EX_UM_ALT = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMMTT_UM_ALT[] { },
                        IM_MATERIAL = codigosMaterial.Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.WSELMATNR { SIGN = "I", OPTION = "EQ", LOW = x }).ToArray()
                    };

                    Log.Info($"SAP sin PI Z_MMRFC_UM_ALTERNATIVAS request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MMRFC_UM_ALTERNATIVAS(request);
                    var unidadMedidaSap = repositorio.Listar<UnidadMedidaSap>();
                    Log.Info($"SAP sin PI Z_MMRFC_UM_ALTERNATIVAS response");
                    Log.Info(response.ToXml());
                    foreach (var umAlt in response.EX_UM_ALT)
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
                else
                {
                    SI_MMRFC_UM_ALTERNATIVASClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_UM_ALTERNATIVAS&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_UM_ALTERNATIVASClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    ObtenerUnidadesDeMedidaAlternativasWebServiceMOA.WSELMATNR[] IM_MATERIAL = new ObtenerUnidadesDeMedidaAlternativasWebServiceMOA.WSELMATNR[] { };
                    ObtenerUnidadesDeMedidaAlternativasWebServiceMOA.ZMMTT_UM_ALT[] EX_UM_ALT = new ObtenerUnidadesDeMedidaAlternativasWebServiceMOA.ZMMTT_UM_ALT[] { };


                    IM_MATERIAL = codigosMaterial.Select(x => new ObtenerUnidadesDeMedidaAlternativasWebServiceMOA.WSELMATNR { SIGN = "I", OPTION = "EQ", LOW = x }).ToArray();
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
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}