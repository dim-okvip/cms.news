namespace CMS.News.Business.Handlers
{
    public interface IMenuHandler
    {
        Task<Response<List<MenuQueryResult>>> GetAsync(MenuQueryFilterRequest filter);
        Task<Response<bool>> CreateAsync(CreateMenuRequest request);
        Task<Response<bool>> UpdateAsync(UpdateMenuRequest request);
        Task<Response<bool>> DeleteAsync(Guid id);
    }
}
