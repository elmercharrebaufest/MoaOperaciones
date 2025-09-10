using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Mail;
using System.Text;
using System.Web.Security;
using SustitucionMOAUtils.Services.Email.Dto;
using SustitucionMOAUtils.Email;

namespace SustitucionMOAUtils.Services.Email
{
    public class EmailComprasService : IEmailComprasService
    {
        private static readonly string DestinatariosReporteTrabajoYaHecho = ConfigurationManager.AppSettings["EmailToReporteTrabajoHecho"];
        private readonly IEmailService emailService;
        private readonly IHttpContextService httpContextService;

        public EmailComprasService(IEmailService emailService, IHttpContextService httpContextService)
        {
            this.emailService = emailService;
            this.httpContextService = httpContextService;
        }

        public void EnviarMailCotizacionCreada(Cotizacion cotizacion)
        {
            try
            {
                var cotizadorRazonSocial = cotizacion.UsuarioCreador.ObtenerProveedor().RazonSocial;
                var peticion = cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta;
                var enviarA = new List<string> { peticion.Usuario.Mail };

                var solps = peticion.Posiciones.Select(x => x.SolpPosicion.Solp);
                var mailPliego = solps.Where(x => !string.IsNullOrEmpty(x.Pliego.Email)).Select(x => x.Pliego.Email).ToList();
                mailPliego.AddRange(solps.Where(x => !string.IsNullOrEmpty(x.Pliego.SupervisorTrabajo)).Select(x => x.Pliego.SupervisorTrabajo).ToList());
                var mailCreador = solps.Where(x => !string.IsNullOrEmpty(x.UsuarioCreacion?.Mail)).Select(x => x.UsuarioCreacion.Mail).ToList();

                enviarA.AddRange(mailPliego);
                enviarA.AddRange(mailCreador);

                Log.Info($"Copia mail solicitante paso 1: {mailPliego.ToJson()}");
                Log.Info($"Copia mail solicitante: {mailCreador.ToJson()}");

                var asunto = $"NUEVA cotización creada por {cotizadorRazonSocial} - SOLPs " +
                    string.Join(", ", peticion.Posiciones.Select(x => x.SolpPosicion.Solp).Select(x => x.NroSolp).Distinct());

                emailService.EnviarMail(enviarA.Distinct().ToList(), asunto, "", null, ObtenerCuerpoCotizacionCreada(cotizacion), null, null, null, null);
            }
            catch (Exception e)
            {
                Log.Error($"Error al enviar mail GrabarCotizacion en cotizacion: " + cotizacion.Id, e);
            }

        }

        public void EnviarMailSolpLiberada(Solp solp)
        {
            try
            {
                Log.Info($"EnviarMailSolpLiberada. Nro de SOLP {solp.NroSolp}. Copia mail comprador: {solp.UsuarioCompras?.Mail}. Copia mail creador: {solp.UsuarioCreacion?.Mail}");

                var copia = new List<string> { };
                if (!string.IsNullOrEmpty(solp.UsuarioCreacion?.Mail))
                {
                    copia.Add(solp.UsuarioCreacion.Mail);
                    Log.Info($"Copia mail solicitante {solp.UsuarioCreacion.Mail}");
                }

                if (!string.IsNullOrEmpty(solp.Pliego.Email))
                {
                    //Mail del solicitante
                    copia.Add(solp.Pliego.Email);
                    Log.Info($"Copia mail solicitante paso 1 {solp.Pliego.Email}");
                }

                if (!string.IsNullOrEmpty(solp.Pliego.SupervisorTrabajo))
                {
                    //Supervisor
                    copia.Add(solp.Pliego.SupervisorTrabajo);
                    Log.Info($"Copia mail responsable de trabajo paso 2 {solp.Pliego.SupervisorTrabajo}");
                }

                var descripcionSolp = !string.IsNullOrEmpty(solp.Pliego?.NombreObra) ? solp.Pliego.NombreObra : solp.Posiciones.First().Tarea;
                var tituloAsunto = solp.TrabajoYaHecho == true ? "Nueva SOLP de trabajo ya hecho liberada" :
                    (solp.ConPresupuesto ? "Nueva SOLP con presupuesto liberada" : "Nueva SOLP liberada");
                var asunto = $"{tituloAsunto}: {solp.NroSolp} - {descripcionSolp}" +
                    (solp.Adicional == true ? $" - con Adicional OC: {solp.NroOrdenDeCompraAdicional}" : "");
                var enviarA = new List<string> { solp.UsuarioCompras.Mail };

                emailService.EnviarMail(enviarA, asunto, "", copia, ObtenerCuerpoSolpLiberada(solp), null, "");
            }
            catch (Exception ex)
            {
                Log.Error($"Error al enviar mail SOLP liberada Nro de SOLP {solp.NroSolp}", ex);
            }
        }

