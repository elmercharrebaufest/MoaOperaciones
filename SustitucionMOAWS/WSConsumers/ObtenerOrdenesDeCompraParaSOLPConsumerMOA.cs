using System.Collections.Generic;
using System.Linq;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.OrdenesDeCompraParaSolpWebServiceMOA;


namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenesDeCompraParaSOLPConsumerMOA : IObtenerOrdenesDeCompraParaSOLPConsumerMOA
    {
        Z_MMRFC_OC_PARA_SOLPPortTypeClient service;

        public ObtenerOrdenesDeCompraParaSOLPConsumerMOA()
        {

        }

        public List<OrdenDeCompraSAPDto> Request(string SOLPED, string POSICION)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=Z_MMRFC_OC_PARA_SOLP&amp;interfaceNamespace=urn%3Asap-com%3Adocument%3Asap%3Arfc%3Afunctions";
            service = new Z_MMRFC_OC_PARA_SOLPPortTypeClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            ZMMES0027[] lista = service.Z_MMRFC_OC_PARA_SOLP(POSICION, SOLPED, out string exito, out string resultado);
            return Map(lista, exito, resultado);
        }

        private List<OrdenDeCompraSAPDto> Map(ZMMES0027[] items, string exito, string resultado)
        {
            List<OrdenDeCompraSAPDto> result = new List<OrdenDeCompraSAPDto>();
            if (exito != "200")
            {
                if (resultado == "No hay Órdenes de Compra para la Solicitud de Pedido")
                {
                    return result;
                } 
                else throw new ValidationCustomException(resultado);
            }

            var agrupado = items.GroupBy(a => a.EBELN).ToList();
            foreach (var item in agrupado)
            {
                result.Add(new OrdenDeCompraSAPDto
                {
                    Cabecera = new OrdenDeCompraSAPCabecera
                    {
                        OrdenDeCompra = item.Key,
                        CodigoProveedor = item.First().LIFNR,
                        RazonSocialProveedor = item.First().NAME1,
                        CUITProveedor = item.First().STCD1,
                        Moneda = item.First().WAERS,
                        MontoTotal = item.Sum(a => a.NETWR),
                        CreadoPor = item.First().ERNAM,
                        ClaseDocumento = item.First().BSART,
                        FechaCreacion = SAPFormatter.GetDateTime(item.First().AEDAT),
                        Tipo = item.First().PSTYP == "0" ? "Materiales" : "Servicios",
                        TipoDocCompras = item.First().BSTYP,
                        MontoBruto = item.Sum(a => a.BRTWR),
                        EstadoLiberacionCodigo = item.First().FRGKE,
                    }
                });
            }
            return result;
        }
    }

    public interface IObtenerOrdenesDeCompraParaSOLPConsumerMOA
    {
        List<OrdenDeCompraSAPDto> Request(string SOLPED, string POSICION);
    }
}