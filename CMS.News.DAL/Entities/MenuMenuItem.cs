namespace CMS.News.DAL.Entities
{
    public class MenuMenuItem
    {
        public Guid MenuId { get; set; }
        public Guid MenuItemId { get; set; }
        public int Order { get; set; }

        public Menu Menu { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
