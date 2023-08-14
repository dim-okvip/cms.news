namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class RoleRightController : ControllerBase
    {
        private readonly IRoleRightHandler _roleRightHandler;

        public RoleRightController(IRoleRightHandler roleRightHandler)
        {
            _roleRightHandler = roleRightHandler;
        }

        [HttpPut]
        [Route("roleRights")]
        [Role(Constants.CLAIM_TYPE, RightName.ROLE_MANAGEMENT)]
        public async Task<IActionResult> Update([FromBody] UpdateRoleRightRequest request)
        {
            Response<bool> response = await _roleRightHandler.UpdateAsync(request);
            return Ok(response);
        }
    }
}
