namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class SiteController : ControllerBase
    {
        private readonly ISiteHandler _siteHandler;

        public SiteController(ISiteHandler siteHandler)
        {
            _siteHandler = siteHandler;
        }

        #region Read
        [HttpGet]
        [Route("sites")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> GetAllFilter([FromQuery] int? pageNumber, int? pageSize, string? textSearch, Order? orderBy)
        {
            SiteQueryFilterRequest filter = new() { PageNumber = pageNumber, PageSize = pageSize, TextSearch = textSearch, OrderBy = orderBy };
            Response<List<SiteQueryResult>> response = await _siteHandler.GetAsync(filter);
            return Ok(response);
        }
        #endregion

        #region CUD
        [HttpPost]
        [Route("sites")]
        [Role(Constants.CLAIM_TYPE, RightName.SITE_MANAGEMENT)]
        public async Task<IActionResult> Create([FromBody] UpsertSiteRequest upsertSiteRequest)
        {
            Response<bool> response = await _siteHandler.CreateAsync(upsertSiteRequest);
            return Ok(response);
        }
        
        [HttpPut]
        [Route("sites")]
        [Role(Constants.CLAIM_TYPE, RightName.SITE_MANAGEMENT)]
        public async Task<IActionResult> Update([FromBody] UpsertSiteRequest upsertSiteRequest)
        {
            Response<bool> response = await _siteHandler.UpdateAsync(upsertSiteRequest);
            return Ok(response);
        }
        
        [HttpDelete]
        [Route("sites/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.SITE_MANAGEMENT)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<bool> response = await _siteHandler.DeleteAsync(id);
            return Ok(response);
        }
        #endregion
    }
}
