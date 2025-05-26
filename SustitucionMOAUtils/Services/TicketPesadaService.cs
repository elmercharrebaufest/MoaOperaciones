using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ScatoComandosWebService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class TicketPesadaService : ITicketPesadaService
    {
        private readonly IScatoComandosConsumer scatoComandosConsumer;

        public TicketPesadaService(IScatoComandosConsumer scatoComandosConsumer)
        {
            this.scatoComandosConsumer = scatoComandosConsumer;
        }

        public List<ArchivoDescargaDto> ObtenerTicket(ConsultaTicketPesada consultaTicketPesada)
        {
            ResultadoTickets resultado;
            try
            {
                resultado = scatoComandosConsumer.ObtenerTicketPesada(consultaTicketPesada.NumeroCartaPorte);
            }
            catch (Exception)
            {
                throw new ValidationCustomException("No se encontró una CCPP con el número ingresado.");
            }

            if (resultado.Patente == "" || resultado.Patente.ToLower() != consultaTicketPesada.PatenteCamion.ToLower())
            {
                throw new ValidationCustomException("La patente del cambio no coincide con la patente del camión.");
            }

            var listadoArchivos = GenerarListadoArchivos(resultado);
            byte[] archivoResultado = listadoArchivos.FirstOrDefault(a => a.Nombre.Contains("zip")).Datos;

            if (consultaTicketPesada.Mail != null)
            {
                if (consultaTicketPesada.Mail.Length > 0)
                {
                    try
                    {
                        Task.Run(() => EnviarMail(consultaTicketPesada, archivoResultado));
                    }
                    catch
                    {
                        //En el caso de que no podamos mandar el mail, ignoramos la excepción
                    }
                }
            }

            return listadoArchivos;
        }

        public List<ArchivoDescargaDto> ObtenerTicketNoGranos(ConsultaTicketPesadaNoGranos consultaTicketPesadaNoGranos)
        {
            ResultadoConsultarTicketsNoGranos resultado;
            try
            {
                resultado = scatoComandosConsumer.ObtenerTicketPesadaNoGranos(consultaTicketPesadaNoGranos.PatenteCamion, consultaTicketPesadaNoGranos.FechaDesde, consultaTicketPesadaNoGranos.FechaHasta);
            }
            catch (Exception)
            {
                throw new ValidationCustomException("No se encontró una CCPP con el número ingresado.");
            }


            var listadoArchivos = GenerarListadoArchivosNoGranos(resultado);
            byte[] archivoResultado = listadoArchivos.FirstOrDefault(a => a.Nombre.Contains("zip")).Datos;

            /*
            if (consultaTicketPesada.Mail != null)
            {
                if (consultaTicketPesada.Mail.Length > 0)
                {
                    try
                    {
                        Task.Run(() => EnviarMail(consultaTicketPesada, archivoResultado));
                    }
                    catch
                    {
                        //En el caso de que no podamos mandar el mail, ignoramos la excepción
                    }
                }
            }
             */

            return listadoArchivos;
        }

        private static List<ArchivoDescargaDto> GenerarListadoArchivos(ResultadoTickets resultado)
        {
            var outputMemStream = new MemoryStream();

            var listado = new List<ArchivoDescargaDto>();

            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                zipStream.SetLevel(3);

                if (resultado.TicketPesada != null && resultado.TicketPesada.Length > 0)
                {
                    string nombreArchivo = $"Ticket Pesada {resultado.CP}.pdf";
                    AgregarAStream(resultado.TicketPesada, nombreArchivo, zipStream);
                    listado.Add(new ArchivoDescargaDto { Nombre = nombreArchivo, Datos = resultado.TicketPesada });

                }

                if (resultado.CertificadoCP != null && resultado.CertificadoCP.Length > 0)
                {
                    string nombreArchivo = $"Certificado CP {resultado.CP}.pdf";
                    AgregarAStream(resultado.CertificadoCP, nombreArchivo, zipStream);
                    listado.Add(new ArchivoDescargaDto { Nombre = nombreArchivo, Datos = resultado.CertificadoCP });
                }


                if (resultado.TicketReciboMunicipal != null && resultado.TicketReciboMunicipal.Length > 0)
                {
                    string nombreArchivo = $"Ticket Recibo Municipal {resultado.CP}.pdf";
                    AgregarAStream(resultado.TicketReciboMunicipal, nombreArchivo, zipStream);
                    listado.Add(new ArchivoDescargaDto { Nombre = nombreArchivo, Datos = resultado.TicketReciboMunicipal });
                }


                if (resultado.FotoCP != null && resultado.FotoCP.Fotos.Length > 0)
                {
                    int i = 1;
                    foreach (FotoDto foto in resultado.FotoCP.Fotos)
                    {
                        Stream fotoMemoryStream = new MemoryStream(foto.Foto);
                        AgregarAStream(foto.Foto, $"Foto CCPP {resultado.CP} - {i}.jpg", zipStream);
                        listado.Add(new ArchivoDescargaDto { Nombre = $"Foto CCPP {resultado.CP} - {i++}.jpg", Datos = foto.Foto });
                    }
                }


                zipStream.IsStreamOwner = false;
            }

            outputMemStream.Position = 0;

            var archivoResultado = outputMemStream.ToArray();

            if (archivoResultado.Length < 50)
            {
                throw new ValidationCustomException("No hay documentos para la carta de porte ingresada.");
            }

            listado.Add(new ArchivoDescargaDto { Nombre = $"Documentación CCPP {resultado.CP}.zip", Datos = archivoResultado });

            return listado;
        }

        private static List<ArchivoDescargaDto> GenerarListadoArchivosNoGranos(ResultadoConsultarTicketsNoGranos resultado)
        {
            var outputMemStream = new MemoryStream();
            var listado = new List<ArchivoDescargaDto>();
            using (var zipStream = new ZipOutputStream(outputMemStream))
            {
                zipStream.SetLevel(3);

                if (resultado.TicketsNoGranos != null && resultado.TicketsNoGranos.Length > 0)
                {
                    int i = 1;
                    foreach (var ticket in resultado.TicketsNoGranos)
                    {
                        if (ticket.TicketPesada != null && ticket.TicketPesada.Length > 0)
                        {
                            string nombreArchivo = $"Ticket Pesada {ticket.NumeroOrdenOperaciones} - {i++}.pdf";
                            AgregarAStream(ticket.TicketPesada, nombreArchivo, zipStream);
                            listado.Add(new ArchivoDescargaDto { Nombre = nombreArchivo, Datos = ticket.TicketPesada });
                        }
                        if (ticket.TicketReciboMunicipal != null && ticket.TicketReciboMunicipal.Length > 0)
                        {
                            string nombreArchivo = $"Ticket Recibo Municipal {ticket.NumeroOrdenOperaciones} - {i++}.pdf";
                            AgregarAStream(ticket.TicketReciboMunicipal, nombreArchivo, zipStream);
                            listado.Add(new ArchivoDescargaDto { Nombre = nombreArchivo, Datos = ticket.TicketReciboMunicipal });
                        }
                        //if (ticket.CPEDG != null && ticket.CPEDG.Length > 0)
                        //{
                        //    string nombreArchivo = $"Certificado CP {ticket.NumeroOrdenOperaciones} - {i++}.pdf";
                        //    AgregarAStream(ticket.CPEDG, nombreArchivo, zipStream);
                        //    listado.Add(new ArchivoDescargaDto { Nombre = nombreArchivo, Datos = ticket.CPEDG });
                        //}
                    }

                }

                zipStream.IsStreamOwner = false;
            }
            outputMemStream.Position = 0;
            var archivoResultado = outputMemStream.ToArray();
            if (archivoResultado.Length < 50)
            {
                throw new ValidationCustomException("No hay documentos para la Patente y fechas elegidas.");
            }
            listado.Add(new ArchivoDescargaDto { Nombre = $"Documentación CCPP {resultado.TicketsNoGranos[0].NumeroOrdenOperaciones}.zip", Datos = archivoResultado });
            return listado;
        }

        private static void AgregarAStream(byte[] archivo, string nombreArchivo, ZipOutputStream zipStream)
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

        private async Task EnviarMail(ConsultaTicketPesada consultaTicketPesada, byte[] adjunto)
        {
            var asunto = string.Format("Ticket pesada CCPP {0} - Pantente {1}", consultaTicketPesada.NumeroCartaPorte, consultaTicketPesada.PatenteCamion);

            var nombreAchivo = string.Format("Ticket Pesada CCPP {0}.zip", consultaTicketPesada.NumeroCartaPorte);

            string mensaje = string.Format("Te enviamos el archivo comprimido de la CCPP {0}.", consultaTicketPesada.NumeroCartaPorte);

            await EmailSender.EnviarMailAsync(new List<string> { consultaTicketPesada.Mail }, asunto, mensaje, null, null, adjunto, nombreAchivo);
        }
    }
}
