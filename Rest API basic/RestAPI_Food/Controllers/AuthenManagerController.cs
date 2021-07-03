using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RestAPI_Food.configuration;
using RestAPI_Food.Dtos;
using RestAPI_Food.Etities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using RestAPI_Food.Data;

namespace RestAPI_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenManagerController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly FoodDBContext _foodDBContext;
        public AuthenManagerController(UserManager<IdentityUser> userManager,
                                      IOptionsMonitor<JwtConfig> optionsMonitor,
                                      TokenValidationParameters tokenValidationParameters,
                                      FoodDBContext foodDBContext )
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _tokenValidationParameters = tokenValidationParameters;
            _foodDBContext = foodDBContext;
        }

        [HttpPost]
        [Route("Register")]

        public async Task<ActionResult> Register([FromBody]Register register)
        {
            if (ModelState.IsValid)
            {
                var ExistEmail = await _userManager.FindByEmailAsync(register.Email);


                if (ExistEmail != null)
                {
                    return BadRequest(new ResponseDtos()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Email already use!"
                        }
                    });
                }
                var newUser = new IdentityUser()
                {
                    UserName = register.Username,
                    Email = register.Email,
                };

                var isCreated = await _userManager.CreateAsync(newUser, register.Password);
                if (isCreated.Succeeded)
                {
                   var jwtToken = await GenerateJwtToken(newUser);
                    return Ok(jwtToken);
                }
            }
            return BadRequest(new ResponseDtos()
            {
                Errors = new List<string>() {
                        "Invalid payload"
                    },
                Success = false
            });
        }

        [HttpPost]
        [Route("Login")]
        public  async Task<ActionResult> Login([FromBody]Login login)
        {
            if (ModelState.IsValid)
            {
              var hasUser =  await  _userManager.FindByEmailAsync(login.Email);
                if(hasUser != null)
                {
                    var isUser = await _userManager.CheckPasswordAsync(hasUser, login.Password);

                    if (isUser)
                    {
                        var token = await GenerateJwtToken(hasUser);
                        return Ok(token);
                    }
                    return BadRequest(new ResponseDtos()
                    {

                        Success = false,
                        Errors = new List<string>()
                {
                    "Password not correct"
                }

                    });

                }

                return BadRequest(new ResponseDtos()
                {

                    Success = false,
                    Errors = new List<string>()
                {
                    "Email is not exist"
                }

                });


            }
            return BadRequest(new ResponseDtos()
            {

                Success = false,
                Errors = new List<string>()
                {
                    "Invalid payload"
                }

            });

        }
        private async Task<ResponseDtos> GenerateJwtToken(IdentityUser user)
        {
            var jwtSecurityToken =  new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);
            var claim = new ClaimsIdentity(new[]
            {
                new Claim("Id" , user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString())
            });
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claim,
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key) , SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtSecurityToken.CreateToken(tokenDescriptor);
            var jwtToken = jwtSecurityToken.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };
            await _foodDBContext.RefreshTokens.AddAsync(refreshToken);
            await _foodDBContext.SaveChangesAsync();
            return new ResponseDtos() {
                Success = true,
                Token = jwtToken,
                RefreshToken = refreshToken.Token
            };
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
    }
     
}