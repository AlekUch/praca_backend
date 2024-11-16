using AGROCHEM.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AGROCHEM.Models.Entities;
using AGROCHEM.Models.EntitiesDto;
using static System.Reflection.Metadata.BlobBuilder;

namespace AGROCHEM.Services
{

    public class UserService 
    {
        private readonly AgrochemContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UserService(AgrochemContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;

        }

        public async Task<string> RegisterUser(UserDto userDto)
        {
            try
            {
                if (userDto != null)
                {
                    var userExists = await _context.Users
                        .AnyAsync(u => u.Email == userDto.Email);

                    if (userExists)
                    {
                        return "Użytkownik o podanym adresie email posiada już  konto.";
                    }

                    var user = new User
                    {
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        Email = userDto.Email,
                        RoleId = 1
                    };

                    user.Password = _passwordHasher.HashPassword(user, userDto.Password);
                    

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return "Użytkownik został dodany.";

                }
                else
                {
                    return "Nie podano danych do rejestracji";
                }
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }


        public async Task<string> Login(UserDto userDto)
        {
            try
            {
                var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == userDto.Email);
                Console.WriteLine(user.Role.Name);
                if (user is null)
                {
                    return "Niepoprawny email lub hasło";
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return "Niepoprawny email lub hasło";
                }

                var claims = new List<Claim>()
                {
               
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                     new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                };


                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(Convert.ToInt32(jwtSettings["JwtExpireDays"]));
                var token = new JwtSecurityToken(jwtSettings["Issuer"],
                     jwtSettings["Issuer"],
                    claims,
                    expires: expires,
                    signingCredentials: cred
                    );

                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(token) ;
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }


      

       

       

       
    }
}
