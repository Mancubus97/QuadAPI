using FluentAssertions;
using Moq;
using Moq.Protected;
using QuadAPI.Models;
using QuadAPI.Services;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;

public class TriviaControllerServiceTests
{
    private HttpClient CreateMockHttpClient(string responseContent, HttpStatusCode statusCode)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
           .Protected()
           .Setup<Task<HttpResponseMessage>>(
               "SendAsync",
               ItExpr.IsAny<HttpRequestMessage>(),
               ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage
           {
               StatusCode = statusCode,
               Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
           });

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task GetResponseFromOpenTDB_ShouldReturnResponse_WhenApiCallIsSuccessful()
    {
        var fakeResponse = new OpentdbResponse
        {
            ResponseCode = 0,
            Results = new List<OpentdbResult>
            {
                new OpentdbResult
                {
                    Question = "Test Question",
                    CorrectAnswer = "Correct",
                    IncorrectAnswers = new List<string> { "Wrong1", "Wrong2" },
                    Category = "General",
                    Difficulty = "easy",
                    Type = "multiple"
                }
            }
        };

        var json = JsonSerializer.Serialize(fakeResponse);
        var httpClient = CreateMockHttpClient(json, HttpStatusCode.OK);
        var service = new TriviaControllerService(httpClient);

        var result = await service.GetResponseFromOpenTDB(1);

        result.Should().NotBeNull();
        result.ResponseCode.Should().Be(0);
        result.Results.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetResponseFromOpenTDB_ShouldThrow_WhenAmountIsZero()
    {
        var httpClient = CreateMockHttpClient("", HttpStatusCode.OK);
        var service = new TriviaControllerService(httpClient);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetResponseFromOpenTDB(0));
    }

    [Fact]
    public async Task GetQuestions_ShouldThrow_WhenResponseCodeIsNotZero()
    {
        var fakeResponse = new OpentdbResponse
        {
            ResponseCode = 1,
            Results = new List<OpentdbResult>()
        };

        var json = JsonSerializer.Serialize(fakeResponse);
        var httpClient = CreateMockHttpClient(json, HttpStatusCode.OK);
        var service = new TriviaControllerService(httpClient);

        await Assert.ThrowsAsync<Exception>(() =>
            service.GetQuestions(1));
    }

    [Fact]
    public async Task GenerateResponse_ShouldIncludeAllAnswers()
    {
        var service = new TriviaControllerService(new HttpClient());

        var results = new List<OpentdbResult>
        {
            new OpentdbResult
            {
                Question = "Q1",
                CorrectAnswer = "Correct",
                IncorrectAnswers = new List<string> { "Wrong1", "Wrong2" },
                Category = "General",
                Difficulty = "easy",
                Type = "multiple"
            }
        };

        var response = await service.GenerateResponse(results);

        response.Should().HaveCount(1);
        response.First().Answers.Should().HaveCount(3);
        response.First().Answers.Should().Contain("Correct");
        response.First().Answers.Should().Contain("Wrong1");
        response.First().Answers.Should().Contain("Wrong2");
    }

    [Fact]
    public async Task CheckAnswers_ShouldReturnTrue_WhenAnswerIsCorrect()
    {
        var service = new TriviaControllerService(new HttpClient());

        var results = new List<OpentdbResult>
        {
            new OpentdbResult
            {
                Question = "Q1",
                CorrectAnswer = "Correct",
                IncorrectAnswers = new List<string> { "Wrong1" },
                Category = "General",
                Difficulty = "easy",
                Type = "multiple"
            }
        };

        await service.GenerateResponse(results);

        var requests = new List<AnswerUserRequest>
        {
            new AnswerUserRequest
            {
                Question = "Q1",
                Answer = "Correct"
            }
        };

        var response = await service.CheckAnswers(requests);

        response.First().IsCorrect.Should().BeTrue();
    }
}