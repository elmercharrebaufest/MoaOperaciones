using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using SustitucionMOAModel.Util;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ModificarEntregaOrdenFasWebServiceMOA;
using SustitucionMOAWS.ModificarOrdenCargaFasWebServiceMOA;
using SustitucionMOAWS.OrdenCargaControlEstadoSAP;
using SustitucionMOAWS.OrdenCargaControlSAP;
using SustitucionMOAWS.OrdenCargaCrearSAP;
using SustitucionMOAWS.OrdenCargaEstadoEntregadaSAP;
using SustitucionMOAWS.OrdenCargaVisualizarCliente;
using SustitucionMOAWS.ResponseHandler.OrdenCarga;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using SustitucionMOAWS.WSRequests.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class OrdenCargaConsumerMOA : IOrdenCargaConsumerMOA
    {

        /*
         * En este punto se utiliza la RFC Z_MPMF_MOAOP_CONTROL_CARGA enviando: 
            http://gslopidevqa00.molinosagro.ad:50000/dir/wsdl?p=ic/a2b857542ded3d3fbf5d19b15e1cd93a

            IM_CORREDOR	Opcional	Número de corredor (LIFNR)
            IM_CLIENTE	Obligatorio	Número de Cliente (LIFNR)
            IM_CUIT	Obligatorio	CUIT de Transportista (STCD1)
            IM_CONTRATO	Obligatorio	Número de Contrato (VBELN)
            IM_PEDIDO	Opcional	Número de Pedido (VBELN)
            IM_MATERIAL	Obligatorio	Numero SAP de Material (MATNR)

            •	Con esta función se verificará que para la combinación de cliente, corredor y material el contrato sea único. En caso de no serlo devolverá el mensaje: 'Más de un contrato vigente para Cliente/Corredor'
            (Lo estamos editando para devolver la lista de contratos disponibles)
            •	Se validará que la CUIT enviada corresponda a un transportista dado de alta, en caso contrario, devolverá: 'Transportista no dado de alta'
            •	Se verifica la existencia del pedido para el cliente, devolviendo 'Verificar Pedido' caso de no existir.
            •	En caso que el pedido esté bloqueado por crédito, devolverá 'Verificar Crédito de pedido'.
            •	Si el pedido estuviera agotado, 'Pedido entregado completamente'.
            •	En caso que todas las validaciones fueran correctas, devuelve 'OK'
            Todo lo anterior se expresa en el campo de salida EX_MENSAJE.

        Función	Código	Mensaje reemplazado
        Z_MPMF_MOAOP_CONTROL_CARGA 	CC-01	'Más de un contrato vigente para Cliente/Corredor'
        Z_MPMF_MOAOP_CONTROL_CARGA 	CC-02	'Transportista no dado de alta'
        Z_MPMF_MOAOP_CONTROL_CARGA 	CC-03	'Verificar Pedido' 
        Z_MPMF_MOAOP_CONTROL_CARGA 	CC-04	'Verificar Crédito de pedido'
        Z_MPMF_MOAOP_CONTROL_CARGA 	CC-05	'Pedido entregado completamente'
        Z_MPMF_MOAOP_CONTROL_CARGA 	CC-00	'OK'

        */

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        private const string TipoContratoFas_Normal = "NORMAL";
        private const string TipoContratoFas_Anticipado = "ANTICIPADO";
        private const string TipoContratoFas_Todos = "";

        public ControlCargaResponseHandler ControlarCarga(ControlCargaRequest datosCarga)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var request = new Z_MPMF_MOAOP_CONTROL_CARGA()
                {
                    IM_CLIENTE = string.IsNullOrWhiteSpace(datosCarga.Cliente) ? "" : datosCarga.Cliente,
                    IM_CONTRATO = string.IsNullOrWhiteSpace(datosCarga.Contrato) ? "" : datosCarga.Contrato,
                    IM_CORREDOR = string.IsNullOrWhiteSpace(datosCarga.Corredor) ? "" : datosCarga.Corredor,
                    IM_CUIT = string.IsNullOrWhiteSpace(datosCarga.Cuit) ? "" : datosCarga.Cuit,
                    IM_CUITDESTF = string.IsNullOrWhiteSpace(datosCarga.CuitDestino) ? "" : datosCarga.CuitDestino,
                    IM_CUITDESTINAT = string.IsNullOrWhiteSpace(datosCarga.CuitDestinatario) ? "" : datosCarga.CuitDestinatario,
                    IM_MATERIAL = string.IsNullOrWhiteSpace(datosCarga.Material) ? "" : datosCarga.Material,
                    IM_PEDIDO = string.IsNullOrWhiteSpace(datosCarga.Pedido) ? "" : datosCarga.Pedido,
                    IM_SOLO_SISA = datosCarga.SoloSisa ? "X" : ""
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_CONTROL_CARGA request");
                Log.Info(request.ToXml());

                var response = agent.Z_MPMF_MOAOP_CONTROL_CARGA(request);

                Log.Info($"SAP sin PI Z_MPMF_MOAOP_CONTROL_CARGA response");
                Log.Info(response.ToXml());

                return new ControlCargaResponseHandler(response.EX_MENSAJE);


            }
            else
            {
                var service = new SI_MPMF_MOAOP_CONTROL_CARGAClient();

                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Log.Info($"SI_MPMF_MOAOP_CONTROL_CARGA Request: {datosCarga.ToJson()}");

                var result = service.SI_MPMF_MOAOP_CONTROL_CARGA(
                    IM_CLIENTE: datosCarga.Cliente,
                    IM_CONTRATO: datosCarga.Contrato,
                    IM_CORREDOR: datosCarga.Corredor,
                    IM_CUIT: datosCarga.Cuit,
                    IM_CUITDESTF: datosCarga.CuitDestino,
                    IM_CUITDESTINAT: datosCarga.CuitDestinatario,
                    IM_MATERIAL: datosCarga.Material,
                    IM_PEDIDO: datosCarga.Pedido,
                    IM_SOLO_SISA: datosCarga.SoloSisa ? "X" : "");

                Log.Info($"SI_MPMF_MOAOP_CONTROL_CARGA Result: {result.ToJson()}");
                return new ControlCargaResponseHandler(result);
            }
        }
        /*
        * RFC Z_MPMF_MOAOP_CREAR_ORDEN_CARGA con:
        http://gslopidevqa00.molinosagro.ad:50000/dir/wsdl?p=ic/5147d92447383f65943ba1a0023727db

        IM_CORREDOR	Opcional	Número de corredor (LIFNR)
        IM_CLIENTE	Obligatorio	Número de Cliente (LIFNR)
        IM_CONTRATO	Obligatorio	Número de Contrato (VBELN)
        IM_PEDIDO	Opcional	Número de Pedido (VBELN) (no se utilizaría)
        IM_MATERIAL	Obligatorio	Numero SAP de Material (MATNR)
        IM_KILOS	Obligatorio	Kilos solicitados (DZMENG)


        •	Con esta función se verificará que para la combinación de cliente, contrato y material exista el contrato. En caso de no encontrarlo devolverá el mensaje: 'Verificar Contrato, Material, Cliente'
        •	Se validará que el contrato cuente con la cantidad pendiente disponible suficiente para cubrir al pedido. En caso contrario se devolverá: 'Verificar cantidad pendiente de Contratada'
        •	Se Procederá a crear el pedido y en caso de ser satisfactorio se validará si el mismo se crea con bloqueo por crédito. En este caso se devolverá: 'Pedido creado - Verificar Crédito de pedido'  En caso de no nacer bloqueado, se devolverá 'OK'.
        •	En campo aparte se devuelve el Número de pedido.

        Función	Código	Mensaje reemplazado
        Z_MPMF_MOAOP_CREAR_ORDEN_CARGA 	OV-01	'Verificar Contrato, Material, Cliente'
        Z_MPMF_MOAOP_CREAR_ORDEN_CARGA 	OV-02	'Verificar cantidad pendiente de Contratada'
        Z_MPMF_MOAOP_CREAR_ORDEN_CARGA 	OV-03	'Pedido creado - Verificar Crédito de pedido'  
        Z_MPMF_MOAOP_CREAR_ORDEN_CARGA 	OV-00	'OK'


        */

        public CrearOrdenResEnum CrearOrden(CrearOrdenRequest req, out string pedidoOutput, out string resultOutput)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                try
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_CREAR_ORDEN_CARGA()
                    {
                        IM_CLIENTE = string.IsNullOrWhiteSpace(req.Cliente) ? "" : req.Cliente,
                        IM_CODPLANTA = string.IsNullOrWhiteSpace(req.PlantaCodigo) ? "" : req.PlantaCodigo,
                        IM_CONTRATO = string.IsNullOrWhiteSpace(req.Contrato) ? "" : req.Contrato,
                        IM_CORREDOR = string.IsNullOrWhiteSpace(req.Corredor) ? "" : req.Corredor,
                        IM_CUITDESTF = string.IsNullOrWhiteSpace(req.CuitDestino) ? "" : req.CuitDestino,
                        IM_CUITDESTINAT = string.IsNullOrWhiteSpace(req.CuitDestinatario) || req.CuitDestinatario == req.CuitCliente ? "" : req.CuitDestinatario,
                        IM_DOMORDEN = string.IsNullOrWhiteSpace(req.DomicilioDescr) ? "" : req.DomicilioDescr,
                        IM_INDRVTA = req.Reventa ? "X" : "",
                        IM_KILOS = req.Kilos,
                        IM_MATERIAL = string.IsNullOrWhiteSpace(req.Material) ? "" : req.Material,
                        IM_NAMEDESTF = string.IsNullOrWhiteSpace(req.RazonSocialDestino) ? "" : req.RazonSocialDestino,
                        IM_NAMEDESTINAT = string.IsNullOrWhiteSpace(req.RazonSocialDestinatario) ? "" : req.RazonSocialDestinatario,
                        IM_ORDENDOM = string.IsNullOrWhiteSpace(req.DomicilioOrden?.ToString()) ? "" : req.DomicilioOrden.ToString(),
                        IM_PEDIDO = string.IsNullOrWhiteSpace(req.PedidoInput) ? "" : req.PedidoInput,
                        IM_TIPODOM = string.IsNullOrWhiteSpace(req.DomicilioTipo) ? "" : req.DomicilioTipo,
                        IM_USUARIO = string.IsNullOrWhiteSpace(req.UsuarioSAP) ? "" : req.UsuarioSAP,
                        IM_VALIDA_KG = req.ValidaKg ? "X" : "",
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CREAR_ORDEN_CARGA request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_CREAR_ORDEN_CARGA(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_CREAR_ORDEN_CARGA response");
                    Log.Info(response.ToXml());

                    pedidoOutput = response.EX_PEDIDO;
                    resultOutput = response.EX_MENSAJE;
                    return ResponseConverter.GetOrdenCargaCrearOrden(response.EX_MENSAJE);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    throw;
                }

            }
            else
            {
                var service = new SI_MPMF_MOAOP_CREAR_ORDEN_CARGAClient();
                var indrvta = req.Reventa ? "X" : "";
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Log.Info($"SI_MPMF_MOAOP_CREAR_ORDEN_CARGA Request: {req.ToJson()}");

                if (req.CuitDestino == req.CuitDestinatario)
                {
                    req.CuitDestinatario = string.Empty;
                    req.RazonSocialDestinatario = string.Empty;
                }

                var result = service.SI_MPMF_MOAOP_CREAR_ORDEN_CARGA(
                    IM_CLIENTE: req.Cliente,
                    IM_CODPLANTA: req.PlantaCodigo,
                    IM_CONTRATO: req.Contrato,
                    IM_CORREDOR: req.Corredor,
                    IM_CUITDESTF: string.IsNullOrWhiteSpace(req.CuitDestino) ? "" : req.CuitDestino,
                    IM_CUITDESTINAT: req.CuitDestinatario,
                    IM_DOMORDEN: req.DomicilioDescr,
                    IM_INDRVTA: indrvta,//IM_INDRVTA
                    IM_KILOS: req.Kilos,
                    IM_MATERIAL: req.Material,
                    IM_NAMEDESTF: req.RazonSocialDestino,
                    IM_NAMEDESTINAT: req.RazonSocialDestinatario,
                    IM_ORDENDOM: req.DomicilioOrden.ToString(),
                    IM_PEDIDO: req.PedidoInput,
                    IM_TIPODOM: req.DomicilioTipo,
                    IM_USUARIO: req.UsuarioSAP,
                    IM_VALIDA_KG: req.ValidaKg ? "X" : "",
                    EX_PEDIDO: out pedidoOutput).Trim();

                Log.Info($"SI_MPMF_MOAOP_CREAR_ORDEN_CARGA Response: {new { result, pedidoOutput }}");
                resultOutput = result;

                return ResponseConverter.GetOrdenCargaCrearOrden(result);
            }
        }



        /*
            * RFC Z_SI_MPMF_MOAOP_ORD_CARGA_ENT_V2 
        http://gslopidevqa00.molinosagro.ad:50000/dir/wsdl?p=ic/307fa30560243c4f9617b38cff85b111
        enviando:
        IM_PEDIDO	Obligatorio	Número de Pedido (VBELN)
        IM_TRANSPORTISTA	Obligatorio	CUIT de Transportista (STCD1)
        IM_PATENTECHASIS	Obligatorio	Patente chasis (Char07)
        IM_PATENTEACOPLADO	Obligatorio	Patente Acoplado (Char07)
        IM_NOMBRECONDUCTOR	Obligatorio	Nombre completo (Char40)
        IM_TIPODOCUMENTO	Obligatorio	Tipo Doc (CHAR05)
        IM_DOCUMENTO	Obligatorio	Nro Documento (CHAR15)
        IM_KILOS	Obligatorio	Kilos de entrega (LFIMG)

        •	Con esta función se verificará El transportista por CUIT: 'No existe tranportista'
        •	Se Generará la entrega, en caso de éxito se devolverá: 'OK'
        •	Se Procederá a guardar los textos correspondientes a los datos de Carga (Nombre, Documento, patentes, etc). En caso de error: 'Entrega Creada - Error al insertar'
        •	En campo aparte se devuelve el Número de Entrega

        Función	Código	Mensaje reemplazado
        Z_SI_MPMF_MOAOP_ORD_CARGA_ENT_V2 	OE-00	'OK'
        Z_SI_MPMF_MOAOP_ORD_CARGA_ENT_V2 	OE-01	'No existe tranportista'
        Z_SI_MPMF_MOAOP_ORD_CARGA_ENT_V2 	OE-02	'Entrega Creada - Error al insertar'

        */
        public OrdenCargaEntreResponseHandler CrearEntrega(CrearEntregaRequest entregaReq, bool pedidoAnticipado = false)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var cuit_tr = !string.IsNullOrEmpty(entregaReq.TransportistaReal) ? entregaReq.TransportistaReal : entregaReq.Transportista;
                var cuit_int_flete = !string.IsNullOrEmpty(entregaReq.TransportistaReal) ? entregaReq.Transportista : entregaReq.TransportistaReal;
                if (!pedidoAnticipado)
                    entregaReq = LimpiarRequestSinPedidoAnticipado(entregaReq);

                var request = new Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE()
                {
                    IM_CUITDESTF = string.IsNullOrWhiteSpace(entregaReq.CuitDestino) ? "" : entregaReq.CuitDestino,
                    IM_CUITDESTINAT = string.IsNullOrWhiteSpace(entregaReq.CuitDestinatario) ? "" : entregaReq.CuitDestinatario,
                    IM_DOCUMENTO = string.IsNullOrWhiteSpace(entregaReq.Documento) ? "" : entregaReq.Documento,
                    IM_DOMORDEN = string.IsNullOrWhiteSpace(entregaReq.DomicilioDescr) ? "" : entregaReq.DomicilioDescr,
                    IM_INDRVTA = entregaReq.Reventa ? "X" : "",
                    IM_KILOS = entregaReq.Kilos,
                    IM_NAMEDESTF = string.IsNullOrWhiteSpace(entregaReq.RazonSocialDestino) ? "" : entregaReq.RazonSocialDestino,
                    IM_NAMEDESTINAT = string.IsNullOrWhiteSpace(entregaReq.RazonSocialDestinatario) ? "" : entregaReq.RazonSocialDestinatario,
                    IM_NOMBRECONDUCTOR = string.IsNullOrWhiteSpace(entregaReq.NombreConductor) ? "" : entregaReq.NombreConductor,
                    IM_ORDENDOM = entregaReq.DomicilioOrden == null ? "" : entregaReq.DomicilioOrden.ToString(),
                    IM_PATENTEACOPLADO = string.IsNullOrWhiteSpace(entregaReq.PatenteAcoplado) ? "" : entregaReq.PatenteAcoplado,
                    IM_PATENTECHASIS = string.IsNullOrWhiteSpace(entregaReq.PatenteChasis) ? "" : entregaReq.PatenteChasis,
                    IM_PEDIDO = string.IsNullOrWhiteSpace(entregaReq.Pedido) ? "" : entregaReq.Pedido,
                    IM_TIPODOCUMENTO = string.IsNullOrWhiteSpace(entregaReq.TipoDocumento) ? "" : entregaReq.TipoDocumento,
                    IM_TIPODOM = string.IsNullOrWhiteSpace(entregaReq.DomicilioTipo) ? "" : entregaReq.DomicilioTipo,
                    IM_TRANSPORTISTA = string.IsNullOrWhiteSpace(cuit_tr) ? "" : cuit_tr,
                    IM_TRANSPORTISTA_REAL = string.IsNullOrWhiteSpace(cuit_int_flete) ? "" : cuit_int_flete,
                    IM_USUARIO = "CACERESN",
                    IM_ZZCODPLANTA = string.IsNullOrWhiteSpace(entregaReq.PlantaCodigo) ? "" : entregaReq.PlantaCodigo,
                    IM_DESTINO_MERCADERIA = string.IsNullOrWhiteSpace(entregaReq.DestinoMercaderia) ? "" : entregaReq.DestinoMercaderia,
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE request");
                Log.Info(request.ToXml());

                var response = agent.Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE(request);
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE response");
                Log.Info(response.ToXml());

                return new OrdenCargaEntreResponseHandler(response.EX_MENSAJE, response.EX_ENTREGA);
            }
            else
            {
                var service = new SI_MPMF_MOAOP_ORDEN_CARGA_ENTREClient();
                var cuit_tr = !string.IsNullOrEmpty(entregaReq.TransportistaReal) ? entregaReq.TransportistaReal : entregaReq.Transportista;
                var cuit_int_flete = !string.IsNullOrEmpty(entregaReq.TransportistaReal) ? entregaReq.Transportista : entregaReq.TransportistaReal;
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                if (!pedidoAnticipado)
                    entregaReq = LimpiarRequestSinPedidoAnticipado(entregaReq);

                Log.Info($"SI_SI_MPMF_MOAOP_ORDEN_CARGA_ENTRE " +
                    $"Request: {entregaReq.ToJson()}, " +
                    $"pedidoAnticipado: {pedidoAnticipado}, " +
                    $"cuit_tr: {cuit_tr}, " +
                    $"cuit_int_flete: {cuit_int_flete}.");

                var entrega = service.SI_MPMF_MOAOP_ORDEN_CARGA_ENTRE(
                    IM_CUITDESTF: entregaReq.CuitDestino,
                    IM_CUITDESTINAT: entregaReq.CuitDestinatario,
                    IM_DOCUMENTO: entregaReq.Documento,
                    IM_DOMORDEN: entregaReq.DomicilioDescr,
                    IM_INDRVTA: entregaReq.Reventa ? "X" : "",
                    IM_KILOS: entregaReq.Kilos,
                    IM_NAMEDESTF: entregaReq.RazonSocialDestino,
                    IM_NAMEDESTINAT: entregaReq.RazonSocialDestinatario,
                    IM_NOMBRECONDUCTOR: entregaReq.NombreConductor,
                    IM_ORDENDOM: entregaReq.DomicilioOrden == null ? "" : entregaReq.DomicilioOrden.ToString(),
                    IM_PATENTEACOPLADO: entregaReq.PatenteAcoplado,
                    IM_PATENTECHASIS: entregaReq.PatenteChasis,
                    IM_PEDIDO: entregaReq.Pedido,
                    IM_TIPODOCUMENTO: entregaReq.TipoDocumento,
                    IM_TIPODOM: entregaReq.DomicilioTipo,
                    IM_TRANSPORTISTA: cuit_tr,
                    IM_TRANSPORTISTA_REAL: cuit_int_flete,
                    IM_USUARIO: "CACERESN",
                    IM_ZZCODPLANTA: entregaReq.PlantaCodigo,
                    IM_DESTINO_MERCADERIA: entregaReq.DestinoMercaderia,
                    EX_MENSAJE: out string mensaje).Trim();

                Log.Info($"SI_SI_MPMF_MOAOP_ORDEN_CARGA_ENTRE Response: {new { entrega, mensaje }}");

                return new OrdenCargaEntreResponseHandler(mensaje, entrega);
            }
        }

        /* 
         * la RFC Z_MPMF_MOAOP_CONTROL_ESTADO enviando: 
        http://gslopidevqa00.molinosagro.ad:50000/dir/wsdl?p=ic/43f6042718883d66ac3844c249b34cbe

        IM_PEDIDO	Opcional	Número de Pedido (VBELN)
        IM_ENTREGA	Opcional	Número de Entrega (VBELN)
        IM_TRANSPORTISTA	Opcional	CUIT de Transportista (STCD1)
        Cada Campo es opcional y se deberá verificar de a uno por vez de acuerdo al estado en web.
        •	Con esta función se validará que Se ingrese al menos un campo. En caso de no serlo devolverá el mensaje: 'Ingrese al menos una selección.'

        •	Si el pedido no existe, devolverá: 'Pedido no encontrado'
        •	Si el Pedido, está bloqueado por crédito, devolverá: 'Verificar Crédito de pedido'
        •	Si estuviera completamente entregado:
        'Pedido entregado completamente'
        •	Caso contrario devolverá 'OK'
        •	Para las entregas, De no encontrarse, devolverá: 'Entrega no encontrada'
        •	Si fuese totalmente entregada: 'Entrega completada'
        •	Para el transportista, 'Transportista OK' o 'Transportista no dado de alta' Para actualizar el estado del transportista.

        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-00	'OK'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-01	'Ingrese al menos una selección.'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-02	'Pedido no encontrado'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-03	'Verificar Crédito de pedido'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-04	'Pedido entregado completamente'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-05	'Entrega no encontrada'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-06	'Entrega completada'
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-07	'Transportista OK' 
        Z_MPMF_MOAOP_CONTROL_ESTADO 	CE-08	'Transportista no dado de alta' 

        */

        public string OrdenCargaControlEstadoRequest(string entrega, string pedido, string transportista)
        {
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                var request = new Z_MPMF_MOAOP_CONTROL_ESTADO()
                {
                    IM_ENTREGA = entrega,
                    IM_PEDIDO = pedido,
                    IM_TRANSPORTISTA = transportista
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_CONTROL_ESTADO request");
                Log.Info(request.ToXml());
                var response = agent.Z_MPMF_MOAOP_CONTROL_ESTADO(request);
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_CONTROL_ESTADO response");
                Log.Info(response.ToXml());
                return response.EX_SALIDA;
            }
            else
            {
                var service = new SI_MPMF_MOAOP_CONTROL_ESTADOClient();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                Log.Info($"SI_MPMF_MOAOP_CONTROL_ESTADO Request: {new { entrega, pedido, transportista }}");
                var result = service.SI_MPMF_MOAOP_CONTROL_ESTADO(entrega, pedido, transportista).Trim();
                Log.Info($"SI_MPMF_MOAOP_CONTROL_ESTADO Result: {new { result, entrega, pedido, }}");

                return result;
            }
        }

        public OrdenCargaVisualizarClienteWSMOAResponse OrdenCargaVisualizarClienteExecute(OrdenCargaVisualizarClienteWSMOARequest request)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100> fechasSAP = new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>() { };
                    if (request.Fechas != null)
                    {
                        foreach (var fecha in request.Fechas)
                        {
                            fechasSAP.Add(new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
                            {
                                FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                                FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                            });
                        }
                    }
                    string tipoContrato = ConvertirATipoContratoFasSAP(request.TipoContrato);
                    tipoContrato = tipoContrato.Length > 0 ? tipoContrato.Substring(0, 1) : tipoContrato;
                    WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();

                    var requestFas = new Z_MPMF_MOAOP_VISUALIZAR_ZFAS()
                    {
                        IM_CLIENTE = request.Cliente,
                        IM_CONTRATO = request.Contrato,
                        IM_CORREDOR = request.Corredor,
                        IM_FECHA = fechasSAPArray,
                        IM_MATERIAL = request.Material,
                        IM_PENDIENTE = request.Pendiente ? "X" : "",
                        IM_TIPO_CONTRATO = tipoContrato,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VISUALIZAR_ZFAS request");
                    Log.Info(requestFas.ToXml());

                    var response = agent.Z_MPMF_MOAOP_VISUALIZAR_ZFAS(requestFas);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_VISUALIZAR_ZFAS response");
                    Log.Info(response.ToXml());

                    var responseFAS = MapOrdenCargaVisualizarCliente(response.EX_SALIDA);
                    return responseFAS;
                }
                else
                {
                    var service = new SI_MPMF_MOAOP_VISUALIZAR_ZFASClient();

                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    List<OrdenCargaVisualizarCliente.ZMPES4100> fechasSAP = new List<OrdenCargaVisualizarCliente.ZMPES4100>() { };
                    if (request.Fechas != null)
                    {
                        foreach (var fecha in request.Fechas)
                        {
                            fechasSAP.Add(new OrdenCargaVisualizarCliente.ZMPES4100()
                            {
                                FECHA_OP = SAPFormatter.PrepararFecha(fecha.fechaInicio),
                                FECHA_OP_HASTA = SAPFormatter.PrepararFecha(fecha.fechaFin)
                            });
                        }
                    }
                    string tipoContrato = ConvertirATipoContratoFasSAP(request.TipoContrato);
                    OrdenCargaVisualizarCliente.ZMPES4100[] fechasSAPArray = fechasSAP.ToArray();
                    var result = service.SI_MPMF_MOAOP_VISUALIZAR_ZFAS(
                        request.Cliente,
                        request.Contrato,
                        request.Corredor,
                        fechasSAPArray,
                        request.Material,
                        request.Pendiente ? "X" : "",
                        tipoContrato);
                    var response = MapOrdenCargaVisualizarCliente(result);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        protected virtual OrdenCargaVisualizarClienteWSMOAResponse MapOrdenCargaVisualizarCliente(OrdenCargaVisualizarCliente.ZMPES6750[] result)
        {
            var response = new OrdenCargaVisualizarClienteWSMOAResponse();
            var resultados = new List<SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Result>();
            foreach (var item in result)
            {
                var resultado = new SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Result()
                {
                    Contrato = item.CONTRATO,
                    PedidoCliente = item.PEDIDO_CLIENTE,
                    PosNr = item.POSNR,
                    Cliente = item.CLIENTE,
                    CuitCliente = item.CUIT_CLIENTE,
                    NombreCliente = item.NOMBRE_CLIENTE,
                    Corredor = item.CORREDOR,
                    DescripcionMaterial = item.DESC_MATERIAL,
                    KilosTotales = item.KILOS_TOTALES,
                    KilosEntregados = item.KILOS_ENTREGADOS,
                    KilosFacturados = item.KILOS_FACTURADOS,
                    KilosPendienteEntrega = item.KILOS_PEND_ENTREGA,
                    KilosPendienteFactura = item.KILOS_PEND_FACTURA,
                    KilosTotalesStr = SAPFormatter.FormatearCantidad(item.KILOS_TOTALES, "KG"),
                    KilosEntregadosStr = SAPFormatter.FormatearCantidad(item.KILOS_ENTREGADOS, "KG"),
                    KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(item.KILOS_PEND_ENTREGA, "KG"),
                    FechaDesde = SAPFormatter.FormatearFecha(item.FECHA_DESDE),
                    FechaHasta = item.FECHA_HASTA,
                    Precio = item.PRECIO,
                    Moneda = item.MONEDA,
                    Motivo = item.MOTIVO,
                    DetalleMotivo = item.DET_MOTIVO,
                    CondicionEntrega = item.CONDICION_ENTREGA,
                    Producto = item.PRODUCTO,
                    PuntoExpedicion = item.PTO_EXPEDICION,
                    TipoContrato = ConvertirDeTipoContratoFasSAP(item.TIPO_CONTRATO),
                    PrecioFlete = item.PRECIO_FLETE,
                    BloqueoEntrega = item.BLOQUEO_ENTREGA == "X"
                };
                var detalles = new List<SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Detail>();
                foreach (var detalle in item.DETALLE)
                {
                    detalles.Add(new SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Detail()
                    {
                        Pedido = detalle.PEDIDO,
                        Entrega = detalle.ENTREGA,
                        FechaPedido = detalle.FECHA_PEDIDO,
                        FechaCarga = detalle.FECHA_CARGA,
                        CantidadEntregada = detalle.CANTIDAD_ENTREGADA,
                        Remito = detalle.REMITO,
                        Factura = detalle.FACTURA,
                        CantidadFactura = detalle.CANTIDAD_FACTURA,
                        FacturaLegal = detalle.FACTURA_LEGAL,
                        Chasis = detalle.CHASIS,
                        Acoplado = detalle.ACOPLADO,
                        Chofer = detalle.CHOFER,
                        Destinatario = detalle.DESTINATARIO,
                        NombreDestinatario = detalle.NOMBRE_DESTINATARIO,
                        KilosEntrega = detalle.KILOS_ENTREGA,
                        BloqueoEntrega = detalle.BLOQUEO_ENTREGA == "X"
                    });
                }
                if (detalles != null)
                {
                    if (detalles.Count > 0)
                    {
                        resultado.Detalles = detalles;
                    }
                }
                resultados.Add(resultado);
            }

            if (resultados != null)
            {
                if (resultados.Count > 0)
                {

                    response.Resultados = resultados;
                }
            }
            return response;
        }
        protected virtual OrdenCargaVisualizarClienteWSMOAResponse MapOrdenCargaVisualizarCliente(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES6750[] result)
        {
            var response = new OrdenCargaVisualizarClienteWSMOAResponse();
            var resultados = new List<SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Result>();
            foreach (var item in result)
            {
                var resultado = new SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Result()
                {
                    Contrato = item.CONTRATO,
                    PedidoCliente = item.PEDIDO_CLIENTE,
                    PosNr = item.POSNR,
                    Cliente = item.CLIENTE,
                    CuitCliente = item.CUIT_CLIENTE,
                    NombreCliente = item.NOMBRE_CLIENTE,
                    Corredor = item.CORREDOR,
                    DescripcionMaterial = item.DESC_MATERIAL,
                    KilosTotales = item.KILOS_TOTALES,
                    KilosEntregados = item.KILOS_ENTREGADOS,
                    KilosFacturados = item.KILOS_FACTURADOS,
                    KilosPendienteEntrega = item.KILOS_PEND_ENTREGA,
                    KilosPendienteFactura = item.KILOS_PEND_FACTURA,
                    KilosTotalesStr = SAPFormatter.FormatearCantidad(item.KILOS_TOTALES, "KG"),
                    KilosEntregadosStr = SAPFormatter.FormatearCantidad(item.KILOS_ENTREGADOS, "KG"),
                    KilosPendienteEntregaStr = SAPFormatter.FormatearCantidad(item.KILOS_PEND_ENTREGA, "KG"),
                    FechaDesde = SAPFormatter.FormatearFecha(item.FECHA_DESDE),
                    FechaHasta = item.FECHA_HASTA,
                    Precio = item.PRECIO,
                    Moneda = item.MONEDA,
                    Motivo = item.MOTIVO,
                    DetalleMotivo = item.DET_MOTIVO,
                    CondicionEntrega = item.CONDICION_ENTREGA,
                    Producto = item.PRODUCTO,
                    PuntoExpedicion = item.PTO_EXPEDICION,
                    TipoContrato = ConvertirDeTipoContratoFasSAP(item.TIPO_CONTRATO),
                    PrecioFlete = item.PRECIO_FLETE,
                    BloqueoEntrega = item.BLOQUEO_ENTREGA == "X"
                };
                var detalles = new List<SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Detail>();
                foreach (var detalle in item.DETALLE)
                {
                    detalles.Add(new SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Detail()
                    {
                        Pedido = detalle.PEDIDO,
                        Entrega = detalle.ENTREGA,
                        FechaPedido = detalle.FECHA_PEDIDO,
                        FechaCarga = detalle.FECHA_CARGA,
                        CantidadEntregada = detalle.CANTIDAD_ENTREGADA,
                        Remito = detalle.REMITO,
                        Factura = detalle.FACTURA,
                        CantidadFactura = detalle.CANTIDAD_FACTURA,
                        FacturaLegal = detalle.FACTURA_LEGAL,
                        Chasis = detalle.CHASIS,
                        Acoplado = detalle.ACOPLADO,
                        Chofer = detalle.CHOFER,
                        Destinatario = detalle.DESTINATARIO,
                        NombreDestinatario = detalle.NOMBRE_DESTINATARIO,
                        KilosEntrega = detalle.KILOS_ENTREGA,
                        BloqueoEntrega = detalle.BLOQUEO_ENTREGA == "X"
                    });
                }
                if (detalles != null)
                {
                    if (detalles.Count > 0)
                    {
                        resultado.Detalles = detalles;
                    }
                }
                resultados.Add(resultado);
            }

            if (resultados != null)
            {
                if (resultados.Count > 0)
                {

                    response.Resultados = resultados;
                }
            }
            return response;
        }

        public ControlEstadoResEnum GetOrdenCargaControlEstadoTransportista(string cuitTransportista)
        {
            var resp = OrdenCargaControlEstadoRequest("", "", cuitTransportista);
            return ResponseConverter.GetOrdenCargaControlEstadoResponse(resp);
        }

        public ModOrdenCargaResponseHandler AnularOrdenCarga(OrdenDeCarga orden)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var identificador = !string.IsNullOrEmpty(orden.NumeroPedidoIngresado) ? orden.NumeroPedidoIngresado : orden.NumeroPedido;

                var request = new Z_MPMF_MOAOP_MOD_ORDEN_CARGA()
                {
                    IM_BORRAR = "X",
                    IM_ORDEN_CARGA = identificador
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_MOD_ORDEN_CARGA request");
                Log.Info(request.ToXml());

                var response = agent.Z_MPMF_MOAOP_MOD_ORDEN_CARGA(request);

                Log.Info($"SAP sin PI Z_MPMF_MOAOP_MOD_ORDEN_CARGA response");
                Log.Info(response.ToXml());

                return new ModOrdenCargaResponseHandler(response.EX_MENSAJE);
            }
            else
            {
                var service = new SI_MPMF_MOAOP_MOD_ORDEN_CARGAClient();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                var identificador = !string.IsNullOrEmpty(orden.NumeroPedidoIngresado) ? orden.NumeroPedidoIngresado : orden.NumeroPedido;

                Log.Info($"SI_MPMF_MOAOP_MOD_ORDEN_CARGA Request: {new { identificador }}");

                var result = service.SI_MPMF_MOAOP_MOD_ORDEN_CARGA("X", identificador);

                Log.Info($"SI_MPMF_MOAOP_MOD_ORDEN_CARGA Result: {new { result, identificador }}");

                return new ModOrdenCargaResponseHandler(result);
            }
        }

        public ModEntregaResponseHandler AnularEntregaOrdenCarga(string nroEntrega)
        {
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var request = new Z_MPMF_MOAOP_MOD_ENTREGA()
                {
                    IM_ACOPLADO = "",
                    IM_BORRAR = "X",
                    IM_CHASIS = "",
                    IM_CHOFER = "",
                    IM_DOCUMENTO = "",
                    IM_ENTREGA = nroEntrega,
                    IM_TIPODOC = "",
                    IM_TRANSPORTE = "",
                    IM_TRANSPORTISTA_REAL = ""
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_MOD_ENTREGA request");
                Log.Info(request.ToXml());
                var response = agent.Z_MPMF_MOAOP_MOD_ENTREGA(request);
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_MOD_ENTREGA response");
                Log.Info(response.ToXml());
                return new ModEntregaResponseHandler(response.EX_MENSAJE);
            }
            else
            {
                var service = new SI_MPMF_MOAOP_MOD_ENTREGAClient();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                Log.Info($"SI_MPMF_MOAOP_MOD_ENTREGA Request: {nroEntrega}");

                var result = service.SI_MPMF_MOAOP_MOD_ENTREGA("", "X", "", "", "", nroEntrega, "", "", "");

                Log.Info($"SI_MPMF_MOAOP_MOD_ENTREGA Result: {new { result, nroEntrega }}");

                return new ModEntregaResponseHandler(result);
            }
        }

        public ResultadoGenerico ModificarEntregaOrdenCarga(ModificarEntregaRequest req)
        {
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {

                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var cuitTransporte =
                    !string.IsNullOrEmpty(req.CUITIntermediarioFlete) ?
                        req.CUITIntermediarioFlete :
                        req.CUITTransporte;

                var codigoSapTransporte = DataFormatter.CuitACodigoSap(cuitTransporte);

                var cuitTransportistaReal =
                    !string.IsNullOrEmpty(req.CUITIntermediarioFlete) ?
                        req.CUITTransporte :
                        req.CUITIntermediarioFlete;

                var request = new Z_MPMF_MOAOP_MOD_ENTREGA()
                {
                    IM_ACOPLADO = req.Acoplado,
                    IM_BORRAR = "",
                    IM_CHASIS = req.Chasis,
                    IM_CHOFER = req.Chofer,
                    IM_DOCUMENTO = req.Documento,
                    IM_ENTREGA = req.NumeroEntrega,
                    IM_TIPODOC = req.TipoDoc,
                    IM_TRANSPORTE = codigoSapTransporte,
                    IM_TRANSPORTISTA_REAL = cuitTransportistaReal
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_MOD_ENTREGA request");
                Log.Info(request.ToXml());

                var response = agent.Z_MPMF_MOAOP_MOD_ENTREGA(request);
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_MOD_ENTREGA response");
                Log.Info(response.ToXml());

                var resultado = new ResultadoGenerico();
                if (response.EX_MENSAJE != "Se actualizaron los datos correctamente")
                    resultado.Error("error", response.EX_MENSAJE);

                return resultado;
            }
            else
            {
                var service = new SI_MPMF_MOAOP_MOD_ENTREGAClient();
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                var cuitTransporte =
                    !string.IsNullOrEmpty(req.CUITIntermediarioFlete) ?
                        req.CUITIntermediarioFlete :
                        req.CUITTransporte;

                var codigoSapTransporte = DataFormatter.CuitACodigoSap(cuitTransporte);

                var cuitTransportistaReal =
                    !string.IsNullOrEmpty(req.CUITIntermediarioFlete) ?
                        req.CUITTransporte :
                        req.CUITIntermediarioFlete;

                Log.Info($"SI_MPMF_MOAOP_MOD_ENTREGA " +
                    $"Request: {req.ToJson()}, " +
                    $"codigoSapTransporte: {codigoSapTransporte}, " +
                    $"cuitTransportistaReal: {cuitTransportistaReal}.");

                var result = service.SI_MPMF_MOAOP_MOD_ENTREGA(
                                IM_ACOPLADO: req.Acoplado,
                                IM_BORRAR: "",
                                IM_CHASIS: req.Chasis,
                                IM_CHOFER: req.Chofer,
                                IM_DOCUMENTO: req.Documento,
                                IM_ENTREGA: req.NumeroEntrega,
                                IM_TIPODOC: req.TipoDoc,
                                IM_TRANSPORTE: codigoSapTransporte,
                                IM_TRANSPORTISTA_REAL: cuitTransportistaReal);
                Log.Info($"SI_MPMF_MOAOP_MOD_ENTREGA Result: {new { result, nroEntrega = req.NumeroEntrega }}");

                var resultado = new ResultadoGenerico();
                if (result != "Se actualizaron los datos correctamente")
                    resultado.Error("error", result);

                return resultado;
            }
        }

        public bool VerificarContratoAbierto(string contrato)
        {
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var agent = new Z_WS_MOAOP_DIRECTClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var fechas = ObtenerRangoFechasSinPI();

                var request = new Z_MPMF_MOAOP_VISUALIZAR_ZFAS()
                {
                    IM_CLIENTE = "",
                    IM_CONTRATO = contrato,
                    IM_CORREDOR = "",
                    IM_FECHA = fechas,
                    IM_MATERIAL = "",
                    IM_PENDIENTE = "X",
                    IM_TIPO_CONTRATO = TipoContratoFas_Todos
                };
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_VISUALIZAR_ZFAS request");
                Log.Info(request.ToXml());
                var response = agent.Z_MPMF_MOAOP_VISUALIZAR_ZFAS(request);
                Log.Info($"SAP sin PI Z_MPMF_MOAOP_VISUALIZAR_ZFAS response");
                Log.Info(response.ToXml());
                return response.EX_SALIDA.Length > 0;

            }
            else
            {
                var service = new SI_MPMF_MOAOP_VISUALIZAR_ZFASClient();

                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                var fechas = ObtenerRangoFechas();

                var result = service.SI_MPMF_MOAOP_VISUALIZAR_ZFAS(
                    IM_CLIENTE: "",
                    IM_CONTRATO: contrato,
                    IM_CORREDOR: "",
                    IM_FECHA: fechas,
                    IM_MATERIAL: "",
                    IM_PENDIENTE: "X",
                    IM_TIPO_CONTRATO: TipoContratoFas_Todos);

                return result.Length > 0;
            }
        }

        public SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Result ObtenerContratoSAP(OrdenDeCarga orden, TipoContratoFAS? tipoContrato)
        {
            return ObtenerContratoSAP(string.IsNullOrEmpty(orden.ContratoSAP) ? orden.ContratoIngresado : orden.ContratoSAP, tipoContrato);
        }

        public SustitucionMOAModel.Models.WSMapMOA.OrdenCarga.Result ObtenerContratoSAP(string numeroContrato, TipoContratoFAS? tipoContrato)
        {
            var request = new OrdenCargaVisualizarClienteWSMOARequest
            {
                Contrato = numeroContrato,
                TipoContrato = tipoContrato ?? TipoContratoFAS.Todos,
                Fechas = ObtenerFechas()
            };

            var contratoSAP = OrdenCargaVisualizarClienteExecute(request).Resultados.FirstOrDefault();
            return contratoSAP;
        }

        private List<FechaWS> ObtenerFechas()
        {
            return new List<FechaWS>
                {
                    new FechaWS
                    {
                        fechaFin = DateTime.Now,
                        fechaInicio = DateTime.Parse(Constante.FECHA_BASICA)
                    }
                };
        }

        private OrdenCargaVisualizarCliente.ZMPES4100[] ObtenerRangoFechas()
        {
            var hasta = DateTime.Now;
            var desde = hasta.AddMonths(-Constante.MESES_ATRAS_FAS);
            return new List<OrdenCargaVisualizarCliente.ZMPES4100>{ new OrdenCargaVisualizarCliente.ZMPES4100()
            {
                FECHA_OP = SAPFormatter.PrepararFecha(desde),
                FECHA_OP_HASTA = SAPFormatter.PrepararFecha(hasta)
            }}.ToArray();
        }
        private WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100[] ObtenerRangoFechasSinPI()
        {
            var hasta = DateTime.Now;
            var desde = hasta.AddMonths(-Constante.MESES_ATRAS_FAS);
            return new List<WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100>{ new WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES4100()
            {
                FECHA_OP = SAPFormatter.PrepararFecha(desde),
                FECHA_OP_HASTA = SAPFormatter.PrepararFecha(hasta)
            }}.ToArray();
        }


        private CrearEntregaRequest LimpiarRequestSinPedidoAnticipado(CrearEntregaRequest entregaReq)
        {
            entregaReq.Reventa = false;
            entregaReq.CuitDestino = "";
            entregaReq.CuitDestinatario = "";
            entregaReq.RazonSocialDestinatario = "";
            entregaReq.RazonSocialDestino = "";
            entregaReq.DomicilioDescr = "";
            entregaReq.DomicilioOrden = null;
            entregaReq.DomicilioTipo = "";
            entregaReq.PlantaCodigo = "";

            return entregaReq;
        }

        private string ConvertirATipoContratoFasSAP(TipoContratoFAS? tipoContrato)
        {
            if (!tipoContrato.HasValue)
            {
                return "";
            }
            switch (tipoContrato)
            {
                case TipoContratoFAS.Normal: return TipoContratoFas_Normal;
                case TipoContratoFAS.Anticipado: return TipoContratoFas_Anticipado;
                case TipoContratoFAS.Todos: return TipoContratoFas_Todos;
                default: throw new Exception("Tipo de contrato no mapeado");
            }
        }

        private TipoContratoFAS ConvertirDeTipoContratoFasSAP(string tipoContrato)
        {
            switch (tipoContrato)
            {
                case TipoContratoFas_Normal: return TipoContratoFAS.Normal;
                case TipoContratoFas_Anticipado: return TipoContratoFAS.Anticipado;
                case TipoContratoFas_Todos: return TipoContratoFAS.Todos;
                default: throw new Exception("No se reconoce tipo de contrato " + tipoContrato);
            }
        }
    }
}
