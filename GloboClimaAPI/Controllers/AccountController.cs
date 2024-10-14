namespace GloboClimaAPI.Controllers
{

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Register(
            // [FromBody] Account account
        )
        {
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(
            // [FromBody] EditAccountViewModel account
        )
        {
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }
    }
}