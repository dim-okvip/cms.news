namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuHandler _menuHandler;

        public MenuController(IMenuHandler menuHandler)
        {
            _menuHandler = menuHandler;
        }

        #region Read
        [HttpGet]
        [Route("menus")]
        //[Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> GetAllFilter([FromQuery] int? pageNumber, int? pageSize, string? textSearch, Guid? siteId, Order? orderBy)
        {
            MenuQueryFilterRequest filter = new() { PageNumber = pageNumber, PageSize = pageSize, TextSearch = textSearch, SiteId = siteId, OrderBy = orderBy };
            Response<List<MenuQueryResult>> response = await _menuHandler.GetAsync(filter);
            return Ok(response);
        }
        #endregion

        #region CUD
        [HttpPost]
        [Route("menus")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> Create([FromBody] CreateMenuRequest createMenuRequest)
        {
            Response<bool> response = await _menuHandler.CreateAsync(createMenuRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("menus")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> Update([FromBody] UpdateMenuRequest updateMenuRequest)
        {
            Response<bool> response = await _menuHandler.UpdateAsync(updateMenuRequest);
            return Ok(response);
        }

        [HttpDelete]
        [Route("menus/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<bool> response = await _menuHandler.DeleteAsync(id);
            return Ok(response);
        }
        #endregion
    }
}
