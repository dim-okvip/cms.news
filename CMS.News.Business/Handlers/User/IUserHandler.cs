namespace CMS.News.Business.Handlers
{
    public interface IUserHandler
    {
        public Task<Response<UserLoginResult>> LoginAsync(UserLoginRequest user);
        public Task<Response<List<UserQueryResult>>> GetAllAsync(UserQueryFilterRequest filter);
        public Task<Response<bool>> CreateAsync(CreateUserRequest request);
        public Task<Response<bool>> UpdateProfileAsync(UpdateProfileRequest request);
        public Task<Response<bool>> UpdateRoleAsync(UpdateRoleRequest request);
        public Task<Response<bool>> UpdateStatusAsync(UpdateStatusRequest request);
        public Task<Response<bool>> UpdateStatusSessionLoginAsync(UpdateStatusRequest request);
        public Task<Response<bool>> UpdatePasswordAsync(UpdatePasswordRequest request);
        public Task<Response<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        public Task<Response<bool>> DeleteAsync(Guid id);
    }
}
