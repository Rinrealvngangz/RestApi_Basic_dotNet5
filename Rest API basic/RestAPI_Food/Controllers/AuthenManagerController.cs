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
using Microsoft.EntityFrameworkCore;
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

        [HttpPost]
        [Route("refresh")]

        public async Task<ActionResult> RefreshToken([FromBody] TokenRefresh tokenRefresh)
        {
            if (ModelState.IsValid)
            {
                var result = await VarifyGenerateToken(tokenRefresh);

                if (result == null)
                {
                    return BadRequest(new ResponseDtos()
                    {
                        Errors = new List<string>() {
                            "Invalid tokens"
                        },
                        Success = false
                    });
                }

                return Ok(result);
            }

            return BadRequest(new ResponseDtos()
            {
                Errors = new List<string>() {
                    "Invalid payload"
                },
                Success = false
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

        private async Task<ResponseDtos> VarifyGenerateToken([FromBody]TokenRefresh tokenRefresh)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // Validatiton 1 JWT Format
                var tokenValidation = jwtTokenHandler.ValidateToken(tokenRefresh.Token, _tokenValidationParameters, out var validatedToken);

                // Validation 2 Validate encrytion alg
                if(validatedToken is  JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                // Validation 3 expiry date
                var utcExpiryDate = long.Parse(tokenValidation.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var verifyDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (verifyDate > DateTime.UtcNow)
                {
                    return new ResponseDtos()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }
                // Validation 4 validate exist token 
                var storedToken = await _foodDBContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRefresh.RefreshToken);
                if(storedToken == null)
                {

                    return new ResponseDtos()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }

                // Validation 5 - validate if used
                if (storedToken.IsUsed)
                {
                    return new ResponseDtos()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevorked)
                {
                    return new ResponseDtos()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }

                // Validation 7 - validate the id
                var jti = tokenValidation.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return new ResponseDtos()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }

                // update current token 

                storedToken.IsUsed = true;
                _foodDBContext.RefreshTokens.Update(storedToken);
                await _foodDBContext.SaveChangesAsync();
                // Generate a new token
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch ( Exception er)
            {
                throw er;
            }
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
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