namespace CMS.News.Business.Handlers
{
    public interface IRoleHandler
    {
        Task<Response<List<RoleQueryResult>>> GetAsync(RoleQueryFilterRequest filter);
        Task<Response<bool>> CreateAsync(UpsertRoleRequest request);
        Task<Response<bool>> UpdateAsync(UpsertRoleRequest request);
        Task<Response<bool>> DeleteAsync(Guid id);
    }
}
