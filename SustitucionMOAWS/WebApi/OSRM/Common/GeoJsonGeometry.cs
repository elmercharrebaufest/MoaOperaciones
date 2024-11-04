using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WebApi.OSRM.Common
{
    public class GeoJsonGeometry : Geometry
    {
        /// <summary>
        /// Name of the Geometry. Used in requests.
        /// </summary>
        public const string Name = "geojson";

        /// <summary>
        /// Gets and sets coordinates for response.
        /// </summary>
        [JsonProperty("coordinates")]
        public float[][] Coordinates { get; set; }
    }
}
