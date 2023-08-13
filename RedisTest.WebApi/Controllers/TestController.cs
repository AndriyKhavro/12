using Microsoft.AspNetCore.Mvc;
using RedisTest.Library;

namespace RedisTest.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly RedisClient _redisClient;
        private readonly HttpClient _httpClient;

        public TestController(RedisClient redisClient, HttpClient httpClient)
        {
            _redisClient = redisClient;
            _httpClient = httpClient;
        }

        [HttpGet("probabilistic")]
        public async Task<string> GetProbabilistic()
        {
            var result = await _redisClient.ProbabilisticGet(
                Urls.Censys_1Mb,
                () => _httpClient.GetStringAsync(Urls.Censys_1Mb),
                TimeSpan.FromSeconds(30));

            return result[..100];
        }

        [HttpGet]
        public async Task<string> GetNormal()
        {
            var result = await _redisClient.Get(
                Urls.Censys_1Mb,
                () => _httpClient.GetStringAsync(Urls.Censys_1Mb),
                TimeSpan.FromSeconds(30));

            return result[..100];
        }
    }
}