using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class EmailComposeDto
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public string DownloadLinkUrl { get; set; }
    }
}