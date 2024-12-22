using Dashboard.DAL.ViewModels.Apis;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApisController : BaseController
    {
        [HttpGet("weather")]
        public async Task<IActionResult> WeatherAsync(string? latitude, string? longitude)
        {
            string url = $"https://api.weatherapi.com/v1/current.json?key=9c8ba75b0a7544bf87d173654242904&q={latitude},{longitude}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetFromJsonAsync<WeatherApiVM>(url);

            if(response != null)
            {
                if (response.location == null || response.current == null)
                {
                    return BadRequest("error");
                }

                var result = new WeatherVM
                {
                    City = response.location.name ?? "",
                    Temp = response.current.temp_c,
                    Wind = response.current.wind_kph
                };

                return Ok(result);
            }


            return BadRequest("error");
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Test method");
        }
    }
}
