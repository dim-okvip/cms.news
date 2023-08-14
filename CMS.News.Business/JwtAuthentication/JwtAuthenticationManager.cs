using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS.News.Business.JwtAuthentication
{
    public class JwtAuthenticationManager
    {
        private readonly IUserHandler _userHandler;
        private readonly ITokenLoginHandler _tokenLoginHandler;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtAuthenticationManager> _logger;

        public JwtAuthenticationManager(IUserHandler userHandler, ITokenLoginHandler tokenLoginHandler, IConfiguration configuration, ILogger<JwtAuthenticationManager> logger)
        {
            _userHandler = userHandler;
            _tokenLoginHandler = tokenLoginHandler;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<Response<JwtAuthResponse>> AuthenticateAsync(UserLoginRequest request)
        {
            try
            {
                Response<UserLoginResult> loginResponse = await _userHandler.LoginAsync(request);
                Response<JwtAuthResponse> jwtResponse = new(status: loginResponse.Status, message: loginResponse.Message, data: null);
                if (loginResponse.Status is HttpStatusCode.OK)
                {
                    var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtToValidityMins"]));
                    var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                    var tokenKey = Encoding.ASCII.GetBytes(_configuration["JwtSecurityKey"]);
                    var securityTokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new List<Claim> {
                            new Claim(Constants.CLAIM_TYPE, JsonConvert.SerializeObject(loginResponse.Data))
                        }),
                        NotBefore = DateTime.Now,
                        Expires = tokenExpiryTimeStamp,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                    };

                    SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
                    string token = jwtSecurityTokenHandler.WriteToken(securityToken);

                    JwtAuthResponse jwtAuthResponse = new()
                    {
                        Token = token,
                        LogedInUser = loginResponse.Data,
                        Expired = tokenExpiryTimeStamp
                    };
                    jwtResponse.Data = jwtAuthResponse;

                    #region Save userId & token in database
                    await _tokenLoginHandler.CreateAsync(loginResponse.Data.Id, token);
                    #endregion
                }
                return jwtResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new Response<JwtAuthResponse>(status: HttpStatusCode.InternalServerError, message: ex.Message, data: null);
            }
        }


    }
}
