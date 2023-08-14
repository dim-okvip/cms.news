namespace CMS.News.DAL.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public Guid SiteId { get; set; }

        public User User { get; set; }
        public Role Role { get; set; }
        public Site Site { get; set; }
    }
}
