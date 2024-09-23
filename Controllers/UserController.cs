using AGROCHEM.Models;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Mvc;

namespace AGROCHEM.Controllers
{
    [Route("agrochem")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromForm] User user)
        {
            if (ModelState.IsValid)
            {
                string result = await _userService.RegisterUser(user);
                if (result== "Użytkownik został dodany.")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }              
            }
            return BadRequest(new { message = "Wystąpił błąd w rejestracji." });
        }

    }

}

