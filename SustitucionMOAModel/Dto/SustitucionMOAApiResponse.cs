using Newtonsoft.Json;

namespace SustitucionMOAModel.Dto
{
    public class SustitucionMOAApiResponse<T> : SustitucionMOAApiResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        //[JsonProperty("info")]
        //public string Info { get; set; }

        //[JsonProperty("error")]
        //public string Error { get; set; }

        //[JsonProperty("logout")]
        //public bool Logout { get; set; }
    }

    public class SustitucionMOAApiResponse
    {
        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("logout")]
        public bool Logout { get; set; }
    }
}
