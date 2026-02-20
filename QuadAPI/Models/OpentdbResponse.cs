using System.Text.Json.Serialization;

namespace QuadAPI.Models;

public class OpentdbResponse
{

    [JsonPropertyName("response_code")]
    public int ResponseCode { get; set; }

    [JsonPropertyName("results")]
    public List<OpentdbResult>? Results { get; set; }
}
