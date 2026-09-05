using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiMain.Controllers
{
    [Route("test")]
    [ApiController]

    public class AzureTestController : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> TestAzureFunction()
        {
            return await Task.Run(() =>
            {
                return Ok("Hello From Azure Function");
            });
        } // TestAzureFunction...
    } // class...
}
