namespace CMS.News.Business.Handlers
{
    public class RoleQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public List<RightQueryResult> ListRight { get; set; } = new();
    }

    public class RoleQueryFilterRequest : QueryFilterRequest
    {
        public Guid? UserId { get; set; } = null;
        public Guid? SiteId { get; set; } = null;
        public bool? IsIncludeRight { get; set; } = null;
    }

    public class UpsertRoleRequest
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; } = String.Empty;

    }
}
