using Newtonsoft.Json;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// Metadata related to other annotations.
    /// </summary>
    public class RouteLegAnnotationMetadata
    {
        /// <summary>
        /// The names of the datasources used for the speed between each pair of coordinates.
        /// lua profile is the default profile, other values arethe filenames supplied via --segment-speed-file to osrm-contract or osrm-customize.
        /// </summary>
        [JsonProperty("datasource_names")]
        public string[] DatasourceNames { get; set; }
    }
}
