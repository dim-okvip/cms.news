namespace CMS.News.DAL.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RoleRight> RoleRights { get; set; }
    }
}
