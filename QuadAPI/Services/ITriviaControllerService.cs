using QuadAPI.Models;

namespace QuadAPI.Services
{
    public interface ITriviaControllerService
    {

        Task<IEnumerable<QuestionsResponse>> GetQuestions(int amount);

        Task<IEnumerable<AnswerUserResponse>> CheckAnswers(List<AnswerUserRequest> requests);
    }
}
