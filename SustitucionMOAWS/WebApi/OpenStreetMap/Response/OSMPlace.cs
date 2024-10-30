using Newtonsoft.Json;
using System.Collections.Generic;

namespace SustitucionMOAWS.WebApi.OpenStreetMap.Response
{
    public class OSMPlace
    {
        [JsonProperty("place_id")]
        public long PlaceId { get; set; }

        [JsonProperty("licence")]
        public string Licence { get; set; }

        [JsonProperty("osm_type")]
        public string OsmType { get; set; }

        [JsonProperty("osm_id")]
        public long OsmId { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("place_rank")]
        public int PlaceRank { get; set; }

        [JsonProperty("importance")]
        public double Importance { get; set; }

        [JsonProperty("addresstype")]
        public string Addresstype { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("boundingbox")]
        public List<double> Boundingbox { get; set; } = new List<double>();
    }
}
