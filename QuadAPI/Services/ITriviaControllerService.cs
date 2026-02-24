using QuadAPI.Models;

namespace QuadAPI.Services
{
    public interface ITriviaControllerService
    {
        Task<OpentdbResponse> GetResponseFromOpenTDB(int amount);

        Task<List<QuestionsResponse>> GenerateResponse(List<OpentdbResult> results);

        Task<IEnumerable<QuestionsResponse>> GetQuestions(int amount);

        Task<IEnumerable<AnswerUserResponse>> CheckAnswers(List<AnswerUserRequest> requests);
    }
}
