namespace CMS.News.Business.Handlers
{
    public interface ISiteHandler
    {
        Task<Response<List<SiteQueryResult>>> GetAsync(SiteQueryFilterRequest filter);
        Task<Response<bool>> CreateAsync(UpsertSiteRequest request);
        Task<Response<bool>> UpdateAsync(UpsertSiteRequest request);
        Task<Response<bool>> DeleteAsync(Guid id);
    }
}
