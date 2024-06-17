using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Zad11.Context;
using Zad11.Helpers;
using Zad11.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Zad11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IConfiguration _configuration, ContextDB contextDb) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult RegisterUser(RegisterReqModel registerReqModel)
    {
        var hashedPassword = SecurityHelper.GetHashedPasswordAndSalt(registerReqModel.Password);
        var user = new Application()
        {
            Login = registerReqModel.Login,
            Email = registerReqModel.Email,
            Password = hashedPassword.Item1,
            Salt = hashedPassword.Item2,
            RefreshToken = SecurityHelper.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1)
        };
        contextDb.Users.Add(user);
        contextDb.SaveChanges();

        return Ok( $"User with login: {registerReqModel.Login} was added");
    }
    
    [AllowAnonymous]
    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshTokenReqModel refreshTokenReq)
    {
        Application user = contextDb.Users.Where(u => u.RefreshToken == refreshTokenReq.RefreshToken).FirstOrDefault();
        if (user == null)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        if (user.RefreshTokenExp < DateTime.Now)
        {
            throw new SecurityTokenException("Refresh token expired");
        }
        

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken jwtToken = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        user.RefreshToken = SecurityHelper.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        contextDb.SaveChanges();

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            refreshToken = user.RefreshToken
        });
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginReqModel loginReqModel)
    {
        Application user = contextDb.Users.Where(u => u.Login == loginReqModel.Login).FirstOrDefault();
        
        if (user == null)
        {
            return Unauthorized("Wrong username or password");
        }
        
        string passwordHashFromDb = user.Password;
        string curHashedPassword = SecurityHelper.GetHashedPasswordWithSalt(loginReqModel.Password, user.Salt);

        if (passwordHashFromDb != curHashedPassword)
        {
            return Unauthorized("Wrong username or password");
        }

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
        );

        user.RefreshToken = SecurityHelper.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);
        contextDb.SaveChanges();

        return Ok(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = user.RefreshToken
        });
    }
    
    [HttpGet("get")]
    [Authorize]
    public IActionResult Get()
    {
        return Ok("Hello world");
    }
}
