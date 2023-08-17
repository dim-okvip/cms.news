namespace CMS.News.Business.Handlers
{
    public interface IMenuItemHandler
    {
        Task<Response<List<MenuItemQueryResult>>> GetAsync(MenuItemQueryFilterRequest filter);
        Task<Response<bool>> CreateAsync(CreateMenuItemRequest request);
        Task<Response<bool>> UpdateAsync(UpdateMenuItemRequest request);
        Task<Response<bool>> DeleteAsync(Guid id);
    }
}
