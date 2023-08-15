namespace CMS.News.Business.Handlers
{
    public class SiteQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public class SiteQueryFilterRequest : QueryFilterRequest
    {
    }

    public class UpsertSiteRequest
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        public string? Address { get; set; }
    }
}
