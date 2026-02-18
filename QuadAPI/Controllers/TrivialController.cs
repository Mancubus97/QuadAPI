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
        private readonly HttpClient MyHttpClient;

        public TrivialController(HttpClient httpClient)
        {
            MyHttpClient = httpClient;
        }


        // GET: api/questions?amount=10
        [HttpGet("questions")]
        public IResult GetQuestions([FromQuery] int amount)
        {
            var url = $"https://opentdb.com/api.php?amount={amount}";
            // synchronous call
            var call = MyHttpClient.GetAsync(url).Result; // block until done

            var content = call.Content.ReadAsStringAsync().Result;

            OpentdbResponse opentdbresponse = JsonSerializer.Deserialize<OpentdbResponse>(content);

            if (opentdbresponse == null || opentdbresponse.response_code != 0)
            {
                return Results.BadRequest("Error fetching questions from OpenTDB API.");
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
