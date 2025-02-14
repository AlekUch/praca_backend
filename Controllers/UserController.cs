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
                string result = await _userService.RegisterUser(userdto);

                if (result.Contains("Utworzono konto"))
                {
                    return Ok(new { message = result });
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

        [Route("activate-account/{token}")]
        [HttpPost]
        public async Task<IActionResult> ActivateAccount([FromRoute] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token jest wymagany." });
            }
            bool result = await _userService.ActivateAccount(token);
            if (result == false)
            {
                return BadRequest(new { message = "Błąd aktywacji konta" });
            }
            else
            {
                return Ok();
            }
        }

        [Route("reset-link")]
        [HttpPost]
        public async Task<IActionResult> ResetLink(UserDto userDto)
        {

            string result = await _userService.ResetLink(userDto);
            if (result.Contains("Wysłano"))
            {
                return Ok(new { message = result });
            }
            else
            {
                return BadRequest(new { message = result });
            }
        }


        [Route("reset-password")]
        [HttpPost]
        public async Task<IActionResult> PasswordReset([FromBody] ResetPasswordDTO model)
        {

            bool result = await _userService.PasswordReset(model);
            if (result == false)
            {
                return BadRequest(new { message = "Błąd zmiany hasła" });
            }
            else
            {
                return Ok(new {message = "Zmieniono hasło pomyślnie."});
            }
        }

        [Route("users")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {          
                try
                {
                    var users = await _userService.GetUsers();
                    Console.WriteLine(users);
                    return Ok(users);
                }
                catch (ApplicationException ex)
                {
                    return StatusCode(500, new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Wystąpił błąd podczas pobierania użytkowników." });
                }
            
        }

        [Route("user/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UsersDTO usersDto)
        {
            try
            {
                bool isUpdated = await _userService.UpdateUser(id, usersDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Nie można edytować tego użytkownika." });
                }

                return Ok(new { message = "Edytowano pomyślnie" });

            }
            catch (ApplicationException ex)
            {
                // Złap ApplicationException wyrzucony z serwisu
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

