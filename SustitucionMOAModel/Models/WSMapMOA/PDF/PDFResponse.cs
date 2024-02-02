namespace SustitucionMOAModel.Models.WSMapMOA.PDF
{
    public class PDFResponse
    {
        public Pdf Pdf { get; set; }

        public ErrorWS Error { get; set; }

        public PDFResponse()
        {
            Error = new ErrorWS();
        }
    }
}