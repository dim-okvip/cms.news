namespace CMS.News.Business.Handlers
{
    public interface ITokenLoginHandler
    {
        Task<Response<List<TokenLogin>>> GetAllFromDatabaseAsync();
        Response<List<KeyValuePair<Guid, TokenLogin>>> GetAllFromMemory();
        Response<bool> IsTokenExist(string token);
        Task<Response<bool>> CreateAsync(Guid userId, string token);
        Task<Response<bool>> DeleteAsync(Guid userId);
    }
}
