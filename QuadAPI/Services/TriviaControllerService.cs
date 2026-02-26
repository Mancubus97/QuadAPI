using Microsoft.Extensions.Caching.Memory;
using QuadAPI.Models;
using System.Net;
using System.Text.Json;

namespace QuadAPI.Services
{
    public class TriviaControllerService(
        HttpClient httpClient,
        IMemoryCache cache) : ITriviaControllerService
    {
        public async Task<OpentdbResponse> GetResponseFromOpenTDB(int amount)
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

            return opentdbresponse;
        }

        public async Task<List<QuestionsResponse>> GenerateResponse(List<OpentdbResult> results)
        {
            var quizId = Guid.NewGuid().ToString();
            var answerDictionary = new Dictionary<string, string>();

            List<QuestionsResponse> res = new List<QuestionsResponse>();

            foreach (OpentdbResult result in results)
            {
                answerDictionary[result.Question] = result.CorrectAnswer;

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

                res.Add(new QuestionsResponse
                {
                    QuizId = quizId,
                    Question = WebUtility.HtmlDecode(result.Question),
                    Category = result.Category,
                    Difficulty = result.Difficulty,
                    Type = result.Type,
                    Answers = answersShuffled
                });
            }

            cache.Set(quizId, answerDictionary, TimeSpan.FromMinutes(60));

            return res;
        }

        public async Task<IEnumerable<QuestionsResponse>> GetQuestions(int amount)
        {

            OpentdbResponse opentdbResponse = await GetResponseFromOpenTDB(amount);

            if (opentdbResponse.ResponseCode != 0)
            {
                throw new Exception("OpenTDB responded with response_code: " + opentdbResponse.ResponseCode);
            }

            if (opentdbResponse.Results == null || opentdbResponse.Results.Count == 0)
            {
                throw new Exception("OpenTDB response does not contain results.");
            }

            List<QuestionsResponse> questionsResponse = await GenerateResponse(opentdbResponse.Results);

            return questionsResponse;
        }

        public async Task<IEnumerable<AnswerUserResponse>> CheckAnswers(
            string quizId,
            List<UserAnswer> requests)
        {
            if (!cache.TryGetValue(quizId, out Dictionary<string, string>? answers) || answers == null)
                throw new Exception("Quiz expired or invalid.");


            var results = requests.Select(r =>
            {
                var isCorrect = answers.TryGetValue(r.Question, out var correct)
                                && correct == r.Answer;

                return new AnswerUserResponse(r.Question, isCorrect);
            });

            return results;
        }
    }
}
