namespace CMS.News.Business.Handlers
{
    public interface IRoleRightHandler
    {
        Task<Response<bool>> UpdateAsync(UpdateRoleRightRequest request);
    }
}
