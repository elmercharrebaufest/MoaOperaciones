using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ObtenerOrdenesDeCompraWebServiceMOA;
using SustitucionMOAWS.OrdenesDeCompraParaSolpWebServiceMOA;


namespace SustitucionMOAWS.WSConsumers
{
    /// <summary>
    /// Listado de Ordenes de Compra
    /// </summary>
    public class ObtenerOrdenesDeCompraConsumerMOA : IObtenerOrdenesDeCompraConsumerMOA
    {
        BAPI_PO_GETITEMSPortTypeClient service;

        public ObtenerOrdenesDeCompraConsumerMOA()
        {
            service = new BAPI_PO_GETITEMSPortTypeClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public List<OrdenCompraDto> Request(OrderParamsDto parametros)
        {
            string fechaInicio = parametros.fechaInicio;
            string vendedor = parametros.vendedor;
            string categoria = "9"; // 9 = Servicios
            BAPIEKKOL[] cabecera = new BAPIEKKOL[] { };
            BAPIEKPOC[] detalle = new BAPIEKPOC[] { };
            BAPIRETURN[] bapiReturn = new BAPIRETURN[] { };
            service.BAPI_PO_GETITEMS("", "", "", fechaInicio, "", "", categoria, "", new BAPIMGVMATNR(), "",
                                     "", "", "", "", "", "", "", new BAPIMGVMATNR(), "", "",
                                     "", "", vendedor, "X",
                                     ref cabecera,
                                     ref detalle,
                                     ref bapiReturn
                                    );
            return Map(cabecera, detalle);
            //turn new List<OrdenDeCompraSAPCabecera>();
        }

        private List<OrdenCompraDto> Map(BAPIEKKOL[] cabecera, BAPIEKPOC[] detalle)
        {
            List<OrdenCompraDto> result = new List<OrdenCompraDto>();
            List<OrdenCompraPosicionDto> posiciones = new List<OrdenCompraPosicionDto>();

            foreach (var item in detalle)
            {
                var aux = new OrdenCompraPosicionDto()
                {
                    OrdenCompraId = long.Parse(item.PO_NUMBER),
                    PosicionId = int.Parse(item.PO_ITEM),
                    Material = item.MATERIAL,
                    Descripcion = item.SHORT_TEXT,
                    PrecioNeto = item.NET_PRICE,
                    Solicitante = item.PREQ_NAME,
                    Solped = long.TryParse(item.ADDRESS, out long parsedSolped) ? parsedSolped : 0
                };
                posiciones.Add(aux);
            }

            foreach (var item in cabecera)
            {
                //Suma de los netos de las posiciones
                decimal montoTotal = detalle
                    .Where(det => det.PO_NUMBER == item.PO_NUMBER)
                    .Sum(det => det.NET_PRICE);

                // Parsear la fecha y formatearla
                DateTime fecha = DateTime.ParseExact(item.DOC_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string fechaFormateada = fecha.ToString("dd-MM-yyyy");

                result.Add(new OrdenCompraDto
                {
                    OrdenCompraId = long.Parse(item.PO_NUMBER),
                    ProveedorNombre = item.VEND_NAME,
                    //Fecha = SAPFormatter.GetDateTime(item.DOC_DATE),
                    Fecha = fechaFormateada,
                    Descripcion = "Falta Determinar Descripcion de Orden de Compra",
                    MontoTotal = montoTotal, 
                    //Posiciones = new List<OrdenCompraPosicionDto>()
                    Posiciones = posiciones.Where(x => x.OrdenCompraId == long.Parse(item.PO_NUMBER)).ToList() // Si no tiene nro. Solped fallaba
                });
            }
            
            return result;
        }

    }
}
