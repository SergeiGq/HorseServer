using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClassLibrary;
using ClassLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication2.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : Controller
{
    private readonly DbHorse _context;
    public AuthController(DbHorse context)
    {
        _context = context;
    }
    [HttpPost("/token")]
    public IActionResult Token(string username, string password)
    {
        var identity = GetIdentity(username, password);
        if (identity == null)
        {
            return BadRequest(new { errorText = "Invalid username or password." });
        }
 
        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
 
        var response = new
        {
            access_token = encodedJwt,
            username = identity.Name
        };
        _context.SaveChanges();
        return Json(response.access_token);
    }
 
    private ClaimsIdentity GetIdentity(string username, string password)
    {
        Auth person = _context.Auths.FirstOrDefault(x => x.Login == username && x.Password == password);
        if (person != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
               
            };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
 
        // если пользователя не найдено
        return null;
        
    }
    

    [HttpPost("/registr")]
    public Auth Registr(string login,string password)
    {
        foreach (var item in _context.Auths)
        {
            if (item.Login==login&&item.Password==password)
            {
                return null;
            }
        }

        var auth = new Auth()
        {
            Login = login,
            Password = password

        };
        _context.Auths.Add(auth);
        _context.SaveChanges();
        return auth;
    }

    [HttpGet]
    public IEnumerable<Auth> Get()
    {
        return _context.Auths;
    }
}