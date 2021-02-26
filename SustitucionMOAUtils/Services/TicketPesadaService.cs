using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Collections.Generic;
using System.IO;

namespace SustitucionMOAUtils.Services
{
    public class TicketPesadaService : ITicketPesadaService
    {
        private readonly IScatoComandosConsumer scatoComandosConsumer;

        public TicketPesadaService(IScatoComandosConsumer scatoComandosConsumer)
        {
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

            byte[] archivoResultado = GenerarArchivoZip(resultado);

            if (archivoResultado.Length < 50)
            {
                throw new ValidationCustomException("No hay documentos para la carta de porte ingresada.");
            }

            if (consultaTicketPesada.Mail != null)
            {
                if (consultaTicketPesada.Mail.Length > 0)
                {
                    try
                    {
                        EnviarMail(consultaTicketPesada, archivoResultado);
                    }
                    catch
                    {
                        //En el caso de que no podamos mandar el mail, ignoramos la excepción
                    }
                }
            }

            return archivoResultado;
        }

        private static byte[] GenerarArchivoZip(ResultadoTickets resultado)
        {
            var outputMemStream = new MemoryStream();

            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                zipStream.SetLevel(3);

                if (resultado.TicketPesada != null)
                {
                    if (resultado.TicketPesada.Length > 0)
                    {
                        AgregarAStream(resultado.TicketPesada,"TicketPesada.pdf", zipStream);
                    }
                }

                if (resultado.TicketReciboMunicipal != null)
                {
                    if (resultado.TicketReciboMunicipal.Length > 0)
                    {
                        AgregarAStream(resultado.TicketReciboMunicipal, "TicketReciboMunicipal.pdf", zipStream);
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

                            AgregarAStream(foto.Foto, $"Foto CCPP { i++}.jpg", zipStream);
                        }
                    }
                }

                zipStream.IsStreamOwner = false;
            }

            outputMemStream.Position = 0;

            var archivoResultado = outputMemStream.ToArray();
            return archivoResultado;
        }

        private static void AgregarAStream(byte[] archivo, string nombreArchivo , ZipOutputStream zipStream)
        {
            ZipEntry entry = new ZipEntry(nombreArchivo)
            {
                DateTime = DateTime.Now,
            };

            Stream stream = new MemoryStream(archivo);
            zipStream.PutNextEntry(entry);
            StreamUtils.Copy(stream, zipStream, new byte[4096]);
            zipStream.CloseEntry();
        }

        private void EnviarMail(ConsultaTicketPesada consultaTicketPesada, byte[] adjunto)
        {
            var asunto = string.Format("Ticket pesada CCPP {0} - Pantente {1}", consultaTicketPesada.NumeroCartaPorte, consultaTicketPesada.PatenteCamion);

            var nombreAchivo = string.Format("Ticket Pesada CCPP {0}.zip", consultaTicketPesada.NumeroCartaPorte);

            string mensaje = string.Format("Te enviamos el archivo comprimido de la CCPP {0}.", consultaTicketPesada.NumeroCartaPorte);

            EmailSender.EnviarMail(new List<string> { consultaTicketPesada.Mail }, asunto, mensaje, null, null, adjunto, nombreAchivo);
        }
    }
}
