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
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IEmailService _emailService;
        public UserController(UserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromForm] UserDto userdto)
        {
            if (ModelState.IsValid)
            {
                var confirmationToken = Guid.NewGuid().ToString();

                string result = await _userService.RegisterUser(userdto, confirmationToken);

                if (result == "Użytkownik został dodany.")
                {
                    var confirmationLink = $"https://agrochem/confirm-email?token={confirmationToken}";
                    await _emailService.SendEmailAsync(userdto.Email, "Potwierdź swój e-mail", $"Kliknij w link, aby potwierdzić: {confirmationLink}");
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

