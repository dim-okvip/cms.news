namespace CMS.News.DAL.Entities
{
    public class RoleRight
    {
        public Guid RoleId { get; set; }
        public Guid RightId { get; set; }

        public Role Role { get; set; }
        public Right Right { get; set; }
    }
}
