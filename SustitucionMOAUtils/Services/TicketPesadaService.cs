using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
using SustitucionMOAWS.WSConsumers;
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
        private readonly IScatoComandosConsumer scatoComandosConsumer;

        public TicketPesadaService(IScatoConsumer scatoConsumer, IScatoComandosConsumer scatoComandosConsumer)
        {
            this.scatoConsumer = scatoConsumer;
            this.scatoComandosConsumer = scatoComandosConsumer;
        }

        public byte[] ObtenerTicket(ConsultaTicketPesada consultaTicketPesada)
        {
            ResultadoTickets resultado;
            try
            {
                resultado = scatoComandosConsumer.ObtenerTicketPesada(consultaTicketPesada.NumeroCartaPorte);
            }
            catch
            {
                throw new ValidationCustomException("No se encontró una CCPP con el número ingresado.");
            }

            if (resultado.Patente == "" || resultado.Patente != consultaTicketPesada.PatenteCamion)
            {
                throw new ValidationCustomException("La patente del cambio no coincide con la patente del camión.");
            }

            var outputMemStream = new MemoryStream();

            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                zipStream.SetLevel(3);

                if (resultado.TicketPesada != null)
                {
                    if (resultado.TicketPesada.Length > 0)
                    {
                        ZipEntry ticketPesadaEntry = new ZipEntry("TicketPesada.pdf")
                        {
                            DateTime = DateTime.Now,

                        };
                        Stream ticketPesadaStream = new MemoryStream(resultado.TicketPesada);
                        zipStream.PutNextEntry(ticketPesadaEntry);
                        StreamUtils.Copy(ticketPesadaStream, zipStream, new byte[4096]);
                        zipStream.CloseEntry();
                    }
                }


                if (resultado.TicketReciboMunicipal != null)
                {
                    if (resultado.TicketReciboMunicipal.Length > 0)
                    {
                        ZipEntry ticketReciboMunicipalEntry = new ZipEntry("TicketReciboMunicipal.pdf")
                        {
                            DateTime = DateTime.Now,
                        };
                        Stream ticketReciboMunicipalStream = new MemoryStream(resultado.TicketReciboMunicipal);
                        zipStream.PutNextEntry(ticketReciboMunicipalEntry);
                        StreamUtils.Copy(ticketReciboMunicipalStream, zipStream, new byte[4096]);
                        zipStream.CloseEntry();
                    }
                }

                if (resultado.FotoCP != null)
                {
                    if (resultado.FotoCP.Fotos.Length > 0)
                    {
                        int i = 1;                         
                        foreach (FotoDto foto in resultado.FotoCP.Fotos)
                        {
                            Stream fotoMemoryStream = new MemoryStream(foto.Foto);

                            ZipEntry entry = new ZipEntry(string.Concat($"Foto CCPP {i++}.jpg"))
                            {
                                DateTime = DateTime.Now
                            };
                            zipStream.PutNextEntry(entry);
                            StreamUtils.Copy(fotoMemoryStream, zipStream, new byte[4096]);
                            zipStream.CloseEntry();
                        }
                    }
                }

                zipStream.IsStreamOwner = false;
            }

            outputMemStream.Position = 0;

            if (consultaTicketPesada.Mail != null)
            {
                if (consultaTicketPesada.Mail.Length > 0)
                {
                    try
                    {
                        EnviarMail(consultaTicketPesada, outputMemStream.ToArray());
                    }
                    catch
                    {
                        //En el caso de que no podamos mandar el mail, ignoramos la excepción
                    }
                }
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
