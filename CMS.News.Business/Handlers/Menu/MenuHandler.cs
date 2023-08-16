namespace CMS.News.Business.Handlers
{
    public class MenuHandler : IMenuHandler
    {
        public Task<Response<bool>> CreateAsync(CreateMenuRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<Response<bool>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Response<List<MenuQueryResult>>> GetAsync(MenuQueryFilterRequest filter)
        {
            throw new NotImplementedException();
        }

        public Task<Response<bool>> UpdateAsync(UpdateMenuRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
