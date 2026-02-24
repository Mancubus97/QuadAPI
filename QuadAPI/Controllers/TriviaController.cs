using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using QuadAPI.Models;

using System.Text.Json;

using QuadAPI.Services;

namespace QuadAPI.Controllers;

[Route("api")]
[ApiController]
public class TriviaController(ITriviaControllerService service) : Controller
{

    private readonly ITriviaControllerService _service = service;


    // GET: api/questions?amount=10
    [HttpGet("questions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IResult> GetQuestions([FromQuery] int amount)
        => Results.Ok(await _service.GetQuestions(amount));

    [HttpPost("checkanswers")]
    public async Task<IResult> CheckAnswers([FromBody] List<AnswerUserRequest> requests)
    => Results.Ok(await _service.CheckAnswers(requests));
}

