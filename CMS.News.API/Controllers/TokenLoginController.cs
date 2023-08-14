namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class TokenLoginController : ControllerBase
    {
        private readonly ITokenLoginHandler _tokenLoginHandler;

        public TokenLoginController(ITokenLoginHandler tokenLoginHandler)
        {
            _tokenLoginHandler = tokenLoginHandler;
        }

        [HttpGet]
        [Route("tokenLogins")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> GetAll([FromQuery] DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.Database:
                    Response<List<TokenLogin>> responseDb = await _tokenLoginHandler.GetAllFromDatabaseAsync();
                    return Ok(responseDb);
                case DataSource.Memory:
                    Response<List<KeyValuePair<Guid, TokenLogin>>> responseMemory = _tokenLoginHandler.GetAllFromMemory();
                    return Ok(responseMemory);
                default:
                    Response<List<TokenLogin>> responseDbDefault = await _tokenLoginHandler.GetAllFromDatabaseAsync();
                    return Ok(responseDbDefault);
            }
        }
    }
}
