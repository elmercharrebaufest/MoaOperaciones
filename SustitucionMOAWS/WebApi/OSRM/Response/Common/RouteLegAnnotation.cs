using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi.OSRM.Response.Common
{
    /// <summary>
    /// Annotation of the whole route leg with fine-grained information about each segment or node id.
    /// </summary>
    public class RouteLegAnnotation
    {
        /// <summary>
        /// The distance, in metres, between each pair of coordinates.
        /// </summary>
        [JsonProperty("distance")]
        public float[] Distance { get; set; }

        /// <summary>
        /// The duration between each pair of coordinates, in seconds. Does not include the duration of any turns.
        /// </summary>
        [JsonProperty("duration")]
        public float[] Duration { get; set; }

        /// <summary>
        /// The index of the datasource for the speed between each pair of coordinates.
        /// 0 is the default profile, other values are supplied via --segment-speed-file to osrm-contract or osrm-customize.
        /// String-like names are in the metadata.datasource_names array.
        /// </summary>
        [JsonProperty("datasources")]
        public int[] Datasources { get; set; }

        /// <summary>
        /// The OSM node ID for each coordinate along the route, excluding the first/last user-supplied coordinates.
        /// </summary>
        [JsonProperty("nodes")]
        public long[] Nodes { get; set; }

        /// <summary>
        /// The weights between each pair of coordinates. Does not include any turn costs.
        /// </summary>
        [JsonProperty("weight")]
        public float[] Weight { get; set; }

        /// <summary>
        /// Convenience field, calculation of distance / duration rounded to one decimal place.
        /// </summary>
        [JsonProperty("speed")]
        public float[] Speed { get; set; }

        /// <summary>
        /// Metadata related to other annotations.
        /// </summary>
        [JsonProperty("metadata")]
        public RouteLegAnnotationMetadata Metadata { get; set; }
    }
}