        public void EnviarMailFinalizacionPliegoMultiple(Pliego pliego,List<Solp> solps)
        {
            try
            {
                Log.Info($"EnviarMailFinalizacionPliegoMultiple.Pliego Multiple con nombre {pliego.NombreObra} fue finalizado.");

                var copia = new List<string> { };
                var enviarA = new List<string> { };

                foreach (Solp solp in solps) {
                    Log.Info($"Obteniendo los correos de solp numero {solp.NroSolp}");
                    if (!string.IsNullOrEmpty(solp.UsuarioCreacion?.Mail))
                    {
                        
                        copia.Add(solp.UsuarioCreacion.Mail);
                        Log.Info($"Copia mail solicitante {solp.UsuarioCreacion.Mail}");
                    }

                    if (!string.IsNullOrEmpty(solp.Pliego.Email))
                    {
                        //Mail del solicitante
                        copia.Add(solp.Pliego.Email);
                        Log.Info($"Copia mail solicitante paso 1 {solp.Pliego.Email}");
                    }

                    if (!string.IsNullOrEmpty(solp.Pliego.SupervisorTrabajo))
                    {
                        //Supervisor
                        copia.Add(solp.Pliego.SupervisorTrabajo);
                        Log.Info($"Copia mail responsable de trabajo paso 2 {solp.Pliego.SupervisorTrabajo}");
                    }

                    if (solp.UsuarioCompras != null && !string.IsNullOrEmpty(solp.UsuarioCompras.Mail))
                    {
                        enviarA.Add(solp.UsuarioCompras.Mail);
                    }
                }

                var asunto = $"En el presente mail se informa la finalización de un Nuevo Pliego Múltiple {pliego.NombreObra}";

                emailService.EnviarMail(enviarA.Distinct().ToList(), asunto, "", copia.Distinct().ToList(), ObtenerCuerpoSolpsAsociadas(pliego, solps), null, "");
            }
            catch (Exception ex)
            {
                Log.Error($"Error al enviar mail de finalizacion de Pliego Multiple: {pliego.NombreObra}", ex);
            }
        }

        public void EnviarMailPeticionDeOferta(MailPeticionDeOfertaRequest req)
        {
            var peticion = req.PeticionDeOferta;

            var asunto = req.EsEdicionPO ? $"Modificación en la PO {peticion.Id}" :
                $"MOA - Pedido de Oferta {peticion.Id}: {peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp.Pliego.NombreObra}";

            if (peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp.Adicional == true)
            {
                asunto += $" - con Adicional OC: {peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp.NroOrdenDeCompraAdicional}";
            }

            var cuerpo = GenerarCuerpoMailPeticionDeOferta(peticion, req.EsProveedor, req.ListaArchivosParaMailPO, req.EsEdicionPO, req.Proveedores);

            emailService.EnviarMail(req.Destinatarios, asunto, "", null, cuerpo, null, null, null, null, req.ArchivosAdjuntos);
        }

