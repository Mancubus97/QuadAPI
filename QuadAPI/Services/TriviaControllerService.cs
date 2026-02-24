using Microsoft.AspNetCore.Mvc;
using QuadAPI.Models;
using System.Text.Json;

namespace QuadAPI.Services
{
    public class TriviaControllerService(HttpClient httpClient) : ITriviaControllerService
    {


        private readonly HttpClient httpClient = httpClient;

        static private readonly Dictionary<string, string> correctAnswers = [];

        public async Task<IEnumerable<QuestionsResponse>> GetQuestions(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than 0");

            var url = $"https://opentdb.com/api.php?amount={amount}";

            var call = await httpClient.GetAsync(url);

            if (!call.IsSuccessStatusCode)
            {
                throw new Exception("Error fetching questions from OpenTDB API.");
            }

            var content = await call.Content.ReadAsStringAsync();

            if (content == null)
            {
                throw new Exception("Error reading response content from OpenTDB API.");
            }

            OpentdbResponse? opentdbresponse = JsonSerializer.Deserialize<OpentdbResponse>(content);

            if (opentdbresponse == null)
            {
                throw new Exception("Error deserializing OpenTDB response.");
            }

            if (opentdbresponse.ResponseCode != 0)
            {
                throw new Exception("OpenTDB responded with response_code: " + opentdbresponse.ResponseCode);
            }

            if (opentdbresponse.Results == null || opentdbresponse.Results.Count == 0)
            {
                throw new Exception("OpenTDB response does not contain results.");
            }

            List<OpentdbResult> results = opentdbresponse.Results;
            List<QuestionsResponse> questions_response = new List<QuestionsResponse>();

            foreach (OpentdbResult result in results)
            {
                List<string> answers = new List<string>();
                answers.Add(result.CorrectAnswer);
                answers.AddRange(result.IncorrectAnswers);
                // Shuffle the answers
                List<string> answersShuffled = new List<string>();
                Random random = new Random();
                while (answers.Count > 0)
                {
                    int index = random.Next(answers.Count);
                    answersShuffled.Add(answers[index]);
                    answers.RemoveAt(index);
                }

                correctAnswers.Add(result.Question, result.CorrectAnswer);
                questions_response.Add(new QuestionsResponse
                {
                    Type = result.Type,
                    Difficulty = result.Difficulty,
                    Category = result.Category,
                    Question = result.Question,
                    Answers = answersShuffled
                });
            }

            return questions_response;
        }

        public async Task<IEnumerable<AnswerUserResponse>> CheckAnswers(List<AnswerUserRequest> requests)
        {
            List<AnswerUserResponse> res = [];

            var results = requests.Select(request =>
            {
                var isCorrect = correctAnswers.TryGetValue(request.Question, out var answer) && answer.Equals(request.Answer);
                return new AnswerUserResponse(request.Question, isCorrect);
            });

            return results;
        }
    }
}
