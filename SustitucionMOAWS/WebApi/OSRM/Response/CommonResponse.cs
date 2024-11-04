using Newtonsoft.Json;

namespace SustitucionMOAWS.WebApi.OSRM.Response
{
    public abstract class CommonResponse
    {
        [JsonProperty("code")]
        public ResponseCode Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public bool IsValid => Code == ResponseCode.Ok;
    }
}
