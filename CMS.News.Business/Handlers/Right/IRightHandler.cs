namespace CMS.News.Business.Handlers
{
    public interface IRightHandler
    {
        Task<Response<List<RightQueryResult>>> GetAsync(RightQueryFilterRequest filterRequest);
        Task<Response<List<RightQueryResult>>> GetByRoleIdToAuthorizeAsync(string jwt, UserQueryResult logedInUser);
    }
}
