using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using QuadAPI.Models;

using System.Net.Http;

using System.Collections.Generic;

using System.Text.Json;


namespace QuadAPI.Controllers
{


    [Route("api")]
    [ApiController]
    public class TrivialController : Controller
    {
        private readonly HttpClient httpclient;

        public TrivialController(HttpClient httpClient)
        {
            this.httpclient = httpClient;
        }


        // GET: api/questions?amount=10
        [HttpGet("questions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult GetQuestions([FromQuery] int amount)
        {
            if (amount <= 0)
                return Results.BadRequest("Amount must be greater than 0");

            var url = $"https://opentdb.com/api.php?amount={amount}";

            var call = httpclient.GetAsync(url).Result;

            if (!call.IsSuccessStatusCode)
            {
                return Results.BadRequest("Error fetching questions from OpenTDB API.");
            }

            var content = call.Content.ReadAsStringAsync().Result;

            if (content == null)
            {
                return Results.BadRequest("Error reading response content from OpenTDB API.");
            }

            OpentdbResponse? opentdbresponse = JsonSerializer.Deserialize<OpentdbResponse>(content);

            if (opentdbresponse == null)
            {
                return Results.BadRequest("Error deserializing OpenTDB response.");
            }

            if (opentdbresponse.response_code != 0)
            {
                return Results.BadRequest("OpenTDB returned OK200-299 status and response_code: " + opentdbresponse.response_code);
            }

            if (opentdbresponse.results == null || opentdbresponse.results.Count == 0)
            {
                return Results.BadRequest("OpenTDB response does not contain results.");
            }

            List<OpentdbResult> results = opentdbresponse.results;
            List<QuestionsResponse> questions_response = new List<QuestionsResponse>();

            foreach (OpentdbResult result in results)
            {
                questions_response.Add(new QuestionsResponse
                {
                    type = result.type,
                    difficulty = result.difficulty,
                    category = result.category,
                    question = result.question
                });
            }


            return Results.Ok(questions_response);
        }

      
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
