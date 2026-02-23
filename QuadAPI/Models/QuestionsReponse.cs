namespace QuadAPI.Models;

public class QuestionsResponse
{
    public string Type { get; set; }
    public string Difficulty { get; set; }
    public string Category { get; set; }
    public string Question { get; set; }

    public List<string> Answers { get; set; }
}
