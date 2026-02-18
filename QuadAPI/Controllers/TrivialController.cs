using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using QuadAPI.Models;

using System.Net.Http;


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
            var response = MyHttpClient.GetAsync(url).Result; // block until done

            var content = response.Content.ReadAsStringAsync().Result;

            return Results.Ok(content);
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
