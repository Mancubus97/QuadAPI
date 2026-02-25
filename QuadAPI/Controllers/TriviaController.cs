using Microsoft.AspNetCore.Mvc;

using QuadAPI.Models;

using QuadAPI.Services;

namespace QuadAPI.Controllers;

[Route("api")]
[ApiController]
public class TriviaController(ITriviaControllerService service) : Controller
{
    // GET: api/questions?amount=10
    [HttpGet("questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetQuestions([FromQuery] int amount)
        => Results.Ok(await service.GetQuestions(amount));

    [HttpPost("checkanswers")]
    public async Task<IResult> CheckAnswers([FromBody] CheckAnswersRequest request)
        => Results.Ok(await service.CheckAnswers(request.QuizId, request.Answers));

}