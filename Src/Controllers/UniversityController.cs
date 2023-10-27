using Microsoft.AspNetCore.Mvc;
using MyIgniteApi.Services;
using MyIgniteApi.Requests;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace MyIgniteApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UniversityController : ControllerBase
    {
        private readonly UniversityService _service;

        public UniversityController(UniversityService service)
        {
            _service = service;
        }

        [HttpGet]
        // for testing purposes
        public IActionResult Root()
        {
            return Ok("API is running");
        }

        [HttpPost("parallel")]
        public async Task<IActionResult> GetUniversitiesInParallel([FromBody] UniversityRequest request)
        {
            Console.WriteLine(JsonSerializer.Serialize(request));
            var requestCountries = request.Countries ?? new List<string>();
            var data = await _service.FetchAndStoreUniversitiesParallel(requestCountries);
            return Ok(data);
        }
    }
}
