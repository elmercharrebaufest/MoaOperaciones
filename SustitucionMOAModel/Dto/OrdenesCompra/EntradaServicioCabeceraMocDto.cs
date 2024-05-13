using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    [Serializable, XmlRoot("item")]
    public class EntradaServicioCabeceraConsumerDto
    {
        public string SHEET_NO { get; set; }
        public string EXT_NUMBER { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_ON { get; set; }
        public string CH_ON { get; set; }
        public string CHANGED_BY { get; set; }
        public string PERSON_INT { get; set; }
        public string PERSON_EXT { get; set; }
        public string LOCATION { get; set; }
        public string REF_DATE { get; set; }
        public string BEGDATE { get; set; }
        public string ENDDATE { get; set; }
        public string GROSS_VAL { get; set; }
        public string UNPL_VAL { get; set; }
        public string UNPLC_VAL { get; set; }
        public string CURRENCY { get; set; }
        public string CURR_ISOCD { get; set; }
        public string PCKG_NO { get; set; }
        public string SHORT_TEXT { get; set; }
        public string PO_NUMBER { get; set; }
        public string PO_ITEM { get; set; }
        public string DELETE_IND { get; set; }
        public string ACCEPTANCE { get; set; }
        public string FIN_ENTRY { get; set; }
        public string REL_GROUP { get; set; }
        public string REL_STRAT { get; set; }
        public string REL_IND { get; set; }
        public string REL_STATUS { get; set; }
        public string SUBJ_TO_R { get; set; }
        public string BLOCK_IND { get; set; }
        public string SCORE_TIME { get; set; }
        public string SCORE_QUAL { get; set; }
        public string DOC_DATE { get; set; }
        public string POST_DATE { get; set; }
        public string REF_DOC_NO { get; set; }
        public string ACCASSCAT { get; set; }
        public string NET_VALUE { get; set; }
        public string PREQ_NO { get; set; }
        public string PREQ_ITEM { get; set; }
        public string MAINTPLAN { get; set; }
        public string MAINTITEM { get; set; }
        public string CALL_NO { get; set; }
        public string FRCO_DOC { get; set; }
        public string FRCO_ITEM { get; set; }
        public string COMM_NO { get; set; }
        public string USER_FIELD { get; set; }
        public string REF_DOC_NO_LONG { get; set; }
        public string EXT_NUMBER_LONG { get; set; }
    };

}
