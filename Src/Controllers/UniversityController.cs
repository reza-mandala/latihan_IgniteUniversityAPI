using Microsoft.AspNetCore.Mvc;
using MyIgniteApi.Models;
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
        private readonly UniversityService _universityService;

        public UniversityController(UniversityService universityService)
        {
            _universityService = universityService;
        }

        [HttpGet]
        // for testing purposes
        public IActionResult Root()
        {
            return Ok("API is running");
        }

        [HttpPost]
        public IActionResult GetUniversities([FromBody] UniversityRequest request)
        {
            var data = new Dictionary<string, List<University>>();
            var requestCountries = request.Countries ?? new List<string>();
            if (requestCountries != null && requestCountries.Count() > 0)
            {
                foreach (var country in requestCountries)
                {
                    data[country] = _universityService.FetchAndStoreUniversitiesForCountry(country).Result;
                }
            }
            return Ok(data);
        }

        [HttpPost("async")]
        public IActionResult GetUniversitiesAsync([FromBody] UniversityRequest request)
        {
            var requestCountries = request.Countries ?? new List<string>();
            var taskId = TaskService.CreateNewTask(async () => await _universityService.FetchAndStoreUniversitiesParallel(requestCountries));
            return Ok(new { task_id = taskId });
        }

        [HttpGet("task/{taskId}")]
        public IActionResult GetTaskStatus(int taskId)
        {
            var task = TaskService.GetTask(taskId);
            if (task == null)
                return NotFound();

            return Ok(new
            {
                task_id = taskId,
                task_status = task.Status,
                task_result = task.IsCompletedSuccessfully ? task.Result : null
            });
        }

        [HttpPost("parallel")]
        public async Task<IActionResult> GetUniversitiesInParallel([FromBody] UniversityRequest request)
        {
            var requestCountries = request.Countries ?? new List<string>();
            var data = await _universityService.FetchAndStoreUniversitiesParallel(requestCountries);
            return Ok(data);
        }
    }
}
