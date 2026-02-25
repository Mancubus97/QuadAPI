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
    private async Task<IResult> GetQuestions([FromQuery] int amount)
        => Results.Ok(await service.GetQuestions(amount));

    [HttpPost("checkanswers")]
    private async Task<IResult> CheckAnswers([FromBody] List<AnswerUserRequest> requests)
    => Results.Ok(await service.CheckAnswers(requests));
}