namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleHandler _roleHandler;

        public RoleController(IRoleHandler roleHandler)
        {
            _roleHandler = roleHandler;
        }

        #region Read
        [HttpGet]
        [Route("roles")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> GetAllFilter([FromQuery] int? pageNumber, int? pageSize, string? textSearch, Order? orderBy, bool? isIncludeRight)
        {
            RoleQueryFilterRequest filter = new() { PageNumber = pageNumber, PageSize = pageSize, TextSearch = textSearch, OrderBy = orderBy, IsIncludeRight = isIncludeRight };
            Response<List<RoleQueryResult>> response = await _roleHandler.GetAsync(filter);
            return Ok(response);
        }

        [HttpGet]
        [Route("roles/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> GetById(Guid id, bool? isIncludeRight)
        {
            RoleQueryFilterRequest filter = new() { Id = id, IsIncludeRight = isIncludeRight };
            Response<List<RoleQueryResult>> response = await _roleHandler.GetAsync(filter);
            return Ok(response);
        }
        #endregion

        #region CUD
        [HttpPost]
        [Route("roles")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> Create([FromBody] UpsertRoleRequest upsertRoleRequest)
        {
            Response<bool> response = await _roleHandler.CreateAsync(upsertRoleRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("roles")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> Update([FromBody] UpsertRoleRequest upsertRoleRequest)
        {
            Response<bool> response = await _roleHandler.UpdateAsync(upsertRoleRequest);
            return Ok(response);
        }

        [HttpDelete]
        [Route("roles/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<bool> response = await _roleHandler.DeleteAsync(id);
            return Ok(response);
        }
        #endregion
    }
}
