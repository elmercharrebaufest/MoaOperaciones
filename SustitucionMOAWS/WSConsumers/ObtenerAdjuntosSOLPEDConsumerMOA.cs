using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerAdjuntosSOLPEDWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    
    public class ObtenerAdjuntosSOLPEDConsumerMOA : IObtenerAdjuntosSOLPEDConsumerMOA
    {
        SI_MMRFC_ADJUNTOS_SOLPEDClient service;

        private readonly IRepositorio repositorio;
        public ObtenerAdjuntosSOLPEDConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_ADJUNTOS_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_ADJUNTOS_SOLPEDClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public byte[] ObtenerAdjuntosSolpConsumer(string archivoId, string nombreArchivo)
        {
            try
            {
                
                byte[] file = service.SI_MMRFC_ADJUNTOS_SOLPED(archivoId, nombreArchivo);

                return file;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}