namespace CMS.News.Business.Handlers
{
    public class MenuQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public Guid SiteId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public class MenuQueryFilterRequest : QueryFilterRequest
    {
        public Guid? SiteId { get; set; } = null;
    }

    public class CreateMenuRequest
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public bool Status { get; set; }
        
        public string Description { get; set; }

        [Required]
        public Guid? SiteId { get; set; }
    }
    
    public class UpdateMenuRequest : CreateMenuRequest
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
