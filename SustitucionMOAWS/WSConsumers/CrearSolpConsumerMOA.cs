using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearSolpConsumerMOA : ICrearSolpConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public CrearSolpConsumerMOA(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public CrearSolpConsumerMOAResponse Request(SolpSAPDto solpSAP)
        {
            SI_MMRFC_CREAR_SOLPEDClient service;
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_CREAR_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_CREAR_SOLPEDClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

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
                                                       out CrearSolpWebServiceMOA.BAPIRETURN[] EX_RETURN);


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

        public CrearSolpConsumerMOAResponse RequestSinPI(SolpSAPSinPIDto solpSAPSinPI)
        {
            var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var serxml = new System.Xml.Serialization.XmlSerializer(solpSAPSinPI.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpSAPSinPI);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpSAPSinPI.Id, " - ", fecha, " - llamada crear.xml");
            var nombreArchivoRespuesta = string.Concat(solpSAPSinPI.Id, " - ", fecha, " - respuesta crear.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            var request = new Z_MMRFC_CREAR_SOLPED()
            {
                IM_PRACCOUNT = solpSAPSinPI.IM_PRACCOUNTList.ToArray(),
                IM_PRACCOUNTX = solpSAPSinPI.IM_PRACCOUNTXList.ToArray(),
                IM_PRADDRDELIVERY = solpSAPSinPI.IM_PRADDRDELIVERYList.ToArray(),
                IM_PRHEADERTEXT = solpSAPSinPI.IM_PRHEADERTEXTList.ToArray(),
                IM_PRITEM = solpSAPSinPI.IM_PRITEMList.ToArray(),
                IM_PRITEMTEXT = solpSAPSinPI.IM_PRITEMTEXTList.ToArray(),
                IM_PRITEMX = solpSAPSinPI.IM_PRITEMXList.ToArray(),
                IM_PR_TYPE = solpSAPSinPI.IM_PR_TYPE,
                IM_SERVICEACCOUNT = solpSAPSinPI.IM_SERVICEACCOUNTList.ToArray(),
                IM_SERVICEACCOUNTX = solpSAPSinPI.IM_SERVICEACCOUNTXList.ToArray(),
                IM_SERVICELINES = solpSAPSinPI.IM_SERVICELINESList.ToArray(),
                IM_SERVICELINESX = solpSAPSinPI.IM_SERVICELINESXList.ToArray(),
            };
            Log.Info($"SAP sin PI Z_MMRFC_CREAR_SOLPED request");
            Log.Info(request.ToXml());
            var response = agent.Z_MMRFC_CREAR_SOLPED(request);
            var respuesta = new CrearSolpConsumerMOAResponse();
            Log.Info($"SAP sin PI Z_MMRFC_CREAR_SOLPED response");
            Log.Info(response.ToXml());

            respuesta.NumeroSolp = response.EX_PREQ_NO;
            respuesta.Resultado = response.EX_EXITO;
            respuesta.Errores = new List<CrearSolpConsumerMOAError>();

            foreach (var errorSAP in response.EX_RETURN)
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
        public List<CrearSolpWebServiceMOA.BAPIMEREQACCOUNT> IM_PRACCOUNTList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPIMEREQACCOUNTX> IM_PRACCOUNTXList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.ZMPES7110> IM_PRADDRDELIVERYList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPIMEREQHEADTEXT> IM_PRHEADERTEXTList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.ZMPES7090> IM_PRITEMList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPIMEREQITEMTEXT> IM_PRITEMTEXTList { get; set; }
        public List<CrearSolpWebServiceMOA.ZMPES8000> IM_PRITEMXList { get; set; } //OK?
        public string IM_PR_TYPE { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPI_SRV_ACC_DATA> IM_SERVICEACCOUNTList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPI_SRV_ACC_DATAX> IM_SERVICEACCOUNTXList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPI_SRV_SERVICE_LINE> IM_SERVICELINESList { get; set; } //OK
        public List<CrearSolpWebServiceMOA.BAPI_SRV_SERVICE_LINEX> IM_SERVICELINESXList { get; set; } //OK?
        public string NroSolp { get; set; }
        public int Id { get; set; }

        public SolpSAPDto()
        {
            IM_PR_TYPE = "";
            IM_PRACCOUNTList  = new List<CrearSolpWebServiceMOA.BAPIMEREQACCOUNT>();
            IM_PRACCOUNTXList = new List<CrearSolpWebServiceMOA.BAPIMEREQACCOUNTX>();

            IM_PRADDRDELIVERYList = new List<CrearSolpWebServiceMOA.ZMPES7110>();
            IM_PRHEADERTEXTList = new List<CrearSolpWebServiceMOA.BAPIMEREQHEADTEXT>();

            IM_PRITEMList = new List<CrearSolpWebServiceMOA.ZMPES7090>();
            IM_PRITEMTEXTList = new List<CrearSolpWebServiceMOA.BAPIMEREQITEMTEXT>();

            IM_PRITEMXList = new List<CrearSolpWebServiceMOA.ZMPES8000>();

            IM_SERVICEACCOUNTList = new List<CrearSolpWebServiceMOA.BAPI_SRV_ACC_DATA>();
            IM_SERVICEACCOUNTXList = new List<CrearSolpWebServiceMOA.BAPI_SRV_ACC_DATAX>();
            IM_SERVICELINESList = new List<CrearSolpWebServiceMOA.BAPI_SRV_SERVICE_LINE>();
            IM_SERVICELINESXList = new List<CrearSolpWebServiceMOA.BAPI_SRV_SERVICE_LINEX>();
        }
    }
    public class SolpSAPSinPIDto
    {
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT> IM_PRACCOUNTList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNTX> IM_PRACCOUNTXList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7110> IM_PRADDRDELIVERYList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQHEADTEXT> IM_PRHEADERTEXTList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7090> IM_PRITEMList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQITEMTEXT> IM_PRITEMTEXTList { get; set; }
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES8000> IM_PRITEMXList { get; set; } //OK?
        public string IM_PR_TYPE { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATA> IM_SERVICEACCOUNTList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATAX> IM_SERVICEACCOUNTXList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINE> IM_SERVICELINESList { get; set; } //OK
        public List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINEX> IM_SERVICELINESXList { get; set; } //OK?
        public string NroSolp { get; set; }
        public int Id { get; set; }

        public SolpSAPSinPIDto()
        {
            IM_PR_TYPE = "";
            IM_PRACCOUNTList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT>();
            IM_PRACCOUNTXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNTX>();

            IM_PRADDRDELIVERYList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7110>();
            IM_PRHEADERTEXTList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQHEADTEXT>();

            IM_PRITEMList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7090>();
            IM_PRITEMTEXTList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQITEMTEXT>();

            IM_PRITEMXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES8000>();

            IM_SERVICEACCOUNTList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATA>();
            IM_SERVICEACCOUNTXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATAX>();
            IM_SERVICELINESList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINE>();
            IM_SERVICELINESXList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINEX>();
        }
    }

    public interface ICrearSolpConsumerMOA
    {
        CrearSolpConsumerMOAResponse Request(SolpSAPDto solpSAP);
        CrearSolpConsumerMOAResponse RequestSinPI(SolpSAPSinPIDto solpSAPSinPI);

    }
}
