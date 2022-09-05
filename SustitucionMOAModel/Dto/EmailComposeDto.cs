using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EmailComposeDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public string DownloadLinkUrl { get; set; }
    }
}