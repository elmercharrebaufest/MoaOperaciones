using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearSolpConsumerMOA : ICrearSolpConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly SI_MMRFC_CREAR_SOLPEDClient service;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];

        public CrearSolpConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_CREAR_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_CREAR_SOLPEDClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public CrearSolpConsumerMOAResponse Request(SolpSAPDto solpSAP)
        {
            var serxml = new System.Xml.Serialization.XmlSerializer(solpSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpSAP.Id, " - ", fecha, " - llamada crear.xml");
            var nombreArchivoRespuesta = string.Concat(solpSAP.Id, " - ", fecha, " - respuesta crear.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            var result = service.SI_MMRFC_CREAR_SOLPED(solpSAP.IM_PRACCOUNTList.ToArray(),
                                                       solpSAP.IM_PRACCOUNTXList.ToArray(),
                                                       solpSAP.IM_PRADDRDELIVERYList.ToArray(),
                                                       solpSAP.IM_PRHEADERTEXTList.ToArray(),
                                                       solpSAP.IM_PRITEMList.ToArray(),
                                                       solpSAP.IM_PRITEMTEXTList.ToArray(),
                                                       solpSAP.IM_PRITEMXList.ToArray(),
                                                       solpSAP.IM_PR_TYPE,
                                                       solpSAP.IM_SERVICEACCOUNTList.ToArray(),
                                                       solpSAP.IM_SERVICEACCOUNTXList.ToArray(),
                                                       solpSAP.IM_SERVICELINESList.ToArray(),
                                                       solpSAP.IM_SERVICELINESXList.ToArray(),
                                                       out string EX_PREQ_NO,
                                                       out BAPIRETURN[] EX_RETURN);


            var respuesta = new CrearSolpConsumerMOAResponse();

            respuesta.NumeroSolp = EX_PREQ_NO;
            respuesta.Resultado = result;
            respuesta.Errores = new List<CrearSolpConsumerMOAError>();

            foreach (var errorSAP in EX_RETURN)
            {
                var error = new CrearSolpConsumerMOAError
                {
                    Codigo = errorSAP.CODE,
                    Mensaje = errorSAP.MESSAGE,
                    Tipo = errorSAP.TYPE
                };

                respuesta.Errores.Add(error);
            }

            var jsonRespuesta = JsonConvert.SerializeObject(respuesta);

            FileInfo fileRespuesta = new FileInfo(rutaArchivoRespuesta);
            fileRespuesta.Directory.Create();
            File.WriteAllText(fileRespuesta.FullName, jsonRespuesta);

            return respuesta;
        }
    }

    public class CrearSolpConsumerMOAResponse
    {
        public string NumeroSolp { get; set; }
        public List<CrearSolpConsumerMOAError> Errores { get; set; }
        public string Resultado { get; set; }
    }

    public class CrearSolpConsumerMOAError
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }

    public class SolpSAPDto
    {
        public List<ZMPES5690> IM_PRACCOUNTList { get; set; } //OK
        public List<ZMPES5680> IM_PRACCOUNTXList { get; set; } //OK
        public List<ZMPES5750> IM_PRADDRDELIVERYList { get; set; } //OK
        public List<BAPIMEREQHEADTEXT> IM_PRHEADERTEXTList { get; set; } //OK
        public List<ZMPES5700> IM_PRITEMList { get; set; } //OK
        public List<BAPIMEREQITEMTEXT> IM_PRITEMTEXTList { get; set; }
        public List<ZMPES5660> IM_PRITEMXList { get; set; } //OK?
        public string IM_PR_TYPE { get; set; } //OK
        public List<ZMPES5790> IM_SERVICEACCOUNTList { get; set; } //OK
        public List<BAPI_SRV_ACC_DATAX> IM_SERVICEACCOUNTXList { get; set; } //OK
        public List<ZMPES5780> IM_SERVICELINESList { get; set; } //OK
        public List<ZMPES5720> IM_SERVICELINESXList { get; set; } //OK?
        public string NroSolp { get; set; }
        public int Id { get; set; }

        public SolpSAPDto()
        {
            IM_PR_TYPE = "";
            IM_PRACCOUNTList = new List<ZMPES5690>();
            IM_PRACCOUNTXList = new List<ZMPES5680>();

            IM_PRADDRDELIVERYList = new List<ZMPES5750>();
            IM_PRHEADERTEXTList = new List<BAPIMEREQHEADTEXT>();

            IM_PRITEMList = new List<ZMPES5700>();
            IM_PRITEMTEXTList = new List<BAPIMEREQITEMTEXT>();

            IM_PRITEMXList = new List<ZMPES5660>();

            IM_SERVICEACCOUNTList = new List<ZMPES5790>();
            IM_SERVICEACCOUNTXList = new List<BAPI_SRV_ACC_DATAX>();
            IM_SERVICELINESList = new List<ZMPES5780>();
            IM_SERVICELINESXList = new List<ZMPES5720>();
        }
    }

    public interface ICrearSolpConsumerMOA
    {
        CrearSolpConsumerMOAResponse Request(SolpSAPDto solpSAP);

    }
}
