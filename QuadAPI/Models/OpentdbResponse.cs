namespace QuadAPI.Models
{
    public class OpentdbResponse
    {
        public int response_code { get; set; }
        public List<OpentdbResult>? results { get; set; }
    }
}
