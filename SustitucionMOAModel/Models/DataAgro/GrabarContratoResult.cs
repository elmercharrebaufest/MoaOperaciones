using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SustitucionMOAModel.Models.DataAgro
{
    [DataContract]
    [KnownType("TiposDeResultados")]
    public class GrabarContratoResult
    {
        [DataMember]
        public int? ContratoId { get; set; }

        public List<ErrorMessage> Errores { get; set; } = new List<ErrorMessage>();

        [DataMember]
        public List<ErrorMessage> ListaErrores { get { return Errores; } }

        [DataMember]
        public bool HayError
        {
            get { return Errores.Count != 0; }
        }

        public bool HayErrores
        {
            get { return Errores.Count != 0; }
        }

        public void Error(string clave, string descripcion)
        {
            Errores.Add(new ErrorMessage(descripcion, clave));
        }


        /// <summary>
        /// Este metodo es para que cuando se exponga la clase Resultado por WCF, se expongan tambien todas las subclases
        /// </summary>
        /// <returns>La lista de subclases de Resultado que hay en el assembly</returns>
        public static Type[] TiposDeResultados()
        {
            var tipoResultado = typeof(GrabarContratoResult);
            return tipoResultado.Assembly.GetTypes().Where(tipoResultado.IsAssignableFrom).ToArray();
        }
    }

    [DataContract]
    public class ErrorMessage
    {
        private int mintItem;
        private int mintErrorCode;
        private int mintLogId;
        private string mstrMessage;
        private string mstrSource;
        private string mstrLargeDescription;
        private bool mblnTranslate;
        private string mstrFormat;
        private List<object> marrArgs;

        [DataMember]
        public int Item
        {
            get
            {
                return this.mintItem;
            }
            set
            {
                this.mintItem = value;
            }
        }

        [DataMember]
        public int ErrorCode
        {
            get
            {
                return this.mintErrorCode;
            }
            set
            {
                this.mintErrorCode = value;
            }
        }

        [DataMember]
        public int LogId
        {
            get
            {
                return this.mintLogId;
            }
            set
            {
                this.mintLogId = value;
            }
        }

        [DataMember]
        public string Message
        {
            get
            {
                return this.mstrMessage;
            }
            set
            {
                this.mstrMessage = value;
            }
        }



        [DataMember]
        public string Source
        {
            get
            {
                return this.mstrSource;
            }
            set
            {
                this.mstrSource = value;
            }
        }

        [DataMember]
        public string LargeDescription
        {
            get
            {
                return this.mstrLargeDescription;
            }
            set
            {
                this.mstrLargeDescription = value;
            }
        }

        [DataMember]
        public bool Translate
        {
            get
            {
                return this.mblnTranslate;
            }
            set
            {
                this.mblnTranslate = value;
            }
        }

        [DataMember]
        public string Format
        {
            get
            {
                return this.mstrFormat;
            }
            set
            {
                this.mstrFormat = value;
            }
        }

        public List<object> Args
        {
            get
            {
                return this.marrArgs;
            }
            set
            {
                this.marrArgs = value;
            }
        }

        public ErrorMessage()
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = "";
            this.mstrSource = "";
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(string strMessage)
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(string strMessage, string strSource)
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = strMessage;
            this.mstrSource = strSource;
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(string strFormat, string strSource, params object[] args)
        {
            this.mintItem = 0;
            this.mintErrorCode = 0;
            this.mintLogId = 0;
            this.mstrMessage = "";
            this.mstrSource = strSource;
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = strFormat;
            this.marrArgs = args.ToList<object>();
        }

        public ErrorMessage(int intErrorCode, string strMessage)
        {
            this.mintItem = 0;
            this.mintLogId = 0;
            this.mintErrorCode = intErrorCode;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = "";
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(int intItem, int intErrorCode, string strMessage, string strLargeDescription)
        {
            this.mintItem = intItem;
            this.mintErrorCode = intErrorCode;
            this.mintLogId = 0;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = strLargeDescription;
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(int intItem, int intErrorCode, int intLogId, string strMessage, string strLargeDescription)
        {
            this.mintItem = intItem;
            this.mintErrorCode = intErrorCode;
            this.mintLogId = intLogId;
            this.mstrMessage = strMessage;
            this.mstrSource = "";
            this.mstrLargeDescription = strLargeDescription;
            this.mblnTranslate = false;
            this.mstrFormat = "";
            this.marrArgs = new List<object>();
        }

        public ErrorMessage(int intNroLinea, string strCampo, string strSource, string strMessage)
          : this(strMessage, strSource)
        {
            this.mstrSource = string.Format("#root|{0}|{1}|{2}", strSource, intNroLinea.ToString(), strCampo);
        }


    }


    [DataContract]
    public class GrabarContratoResultDto
    {
        [DataMember]
        public int? ContratoId { get; set; }
        [DataMember]
        public List<string> ListaCupos { get; set; } = new List<string>();

        [DataMember]
        public List<ErrorMessageDtos> Errores { get; set; } = new List<ErrorMessageDtos>();


        [DataMember]
        public bool HayError
        {
            get { return Errores.Count != 0; }
        }

        public int? FijacionDePrecioContratoId { get; set; }
    }

    [DataContract]
    public class ErrorMessageDtos
    {
        [DataMember]

        public string Message { get; set; }
        [DataMember]
        public string Source { get; set; }

    }
}
