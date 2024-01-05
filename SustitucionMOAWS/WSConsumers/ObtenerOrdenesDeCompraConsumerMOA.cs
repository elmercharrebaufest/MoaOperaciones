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
            return Map(cabecera);
            //turn new List<OrdenDeCompraSAPCabecera>();
        }

        private List<OrdenCompraDto> Map(BAPIEKKOL[] cabecera)
        {
            List<OrdenCompraDto> result = new List<OrdenCompraDto>();
            List<OrdenCompraDto> posiciones = new List<OrdenCompraDto>();

            foreach (var item in cabecera)
            {
                //Suma de los netos de las posiciones
                //decimal montoTotal = detalle
                //    .Where(det => det.PO_NUMBER == item.PO_NUMBER)
                //    .Sum(det => det.NET_PRICE);

                // Parsear la fecha y formatearla
                DateTime fecha = DateTime.ParseExact(item.DOC_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string fechaFormateada = fecha.ToString("dd-MM-yyyy");

                result.Add(new OrdenCompraDto
                {
                    Id = long.Parse(item.PO_NUMBER),
                    ProveedorNombre = item.VEND_NAME,
                    MonedaDescripcion = item.CURRENCY_ISO
                    
                });
            }
            
            return result;
        }

    }
}
