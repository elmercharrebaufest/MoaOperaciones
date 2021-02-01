using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class TicketPesadaService : ITicketPesadaService
    {
        readonly IScatoConsumer scatoConsumer;

        public TicketPesadaService(IScatoConsumer scatoConsumer)
        {
            this.scatoConsumer = scatoConsumer;
        }

        public byte[] ObtenerTicket(ConsultaTicketPesada consultaTicketPesada)
        {
            var fotosCP = this.scatoConsumer.ObtenerFotoCartaPorte(consultaTicketPesada.NumeroCartaPorte.ToString());

            var outputMemStream = new MemoryStream();

            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                zipStream.SetLevel(3);

                foreach (CartaPorteFoto foto in fotosCP)
                {
                    MemoryStream fotoMemoryStream = new MemoryStream(Convert.FromBase64String(foto.Foto));

                    ZipEntry entry = new ZipEntry(string.Concat(foto.CartaPorteID, ".jpg"))
                    {
                        DateTime = DateTime.Now
                    };
                    zipStream.PutNextEntry(entry);
                    StreamUtils.Copy(fotoMemoryStream, zipStream, new byte[4096]);
                    zipStream.CloseEntry();
                }

                zipStream.IsStreamOwner = false;
            }

            outputMemStream.Position = 0;

            if (consultaTicketPesada.Mail != "")
            {
                EnviarMail(consultaTicketPesada, outputMemStream.ToArray());
            }

            return outputMemStream.ToArray();
        }

        private void EnviarMail(ConsultaTicketPesada consultaTicketPesada, byte[] adjunto)
        {
            var asunto = string.Format("Ticket pesada CCPP {0} - Pantente {1}", consultaTicketPesada.NumeroCartaPorte, consultaTicketPesada.PatenteCamion);

            var nombreAchivo = string.Format("Ticket Pesada CCPP {0}", consultaTicketPesada.NumeroCartaPorte);

            EmailSender.EnviarMail(new List<string> { consultaTicketPesada.Mail }, asunto, asunto, null, null, adjunto, nombreAchivo);
        }
    }
}
