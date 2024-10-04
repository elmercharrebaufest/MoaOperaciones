using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Helpers
{
    //
    // Summary:
    //     Specifies the media type information for an email message attachment.
    // copiado (y ampliado) de System.Net.Mime.MediaTypeNames
    public static class CustomMediaTypeNames
    {
        public static class Text
        {
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in plain text
            //     format.
            public const string Plain = MediaTypeNames.Text.Plain;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in HTML format.
            public const string Html = MediaTypeNames.Text.Html;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in XML format.
            public const string Xml = MediaTypeNames.Text.Xml;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Text data is in Rich Text Format
            //     (RTF).
            public const string RichText = MediaTypeNames.Text.RichText;
        }

        //
        // Summary:
        //     Specifies the kind of application data in an email message attachment.
        public static class Application
        {
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is a SOAP
            //     document.
            public const string Soap = MediaTypeNames.Application.Soap;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is not interpreted.
            public const string Octet = MediaTypeNames.Application.Octet;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is in Rich
            //     Text Format (RTF).
            public const string Rtf = MediaTypeNames.Application.Rtf;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is in Portable
            //     Document Format (PDF).
            public const string Pdf = MediaTypeNames.Application.Pdf;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Application data is compressed.
            public const string Zip = MediaTypeNames.Application.Zip;

            public const string doc = "application/msword";
            public const string dot = "application/msword";
            public const string docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            public const string dotx = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
            public const string docm = "application/vnd.ms-word.document.macroEnabled.12";
            public const string dotm = "application/vnd.ms-word.template.macroEnabled.12";
            public const string xls = "application/vnd.ms-excel";
            public const string xlt = "application/vnd.ms-excel";
            public const string xla = "application/vnd.ms-excel";
            public const string xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string xltx = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
            public const string xlsm = "application/vnd.ms-excel.sheet.macroEnabled.12";
            public const string xltm = "application/vnd.ms-excel.template.macroEnabled.12";
            public const string xlam = "application/vnd.ms-excel.addin.macroEnabled.12";
            public const string xlsb = "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
            public const string ppt = "application/vnd.ms-powerpoint";
            public const string pot = "application/vnd.ms-powerpoint";
            public const string pps = "application/vnd.ms-powerpoint";
            public const string ppa = "application/vnd.ms-powerpoint";
            public const string pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            public const string potx = "application/vnd.openxmlformats-officedocument.presentationml.template";
            public const string ppsx = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
            public const string ppam = "application/vnd.ms-powerpoint.addin.macroEnabled.12";
            public const string pptm = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
            public const string potm = "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
            public const string ppsm = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
        }

        //
        // Summary:
        //     Specifies the type of image data in an email message attachment.
        public static class Image
        {
            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Graphics Interchange
            //     Format (GIF).
            public const string Gif = MediaTypeNames.Image.Gif;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Tagged Image
            //     File Format (TIFF).
            public const string Tiff = MediaTypeNames.Image.Tiff;

            //
            // Summary:
            //     Specifies that the System.Net.Mime.MediaTypeNames.Image data is in Joint Photographic
            //     Experts Group (JPEG) format.
            public const string Jpeg = MediaTypeNames.Image.Jpeg;
        }
    }
}
