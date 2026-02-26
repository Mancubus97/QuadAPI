namespace QuadAPI.Models;

public class QuestionsResponse
{

    public required string QuizId { get; set; }
    public string? Type { get; set; }
    public string? Difficulty { get; set; }
    public string? Category { get; set; }
    public required string Question { get; set; }
    public required List<string> Answers { get; set; }
}
