using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using AGROCHEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

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
        public async Task<IActionResult> RegisterAsync([FromForm] UserDto userdto)
        {
            if (ModelState.IsValid)
            {
                string result = await _userService.RegisterUser(userdto);
                if (result == "Użytkownik został dodany.")
                {
                    return Ok(new {message = result});
                }
                else
                {
                    return BadRequest(new { message = result });
                }              
            }
            return BadRequest(new { message = "Wystąpił błąd w rejestracji." });
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserDto userdto)
        {
            if (ModelState.IsValid)
            {
                string result = await _userService.Login(userdto);
                if (result == "Niepoprawny email lub hasło")
                {
                    return BadRequest(new { message = result });
                }
                else
                {
                    
                    return Ok(new { token = result, user = userdto.Email });
                }
            }
            return BadRequest(new { message = "Wystąpił błąd w rejestracji." });
        }

      

       

        
    }

}

