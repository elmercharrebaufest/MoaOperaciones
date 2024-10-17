using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Helpers
{
    public static class ImageResizer
    {
        private const int MAX_WIDTH = 600; // Máximo ancho permitido
        private const int MAX_HEIGHT = 800; // Máximo alto permitido

        public static string AjustarImagenesEnHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html; // Devuelve el HTML original si está vacío o solo contiene espacios en blanco.
            }

            // Expresión regular para encontrar imágenes en base64 en el HTML
            string pattern = @"<img[^>]*?src=""data:image/(?<type>.+?);base64,(?<data>.+?)""[^>]*?>";

            // Reemplazamos las imágenes una por una con el tamaño ajustado
            string result = Regex.Replace(html, pattern, new MatchEvaluator(ReemplazarImagen));

            return result;
        }

        private static string ReemplazarImagen(Match match)
        {
            // Extraer información de la imagen del match
            string base64Data = match.Groups["data"].Value;
            string fullImageTag = match.Value;

            // Intentamos cargar la imagen desde el base64
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    using (var img = Image.FromStream(ms))
                    {
                        int originalWidth = img.Width;
                        int originalHeight = img.Height;
                        int newWidth = originalWidth;
                        int newHeight = originalHeight;

                        // Verificar si tiene atributos de width o height
                        Regex widthRegex = new Regex(@"width=""(\d+?)""");
                        Regex heightRegex = new Regex(@"height=""(\d+?)""");

                        Match widthMatch = widthRegex.Match(fullImageTag);
                        Match heightMatch = heightRegex.Match(fullImageTag);

                        // Si tienen width o height, ajustarlos si superan los límites
                        if (widthMatch.Success || heightMatch.Success)
                        {
                            // Obtener los valores actuales
                            int? width = widthMatch.Success ? int.Parse(widthMatch.Groups[1].Value) : (int?)null;
                            int? height = heightMatch.Success ? int.Parse(heightMatch.Groups[1].Value) : (int?)null;

                            // Si solo uno está presente, calcular el otro manteniendo la relación de aspecto
                            if (width.HasValue && !height.HasValue)
                            {
                                height = (int)((originalHeight / (double)originalWidth) * width.Value);
                            }
                            else if (!width.HasValue && height.HasValue)
                            {
                                width = (int)((originalWidth / (double)originalHeight) * height.Value);
                            }

                            // Ajustar si superan el máximo
                            if (width.Value > MAX_WIDTH || height.Value > MAX_HEIGHT)
                            {
                                double aspectRatio = originalWidth / (double)originalHeight;
                                if (width.Value > MAX_WIDTH)
                                {
                                    width = MAX_WIDTH;
                                    height = (int)(MAX_WIDTH / aspectRatio);
                                }
                                if (height.Value > MAX_HEIGHT)
                                {
                                    height = MAX_HEIGHT;
                                    width = (int)(MAX_HEIGHT * aspectRatio);
                                }
                            }

                            // Reemplazar los valores en el tag <img>
                            fullImageTag = widthRegex.Replace(fullImageTag, $@"width=""{width}""");
                            fullImageTag = heightRegex.Replace(fullImageTag, $@"height=""{height}""");
                        }
                        else
                        {
                            // Si no tienen width o height, asignarlos con las proporciones adecuadas
                            double aspectRatio = originalWidth / (double)originalHeight;

                            if (originalWidth > MAX_WIDTH)
                            {
                                newWidth = MAX_WIDTH;
                                newHeight = (int)(MAX_WIDTH / aspectRatio);
                            }
                            if (newHeight > MAX_HEIGHT)
                            {
                                newHeight = MAX_HEIGHT;
                                newWidth = (int)(MAX_HEIGHT * aspectRatio);
                            }

                            // Añadir width y height al tag <img>
                            fullImageTag = fullImageTag.Replace("<img", $@"<img width=""{newWidth}"" height=""{newHeight}""");
                        }
                    }
                }
            }
            catch (FormatException)
            {
                // Si hay un error al decodificar la imagen base64, simplemente devolvemos el tag original
                return fullImageTag;
            }
            catch (Exception ex)
            {
                // Manejar cualquier otro error inesperado
                Console.WriteLine($"Error procesando la imagen: {ex.Message}");
                return fullImageTag; // Devolvemos el tag original en caso de error
            }

            return fullImageTag;
        }
    }
}
