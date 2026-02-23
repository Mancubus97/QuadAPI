using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using QuadAPI.Models;

using System.Text.Json;

namespace QuadAPI.Controllers;

[Route("api")]
[ApiController]
public class TrivialController : Controller
{
    private readonly HttpClient httpClient;

    static private readonly Dictionary<string, string> correctAnswers = [];

    public TrivialController(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }


    // GET: api/questions?amount=10
    [HttpGet("questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetQuestions([FromQuery] int amount)
    {
        if (amount <= 0)
            return Results.BadRequest("Amount must be greater than 0");

        var url = $"https://opentdb.com/api.php?amount={amount}";

        var call = await httpClient.GetAsync(url);

        if (!call.IsSuccessStatusCode)
        {
            return Results.BadRequest("Error fetching questions from OpenTDB API.");
        }

        var content = await call.Content.ReadAsStringAsync();

        if (content == null)
        {
            return Results.BadRequest("Error reading response content from OpenTDB API.");
        }

        OpentdbResponse? opentdbresponse = JsonSerializer.Deserialize<OpentdbResponse>(content);

        if (opentdbresponse == null)
        {
            return Results.BadRequest("Error deserializing OpenTDB response.");
        }

        if (opentdbresponse.ResponseCode != 0)
        {
            return Results.BadRequest("OpenTDB responded with response_code: " + opentdbresponse.ResponseCode);
        }

        if (opentdbresponse.Results == null || opentdbresponse.Results.Count == 0)
        {
            return Results.BadRequest("OpenTDB response does not contain results.");
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

        //DEBUG
        //foreach (string answer in correctAnswers.Values)
        //{
        //    Console.WriteLine(answer);
        //}

        return Results.Ok(questions_response);
    }

    [HttpPost("checkanswers")]
    public async Task<IResult> CheckAnswers([FromBody] List<AnswerUserRequest> requests)
    {
        List<AnswerUserResponse> res = [];
        //var correct_answers = requests.Select(request => correctAnswers[request.Question] == request.Answer);

        var results = requests.Select(request =>
        {
            var isCorrect = correctAnswers.TryGetValue(request.Question, out var answer) && answer.Equals(request.Answer);
            return new AnswerUserResponse(request.Question, isCorrect);
        });
        //foreach(AnswerUserRequest answer in answers)
        //{
        //    foreach (var correctAnswer in correctAnswers)
        //    {
        //        if (correctAnswer.Key == answer.Question)
        //        {
        //            if (correctAnswer.Value == answer.Answer)
        //            {
        //                Console.WriteLine("Correct!");
        //            }
        //            else
        //            {
        //                Console.WriteLine("Incorrect!");
        //            }
        //        }
        //    }
        //    Console.WriteLine(answer.Question);
        //    Console.WriteLine(answer.Answer);
        //}

        return Results.Ok(results);
    }
}

