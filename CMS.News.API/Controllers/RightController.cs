namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RightController : ControllerBase
    {
        private readonly IRightHandler _rightHandler;

        public RightController(IRightHandler rightHandler)
        {
            _rightHandler = rightHandler;
        }

        [HttpGet]
        [Route("rights")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> GetAllFilter([FromQuery] int? pageNumber, int? pageSize, string? textSearch)
        {
            RightQueryFilterRequest filter = new() { PageNumber = pageNumber, PageSize = pageSize, TextSearch = textSearch };
            Response<List<RightQueryResult>> response = await _rightHandler.GetAsync(filter);
            return Ok(response);
        }
    }
}
