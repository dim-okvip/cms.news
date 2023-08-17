namespace CMS.News.Business.Handlers
{
    public class MenuItemQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public Target Target { get; set; }
        public string FileUrl { get; set; }
        public int Order { get; set; }
        public bool Status { get; set; }
        public Guid ParentId { get; set; }
        public Guid MenuId { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<MenuItemQueryResult> ListChildMenuItem { get; set; } = new();
    }

    public class MenuItemQueryFilterRequest : QueryFilterRequest
    {
        public Guid? MenuId { get; set; } = null;
        public Guid? ParentId { get; set; } = null;
        public bool? IsIncludeChildMenuItem { get; set; } = null;
    }

    public class CreateMenuItemRequest
    {
        [Required]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public Target? Target { get; set; }

        public string FileUrl { get; set; }

        [Required]
        public int Order { get; set; }
        
        [Required]
        public bool? Status { get; set; }

        [Required]
        public Guid? ParentId { get; set; }
        
        [Required]
        public Guid? MenuId { get; set; }
    }

    public class UpdateMenuItemRequest : CreateMenuItemRequest
    {
        [Required]
        public Guid? Id { get; set; }
    }
}
