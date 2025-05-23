using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerAdjuntosSOLPEDWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    
    public class ObtenerAdjuntosSOLPEDConsumerMOA : IObtenerAdjuntosSOLPEDConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ObtenerAdjuntosSOLPEDConsumerMOA(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public byte[] ObtenerAdjuntosSolpConsumer(string archivoId, string nombreArchivo)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MMRFC_ADJUNTOS_SOLPED(){
                         ID_ARCHIVO = archivoId,
                         NOMBRE_ARCHIVO = nombreArchivo,
                    };
                    Log.Info($"SAP sin PI Z_MMRFC_ADJUNTOS_SOLPED request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MMRFC_ADJUNTOS_SOLPED(request);

                    Log.Info($"SAP sin PI Z_MMRFC_ADJUNTOS_SOLPED response");
                    Log.Info(response.ToXml());

                    byte[] file = response.EX_CONT_BINARIO;
                    return file;
                }
                else
                {
                    SI_MMRFC_ADJUNTOS_SOLPEDClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_ADJUNTOS_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_ADJUNTOS_SOLPEDClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    byte[] file = service.SI_MMRFC_ADJUNTOS_SOLPED(archivoId, nombreArchivo);
                    return file;
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}