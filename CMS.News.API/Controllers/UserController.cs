namespace CMS.News.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtAuthenticationManager _jwtAuthenticationManager;
        private readonly IUserHandler _userHandler;

        public UserController(JwtAuthenticationManager jwtAuthenticationManager, IUserHandler userHandler)
        {
            _jwtAuthenticationManager = jwtAuthenticationManager;
            _userHandler = userHandler;
        }

        #region Read
        [HttpGet]
        [Route("users")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> GetAllFilter([FromQuery] int? pageNumber, int? pageSize,
            string? roleName, bool? isIncludeRole,
            string? textSearch,
            bool? status, bool? isAllowLoginMultiSession,
            Order? orderBy)
        {
            UserQueryFilterRequest filter = new()
            {
                PageNumber = pageNumber, PageSize = pageSize,
                RoleName = roleName, IsIncludeRole = isIncludeRole,
                TextSearch = textSearch,
                Status = status, IsAllowLoginMultiSession = isAllowLoginMultiSession,
                OrderBy = orderBy
            };
            Response<List<UserQueryResult>> response = await _userHandler.GetAllAsync(filter);
            return Ok(response);
        }

        [HttpGet]
        [Route("users/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.NOT_AUTHORIZATION)]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] bool? isIncludeRole)
        {
            UserQueryFilterRequest filter = new() { Id = id, IsIncludeRole = isIncludeRole };
            Response<List<UserQueryResult>> response = await _userHandler.GetAllAsync(filter);
            return Ok(response);
        }

        [HttpPost]
        [Route("users/login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            Response<JwtAuthResponse> response = await _jwtAuthenticationManager.AuthenticateAsync(user);
            return Ok(response);
        }
        #endregion

        #region CUD
        [HttpPost]
        [Route("users")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest createUserRequest)
        {
            Response<bool> response = await _userHandler.CreateAsync(createUserRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("users")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest upsertUserRequest)
        {
            Response<bool> response = await _userHandler.UpdateProfileAsync(upsertUserRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("users/updateRole")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleRequest updateRoleRequest)
        {
            Response<bool> response = await _userHandler.UpdateRoleAsync(updateRoleRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("users/updateStatus")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest updateStatusRequest)
        {
            Response<bool> response = await _userHandler.UpdateStatusAsync(updateStatusRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("users/updateStatusSessionLogin")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> UpdateStatusSessionLogin([FromBody] UpdateStatusRequest updateStatusRequest)
        {
            Response<bool> response = await _userHandler.UpdateStatusSessionLoginAsync(updateStatusRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("users/updatePassword")]
        [Role(Constants.CLAIM_TYPE, RightName.NOT_AUTHORIZATION)]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest updatePasswordRequest)
        {
            Response<bool> response = await _userHandler.UpdatePasswordAsync(updatePasswordRequest);
            return Ok(response);
        }

        [HttpPut]
        [Route("users/resetPassword")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            Response<bool> response = await _userHandler.ResetPasswordAsync(resetPasswordRequest);
            return Ok(response);
        }

        [HttpDelete]
        [Route("users/{id}")]
        [Role(Constants.CLAIM_TYPE, RightName.USER_MANAGEMENT)]
        public async Task<IActionResult> Delete(Guid id)
        {
            Response<bool> response = await _userHandler.DeleteAsync(id);
            return Ok(response);
        }
        #endregion
    }
}
