namespace CMS.News.DAL.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public Target Target { get; set; }
        public string SmallFileUrl { get; set; }
        public string LargeFileUrl { get; set; }
        public bool Status { get; set; }
        public Guid ParentId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public ICollection<MenuMenuItem> MenuMenuItems { get; set; }
    }

    public enum Target
    {
        Blank,
        Self
    }
}