        public void EnviarMailReporteTrabajoYaHecho(byte[] reporteExcel, string nombreArchivo)
        {
            var asunto = "Reporte trabajos ya hechos";

            var htmlBody = $"En el presente mail se informan los trabajos aprobados bajo el concepto de \"trabajo ya hecho\".<br /><br/>Equipo Compras";
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");

            emailService.EnviarMail(
                emailService.ObtenerListaDestinatarios(new string[] { DestinatariosReporteTrabajoYaHecho }),
                asunto, "", null, alternateView, reporteExcel, nombreArchivo);
        }

        private AlternateView ObtenerCuerpoCotizacionCreada(Cotizacion cotizacion)
        {
            var logoMailPath = httpContextService.ObtenerPathLogoMail();
            var logoMailResource = new LinkedResource(logoMailPath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            var proveedor = cotizacion.PeticionDeOfertaUsuario.Usuario.ObtenerProveedor();

            var htmlBody = "En el presente mail se informa la cotización realizada para la SOLP " +
                $"{string.Join(", ", cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Posiciones.Select(x => x.SolpPosicion.Solp).Select(x => x.NroSolp).Distinct())} " +
                $"y la PO {cotizacion.PeticionDeOfertaUsuario.PeticionDeOferta.Id}, generada por el proveedor {proveedor.RazonSocial} ({proveedor.CUIT}). <br /> <br/>";

            var todasLasPosicionesNoDisponibles = cotizacion.CotizacionPosiciones.All(x => x.NoDisponible != null && x.NoDisponible.Value);
            var algunaPosicionNoDisponible = cotizacion.CotizacionPosiciones.Any(x => x.NoDisponible != null && x.NoDisponible.Value);
            var noRespetaMateriales = cotizacion.RespetaMateriales == false;

            if (todasLasPosicionesNoDisponibles || algunaPosicionNoDisponible || noRespetaMateriales)
            {
                htmlBody += $"<strong>Nota:</strong><br/>";
            }
            if (todasLasPosicionesNoDisponibles)
            {
                htmlBody += $"El proveedor no cuenta con el material disponible.<br/>";
            }
            else if (algunaPosicionNoDisponible)
            {
                htmlBody += $"El proveedor no cuenta con algún material disponible.<br/>";
            }
            if (noRespetaMateriales)
            {
                htmlBody += $"La propuesta no cumple con las especificaciones técnicas solicitadas. Revisar con prioridad. <br/>";
            }

            htmlBody += " <br/>Puede visualizar la cotización en www.moaoperaciones.com.ar " +
                 "<br/><br/>Saludos Cordiales<br/>" +
                 "Molinos Agro S.A. <br/><br/> " +
                  @"<img width:'5%' src='cid:" + logoMailResource.ContentId + @"'/>";

            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(logoMailResource);
            return alternateView;
        }

        private AlternateView ObtenerCuerpoSolpLiberada(Solp solp)
        {
            var logoMailPath = httpContextService.ObtenerPathLogoMail();
            var logoMailResource = new LinkedResource(logoMailPath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            var htmlBody = $"En el presente mail se informa la liberación de la SOLP {solp.NroSolp} generada con Molinos Agro S.A. <br /><br/>";
            var esMaterial = solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES";

            // Agregar la tabla de posiciones y subposiciones
            if (solp.Posiciones != null && solp.Posiciones.Any())
            {
                htmlBody += "<b>Detalle:</b><br/>";
                htmlBody += "<br/>";

                foreach (var posicion in solp.Posiciones)
                {
                    htmlBody += "<table style=\"border-collapse: collapse; border: 2px solid #ddd; text-align: center; font-size: 13px; width: 100%;\">";
                    htmlBody += "<tr>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Posición</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Centro</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Descripción</th>";
                    if (esMaterial)
                    {
                        htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Precio Bruto</th>";
                    }

                    htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Moneda</th>" +
                                "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Grupo de compras</th>" +
                                "</tr>";

                    // Agregar la fila para la posición
                    htmlBody += "<tr>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.Indice}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.Centro.Codigo}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{(!string.IsNullOrEmpty(posicion.MaterialSolp?.Descripcion) ? posicion.MaterialSolp.Descripcion : posicion.Tarea)}</td>";

                    if (esMaterial)
                    {
                        htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Unidad?.CodigoSap}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Cantidad.Value:n2}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.PrecioBruto.Value:n2}</td>";
                    }

                    htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Moneda?.CodigoSap}</td>" +
                                $"<td style=\"border: 2px solid #ddd;\">{posicion.GrupoCompras?.CodigoSap}</td>" +
                                "</tr>";

                    if (!esMaterial)
                    {
                        htmlBody += "<tr>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Subposición</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Tarea a subcontratar</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Precio bruto</th>" +
                                    "</tr>";

                        foreach (var subpos in posicion.Subposiciones)
                        {
                            htmlBody += "<tr>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Numero}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Tarea}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Cantidad.Value:n2}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.Unidad.CodigoSap}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{subpos.PrecioBruto.Value:n2}</td>" +
                                        "</tr>";
                        }
                    }

                    htmlBody += "</table>";
                    htmlBody += "<br/>";
                }
            }

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales<br/>" +
                "Molinos Agro S.A. <br/><br/> " +
                 @"<img width='15%' src='cid:" + logoMailResource.ContentId + @"'/>";
            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(logoMailResource);
            return alternateView;
        }

        private AlternateView ObtenerCuerpoSolpsAsociadas(Pliego pliego, List<Solp> solps)
        {
            var logoMailPath = httpContextService.ObtenerPathLogoMail();
            var logoMailResource = new LinkedResource(logoMailPath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            var htmlBody = $"En el presente mail se informa la finalización de un Nuevo Pliego Múltiple {pliego.NombreObra} <br /><br/>";

            foreach (Solp solp in solps) {
                var esMaterial = solp.Posiciones.FirstOrDefault().TipoPosicion.Codigo == "MATERIALES";

                // Agregar la tabla de posiciones y subposiciones
                if (solp.Posiciones != null && solp.Posiciones.Any())
                {
                    htmlBody += $"<b>Solp Nro {solp.NroSolp}:</b><br/>";
                    htmlBody += "<br/>";

                    foreach (var posicion in solp.Posiciones)
                    {
                        htmlBody += "<table style=\"border-collapse: collapse; border: 2px solid #ddd; text-align: center; font-size: 13px; width: 100%;\">";
                        htmlBody += "<tr>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Posición</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 100px;\">Centro</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Descripción</th>";
                        if (esMaterial)
                        {
                            htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Precio Bruto</th>";
                        }

                        htmlBody += "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Moneda</th>" +
                                    "<th style=\"border: 2px solid #ddd; background-color: #017940; color: white; padding: 5px 0; width: 250px;\">Grupo de compras</th>" +
                                    "</tr>";

                        // Agregar la fila para la posición
                        htmlBody += "<tr>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Indice}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.Centro.Codigo}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{(!string.IsNullOrEmpty(posicion.MaterialSolp?.Descripcion) ? posicion.MaterialSolp.Descripcion : posicion.Tarea)}</td>";

                        if (esMaterial)
                        {
                            htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Unidad?.CodigoSap}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{posicion.Cantidad.Value:n2}</td>" +
                                        $"<td style=\"border: 2px solid #ddd;\">{posicion.PrecioBruto.Value:n2}</td>";
                        }

                        htmlBody += $"<td style=\"border: 2px solid #ddd;\">{posicion.Moneda?.CodigoSap}</td>" +
                                    $"<td style=\"border: 2px solid #ddd;\">{posicion.GrupoCompras?.CodigoSap}</td>" +
                                    "</tr>";

                        if (!esMaterial)
                        {
                            htmlBody += "<tr>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Subposición</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Tarea a subcontratar</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Cantidad</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">UM</th>" +
                                        "<th style=\"border: 2px solid #ddd; background-color: #2e8b57; color: white; padding: 5px 0; width: 250px;\">Precio bruto</th>" +
                                        "</tr>";

                            foreach (var subpos in posicion.Subposiciones)
                            {
                                htmlBody += "<tr>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Numero}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Tarea}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Cantidad.Value:n2}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.Unidad.CodigoSap}</td>" +
                                            $"<td style=\"border: 2px solid #ddd;\">{subpos.PrecioBruto.Value:n2}</td>" +
                                            "</tr>";
                            }
                        }

                        htmlBody += "</table>";
                        htmlBody += "<br/>";
                    }
                }

            }

            htmlBody += "En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                        "<br/><br/>Saludos Cordiales<br/>" +
                        "Molinos Agro S.A. <br/><br/> " +
                         @"<img width='15%' src='cid:" + logoMailResource.ContentId + @"'/>";

            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(logoMailResource);
            return alternateView;
        }

        private AlternateView GenerarCuerpoMailPeticionDeOferta(PeticionDeOferta peticion, bool esProveedor, List<FileDto> listaArchivosParaMailPO, bool esEdicionPO, List<string> proveedores = null)
        {
            var filePath = httpContextService.ObtenerPathLogoMail();
            var logoMoaResource = new LinkedResource(filePath) { ContentId = Guid.NewGuid().ToString() };

            var htmlBodyBuilder = new StringBuilder();
            htmlBodyBuilder.Append($"En el presente mail se informa la {(esEdicionPO ? "modificación de la " : "nueva ")}PO {peticion.Id} ");

            if (esProveedor)
            {
                htmlBodyBuilder.Append("generada con Molinos Agro S.A <br />");
            }
            else
            {
                htmlBodyBuilder.Append("que se envió a los siguientes proveedores: <br />");
                proveedores.ForEach(prov => htmlBodyBuilder.AppendLine($"{prov} <br />"));
            }

            if (!string.IsNullOrEmpty(peticion.Observaciones))
            {
                var observacionesFormatted = peticion.Observaciones.Replace("\n", "<br />");
                htmlBodyBuilder.AppendLine($"<br />Observaciones: {observacionesFormatted} <br /><br />");
            }

            if (esProveedor)
            {
                var primeraPosicion = peticion.Posiciones.First().SolpPosicion;
                var esServicio = peticion.Posiciones.First().SolpPosicion.Solp.Posiciones.Select(x => x.TipoPosicion.Codigo).FirstOrDefault() == "SERVICIO";

                if (esServicio)
                {
                    var solpConPliegoSinRepetir
                        = peticion
                            .Posiciones
                            .GroupBy(x => x.SolpPosicion.Solp.Id)
                            .Select(x => x.First()) // groupBy + select => distinctBy
                            .Where(x => ValidarSolpSiTienePliego(x.SolpPosicion));

                    var cantidadSolpConPliego = solpConPliegoSinRepetir.Count();

                    if (cantidadSolpConPliego == 1)
                    {
                        var posicion = solpConPliegoSinRepetir.Single().SolpPosicion;
                        var downloadLinkUrl = ConfigurationManager.AppSettings["ida:RedirectUri"] +
                            $"/api/compras/DescargarPliegoDesdeLink?solpId={posicion.Solp.Id}&token={posicion.Solp.EmailLinkToken}";

                        htmlBodyBuilder
                            .Append("<p style = 'line-height: 24px; font-size: 16px; margin: 0;' align = 'center' >")
                            .Append(" Para descargar el legajo, haga ")
                            .Append($"<a href = '{downloadLinkUrl}' download rel='noopener noreferrer'>click aquí</a>")
                            .AppendLine("</p> <br />");
                    }
                    if (cantidadSolpConPliego > 1)
                    {
                        htmlBodyBuilder.Append("<p style = 'line-height: 24px; font-size: 16px; margin: 0;' align = 'center' >")
                            .Append("Algunas SOLP tienen legajo disponible para descarga. Haga click en los elementos para descargarlos")
                            .Append("</p> <ul>");

                        foreach (Solp solp in solpConPliegoSinRepetir.Select(x => x.SolpPosicion.Solp))
                        {
                            var downloadLinkUrl = ConfigurationManager.AppSettings["ida:RedirectUri"] +
                                $"/api/compras/DescargarPliegoDesdeLink?solpId={solp.Id}&token={solp.EmailLinkToken}";

                            htmlBodyBuilder.Append($"<li> <a href = '{downloadLinkUrl}' download rel='noopener noreferrer'>{solp.NroSolp}</a> </li>");
                        }
                        htmlBodyBuilder.AppendLine("</ul> <br />");
                    }
                }

                if (peticion.AdjuntoPliego == true)
                {
                    htmlBodyBuilder.Append("<p style = 'line-height: 24px; font-size: 16px; margin: 0;' align = 'center' >")
                        .Append("A continuación, se listan los documentos de pliego de generalidades y documentación relevante para la contratista:")
                        .AppendLine("<ul>");

                    foreach (FileDto archivo in listaArchivosParaMailPO)
                    {
                        htmlBodyBuilder.AppendLine("<li>")
                            .Append($"<a href='{archivo.Url}' download rel='noopener noreferrer'>{archivo.Filename}</a>")
                            .AppendLine("</li>");
                    }
                    htmlBodyBuilder.AppendLine("</ul>");
                }

                if (primeraPosicion.Solp.Pliego.RequisitoCiberseguridad == true && esServicio)
                {
                    htmlBodyBuilder
                        .AppendLine("<p>Le enviamos los requisitos de ciberseguridad obligatorios para todos los proveedores, contratistas y consultores que se conecten a la red LAN y/o VPN, o a las aplicaciones internas de Molinos Agro durante la prestación de sus servicios. Por favor, asegúrese de cumplir con estos requisitos para garantizar la seguridad de nuestras operaciones:</p>")
                        .AppendLine("<p>Solicitamos puedan firmar el documento adjunto considerando las siguientes condiciones:</p>")
                        .AppendLine("<ul>")
                        .AppendLine("<li>Si el servicio es prestado directamente por su empresa, el documento debe firmarlo el titular o apoderado legal de la empresa.</li>")
                        .AppendLine("<li>Si el servicio es prestado por un colaborador de la empresa, el documento deberá ser firmado por la empresa principal y no por su colaborador.</li>")
                        .AppendLine("<li>Cualquier otra prestación en la que se conecten a la red LAN y/o VPN, o aplicaciones internas de Molinos Agro requerirá la firma de la empresa principal.</li>")
                        .AppendLine("</ul>")
                        .AppendLine("<p>Puede descargar el documento de requisitos de ciberseguridad desde el siguiente enlace: <a href='https://b2cmoagro.blob.core.windows.net/moaopublic/Requisitos%20de%20seguridad%20de%20terceros_v1.4.docx' download rel='noopener noreferrer'>Requisitos de seguridad de terceros_v1.4</a></p>");
                }
            }

            htmlBodyBuilder.AppendLine("<br />En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar ")
                .AppendLine("<br/><br/>Saludos Cordiales<br/>")
                .AppendLine("Molinos Agro S.A. <br/><br/> ")
                .AppendLine("<img width:'5%' src='cid:").Append(logoMoaResource.ContentId).Append("'/>");

            var alternateView = AlternateView.CreateAlternateViewFromString(htmlBodyBuilder.ToString(), null, "text/html");
            alternateView.LinkedResources.Add(logoMoaResource);
            return alternateView;
        }

        private bool ValidarSolpSiTienePliego(SolpPosicion posicion)
        {
            var tieneCondicionEspecial =
                posicion.Solp.TrabajoYaHecho == true ||
                posicion.Solp.Urgencia == true ||
                posicion.Solp.Adicional == true ||
                posicion.Solp.CondEspProveedorAsignado == true;

            var casoConPliego =
                posicion.Solp.TipoSolp?.Codigo == "CON_PLIEGO" || (posicion.Solp.TipoSolp?.Codigo == "SIN_PLIEGO" &&
                posicion.Solp.Pliego.Archivos.Any(x => x.FileKey == FileKeys.AdjuntoCotizacionesSolp || x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp) &&
                !tieneCondicionEspecial);

            return casoConPliego;
        }
    }
}
