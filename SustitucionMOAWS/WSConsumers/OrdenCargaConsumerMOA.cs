using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.OrdenCargaControlWebServiceMOA;
using SustitucionMOAWS.OrdenCargaCrearWebServiceMOA;
using SustitucionMOAWS.OrdenCargaEntregadaWebServiceMOA;
using SustitucionMOAWS.OrdenCargarControlEstadoWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class OrdenCargaConsumerMOA
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


        public string ControlCargaRequest(string cliente, string contrato, string corredor, string cuit, string material, string pedido)
        {
            var service = new SI_MPMF_MOAOP_CONTROL_CARGAClient();

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            return service.SI_MPMF_MOAOP_CONTROL_CARGA(cliente, contrato, corredor, cuit, material, pedido);
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

        public string CrearOrdenRequest(string cliente, string contrato, string corredor, decimal kilos, string material, string pedidoInput, out string pedidoOutput)
        {
            var service = new SI_MPMF_MOAOP_CREAR_ORDEN_CARGAClient();

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            return service.SI_MPMF_MOAOP_CREAR_ORDEN_CARGA(cliente, contrato, corredor, kilos, material, pedidoInput, out pedidoOutput);
        }



        /*
            * RFC Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE 
        http://gslopidevqa00.molinosagro.ad:50000/dir/wsdl?p=ic/369af869c9a8315aa009c44333c52001
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
        Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE 	OE-00	'OK'
        Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE 	OE-01	'No existe tranportista'
        Z_MPMF_MOAOP_ORDEN_CARGA_ENTRE 	OE-02	'Entrega Creada - Error al insertar'

        */

        public string OrdenCargaEntregadaRequest(string documento, decimal kilos, string nombreConductor,
                                                 string patenteAcoplado, string patenteChasis, string pedido,
                                                 string tipoDocumento, string transportista, out string mensaje)
        {
            var service = new SI_MPMF_MOAOP_ORDEN_CARGA_ENTREClient();

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            return service.SI_MPMF_MOAOP_ORDEN_CARGA_ENTRE(documento, kilos, nombreConductor, patenteAcoplado, patenteChasis, pedido, tipoDocumento, transportista, out mensaje);
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
            var service = new SI_MPMF_MOAOP_CONTROL_ESTADOClient();

            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            return service.SI_MPMF_MOAOP_CONTROL_ESTADO(entrega, pedido, transportista);
        }
    }
}
