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
        List<QuestionsResponse> questions_response = new List<QuestionsResponse>(); // Naam?

        foreach (OpentdbResult result in results)
        {
            correctAnswers.Add(result.Question, result.CorrectAnswer);
            questions_response.Add(new QuestionsResponse
            {
                Type = result.Type,
                Difficulty = result.Difficulty,
                Category = result.Category,
                Question = result.Question
            });
        }


        return Results.Ok(questions_response);
    }

    [HttpPost("checkanswers")]
    public async Task<IResult> CheckAnswers([FromBody] List<AnswerUser> answers)
    {
        foreach(AnswerUser answer in answers)
        {
            Console.WriteLine(answer.Question);
            Console.WriteLine(answer.Answer);
        }
        return Results.Ok(correctAnswers);
    }
}

