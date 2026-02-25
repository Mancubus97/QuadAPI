namespace QuadAPI.Models
{
    public class CheckAnswersRequest
    {
        public string QuizId { get; set; } = string.Empty;

        public List<UserAnswer> Answers { get; set; } = new();
    }
}