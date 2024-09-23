using AGROCHEM.Data;
using AGROCHEM.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AGROCHEM.Services
{
    public class UserService 
    {
        private readonly AgrochemContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(AgrochemContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;

        }

        public async Task<string> RegisterUser(User user)
        {
            try
            {
                if (user != null)
                {
                    var userExists = await _context.Users
                        .AnyAsync(u => u.Email == user.Email);

                    if (userExists)
                    {
                        return "Użytkownik o podanym adresie email posiada już  konto.";
                    }

                    var passwordHash = _passwordHasher.HashPassword(user, user.Password);
                    var userNew = new User
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Password = passwordHash,
                        RoleId = 1
                    };

                    _context.Users.Add(userNew);
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
    }
}
