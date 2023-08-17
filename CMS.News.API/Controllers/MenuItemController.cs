namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemHandler _menuItemHandler;

        public MenuItemController(IMenuItemHandler menuItemHandler)
        {
            _menuItemHandler = menuItemHandler;
        }

        #region Read
        [HttpGet]
        [Route("menuItems")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> GetAllFilter([FromQuery] int? pageNumber, int? pageSize, string? textSearch, Guid? menuId, Guid? parentId, bool? isIncludeChildMenuItem, Order? orderBy)
        {
            MenuItemQueryFilterRequest filter = new() { PageNumber = pageNumber, PageSize = pageSize, TextSearch = textSearch, MenuId = menuId, ParentId = parentId, IsIncludeChildMenuItem = isIncludeChildMenuItem, OrderBy = orderBy };
            Response<List<MenuItemQueryResult>> response = await _menuItemHandler.GetAsync(filter);
            return Ok(response);
        }
        #endregion

        #region CUD
        [HttpPost]
        [Route("menuItems")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest request)
        {
            Response<bool> response = await _menuItemHandler.CreateAsync(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("menuItems")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> Update([FromBody] UpdateMenuItemRequest request)
        {
            Response<bool> response = await _menuItemHandler.UpdateAsync(request);
            return Ok(response);
        }

        [HttpDelete]
        [Route("menuItems/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.STORYLINE_MANAGEMENT)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<bool> response = await _menuItemHandler.DeleteAsync(id);
            return Ok(response);
        }
        #endregion
    }
}
