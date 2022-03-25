using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Enums
{
    public enum EstadoComprobantesNG
    {    

        //En gestion
        Creado = 00, //00 – Creado
        Indexado = 02, //02 – Indexado
        EnviadoCreaciónDocumentos = 03, //03 – Enviado para la creación de documentos
        PosibleDuplicado = 07, //07 – Posible duplicado 

        //En aprobacion
        EsperandoAutorizacion = 11, //11 – Esperando autorización (autorización de la COA)
        AutorizaciónFinalizada = 12, //12 – Autorización finalizada (autorización de la COA)
        RechazadoPorAutorizador = 13, //13 – Rechazado por autorizador (autorización de la COA)
        AprobaciónRetirada = 29, //29 – Aprobación retirada
           
        //Rechazados
        Obsoletos = 10, //10 – Obsoleto con el campo DELREASON igual a 01 – Devolución de factura al proveedor
            
        //Mensajes genericos
        ListoValidacion = 74, //74 – Listo para la validación (nunca se ve en el WP): la web lo agrupará en un mensaje genérico  //(Esto es lo que se ve en el modal)

        //No se vizualizara
        RechazadoValidacion = 90, //90 – Rechazado en la validación (nunca se ve en el WP): implica que el proveedor entró un documento que corresponde a Factura, ND o NC (por ejemplo: entrega remito, orden de compra, etc.).
        DuplicadoConfirmado = 08, //08 – Duplicado confirmado.

        //16 – Borrado (luego de la contabilización): implica que ya se contabilizó y se anuló sin pagar (queda pendiente confirmar si la web lo mostrará en función de la prueba que se realizará en QA  
        //17 – Cancelado (luego de la contabilización): implica que ya se contabilizó y se cancela sin pagar (queda pendiente confirmar si la web lo mostrará en función de la prueba que se realizará en QA
    }

    public static class EstadoComprobantesNGExtensions
    {
        public static string ToFriendlyString(this EstadoComprobantesNG me)
        {
            switch (me)
            {
                case EstadoComprobantesNG.Creado:
                case EstadoComprobantesNG.Indexado:
                case EstadoComprobantesNG.EnviadoCreaciónDocumentos:
                case EstadoComprobantesNG.PosibleDuplicado:
                    return "En gestión";
                case EstadoComprobantesNG.EsperandoAutorizacion:
                case EstadoComprobantesNG.AutorizaciónFinalizada:
                case EstadoComprobantesNG.RechazadoPorAutorizador:
                case EstadoComprobantesNG.AprobaciónRetirada:
                    return "En aprobación";
                case EstadoComprobantesNG.Obsoletos:
                    return "Rechazados";
                case EstadoComprobantesNG.ListoValidacion:
                    return "mensajeGenerico";
                default:
                    return "noSeVisualizaran";
            }
        }

        public static string ObtenerColorEstado(this EstadoComprobantesNG me)
        {
            switch (me)
            {
                case EstadoComprobantesNG.Creado:
                case EstadoComprobantesNG.Indexado:
                case EstadoComprobantesNG.EnviadoCreaciónDocumentos:
                case EstadoComprobantesNG.PosibleDuplicado:
                    return "orange";
                case EstadoComprobantesNG.EsperandoAutorizacion:
                case EstadoComprobantesNG.AutorizaciónFinalizada:
                case EstadoComprobantesNG.RechazadoPorAutorizador:
                case EstadoComprobantesNG.AprobaciónRetirada:
                    return "green";
                case EstadoComprobantesNG.Obsoletos:
                    return "red";
                default:
                    return "white";
            }
        }
    }
}

//Se considerará como En gestión los que tengan el campo STATUS igual a:s

//00 – Creado
//02 – Indexado
//03 – Enviado para la creación de documentos
//07 – Posible duplicado 


//Se considerará como En aprobación los que tengan el campo STATUS igual a:

//11 – Esperando autorización (autorización de la COA)
//12 – Autorización finalizada (autorización de la COA)
//13 – Rechazado por autorizador (autorización de la COA)
//29 – Aprobación retirada


//Se considerará como Rechazados los que tengan el campo STATUS igual a:

//10 – Obsoleto con el campo DELREASON igual a 01 – Devolución de factura al proveedor


//Se visualizarán con mensaje genérico (modal) los que tengan el campo STATUS igual a:

//74 – Listo para la validación (nunca se ve en el WP): la web lo agrupará en un mensaje genérico  //(Esto es lo que se ve en el modal)


//NO se visualizarán (no aparecen en la tabla) en la web los que tengan el campo STATUS igual a:

//90 – Rechazado en la validación (nunca se ve en el WP): implica que el proveedor entró un documento que corresponde a Factura, ND o NC (por ejemplo: entrega remito, orden de compra, etc.).
//08 – Duplicado confirmado.

//Quedan pendientes los que tengan el campo STATUS igual a:

//16 – Borrado (luego de la contabilización): implica que ya se contabilizó y se anuló sin pagar (queda pendiente confirmar si la web lo mostrará en función de la prueba que se realizará en QA  
//17 – Cancelado (luego de la contabilización): implica que ya se contabilizó y se cancela sin pagar (queda pendiente confirmar si la web lo mostrará en función de la prueba que se realizará en QA