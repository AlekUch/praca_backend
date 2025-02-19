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
        private readonly IEmailService _emailService;

        public UserService(AgrochemContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _emailService = emailService;
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

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var confirmationToken = Guid.NewGuid().ToString();
                        var user = new User
                        {
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            Email = userDto.Email,
                            RoleId = 1,
                            EmailConfirmed = false,
                            EmailConfirmationToken = confirmationToken
                        };

                        user.Password = _passwordHasher.HashPassword(user, userDto.Password);


                        _context.Users.Add(user);
                        await _context.SaveChangesAsync();
                        var myVariable = Environment.GetEnvironmentVariable("FRONTEND_URL");
                        var confirmationLink = $"{myVariable}/activate/{confirmationToken}";
                        await _emailService.SendEmailAsync(user.Email, "Potwierdź swój e-mail", $"Kliknij w link, aby aktywować utworzone konto: {confirmationLink}");
                        await transaction.CommitAsync();
                        return "Utworzono konto. Wysłano link aktywacyjny na podany email.";
                    }
                    catch (Exception ex)
                    {
                        // Logowanie błędu
                        Console.WriteLine(ex.Message);
                        await transaction.RollbackAsync();
                        return "Nie można utworzyć konta";
                    }

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

                if (user is null)
                {
                    return "Niepoprawny email lub hasło";
                }

                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, userDto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return "Niepoprawny email lub hasło";
                }

                if (user.EmailConfirmed == false)
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


        public async Task<bool> ActivateAccount(string token)
        {
            try
            {
                var user = _context.Users
                               .Include(u => u.Role)
                               .FirstOrDefault(u => u.EmailConfirmationToken == token);
                if (user is null)
                {
                    return false;
                }

                user.EmailConfirmed = true;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<string> ResetLink(UserDto userDto)
        {
            try
            {
                var user = _context.Users
                               .Include(u => u.Role)
                               .FirstOrDefault(u => u.Email == userDto.Email);
                if (user is null)
                {
                    return "Użytownik nie istnieje";
                }
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var restetToken = Guid.NewGuid().ToString();
                    user.PasswordResetToken = restetToken;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    var myVariable = Environment.GetEnvironmentVariable("FRONTEND_URL");
                    var resetLink = $"{myVariable}/ reset-password/{restetToken}";
                    await _emailService.SendEmailAsync(user.Email, "Zresetuj swoje hasło", $"Kliknij w link, aby utworzyć nowe hasło: {resetLink}");
                    await transaction.CommitAsync();
                    return "Wysłano e-mail z linkiem do zmiany hasła";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await transaction.RollbackAsync();
                    return ex.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        public async Task<bool> PasswordReset(ResetPasswordDTO model)
        {
            try
            {
                var user = _context.Users
                               .Include(u => u.Role)
                               .FirstOrDefault(u => u.PasswordResetToken == model.Token);
                if (user is null)
                {
                    return false;
                }
                string password = _passwordHasher.HashPassword(user, model.NewPassword); ;
                user.Password = password;
                _context.Entry(user).Property(x => x.Password).IsModified = true;
                //_context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<UsersDTO>> GetUsers()
        {
            try
            {
                    var users = await _context.Users
                   .Select(c => new UsersDTO
                   {
                       UserId = c.UserId,
                       FirstName = c.FirstName,
                       LastName = c.LastName,
                       Email = c.Email,
                       EmailConfirmed = c.EmailConfirmed
                   })
                   .ToListAsync();
                    return users;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                throw new ApplicationException("Błąd podczas pobierania działek", ex);
            }
        }

        public async Task<bool> UpdateUser(int id, UsersDTO usersDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false; // Użytkownik nie istnieje
            }

            user.FirstName = usersDto.FirstName;
            user.LastName = usersDto.LastName;
            user.Email = usersDto.Email;
            user.EmailConfirmed = usersDto.EmailConfirmed;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
