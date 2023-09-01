using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class ObtenerProveedorWSMOAResponse
    {
        public string VENDOR { get; set; }//VENDOR LIFNR   Número de Proveedor(LFA1)
        public string COMPCODE { get; set; }//COMP_CODE BUKRS   Sociedad(LFB1)
        public string PURCHORG { get; set; }//PURCH_ORG EKORG   Organización de Compras(LFM1)
        public string SORT1 { get; set; }//SORT1 SORTL   Concepto de busqueda(ADRC)
        public string SORT2 { get; set; }//SORT2 SORTL   Concepto de busqueda 2 (ADRC)
        public string KTOKK { get; set; }//KTOKK   KTOKK Grupo de Cuentas(LFA1)
        public string CURRENCY { get; set; }//CURRENCY WAERS   Moneda del Pedido(LFM1)
        public string PMNTTRMS { get; set; }//PMNTTRMS ZTERM   Condición Pago por defecto(LFM1)
        public string VERIFFEM { get; set; }//VERIF_F_EM WEBRE   Verific.fact.base EM(LFM1)
        public string LAND1 { get; set; }//LAND1 LAND1   Pais(LFA1)
        public string NAME { get; set; }//NAME NAME1   Nombre del Proveedor(LFA1)
        public string COUNTRY { get; set; }//COUNTRY ORT01   Población(LFA1)
        public string STREET { get; set; }//STREET STRAS   Calle(LFA1)
        public string TELEFONO { get; set; }//TELEFONO TELF1   Telefono(LFA1)
        public string MAIL { get; set; }//MAIL ADRNR   E-mail(ADR6)


    }
}
